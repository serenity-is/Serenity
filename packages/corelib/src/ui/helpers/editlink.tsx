import { replaceAll } from "../../compat";

/**
 * Renders an edit link for a given item type and ID.
 * The link will have a CSS class based on the item type and will point to a URL fragment
 * that includes the item type and ID.
 * This is similar to SlickHelper.itemLink function, but it doesn't require a grid context
 * and does not accept FormatterResult (e.g. html string markup) as children.
 *
 * @param props - The properties for the edit link.
 * @returns An HTML anchor element representing the edit link.
 */
export function EditLink(props: {
    /**
     * The ID of the item to link to.
     */
    itemId: any,
    /**
     * The type of the item, e.g. "Northwind.Customer".
     */
    itemType?: string,
    /**
     * Additional CSS class to add to the link (besides s-EditLink and s-[ItemType]Link)
     */
    cssClass?: string,
    /**
     * Tab index for the link. Default is null, which means no tabindex attribute.
     */
    tabindex?: number,
    /** @deprecated Use tabindex. */
    tabIndex?: number,
    /**
     * Child elements or text to be displayed inside the link.
     */
    children?: any
}) {
    const cssClass = ['s-EditLink'];
    if (props.itemType) {
        cssClass.push(`s-${replaceAll(props.itemType, '.', '-')}-Link`);
    }
    if (props.cssClass) {
        cssClass.push(props.cssClass);
    }

    return <a class={cssClass}
        href={"#" + replaceAll(props.itemType ?? "", '.', '-') + '/' + (props.itemId ?? "")}
        data-item-type={props.itemType}
        data-item-id={props.itemId}
        tabindex={props.tabindex ?? (props as any).tabIndex}>
        {props.children}
    </a> as HTMLAnchorElement;
}