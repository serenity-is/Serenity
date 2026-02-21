import { Attributes, BaseDialog, faIcon, type Dialog } from "@serenity-is/corelib";
import { BarController, BarElement, CategoryScale, Chart, Legend, LinearScale } from "chart.js";
import { SampleInfo } from "../../sample-info";
import { BasicSamplesService } from "../../ServerTypes/Demo";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";
import "./ChartInDialogPage.css";

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
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples, [Attributes.resizable, Attributes.maximizable]);

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
    }
}

function makeModalMaximizable(dialog: Dialog) {
    if (!dialog || dialog.type !== "bsmodal")
        return;

    const header = dialog.getHeaderNode();
    if (!header)
        return;

    header.classList.add("has-maximize-button");

    const closeButton = header?.querySelector<HTMLElement>(".btn-close");

    const maximizeButton = <button type="button" class="btn btn-window-maximize" style={{
        marginLeft: "auto"
    }} onClick={(e) => {
        const dlg = header.closest(".modal-dialog");
        dlg.classList.toggle("modal-fullscreen");
    }}>
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-window-fullscreen maximize-image" viewBox="0 0 16 16" style={{ transform: "scale(-1,1)" }}>
            <path d="M3 3.5a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0m1.5 0a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0m1 .5a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1" />
            <path d="M.5 1a.5.5 0 0 0-.5.5v13a.5.5 0 0 0 .5.5h15a.5.5 0 0 0 .5-.5v-13a.5.5 0 0 0-.5-.5zM1 5V2h14v3zm0 1h14v8H1z" />
        </svg>
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-window-stack restore-image" viewBox="0 0 16 16" style={{ transform: "scale(-1,1)" }}>
            <path d="M4.5 6a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1M6 6a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1m2-.5a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0" />
            <path d="M12 1a2 2 0 0 1 2 2 2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2 2 2 0 0 1-2-2V3a2 2 0 0 1 2-2zM2 12V5a2 2 0 0 1 2-2h9a1 1 0 0 0-1-1H2a1 1 0 0 0-1 1v8a1 1 0 0 0 1 1m1-4v5a1 1 0 0 0 1 1h10a1 1 0 0 0 1-1V8zm12-1V5a1 1 0 0 0-1-1H4a1 1 0 0 0-1 1v2z" />
        </svg>        
    </button>

    if (closeButton)
        header.insertBefore(maximizeButton, closeButton);
    else
        header.appendChild(maximizeButton);
}
