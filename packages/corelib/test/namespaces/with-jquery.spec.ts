import { loadNSCorelib } from "@/mocks";

test('works when jQuery script loaded', function() {
    loadNSCorelib();
    const Serenity = (window as any).Serenity;
    expect(Serenity.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});