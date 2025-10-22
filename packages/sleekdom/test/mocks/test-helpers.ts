
export function forAllDescendants(element: Node, action: (node: Node) => void | false,
    filter = NodeFilter.SHOW_ALL | NodeFilter.SHOW_COMMENT) {
    const iterator = document.createTreeWalker(element, filter);
    let node: Node;
    while (node = iterator.nextNode()) {
        if (action(node) === false) 
            break;
    }
}