var path = require("path");
var fs = require("fs");

const root = path.resolve('../').replace(/\\/g, '/');
const assets = path.resolve('../../Serenity.Assets/wwwroot/').replace(/\\/g, '/');

for (var url of [
    "~/node_modules/jquery/dist/jquery.slim.min.js",
    "~/Serenity.Assets/Scripts/jquery.event.drag.min.js",
    "~/SlickGrid/out/slick.core.js",
    "~/SlickGrid/out/slick.grid.js",
]) {

    if (url.charAt(0) != '~')
        continue;

    url = url.substring(1);

    if (/^\/Serenity.Assets\//.test(url))
        url = assets + url.substring("/Serenity.Assets".length);
    else
        url = root + url;

    url = url.replace(/\\/g, '/');

    var content = fs.readFileSync(url, 'utf-8');
    window.eval(content);
}