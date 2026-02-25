import { Attributes, BaseDialog } from "@serenity-is/corelib";
import { BarController, BarElement, CategoryScale, Chart, Legend, LinearScale } from "chart.js";
import { SampleInfo } from "../../sample-info";
import { BasicSamplesService } from "../../ServerTypes/Demo";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";
import "./ChartInDialogPage.css";
import { makeModalDraggable, makeModalMaximizable } from "./modal-utils";

Chart.register(BarController, BarElement, CategoryScale, Legend, LinearScale);

const chartColors = ['#4E79A7', '#A0CBE8', '#F28E2B', '#FFBE7D', '#59A14F', '#8CD17D', '#B6992D', '#F1CE63', '#499894', '#86BCB6',
    '#E15759', '#FF9D9A', '#79706E', '#BAB0AC', '#D37295', '#FABFD2', '#B07AA1', '#D4A6C8', '#9D7660', '#D7B5A6'];

export default () => {
    let buttonClick = () => new ChartInDialog().dialogOpen();

    document.getElementById("PanelDiv").append(
        <button class="btn btn-block btn-primary" onClick={buttonClick}>Launch Dialog</button>);

    return <SampleInfo sources={[".css"]}>
        <p>This sample demonstrates showing a resizable chart in a dialog. Chart data is populated from Northwind Orders with a service call.</p>
        <p>Click button below to launch the dialog.</p>
    </SampleInfo>
}

export class ChartInDialog<P = {}> extends BaseDialog<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples, [Attributes.maximizable]);

    declare private canvas: HTMLCanvasElement;

    protected override onDialogOpen() {
        super.onDialogOpen();

        BasicSamplesService.OrdersByShipper({}, response => {
            new Chart(this.canvas, {
                type: "bar",
                data: {
                    labels: response.Values.map(x => x.Month),
                    datasets: response.ShipperKeys.map((shipperKey, shipperIdx) => ({
                        label: response.ShipperLabels[shipperIdx],
                        backgroundColor: chartColors[shipperIdx % chartColors.length],
                        data: response.Values.map((x, ix) => response.Values[ix][shipperKey])
                    }))
                }
            });
        });
    }

    protected override renderContents(): any {
        return (<canvas id={`${this.idPrefix}Chart`} ref={el => this.canvas = el}></canvas>);
    }

    protected override getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.title = 'Orders by Shipper';
        opt.modal = false;
        opt.backdrop = true;
        return opt;
    }

    protected override initDialog() {
        super.initDialog();

        makeModalMaximizable(this.dialog);
        makeModalDraggable(this.dialog);
    }
}
