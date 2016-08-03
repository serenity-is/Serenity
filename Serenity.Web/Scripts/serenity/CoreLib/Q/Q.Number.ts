/// <reference path="Q.StringHelpers.ts" />
/// <reference path="Q.Culture.ts" />

namespace Q {

    export function formatNumber(n: number, fmt: string, dec?: string, grp?: string): string {
        let neg = '-';
        if (isNaN(n)) {
            return null;
        }

        dec = dec || Culture.decimalSeparator;
        grp = grp || Culture.get_groupSeperator();

        let r = "";
        if (fmt.indexOf(".") > -1) {
            let dp = dec;
            let df = fmt.substring(fmt.lastIndexOf(".") + 1);
            n = roundNumber(n, df.length);
            let dv = n % 1;
            let ds = new String(dv.toFixed(df.length));
            ds = ds.substring(ds.lastIndexOf(".") + 1);
            for (let i = 0; i < df.length; i++) {
                if (df.charAt(i) == '#' && ds.charAt(i) != '0') {
                    dp += ds.charAt(i);
                    continue;
                }
                else if (df.charAt(i) == '#' && ds.charAt(i) == '0') {
                    let notParsed = ds.substring(i);
                    if (notParsed.match('[1-9]')) {
                        dp += ds.charAt(i);
                        continue;
                    }
                    else
                        break;
                }
                else if (df.charAt(i) == "0")
                    dp += ds.charAt(i);
                else
                    dp += df.charAt(i);
            }
            r += dp;
        }
        else
            n = Math.round(n);

        let ones = Math.floor(n);
        if (n < 0)
            ones = Math.ceil(n);
        let of = "";
        if (fmt.indexOf(".") == -1)
            of = fmt;
        else
            of = fmt.substring(0, fmt.indexOf("."));

        let op = "";
        if (!(ones == 0 && of.substr(of.length - 1) == '#')) {
            // find how many digits are in the group
            let oneText = new String(Math.abs(ones));
            let gl = 9999;
            if (of.lastIndexOf(",") != -1)
                gl = of.length - of.lastIndexOf(",") - 1;
            let gc = 0;
            for (let i = oneText.length - 1; i > -1; i--) {
                op = oneText.charAt(i) + op;
                gc++;
                if (gc == gl && i != 0) {
                    op = grp + op;
                    gc = 0;
                }
            }

            // account for any pre-data padding
            if (of.length > op.length) {
                let padStart = of.indexOf('0');
                if (padStart != -1) {
                    let padLen = of.length - padStart;
                    // pad to left with 0's or group char
                    let pos = of.length - op.length - 1;
                    while (op.length < padLen) {
                        let pc = of.charAt(pos);
                        // replace with real group char if needed
                        if (pc == ',')
                            pc = grp;
                        op = pc + op;
                        pos--;
                    }
                }
            }
        }

        if (!op && of.indexOf('0', of.length - 1) !== -1)
            op = '0';

        r = op + r;
        if (n < 0)
            r = neg + r;

        if (r.lastIndexOf(dec) == r.length - 1) {
            r = r.substring(0, r.length - 1);
        }

        return r;
    }

    let isoRegexp = /(\d{4,})(?:-(\d{1,2})(?:-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?(?:(Z)|([+-])(\d{1,2})(?::(\d{1,2}))?)?)?)?)?/;

    export function parseInteger(s: string): number {
        s = trim(s.toString());
        let ts = Culture.get_groupSeperator();
        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }
        if (!(/^[-\+]?\d+$/.test(s)))
            return NaN;
        return parseInt(s, 10);
    }

    export function parseDecimal(s: string): number {
        if (s == null)
            return null;

        s = trim(s.toString());
        if (s.length == 0)
            return null;

        let ts = Culture.get_groupSeperator();

        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }

        if (!(new RegExp("^\\s*([-\\+])?(\\d*)\\" + Culture.decimalSeparator + "?(\\d*)\\s*$").test(s)))
            return NaN;

        return parseFloat(s.toString().replace(Culture.decimalSeparator, '.'));
    }

    function roundNumber(n: number, dec?: number): number {
        let power = Math.pow(10, dec || 0);
        let value = (Math.round(n * power) / power).toString();
        // ensure the decimal places are there
        if (dec > 0) {
            let dp = value.indexOf(".");
            if (dp == -1) {
                value += '.';
                dp = 0;
            }
            else {
                dp = value.length - (dp + 1);
            }
            while (dp < dec) {
                value += '0';
                dp++;
            }
        }
        return parseFloat(value);
    }

    export function toId(id: any): any {
        if (id == null)
            return null;
        if (typeof id == "number")
            return id;
        id = $.trim(id);
        if (id == null || !id.length)
            return null;
        if (id.length >= 15 || !(/^-?\d+$/.test(id)))
            return id;
        let v = parseInt(id, 10);
        if (isNaN(v))
            return id;
        return v;
    }
}