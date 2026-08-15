import { describe, expect, it } from "vitest";
import { Select2 } from "./select2";

/**
 * Guards the exact behavior of Select2.stripDiacritics, which currently relies on a large DIACRITICS
 * lookup table. Several categories below are intentionally tested because a naive replacement with
 * String.prototype.normalize("NFD") (which only strips Unicode combining marks) would NOT handle them:
 * fullwidth forms, circled letters, barred/reversed letters, medieval ligatures, final sigma, etc.
 */
describe("Select2.stripDiacritics", () => {
    it("leaves ASCII text untouched", () => {
        expect(Select2.stripDiacritics("")).toBe("");
        expect(Select2.stripDiacritics("Hello World 12345")).toBe("Hello World 12345");
        expect(Select2.stripDiacritics("ABCdef")).toBe("ABCdef");
    });

    it.each([
        ["\u00E9", "e"], ["\u00E8", "e"], ["\u00EA", "e"], ["\u00EB", "e"],
        ["\u00E0", "a"], ["\u00E2", "a"], ["\u00E4", "a"], ["\u00E5", "a"], ["\u00C5", "A"],
        ["\u00F3", "o"], ["\u00F6", "o"], ["\u00F8", "o"],
        ["\u00FC", "u"], ["\u00F1", "n"], ["\u00E7", "c"],
        ["\u00C9", "E"], ["\u00C4", "A"], ["\u00D6", "O"], ["\u00DC", "U"]
    ])("strips common accented Latin letters (%s -> %s)", (input, expected) => {
        expect(Select2.stripDiacritics(input)).toBe(expected);
    });

    it("strips diacritics from words and phrases", () => {
        expect(Select2.stripDiacritics("caf\u00E9")).toBe("cafe");
        expect(Select2.stripDiacritics("Caf\u00E9")).toBe("Cafe");
        expect(Select2.stripDiacritics("H\u00E9llo, w\u00F3rld!")).toBe("Hello, world!");
        expect(Select2.stripDiacritics("\u0130stanbul")).toBe("Istanbul");
        expect(Select2.stripDiacritics("\u00C6sir")).toBe("AEsir");
    });

    it.each([
        ["\u00C6", "AE"], ["\u00E6", "ae"],
        ["\u00D8", "O"], ["\u00F8", "o"],
        ["\u00DF", "s"], ["\u1E9E", "S"],
        ["\u0110", "D"], ["\u0111", "d"],
        ["\u0141", "L"], ["\u0142", "l"],
        ["\u0126", "H"], ["\u0127", "h"],
        ["\u0166", "T"], ["\u0167", "t"],
        ["\u0131", "i"], ["\u017F", "l"]
    ])("transliterates special Latin letters NFD does not decompose (%s -> %s)", (input, expected) => {
        expect(Select2.stripDiacritics(input)).toBe(expected);
    });

    it.each([
        ["\uFF21", "A"], ["\uFF22", "B"], ["\uFF3A", "Z"], ["\uFF41", "a"], ["\uFF5A", "z"]
    ])("normalizes fullwidth forms (%s -> %s)", (input, expected) => {
        expect(Select2.stripDiacritics(input)).toBe(expected);
    });

    it.each([
        ["\u24B6", "A"], ["\u24D1", "b"], ["\u24E9", "z"]
    ])("normalizes circled alphanumerics (%s -> %s)", (input, expected) => {
        expect(Select2.stripDiacritics(input)).toBe(expected);
    });

    it.each([
        ["\u023A", "A"], ["\u0180", "b"], ["\u0183", "b"], ["\u023B", "C"], ["\u0250", "a"], ["\u0254", "o"]
    ])("transliterates barred and reversed letters (%s -> %s)", (input, expected) => {
        expect(Select2.stripDiacritics(input)).toBe(expected);
    });

    it.each([
        ["\uA732", "AA"], ["\uA734", "AO"], ["\uA736", "AU"], ["\uA738", "AV"], ["\uA73C", "AY"], ["\uA733", "aa"]
    ])("expands medieval Latin ligatures (%s -> %s)", (input, expected) => {
        expect(Select2.stripDiacritics(input)).toBe(expected);
    });

    it.each([
        ["\u01C4", "DZ"], ["\u01C6", "dz"], ["\u01C7", "LJ"], ["\u01C9", "lj"], ["\u01CA", "NJ"], ["\u01CC", "nj"]
    ])("expands uppercase and lowercase digraph ligatures (%s -> %s)", (input, expected) => {
        expect(Select2.stripDiacritics(input)).toBe(expected);
    });

    it.each([
        ["\u0386", "\u0391"], ["\u0388", "\u0395"], ["\u03AC", "\u03B1"], ["\u03AD", "\u03B5"], ["\u0390", "\u03B9"], ["\u03C2", "\u03C3"]
    ])("transliterates accented Greek to base letters (%s -> %s)", (input, expected) => {
        expect(Select2.stripDiacritics(input)).toBe(expected);
    });

    it("keeps non-ASCII characters not present in the table unchanged", () => {
        expect(Select2.stripDiacritics("\u03A9")).toBe("\u03A9");
        expect(Select2.stripDiacritics("\u03C9")).toBe("\u03C9");
        expect(Select2.stripDiacritics("\u4E2D")).toBe("\u4E2D");
        expect(Select2.stripDiacritics("\u20AC")).toBe("\u20AC");
    });

    it("matches case-insensitively through the default matcher", () => {
        expect(Select2.defaults.matcher!("caf", "Caf\u00E9", undefined)).toBe(true);
        expect(Select2.defaults.matcher!("gr\u00FC\u00DFe", "GRU\u00DFE", undefined)).toBe(true);
    });

    // The blocks below pin the exact output of the full DIACRITICS table for every remaining
    // character not covered above, grouped into strings per Unicode block.
    it("covers Latin Extended Additional (1/7) characters", () => {
        expect(Select2.stripDiacritics("\u1EA6\u1EA4\u1EAA\u1EA8\u1EB0\u1EAE\u1EB4\u1EB2\u1EA2\u1EA0\u1EAC\u1EB6\u1E00\u1E02\u1E04\u1E06\u1E08\u1E0A\u1E0C\u1E10\u1E12\u1E0E\u1EC0\u1EBE\u1EC4\u1EC2\u1EBC\u1E14\u1E16\u1EBA\u1EB8\u1EC6\u1E1C\u1E18\u1E1A\u1E1E\u1E20\u1E22\u1E26\u1E24")).toBe("AAAAAAAAAAAAABBBCDDDDDEEEEEEEEEEEEEFGHHH");
    });

    it("covers Latin Extended Additional (2/7) characters", () => {
        expect(Select2.stripDiacritics("\u1E28\u1E2A\u1E2E\u1EC8\u1ECA\u1E2C\u1E30\u1E32\u1E34\u1E36\u1E38\u1E3C\u1E3A\u1E3E\u1E40\u1E42\u1E44\u1E46\u1E4A\u1E48\u1ED2\u1ED0\u1ED6\u1ED4\u1E4C\u1E4E\u1E50\u1E52\u1ECE\u1EDC\u1EDA\u1EE0\u1EDE\u1EE2\u1ECC\u1ED8\u1E54\u1E56\u1E58\u1E5A")).toBe("HHIIIIKKKLLLLMMMNNNNOOOOOOOOOOOOOOOOPPRR");
    });

    it("covers Latin Extended Additional (3/7) characters", () => {
        expect(Select2.stripDiacritics("\u1E5C\u1E5E\u1E64\u1E60\u1E66\u1E62\u1E68\u1E6A\u1E6C\u1E70\u1E6E\u1E78\u1E7A\u1EE6\u1EEA\u1EE8\u1EEE\u1EEC\u1EF0\u1EE4\u1E72\u1E76\u1E74\u1E7C\u1E7E\u1E80\u1E82\u1E86\u1E84\u1E88\u1E8A\u1E8C\u1EF2\u1EF8\u1E8E\u1EF6\u1EF4\u1EFE\u1E90\u1E92")).toBe("RRSSSSSTTTTUUUUUUUUUUUUVVWWWWWXXYYYYYYZZ");
    });

    it("covers Latin Extended Additional (4/7) characters", () => {
        expect(Select2.stripDiacritics("\u1E94\u1E9A\u1EA7\u1EA5\u1EAB\u1EA9\u1EB1\u1EAF\u1EB5\u1EB3\u1EA3\u1EA1\u1EAD\u1EB7\u1E01\u1E03\u1E05\u1E07\u1E09\u1E0B\u1E0D\u1E11\u1E13\u1E0F\u1EC1\u1EBF\u1EC5\u1EC3\u1EBD\u1E15\u1E17\u1EBB\u1EB9\u1EC7\u1E1D\u1E19\u1E1B\u1E1F\u1E21\u1E23")).toBe("Zaaaaaaaaaaaaaabbbcdddddeeeeeeeeeeeeefgh");
    });

    it("covers Latin Extended Additional (5/7) characters", () => {
        expect(Select2.stripDiacritics("\u1E27\u1E25\u1E29\u1E2B\u1E96\u1E2F\u1EC9\u1ECB\u1E2D\u1E31\u1E33\u1E35\u1E37\u1E39\u1E3D\u1E3B\u1E3F\u1E41\u1E43\u1E45\u1E47\u1E4B\u1E49\u1ED3\u1ED1\u1ED7\u1ED5\u1E4D\u1E4F\u1E51\u1E53\u1ECF\u1EDD\u1EDB\u1EE1\u1EDF\u1EE3\u1ECD\u1ED9\u1E55")).toBe("hhhhhiiiikkkllllmmmnnnnoooooooooooooooop");
    });

    it("covers Latin Extended Additional (6/7) characters", () => {
        expect(Select2.stripDiacritics("\u1E57\u1E59\u1E5B\u1E5D\u1E5F\u1E65\u1E61\u1E67\u1E63\u1E69\u1E9B\u1E6B\u1E97\u1E6D\u1E71\u1E6F\u1E79\u1E7B\u1EE7\u1EEB\u1EE9\u1EEF\u1EED\u1EF1\u1EE5\u1E73\u1E77\u1E75\u1E7D\u1E7F\u1E81\u1E83\u1E87\u1E85\u1E98\u1E89\u1E8B\u1E8D\u1EF3\u1EF9")).toBe("prrrrsssssstttttuuuuuuuuuuuuvvwwwwwwxxyy");
    });

    it("covers Latin Extended Additional (7/7) characters", () => {
        expect(Select2.stripDiacritics("\u1E8F\u1EF7\u1E99\u1EF5\u1EFF\u1E91\u1E93\u1E95")).toBe("yyyyyzzz");
    });

    it("covers Latin Extended-B (1/4) characters", () => {
        expect(Select2.stripDiacritics("\u0226\u01E0\u01DE\u01FA\u01CD\u0200\u0202\u01FC\u01E2\u0243\u0182\u0181\u0187\u018B\u018A\u0189\u01F1\u01F2\u01C5\u0204\u0206\u0228\u0190\u018E\u0191\u01F4\u01E6\u01E4\u0193\u021E\u01CF\u0208\u020A\u0197\u0248\u01E8\u0198\u023D\u01C8\u019C")).toBe("AAAAAAAAEAEBBBCDDDDZDzDzEEEEEFGGGGHIIIIJKKLLjM");
    });

    it("covers Latin Extended-B (2/4) characters", () => {
        expect(Select2.stripDiacritics("\u01F8\u0220\u019D\u01CB\u022C\u022E\u0230\u022A\u01D1\u020C\u020E\u01A0\u01EA\u01EC\u01FE\u0186\u019F\u01A2\u0222\u01A4\u024A\u0210\u0212\u024C\u0218\u021A\u01AC\u01AE\u023E\u01DB\u01D7\u01D5\u01D9\u01D3\u0214\u0216\u01AF\u0244\u01B2\u0245")).toBe("NNNNjOOOOOOOOOOOOOOIOUPQRRRSTTTTUUUUUUUUUVV");
    });

    it("covers Latin Extended-B (3/4) characters", () => {
        expect(Select2.stripDiacritics("\u0232\u01B3\u024E\u01B5\u0224\u0227\u01E1\u01DF\u01FB\u01CE\u0201\u0203\u01FD\u01E3\u0188\u023C\u018C\u01F3\u0205\u0207\u0229\u0247\u01DD\u0192\u01F5\u01E7\u01E5\u021F\u0195\u01D0\u0209\u020B\u01F0\u0249\u01E9\u0199\u019A\u01F9\u019E\u022D")).toBe("YYYZZaaaaaaaaeaeccddzeeeeefggghhviiijjkklnno");
    });

    it("covers Latin Extended-B (4/4) characters", () => {
        expect(Select2.stripDiacritics("\u022F\u0231\u022B\u01D2\u020D\u020F\u01A1\u01EB\u01ED\u01FF\u01A3\u0223\u01A5\u024B\u0211\u0213\u024D\u0219\u023F\u021B\u01AD\u01DC\u01D8\u01D6\u01DA\u01D4\u0215\u0217\u01B0\u0233\u01B4\u024F\u01B6\u0225\u0240")).toBe("oooooooooooioupqrrrssttuuuuuuuuyyyzzz");
    });

    it("covers Latin Extended-A (1/3) characters", () => {
        expect(Select2.stripDiacritics("\u0100\u0102\u0104\u0106\u0108\u010A\u010C\u010E\u0112\u0114\u0116\u011A\u0118\u011C\u011E\u0120\u0122\u0124\u0128\u012A\u012C\u012E\u0134\u0136\u013F\u0139\u013D\u013B\u0143\u0147\u0145\u014C\u014E\u0150\u0154\u0158\u0156\u015A\u015C\u0160")).toBe("AAACCCCDEEEEEGGGGHIIIIJKLLLLNNNOOORRRSSS");
    });

    it("covers Latin Extended-A (2/3) characters", () => {
        expect(Select2.stripDiacritics("\u015E\u0164\u0162\u0168\u016A\u016C\u016E\u0170\u0172\u0174\u0176\u0178\u0179\u017B\u017D\u0101\u0103\u0105\u0107\u0109\u010B\u010D\u010F\u0113\u0115\u0117\u011B\u0119\u011D\u011F\u0121\u0123\u0125\u0129\u012B\u012D\u012F\u0135\u0137\u0140")).toBe("STTUUUUUUWYYZZZaaaccccdeeeeegggghiiiijkl");
    });

    it("covers Latin Extended-A (3/3) characters", () => {
        expect(Select2.stripDiacritics("\u013A\u013E\u013C\u0144\u0148\u0146\u0149\u014D\u014F\u0151\u0155\u0159\u0157\u015B\u015D\u0161\u015F\u0165\u0163\u0169\u016B\u016D\u016F\u0171\u0173\u0175\u0177\u017A\u017C\u017E")).toBe("lllnnnnooorrrssssttuuuuuuwyzzz");
    });

    it("covers Latin Extended-D (1/2) characters", () => {
        expect(Select2.stripDiacritics("\uA73A\uA73E\uA779\uA77B\uA7A0\uA77D\uA77E\uA78D\uA740\uA742\uA744\uA7A2\uA748\uA746\uA780\uA790\uA7A4\uA74A\uA74C\uA74E\uA750\uA752\uA754\uA756\uA758\uA75A\uA7A6\uA782\uA7A8\uA784\uA786\uA728\uA75E\uA760\uA762\uA735\uA737\uA739\uA73B\uA73D")).toBe("AVCDFGGGHKKKKLLLNNOOOOPPPQQRRRSSTTZVVYZaoauavavay");
    });

    it("covers Latin Extended-D (2/2) characters", () => {
        expect(Select2.stripDiacritics("\uA73F\uA77A\uA77C\uA7A1\uA77F\uA741\uA743\uA745\uA7A3\uA749\uA781\uA747\uA791\uA7A5\uA74B\uA74D\uA74F\uA751\uA753\uA755\uA757\uA759\uA75B\uA7A7\uA783\uA7A9\uA785\uA787\uA729\uA75F\uA761\uA763")).toBe("cdfggkkkklllnnoooopppqqrrrssttzvvyz");
    });

    it("covers Enclosed Alphanumerics (1/2) characters", () => {
        expect(Select2.stripDiacritics("\u24B7\u24B8\u24B9\u24BA\u24BB\u24BC\u24BD\u24BE\u24BF\u24C0\u24C1\u24C2\u24C3\u24C4\u24C5\u24C6\u24C7\u24C8\u24C9\u24CA\u24CB\u24CC\u24CD\u24CE\u24CF\u24D0\u24D2\u24D3\u24D4\u24D5\u24D6\u24D7\u24D8\u24D9\u24DA\u24DB\u24DC\u24DD\u24DE\u24DF")).toBe("BCDEFGHIJKLMNOPQRSTUVWXYZacdefghijklmnop");
    });

    it("covers Enclosed Alphanumerics (2/2) characters", () => {
        expect(Select2.stripDiacritics("\u24E0\u24E1\u24E2\u24E3\u24E4\u24E5\u24E6\u24E7\u24E8")).toBe("qrstuvwxy");
    });

    it("covers Halfwidth and Fullwidth Forms (1/2) characters", () => {
        expect(Select2.stripDiacritics("\uFF23\uFF24\uFF25\uFF26\uFF27\uFF28\uFF29\uFF2A\uFF2B\uFF2C\uFF2D\uFF2E\uFF2F\uFF30\uFF31\uFF32\uFF33\uFF34\uFF35\uFF36\uFF37\uFF38\uFF39\uFF42\uFF43\uFF44\uFF45\uFF46\uFF47\uFF48\uFF49\uFF4A\uFF4B\uFF4C\uFF4D\uFF4E\uFF4F\uFF50\uFF51\uFF52")).toBe("CDEFGHIJKLMNOPQRSTUVWXYbcdefghijklmnopqr");
    });

    it("covers Halfwidth and Fullwidth Forms (2/2) characters", () => {
        expect(Select2.stripDiacritics("\uFF53\uFF54\uFF55\uFF56\uFF57\uFF58\uFF59")).toBe("stuvwxy");
    });

    it("covers Latin-1 Supplement characters", () => {
        expect(Select2.stripDiacritics("\u00C0\u00C1\u00C2\u00C3\u00C7\u00C8\u00CA\u00CB\u00CC\u00CD\u00CE\u00CF\u00D1\u00D2\u00D3\u00D4\u00D5\u00D9\u00DA\u00DB\u00DD\u00E1\u00E3\u00EC\u00ED\u00EE\u00EF\u00F2\u00F4\u00F5\u00F9\u00FA\u00FB\u00FD\u00FF")).toBe("AAAACEEEIIIINOOOOUUUYaaiiiiooouuuyy");
    });

    it("covers Latin Extended-C characters", () => {
        expect(Select2.stripDiacritics("\u2C6F\u2C67\u2C75\u2C69\u2C62\u2C60\u2C6E\u2C63\u2C64\u2C7E\u2C72\u2C7F\u2C6B\u2C65\u2C68\u2C76\u2C6A\u2C61\u2C66\u2C73\u2C6C")).toBe("AHHKLLMPRSWZZahhkltwz");
    });

    it("covers IPA Extensions characters", () => {
        expect(Select2.stripDiacritics("\u0253\u0256\u0257\u025B\u0260\u0265\u0268\u026B\u0271\u026F\u0272\u0275\u027D\u0288\u0289\u028B\u028C")).toBe("bddeghilmmnortuvv");
    });

    it("covers Greek and Coptic characters", () => {
        expect(Select2.stripDiacritics("\u0389\u038A\u03AA\u038C\u038E\u03AB\u038F\u03AE\u03AF\u03CA\u03CC\u03CD\u03CB\u03B0")).toBe("\u0397\u0399\u0399\u039F\u03A5\u03A5\u03A9\u03B7\u03B9\u03B9\u03BF\u03C5\u03C5\u03C5");
    });

    it("covers Phonetic Extensions characters", () => {
        expect(Select2.stripDiacritics("\u1D79\u1D7D")).toBe("gp");
    });

    it("covers Number Forms characters", () => {
        expect(Select2.stripDiacritics("\u2184")).toBe("c");
    });
});
