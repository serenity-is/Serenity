import { loadNSCorelib } from "../testutil";
import type { Criteria } from "@/q/criteria";

beforeEach(() => {
    jest.resetModules();
});

test('applies workaround for criteria builder array.of issue in ES5', function () {
    loadNSCorelib(window);
    var criteria = (window as any)?.Serenity?.Criteria as typeof Criteria;
    expect(criteria).toBeDefined();
    var c = criteria("test");
    expect(c).toBeDefined();
    expect(typeof c.eq).toBeDefined();
    expect(c.eq(1)).toEqual([["test"], '=', 1]);
});