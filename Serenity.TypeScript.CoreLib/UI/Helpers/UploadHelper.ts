declare namespace Serenity {
    namespace UploadHelper {
        function addUploadInput(options: UploadInputOptions): JQuery;
        function checkImageConstraints(file: UploadResponse, opt: ImageUploadEditorOptions): boolean;
        function fileNameSizeDisplay(name: string, bytes: number): string;
        function fileSizeDisplay(bytes: number): string;
        function hasImageExtension(filename: string): boolean;
        function thumbFileName(filename: string): string;
        function dbFileUrl(filename: string): string;
        function colorBox(link: JQuery, options: any): void;
        function populateFileSymbols(container: JQuery, items: UploadedFile[], displayOriginalName?: boolean, urlPrefix?: string): void;
    }

    interface UploadedFile {
        Filename: string;
        OriginalName: string;
    }

    interface UploadInputOptions {
        container?: JQuery;
        progress?: JQuery;
        inputName?: string;
        allowMultiple?: boolean;
        fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
    }

    interface UploadResponse {
        TemporaryFile: string;
        Size: number;
        IsImage: boolean;
        Width: number;
        Height: number;
    }
}