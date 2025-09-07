import { replaceAll } from "../../compat";


export function EditLink(props: {
    /**
     * The ID of the item to link to.
     */
    id: any,
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
        href={props.id != null ? "#" + replaceAll(props.itemType ?? "", '.', '-') + '/' + props.id : ''}
        data-item-type={props.itemType}
        data-item-id={props.id}
        tabIndex={props.tabIndex}>
        {props.children}
    </a> as HTMLAnchorElement;
}