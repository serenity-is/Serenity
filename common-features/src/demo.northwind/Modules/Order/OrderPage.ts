import { gridPageInit, parseQueryString } from "@serenity-is/corelib";
import { OrderGrid } from "./OrderGrid";

export default () => {
    const orderGrid = gridPageInit(OrderGrid);
    const q = parseQueryString() as any;
    if (q.shippingState?.length) {
        orderGrid.set_shippingState(parseInt(q.shippingState, 10));
    }
}