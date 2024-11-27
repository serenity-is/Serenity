import { ServiceResponse, PropertyItem } from "@serenity-is/corelib";

export interface ReportRetrieveResult extends ServiceResponse {
    ReportKey?: string;
    Title?: string;
    Properties?: PropertyItem[];
    InitialSettings?: any;
    IsDataOnlyReport?: boolean;
    IsExternalReport?: boolean;
}