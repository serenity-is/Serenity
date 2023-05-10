import { useIdPrefix } from './templatedwidget';

describe('useIdPrefix', () => {

    it('uses passed prefix', () => {
        var id = useIdPrefix('my_');

        expect(id._).toBe('my__');
        expect(id._x).toBe('my__x');
        expect(id.Form).toBe('my_Form');
        expect(id.PropertyGrid).toBe('my_PropertyGrid');
        expect(id.something).toBe('my_something');
    });

    it('handles hashes differently for in-page href generation', () => {
        var id = useIdPrefix('my_');

        expect(id["#_"]).toBe('#my__');
        expect(id["#_x"]).toBe('#my__x');
        expect(id["#Form"]).toBe('#my_Form');
        expect(id["#PropertyGrid"]).toBe('#my_PropertyGrid');
        expect(id["#something"]).toBe('#my_something');
    });

});