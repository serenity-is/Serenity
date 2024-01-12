import { loadNSCorelib } from "../testutil";

test('works when a dummy jQuery ui object exists', function() {
    (window as any).jQuery = (window as any).$ = { fn: {} }
    loadNSCorelib(window);
    const Serenity = (window as any).Serenity;
    expect(Serenity.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});