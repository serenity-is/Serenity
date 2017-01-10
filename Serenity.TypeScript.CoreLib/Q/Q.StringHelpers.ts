
namespace Q {
    export function endsWith(s: string, search: string): boolean {
        return (ss as any).endsWithString(s, search);
    }

    export function isEmptyOrNull(s: string) {
        return s == null || s.length === 0;
    }

    export function isTrimmedEmpty(s: string) {
        return trimToNull(s) == null;
    }

    export function format(msg: string, ...prm: any[]): string {
        return (ss as any).formatString(msg, ...prm);
    }

    export function padLeft(s: string, len: number, ch: string = ' ') {
        while (s.length < len)
            s = "0" + s;
        return s;
    }

    export function startsWith(s: string, search: string): boolean {
        return (ss as any).startsWithString(s, search);
    }

    export function toSingleLine(str: string) {
        return Q.replaceAll(Q.replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
    }

    export function trim(s: string) {
        return (s == null ? '' : s).replace(new RegExp('^\\s+|\\s+$', 'g'), '');
    }

    export function trimToEmpty(s: string) {
        if (s == null || s.length === 0) {
            return '';
        }

        return trim(s);
    }

    export function trimToNull(s: string) {
        if (s == null || s.length === 0) {
            return null;
        }

        s = trim(s);
        if (s.length === 0) {
            return null;
        }
        else {
            return s;
        }
    }

    let turkishOrder: {};

    export function turkishLocaleCompare(a: string, b: string): number {
        let alphabet = "AaBbCcÇçFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpRrSsŞşTtUuÜüVvYyZz";
        a = a || "";
        b = b || "";
        if (a == b)
            return 0;
        if (!turkishOrder) {
            turkishOrder = {};
            for (let z = 0; z < alphabet.length; z++) {
                turkishOrder[alphabet.charAt(z)] = z + 1;
            }
        }
        for (let i = 0, _len = Math.min(a.length, b.length); i < _len; i++) {
            let x = a.charAt(i), y = b.charAt(i);
            if (x === y)
                continue;
            let ix = turkishOrder[x], iy = turkishOrder[y];
            if (ix != null && iy != null)
                return ix < iy ? -1 : 1;
            let c = x.localeCompare(y);
            if (c == 0)
                continue;
            return c;
        }
        return a.localeCompare(b);
    }

    export function turkishLocaleToUpper(a: string): string {
        if (!a)
            return a;
        return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
    }

    export function replaceAll(s: string, f: string, r: string): string {
        return (ss as any).replaceAllString(s, f, r);
    }

    export function zeroPad(n: number, digits: number): string {
        let s = n.toString();
        while (s.length < digits)
            s = "0" + s;
        return s;
    }
}