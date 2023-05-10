import $ from "@optionaldeps/jquery"
import { loadNSCorelib } from "../testutil"

test('works when a dummy jQuery ui object exists', function() {
    ($ as any).ui = {};
    loadNSCorelib(window);
    const Q = (window as any).Q;
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});