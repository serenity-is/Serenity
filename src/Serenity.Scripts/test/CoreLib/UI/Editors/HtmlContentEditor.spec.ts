import { HtmlContentEditor } from "@CoreLib/UI/Editors/HtmlContentEditor";

test('getCKEditorBasePath returns ~/Scripts/ckeditor/ by default', function() {
    expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/Scripts/ckeditor/');
});

test('getCKEditorBasePath returns CKEDITOR_BASEPATH if set', function() {
    global["CKEDITOR_BASEPATH"] = "/ceb/";
    try {
        expect(HtmlContentEditor.getCKEditorBasePath()).toBe('/ceb/');
    }
    finally {
        delete global["CKEDITOR_BASEPATH"];        
    }
});

test('getCKEditorBasePath returns HtmlContentEditor.CKEditorBasePath if set', function() {
    HtmlContentEditor.CKEditorBasePath = '~/myck/';
    try {
        expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck/');
    }
    finally {
        delete HtmlContentEditor.CKEditorBasePath;
    }
});

test('getCKEditorBasePath gives priority to HtmlContentEditor.CKEditorBasePath', function() {
    HtmlContentEditor.CKEditorBasePath = '~/myck2/';
    global["CKEDITOR_BASEPATH"] = "/ceb/";
    try {
        expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck2/');
    }
    finally {
        delete HtmlContentEditor.CKEditorBasePath;
        delete global["CKEDITOR_BASEPATH"];
    }
});

test('getCKEditorBasePath adds trailing slash to result for global var', function() {
    global["CKEDITOR_BASEPATH"] = "/ceb";
    try {
        expect(HtmlContentEditor.getCKEditorBasePath()).toBe('/ceb/');
    }
    finally {
        delete global["CKEDITOR_BASEPATH"];
    }
});

test('getCKEditorBasePath adds trailing slash to result for static member', function() {
    HtmlContentEditor.CKEditorBasePath = '~/myck';
    try {
        expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck/');
    }
    finally {
        delete HtmlContentEditor.CKEditorBasePath;
    }
});