import { debounce } from "./debounce";

describe("debounce", function () {

    it("debounces consecutive function calls", () => new Promise(done => {
        vi.useFakeTimers();

        var counter = 0;
        const increment = function () { counter++; };
        const debounced = debounce(increment, 100);
        debounced();
        debounced();
        setTimeout(debounced, 5);
        setTimeout(debounced, 10);
        setTimeout(function () {
            try {
                expect(counter).toBe(1);
                debounced.clear();
            }
            finally {
                done(void 0);
            }
        }, 150);
        vi.runOnlyPendingTimers();
    }));

    it('can clear pending invocations', () => new Promise(done => {
        vi.useFakeTimers();

        var counter = 0;
        var increment = function () { counter++; };
        var debounced = debounce(increment, 100);
        debounced();
        debounced.clear();
        setTimeout(function () {
            try {
                expect(counter).toBe(0);
                debounced.clear();
            }
            finally {
                done(void 0);
            }
        }, 150);
        vi.runOnlyPendingTimers();
    }));

    it('can invoke immediately, e.g. leading edge', () => new Promise(done => {
        vi.useFakeTimers();

        var a, b, c;
        var counter = 0;
        var increment = function () { return ++counter; };
        var debounced = debounce(increment, 100, true);
        a = debounced();
        b = debounced();
        expect(a).toBe(1);
        expect(b).toBe(1);
        expect(counter).toBe(1);
        setTimeout(debounced, 15);
        setTimeout(debounced, 30);
        setTimeout(function () {
            debounced();
            setTimeout(finish, 150);
        }, 50);
        var finish = function () {
            expect(counter).toBe(1);
            c = debounced();
            expect(c).toBe(2);
            expect(counter).toBe(2);
            debounced.clear();
            done(void 0);
        };
        vi.runAllTimers();
    }));

    it('can clear in immediate mode', () => new Promise(done => {
        vi.useFakeTimers();

        var a, b;
        var counter = 0;
        var increment = function () { return ++counter; };
        var debounced = debounce(increment, 100, true);
        a = debounced();
        debounced.clear();
        expect(counter).toBe(1);
        b = debounced();
        expect(a).toBe(1);
        expect(b).toBe(2);
        expect(counter).toBe(2);
        setTimeout(debounced, 15);
        setTimeout(debounced, 30);
        setTimeout(debounced, 45);
        setTimeout(function () {
            try {
                expect(counter).toBe(2);
                debounced.clear();
            }
            finally {
                done(void 0);
            }
        }, 10);
        vi.runAllTimers();
    }));

    it('can execute immediate recursively', () => new Promise(done => {
        vi.useFakeTimers();

        var counter = 0;
        var debounced = debounce(function () {
            counter++;
            if (counter < 10)
                debounced();
        }, 50, true);

        debounced();
        expect(counter).toBe(1);
        setTimeout(function () {
            try {
                expect(counter).toBe(1);
            }
            finally {
                done(void 0);
            }
        }, 150);
        vi.runAllTimers();
    }));

    it('can work after system time is set backwards', () => new Promise(done => {
        vi.useFakeTimers();

        var counter = 0;
        var debounced = debounce(function () {
            counter++;
        }, 100, true);

        var originalNowFunc = Date.now;
        var originalGetTimeFunc = Date.prototype.getTime;
        debounced();
        expect(counter).toBe(1);
        Date.prototype.getTime = function () {
            return +(new Date(new Date(originalNowFunc()).getFullYear() - 1, 0, 1, 1, 1, 1));
        }
        Date.now = function () {
            return +(new Date(new Date(originalNowFunc()).getFullYear() - 1, 0, 1, 1, 1, 1));
        }
        setTimeout(function () {
            try {
                debounced();
                expect(counter).toBe(2);
            }
            finally {
                Date.now = originalNowFunc;
                Date.prototype.getTime = originalGetTimeFunc;
                done(void 0);
            }
        }, 200);
        vi.runAllTimers();
    }));

    it('works after system time is is not accessible (or in invalid format)', () => new Promise(done => {
        vi.useFakeTimers();

        var counter = 0;
        var debounced = debounce(function () {
            counter++;
        }, 100, true);
        
        var originalNowFunc = Date.now;
        var originalGetTimeFunc = Date.prototype.getTime;
        var originalValueOfFunc = Date.prototype.valueOf;

        debounced();
        expect(counter).toBe(1);

        Date.prototype.valueOf = function () {
            return null;
        };
        Date.prototype.getTime = function () {
            return null;
        };
        Date.now = function () {
            return null;
        };

        setTimeout(function () {
            try {
                debounced();
                expect(counter).toBe(2);
            }
            finally {
                Date.now = originalNowFunc;
                Date.prototype.getTime = originalGetTimeFunc;
                Date.prototype.valueOf = originalValueOfFunc;
            }
        }, 200);

        setTimeout(function () {
            try {
                debounced();
                expect(counter).toBe(3);
            }
            finally {
                done(void 0);
            }
        }, 400);

        vi.runAllTimers();
    }));

    it('is re-entrant', () => new Promise(done => {
        vi.useFakeTimers();

        var sequence = [
            ['b1', 'b2']
        ];
        var value = '';
        var debounced: typeof append;
        const append = function (arg: string) {
            value += this + arg;
            var args = sequence.pop();
            if (args) {
                debounced.call(args[0], args[1]);
            }
        };
        debounced = debounce(append, 32);
        debounced.call('a1', 'a2');
        expect(value).toBe('');
        setTimeout(function () {
            try {
                expect(value).toBe('a1a2b1b2');
            }
            finally {
                done(void 0);
            }
        }, 100);

        vi.runAllTimers();
    }));

    it('uses a default wait of 100', () => new Promise(done => {
        vi.useFakeTimers();

        var counter = 0;
        const increment = function () { counter++; };
        const debounced = debounce(increment);
        try {
            expect(counter).toBe(0);
            debounced();
            expect(counter).toBe(0);
            setTimeout(function() {
                expect(counter).toBe(0);
            }, 50);
            setTimeout(function() {
                expect(counter).toBe(0);
            }, 99);
            setTimeout(function() {
                try {
                    expect(counter).toBe(1);
                }
                finally {
                    done(void 0);
                }
            }, 101);
        }
        catch {
            done(void 0);
        }

        vi.runAllTimers();
    }));

    it('supports flush method which calls the function if any pending calls', () => new Promise(done => {
        vi.useFakeTimers();

        var counter = 0;
        const increment = function () { counter++; };
        const debounced = debounce(increment);
        try {
            expect(counter).toBe(0);
            debounced();
            expect(counter).toBe(0);
            setTimeout(function() {
                expect(counter).toBe(0);
            }, 50);
            setTimeout(function() {
                expect(counter).toBe(0);
                debounced.flush();
                expect(counter).toBe(1);
            }, 99);
            setTimeout(function() {
                try {
                    expect(counter).toBe(1);
                    debounced.flush();
                    expect(counter).toBe(1);
                }
                finally {
                    done(void 0);
                }
            }, 101);
        }
        catch {
            done(void 0);
        }

        vi.runAllTimers();
    }));

});
