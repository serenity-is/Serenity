import { readFileSync } from "fs";
import { join, resolve } from "path";
import { fileURLToPath, URL } from "url";

const root = resolve('./');

const assetsScripts = "~/../../Serenity.Assets/wwwroot/Scripts/";
export const jqueryPath = assetsScripts + "jquery-3.5.1.min.js";
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