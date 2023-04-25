import { LT } from "../../q/localtext";
import { PropertyItemSlickConverter } from "./slickhelpers";

describe('SlickHelpers.toSlickColumn', () => {
    it('tries to load a localText with the items name as key', () => {
        var converted = PropertyItemSlickConverter.toSlickColumn({
            title: 'Test.Local.Text.Key'
        });
        
        expect(converted.name).toBe('Test.Local.Text.Key');

        LT.add('Test.Local.Text.Key', 'translated');

        var converted2 = PropertyItemSlickConverter.toSlickColumn({
            title: 'Test.Local.Text.Key'
        });

        expect(converted2.name).toBe('translated');
    });
});