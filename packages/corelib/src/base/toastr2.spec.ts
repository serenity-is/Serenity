import { Toastr } from './toastr2';

describe('Toastr', () => {
    let toastr: Toastr;

    beforeEach(() => {
        toastr = new Toastr();
        toastr.clear();
    });

    it('should initialize correctly', () => {
        expect(toastr).toBeDefined();
    });

    it('should create a container', () => {
        const container = toastr.getContainer(null, true);
        expect(container).toBeDefined();
        expect(container.getAttribute('id')).toBe('toast-container');
    });

    it('should get a container', () => {
        const container = toastr.getContainer();
        expect(container).toBeDefined();
    });

    it('should return existing container if it exists', () => {
        const initialContainer = toastr.getContainer(null, true);
        const container = toastr.getContainer(null, true);
        expect(container).toBeDefined();
        expect(container).toBe(initialContainer);
    });

    it('should create and return a new container if container does not exist and create is true', () => {
        toastr = new Toastr({ containerId: "mycontainer" });
        const container = toastr.getContainer(null, true);
        expect(container).toBeDefined();
        expect(container.getAttribute('id')).toBe('mycontainer');
        container.remove();
    });

    it('should return null if container does not exist and create is false', () => {
        toastr.clear();
        const container = toastr.getContainer();
        expect(container).toBeNull();
    });

    it('should create an error toast', () => {
        const toast = toastr.error('Error message', 'Error title');
        expect(toast).toBeDefined();
    });

    it('should create a warning toast', () => {
        const toast = toastr.warning('Warning message', 'Warning title');
        expect(toast).toBeDefined();
    });

    it('should create a success toast', () => {
        const toast = toastr.success('Success message', 'Success title');
        expect(toast).toBeDefined();
    });

    it('should create an info toast', () => {
        const toast = toastr.info('Info message', 'Info title');
        expect(toast).toBeDefined();
    });

    it('should subscribe and publish events', () => {
        const callback = jest.fn();
        toastr.subscribe(callback);
        toastr.publish(toastr);
        expect(callback).toHaveBeenCalledWith(toastr);
    });

    it('should clear and remove toasts', () => {
        const toast = toastr.error('Error message', 'Error title');
        toastr.removeToast(toast);
        expect(toastr.getContainer()).toBeNull();
    });

    it('hides toasts', (done) => {
        let onHidden = jest.fn(() => {});
        const toast = toastr.info('Test', null, {
            onHidden,
            timeOut: 1
        });
        setTimeout(() => {
            try {
                expect(onHidden).toHaveBeenCalledTimes(1);
            }
            finally {
                done();
            }
        }, 5);
    });

    it('does not create duplicate toast if preventDuplicates is true', () => {
        const toast1 = toastr.info('Test Message', null, {
            preventDuplicates: true
        });
        expect(toast1).toBeDefined();
        const toast2 = toastr.info('Test Message', null, {
            preventDuplicates: true
        });
        expect(toast2).toBeNull();
    });

    it('adds rtl to classlist if rtl is true', () => {
        const toast = toastr.info('Test Message', null, {
            rtl: true
        });
        expect(toast).toBeDefined();
        expect(toast.classList.contains("rtl")).toBe(true);
    });

    it('can create a close button', (done) => {
        const toast = toastr.info('Test Message', null, {
            closeButton: true,
            closeClass: 'my-close-button',
            timeOut: 1000,
            closeDuration: false
        });
        expect(toast).toBeDefined();
        let closeButton = toast.querySelector('.my-close-button') as HTMLElement;
        expect(closeButton).toBeDefined();
        closeButton.click();
        setTimeout(() => {
            try {
                expect(toast.parentElement).toBeNull();
            }
            finally {
                done();
            }
        }, 0);
    });

    it('can does not close if tapToDismiss is false and onClick is null', (done) => {
        const toast = toastr.info('Test Message', null, {
            tapToDismiss: false,
            timeOut: 1000,
            closeDuration: false
        });
        expect(toast).toBeDefined();
        toast.click();
        setTimeout(() => {
            try {
                expect(toast.parentElement).not.toBeNull();
            }
            finally {
                done();
            }
        }, 0);
    });
});


