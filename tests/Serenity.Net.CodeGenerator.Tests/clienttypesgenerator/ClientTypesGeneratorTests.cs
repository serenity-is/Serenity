using Serenity.CodeGenerator;

namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    const string root = "/root/";

    const string CorelibPath = "node_modules/@serenity-is/corelib/index.d.ts";

    const string Corelib = /*lang=typescript*/ """
        export class Widget<T = any> {
            protected static registerEditor<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): EditorTypeInfo<TypeName>;
            protected static registerClass<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName>;
        }
        export class StringEditor<P = any> extends Widget<P> {
        }
        export class FormatterBase {
        }
        export class LookupEditorBase<TOptions = any, TItem = any> extends Widget<TOptions> {
        }
        export class ServiceLookupEditorBase<TOptions = any, TItem = any> extends Widget<TOptions> {
        }
        export interface Select2CommonOptions {
            allowClear?: boolean;
        }
        export interface LookupEditorOptions extends Select2CommonOptions {
            lookupKey?: string;
            async?: boolean;
        }
        export interface ServiceLookupEditorOptions {
            service?: string;
            idField?: string;
        }
        export interface TextAreaEditorOptions {
            rows: number;
            cols: number;
        }
        export class TextAreaEditor extends Widget<TextAreaEditorOptions> {
            constructor(opt?: WidgetProps<TextAreaEditorOptions>) {
            }
        }
        export interface TransformInclude<TypeName = ""> {
        }
        export interface Formatter {
        }
        export interface EditorTypeInfo<TypeName = any> {
        }
        export interface FormatterTypeInfo {
        }
        export interface ClassTypeInfo<TypeName = any> {
        }
        export interface IInitializeColumn {
        }
        export interface IStringValue {
        }
        export interface IReadOnly {
        }
        export interface WidgetProps<P = any> {
        }
        export interface EditorProps<P = any> {
        }
        export function formatterTypeInfo(ns?: any, attr?: any[]): FormatterTypeInfo;
        export function registerType(type: any): void;
        export const Decorators: {
            registerEditor(typeName: string, intfAndAttr?: any[]): any;
            registerFormatter(typeName: string, intfAndAttr?: any[]): any;
            registerClass(typeName: string, intfAndAttr?: any[]): any;
            option(): any;
        };
        """;

    private static List<GeneratedSource> Generate(string? nullableProp, bool omitComments,
        params (string path, string content)[] files)
    {
        return Generate(nullableProp, omitComments, false, files);
    }

    private static List<GeneratedSource> Generate(string? nullableProp, bool omitComments,
        bool fileScopedNamespaces, params (string path, string content)[] files)
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);

        void addFile(string path, string content)
        {
            var full = root + path;
            fileSystem.CreateDirectory(fileSystem.GetDirectoryName(full)!);
            fileSystem.WriteAllText(full, content.ReplaceLineEndings());
        }

        addFile(CorelibPath, Corelib);
        foreach (var (path, content) in files)
            addFile(path, content);

        var typeLister = new TSTypeListerAST(fileSystem, tsConfigDir: root, tsConfig: new TSConfig
        {
            CompilerOptions = new TSConfig.CompilerConfig
            {
                Module = "ES2015"
            }
        });

        foreach (var (path, _) in files)
            typeLister.AddInputFile(root + path);

        var generator = new ClientTypesGenerator
        {
            OmitComments = omitComments,
            NullableProp = nullableProp,
            FileScopedNamespaces = fileScopedNamespaces
        };
        generator.RootNamespaces.Add("MyProject");

        foreach (var type in typeLister.ExtractTypes())
            generator.AddTSType(type);

        return generator.Run();
    }

    private static string Read(List<GeneratedSource> files, string name)
    {
        return Assert.Single(files, x => x.Filename == name).Text;
    }
}
