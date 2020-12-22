import { alert, confirm, information, warning, iframeDialog } from "../../../../CoreLib/Q/Dialogs";

test('Q.alert uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    (global as any).window = global;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }
    try {
        alert('test message');
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe('test message');
    } 
    finally {
        delete global.alert;
        delete global.window;
    }
});

test('Q.information uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    try {
        (global as any).window = global;
        global.alert = function(message) {
            alertCount++;
            alertMessage = message;
        }
        
        information('test message', () => { });
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe('test message');
    }
    finally {
        delete global.alert;
        delete global.window;
    }
});

test('Q.warning uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    (global as any).window = global;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }

    try {
        warning('test message');
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe('test message');
    }
    finally {
        delete global.alert;
        delete global.window;
    }
});

test('Q.confirm uses window.confirm when no BS/jQuery UI loaded', function() {
    var confirmCount = 0;           
    var confirmMessage: string = null;
    (global as any).window = global;
    global.confirm = function(message) {
        confirmCount++;
        confirmMessage = message;
        return true;
    }
    try {
        var onYesCalled;
        confirm('test message', function() {
            onYesCalled = true;
        });
        expect(confirmCount).toBe(1);
        expect(confirmMessage).toBe('test message');
        expect(onYesCalled).toBe(true);
    }
    finally {
        delete global.confirm;
        delete global.window;
    }
});

test('Q.iframeDialog uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    (global as any).window = global;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }
    try {
        var testHtml = '<html><body>test message<body></html>';
        iframeDialog({
            html: testHtml 
        });
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe(testHtml);
    }
    finally {
        delete global.alert;
        delete global.window;
    }
});