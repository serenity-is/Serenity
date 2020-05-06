Q = require("SerenityCoreLibBase").Q;

test('Q.alert uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }
    try {
        Q.alert('test message');
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe('test message');
    } 
    finally {
        delete global.alert;
    }
});

test('Q.information uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    try {
        global.alert = function(message) {
            alertCount++;
            alertMessage = message;
        }
        
        Q.information('test message');
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe('test message');
    }
    finally {
        delete global.alert;
    }
});

test('Q.warning uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }

    try {
        Q.warning('test message');
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe('test message');
    }
    finally {
        delete global.alert;
    }
});

test('Q.confirm uses window.confirm when no BS/jQuery UI loaded', function() {
    var confirmCount = 0;
    var confirmCount = null;           
    global.confirm = function(message) {
        confirmCount++;
        confirmMessage = message;
        return true;
    }
    try {
        var onYesCalled;
        Q.confirm('test message', function() {
            onYesCalled = true;
        });
        expect(confirmCount).toBe(1);
        expect(confirmMessage).toBe('test message');
        expect(onYesCalled).toBe(true);
    }
    finally {
        delete global.confirm;
    }
});

test('Q.iframeDialog uses window.alert when no BS/jQuery UI loaded', function() {
    var alertCount = 0;
    var alertMessage = null;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }
    try {
        var testHtml = '<html><body>test message<body></html>';
        Q.iframeDialog({
            html: testHtml 
        });
        expect(alertCount).toBe(1);
        expect(alertMessage).toBe(testHtml);
    }
    finally {
        delete global.alert;
    }
});