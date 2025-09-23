import { proxyTexts } from "../localtext";

namespace servicesTexts {
    export declare namespace Controls {
        export function asKey(): typeof Controls;
        export function asTry(): typeof Controls;
        namespace ImageUpload {
            export function asKey(): typeof ImageUpload;
            export function asTry(): typeof ImageUpload;
            export const AddFileButton: string;
            export const ColorboxClose: string;
            export const ColorboxCurrent: string;
            export const ColorboxNext: string;
            export const ColorboxPrior: string;
            export const DeleteButtonHint: string;
            export const ExtensionBlacklisted: string;
            export const ExtensionNotAllowed: string;
            export const FailedScan: string;
            export const ImageExtensionMismatch: string;
            export const InfectedFile: string;
            export const InfectedFileOrError: string;
            export const MaxHeight: string;
            export const MaxWidth: string;
            export const MinHeight: string;
            export const MinWidth: string;
            export const NotAnImageFile: string;
            export const NotAnImageWithExtensions: string;
            export const UploadFileTooBig: string;
            export const UploadFileTooSmall: string;
        }
    }
    export declare namespace Enums {
        export function asKey(): typeof Enums;
        export function asTry(): typeof Enums;
        namespace ImageCheckResult {
            export function asKey(): typeof ImageCheckResult;
            export function asTry(): typeof ImageCheckResult;
            export const DataSizeTooHigh: string;
            export const HeightMismatch: string;
            export const HeightTooHigh: string;
            export const HeightTooLow: string;
            export const ImageIsEmpty: string;
            export const InvalidImage: string;
            export const SizeMismatch: string;
            export const StreamReadError: string;
            export const UnsupportedFormat: string;
            export const WidthMismatch: string;
            export const WidthTooHigh: string;
            export const WidthTooLow: string;
        }
    }

    export declare namespace Validation {
        export function asKey(): typeof Validation;
        export function asTry(): typeof Validation;
        export const ArgumentIsNull: string;
        export const ArgumentOutOfRange: string;
        export const EntityForeignKeyViolation: string;
        export const EntityHasDeletedParent: string;
        export const EntityIsNotActive: string;
        export const EntityNotFound: string;
        export const EntityReadAccessViolation: string;
        export const EntityWriteAccessViolation: string;
        export const FieldInvalidDateRange: string;
        export const FieldInvalidValue: string;
        export const FieldIsReadOnly: string;
        export const FieldIsRequired: string;
        export const Recaptcha: string;
        export const RequestIsNull: string;
        export const UnexpectedError: string;
    }
}

const textsProxy: typeof servicesTexts = proxyTexts({}, '', {
    Controls: {
        ImageUpload: {}
    },
    Enums: {
        ImageCheckResult: {}
    },
    Validation: {}
}) as any;

export const DataValidationTexts = textsProxy.Validation;
export const FileUploadTexts = textsProxy.Controls.ImageUpload;
export const ImageCheckResultTexts = textsProxy.Enums.ImageCheckResult;