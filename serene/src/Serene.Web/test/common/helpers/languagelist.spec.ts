import { mockRowLookup } from "test-utils";
import { getLanguageList } from "../../../Modules/Common/Helpers/LanguageList";
import { LanguageRow } from "../../../Modules/ServerTypes/Administration/LanguageRow";

beforeEach(() => {
    mockRowLookup(LanguageRow, [
        { LanguageId: "en", LanguageName: "English" },
        { LanguageId: "tr", LanguageName: "Turkish" }
    ] as any);
});

describe("getLanguageList", () => {
    it("returns language list excluding english", () => {
        expect(getLanguageList()).toEqual([{ id: "tr", text: "Turkish" }]);
    });

    it("exposes the deprecated lookup accessor", () => {
        expect(LanguageRow.getLookup().items.length).toBe(2);
    });
});
