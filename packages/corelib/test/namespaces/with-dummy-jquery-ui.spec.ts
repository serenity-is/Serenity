import { getjQuery } from "@serenity-is/base";
import { loadNSCorelib } from "../testutil"

test('works when a dummy jQuery ui object exists', function() {
    (window as any).jQuery = (window as any).$ = { fn: {} }
    let $ = getjQuery();
    loadNSCorelib(window);
    const Q = (window as any).Q;
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});