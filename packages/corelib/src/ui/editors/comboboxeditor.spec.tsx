import { ComboboxSearchQuery, ComboboxSearchResult } from "./combobox";
import { ComboboxEditor } from "./comboboxeditor";

describe("ComboboxEditor async behavior", () => {

    type TestItem = {
        ID: string,
    }

    class ReverseAsyncCombo extends ComboboxEditor<{ filter?: (x: string) => boolean }, TestItem> {
        protected override hasAsyncSource(): boolean {
            return true;
        }

        protected override isMultiple() {
            return true;
        }

        protected override getIdField() {
            return "ID";
        }

        protected asyncSearch(query: ComboboxSearchQuery): PromiseLike<ComboboxSearchResult<TestItem>> {
            if (query.initSelection) {
                return Promise.resolve({
                    items: query.idList.slice().reverse().filter(this.props.filter ?? (() => true)).map(id => ({
                        ID: id,
                        Name: "Name" + id
                    })),
                    more: false
                });
            }

            throw "Not Implemented";
        }
    }

    it("should preserve order for async initSelection", async () => {
        var combo = new ReverseAsyncCombo({
        });
        combo.values = ["3", "2", "1"];
        expect(combo.values).toStrictEqual(["3", "2", "1"]);
        await Promise.resolve();
        expect(combo.values).toStrictEqual(["3", "2", "1"]);
    });

    it("should preserve order when some items not found for async initSelection", async () => {
        jest.useFakeTimers();
        try {
            var combo = new ReverseAsyncCombo({
                filter: x => x !== "2"
            });
            combo.values = ["3", "2", "1"];
            expect(combo.values).toStrictEqual(["3", "2", "1"]);
            await jest.runAllTimersAsync();
            expect(combo.values).toStrictEqual(["3", "1"]);
        }
        finally {
            jest.useRealTimers();
        }
    });
});