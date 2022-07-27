import esbuild from "esbuild";
import { slickCoreBuildOptions, slickGridBuildOptions } from "../../lib/SleekGrid/config/esbuildOptions.mjs";

const src = '../../lib/SleekGrid/src';
const out = '../Serenity.Assets/wwwroot/Scripts/SlickGrid';

for (var esmOpt of [
    { ...slickCoreBuildOptions, entryPoints: [`${src}/core/index.ts`], outfile: `${out}/slick.core.js` },
    { ...slickGridBuildOptions, entryPoints: [`${src}/grid/index.ts`], outfile: `${out}/slick.grid.js` }
]) {
    esbuild.build(esmOpt).catch(() => process.exit());
}