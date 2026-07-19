import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { Fluent, PropertyItem } from "../../base";
import { EntityLocalizer, EntityLocalizerOptions } from "./entitylocalizer";
// Import StringEditor to register it in the editor registry, needed by PropertyGrid
import "../editors/stringeditor";

function createMockOptions(overrides?: Partial<EntityLocalizerOptions>): EntityLocalizerOptions {
    const pgDiv = document.createElement("div");
    const button = document.createElement("button");
    const fluentButton = Fluent(button);

    return {
        byId: vi.fn((id: string) => Fluent(document.createElement("input"))),
        idPrefix: "Test_",
        isNew: vi.fn(() => true),
        getButton: vi.fn(() => fluentButton),
        getEntity: vi.fn(() => ({})),
        getLanguages: vi.fn(() => []),
        getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
        getToolButtons: vi.fn(() => []),
        pgOptions: {
            idPrefix: "Test_",
            items: [],
            mode: 0 as any,
            localTextPrefix: "Forms.Test."
        },
        retrieveLocalizations: vi.fn(async () => ({})),
        validateForm: vi.fn(() => true),
        ...overrides
    };
}

describe("EntityLocalizer", () => {
    beforeEach(() => {
        document.body.innerHTML = "";
    });

    afterEach(() => {
        document.body.innerHTML = "";
    });

    describe("constructor", () => {
        it("creates instance without localization grid when no localizable items", () => {
            const opt = createMockOptions();
            const localizer = new EntityLocalizer(opt);
            expect(localizer).toBeInstanceOf(EntityLocalizer);
            expect(localizer.isEnabled()).toBe(false);
        });

        it("creates instance with localization grid when localizable items exist", () => {
            const pgDiv = document.createElement("div");
            pgDiv.id = "pg";
            
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true },
                { name: "Description", title: "Description" }
            ];

            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                pgOptions: {
                    idPrefix: "Test_",
                    items,
                    mode: 0 as any,
                    localTextPrefix: "Forms.Test."
                }
            });

            const localizer = new EntityLocalizer(opt);
            expect(localizer.isEnabled()).toBe(true);
        });

        it("does not create grid when pgDiv has no node", () => {
            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => null as any)
            });
            const localizer = new EntityLocalizer(opt);
            expect(localizer.isEnabled()).toBe(false);
        });

        it("does not create grid when no items have localizable flag", () => {
            const pgDiv = document.createElement("div");
            const items: PropertyItem[] = [
                { name: "Name", title: "Name" },
                { name: "Description", title: "Description" }
            ];

            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                pgOptions: {
                    idPrefix: "Test_",
                    items,
                    mode: 0 as any,
                    localTextPrefix: "Forms.Test."
                }
            });

            const localizer = new EntityLocalizer(opt);
            expect(localizer.isEnabled()).toBe(false);
        });

        it("handles languages list and creates target language selector", () => {
            const pgDiv = document.createElement("div");
            
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true }
            ];

            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                getLanguages: vi.fn(() => [{ id: "en", text: "English" }, { id: "tr", text: "Turkish" }]),
                pgOptions: {
                    idPrefix: "Test_",
                    items,
                    mode: 0 as any,
                    localTextPrefix: "Forms.Test."
                }
            });

            const localizer = new EntityLocalizer(opt);
            expect(localizer.isEnabled()).toBe(true);
        });
    });

    describe("destroy", () => {
        it("destroys the grid and sets to null", () => {
            const pgDiv = document.createElement("div");
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true }
            ];

            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                getLanguages: vi.fn(() => [{ id: "en", text: "English" }]),
                pgOptions: {
                    idPrefix: "Test_",
                    items,
                    mode: 0 as any,
                    localTextPrefix: "Forms.Test."
                }
            });

            const localizer = new EntityLocalizer(opt);
            const destroySpy = vi.fn();
            localizer["grid"] = { destroy: destroySpy } as any;
            
            localizer.destroy();
            expect(destroySpy).toHaveBeenCalled();
            expect(localizer["grid"]).toBeNull();
        });
    });

    describe("clearValue", () => {
        it("clears pending and last values", () => {
            const opt = createMockOptions();
            const localizer = new EntityLocalizer(opt);
            localizer["pendingValue"] = { some: "value" };
            localizer["lastValue"] = { some: "value" };
            
            localizer.clearValue();
            expect(localizer["pendingValue"]).toBeNull();
            expect(localizer["lastValue"]).toBeNull();
        });
    });

    describe("isEnabled", () => {
        it("returns false when grid is not initialized", () => {
            const opt = createMockOptions();
            const localizer = new EntityLocalizer(opt);
            expect(localizer.isEnabled()).toBe(false);
        });
    });

    describe("buttonClick", () => {
        it("handles click when not in localization mode", () => {
            const pgDiv = document.createElement("div");
            const button = document.createElement("button");
            
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true }
            ];

            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                getButton: vi.fn(() => Fluent(button)),
                getLanguages: vi.fn(() => []),
                pgOptions: {
                    idPrefix: "Test_",
                    items,
                    mode: 0 as any,
                    localTextPrefix: "Forms.Test."
                }
            });

            const localizer = new EntityLocalizer(opt);
            // Should not throw
            expect(() => localizer.buttonClick()).not.toThrow();
        });

        it("returns early when validateForm fails in localization mode", () => {
            const pgDiv = document.createElement("div");
            const button = document.createElement("button");
            button.classList.add("pressed"); // Already in localization mode
            
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true }
            ];

            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                getButton: vi.fn(() => Fluent(button)),
                getLanguages: vi.fn(() => []),
                pgOptions: {
                    idPrefix: "Test_",
                    items,
                    mode: 0 as any,
                    localTextPrefix: "Forms.Test."
                },
                validateForm: vi.fn(() => false)
            });

            const localizer = new EntityLocalizer(opt);
            expect(() => localizer.buttonClick()).not.toThrow();
        });
    });

    describe("editSaveRequest", () => {
        it("adds localizations when pendingValue is set", () => {
            const opt = createMockOptions({
                getLanguages: vi.fn(() => [{ id: "en", text: "English" }])
            });
            const localizer = new EntityLocalizer(opt);
            
            localizer["pendingValue"] = { "en$Name": "English Name" };
            
            const req: any = {};
            localizer.editSaveRequest(req);
            expect(req.Localizations).toBeDefined();
            expect(req.Localizations.en).toBeDefined();
            expect(req.Localizations.en.Name).toBe("English Name");
        });

        it("does not modify request when pendingValue is null", () => {
            const opt = createMockOptions();
            const localizer = new EntityLocalizer(opt);
            
            localizer["pendingValue"] = null;
            
            const req: any = { Entity: {} };
            localizer.editSaveRequest(req);
            expect(req.Localizations).toBeUndefined();
        });
    });

    describe("updateInterface", () => {
        it("returns early when not enabled", () => {
            const opt = createMockOptions();
            const localizer = new EntityLocalizer(opt);
            
            expect(() => localizer.updateInterface()).not.toThrow();
        });
    });

    describe("getPendingLocalizations", () => {
        it("returns null when pendingValue is null", () => {
            const opt = createMockOptions({
                getLanguages: vi.fn(() => [{ id: "en", text: "English" }, { id: "tr", text: "Turkish" }])
            });
            const localizer = new EntityLocalizer(opt);
            localizer["pendingValue"] = null;
            expect((localizer as any).getPendingLocalizations()).toBeNull();
        });

        it("splits pendingValue by language prefix", () => {
            const opt = createMockOptions({
                getLanguages: vi.fn(() => [{ id: "en", text: "English" }, { id: "tr", text: "Turkish" }])
            });
            const localizer = new EntityLocalizer(opt);
            localizer["pendingValue"] = {
                "en$Name": "English Name",
                "en$Description": "English Desc",
                "tr$Name": "Turkish Name",
                "NonLocalized": "Should be ignored"
            };
            const result = (localizer as any).getPendingLocalizations();
            expect(result.en).toEqual({ Name: "English Name", Description: "English Desc" });
            expect(result.tr).toEqual({ Name: "Turkish Name" });
        });
    });

    describe("isLocalizationMode", () => {
        function createEnabledLocalizer() {
            const pgDiv = document.createElement("div");
            const button = document.createElement("button");
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true }
            ];
            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                getButton: vi.fn(() => Fluent(button)),
                getLanguages: vi.fn(() => []),
                pgOptions: { idPrefix: "Test_", items, mode: 0 as any, localTextPrefix: "Forms.Test." }
            });
            const localizer = new EntityLocalizer(opt);
            return { localizer, button };
        }

        it("returns true when button has pressed class", () => {
            const { localizer, button } = createEnabledLocalizer();
            button.classList.add("pressed");
            expect((localizer as any).isLocalizationMode()).toBe(true);
        });

        it("returns false when button does not have pressed class", () => {
            const { localizer } = createEnabledLocalizer();
            expect((localizer as any).isLocalizationMode()).toBe(false);
        });
    });

    describe("isLocalizationModeAndChanged", () => {
        it("returns false when not in localization mode", () => {
            const button = document.createElement("button");
            const opt = createMockOptions({
                getButton: vi.fn(() => Fluent(button))
            });
            const localizer = new EntityLocalizer(opt);
            expect((localizer as any).isLocalizationModeAndChanged()).toBe(false);
        });
    });

    describe("getLocalizationGridValue", () => {
        it("invokes grid.save and filters non-translation keys", () => {
            const pgDiv = document.createElement("div");
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true }
            ];
            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                getLanguages: vi.fn(() => [{ id: "en", text: "English" }]),
                pgOptions: { idPrefix: "Test_", items, mode: 0 as any, localTextPrefix: "Forms.Test." }
            });
            const localizer = new EntityLocalizer(opt);
            // Mock grid.save
            const saveSpy = vi.fn((value: any) => {
                value["en$Name"] = "English Value";
                value["NormalField"] = "Should be removed";
            });
            localizer["grid"] = { save: saveSpy, destroy: vi.fn() } as any;
            const result = (localizer as any).getLocalizationGridValue();
            expect(result["en$Name"]).toBe("English Value");
            expect(result["NormalField"]).toBeUndefined();
        });
    });

    describe("loadLocalization", () => {
        function createEnabledLocalizer() {
            const pgDiv = document.createElement("div");
            const button = document.createElement("button");
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true }
            ];
            const retrieveLocs = vi.fn(async () => ({}));
            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                getButton: vi.fn(() => Fluent(button)),
                getLanguages: vi.fn(() => [{ id: "en", text: "English" }]),
                isNew: vi.fn(() => true),
                pgOptions: { idPrefix: "Test_", items, mode: 0 as any, localTextPrefix: "Forms.Test." },
                retrieveLocalizations: retrieveLocs
            });
            const localizer = new EntityLocalizer(opt);
            return { localizer, retrieveLocs, button };
        }

        it("loads empty when new and no lastValue", () => {
            const { localizer } = createEnabledLocalizer();
            localizer["lastValue"] = null;
            const gridLoadSpy = vi.fn();
            // Use a grid mock that also has enumerateItems and save
            localizer["grid"] = { load: gridLoadSpy, save: vi.fn(), enumerateItems: vi.fn(), destroy: vi.fn() } as any;
            (localizer as any).loadLocalization();
            expect(gridLoadSpy).toHaveBeenCalledWith({});
        });

        it("loads from lastValue when available", () => {
            const { localizer } = createEnabledLocalizer();
            const lastVal = { "en$Name": "Stored Value" };
            localizer["lastValue"] = lastVal;
            const gridLoadSpy = vi.fn();
            localizer["grid"] = { load: gridLoadSpy, save: vi.fn(), enumerateItems: vi.fn(), destroy: vi.fn() } as any;
            (localizer as any).loadLocalization();
            expect(gridLoadSpy).toHaveBeenCalledWith(lastVal);
        });

        it("retrieves localizations when not new and no lastValue", async () => {
            const pgDiv = document.createElement("div");
            const button = document.createElement("button");
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true }
            ];
            const retrieveLocs = vi.fn(async () => ({
                en: { Name: "Retrieved Name" }
            }));
            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                getButton: vi.fn(() => Fluent(button)),
                getLanguages: vi.fn(() => [{ id: "en", text: "English" }]),
                isNew: vi.fn(() => false),
                getEntity: vi.fn(() => ({ Name: "Original" })),
                pgOptions: { idPrefix: "Test_", items, mode: 0 as any, localTextPrefix: "Forms.Test." },
                retrieveLocalizations: retrieveLocs
            });
            const localizer = new EntityLocalizer(opt);
            localizer["lastValue"] = null;
            const gridLoadSpy = vi.fn();
            localizer["grid"] = { load: gridLoadSpy, save: vi.fn(), destroy: vi.fn() } as any;
            await (localizer as any).loadLocalization();
            expect(retrieveLocs).toHaveBeenCalled();
            expect(gridLoadSpy).toHaveBeenCalled();
            // Should contain the merged localization
            const callArg = gridLoadSpy.mock.calls[0][0];
            expect(callArg["en$Name"]).toBe("Retrieved Name");
        });
    });

    describe("buttonClick full flow", () => {
        function createEnabledLocalizer(overrides?: any) {
            const pgDiv = document.createElement("div");
            const button = document.createElement("button");
            const items: PropertyItem[] = [
                { name: "Name", title: "Name", localizable: true }
            ];
            const opt = createMockOptions({
                getPropertyGrid: vi.fn(() => Fluent(pgDiv)),
                getButton: vi.fn(() => Fluent(button)),
                getLanguages: vi.fn(() => [{ id: "en", text: "English" }]),
                isNew: vi.fn(() => true),
                pgOptions: { idPrefix: "Test_", items, mode: 0, localTextPrefix: "Forms.Test." },
                ...overrides
            });
            return new EntityLocalizer(opt);
        }

        it("enters localization mode when clicked outside mode", () => {
            const localizer = createEnabledLocalizer();
            const loadLocSpy = vi.fn();
            localizer["loadLocalization"] = loadLocSpy;
            localizer.buttonClick();
            // Button should have 'pressed' class now
            expect((localizer as any).isLocalizationMode()).toBe(true);
        });

        it("exits localization mode when clicked inside mode", () => {
            const localizer = createEnabledLocalizer();
            // First enter mode
            localizer.buttonClick();
            expect((localizer as any).isLocalizationMode()).toBe(true);
            // Then exit
            localizer.buttonClick();
            expect((localizer as any).isLocalizationMode()).toBe(false);
        });
    });
});
