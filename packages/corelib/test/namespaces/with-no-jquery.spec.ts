import { loadNSCorelib } from "@/mocks";

test('works when jQuery not loaded', function() {
    loadNSCorelib();
    const Serenity = (window as any).Serenity;
    expect(Serenity.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});