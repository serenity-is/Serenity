// @ts-ignore
import { readFileSync } from "fs";
// @ts-ignore
import { join, resolve } from "path";

const root = resolve('./');

const nscorelibPath = "~/out/Serenity.CoreLib.js";

export function loadNSCorelib(window) {
    loadExternalScripts(window, nscorelibPath);
}

export function loadExternalScripts(window, ...scripts) {
    scripts.forEach(path => {
        if (path.startsWith('~/'))
            path = join(root, path.substring(2));
        const src = readFileSync(path, 'utf8');
        const scriptEl = window.document.createElement("script");
        scriptEl.textContent = src;
        window.document.body.appendChild(scriptEl);
    });
}