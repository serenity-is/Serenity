import { corelibPath, loadExternalScripts } from "../testutil"

test('summary type enum is registered', function() {
    loadExternalScripts(window, corelibPath);
    const Q = (window as any).Q;
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