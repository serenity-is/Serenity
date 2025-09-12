import { DateTimeFormatter } from "./datetimeformatter";

describe("DateTimeFormatter", () => {
    it("shows correctly formatted date time", () => {
        var formatter = new DateTimeFormatter();
        var date = new Date(2023, 0, 1, 12, 15, 18); // 01-01-2023 12:15:18
        expect(formatter.format({ value: date, escape: (s) => s })).toBe("01/01/2023 12:15:18");
    });
})
