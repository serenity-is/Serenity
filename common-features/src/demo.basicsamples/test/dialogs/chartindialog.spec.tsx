import { mockDynamicData } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { BasicSamplesService } from "../../Modules/ServerTypes/Demo";

const { chartSpy } = vi.hoisted(() => ({ chartSpy: vi.fn() }));

vi.mock("chart.js", () => ({
    Chart: class {
        static register() { }
        constructor(...args: any[]) { chartSpy(...args); }
    },
    BarController: class { },
    BarElement: class { },
    CategoryScale: class { },
    Legend: class { },
    LinearScale: class { }
}));

vi.mock(import("../../Modules/Dialogs/ChartInDialog/modal-utils"), async () => ({
    makeModalMaximizable: vi.fn(),
    makeModalDraggable: vi.fn()
}));

import initPage, { ChartInDialog } from "../../Modules/Dialogs/ChartInDialog/ChartInDialogPage";
import { makeModalDraggable, makeModalMaximizable } from "../../Modules/Dialogs/ChartInDialog/modal-utils";

beforeAll(() => {
    mockDynamicData();
});

beforeEach(() => {
    document.body.innerHTML = "";
    chartSpy.mockClear();
});

afterEach(() => {
    vi.restoreAllMocks();
});

describe("ChartInDialogPage", () => {
    it("renders the launch button and opens the dialog on click", () => {
        const panel = document.body.appendChild(document.createElement("div"));
        panel.id = "PanelDiv";
        const open = vi.spyOn(ChartInDialog.prototype, "dialogOpen").mockImplementation((() => ({})) as any);
        initPage();
        const button = panel.querySelector("button");
        expect(button).toBeTruthy();
        button.click();
        expect(open).toHaveBeenCalled();
    });
});

describe("ChartInDialog", () => {
    it("configures dialog options", () => {
        const dialog = new ChartInDialog({});
        const opt = dialog["getDialogOptions"]();
        expect(opt.title).toBe("Orders by Shipper");
        expect(opt.modal).toBe(false);
        expect(opt.backdrop).toBe(true);
        dialog.destroy();
    });

    it("renders a canvas element", () => {
        const dialog = new ChartInDialog({});
        expect(dialog["renderContents"]()).toBeTruthy();
        dialog.destroy();
    });

    it("builds a chart when the dialog opens", () => {
        const dialog = new ChartInDialog({});
        (dialog as any).canvas = document.createElement("canvas");
        const spy = vi.spyOn(BasicSamplesService, "OrdersByShipper").mockImplementation(((request: any, onSuccess: any) => {
            onSuccess({
                Values: [{ Month: "January", Speedy: 3 }, { Month: "February", Speedy: 5 }],
                ShipperKeys: ["Speedy"],
                ShipperLabels: ["Speedy Express"]
            });
            return Promise.resolve({}) as any;
        }) as any);

        dialog["onDialogOpen"]();

        expect(spy).toHaveBeenCalledWith({}, expect.any(Function));
        expect(chartSpy).toHaveBeenCalledWith(expect.any(HTMLCanvasElement), expect.objectContaining({
            type: "bar",
            data: expect.objectContaining({
                labels: ["January", "February"],
                datasets: [expect.objectContaining({ label: "Speedy Express", data: [3, 5] })]
            })
        }));
        dialog.destroy();
    });

    it("makes the dialog maximizable and draggable", () => {
        const dialog = new ChartInDialog({});
        dialog["initDialog"]();
        expect(makeModalMaximizable).toHaveBeenCalled();
        expect(makeModalDraggable).toHaveBeenCalled();
        dialog.destroy();
    });
});
