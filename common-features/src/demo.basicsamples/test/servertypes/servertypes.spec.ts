import * as corelib from "@serenity-is/corelib";
import { mockFetch, unmockFetch } from "test-utils";
import * as Demo from "../../Modules/ServerTypes/Demo";
import * as Namespaces from "../../Modules/ServerTypes/Namespaces";
import "../../Modules/ServerTypes/Texts";

const allExports = Object.entries(Demo as Record<string, any>);

const columnTypes = allExports.filter(([, v]) =>
    typeof v === "function" && v.columnsKey);

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

describe("BasicSamples ServerTypes namespaces", () => {
    it("exposes the basicsamples namespaces", () => {
        expect(Namespaces.DemoBasicSamplesNS).toBe("Serenity.Demo.BasicSamples");
        expect(Namespaces.nsDemoBasicSamples).toBe("Serenity.Demo.BasicSamples.");
    });
});

describe("BasicSamples ServerTypes columns", () => {
    it("includes the inline image columns", () => {
        expect(columnTypes.length).toBeGreaterThan(0);
    });

    it.each(columnTypes)("columns %s exposes a columns key", (_name, columnType) => {
        expect(typeof columnType.columnsKey).toBe("string");
        const columns = new columnType([]);
        expect(columns.valueOf()).toBeTruthy();
    });
});

describe("BasicSamples ServerTypes forms", () => {
    it("exposes generated forms", () => {
        expect(formTypes.length).toBeGreaterThan(0);
    });

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

describe("BasicSamples ServerTypes services", () => {
    it("includes the basic samples service", () => {
        expect(serviceTypes.length).toBeGreaterThan(0);
    });

    it.each(serviceTypes)("service %s calls each method", (_name, service) => {
        expect(typeof service.baseUrl).toBe("string");
        for (const method of Object.keys(service.Methods)) {
            expect(typeof service[method]).toBe("function");
            service[method]({}, () => { }, {});
        }
        expect(serviceRequestSpy).toHaveBeenCalled();
    });
});
