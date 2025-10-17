import { EntryPointConfig, CompilationOptions } from "dts-bundle-generator";

export async function dtsBundle(entries: EntryPointConfig[], options: CompilationOptions);
export function writeIfChanged();
