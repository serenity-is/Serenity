import { Enum } from "../../../../CoreLib/Q/System";

enum Test {
    Some = 1,
    Other = 5,
    Another = -1
}

test('Enum.toString returns names', function() {
    expect(Enum.toString(Test, 1)).toBe("Some");
    expect(Enum.toString(Test, 5)).toBe("Other");
    expect(Enum.toString(Test, -1)).toBe("Another");
});

test('Enum.toString returns number if non-existent', function() {
    expect(Enum.toString(Test, 0)).toBe("0");
    expect(Enum.toString(Test, -5)).toBe("-5");
});