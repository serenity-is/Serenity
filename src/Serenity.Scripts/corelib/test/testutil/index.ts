import { readFileSync } from "fs";
import { join, resolve } from "path";

const root = resolve('./');

const nscorelibPath = "~/out/Serenity.CoreLib.js";

export function loadNSCorelib(window: any) {
    loadExternalScripts(window, nscorelibPath);
}

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