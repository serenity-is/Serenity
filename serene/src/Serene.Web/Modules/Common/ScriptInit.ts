import { Config, ErrorHandling, HtmlContentEditor, TranslationConfig, getjQuery } from "@serenity-is/corelib";
import { gridDefaults } from "@serenity-is/sleekgrid";
import { getLanguageList } from "./Helpers/LanguageList";

Config.rootNamespaces.push('Serene');
TranslationConfig.getLanguageList = getLanguageList;
HtmlContentEditor.CKEditorBasePath = "~/Serenity.Assets/Scripts/ckeditor/";
gridDefaults.useCssVars = false;

let $ = getjQuery();
if ($?.fn?.colorbox) {
    $.fn.colorbox.settings.maxWidth = "95%";
    $.fn.colorbox.settings.maxHeight = "95%";
}

window.onerror = ErrorHandling.runtimeErrorHandler;
window.addEventListener('unhandledrejection', ErrorHandling.unhandledRejectionHandler);