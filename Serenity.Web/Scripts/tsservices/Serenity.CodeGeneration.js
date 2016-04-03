var Serenity;
(function (Serenity) {
    var CodeGeneration;
    (function (CodeGeneration) {
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
        function isFormatter(node) {
            for (var _i = 0, _a = node.heritageClauses; _i < _a.length; _i++) {
                var heritage = _a[_i];
                if (heritage.token == ts.SyntaxKind.ImplementsKeyword &&
                    heritage.types != null) {
                    for (var _b = 0, _c = heritage.types; _b < _c.length; _b++) {
                        var type = _c[_b];
                        if (type.typeArguments == null ||
                            !type.typeArguments.length) {
                            var expression = getExpandedExpression(type);
                            if (expression == "Slick.Formatter") {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        function isUnderAmbientNamespace(node) {
            return any(getParents(node), function (x) {
                return x.kind == ts.SyntaxKind.ModuleDeclaration &&
                    any(x.modifiers, function (z) { return z.kind == ts.SyntaxKind.DeclareKeyword; });
            });
        }
        function hasExportModifier(node) {
            return any(node.modifiers, function (x) { return x.kind == ts.SyntaxKind.ExportKeyword; });
        }
        function isPrivateOrProtected(node) {
            return !any(node.modifiers, function (x) { return x.kind == ts.SyntaxKind.PrivateKeyword ||
                x.kind == ts.SyntaxKind.ProtectedKeyword; });
        }
        function isInterfaceOption(node) {
            return false;
        }
        function isClassOption(node) {
            return false;
        }
        function getExpandedExpression(node) {
            if (!node)
                return "";
            var expression = node.getText();
            var parts = expression.split(".");
            if (parts.length > 1 && node.$imports) {
                var resolved = node.$imports[parts[0]];
                if (resolved) {
                    parts[0] = resolved;
                    expression = parts.join(".");
                }
            }
            return expression;
        }
        function isOptionDecorator(decorator) {
            if (decorator.expression == null)
                return false;
            var pae = null;
            if (decorator.expression.kind == ts.SyntaxKind.CallExpression) {
                var ce = decorator.expression;
                if (ce.expression != null &&
                    ce.expression.kind == ts.SyntaxKind.PropertyAccessExpression) {
                    pae = ce.expression;
                }
            }
            else if (decorator.expression.kind == ts.SyntaxKind.PropertyAccessExpression) {
                pae = decorator.expression;
            }
            if (!pae)
                return;
            var expression = getExpandedExpression(pae);
            return expression == "Serenity.Decorators.option";
        }
        function getMembers(sourceFile, node) {
            var result = [];
            var isInterface = node.kind == ts.SyntaxKind.InterfaceDeclaration;
            var isClass = node.kind == ts.SyntaxKind.ClassDeclaration;
            if (isInterface) {
                for (var _i = 0, _a = node.members; _i < _a.length; _i++) {
                    var member = _a[_i];
                    var name_1 = member.name.getText();
                    if (result[name_1] != null)
                        continue;
                }
            }
            else if (isClass) {
                for (var _b = 0, _c = node.members; _b < _c.length; _b++) {
                    var member = _c[_b];
                    if (member.kind != ts.SyntaxKind.MethodDeclaration &&
                        member.kind != ts.SyntaxKind.PropertyDeclaration)
                        continue;
                    var name_2 = member.name.getText();
                    if (result[name_2])
                        continue;
                    var typeName = "";
                    if (member.kind == ts.SyntaxKind.PropertyDeclaration) {
                        var pd = member;
                        if (pd.type)
                            typeName = pd.type.getText();
                    }
                    result[name_2] = {
                        Name: name_2,
                        Type: typeName
                    };
                }
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
                        node.$imports = cloneDictionary(node.$imports);
                        break;
                    case ts.SyntaxKind.ModuleDeclaration:
                        node.$imports = cloneDictionary(node.$imports);
                        break;
                    case ts.SyntaxKind.InterfaceDeclaration:
                        node.$imports = cloneDictionary(node.$imports);
                        break;
                }
                ts.forEachChild(node, function (child) { return visitNode(child); });
            }
            visitNode(sourceFile);
        }
        function typeParameterstoExternal(p) {
            if (p == null || p.length == 0)
                return [];
            var result = [];
            for (var _i = 0, p_1 = p; _i < p_1.length; _i++) {
                var k = p_1[_i];
                result.push(k.getText());
            }
        }
        function classToExternalType(klass) {
            var result = {
                AssemblyName: "",
                Attributes: [],
                BaseType: getBaseType(klass),
                Fields: [],
                GenericParameters: typeParameterstoExternal(klass.typeParameters),
                IsAbstract: any(klass.modifiers, function (x) { return x.getText() == "abstract"; }),
                Interfaces: [],
                IsSealed: false,
                IsSerializable: false,
                Methods: [],
                Origin: 3 /* TS */,
                Properties: [],
                Namespace: getNamespace(klass),
                Name: klass.name.getText(),
                IsInterface: false,
                IsDeclaration: isUnderAmbientNamespace(klass)
            };
            return result;
        }
        function extractTypes(sourceFile) {
            var result = [];
            function visitNode(node) {
                switch (node.kind) {
                    case ts.SyntaxKind.ClassDeclaration:
                        var klass = node;
                        if (hasExportModifier(node)) {
                            var name = prependNamespace(klass.name.getText(), klass);
                            var exportedType = classToExternalType(klass);
                            result[name] = exportedType;
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
        function parseTypes(sourceText) {
            return extractTypes(parseSourceFile(sourceText));
        }
        CodeGeneration.parseTypes = parseTypes;
        function parseSourceToJson(sourceText) {
            return stringifyNode(parseSourceFile(sourceText));
        }
        CodeGeneration.parseSourceToJson = parseSourceToJson;
    })(CodeGeneration = Serenity.CodeGeneration || (Serenity.CodeGeneration = {}));
})(Serenity || (Serenity = {}));
