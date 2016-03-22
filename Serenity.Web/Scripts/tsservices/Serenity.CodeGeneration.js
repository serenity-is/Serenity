var Serenity;
(function (Serenity) {
    var CodeGeneration;
    (function (CodeGeneration) {
        function extractFormatterTypes(sourceFile) {
            var result = {};
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
            function visitNode(node, imports) {
                switch (node.kind) {
                    case ts.SyntaxKind.ImportEqualsDeclaration:
                        var ied = node;
                        imports[ied.name.getText()] = ied.moduleReference.getText();
                        break;
                    case ts.SyntaxKind.ClassDeclaration:
                        imports = cloneDictionary(imports);
                        var declaration = node;
                        if (isFormatter(declaration, imports)) {
                            var name = prependNamespace(declaration.name.getText(), declaration);
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
        function parseFormatterTypes(sourceText) {
            var sourceFile = ts.createSourceFile("dummy.ts", sourceText, ts.ScriptTarget.ES6, /*setParentNodes */ true);
            //JSON.stringify(sourceFile, function(key, value) {
            //    if (key == "parent")
            //        return undefined;
            //    if (key == "kind")
            //        return ts.SyntaxKind[value];
            //    return value;
            //}, "    ");
            return extractFormatterTypes(sourceFile);
        }
        CodeGeneration.parseFormatterTypes = parseFormatterTypes;
    })(CodeGeneration = Serenity.CodeGeneration || (Serenity.CodeGeneration = {}));
})(Serenity || (Serenity = {}));
