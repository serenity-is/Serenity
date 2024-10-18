import { mockAdmin, mockDynamicData, mockGridSize } from "test-utils";
import { InlineActionGrid } from "../../Modules/Grids/InlineActionButtons/InlineActionButtonsPage";
import { Fluent, confirmDialog } from "@serenity-is/corelib";
import { CustomerService } from "@serenity-is/demo.northwind";

const orderLoadEntitySpy = jest.fn();

jest.mock("@serenity-is/demo.northwind", () => {
    const northwindActual = jest.requireActual("@serenity-is/demo.northwind");
    return {
        ...northwindActual,
        OrderDialog: class extends northwindActual.OrderDialog {
            constructor(opt?: any) {
                super(opt);
                this.loadEntityAndOpenDialog = orderLoadEntitySpy;
            }
        }
    }
});

jest.mock("@serenity-is/corelib", () => {
    const corelibActual = jest.requireActual("@serenity-is/corelib");
    return {
        ...corelibActual,
        confirmDialog: jest.fn().mockImplementation((msg, onYes) => onYes())
    }
});

beforeAll(() => {
    mockAdmin();
    mockDynamicData();
    mockGridSize();
});

afterEach(() => {
    jest.clearAllMocks();
    Fluent(document.body).empty();
});

function createInlineActionGrid(): InlineActionGrid {
    const div = document.body.appendChild(document.createElement("div"));
    const grid = new InlineActionGrid({ element: div });

    grid.setItems([
        {
            ID: 1357,
            CustomerID: "A1357"
        }, {
            ID: 2468,
            CustomerID: "A2468"
        }
    ]);

    return grid;
}

describe("Inline Action Buttons", () => {
    it("should call edit item when clicking view-details action", () => {
        const grid = createInlineActionGrid();
        const editSpy = (grid as any).editItem = jest.fn();

        var actions = grid.element.findAll<HTMLAnchorElement>(".inline-action[data-action=view-details]");
        expect(actions.length).toBe(2);
        actions[1].click();
        expect(editSpy).toHaveBeenCalledTimes(1);
        expect(editSpy).toHaveBeenLastCalledWith(2468);
        actions[0].click();
        expect(editSpy).toHaveBeenLastCalledWith(1357);
    });

    it("should create a new order dialog when clicking new-order action", () => {
        const grid = createInlineActionGrid();
        var actions = grid.element.findAll<HTMLAnchorElement>(".inline-action[data-action=new-order]");
        expect(actions.length).toBe(2);

        actions[1].click();
        expect(orderLoadEntitySpy).toHaveBeenCalledTimes(1);
        expect(orderLoadEntitySpy).toHaveBeenLastCalledWith({ CustomerID: "A2468" });
        actions[0].click();
        expect(orderLoadEntitySpy).toHaveBeenLastCalledWith({ CustomerID: "A1357" });
    });

    it("should confirm and delete the customer when clicking delete-row action", () => {
        const grid = createInlineActionGrid();
        var actions = grid.element.findAll<HTMLAnchorElement>(".inline-action[data-action=delete-row]");
        expect(actions.length).toBe(2);

        const deleteSpy = jest.spyOn(CustomerService, "Delete").mockImplementation();

        actions[1].click();
        expect(confirmDialog).toHaveBeenCalledTimes(1);
        expect(deleteSpy).toHaveBeenCalledTimes(1);
        expect(deleteSpy).toHaveBeenLastCalledWith({ EntityId: 2468 }, expect.anything());
        actions[0].click();
        expect(confirmDialog).toHaveBeenCalledTimes(2);
        expect(deleteSpy).toHaveBeenLastCalledWith({ EntityId: 1357 }, expect.anything());
    });
});