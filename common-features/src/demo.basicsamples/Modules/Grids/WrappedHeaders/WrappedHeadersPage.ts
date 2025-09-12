import { gridPageInit } from "@serenity-is/corelib";
import { OrderGrid } from "@serenity-is/demo.northwind";
import "./WrappedHeadersPage.css";

export default () => gridPageInit(WrappedHeadersGrid)

export class WrappedHeadersGrid extends OrderGrid {
    static override typeInfo = this.registerClass("Serenity.Demo.BasicSamples.WrappedHeadersGrid");
}