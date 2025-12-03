import { copyFileSync, constants, existsSync, mkdirSync, readFileSync } from 'node:fs';
import { dirname } from 'node:path';

async function copyFileIfChanged(srcFile, dstfile) {
    if (existsSync(dstfile)) {
        const srcContent = readFileSync(srcFile);
        if ((readFileSync(dstfile)).equals(srcContent))
            return;
    }
    else {
        mkdirSync(dirname(dstfile), { recursive: true });
    }

    copyFileSync(srcFile, dstfile, constants.COPYFILE_FICLONE);
    console.log(`Copied: ${srcFile} to ${dstfile}.`);
}

await copyFileIfChanged("node_modules/jquery/dist/jquery.js", "wwwroot/jquery/jquery.js");
await copyFileIfChanged("node_modules/jquery/dist/jquery.min.js", "wwwroot/jquery/jquery.min.js");
