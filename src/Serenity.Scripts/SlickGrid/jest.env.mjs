import { resolve } from 'path';
import { readFileSync } from 'fs';
import HappyDOMEnvironment from "@happy-dom/jest-environment";

const root = resolve('../').replace(/\\/g, '/');
const assets = resolve('../../Serenity.Assets/wwwroot/').replace(/\\/g, '/');

var BaseEnvironment = HappyDOMEnvironment.default ?? HappyDOMEnvironment;

export default class MyEnvironment extends BaseEnvironment {

	constructor(config, options) {
		super(config, options);		
        
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

			var content = readFileSync(url);
			this.window.eval(content);
		}
    }
}