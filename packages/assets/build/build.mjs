import { copyFileSync, constants, existsSync, mkdirSync, readFileSync, writeFileSync } from 'node:fs';
import { dirname } from 'node:path';
import esbuild from 'esbuild';

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

// Apply RTL support from SortableJS PR #2368 (https://github.com/SortableJS/Sortable/pull/2368).
// This PR is not yet merged upstream, so patch the copied global dist file manually.
function patchSortableRtl(content) {
    // Turn a code snippet into a regex that tolerates whitespace differences
    // (indentation, line breaks, blank lines, extra spaces), so basic reformatting
    // of the dist file doesn't break the patch. The leading whitespace run matches
    // horizontal whitespace only, so it never swallows the preceding line's newline.
    const flex = code => {
        const escaped = code.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // escape regex metacharacters
        return new RegExp(
            escaped
                .replace(/\s+/g, '\\s+')     // any whitespace run -> flexible whitespace
                .replace(/^\\s\+/, '[ \\t]*') // leading run -> horizontal-only, optional
        );
    };

    const replace = (from, to) => {
        const re = flex(from);
        if (!re.test(content)) {
            throw new Error(`Sortable.js RTL patch: pattern not found:\n${from}`);
        }
        content = content.replace(re, () => to);
    };

    // 1. Add 'rtl' default option after 'direction'. Detects RTL from the container's
    //    computed direction, ignoring lists whose layout is already row-reversed.
    replace(
        `      direction: function direction() {\n        return _detectDirection(el, this.options);\n      },`,
        `      direction: function direction() {\n        return _detectDirection(el, this.options);\n      },\n      rtl: function rtl() {\n        return (css(el, 'direction') === 'rtl') !== (css(el, 'flex-direction') === 'row-reverse');\n      },`
    );

    // 2. Generalize _getDirection so both 'direction' and 'rtl' options resolve
    //    either as a plain value or as a function.
    replace(
        `    _getDirection: function _getDirection(evt, target) {\n      return typeof this.options.direction === 'function' ? this.options.direction.call(this, evt, target, dragEl) : this.options.direction;\n    },`,
        `    _getOptionValue: function _getOptionValue(evt, target, optionName) {\n      return typeof this.options[optionName] === 'function' ? this.options[optionName].call(this, evt, target, dragEl) : this.options[optionName];\n    },`
    );

    // 3. Declare the rtl variable in _onMove.
    replace(
        `        vertical,\n        _this = this,`,
        `        vertical,\n        rtl,\n        _this = this,`
    );

    // 4. Resolve vertical / rtl through _getOptionValue.
    replace(
        `        vertical = this._getDirection(evt, target) === 'vertical';`,
        `        vertical = this._getOptionValue(evt, target, 'direction') === 'vertical';\n        rtl = this._getOptionValue(evt, target, 'rtl');`
    );

    // 5. Pass rtl through to the ghost position helpers.
    replace(
        `_ghostIsLast(evt, vertical, this)`,
        `_ghostIsLast(evt, vertical, rtl, this)`
    );
    replace(
        `_ghostIsFirst(evt, vertical, this)`,
        `_ghostIsFirst(evt, vertical, rtl, this)`
    );

    // 6. Rewrite _ghostIsFirst to account for RTL horizontal lists.
    replace(
        `  function _ghostIsFirst(evt, vertical, sortable) {\n    var firstElRect = getRect(getChild(sortable.el, 0, sortable.options, true));\n    var childContainingRect = getChildContainingRectFromElement(sortable.el, sortable.options, ghostEl);\n    var spacer = 10;\n    return vertical ? evt.clientX < childContainingRect.left - spacer || evt.clientY < firstElRect.top && evt.clientX < firstElRect.right : evt.clientY < childContainingRect.top - spacer || evt.clientY < firstElRect.bottom && evt.clientX < firstElRect.left;\n  }`,
        `  function _ghostIsFirst(evt, vertical, rtl, sortable) {\n    var firstElRect = getRect(getChild(sortable.el, 0, sortable.options, true));\n    var childContainingRect = getChildContainingRectFromElement(sortable.el, sortable.options, ghostEl);\n    var spacer = 10;\n    if (vertical) {\n      return evt.clientX < childContainingRect.left - spacer || evt.clientY < firstElRect.top && evt.clientX < firstElRect.right;\n    } else if (!rtl) {\n      return evt.clientY < childContainingRect.top - spacer || evt.clientY < firstElRect.bottom && evt.clientX < firstElRect.left;\n    } else {\n      return evt.clientY < childContainingRect.top - spacer || evt.clientY < firstElRect.bottom && evt.clientX > firstElRect.right;\n    }\n  }`
    );

    // 7. Rewrite _ghostIsLast to account for RTL horizontal lists.
    replace(
        `  function _ghostIsLast(evt, vertical, sortable) {\n    var lastElRect = getRect(lastChild(sortable.el, sortable.options.draggable));\n    var childContainingRect = getChildContainingRectFromElement(sortable.el, sortable.options, ghostEl);\n    var spacer = 10;\n    return vertical ? evt.clientX > childContainingRect.right + spacer || evt.clientY > lastElRect.bottom && evt.clientX > lastElRect.left : evt.clientY > childContainingRect.bottom + spacer || evt.clientX > lastElRect.right && evt.clientY > lastElRect.top;\n  }`,
        `  function _ghostIsLast(evt, vertical, rtl, sortable) {\n    var lastElRect = getRect(lastChild(sortable.el, sortable.options.draggable));\n    var childContainingRect = getChildContainingRectFromElement(sortable.el, sortable.options, ghostEl);\n    var spacer = 10;\n    if (vertical) {\n      return evt.clientX > childContainingRect.right + spacer || evt.clientY > lastElRect.bottom && evt.clientX > lastElRect.left;\n    } else if (!rtl) {\n      return evt.clientY > childContainingRect.bottom + spacer || evt.clientX > lastElRect.right && evt.clientY > lastElRect.top;\n    } else {\n      return evt.clientY > childContainingRect.bottom + spacer || evt.clientX < lastElRect.left && evt.clientY > lastElRect.top;\n    }\n  }`
    );

    return content;
}

copyFileIfChanged(`node_modules/sortablejs/Sortable.js`, `wwwroot/Scripts/sortable.js`, {
    encoding: 'utf-8',
    patchContents: patchSortableRtl
});

// The npm Sortable.min.js does not contain the RTL fix, and patching minified code is
// fragile, so generate the minified version from the patched source using esbuild.
copyFileIfChanged(`node_modules/sortablejs/Sortable.js`, `wwwroot/Scripts/sortable.min.js`, {
    encoding: 'utf-8',
    patchContents: content => esbuild.transformSync(patchSortableRtl(content), {
        minify: true,
        target: 'es5',
        legalComments: 'inline'
    }).code
});

function writeIfChanged() {
    return {
        name: "write-if-changed",
        setup(build) {
            const write = build.initialOptions.write;
            build.initialOptions.write = false;
            build.onEnd(result => {
                if (!(write === undefined || write))
                    return;
                result.outputFiles?.forEach(file => {
                    if (existsSync(file.path)) {
                        const old = readFileSync(file.path);
                        if (old.equals(file.contents))
                            return;
                    }
                    else {
                        mkdirSync(dirname(file.path), { recursive: true });
                    }
                    writeFileSync(file.path, file.text);
                });
            });
        }
    };
}

await esbuild.build({
    entryPoints: ['./src/**/*.mjs'],
    bundle: true,
    color: true,
    external: [
        "canvg", // jspdf optional dependency
        "core-js", // jspdf optional dependency
        "dompurify", // jspdf optional dependency
        "html2canvas" // jspdf optional dependency
    ],
    outdir: 'wwwroot',
    outbase: './src',
    format: 'esm',
    minify: true,
    lineLimit: 1000,
    logLevel: "info",
    sourcemap: false,
    sourceRoot: "https://packages.serenity.is/assets/src/",
    plugins: [writeIfChanged()],
});