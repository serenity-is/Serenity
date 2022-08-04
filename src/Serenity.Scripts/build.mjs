import esbuild from "esbuild";
import { compatCore, compatGrid } from "@serenity-is/sleekgrid/build/defines.js";

const src = 'node_modules/@serenity-is/sleekgrid/src';
const out = '../Serenity.Assets/wwwroot/Scripts/SlickGrid';

for (var esmOpt of [
    { ...compatCore, entryPoints: [`${src}/core/index.ts`], outfile: `${out}/slick.core.js`, sourcemap: false },
    { ...compatGrid, entryPoints: [`${src}/grid/index.ts`], outfile: `${out}/slick.grid.js`, sourcemap: false }
]) {
    esbuild.build(esmOpt).catch(() => process.exit());
}