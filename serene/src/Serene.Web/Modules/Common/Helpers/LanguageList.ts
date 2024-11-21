import { LanguageRow } from "../../ServerTypes/Administration/LanguageRow";

export function getLanguageList() {
    return ((LanguageRow as any).getLookup().items as (LanguageRow[]))
        .filter(x => x.LanguageId != "en")
        .map(k => ({ id: k.LanguageId, text: k.LanguageName }));
}