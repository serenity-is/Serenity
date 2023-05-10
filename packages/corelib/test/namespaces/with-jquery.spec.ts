import { loadNSCorelib } from "../testutil"

test('works when jQuery script loaded', function() {
    import("@optionaldeps/jquery");
    loadNSCorelib(window);
    const Q = (window as any).Q;
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});