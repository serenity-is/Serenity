
export function forAllDescendants(element: Node, action: (node: Node) => void | false,
    filter = NodeFilter.SHOW_ELEMENT | NodeFilter.SHOW_COMMENT | NodeFilter.SHOW_TEXT) {
    const iterator = document.createTreeWalker(element, filter);
    let node: Node;
    while (node = iterator.nextNode()) {
        if (action(node) === false) 
            break;
    }
}