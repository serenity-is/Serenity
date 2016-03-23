declare namespace ts {
    interface Node {
        $imports?: Serenity.CodeGeneration.Imports;
    }
}
declare namespace Serenity.CodeGeneration {
    type Imports = {
        [key: string]: string;
    };
    interface OptionInfo {
        Name: string;
        Type: string;
    }
    type OptionInfos = {
        [key: string]: OptionInfo;
    };
    interface FormatterTypeInfo {
        Options: OptionInfos;
    }
    type FormatterTypes = {
        [key: string]: FormatterTypeInfo;
    };
    function stringifyNode(node: any): string;
    function parseFormatterTypes(sourceText: string): FormatterTypes;
    function parseSourceToJson(sourceText: string): string;
}
