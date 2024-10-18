import { LanguageRow } from "../../Administration";

export function siteLanguageList() {
    var result: string[][] = [];
    for (var k of LanguageRow.getLookup().items) {
        if (k.LanguageId !== 'en') {
            result.push([k.Id.toString(), k.LanguageName]);
        }
    }
    return result;
}
