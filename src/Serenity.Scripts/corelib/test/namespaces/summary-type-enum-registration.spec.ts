import { corelibPath, loadExternalScripts } from "../testutil"

test('summary type enum is registered', function() {
    loadExternalScripts(window, corelibPath);
    const Q = (window as any).Q;
    const QSummaryType = (window as any).Q.SummaryType;
    expect(QSummaryType != null).toBe(true);
    const Serenity = (window as any).Serenity;
    const SerenitySummaryType = Serenity.SummaryType;
    expect(SerenitySummaryType != null).toBe(true);
    const fromRegistry = Serenity.EnumTypeRegistry.tryGet('Serenity.SummaryType')
    expect(fromRegistry != null).toBe(true);

});