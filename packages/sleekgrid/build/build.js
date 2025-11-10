
import esbuild from "esbuild";
import { cpSync, existsSync, mkdirSync, readFileSync, writeFileSync } from "fs";
import { dirname, join, resolve } from "path";
import process from 'process';
import { fileURLToPath } from 'url';

export function globalExternals(filter, externals) {
    return {
        name: "global-externals",
        setup(build) {
            build.onResolve({ filter }, (args) => {
                return {
                    path: '_',
                    namespace: 'global-externals'
                };
            });

            build.onLoad(
                {
                    filter: /^\_$/,
                    namespace: "global-externals",
                },
                async (args) => {
                    return {
                        contents: Object.keys(externals).map(k => `const { ${externals[k].join(', ')} } = ${k};`).join('\n') +
                            'export {\n ' + Object.keys(externals).map(k => '    ' + externals[k].join(', ')).join(',\n') + '\n};',
                        loader: "js"
                    };
                }
            );
        },
    };
}

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

const compatDefaults = {
    bundle: true,
    target: 'es6',
    format: 'iife',
    globalName: 'Slick._',
    color: true,
    logLevel: 'info',
    sourcemap: true,
    footer: {
        js: '["Data", "Editors", "Formatters", "Plugins"].forEach(ns => Slick._[ns] && (Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns])) && delete Slick._[ns]); Object.assign(Slick, Slick._); delete Slick._;'
    }
}

const compatCore = {
    ...compatDefaults,
    entryPoints: ['./src/core/index.ts'],
    outfile: './dist/compat/slick.core.js',
    footer: {
        js: compatDefaults.footer.js + " Slick.Event = Slick.EventEmitter; Slick.EventHandler = Slick.EventSubscriber; Slick.Range = Slick.CellRange; typeof Map !== 'undefined' && (Slick.Map = Map);"
    }
}

const compatGrid = {
    ...compatDefaults,
    entryPoints: ['./src/grid/index.ts'],
    outfile: './dist/compat/slick.grid.js',
    plugins: [globalExternals(/\.\.\/core/, {
        Slick: ["addClass", "applyFormatterResultToCellNode", "basicRegexSanitizer", "columnDefaults", "convertCompatFormatter", "ensureUniqueColumnIds", "escapeHtml", "defaultColumnFormat", "disableSelection", "Draggable", "EventEmitter", "EventData", "formatterContext", "gridDefaults", "GlobalEditorLock", "initColumnProps", "keyCode", "NonDataRow", "parsePx", "preClickClassName", "CellRange", "removeClass", "RowCell", "spacerDiv", "titleize"]
    })]
}

const compatFormatters = {
    ...compatDefaults,
    entryPoints: ['./src/formatters/index.ts'],
    outfile: './dist/compat/slick.formatters.js',
    plugins: [globalExternals(/\.\.\/core/, {
        Slick: [ "escapeHtml", "formatterContext" ]
    })]
}

const compatEditors = {
    ...compatDefaults,
    entryPoints: ['./src/editors/index.ts'],
    outfile: './dist/compat/slick.editors.js',
    plugins: [globalExternals(/\.\.\/core/, {
        Slick: [ "escapeHtml", "H", "keyCode", "parsePx" ]
    })]
}

const compatPluginsAutoTooltips = {
    ...compatDefaults,
    entryPoints: ['./src/plugins/autotooltips.ts'],
    outfile: './dist/compat/plugins/slick.autotooltips.js',
    plugins: [globalExternals(/\.\.\/(core|grid)/, {
        Slick: []
    })]
}

const compatPluginsRowMoveManager = {
    ...compatDefaults,
    entryPoints: ['./src/plugins/rowmovemanager.ts'],
    outfile: './dist/compat/plugins/slick.rowmovemanager.js',
    plugins: [globalExternals(/\.\.\/(core|grid)/, {
        Slick: ["EventEmitter", "EventSubscriber", "H"]
    })]
}

const compatPluginsRowSelectionModel = {
    ...compatDefaults,
    entryPoints: ['./src/plugins/rowselectionmodel.ts'],
    outfile: './dist/compat/plugins/slick.rowselectionmodel.js',
    plugins: [globalExternals(/\.\.\/(core|grid)/, {
        Slick: ["EventEmitter", "EventSubscriber", "CellRange"]
    })]
}

const compatDataGroupItemMetadataProvider = {
    ...compatDefaults,
    entryPoints: ['./src/data/groupitemmetadataprovider.tsx'],
    outfile: './dist/compat/slick.groupitemmetadataprovider.js',
    plugins: [globalExternals(/\.\.\/(core|grid)/, {
        Slick: [ "convertCompatFormatter", "Group", "applyFormatterResultToCellNode" ]
    })],
    footer: {
        js: compatDefaults.footer.js + " Slick.Data = Slick.Data || {}; Slick.Data.GroupItemMetadataProvider = Slick.GroupItemMetadataProvider;"
    }
}

const sleekDefaults = {
    bundle: true,
    entryPoints: ['./src/index.ts'],
    color: true,
    logLevel: 'info',
    target: 'es6',
    sourcemap: true
}

const sleekIndex = {
    ...sleekDefaults,
    format: 'esm',
    minify: true,
    outfile: './dist/index.js'
}

const sleekGlobal = {
    ...sleekDefaults,
    globalName: compatCore.globalName,
    format: 'iife',
    footer: compatCore.footer,
    outfile: './wwwroot/index.global.js',
    plugins: [
        writeIfChanged()
    ]
}

var buildList = [];

if (process.argv.includes('--compat') ||
    process.argv.includes('--full')) {
    buildList.push(
        compatCore,
        compatGrid,
        compatFormatters,
        compatEditors,
        compatDataGroupItemMetadataProvider,
        compatPluginsAutoTooltips,
        compatPluginsRowMoveManager,
        compatPluginsRowSelectionModel
    );
}

buildList.push(
    sleekIndex,
    sleekGlobal
)

for (var esmOpt of buildList) {
    await esbuild.build({
        ...esmOpt,
    }).catch(() => process.exit());

    if (!esmOpt.minify) {
        await esbuild.build({
            ...esmOpt,
            minify: true,
            outfile: esmOpt.outfile.replace(/\.js/, '.min.js')
        }).catch(() => process.exit());
    }
}

const root = resolve(join(fileURLToPath(new URL('.', import.meta.url)), '../'));
if (existsSync(join(root, "docs/_config.yml"))) {
    const target = join(root, 'docs/assets/local');
    !existsSync(target) && mkdirSync(target);
    existsSync(join(root, 'css')) && cpSync(join(root, 'css'), join(target, 'css'), { force: true, recursive: true });
    existsSync(join(root, 'dist')) && cpSync(join(root, 'dist'), join(target, 'dist'), { force: true, recursive: true });
    existsSync(join(root, 'lib')) && cpSync(join(root, 'lib'), join(target, 'lib'), { force: true, recursive: true });
}
