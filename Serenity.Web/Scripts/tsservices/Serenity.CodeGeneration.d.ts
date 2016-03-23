declare namespace Serenity.CodeGeneration {
    interface FormatterOptionInfo {
        Name: string;
        Type: string;
    }
    interface FormatterTypeInfo {
        Options: {
            [key: string]: FormatterOptionInfo;
        };
    }
    type FormatterTypes = {
        [key: string]: FormatterTypeInfo;
    };
    function stringifyNode(node: any): string;
    function parseFormatterTypes(sourceText: string): FormatterTypes;
    function parseSourceToJson(sourceText: string): string;
}
