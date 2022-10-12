import { corelibPath, loadExternalScripts } from "../testutil"

test('works when jQuery not loaded', function() {
    loadExternalScripts(window, corelibPath);
    const Q = (window as any).Q;
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});