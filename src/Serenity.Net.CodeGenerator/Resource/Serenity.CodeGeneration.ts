declare namespace ts {
    interface Node {
        $imports?: Serenity.CodeGeneration.Imports;
    }
}

namespace Serenity.CodeGeneration {
    let typeChecker: ts.TypeChecker;

    export type Imports = { [key: string]: string };

    export interface ExternalType {
        Namespace?: string;
        Name?: string;
        BaseType?: string;
        Interfaces?: string[];
        Attributes?: ExternalAttribute[];
        Properties?: ExternalProperty[];
        Fields?: ExternalMember[];
        Methods?: ExternalMethod[];
        GenericParameters?: ExternalGenericParameter[];
        IsAbstract?: boolean;
        IsDeclaration?: boolean;
        IsInterface?: boolean;
        IsSealed?: boolean;
        IsSerializable?: boolean;
    }

    export interface ExternalMember {
        Name?: string;
        Type?: string;
        Attributes?: ExternalAttribute[];
        IsDeclaration?: boolean;
        IsNullable?: boolean;
        IsProtected?: boolean;
        IsStatic?: boolean;
    }

    export interface ExternalMethod extends ExternalMember {
        Arguments?: ExternalArgument[];
        IsConstructor?: boolean;
        IsOverride?: boolean;
        IsGetter?: boolean;
        IsSetter?: boolean;
    }

    export interface ExternalProperty extends ExternalMember {
        GetMethod?: string;
        SetMethod?: string;
    }

    export interface ExternalAttribute {
        Type?: string;
        Arguments?: ExternalArgument[];
    }

    export interface ExternalArgument {
        Type?: string;
        Value?: any;
        Name?: string;
        IsOptional?: boolean;
        HasDefault?: boolean;
    }

    export interface ExternalGenericParameter {
        Name?: string;
    }

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

    function getBaseType(node: ts.ClassDeclaration): string {
        if (!node.heritageClauses)
            return null;

        for (let heritage of node.heritageClauses) {
            if (heritage.token == ts.SyntaxKind.ExtendsKeyword &&
                heritage.types != null) {

                for (let type of heritage.types) {
                    return getExpandedExpression(type);
                }
            }
        }
    }

    function getInterfaces(node: ts.ClassDeclaration): string[] {
        var result: string[] = [];
        if (!node.heritageClauses)
            return result;

        for (let heritage of node.heritageClauses) {
            if (heritage.token == ts.SyntaxKind.ImplementsKeyword &&
                heritage.types != null) {

                for (let type of heritage.types) {
                    result.push(getExpandedExpression(type));
                }
            }
        }

        return result;
    }

    function getBaseInterfaces(node: ts.InterfaceDeclaration): string[] {
        var result: string[] = [];
        if (!node.heritageClauses)
            return result;

        for (let heritage of node.heritageClauses) {
            if (heritage.token == ts.SyntaxKind.ExtendsKeyword &&
                heritage.types != null) {

                for (let type of heritage.types) {
                    result.push(getExpandedExpression(type));
                }
            }
        }

        return result;
    }

    function isUnderAmbientNamespace(node: ts.Node): boolean {
        return any(getParents(node), x =>
            (x.kind == ts.SyntaxKind.ModuleDeclaration &&
                any(x.modifiers, z => z.kind == ts.SyntaxKind.DeclareKeyword)) ||
            (x.kind == ts.SyntaxKind.SourceFile &&
                (x as ts.SourceFile).isDeclarationFile));
    }

    function hasExportModifier(node: ts.Node): boolean {
        return any(node.modifiers, x => x.kind == ts.SyntaxKind.ExportKeyword);
    }

    function hasDeclareModifier(node: ts.Node): boolean {
        return any(node.modifiers, x => x.kind == ts.SyntaxKind.DeclareKeyword);
    }

    function getExpandedExpression(node: ts.Node) {
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

    function decoratorToExternalAttribute(decorator: ts.Decorator): ExternalAttribute {
        let result: ExternalAttribute = {
            Type: "",
            Arguments: []
        };

        if (decorator.expression == null)
            return null;

        let pae: ts.PropertyAccessExpression = null;
        if (decorator.expression.kind == ts.SyntaxKind.CallExpression) {
            let ce = decorator.expression as ts.CallExpression;

            if (ce.expression != null &&
                ce.expression.kind == ts.SyntaxKind.PropertyAccessExpression) {
                pae = ce.expression as ts.PropertyAccessExpression;
                result.Type = getExpandedExpression(pae);
            }

            for (let arg of ce.arguments) {
                switch (arg.kind) {
                    case ts.SyntaxKind.StringLiteral:
                        result.Arguments.push({
                            Value: (arg as ts.StringLiteral).text
                        });
                        break;
                    case ts.SyntaxKind.NumericLiteral:
                        result.Arguments.push({
                            Value: parseFloat((arg as ts.LiteralExpression).text)
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
            pae = decorator.expression as ts.PropertyAccessExpression;
            result.Type = getExpandedExpression(pae);
        }

        return result;
    }

    function getInterfaceMembers(node: ts.InterfaceDeclaration): ExternalMember[] {
        let result: ExternalMember[] = [];

        for (let member of node.members) {

            if (member.kind != ts.SyntaxKind.PropertySignature &&
                member.kind != ts.SyntaxKind.MethodSignature)
                continue;

            let name = member.name.getText();
            if (result[name])
                continue;

            var externalMember: ExternalMember = {
                Name: name,
                Type: "",
                Attributes: member.decorators == null ? [] :
                    member.decorators.map(decoratorToExternalAttribute)
            };

            if (member.kind == ts.SyntaxKind.PropertySignature) {
                let pd = (member as ts.PropertySignature);
                if (pd.type)
                    externalMember.Type = getExpandedExpression(pd.type);
            }
            else if (member.kind == ts.SyntaxKind.MethodSignature) {
                let emo = externalMember as ExternalMethod;
                emo.Arguments = [];
                let md = (member as ts.MethodSignature);
                if (md.type) {
                    externalMember.Type = getExpandedExpression(md.type);
                }

                for (var arg of md.parameters) {
                    emo.Arguments.push({
                        Name: arg.name.getText(),
                        Type: getExpandedExpression(arg.type)
                    });
                }
            }

            result[name] = externalMember;
            result.push(externalMember);
        }
        return result;
    }

    function getClassMembers(node: ts.ClassDeclaration): ExternalMember[] {
        let result: ExternalMember[] = [];

        for (let member of node.members) {

            if (member.kind != ts.SyntaxKind.MethodDeclaration &&
                member.kind != ts.SyntaxKind.PropertyDeclaration &&
                member.kind != ts.SyntaxKind.Constructor)
                continue;

            let name = member.name ? member.name.getText() : "$ctor";

            if (result[name])
                continue;

            var externalMember: ExternalMember = {
                Name: name,
                IsStatic: any(member.modifiers, x => x.getText() == "static"),
                Type: "",
                Attributes: member.decorators == null ? [] :
                    member.decorators.map(decoratorToExternalAttribute)
            };

            if (member.kind == ts.SyntaxKind.PropertyDeclaration) {
                let pd = (member as ts.PropertyDeclaration);
                if (pd.type)
                    externalMember.Type = getExpandedExpression(pd.type);
            }
            else if (member.kind == ts.SyntaxKind.MethodDeclaration) {
                let emo = externalMember as ExternalMethod;
                emo.Arguments = [];
                let md = (member as ts.MethodDeclaration);
                if (md.type) {
                    externalMember.Type = getExpandedExpression(md.type);
                }

                for (var arg of md.parameters) {
                    emo.Arguments.push({
                        Name: arg.name.getText(),
                        Type: getExpandedExpression(arg.type)
                    });
                }
            }
            else if (member.kind == ts.SyntaxKind.Constructor) {
                let emo = externalMember as ExternalMethod;
                emo.Arguments = [];
                emo.IsConstructor = true;
                let md = (member as ts.ConstructorDeclaration);

                for (var arg of md.parameters) {
                    emo.Arguments.push({
                        Name: arg.name.getText(),
                        Type: getExpandedExpression(arg.type)
                    });
                }
            }

            result[name] = externalMember;
            result.push(externalMember);
        }

        return result;
    }

    function getModuleMembers(node: ts.ModuleDeclaration): ExternalMember[] {
        let result: ExternalMember[] = [];

        for (let member of node.body.getChildren()) {

            if (member.kind != ts.SyntaxKind.MethodDeclaration &&
                member.kind != ts.SyntaxKind.PropertyDeclaration)
                continue;

            let name = (member as any).name ? (member as any).name.getText() : "";
            if (!name || result[name])
                continue;

            var externalMember: ExternalMember = {
                Name: name,
                IsStatic: true,
                Type: "",
                Attributes: member.decorators == null ? [] :
                    member.decorators.map(decoratorToExternalAttribute)
            };

            if (member.kind == ts.SyntaxKind.PropertyDeclaration) {
                let pd = (member as ts.PropertyDeclaration);
                if (pd.type)
                    externalMember.Type = getExpandedExpression(pd.type);
            }
            else if (member.kind == ts.SyntaxKind.MethodDeclaration) {
                let emo = externalMember as ExternalMethod;
                emo.Arguments = [];
                let md = (member as ts.MethodDeclaration);
                if (md.type) {
                    externalMember.Type = getExpandedExpression(md.type);
                }

                for (var arg of md.parameters) {
                    emo.Arguments.push({
                        Name: arg.name.getText(),
                        Type: getExpandedExpression(arg.type)
                    });
                }
            }

            result[name] = externalMember;
            result.push(externalMember);
        }

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
                case ts.SyntaxKind.ModuleDeclaration:
                case ts.SyntaxKind.InterfaceDeclaration:
                    node.$imports = cloneDictionary(node.$imports);
                    break;
            }

            ts.forEachChild(node, child => visitNode(child));
        }

        visitNode(sourceFile);
    }

    function typeParametersToExternal(p: ts.NodeArray<ts.TypeParameterDeclaration>): ExternalArgument[] {
        if (p == null || p.length == 0)
            return;

        let result: ExternalArgument[] = [];

        for (var k of p)
            result.push({
                Name: k.getText()
            });

        return result;
    }

    function classToExternalType(klass: ts.ClassDeclaration): ExternalType {
        let result: ExternalType = {
            BaseType: getBaseType(klass),
            GenericParameters: typeParametersToExternal(klass.typeParameters),
            IsAbstract: any(klass.modifiers, x => x.getText() == "abstract"),
            IsSealed: false,
            IsSerializable: false,
            Properties: [],
            Namespace: getNamespace(klass),
            Name: klass.name.getText(),
            IsInterface: false,
            IsDeclaration: isUnderAmbientNamespace(klass)
        };

        var members = getClassMembers(klass);
        result.Fields = members.filter(x => (x as ExternalMethod).Arguments == null);
        result.Methods = members.filter(x => (x as ExternalMethod).Arguments != null);

        result.Interfaces = getInterfaces(klass);
        result.Attributes = klass.decorators == null ? <ExternalAttribute[]>[] :
            klass.decorators.map(x => decoratorToExternalAttribute(x));

        return result;
    }

    function interfaceToExternalType(intf: ts.InterfaceDeclaration): ExternalType {
        let result: ExternalType = {
            GenericParameters: typeParametersToExternal(intf.typeParameters),
            Properties: [],
            Namespace: getNamespace(intf),
            Name: intf.name.getText(),
            IsInterface: true,
            IsDeclaration: isUnderAmbientNamespace(intf)
        };

        var members = getInterfaceMembers(intf);
        result.Fields = members.filter(x => (x as ExternalMethod).Arguments == null);
        result.Methods = members.filter(x => (x as ExternalMethod).Arguments != null);

        result.Interfaces = getBaseInterfaces(intf);
        result.Attributes = intf.decorators == null ? <ExternalAttribute[]>[] :
            intf.decorators.map(x => decoratorToExternalAttribute(x));

        return result;
    }

    function moduleToExternalType(module: ts.ModuleDeclaration): ExternalType {
        let result: ExternalType = {
            Properties: [],
            Namespace: getNamespace(module),
            Name: module.name.getText(),
            IsInterface: true,
            IsDeclaration: isUnderAmbientNamespace(module)
        };

        var members = getModuleMembers(module);
        result.Fields = members.filter(x => (x as ExternalMethod).Arguments == null);
        result.Methods = members.filter(x => (x as ExternalMethod).Arguments != null);

        result.Interfaces = [];
        result.Attributes = [];

        return result;
    }

    function extractTypes(sourceFile: ts.SourceFile): ExternalType[] {

        let result: ExternalType[] = [];

        function visitNode(node: ts.Node) {
            if (!node)
                return;

            switch (node.kind) {
                case ts.SyntaxKind.ClassDeclaration:
                    let klass = node as ts.ClassDeclaration;

                    if (sourceFile.isDeclarationFile || hasExportModifier(node)) {
                        let name = prependNamespace(klass.name.getText(), klass);
                        let exportedType = classToExternalType(klass);
                        result[name] = exportedType;
                        result.push(exportedType);
                    }

                    return;

                case ts.SyntaxKind.InterfaceDeclaration:
                    let intf = node as ts.InterfaceDeclaration;

                    if (sourceFile.isDeclarationFile || hasExportModifier(node)) {
                        let name = prependNamespace(intf.name.getText(), intf);
                        let exportedType = interfaceToExternalType(intf);
                        result[name] = exportedType;
                        result.push(exportedType);
                    }
                    return;

                case ts.SyntaxKind.ModuleDeclaration:
                    let modul = node as ts.ModuleDeclaration;

                    if (sourceFile.isDeclarationFile || hasExportModifier(modul) ||
                        (!isUnderAmbientNamespace(modul) &&
                         !hasDeclareModifier(modul))) {
                        let name = prependNamespace(modul.name.getText(), modul);
                        let exportedType = moduleToExternalType(modul);
                        result[name] = exportedType;
                        result.push(exportedType);
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

    export function parseSourceToJson(sourceText: string): string {
        return stringifyNode(parseSourceFile(sourceText));
    }

    class MyCompilerHost implements ts.CompilerHost {
        files: { [fileName: string]: string } = {};
        filesLower: { [fileName: string]: string } = {};
        options: ts.CompilerOptions  = {
            target: ts.ScriptTarget.ES5,
            moduleKind: ts.ModuleKind.None
        };

        fileExists = (fileName: string) => {
            return !!(this.files[fileName] || this.filesLower[(fileName || "").toLowerCase()]);
        }

        getCurrentDirectory = () => "/";
        getDirectories = (path: string) => [];
        getDefaultLibFileName = _ => "/lib.d.ts";
        getCanonicalFileName = fileName => fileName.toLowerCase();
        useCaseSensitiveFileNames = () => false;
        getNewLine = () => "\r\n";
        readFile = (fileName: string) => {
            var content = this.files[fileName];
            if (content === undefined)
                return this.filesLower[(fileName || "").toLowerCase()];
            return content;
        }
        writeFile(fileName: string, data: string, writeByteOrderMark: boolean, onError?: (message: string) => void) {
        };
        getSourceFile(fileName: string, languageVersion: ts.ScriptTarget, onError?: (message: string) => void) {
            var sourceText = this.files[fileName];
            if (sourceText === undefined)
                sourceText = this.filesLower[(fileName || "").toLowerCase()];
            var src = sourceText !== undefined ? ts.createSourceFile(fileName, sourceText, languageVersion, true) : undefined;
            if (src != null && fileName.substr(-5).toLowerCase() == '.d.ts')
                src.isDeclarationFile = true;

            return src;
        }

        resolveModuleNames(moduleNames: string[], containingFile: string): ts.ResolvedModule[] {
            return moduleNames.map(moduleName => {
                // try to use standard resolution
                let result = ts.resolveModuleName(moduleName, containingFile, this.options,
                    { fileExists: this.fileExists, readFile: this.readFile });

                if (result.resolvedModule) {
                    return result.resolvedModule;
                }

                return undefined;
            });
        }
    }

    let host = new MyCompilerHost();

    export function addSourceFile(fileName: string, body: string) {
        host.files[fileName] = body;
        host.filesLower[(fileName || "").toLowerCase()] = body;
    }
    
    export function parseTypes(): any[] {
        try {
            var fileNames = Object.getOwnPropertyNames(host.files);
            var program = ts.createProgram(fileNames, host.options, host);
            typeChecker = program.getTypeChecker();

            let result: ExternalType[] = [];
            for (var fileName in host.files) {
                if (fileName == "/lib.d.ts")
                    continue;

                var types = extractTypes(program.getSourceFile(fileName));

                for (var k of types) {
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

}