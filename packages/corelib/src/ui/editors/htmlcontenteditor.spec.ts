import { HtmlContentEditor } from "./htmlcontenteditor";

describe("HtmlContextEditor", () => {
    it('getCKEditorBasePath returns CDNJS url by default', function () {
        expect(HtmlContentEditor.getCKEditorBasePath()).toBe("https://cdnjs.cloudflare.com/ajax/libs/ckeditor/4.22.1/");
    });

    it('getCKEditorBasePath returns CKEDITOR_BASEPATH if set', function () {
        (window as any)["CKEDITOR_BASEPATH"] = "/ceb/";
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('/ceb/');
        }
        finally {
            delete (window as any)["CKEDITOR_BASEPATH"];
        }
    });

    it('getCKEditorBasePath returns HtmlContentEditor.CKEditorBasePath if set', function () {
        HtmlContentEditor.CKEditorBasePath = '~/myck/';
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck/');
        }
        finally {
            delete HtmlContentEditor.CKEditorBasePath;
        }
    });

    it('getCKEditorBasePath gives priority to HtmlContentEditor.CKEditorBasePath', function () {
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

    it('getCKEditorBasePath adds trailing slash to result for global var', function () {
        (window as any)["CKEDITOR_BASEPATH"] = "/ceb";
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('/ceb/');
        }
        finally {
            delete (window as any)["CKEDITOR_BASEPATH"];
        }
    });

    it('getCKEditorBasePath adds trailing slash to result for static member', function () {
        HtmlContentEditor.CKEditorBasePath = '~/myck';
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck/');
        }
        finally {
            delete HtmlContentEditor.CKEditorBasePath;
        }
    });
});