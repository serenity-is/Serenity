import { Config, EntityDialog, ErrorHandling, HtmlContentEditor, getjQuery } from "@serenity-is/corelib";
import { gridDefaults } from "@serenity-is/sleekgrid";
import { siteLanguageList } from "./Helpers/LanguageList";

Config.rootNamespaces.push('Serene');
EntityDialog.defaultLanguageList = siteLanguageList;
HtmlContentEditor.CKEditorBasePath = "~/Serenity.Assets/Scripts/ckeditor/";
gridDefaults.useCssVars = false;

let $ = getjQuery();
if ($?.fn?.colorbox) {
    $.fn.colorbox.settings.maxWidth = "95%";
    $.fn.colorbox.settings.maxHeight = "95%";
}

window.onerror = ErrorHandling.runtimeErrorHandler;
window.addEventListener('unhandledrejection', ErrorHandling.unhandledRejectionHandler);