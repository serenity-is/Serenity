declare global {
    interface ImportMeta {
        dirname: string;
    }
}

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