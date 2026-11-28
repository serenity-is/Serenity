// import.meta.dirname and import.meta.filename, for the vitest configs
import "./node";

export function serenityTestOptions(projectRoot?: string): {
    fsModuleCachePath: string,
    testTimeout: number,
    hookTimeout: number,
};

export default function vitestDefaults(opt?: {
    name?: string,
    projectRoot?: string,
    dynamicData?: boolean,
}): any;