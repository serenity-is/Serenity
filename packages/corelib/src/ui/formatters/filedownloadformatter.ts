import { Column, FormatterContext } from "@serenity-is/sleekgrid";
import { faIcon, formatterTypeInfo, htmlEncode, iconClassName, registerType, resolveUrl, stringFormat } from "../../base";
import { replaceAll } from "../../compat";
import { Formatter } from "../../slick";
import { IInitializeColumn } from "./iinitializecolumn";

export class FileDownloadFormatter implements Formatter, IInitializeColumn {
    static typeInfo = formatterTypeInfo("Serenity.IFileDownloadFormatter", [IInitializeColumn]); static { registerType(this); }

    constructor(public readonly props: { displayFormat?: string, originalNameProperty?: string, iconClass?: string } = {}) {
        this.props ??= {};
    }

    format(ctx: FormatterContext): string {
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

        return "<a class='file-download-link' target='_blank' href='" +
            htmlEncode(downloadUrl) + "'><i class='" + iconClass + "'></i> " + htmlEncode(text) + '</a>';
    }

    static dbFileUrl(filename: string): string {
        filename = replaceAll((filename ?? ''), '\\', '/');
        return resolveUrl('~/upload/') + filename;
    }

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