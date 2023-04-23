import { loadNSCorelib } from "../testutil"
import "jquery";

test('works when jQuery script loaded', function() {
    loadNSCorelib(window);
    const Q = (window as any).Q;
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});