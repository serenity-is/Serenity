import { ToolbarButton } from "./toolbar";

describe("ToolButton", () => {
    it('clicking .tool-button directly does calls onClick if it does not have disabled class', function () {
        const onClick = jest.fn();
        var btn = <ToolbarButton onClick={onClick}></ToolbarButton> as HTMLElement;
        btn.click();
        expect(onClick).toHaveBeenCalledTimes(1);
    });


    it('clicking .button-inner calls onClick if tool-button element does not have disabled class', function () {
        const onClick = jest.fn();
        var btn = <ToolbarButton onClick={onClick}></ToolbarButton> as HTMLElement;
        var inner = btn.querySelector<HTMLElement>(".button-inner");
        expect(inner).not.toBeNull();
        inner.click();
        expect(onClick).toHaveBeenCalledTimes(1);
    });

    it('clicking .tool-button directly does not call onClick if it has disabled class', function () {
        const onClick = jest.fn();
        var btn = <ToolbarButton onClick={onClick}></ToolbarButton> as HTMLElement;
        btn.classList.add("disabled");
        btn.click();
        expect(onClick).not.toHaveBeenCalled();
    });

   
    it('clicking .button-inner does not call onClick if tool-button element has disabled class', function () {
        const onClick = jest.fn();
        var btn = <ToolbarButton onClick={onClick}></ToolbarButton> as HTMLElement;
        btn.classList.add("disabled");
        var inner = btn.querySelector<HTMLElement>(".button-inner");
        expect(inner).not.toBeNull();
        inner.click();
        expect(onClick).not.toHaveBeenCalled();
    });
});