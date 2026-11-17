import { beforeEach, inject } from "vitest";

// A test that runs without a DOM (environment: "node") needs none of this, and
// corelib itself needs window, so everything is imported only when there is one.
//
// This applies to every test of a package, and the escapes are per test:
// - no dynamic data: unmockDynamicData() from test-utils, in a beforeEach rather
//   than an afterEach, as the install is once per file and an afterEach would only
//   drop it for the rest of the file
// - a different text: addLocalText("Forms.Some.Key", "Custom"), which sets one key
// - a different address: spy on getWindowLocation / windowOpen, or call
//   mockLocation() and check what it recorded
if (typeof window !== "undefined") {
    const { addLocalText } = await import("@serenity-is/corelib");
    const { mockDynamicData } = await import("./mocks");
    const { mockLocation } = await import("./location");

    // the forms, columns and lookups the server would send, from the dynamic data
    // that vitest-defaults loads into provide, once per project
    mockDynamicData();

    // the texts the server would send: this project's own, the framework's and
    // those of the @serenity-is packages it references, so a test sees what a user does
    addLocalText(inject("localTexts") ?? {});

    // jsdom does not navigate: the location records where the page would go
    // instead, installed again before each test as a spec may restore its mocks
    beforeEach(() => mockLocation().reset());
}
