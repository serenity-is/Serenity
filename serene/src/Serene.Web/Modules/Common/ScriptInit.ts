import { Config, ErrorHandling, HtmlContentEditor, TranslationConfig } from "@serenity-is/corelib";
import { gridDefaults } from "@serenity-is/sleekgrid";
import { getLanguageList } from "./Helpers/LanguageList";
import flatpickr from "flatpickr";
import "flatpickr/dist/l10n";

Config.rootNamespaces.push('Serene');
TranslationConfig.getLanguageList = getLanguageList;
HtmlContentEditor.CKEditorBasePath = "~/Serenity.Assets/Scripts/ckeditor/";
gridDefaults.useCssVars = false;

let culture = (document.documentElement?.lang || 'en').toLowerCase();
if (flatpickr.l10ns[culture]) {
    flatpickr.localize(flatpickr.l10ns[culture]);
} else {
    culture = culture.split('-')[0];
    flatpickr.l10ns[culture] && flatpickr.localize(flatpickr.l10ns[culture]);
}

window.onerror = ErrorHandling.runtimeErrorHandler;
window.addEventListener('unhandledrejection', ErrorHandling.unhandledRejectionHandler);
