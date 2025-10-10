import { Config, ErrorHandling, TranslationConfig } from "@serenity-is/corelib";
import { gridDefaults } from "@serenity-is/sleekgrid";
import flatpickr from "flatpickr";
import "flatpickr/dist/l10n";
import { getLanguageList } from "./Helpers/LanguageList";
import DOMPurify from "dompurify";

Config.rootNamespaces.push('Serene');
TranslationConfig.getLanguageList = getLanguageList;
gridDefaults.sanitizer = (globalThis.DOMPurify = DOMPurify).sanitize;

let culture = (document.documentElement?.lang || 'en').toLowerCase();
if (flatpickr.l10ns[culture]) {
    flatpickr.localize(flatpickr.l10ns[culture]);
} else {
    culture = culture.split('-')[0];
    flatpickr.l10ns[culture] && flatpickr.localize(flatpickr.l10ns[culture]);
}

window.onerror = ErrorHandling.runtimeErrorHandler;
window.addEventListener('unhandledrejection', ErrorHandling.unhandledRejectionHandler);
