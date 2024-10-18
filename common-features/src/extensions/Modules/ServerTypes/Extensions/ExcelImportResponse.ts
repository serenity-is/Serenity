import { ServiceResponse } from "@serenity-is/corelib";

export interface ExcelImportResponse extends ServiceResponse {
    Inserted?: number;
    Updated?: number;
    ErrorList?: string[];
}