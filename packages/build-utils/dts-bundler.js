import { generateDtsBundle } from "dts-bundle-generator";
import getCompilerOptMod from "dts-bundle-generator/dist/get-compiler-options.js";
import diagErrorsMod from "dts-bundle-generator/dist/helpers/check-diagnostics-errors.js";
import measureTimeMod from "dts-bundle-generator/dist/helpers/measure-time.js";
import loggerMod from "dts-bundle-generator/dist/logger.js";
import ts from "typescript";

export function dtsBundle(entries, options) {

     function run() {

        entries = (entries || [{}]).map((e, i) => ({
            filePath: i === 0 ? './src/index.ts' : null,
            outFile: i === 0 ? './dist/index.d.ts' : null,
            ...e,
            output: {
                noBanner: true,
                ...e.output
            }
        }));

        options = {
            followSymlinks: false,
            preferredConfigPath: "./tsconfig.json",
            ...options
        }
        if (options.verbose || false)
            loggerMod.enableVerboseLog();
        else if (options.silent || false)
            loggerMod.enableSilentLog();
        else 
            loggerMod.enableNormalLog();
        delete options.verbose;
        delete options.silent;

        function generateOutFileName(inputFilePath) {
            const inputFileName = path.parse(inputFilePath).name;
            return fixPath(path.join(inputFilePath, '..', inputFileName + '.d.ts'));
        }

        loggerMod.verboseLog(`Total entries count=${entries.length}`);
        const generatedDts = generateDtsBundle(entries, options);
        const outFilesToCheck = [];
        for (let i = 0; i < entries.length; ++i) {
            const entry = entries[i];
            const outFile = entry.outFile !== undefined ? entry.outFile : generateOutFileName(entry.filePath);
            loggerMod.normalLog(`Writing ${entry.filePath} -> ${outFile}`);
            ts.sys.writeFile(outFile, generatedDts[i]);
            if (!entry.noCheck) {
                outFilesToCheck.push(outFile);
            }
        }
        if (outFilesToCheck.length === 0) {
            loggerMod.normalLog('File checking is skipped (due nothing to check)');
            return;
        }
        loggerMod.normalLog('Checking generated files...');
        const preferredConfigFile = options?.preferredConfigPath;
        const compilerOptions = getCompilerOptMod.getCompilerOptions(outFilesToCheck, preferredConfigFile);
        if (compilerOptions.skipLibCheck) {
            compilerOptions.skipLibCheck = false;
            loggerMod.warnLog('Compiler option "skipLibCheck" is disabled to properly check generated output');
        }
        // we want to turn this option on because in this case the compile will generate declaration diagnostics out of the box
        compilerOptions.declaration = true;
        let checkFailed = false;
        for (const outputFile of outFilesToCheck) {
            const program = ts.createProgram([outputFile], compilerOptions);
            try {
                diagErrorsMod.checkProgramDiagnosticsErrors(program);
            }
            catch (e) {
                loggerMod.errorLog(`Error in file ${outputFile}: ${e.message}`);
                checkFailed = true;
            }
        }
        if (checkFailed) {
            throw new Error('Failed to check some of generated bundles, check error messages above');
        }
    }

    try {
        const executionTime = measureTimeMod.measureTime(run);
        loggerMod.normalLog(`Done in ${(executionTime / 1000).toFixed(2)}s`);
    }
    catch (ex) {
        loggerMod.normalLog('');
        loggerMod.errorLog(`Error: ${ex.message}`);
        process.exit(1);
    }
}

function fixPath(path) {
    return path.replace(/\\/g, '/');
}

