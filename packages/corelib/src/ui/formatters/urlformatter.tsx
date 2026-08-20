import { Column, FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { formatterTypeInfo, nsSerenity, registerType, resolveUrl, sanitizeUrl, stringFormat } from "../../base";
import { Formatter } from "../../slick";
import { IInitializeColumn } from "./iinitializecolumn";

/** Renders a value as a hyperlink with configurable URL / display mapping. */
export class UrlFormatter implements Formatter, IInitializeColumn {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity, [IInitializeColumn]); static { registerType(this); }

    /**
     * Creates a new UrlFormatter.
     * @param props - Formatter options.
     * @param props.displayProperty - Item field used for link text (defaults to cell value).
     * @param props.displayFormat - Format string applied to display value.
     * @param props.urlProperty - Item field used for URL (defaults to cell value).
     * @param props.urlFormat - Format string applied to URL value.
     * @param props.target - Anchor target (e.g. `"_blank"`).
     */
    constructor(readonly props: { displayProperty?: string, displayFormat?: string, urlProperty?: string, urlFormat?: string, target?: string } = {}) {
    }

    /**
     * Formats the cell value as a hyperlink.
     * @param ctx - Formatter context containing the cell value and row item.
     * @returns Anchor element markup or an empty string if the URL is empty.
     */
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

    /** Gets the field used for link text. @returns The display property name. */
    get displayProperty() { return this.props.displayProperty }
    /**
     * Sets the field used for link text.
     * @param value - The display property name.
     */
    set displayProperty(value) { this.props.displayProperty = value }

    /** Gets the format string applied to the display value. @returns The display format. */
    get displayFormat() { return this.props.displayFormat }
    /**
     * Sets the format string applied to the display value.
     * @param value - The display format string.
     */
    set displayFormat(value) { this.props.displayFormat = value }

    /** Gets the field used for the URL. @returns The URL property name. */
    get urlProperty() { return this.props.urlProperty }
    /**
     * Sets the field used for the URL.
     * @param value - The URL property name.
     */
    set urlProperty(value) { this.props.urlProperty = value }

    /** Gets the format string applied to the URL value. @returns The URL format. */
    get urlFormat() { return this.props.urlFormat }
    /**
     * Sets the format string applied to the URL value.
     * @param value - The URL format string.
     */
    set urlFormat(value) { this.props.urlFormat = value }

    /** Gets the anchor target (e.g. `"_blank"`). @returns The target value. */
    get target() { return this.props.target }
    /**
     * Sets the anchor target.
     * @param value - The target value (e.g. `"_blank"`).
     */
    set target(value) { this.props.target = value }
}