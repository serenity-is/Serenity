describe("supports pseudo-boolean ('true', 'false') attribute values for some attributes", () => {

    it("supports contenteditable with pseudo-boolean values", () => {
        const spy = vi.spyOn(HTMLDivElement.prototype, 'setAttribute');

        Object(<div contenteditable="true">Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("contenteditable", "true");
        spy.mockClear();

        Object(<div contenteditable>Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("contenteditable", "true");
        spy.mockClear();

        Object(<div contenteditable="">Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("contenteditable", "true");
        spy.mockClear();

        Object(<div contenteditable={false}>Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("contenteditable", "false");
        spy.mockClear();

        Object(<div contenteditable="false">Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("contenteditable", "false");
        spy.mockClear();

        Object(<div contenteditable={null}>Test</div> as HTMLDivElement)
        expect(spy).not.toHaveBeenCalled();
        spy.mockClear();

        spy.mockRestore();
    });


    it("supports contenteditable with pseudo-boolean values", () => {
        const spy = vi.spyOn(HTMLDivElement.prototype, 'setAttribute');
        Object(<div contenteditable>Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("contenteditable", "true");
        spy.mockClear();

        Object(<div contenteditable="">Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("contenteditable", "true");
        spy.mockClear();

        Object(<div contenteditable={false}>Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("contenteditable", "false");
        spy.mockClear();

        Object(<div contenteditable="false">Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("contenteditable", "false");
        spy.mockClear();

        Object(<div contenteditable={null}>Test</div> as HTMLDivElement)
        expect(spy).not.toHaveBeenCalled();
        spy.mockClear();

        spy.mockRestore();
    });

    it("supports draggable with pseudo-boolean values", () => {
        const spy = vi.spyOn(HTMLDivElement.prototype, 'setAttribute');

        Object(<div draggable="true">Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("draggable", "true");
        spy.mockClear();

        // https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Global_attributes/draggable
        // forbidden to use the attribute without a value
        // Object(<div draggable>Test</div> as HTMLDivElement)
        // expect(spy).toHaveBeenCalledWith("draggable", "true");
        // spy.mockClear();
        // Object(<div draggable="">Test</div> as HTMLDivElement)
        // expect(spy).toHaveBeenCalledWith("draggable", "true");
        // spy.mockClear();

        Object(<div draggable={false}>Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("draggable", "false");
        spy.mockClear();

        Object(<div draggable="false">Test</div> as HTMLDivElement)
        expect(spy).toHaveBeenCalledWith("draggable", "false");
        spy.mockClear();

        Object(<div draggable={null}>Test</div> as HTMLDivElement)
        expect(spy).not.toHaveBeenCalled();
        spy.mockClear();

        spy.mockRestore();
    });

    it("supports spellcheck with pseudo-boolean values", () => {
        expect((<input spellcheck="true">Test</input> as HTMLInputElement).spellcheck).toBe(true);
        expect((<input spellcheck>Test</input> as HTMLInputElement).spellcheck).toBe(true);
        expect((<input spellcheck="">Test</input> as HTMLInputElement).spellcheck).toBe(true);
        expect((<input spellcheck={true}>Test</input> as HTMLInputElement).spellcheck).toBe(true);
        expect((<input spellcheck={null}>Test</input> as HTMLInputElement).spellcheck).toBe(null);
        expect((<input spellcheck="false">Test</input> as HTMLInputElement).spellcheck).toBe(false);
        expect((<input spellcheck={false}>Test</input> as HTMLInputElement).spellcheck).toBe(false);
    });
});
