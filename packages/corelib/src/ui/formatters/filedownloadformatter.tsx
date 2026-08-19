import { Column, FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { faIcon, formatterTypeInfo, iconClassName, nsSerenity, registerType, resolveUrl, stringFormat } from "../../base";
import { replaceAll } from "../../compat";
import { Formatter } from "../../slick";
import { IInitializeColumn } from "./iinitializecolumn";

/** Renders a DB file path as a download link with an icon and optional original name. */
export class FileDownloadFormatter implements Formatter, IInitializeColumn {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity, [IInitializeColumn]); static { registerType(this); }

    /**
     * @param props.displayFormat - Format string for link text (default `"{0}"`).
     * @param props.originalNameProperty - Field holding the original file name.
     * @param props.iconClass - Icon class for the download icon.
     */
    constructor(public readonly props: { displayFormat?: string, originalNameProperty?: string, iconClass?: string } = {}) {
        this.props ??= {};
    }

    /** @param ctx - Formatter context. @returns Anchor element for the file. */
    format(ctx: FormatterContext): FormatterResult {
        var dbFile = ctx.value as string;
        if (!dbFile)
            return '';

        var downloadUrl = FileDownloadFormatter.dbFileUrl(dbFile);
        var originalName = this.originalNameProperty ?
            ctx.item[this.originalNameProperty] as string : null;

        originalName = (originalName ?? '');
        var text = stringFormat((this.displayFormat ?? '{0}'),
            originalName, dbFile, downloadUrl);

        var iconClass = iconClassName(this.iconClass ?? faIcon("download"));

        return <a class="file-download-link" target="_blank" href={downloadUrl}><i class={iconClass}></i> {text}</a>;
    }

    /**
     * Builds the download URL for a temp/upload file.
     * @param filename - Stored file path.
     * @returns Resolved URL under `~/upload/`.
     */
    static dbFileUrl(filename: string): string {
        filename = replaceAll((filename ?? ''), '\\', '/');
        return resolveUrl('~/upload/') + filename;
    }

    /**
     * Declares `originalNameProperty` as a referenced field so it is fetched for formatting.
     * @param column - Column being initialized.
     */
    initializeColumn(column: Column): void {
        column.referencedFields = column.referencedFields || [];
        if (this.originalNameProperty) {
            column.referencedFields.push(this.originalNameProperty);
            return;
        }
    }

    get displayFormat() { return this.props.displayFormat; }
    set displayFormat(value) { this.props.displayFormat = value; }

    get originalNameProperty() { return this.props.originalNameProperty; }
    set originalNameProperty(value) { this.props.originalNameProperty = value; }

    get iconClass() { return this.props.iconClass; }
    set iconClass(value) { this.props.iconClass = value; }
}