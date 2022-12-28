import { alertDialog, confirmDialog, informationDialog, warningDialog, iframeDialog } from "@/q/dialogs";

test('Q.alertDialog uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    (global as any).window = global;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }
    try {
        alertDialog('test message');
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe('test message');
    } 
    finally {
        delete global.alert;
        delete global.window;
    }
});

test('Q.informationDialog uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    try {
        (global as any).window = global;
        global.alert = function(message) {
            alertCount++;
            alertMessage = message;
        }
        
        informationDialog('test message', () => { });
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe('test message');
    }
    finally {
        delete global.alert;
        delete global.window;
    }
});

test('Q.warningDialog uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    (global as any).window = global;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }

    try {
        warningDialog('test message');
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe('test message');
    }
    finally {
        delete global.alert;
        delete global.window;
    }
});

test('Q.confirmDialog uses window.confirm when no BS/jQuery UI loaded', function() {
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
        confirmDialog('test message', function() {
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