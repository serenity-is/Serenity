import { gridPageInit } from "@serenity-is/corelib";
import { OrderGrid } from "@serenity-is/demo.northwind";
import "./WrappedHeadersPage.css";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";

export default () => gridPageInit(WrappedHeadersGrid)

export class WrappedHeadersGrid extends OrderGrid {
    static [Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);
}