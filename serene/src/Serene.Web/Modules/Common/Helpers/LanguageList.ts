import { LanguageRow } from "@/ServerTypes/Administration/LanguageRow";

export function siteLanguageList() {
    var result: string[][] = [];
    for (var k of (LanguageRow as any).getLookup().items) {
        if (k.LanguageId !== 'en') {
            result.push([k.Id.toString(), k.LanguageName]);
        }
    }
    return result;
}
