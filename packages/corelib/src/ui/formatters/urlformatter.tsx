import { Column, FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { formatterTypeInfo, nsSerenity, registerType, resolveUrl, sanitizeUrl, stringFormat } from "../../base";
import { Formatter } from "../../slick";
import { IInitializeColumn } from "./iinitializecolumn";

/** Renders a value as a hyperlink with configurable URL / display mapping. */
export class UrlFormatter implements Formatter, IInitializeColumn {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity, [IInitializeColumn]); static { registerType(this); }

    /**
     * @param props.displayProperty - Item field used for link text (defaults to cell value).
     * @param props.displayFormat - Format string applied to display value.
     * @param props.urlProperty - Item field used for URL (defaults to cell value).
     * @param props.urlFormat - Format string applied to URL value.
     * @param props.target - Anchor target (e.g. `"_blank"`).
     */
    constructor(readonly props: { displayProperty?: string, displayFormat?: string, urlProperty?: string, urlFormat?: string, target?: string } = {}) {
    }

    /** @param ctx - Formatter context. @returns Anchor element or empty string. */
    format(ctx: FormatterContext): FormatterResult {
        var url = (this.urlProperty ?
            (ctx.item[this.urlProperty] ?? '').toString() :
            (ctx.value ?? '').toString());

        if (!url)
            return '';

        if (this.urlFormat)
            url = stringFormat(this.urlFormat, url);

        url = resolveUrl(url);
        url = sanitizeUrl(url);

        var display = (this.displayProperty ?
            (ctx.item[this.displayProperty] ?? '').toString() :
            (ctx.value ?? '').toString());

        if (this.displayFormat)
            display = stringFormat(this.displayFormat, display);

        return <a href={url} target={this.target}>{display}</a>;
    }

    /**
     * Declares any referenced fields so they are fetched for formatting.
     * @param column - Column being initialized.
     */
    initializeColumn(column: Column): void {
        column.referencedFields = column.referencedFields || [];

        if (this.displayProperty) {
            column.referencedFields.push(this.displayProperty);
        }

        if (this.urlProperty && this.urlProperty !== this.displayProperty) {
            column.referencedFields.push(this.urlProperty);
        }
    }

    get displayProperty() { return this.props.displayProperty }
    set displayProperty(value) { this.props.displayProperty = value }

    get displayFormat() { return this.props.displayFormat }
    set displayFormat(value) { this.props.displayFormat = value }

    get urlProperty() { return this.props.urlProperty }
    set urlProperty(value) { this.props.urlProperty = value }

    get urlFormat() { return this.props.urlFormat }
    set urlFormat(value) { this.props.urlFormat = value }

    get target() { return this.props.target }
    set target(value) { this.props.target = value }
}