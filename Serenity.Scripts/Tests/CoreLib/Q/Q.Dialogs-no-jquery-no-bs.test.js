Q = require("SerenityCoreLibBase").Q;

test('Q.alert uses window.alert when no BS/jQuery UI loaded', function() {
    alertCount = 0;
    alertMessage = null;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }
    
    Q.alert('test message');
    expect(alertCount).toBe(1);
    expect(alertMessage).toBe('test message');
});

test('Q.information uses window.alert when no BS/jQuery UI loaded', function() {
    alertCount = 0;
    alertMessage = null;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }
    
    Q.information('test message');
    expect(alertCount).toBe(1);
    expect(alertMessage).toBe('test message');
});

test('Q.warning uses window.alert when no BS/jQuery UI loaded', function() {
    alertCount = 0;
    alertMessage = null;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }
    
    Q.warning('test message');
    expect(alertCount).toBe(1);
    expect(alertMessage).toBe('test message');
});

test('Q.confirm uses window.confirm when no BS/jQuery UI loaded', function() {
    confirmCount = 0;
    confirmCount = null;
    global.confirm = function(message) {
        confirmCount++;
        confirmMessage = message;
        return true;
    }
    
    var onYesCalled;
    Q.confirm('test message', function() {
        onYesCalled = true;
    });
    expect(confirmCount).toBe(1);
    expect(confirmMessage).toBe('test message');
    expect(onYesCalled).toBe(true);
});

test('Q.iframeDialog uses window.alert when no BS/jQuery UI loaded', function() {
    alertCount = 0;
    alertMessage = null;
    global.alert = function(message) {
        alertCount++;
        alertMessage = message;
    }
    
    var testHtml = '<html><body>test message<body></html>';
    Q.iframeDialog({
        html: testHtml 
    });
    expect(alertCount).toBe(1);
    expect(alertMessage).toBe(testHtml);
});