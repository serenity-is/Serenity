import { defaultNotifyOptions, notifyError, notifyInfo, notifySuccess, notifyWarning, positionToastContainer } from "./notify";

jest.mock("./toastr2", () => ({
    __esModule: true,
    default: {
        error: jest.fn(),
        info: jest.fn(),
        success: jest.fn(),
        warning: jest.fn(),
        clear: jest.fn((options: { containerId: string }) => {
            document.getElementById(options?.containerId ?? 'toast-container')?.remove();
        }),
        getContainer: jest.fn((options: { containerId: string, positionClass: string }, create: boolean) => {
            const id = options?.containerId ?? 'toast-container';
            let div = document.getElementById(id);
            if (div)
                return div;
            div = document.createElement('div');
            div.setAttribute('id', id);
            div.setAttribute('class', (options?.positionClass ?? 'position-toast toast-top-full-width'));
            document.body.appendChild(div);
            return div;
        })
    }
}));

beforeEach(() => {
    jest.clearAllMocks();
});

describe("notifyError", () => {
    it("should show an error toast with the provided message and default title and options", async () => {
        const message = "An error occurred";
        const toastr = (await import("./toastr2")).default;
        const expectedOptions = Object.assign({}, defaultNotifyOptions);
        notifyError(message);
        try {
            expect(toastr.getContainer).toHaveBeenCalledWith(expectedOptions, true);
            expect(toastr.getContainer).toHaveBeenCalledTimes(1);
            expect(toastr.error).toHaveBeenCalledWith(message, undefined, expectedOptions);
            expect(toastr.error).toHaveBeenCalledTimes(1);
        }
        finally {
            toastr.clear(expectedOptions);
        }
    });

    it("should pass the title and provided options", async () => {
        const message = "An error occurred";
        const title = "Error title";
        const toastr = (await import("./toastr2")).default;
        const options = {
            showDuration: 1111,
            messageClass: "my-class"
        }
        const expectedOptions = Object.assign({}, defaultNotifyOptions, options);
        notifyError(message, title, options);
        try {
            expect(toastr.getContainer).toHaveBeenCalledWith(expectedOptions, true);
            expect(toastr.getContainer).toHaveBeenCalledTimes(1);
            expect(toastr.error).toHaveBeenCalledWith(message, title, expectedOptions);
            expect(toastr.error).toHaveBeenCalledTimes(1);
        }
        finally {
            toastr.clear(expectedOptions);
        }
    });
});

describe("notifyInfo", () => {
    it("should show an error toast with the provided message and default title and options", async () => {
        const message = "Some information";
        const toastr = (await import("./toastr2")).default;
        const expectedOptions = Object.assign({}, defaultNotifyOptions);
        notifyInfo(message);
        try {
            expect(toastr.getContainer).toHaveBeenCalledWith(expectedOptions, true);
            expect(toastr.getContainer).toHaveBeenCalledTimes(1);
            expect(toastr.info).toHaveBeenCalledWith(message, undefined, expectedOptions);
            expect(toastr.info).toHaveBeenCalledTimes(1);
        }
        finally {
            toastr.clear(expectedOptions);
        }
    });

    it("should pass the title and provided options", async () => {
        const message = "Some information";
        const title = "Info title";
        const toastr = (await import("./toastr2")).default;
        const options = {
            showDuration: 1111,
            messageClass: "my-class"
        }
        const expectedOptions = Object.assign({}, defaultNotifyOptions, options);
        notifyInfo(message, title, options);
        try {
            expect(toastr.getContainer).toHaveBeenCalledWith(expectedOptions, true);
            expect(toastr.getContainer).toHaveBeenCalledTimes(1);
            expect(toastr.info).toHaveBeenCalledWith(message, title, expectedOptions);
            expect(toastr.info).toHaveBeenCalledTimes(1);
        }
        finally {
            toastr.clear(expectedOptions);
        }
    });
});


describe("notifySuccess", () => {
    it("should show an error toast with the provided message and default title and options", async () => {
        const message = "Some success";
        const toastr = (await import("./toastr2")).default;
        const expectedOptions = Object.assign({}, defaultNotifyOptions);
        notifySuccess(message);
        try {
            expect(toastr.getContainer).toHaveBeenCalledWith(expectedOptions, true);
            expect(toastr.getContainer).toHaveBeenCalledTimes(1);
            expect(toastr.success).toHaveBeenCalledWith(message, undefined, expectedOptions);
            expect(toastr.success).toHaveBeenCalledTimes(1);
        }
        finally {
            toastr.clear(expectedOptions);
        }
    });

    it("should pass the title and provided options", async () => {
        const message = "Some success";
        const title = "Success title";
        const toastr = (await import("./toastr2")).default;
        const options = {
            showDuration: 1111,
            messageClass: "my-class"
        }
        const expectedOptions = Object.assign({}, defaultNotifyOptions, options);
        notifySuccess(message, title, options);
        try {
            expect(toastr.getContainer).toHaveBeenCalledWith(expectedOptions, true);
            expect(toastr.getContainer).toHaveBeenCalledTimes(1);
            expect(toastr.success).toHaveBeenCalledWith(message, title, expectedOptions);
            expect(toastr.success).toHaveBeenCalledTimes(1);
        }
        finally {
            toastr.clear(expectedOptions);
        }
    });
});


describe("notifyWarning", () => {
    it("should show an error toast with the provided message and default title and options", async () => {
        const message = "Some success";
        const toastr = (await import("./toastr2")).default;
        const expectedOptions = Object.assign({}, defaultNotifyOptions);
        notifyWarning(message);
        try {
            expect(toastr.getContainer).toHaveBeenCalledWith(expectedOptions, true);
            expect(toastr.getContainer).toHaveBeenCalledTimes(1);
            expect(toastr.warning).toHaveBeenCalledWith(message, undefined, expectedOptions);
            expect(toastr.warning).toHaveBeenCalledTimes(1);
        }
        finally {
            toastr.clear(expectedOptions);
        }
    });

    it("should pass the title and provided options", async () => {
        const message = "Some success";
        const title = "Success title";
        const toastr = (await import("./toastr2")).default;
        const options = {
            showDuration: 1111,
            messageClass: "my-class"
        }
        const expectedOptions = Object.assign({}, defaultNotifyOptions, options);
        notifyWarning(message, title, options);
        try {
            expect(toastr.getContainer).toHaveBeenCalledWith(expectedOptions, true);
            expect(toastr.getContainer).toHaveBeenCalledTimes(1);
            expect(toastr.warning).toHaveBeenCalledWith(message, title, expectedOptions);
            expect(toastr.warning).toHaveBeenCalledTimes(1);
        }
        finally {
            toastr.clear(expectedOptions);
        }
    });
});

describe("positionToastContainer", () => {
    it("ignores if container does not have positioned-toast class", async () => {
        const toastr = (await import("./toastr2")).default;
        var modal = document.body.appendChild(document.createElement("div"));
        try {
            modal.setAttribute("class", "modal in");
            jest.spyOn(modal, "getBoundingClientRect");
            positionToastContainer({ 
                positionClass: "top-right"
            });
            expect(modal.getBoundingClientRect).not.toHaveBeenCalled();
        }
        finally {
            modal.remove();
            toastr.clear();
        }
    });

    it("ignores modals with display: none or hidden class", async () => {
        const toastr = (await import("./toastr2")).default;
        var modal = document.body.appendChild(document.createElement("div"));
        try {
            modal.setAttribute("class", "modal in");
            modal.setAttribute("style", "display: none");
            jest.spyOn(modal, "getBoundingClientRect");
            modal.setAttribute("style", "display: none");
            positionToastContainer();
            expect(modal.getBoundingClientRect).not.toHaveBeenCalled();
            modal.setAttribute("style", "display:none");
            positionToastContainer();
            expect(modal.getBoundingClientRect).not.toHaveBeenCalled();
            modal.setAttribute("style", "");
            modal.classList.add("hidden");
            positionToastContainer();
            expect(modal.getBoundingClientRect).not.toHaveBeenCalled();
        }
        finally {
            modal.remove();
            toastr.clear();
        }
    });

    it("positions notification container based on modal position", async () => {
        const toastr = (await import("./toastr2")).default;
        var modal = document.body.appendChild(document.createElement("div"));
        try {
            modal.setAttribute("class", "modal in");
            jest.spyOn(modal, "getBoundingClientRect").mockImplementation(() => ({ top: 100, left: 100, bottom: 400, right: 500 } as any));
            positionToastContainer();
            const container = toastr.getContainer();
            expect(container).toBeDefined();
            expect(container.classList.contains('positioned-toast')).toBe(true);
            const style = container.style;
            expect(style.position).toBe('absolute');
            expect(style.top).toBe('128px');
            expect(style.left).toBe('106px');
            expect(style.width).toBe('388px');
        }
        finally {
            modal.remove();
            toastr.clear();
        }
    });

    it("removes positioned-toast related styles in next call without modals", async () => {
        const toastr = (await import("./toastr2")).default;
        var modal = document.body.appendChild(document.createElement("div"));
        try {
            modal.setAttribute("class", "modal in");
            jest.spyOn(modal, "getBoundingClientRect").mockImplementation(() => ({ top: 100, left: 100, bottom: 400, right: 500 } as any));
            positionToastContainer();
            const container = toastr.getContainer();
            modal.remove();
            positionToastContainer();
            expect(container).toBeDefined();
            expect(container.classList.contains('positioned-toast')).toBe(false);
            const style = container.style;
            expect(style.position).toBe('');
            expect(style.top).toBe('');
            expect(style.left).toBe('');
            expect(style.width).toBe('');
        }
        finally {
            modal.remove();
            toastr.clear();
        }
    });

});