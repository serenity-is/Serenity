import { ServiceRequest } from "@serenity-is/corelib";

export interface ExcelImportRequest extends ServiceRequest {
    FileName?: string;
}