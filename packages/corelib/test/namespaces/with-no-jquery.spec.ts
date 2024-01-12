import { loadNSCorelib } from "../testutil"

test('works when jSerenityuery not loaded', function() {
    loadNSCorelib(window);
    const Serenity = (window as any).Serenity;
    expect(Serenity.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});