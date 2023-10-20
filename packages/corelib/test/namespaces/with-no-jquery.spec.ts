import { loadNSCorelib } from "../testutil"

test('works when jQuery not loaded', function() {
    loadNSCorelib(window);
    const Q = (window as any).Serenity;
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});