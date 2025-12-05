import { copyFileSync, constants, existsSync, mkdirSync, readFileSync, writeFileSync } from 'node:fs';
import { dirname } from 'node:path';

function copyFileIfChanged(srcFile, dstfile, opt) {

    let srcContent;
    function getSrcContent() {
        if (srcContent === undefined) {
            srcContent = readFileSync(srcFile, opt?.encoding);
            if (opt?.patchContents) {
                srcContent = opt.patchContents(srcContent, srcFile);
            }
        }
    }

    if (existsSync(dstfile)) {
        getSrcContent();
        const dstContent = readFileSync(dstfile, opt?.encoding);
        if (typeof dstContent === 'string' && typeof srcContent === 'string' && dstContent === srcContent)
            return;
        if (dstContent.equals && dstContent.equals(srcContent))
            return;
    }
    else {
        mkdirSync(dirname(dstfile), { recursive: true });
    }

    if (opt?.patchContents) {
        getSrcContent();
        writeFileSync(dstfile, srcContent, opt?.encoding);
        console.log(`Patched and copied: ${srcFile} to ${dstfile}.`);
        return;
    }

    copyFileSync(srcFile, dstfile, constants.COPYFILE_FICLONE);
    console.log(`Copied: ${srcFile} to ${dstfile}.`);
}

function removeSourceMappingURL(content, srcFile) {
    content = content.replace(/^\/[*\/]\s*[#@]\s(source(?:Mapping)?URL)=\s*(\S+)\s*\*?\/?$/gm, '');
    if (content.indexOf("sourceMappingURL") >= 0) {
        console.warn(`Warning: sourceMappingURL comment not removed from ${srcFile}!`);
    }
    return content;
}

for (const file of [
    "jquery.js",
    "jquery.min.js"
]) {
    copyFileIfChanged(`node_modules/jquery/dist/${file}`, `wwwroot/jquery/${file}`);
}

for (const file of [
    "mousetrap.js",
    "mousetrap.min.js"
]) {
    copyFileIfChanged(`node_modules/mousetrap/${file}`, `wwwroot/mousetrap/${file}`);
}

for (const file of [
    "css/bootstrap.css",
    "css/bootstrap.min.css",
    "css/bootstrap.rtl.css",
    "css/bootstrap.rtl.min.css",
    "js/bootstrap.bundle.js",
    "js/bootstrap.bundle.min.js"
]) {
    copyFileIfChanged(`node_modules/bootstrap/dist/${file}`, `wwwroot/bootstrap/${file}`, {
        encoding: 'utf-8',
        patchContents: removeSourceMappingURL
    });
}

copyFileIfChanged(`node_modules/glightbox/dist/js/glightbox.js`, `wwwroot/glightbox/js/glightbox.js`, {
    encoding: 'utf-8',
    patchContents: content => {
        // fix for csp issues
        content = content.replace(/setAttribute\s*\(\s*['"]style['"]\s*,\s*/g, 'style = (');
        if (/setAttribute\(['"]style['"]/.test(content)) {
            console.warn(`Warning: Some setAttribute('style', ...) calls may remain in glightbox.js!`);
        }
        return content;
    }
});

copyFileIfChanged(`node_modules/glightbox/dist/css/glightbox.css`, `wwwroot/glightbox/css/glightbox.css`);
for (const file of [
    "nprogress.js",
    "nprogress.css"
]) {
    copyFileIfChanged(`node_modules/nprogress/${file}`, `wwwroot/nprogress/${file}`);
};