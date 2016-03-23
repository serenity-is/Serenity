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
        function isFormatter(node, imports) {
            for (var _i = 0, _a = node.heritageClauses; _i < _a.length; _i++) {
                var heritage = _a[_i];
                if (heritage.token == ts.SyntaxKind.ImplementsKeyword &&
                    heritage.types != null) {
                    for (var _b = 0, _c = heritage.types; _b < _c.length; _b++) {
                        var type = _c[_b];
                        if (type.typeArguments == null ||
                            !type.typeArguments.length) {
                            var expression = type.expression.getText();
                            var parts = expression.split(".");
                            if (parts.length > 1) {
                                var resolved = imports[parts[0]];
                                if (resolved) {
                                    parts[0] = resolved;
                                    expression = parts.join(".");
                                }
                            }
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
        function extractFormatterTypes(sourceFile) {
            var result = {};
            function visitNode(node, imports) {
                switch (node.kind) {
                    case ts.SyntaxKind.ImportEqualsDeclaration:
                        var ied = node;
                        imports[ied.name.getText()] = ied.moduleReference.getText();
                        break;
                    case ts.SyntaxKind.ClassDeclaration:
                        imports = cloneDictionary(imports);
                        var klass = node;
                        if (!isUnderAmbientNamespace(node) &&
                            hasExportModifier(node) &&
                            isFormatter(klass, imports)) {
                            var name = prependNamespace(klass.name.getText(), klass);
                            result[name] = {
                                Options: {}
                            };
                        }
                        break;
                }
                ts.forEachChild(node, function (child) { return visitNode(child, imports); });
            }
            visitNode(sourceFile, {});
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
            return ts.createSourceFile("dummy.ts", sourceText, ts.ScriptTarget.ES5, /*setParentNodes */ true);
        }
        function parseFormatterTypes(sourceText) {
            return extractFormatterTypes(parseSourceFile(sourceText));
        }
        CodeGeneration.parseFormatterTypes = parseFormatterTypes;
        function parseSourceToJson(sourceText) {
            return stringifyNode(parseSourceFile(sourceText));
        }
        CodeGeneration.parseSourceToJson = parseSourceToJson;
    })(CodeGeneration = Serenity.CodeGeneration || (Serenity.CodeGeneration = {}));
})(Serenity || (Serenity = {}));
