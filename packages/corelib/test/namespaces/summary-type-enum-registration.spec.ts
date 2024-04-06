import { loadNSCorelib } from "@/mocks";

beforeEach(() => {
    jest.resetModules();
});

test('summary type enum is registered', function () {
    loadNSCorelib();
    const Q = (window as any)?.Q;
    expect(Q).toBeDefined();
    const QSummaryType = Q.SummaryType;
    expect(QSummaryType).toBeDefined();
    const Serenity = (window as any).Serenity;
    expect(Serenity).toBeDefined();
    const SerenitySummaryType = Serenity.SummaryType;
    expect(SerenitySummaryType).toBeDefined();
    expect(Serenity.EnumTypeRegistry).toBeDefined();
    const fromRegistry = Serenity.EnumTypeRegistry.tryGet('Serenity.SummaryType')
    expect(fromRegistry).toBeDefined();
});