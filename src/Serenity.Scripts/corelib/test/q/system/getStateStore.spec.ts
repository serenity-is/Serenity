import { Exception, getStateStore } from "@/q/system";

test('if globalThis.Q is null, it can assign it', function() {
    var q = globalThis.Q;
    try {
        delete globalThis.Q;
        expect(globalThis.Q).toBeUndefined();
        var stateStore = getStateStore();
        expect(globalThis.Q).toBeTruthy();
        expect(stateStore).toBe(globalThis.Q.__stateStore);
    }
    finally {
        globalThis.Q = q;
    }
});

test('if globalThis.Q is not null, it can create __stateStore', function() {
    var q = globalThis.Q;
    try {
        delete globalThis.Q;
        expect(globalThis.Q).toBeUndefined();
        var newQ = globalThis.Q = Object.create(null);
        expect(newQ.__stateStore).toBeUndefined();
        var stateStore = getStateStore();
        expect(globalThis.Q).toBeTruthy();
        expect(globalThis.Q === newQ).toBe(true);
        expect(typeof globalThis.Q.__stateStore == "object").toBe(true);
        expect(Object.keys(globalThis.Q.__stateStore).length).toBe(0);
        expect(stateStore === globalThis.Q.__stateStore).toBe(true);
    }
    finally {
        globalThis.Q = q;
    }
});

test('if globalThis.Q.__stateStore is not null, it returns that', function() {
    var q = globalThis.Q;
    try {
        delete globalThis.Q;
        expect(globalThis.Q).toBeUndefined();
        var newQ = globalThis.Q = Object.create(null);
        var newStore = newQ.__stateStore = Object.create(null);
        newStore.__myKey = "theKey";
        var stateStore = getStateStore();
        expect(globalThis.Q).toBeTruthy();
        expect(globalThis.Q === newQ).toBe(true);
        expect(stateStore === newStore).toBe(true);
        expect(globalThis.Q.__stateStore === newStore).toBe(true);
        expect(Object.keys(globalThis.Q.__stateStore).length).toBe(1);
        expect(globalThis.Q.__stateStore.__myKey).toBe("theKey");
    }
    finally {
        globalThis.Q = q;
    }
});

test('if a new store key is provided, it auto initializes it to empty object and returns', function() {
    var q = globalThis.Q;
    try {
        delete globalThis.Q;
        var newStore = {};
        globalThis.Q = { __stateStore: newStore };
        var sub = getStateStore("sub");
        expect(typeof sub).toBe("object");
        expect(Object.keys(sub).length).toBe(0);
        expect(globalThis.Q.__stateStore === newStore).toBe(true);
        expect(Object.keys(newStore).length).toBe(1);
    }
    finally {
        globalThis.Q = q;
    }
});

test('if returns same sub store instance every time', function() {
    var q = globalThis.Q;
    try {
        delete globalThis.Q;
        globalThis.Q = { __stateStore: {} };
        var sub1 = getStateStore("sub");
        sub1.test = "A";
        var sub2 = getStateStore("sub");
        expect(sub1 === sub2).toBe(true);
        expect(sub2.test).toBe("A");
    }
    finally {
        globalThis.Q = q;
    }
});

test('if does not return same sub store for different keys', function() {
    var q = globalThis.Q;
    try {
        delete globalThis.Q;
        globalThis.Q = { __stateStore: {} };
        var sub1 = getStateStore("sub1");
        sub1.test = "A";
        var sub2 = getStateStore("sub2");
        expect(sub1 !== sub2).toBe(true);
        expect(sub1.test).toBe("A");
        expect(sub2.test).toBeUndefined();
    }
    finally {
        globalThis.Q = q;
    }
});