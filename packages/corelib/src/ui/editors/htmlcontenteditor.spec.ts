import { HtmlContentEditor } from "./htmlcontenteditor";

describe("HtmlContextEditor", () => {
    test('getCKEditorBasePath returns "~/Serenity.Assets/Scripts/ckeditor/" by default', function () {
        expect(HtmlContentEditor.getCKEditorBasePath()).toBe("~/Serenity.Assets/Scripts/ckeditor/");
    });

    test('getCKEditorBasePath returns CKEDITOR_BASEPATH if set', function () {
        (window as any)["CKEDITOR_BASEPATH"] = "/ceb/";
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('/ceb/');
        }
        finally {
            delete (window as any)["CKEDITOR_BASEPATH"];
        }
    });

    test('getCKEditorBasePath returns HtmlContentEditor.CKEditorBasePath if set', function () {
        HtmlContentEditor.CKEditorBasePath = '~/myck/';
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck/');
        }
        finally {
            delete HtmlContentEditor.CKEditorBasePath;
        }
    });

    test('getCKEditorBasePath gives priority to HtmlContentEditor.CKEditorBasePath', function () {
        HtmlContentEditor.CKEditorBasePath = '~/myck2/';
        (window as any)["CKEDITOR_BASEPATH"] = "/ceb/";
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck2/');
        }
        finally {
            delete HtmlContentEditor.CKEditorBasePath;
            delete (window as any)["CKEDITOR_BASEPATH"];
        }
    });

    test('getCKEditorBasePath adds trailing slash to result for global var', function () {
        (window as any)["CKEDITOR_BASEPATH"] = "/ceb";
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('/ceb/');
        }
        finally {
            delete (window as any)["CKEDITOR_BASEPATH"];
        }
    });

    test('getCKEditorBasePath adds trailing slash to result for static member', function () {
        HtmlContentEditor.CKEditorBasePath = '~/myck';
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck/');
        }
        finally {
            delete HtmlContentEditor.CKEditorBasePath;
        }
    });
});