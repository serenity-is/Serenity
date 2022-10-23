import { corelibPath, jqueryPath, loadExternalScripts } from "../testutil"

test('works when a dummy jQuery ui object exists', function() {
    loadExternalScripts(window, jqueryPath, corelibPath);
    ($ as any).ui = {};
    loadExternalScripts(window, corelibPath);
    const Q = (window as any).Q;
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});