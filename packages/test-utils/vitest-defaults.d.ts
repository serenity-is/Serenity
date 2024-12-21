declare global {
    var __dirname: string;
}

export default function vitestDefaults(opt?: {
    name?: string,
    projectRoot?: string,
    dynamicData?: boolean,
}): any;