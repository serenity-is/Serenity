beforeEach(() => {
    jest.resetModules();
});

afterEach(() => {
    jest.resetModules();
});

function setupDummyJQueryForModal(callback) {
    global.$ = global.jQuery = function dummyJQueryForModal(html) {
        return {
            _html: html,
            html: function() {
                return null;
            },
            eq: function(index) {
                return this;
            },
            find: function(selector) {
                return this;
            },
            click: function() {
            },
            appendTo: function(target) {
                return this;
            },
            modal: function() {
                expect(this._html).toBe(html);
                callback(html);
            }
        }
    }
}

test('BS3 is detected when modal version starts with 3', function() {

    var Q = require("serenity-core").Q;

    var passedHtml;
    setupDummyJQueryForModal(function(html) {
        passedHtml = html;
    });
    try {
        global.$.fn = {
            modal: {
                Constructor: {
                    VERSION: '3.3.1'
                }
            }
        }

        Q.alert("hello");

        expect(passedHtml).not.toBeNull();

        var idx1 = passedHtml.indexOf('class="close"');
        var idx2 = passedHtml.indexOf('<h5');
        expect(idx1).toBeGreaterThan(-1);
        expect(idx2).toBeGreaterThan(idx1);
    }
    finally {
        delete global.$;
        delete global.jQuery;
    }
});


test('BS4 is detected when modal version does not exist', function() {
    
    var Q = require("serenity-core").Q;

    var passedHtml;
    setupDummyJQueryForModal(function(html) {
        passedHtml = html;
    });
    try {
        global.$.fn = {
            modal: {
            }
        }        

        Q.alert("hello");

        expect(passedHtml).not.toBeNull();

        var idx1 = passedHtml.indexOf('class="close"');
        var idx2 = passedHtml.indexOf('<h5');
        expect(idx1).toBeGreaterThan(-1);
        expect(idx2).toBeGreaterThan(-1);
        expect(idx1).toBeGreaterThan(idx2);
    }
    finally {
        delete global.$;
        delete global.fn;
    }
});

test('BS4 is detected when modal version is something other than 3', function() {
    
    var Q = require("serenity-core").Q;
    var passedHtml;
    setupDummyJQueryForModal(function(html) {
        passedHtml = html;
    });
    try {
        global.$.fn = {
            modal: {
                Constructor: {
                    VERSION: '4.1.0'
                }
            }
        }           

        Q.alert("hello");

        expect(passedHtml).not.toBeNull();

        var idx1 = passedHtml.indexOf('class="close"');
        var idx2 = passedHtml.indexOf('<h5');
        expect(idx1).toBeGreaterThan(-1);
        expect(idx2).toBeGreaterThan(-1);
        expect(idx1).toBeGreaterThan(idx2);
    }
    finally {
        delete global.$;
        delete global.fn;
    }    
});
