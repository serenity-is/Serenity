declare namespace ts {
    interface Node {
        $imports?: Serenity.CodeGeneration.Imports;
    }
}

namespace Serenity.CodeGeneration {
    export type Imports = { [key: string]: string };

    export interface OptionInfo {
        Name: string;
        Type: string;
    }

    export type OptionInfos = { [key: string]: OptionInfo };
    
    export interface FormatterTypeInfo {
        Options: OptionInfos;
    }

    export type FormatterTypes = { [key: string]: FormatterTypeInfo };

    function any<T>(arr: T[], check: (item: T) => boolean): boolean {
        if (!arr || !arr.length)
            return false;

        for (let k of arr)
            if (check(k))
                return true;

        return false;
    }

    function first<T>(arr: T[], check: (item: T) => boolean): T {
        if (!arr || !arr.length)
            return null;

        for (let k of arr)
            if (check(k))
                return k;

        return null;
    }

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

    function isFormatter(node: ts.ClassDeclaration): boolean {
        for (let heritage of node.heritageClauses) {
            if (heritage.token == ts.SyntaxKind.ImplementsKeyword &&
                heritage.types != null) {

                for (let type of heritage.types) {
                    if (type.typeArguments == null ||
                        !type.typeArguments.length) {
                        var expression = type.expression.getText();

                        var parts = expression.split(".");
                        if (parts.length > 1 && node.$imports) {
                            var resolved = node.$imports[parts[0]];
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

    function isUnderAmbientNamespace(node: ts.Node): boolean {
        return any(getParents(node), x =>
            x.kind == ts.SyntaxKind.ModuleDeclaration &&
            any(x.modifiers, z => z.kind == ts.SyntaxKind.DeclareKeyword))
    }

    function hasExportModifier(node: ts.Node): boolean {
        return any(node.modifiers, x => x.kind == ts.SyntaxKind.ExportKeyword);
    }

    function isPrivateOrProtected(node: ts.Node): boolean {
        return !any(node.modifiers, x => x.kind == ts.SyntaxKind.PrivateKeyword ||
            x.kind == ts.SyntaxKind.ProtectedKeyword);
    }

    function isInterfaceOption(node: ts.TypeElement): boolean {
        return false;
    }

    function isClassOption(node: ts.TypeElement): boolean {
        return false;
    }

    function isOptionDecorator(decorator: ts.Decorator): boolean {
        if (decorator.expression == null)
            return false;

        let pae: ts.PropertyAccessExpression = null;
        if (decorator.expression.kind == ts.SyntaxKind.CallExpression) {
            let ce = decorator.expression as ts.CallExpression;

            if (ce.expression != null &&
                ce.expression.kind == ts.SyntaxKind.PropertyAccessExpression) {
                pae = ce.expression as ts.PropertyAccessExpression;
            }
        }
        else if (decorator.expression.kind == ts.SyntaxKind.PropertyAccessExpression) {
            pae = decorator.expression as ts.PropertyAccessExpression;
        }

        if (!pae)
            return;

        let expression = pae.getText();
        var parts = expression.split(".");
        if (parts.length > 1 && pae.$imports) {
            var resolved = pae.$imports[parts[0]];
            if (resolved) {
                parts[0] = resolved;
                expression = parts.join(".");
            }
        }

        return expression == "Serenity.Decorators.option";
    }

    function getOptions(sourceFile: ts.SourceFile, node: ts.Node): OptionInfos {
        let result : OptionInfos = {};

        function scanOptions(node: ts.Node, isOptions: boolean): void {
            let isInterface = node.kind == ts.SyntaxKind.InterfaceDeclaration;
            let isClass = node.kind == ts.SyntaxKind.ClassDeclaration;

            if (isInterface) {
                for (let member of (node as ts.InterfaceDeclaration).members) {

                    let name = member.name.getText();

                    if (result[name])
                        continue;

                    if (!isOptions && !any(member.decorators, isOptionDecorator))
                        continue;

                    result[name] = {
                        Name: name,
                        Type: "System.Object"
                    };
                }
            }
            else if (isClass) {
                for (let member of (node as ts.ClassDeclaration).members) {

                    let name = member.name.getText();

                    if (result[name])
                        continue;

                    if (!isOptions && !any(member.decorators, isOptionDecorator))
                        continue;

                    result[name] = {
                        Name: name,
                        Type: "System.Object"
                    };
                }
            }
            else
                return;
        }

        scanOptions(node, false);
        return result;
    }

    function setImports(sourceFile: ts.SourceFile) {
        function visitNode(node: ts.Node) {
            node.$imports = node.parent ? node.parent.$imports : {};

            switch (node.kind) {
                case ts.SyntaxKind.ImportEqualsDeclaration:
                    var ied = <ts.ImportEqualsDeclaration>node;
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

            ts.forEachChild(node, child => visitNode(child));
        }

        visitNode(sourceFile);
    }

    function extractFormatterTypes(sourceFile: ts.SourceFile): FormatterTypes {

        var result: FormatterTypes = {};

        function visitNode(node: ts.Node) {
            switch (node.kind) {
                case ts.SyntaxKind.ClassDeclaration:
                    let klass = node as ts.ClassDeclaration;

                    if (!isUnderAmbientNamespace(node) &&
                        hasExportModifier(node) &&
                        isFormatter(klass))
                    {
                        var name = prependNamespace(klass.name.getText(), klass);
                        result[name] = {
                            Options: getOptions(sourceFile, node)
                        }
                    }

                    break;
            }

            ts.forEachChild(node, child => visitNode(child));
        }

        visitNode(sourceFile);

        return result;
    }

    export function stringifyNode(node) {
        var id = 1;

        return JSON.stringify(node,
            function (key, value) {

                if (key == "kind")
                    return ts.SyntaxKind[value];

                if (Object.prototype.toString.apply(value) == "[object Object]") {
                    if (!value.$id && value.kind) {
                        value.$id = (id++).toString();
                        var replacement = {
                            $id: value.$id
                        }
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
                        }
                    }
                }

                return value;

            }, "    ");
    }

    function parseSourceFile(sourceText: string): ts.SourceFile {
        var sourceFile = ts.createSourceFile("dummy.ts", sourceText,
            ts.ScriptTarget.ES5, /*setParentNodes */ true);

        setImports(sourceFile);
        return sourceFile;
    }

    export function parseFormatterTypes(sourceText: string): FormatterTypes {
        return extractFormatterTypes(parseSourceFile(sourceText));
    }

    export function parseSourceToJson(sourceText: string): string {
        return stringifyNode(parseSourceFile(sourceText));
    }
}