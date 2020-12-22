import { Enum } from "@Q/System";

enum Test {
    Some = 1,
    Other = 5,
    Another = -1
}

test('Enum.getValues returns correct values', function() {
    expect(Enum.getValues(Test)).toStrictEqual([1, 5, -1]);
});