var Serenity;
(function (Serenity) {
    var CodeGeneration;
    (function (CodeGeneration) {
        var typeChecker;
        function any(arr, check) {
            if (!arr || !arr.length)
                return false;
            for (var _i = 0, arr_1 = arr; _i < arr_1.length; _i++) {
                var k = arr_1[_i];
                if (check(k))
                    return true;
            }
            return false;
        }
        function first(arr, check) {
            if (!arr || !arr.length)
                return null;
            for (var _i = 0, arr_2 = arr; _i < arr_2.length; _i++) {
                var k = arr_2[_i];
                if (check(k))
                    return k;
            }
            return null;
        }
        function getParents(node) {
            var parents = [];
            if (!node)
                return parents;
            while (node = node.parent) {
                parents.push(node);
            }
            return parents.reverse();
        }
        function getNamespace(node) {
            var s = "";
            for (var _i = 0, _a = getParents(node); _i < _a.length; _i++) {
                var parent_1 = _a[_i];
                if (parent_1.kind == ts.SyntaxKind.ModuleDeclaration) {
                    if (s.length > 0)
                        s += ".";
                    s += parent_1.name.getText();
                }
            }
            return s;
        }
        function prependNamespace(s, node) {
            var ns = getNamespace(node);
            if (ns.length)
                return ns + "." + s;
            return s;
        }
        function cloneDictionary(obj) {
            if (!obj)
                return obj;
            var copy = {};
            for (var attr in obj) {
                if (obj.hasOwnProperty(attr))
                    copy[attr] = obj[attr];
            }
            return copy;
        }
        function getBaseType(node) {
            if (!node.heritageClauses)
                return null;
            for (var _i = 0, _a = node.heritageClauses; _i < _a.length; _i++) {
                var heritage = _a[_i];
                if (heritage.token == ts.SyntaxKind.ExtendsKeyword &&
                    heritage.types != null) {
                    for (var _b = 0, _c = heritage.types; _b < _c.length; _b++) {
                        var type = _c[_b];
                        return getExpandedExpression(type);
                    }
                }
            }
        }
        function getInterfaces(node) {
            var result = [];
            if (!node.heritageClauses)
                return result;
            for (var _i = 0, _a = node.heritageClauses; _i < _a.length; _i++) {
                var heritage = _a[_i];
                if (heritage.token == ts.SyntaxKind.ImplementsKeyword &&
                    heritage.types != null) {
                    for (var _b = 0, _c = heritage.types; _b < _c.length; _b++) {
                        var type = _c[_b];
                        result.push(getExpandedExpression(type));
                    }
                }
            }
            return result;
        }
        function getBaseInterfaces(node) {
            var result = [];
            if (!node.heritageClauses)
                return result;
            for (var _i = 0, _a = node.heritageClauses; _i < _a.length; _i++) {
                var heritage = _a[_i];
                if (heritage.token == ts.SyntaxKind.ExtendsKeyword &&
                    heritage.types != null) {
                    for (var _b = 0, _c = heritage.types; _b < _c.length; _b++) {
                        var type = _c[_b];
                        result.push(getExpandedExpression(type));
                    }
                }
            }
            return result;
        }
        function isUnderAmbientNamespace(node) {
            return any(getParents(node), function (x) {
                return (x.kind == ts.SyntaxKind.ModuleDeclaration &&
                    any(x.modifiers, function (z) { return z.kind == ts.SyntaxKind.DeclareKeyword; })) ||
                    (x.kind == ts.SyntaxKind.SourceFile &&
                        x.isDeclarationFile);
            });
        }
        function hasExportModifier(node) {
            return any(node.modifiers, function (x) { return x.kind == ts.SyntaxKind.ExportKeyword; });
        }
        function hasDeclareModifier(node) {
            return any(node.modifiers, function (x) { return x.kind == ts.SyntaxKind.DeclareKeyword; });
        }
        function getExpandedExpression(node) {
            if (!node)
                return "";
            try {
                var type = typeChecker.getTypeAtLocation(node);
                return typeChecker.getFullyQualifiedName(type.getSymbol());
            }
            catch (e) {
                return node.getText();
            }
        }
        function decoratorToExternalAttribute(decorator) {
            var result = {
                Type: "",
                Arguments: []
            };
            if (decorator.expression == null)
                return null;
            var pae = null;
            if (decorator.expression.kind == ts.SyntaxKind.CallExpression) {
                var ce = decorator.expression;
                if (ce.expression != null &&
                    ce.expression.kind == ts.SyntaxKind.PropertyAccessExpression) {
                    pae = ce.expression;
                    result.Type = getExpandedExpression(pae);
                }
                for (var _i = 0, _a = ce.arguments; _i < _a.length; _i++) {
                    var arg = _a[_i];
                    switch (arg.kind) {
                        case ts.SyntaxKind.StringLiteral:
                            result.Arguments.push({
                                Value: arg.text
                            });
                            break;
                        case ts.SyntaxKind.NumericLiteral:
                            result.Arguments.push({
                                Value: parseFloat(arg.text)
                            });
                            break;
                        default:
                            result.Arguments.push({
                                Value: null
                            });
                            break;
                    }
                }
            }
            else if (decorator.expression.kind == ts.SyntaxKind.PropertyAccessExpression) {
                pae = decorator.expression;
                result.Type = getExpandedExpression(pae);
            }
            return result;
        }
        function getInterfaceMembers(node) {
            var result = [];
            for (var _i = 0, _a = node.members; _i < _a.length; _i++) {
                var member = _a[_i];
                if (member.kind != ts.SyntaxKind.PropertySignature &&
                    member.kind != ts.SyntaxKind.MethodSignature)
                    continue;
                var name_1 = member.name.getText();
                if (result[name_1])
                    continue;
                var externalMember = {
                    Name: name_1,
                    Type: "",
                    Attributes: member.decorators == null ? [] :
                        member.decorators.map(decoratorToExternalAttribute)
                };
                if (member.kind == ts.SyntaxKind.PropertySignature) {
                    var pd = member;
                    if (pd.type)
                        externalMember.Type = getExpandedExpression(pd.type);
                }
                else if (member.kind == ts.SyntaxKind.MethodSignature) {
                    var emo = externalMember;
                    emo.Arguments = [];
                    var md = member;
                    if (md.type) {
                        externalMember.Type = getExpandedExpression(md.type);
                    }
                    for (var _b = 0, _c = md.parameters; _b < _c.length; _b++) {
                        var arg = _c[_b];
                        emo.Arguments.push({
                            Name: arg.name.getText(),
                            Type: getExpandedExpression(arg.type)
                        });
                    }
                }
                result[name_1] = externalMember;
                result.push(externalMember);
            }
            return result;
        }
        function getClassMembers(node) {
            var result = [];
            for (var _i = 0, _a = node.members; _i < _a.length; _i++) {
                var member = _a[_i];
                if (member.kind != ts.SyntaxKind.MethodDeclaration &&
                    member.kind != ts.SyntaxKind.PropertyDeclaration &&
                    member.kind != ts.SyntaxKind.Constructor)
                    continue;
                var name_2 = member.name ? member.name.getText() : "$ctor";
                if (result[name_2])
                    continue;
                var externalMember = {
                    Name: name_2,
                    IsStatic: any(member.modifiers, function (x) { return x.getText() == "static"; }),
                    Type: "",
                    Attributes: member.decorators == null ? [] :
                        member.decorators.map(decoratorToExternalAttribute)
                };
                if (member.kind == ts.SyntaxKind.PropertyDeclaration) {
                    var pd = member;
                    if (pd.type)
                        externalMember.Type = getExpandedExpression(pd.type);
                }
                else if (member.kind == ts.SyntaxKind.MethodDeclaration) {
                    var emo = externalMember;
                    emo.Arguments = [];
                    var md = member;
                    if (md.type) {
                        externalMember.Type = getExpandedExpression(md.type);
                    }
                    for (var _b = 0, _c = md.parameters; _b < _c.length; _b++) {
                        var arg = _c[_b];
                        emo.Arguments.push({
                            Name: arg.name.getText(),
                            Type: getExpandedExpression(arg.type)
                        });
                    }
                }
                else if (member.kind == ts.SyntaxKind.Constructor) {
                    var emo = externalMember;
                    emo.Arguments = [];
                    emo.IsConstructor = true;
                    var md = member;
                    for (var _d = 0, _e = md.parameters; _d < _e.length; _d++) {
                        var arg = _e[_d];
                        emo.Arguments.push({
                            Name: arg.name.getText(),
                            Type: getExpandedExpression(arg.type)
                        });
                    }
                }
                result[name_2] = externalMember;
                result.push(externalMember);
            }
            return result;
        }
        function getModuleMembers(node) {
            var result = [];
            for (var _i = 0, _a = node.body.getChildren(); _i < _a.length; _i++) {
                var member = _a[_i];
                if (member.kind != ts.SyntaxKind.MethodDeclaration &&
                    member.kind != ts.SyntaxKind.PropertyDeclaration)
                    continue;
                var name_3 = member.name ? member.name.getText() : "";
                if (!name_3 || result[name_3])
                    continue;
                var externalMember = {
                    Name: name_3,
                    IsStatic: true,
                    Type: "",
                    Attributes: member.decorators == null ? [] :
                        member.decorators.map(decoratorToExternalAttribute)
                };
                if (member.kind == ts.SyntaxKind.PropertyDeclaration) {
                    var pd = member;
                    if (pd.type)
                        externalMember.Type = getExpandedExpression(pd.type);
                }
                else if (member.kind == ts.SyntaxKind.MethodDeclaration) {
                    var emo = externalMember;
                    emo.Arguments = [];
                    var md = member;
                    if (md.type) {
                        externalMember.Type = getExpandedExpression(md.type);
                    }
                    for (var _b = 0, _c = md.parameters; _b < _c.length; _b++) {
                        var arg = _c[_b];
                        emo.Arguments.push({
                            Name: arg.name.getText(),
                            Type: getExpandedExpression(arg.type)
                        });
                    }
                }
                result[name_3] = externalMember;
                result.push(externalMember);
            }
            return result;
        }
        function setImports(sourceFile) {
            function visitNode(node) {
                node.$imports = node.parent ? node.parent.$imports : {};
                switch (node.kind) {
                    case ts.SyntaxKind.ImportEqualsDeclaration:
                        var ied = node;
                        node.$imports[ied.name.getText()] = ied.moduleReference.getText();
                        break;
                    case ts.SyntaxKind.ClassDeclaration:
                    case ts.SyntaxKind.ModuleDeclaration:
                    case ts.SyntaxKind.InterfaceDeclaration:
                        node.$imports = cloneDictionary(node.$imports);
                        break;
                }
                ts.forEachChild(node, function (child) { return visitNode(child); });
            }
            visitNode(sourceFile);
        }
        function typeParametersToExternal(p) {
            if (p == null || p.length == 0)
                return;
            var result = [];
            for (var _i = 0, p_1 = p; _i < p_1.length; _i++) {
                var k = p_1[_i];
                result.push({
                    Name: k.getText()
                });
            }
            return result;
        }
        function classToExternalType(klass) {
            var result = {
                BaseType: getBaseType(klass),
                GenericParameters: typeParametersToExternal(klass.typeParameters),
                IsAbstract: any(klass.modifiers, function (x) { return x.getText() == "abstract"; }),
                IsSealed: false,
                IsSerializable: false,
                Properties: [],
                Namespace: getNamespace(klass),
                Name: klass.name.getText(),
                IsInterface: false,
                IsDeclaration: isUnderAmbientNamespace(klass)
            };
            var members = getClassMembers(klass);
            result.Fields = members.filter(function (x) { return x.Arguments == null; });
            result.Methods = members.filter(function (x) { return x.Arguments != null; });
            result.Interfaces = getInterfaces(klass);
            result.Attributes = klass.decorators == null ? [] :
                klass.decorators.map(function (x) { return decoratorToExternalAttribute(x); });
            return result;
        }
        function interfaceToExternalType(intf) {
            var result = {
                GenericParameters: typeParametersToExternal(intf.typeParameters),
                Properties: [],
                Namespace: getNamespace(intf),
                Name: intf.name.getText(),
                IsInterface: true,
                IsDeclaration: isUnderAmbientNamespace(intf)
            };
            var members = getInterfaceMembers(intf);
            result.Fields = members.filter(function (x) { return x.Arguments == null; });
            result.Methods = members.filter(function (x) { return x.Arguments != null; });
            result.Interfaces = getBaseInterfaces(intf);
            result.Attributes = intf.decorators == null ? [] :
                intf.decorators.map(function (x) { return decoratorToExternalAttribute(x); });
            return result;
        }
        function moduleToExternalType(module) {
            var result = {
                Properties: [],
                Namespace: getNamespace(module),
                Name: module.name.getText(),
                IsInterface: true,
                IsDeclaration: isUnderAmbientNamespace(module)
            };
            var members = getModuleMembers(module);
            result.Fields = members.filter(function (x) { return x.Arguments == null; });
            result.Methods = members.filter(function (x) { return x.Arguments != null; });
            result.Interfaces = [];
            result.Attributes = [];
            return result;
        }
        function extractTypes(sourceFile) {
            var result = [];
            function visitNode(node) {
                if (!node)
                    return;
                switch (node.kind) {
                    case ts.SyntaxKind.ClassDeclaration:
                        var klass = node;
                        if (sourceFile.isDeclarationFile || hasExportModifier(node)) {
                            var name_4 = prependNamespace(klass.name.getText(), klass);
                            var exportedType = classToExternalType(klass);
                            result[name_4] = exportedType;
                            result.push(exportedType);
                        }
                        return;
                    case ts.SyntaxKind.InterfaceDeclaration:
                        var intf = node;
                        if (sourceFile.isDeclarationFile || hasExportModifier(node)) {
                            var name_5 = prependNamespace(intf.name.getText(), intf);
                            var exportedType = interfaceToExternalType(intf);
                            result[name_5] = exportedType;
                            result.push(exportedType);
                        }
                        return;
                    case ts.SyntaxKind.ModuleDeclaration:
                        var modul = node;
                        if (sourceFile.isDeclarationFile || hasExportModifier(modul) ||
                            (!isUnderAmbientNamespace(modul) &&
                                !hasDeclareModifier(modul))) {
                            var name_6 = prependNamespace(modul.name.getText(), modul);
                            var exportedType = moduleToExternalType(modul);
                            result[name_6] = exportedType;
                            result.push(exportedType);
                        }
                        break;
                }
                ts.forEachChild(node, function (child) { return visitNode(child); });
            }
            visitNode(sourceFile);
            return result;
        }
        function stringifyNode(node) {
            var id = 1;
            return JSON.stringify(node, function (key, value) {
                if (key == "kind")
                    return ts.SyntaxKind[value];
                if (Object.prototype.toString.apply(value) == "[object Object]") {
                    if (!value.$id && value.kind) {
                        value.$id = (id++).toString();
                        var replacement = {
                            $id: value.$id
                        };
                        for (var k in value) {
                            if (k != "$id" && Object.hasOwnProperty.call(value, k)) {
                                replacement[k] = value[k];
                            }
                        }
                        return replacement;
                    }
                    else if (value.$id && value.kind) {
                        return {
                            $ref: value.$id
                        };
                    }
                }
                return value;
            }, "    ");
        }
        CodeGeneration.stringifyNode = stringifyNode;
        function parseSourceFile(sourceText) {
            var sourceFile = ts.createSourceFile("dummy.ts", sourceText, ts.ScriptTarget.ES5, /*setParentNodes */ true);
            setImports(sourceFile);
            return sourceFile;
        }
        function parseSourceToJson(sourceText) {
            return stringifyNode(parseSourceFile(sourceText));
        }
        CodeGeneration.parseSourceToJson = parseSourceToJson;
        var MyCompilerHost = /** @class */ (function () {
            function MyCompilerHost() {
                var _this = this;
                this.files = {};
                this.filesLower = {};
                this.options = {
                    target: ts.ScriptTarget.ES5,
                    moduleKind: ts.ModuleKind.None
                };
                this.fileExists = function (fileName) {
                    return !!(_this.files[fileName] || _this.filesLower[(fileName || "").toLowerCase()]);
                };
                this.getCurrentDirectory = function () { return "/"; };
                this.getDirectories = function (path) { return []; };
                this.getDefaultLibFileName = function (_) { return "/lib.d.ts"; };
                this.getCanonicalFileName = function (fileName) { return fileName.toLowerCase(); };
                this.useCaseSensitiveFileNames = function () { return false; };
                this.getNewLine = function () { return "\r\n"; };
                this.readFile = function (fileName) {
                    var content = _this.files[fileName];
                    if (content === undefined)
                        return _this.filesLower[(fileName || "").toLowerCase()];
                    return content;
                };
            }
            MyCompilerHost.prototype.writeFile = function (fileName, data, writeByteOrderMark, onError) {
            };
            ;
            MyCompilerHost.prototype.getSourceFile = function (fileName, languageVersion, onError) {
                var sourceText = this.files[fileName];
                if (sourceText === undefined)
                    sourceText = this.filesLower[(fileName || "").toLowerCase()];
                var src = sourceText !== undefined ? ts.createSourceFile(fileName, sourceText, languageVersion, true) : undefined;
                if (src != null && fileName.substr(-5).toLowerCase() == '.d.ts')
                    src.isDeclarationFile = true;
                return src;
            };
            MyCompilerHost.prototype.resolveModuleNames = function (moduleNames, containingFile) {
                var _this = this;
                return moduleNames.map(function (moduleName) {
                    // try to use standard resolution
                    var result = ts.resolveModuleName(moduleName, containingFile, _this.options, { fileExists: _this.fileExists, readFile: _this.readFile });
                    if (result.resolvedModule) {
                        return result.resolvedModule;
                    }
                    return undefined;
                });
            };
            return MyCompilerHost;
        }());
        var host = new MyCompilerHost();
        function addSourceFile(fileName, body) {
            host.files[fileName] = body;
            host.filesLower[(fileName || "").toLowerCase()] = body;
        }
        CodeGeneration.addSourceFile = addSourceFile;
        function parseTypes() {
            try {
                var fileNames = Object.getOwnPropertyNames(host.files);
                var program = ts.createProgram(fileNames, host.options, host);
                typeChecker = program.getTypeChecker();
                var result = [];
                for (var fileName in host.files) {
                    if (fileName == "/lib.d.ts")
                        continue;
                    var types = extractTypes(program.getSourceFile(fileName));
                    for (var _i = 0, types_1 = types; _i < types_1.length; _i++) {
                        var k = types_1[_i];
                        var fullName = k.Namespace ? k.Namespace + "." + k.Name : k.Name;
                        if (result[fullName])
                            continue;
                        result[fullName] = k;
                        result.push(k);
                    }
                }
                return result;
            }
            catch (e) {
                throw new Error(e.toString() + e.stack);
            }
        }
        CodeGeneration.parseTypes = parseTypes;
    })(CodeGeneration = Serenity.CodeGeneration || (Serenity.CodeGeneration = {}));
})(Serenity || (Serenity = {}));
