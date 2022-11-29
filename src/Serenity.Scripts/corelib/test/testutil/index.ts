import { readFileSync } from "fs";
import { join, resolve } from "path";
import { URL } from "url";

const root = resolve('./');

const assetsRoot = "~/../../Serenity.Assets/wwwroot/";
export const jqueryPath = assetsRoot + "jquery/jquery.min.js";
export const corelibPath = "~/out/Serenity.CoreLib.js";

export function loadExternalScripts(window: any, ...scripts: string[]) {
    scripts.forEach(path => {
        if (path.startsWith('~/'))
            path = join(root, path.substring(2));
        const src = readFileSync(path, 'utf8');
        const scriptEl = window.document.createElement("script");
        scriptEl.textContent = src;
        window.document.body.appendChild(scriptEl);
    });
}