import { debounce } from "./debounce";

describe("debounce", function () {

    it("debounces consecutive function calls", function (done) {
        jest.useFakeTimers();

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
                done();
            }
        }, 150);
        jest.runOnlyPendingTimers();
    });

    it('can clear pending invocations', function (done) {
        jest.useFakeTimers();

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
                done();
            }
        }, 150);
        jest.runOnlyPendingTimers();
    });

    it('can invoke immediately, e.g. leading edge', function (done) {
        jest.useFakeTimers();

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
            done();
        };
        jest.runAllTimers();
    });

    it('can clear in immediate mode', function (done) {
        jest.useFakeTimers();

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
                done();
            }
        }, 10);
        jest.runAllTimers();
    });

    it('can execute immediate recursively', function (done) {
        jest.useFakeTimers();

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
                done();
            }
        }, 150);
        jest.runAllTimers();
    });

    it('can work after system time is set backwards', function (done) {
        jest.useFakeTimers();

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
                done();
            }
        }, 200);
        jest.runAllTimers();
    });

    it('works after system time is is not accessible (or in invalid format)', function (done) {
        jest.useFakeTimers();

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
                done();
            }
        }, 400);

        jest.runAllTimers();
    });

    it('is re-entrant', function (done) {
        jest.useFakeTimers();

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
                done();
            }
        }, 100);

        jest.runAllTimers();
    });

    it('uses a default wait of 100', (done) => {
        jest.useFakeTimers();

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
                    done();
                }
            }, 101);
        }
        catch {
            done();
        }

        jest.runAllTimers();
    });

    it('supports flush method which calls the function if any pending calls', (done) => {
        jest.useFakeTimers();

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
                    done();
                }
            }, 101);
        }
        catch {
            done();
        }

        jest.runAllTimers();
    });

});
