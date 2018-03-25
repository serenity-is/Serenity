declare namespace ts {
    interface Node {
        $imports?: Serenity.CodeGeneration.Imports;
    }
}
declare namespace Serenity.CodeGeneration {
    type Imports = {
        [key: string]: string;
    };
    interface ExternalType {
        AssemblyName?: string;
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
        Origin?: ExternalTypeOrigin;
    }
    interface ExternalMember {
        Name?: string;
        Type?: string;
        Attributes?: ExternalAttribute[];
        IsDeclaration?: boolean;
        IsNullable?: boolean;
        IsProtected?: boolean;
        IsStatic?: boolean;
    }
    interface ExternalMethod extends ExternalMember {
        Arguments?: ExternalArgument[];
        IsConstructor?: boolean;
        IsOverride?: boolean;
        IsGetter?: boolean;
        IsSetter?: boolean;
    }
    interface ExternalProperty extends ExternalMember {
        GetMethod?: string;
        SetMethod?: string;
    }
    interface ExternalAttribute {
        Type?: string;
        Arguments?: ExternalArgument[];
    }
    interface ExternalArgument {
        Type?: string;
        Value?: any;
        Name?: string;
        IsOptional?: boolean;
        HasDefault?: boolean;
    }
    interface ExternalGenericParameter {
        Name?: string;
    }
    const enum ExternalTypeOrigin {
        Server = 1,
        SS = 2,
        TS = 3,
    }
    function stringifyNode(node: any): string;
    function parseSourceToJson(sourceText: string): string;
    function addSourceFile(fileName: string, body: string): void;
    function parseTypes(): any[];
}
