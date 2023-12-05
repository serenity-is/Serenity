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

    it('can add a close button', () => {

    });
});


