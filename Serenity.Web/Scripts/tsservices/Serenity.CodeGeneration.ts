namespace Serenity.CodeGeneration {

    export interface FormatterOptionInfo {
        Name: string;
        Type: string;
    }

    export interface FormatterTypeInfo {
        Options: { [key: string]: FormatterOptionInfo };
    }

    export type FormatterTypes = { [key: string]: FormatterTypeInfo };

    function extractFormatterTypes(sourceFile: ts.SourceFile): FormatterTypes {

        var result: FormatterTypes = {};

        function getParents(node: ts.Node) {
            let parents: ts.Node[] = [];

            if (!node)
                return parents;

            while (node = node.parent) {
                parents.push(node);
            }

            return parents.reverse();
        }

        function getNamespace(node: ts.Node): string {
            let s = "";
            for (let parent of getParents(node)) {
                if (parent.kind == ts.SyntaxKind.ModuleDeclaration) {
                    if (s.length > 0)
                        s += ".";

                    s += (parent as ts.ModuleDeclaration).name.getText();
                }
            }
            return s;
        }

        function prependNamespace(s: string, node: ts.Node): string {
            var ns = getNamespace(node);
            if (ns.length)
                return ns + "." + s;
            return s;
        }

        function cloneDictionary<TValue>(obj: { [key: string]: TValue }) {
            if (!obj)
                return obj;

            let copy: { [key: string]: TValue } = {};

            for (var attr in obj) {
                if (obj.hasOwnProperty(attr))
                    copy[attr] = obj[attr];
            }

            return copy;
        }

        function isFormatter(node: ts.ClassDeclaration, imports: { [key: string]: string }): boolean {
            for (let heritage of node.heritageClauses) {
                if (heritage.token == ts.SyntaxKind.ImplementsKeyword &&
                    heritage.types != null) {

                    for (let type of heritage.types) {
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

        function visitNode(node: ts.Node, imports: { [key: string]: string) {
            switch (node.kind) {
                case ts.SyntaxKind.ImportEqualsDeclaration:
                    var ied = <ts.ImportEqualsDeclaration>node;
                    imports[ied.name.getText()] = ied.moduleReference.getText();
                    break;

                case ts.SyntaxKind.ClassDeclaration:
                    imports = cloneDictionary(imports);
                    let declaration = node as ts.ClassDeclaration;
                    if (isFormatter(declaration, imports)) {
                        var name = prependNamespace(declaration.name.getText(), declaration);
                        result[name] = {
                            Options: {
                            }
                        };
                    }

                    break;
            }

            ts.forEachChild(node, child => visitNode(child, imports));
        }

        visitNode(sourceFile, {});

        return result;
    }

    export function parseFormatterTypes(sourceText: string): FormatterTypes {
        let sourceFile = ts.createSourceFile("dummy.ts", sourceText,
            ts.ScriptTarget.ES6, /*setParentNodes */ true);

        //JSON.stringify(sourceFile, function(key, value) {
        //    if (key == "parent")
        //        return undefined;

        //    if (key == "kind")
        //        return ts.SyntaxKind[value];

        //    return value;
        //}, "    ");

        return extractFormatterTypes(sourceFile);
    }
}