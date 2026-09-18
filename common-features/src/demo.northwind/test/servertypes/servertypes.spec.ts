import * as corelib from "@serenity-is/corelib";
import { mockFetch, unmockFetch } from "test-utils";
import * as Demo from "../../Modules/ServerTypes/Demo";
import * as Namespaces from "../../Modules/ServerTypes/Namespaces";
import { NorthwindDbTexts, NorthwindValidationTexts } from "../../Modules/ServerTypes/Texts";

const allExports = Object.entries(Demo as Record<string, any>);

const rowTypes = allExports.filter(([, v]) =>
    typeof v === "function" && v.idProperty && v.Fields);

const columnTypes = allExports.filter(([, v]) =>
    typeof v === "function" && v.columnsKey && !v.idProperty);

const formTypes = allExports.filter(([, v]) =>
    typeof v === "function" && v.formKey);

const serviceTypes = allExports.filter(([, v]) =>
    v && typeof v === "object" && typeof v.baseUrl === "string" && v.Methods);

let serviceRequestSpy: ReturnType<typeof vi.spyOn>;

beforeEach(() => {
    mockFetch({ "*": () => ({ Entities: [] }) });
    serviceRequestSpy = vi.spyOn(corelib, "serviceRequest").mockImplementation(((service: any, request: any, onSuccess: any) => {
        onSuccess?.({});
        return Promise.resolve({}) as any;
    }) as any);
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("Northwind ServerTypes namespaces", () => {
    it("exposes the northwind namespaces", () => {
        expect(Namespaces.DemoNorthwindNS).toBe("Serenity.Demo.Northwind");
        expect(Namespaces.nsDemoNorthwind).toBe("Serenity.Demo.Northwind.");
    });

    it("exposes texts", () => {
        expect(typeof NorthwindDbTexts.Customer.EntitySingular).toBe("string");
        expect(typeof NorthwindValidationTexts.NorthwindPhone).toBe("string");
    });
});

describe("Northwind ServerTypes rows", () => {
    it("exposes generated row types", () => {
        expect(rowTypes.length).toBeGreaterThan(0);
    });

    it.each(rowTypes)("row %s exposes static metadata", (_name, rowType) => {
        expect(typeof rowType.idProperty).toBe("string");
        expect(typeof rowType.localTextPrefix).toBe("string");
        expect(rowType.Fields).toBeTruthy();
    });

    it.each(rowTypes)("row %s can resolve a lookup", async (_name, rowType) => {
        if (typeof rowType.getLookup !== "function")
            return;
        expect(rowType.getLookup()).toBeTruthy();
        await expect(rowType.getLookupAsync()).resolves.toBeTruthy();
    });
});

describe("Northwind ServerTypes columns", () => {
    it.each(columnTypes)("columns %s exposes a columns key", (_name, columnType) => {
        expect(typeof columnType.columnsKey).toBe("string");
        expect(columnType.Fields).toBeTruthy();
        const columns = new columnType([]);
        expect(columns).toBeTruthy();
        expect(columns.valueOf()).toBeTruthy();
    });
});

describe("Northwind ServerTypes forms", () => {
    it.each(formTypes)("form %s registers field accessors", (_name, formType) => {
        const el = document.body.appendChild(document.createElement("div"));
        const form = new formType({ idPrefix: "Test_", domNode: el });
        expect(form.idPrefix).toBe("Test_");
        expect(formType.formKey).toBeTruthy();
    });

    it("constructs forms with a string prefix", () => {
        for (const [, formType] of formTypes)
            expect(new formType("Test_")).toBeTruthy();
    });
});

describe("Northwind ServerTypes services", () => {
    it.each(serviceTypes)("service %s calls each method", (_name, service) => {
        expect(typeof service.baseUrl).toBe("string");
        expect(service.Methods).toBeTruthy();
        for (const method of Object.keys(service.Methods)) {
            expect(typeof service.Methods[method]).toBe("string");
            expect(typeof service[method]).toBe("function");
            service[method]({}, () => { }, {});
        }
        expect(serviceRequestSpy).toHaveBeenCalled();
    });
});

describe("Northwind ServerTypes enums and permission keys", () => {
    it("exposes enum values", () => {
        expect(Demo.Gender.Male).toBe(1);
        expect(Demo.Gender.Female).toBe(2);
        expect(Demo.OrderShippingState.NotShipped).toBe(0);
        expect(Demo.OrderShippingState.Shipped).toBe(1);
    });

    it("exposes permission keys", () => {
        expect(Demo.PermissionKeys.General).toBe("Northwind:General");
        expect(Demo.PermissionKeys.Customer.Delete).toBe("Northwind:Customer:Delete");
        expect(Demo.PermissionKeys.Customer.Modify).toBe("Northwind:Customer:Modify");
        expect(Demo.PermissionKeys.Customer.View).toBe("Northwind:Customer:View");
    });
});
