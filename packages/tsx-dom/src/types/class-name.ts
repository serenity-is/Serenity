export type ClassNameRecord = Record<string, null | undefined | boolean>;
export type ClassNameEntry = string | null | undefined | false | ClassNameRecord;
export type ClassNameType = ClassNameEntry | ClassNameEntry[];

