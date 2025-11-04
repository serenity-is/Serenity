describe("supports lowercase attribute variants by default but also allows some uppercase", () => {

    it("supports for and htmlFor", () => {
        expect((<label for="test">Test</label> as HTMLLabelElement).htmlFor).toBe("test");
        expect((<label htmlFor="test">Test</label> as HTMLLabelElement).htmlFor).toBe("test");
    });

    it("supports class and className", () => {
        expect((<div class="test">Test</div> as HTMLDivElement).className).toBe("test");
        expect((<div className="test">Test</div> as HTMLDivElement).className).toBe("test");
    });

    it("supports tabindex and tabIndex with both string and ints", () => {
        expect((<input tabindex={5}>Test</input> as HTMLInputElement).tabIndex).toBe(5);
        expect((<input tabindex="5">Test</input> as HTMLInputElement).tabIndex).toBe(5);
        expect((<input tabIndex={5}>Test</input> as HTMLInputElement).tabIndex).toBe(5);
        expect((<input tabIndex="5">Test</input> as HTMLInputElement).tabIndex).toBe(5);
    });

    it("supports maxlength and maxLength with both string and ints", () => {
        expect((<input maxlength={5}>Test</input> as HTMLInputElement).maxLength).toBe(5);
        expect((<input maxlength="5">Test</input> as HTMLInputElement).maxLength).toBe(5);
        expect((<input maxLength={5}>Test</input> as HTMLInputElement).maxLength).toBe(5);
        expect((<input maxLength="5">Test</input> as HTMLInputElement).maxLength).toBe(5);
    });

    it("supports minlength and minLength with both string and ints", () => {
        expect((<input minlength={5}>Test</input> as HTMLInputElement).minLength).toBe(5);
        expect((<input minlength="5">Test</input> as HTMLInputElement).minLength).toBe(5);
        expect((<input minLength={5}>Test</input> as HTMLInputElement).minLength).toBe(5);
        expect((<input minLength="5">Test</input> as HTMLInputElement).minLength).toBe(5);
    });
})
