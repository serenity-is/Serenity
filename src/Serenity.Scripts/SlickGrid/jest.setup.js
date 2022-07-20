var path = require('path');
var root = path.resolve('../').replace(/\\/g, '/');
var assets = path.resolve('../../Serenity.Assets/wwwroot/').replace(/\\/g, '/');

beforeAll(function(done) {
   
    Object.defineProperty(document, "readyState", {
        get() { return "loading"; },
        configurable: true
    });
    
    function myDone() {
        Object.defineProperty(document, "readyState", {
            get() { return "complete"; },
            configurable: true
        });        

        window.document.dispatchEvent(new Event("DOMContentLoaded", {
            bubbles: true,
            cancelable: true
        }));

        setTimeout(done);
    }

    var loading = 0;
    function loadScriptFile(url) {
        url = url.replace(/\\/g, '/');
        url = 'file:///' + url;
        var script = document.createElement('script');
        script.src = url;
        document.head.appendChild(script);
        loading++;
        script.onload = function() {
            loading--;
            if (loading <= 0)
                myDone();
        }
        script.onerror = function() {
            myDone();
        }
    }

    function loadScripts(scripts) {
        for (var i = 0; i < scripts.length; i++) {
            var url = scripts[i];

            if (url.charAt(0) != '~')
                continue;

            url = url.substr(1);
            
            if (/^\/Serenity.Assets\//.test(url))
                url = assets + url.substr("/Serenity.Assets".length);
            else
                url = root + url;

            loadScriptFile(url);
        }
    }

    loadScripts([
        "~/node_modules/jquery/dist/jquery.min.js",
        "~/Serenity.Assets/Scripts/jquery.event.drag.js",
        "~/SlickGrid/out/slick.core.js",
        "~/SlickGrid/out/slick.grid.js",
    ]);

    if (loading == 0)
        myDone();
});