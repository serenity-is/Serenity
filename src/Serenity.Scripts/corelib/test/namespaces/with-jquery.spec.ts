import { corelibPath, jqueryPath, loadExternalScripts } from "../testutil"

test('works when jQuery script loaded', function() {
    loadExternalScripts(window, jqueryPath, corelibPath);
    const Q = (window as any).Q;
    expect(Q.replaceAll('xyx', 'x', 'y')).toBe('yyy');
});