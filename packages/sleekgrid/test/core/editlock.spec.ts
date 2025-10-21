import { EditController, EditorLock, GlobalEditorLock } from "../../src/core/editing";

describe('EditLock', () => {
    it('should throw an exception when deactivating not active editor', () => {
        const editorLock = new EditorLock();

        expect(() => editorLock.deactivate({} as EditController))
            .toThrow("SleekGrid.EditorLock.deactivate: specified editController is not the currently active one");
    });

    it('should not throw when activate gets current active editor again', () => {
        const editorLock = new EditorLock();

        const editor = <EditController>{
            commitCurrentEdit: () => true,
            cancelCurrentEdit: () => true
        };

        editorLock.activate(editor);
        editorLock.activate(editor);
    });

    it('should return true isActive gets called with current active editor', () => {
        const editorLock = new EditorLock();

        const editor = <EditController>{
            commitCurrentEdit: () => true,
            cancelCurrentEdit: () => true
        };

        editorLock.activate(editor);

        expect(editorLock.isActive(editor)).toBeTruthy();
    });

    it('should return false isActive gets called with another editor', () => {
        const editorLock = new EditorLock();

        const firstEditor = <EditController>{
            commitCurrentEdit: () => true,
            cancelCurrentEdit: () => true
        };

        const secondEditor = <EditController>{
            commitCurrentEdit: () => true,
            cancelCurrentEdit: () => true
        };

        editorLock.activate(firstEditor);

        expect(editorLock.isActive(secondEditor)).toBeFalsy();
    });

    it('should return true when there is an active editor and isActive called with falsy value', () => {
        const editorLock = new EditorLock();

        const editor = <EditController>{
            commitCurrentEdit: () => true,
            cancelCurrentEdit: () => true
        };

        editorLock.activate(editor);

        expect(editorLock.isActive()).toBeTruthy();
        expect(editorLock.isActive(null)).toBeTruthy();
        expect(editorLock.isActive(undefined)).toBeTruthy();
    });

    it('should return false when there is not an active editor and isActive called with falsy value', () => {
        const editorLock = new EditorLock();

        expect(editorLock.isActive()).toBeFalsy();
        expect(editorLock.isActive(null)).toBeFalsy();
        expect(editorLock.isActive(undefined)).toBeFalsy();
    });

    it('should throw when activate gets called when it has an active editor', () => {
        const editorLock = new EditorLock();

        editorLock.activate({
           commitCurrentEdit: () => true,
           cancelCurrentEdit: () => true
        });

        expect(() => editorLock.activate(null as EditController))
            .toThrow("SleekGrid.EditorLock.activate: an editController is still active, can't activate another editController");
    });

    it('should throw when activate gets called and editor doesnt implement commitCurrentEdit', () => {
        const editorLock = new EditorLock();

        const editor = {
            commitCurrentEdit: () => true,
        };

        expect(() => editorLock.activate(editor as EditController))
            .toThrow("SleekGrid.EditorLock.activate: editController must implement .cancelCurrentEdit()");
    });

    it('should throw when activate gets called and editor doesnt implement cancelCurrentEdit', () => {
        const editorLock = new EditorLock();

        const editor = {
            cancelCurrentEdit: () => true,
        };

        expect(() => editorLock.activate(editor as EditController))
            .toThrow("SleekGrid.EditorLock.activate: editController must implement .commitCurrentEdit()");
    });

    it('should set active editor to null if deactivate called with active editor lock', () => {
        const editorLock = new EditorLock();

        const editor = <EditController>{
            commitCurrentEdit: () => true,
            cancelCurrentEdit: () => true
        };

        editorLock.activate(editor);
        expect(editorLock.isActive()).toBeTruthy();

        editorLock.deactivate(editor);
        expect(editorLock.isActive()).toBeFalsy();
    });

    it('should call current active editors commitCurrentEdit() on commitCurrentEdit and return its value when there is an active editor', () => {
        const editorLock = new EditorLock();
        let isCommitCurrentEditCalled = false;

        const editor = <EditController>{
            commitCurrentEdit: () => {
                isCommitCurrentEditCalled = true;
                return true;
            },
            cancelCurrentEdit: () => true
        };

        editorLock.activate(editor);
        expect(editorLock.isActive(editor)).toBeTruthy();
        expect(isCommitCurrentEditCalled).toBeFalsy();

        const commitCurrentEditReturnValue = editorLock.commitCurrentEdit();
        expect(commitCurrentEditReturnValue).toBeTruthy();
        expect(isCommitCurrentEditCalled).toBeTruthy();
    });

    it('should return true when commitCurrentEdit gets called and there is no active editor', () => {
        const editorLock = new EditorLock();

        expect(editorLock.commitCurrentEdit()).toBeTruthy();

        const editor = <EditController>{
            commitCurrentEdit: () => false,
            cancelCurrentEdit: () => false
        };

        editorLock.activate(editor);
        editorLock.deactivate(editor);

        expect(editorLock.commitCurrentEdit()).toBeTruthy();
    });

    it('should call current active editors cancelCurrentEdit() on cancelCurrentEdit and return its value when there is an active editor', () => {
        const editorLock = new EditorLock();
        let isCancelCurrentEditCalled = false;

        const editor = <EditController>{
            commitCurrentEdit: () => true,
            cancelCurrentEdit: () => {
                isCancelCurrentEditCalled = true;
                return true;
            }
        };

        editorLock.activate(editor);
        expect(editorLock.isActive(editor)).toBeTruthy();
        expect(isCancelCurrentEditCalled).toBeFalsy();

        const cancelCurrentEditReturnValue = editorLock.cancelCurrentEdit();
        expect(cancelCurrentEditReturnValue).toBeTruthy();
        expect(isCancelCurrentEditCalled).toBeTruthy();
    });

    it('should return true when cancelCurrentEdit gets called and there is no active editor', () => {
        const editorLock = new EditorLock();

        expect(editorLock.cancelCurrentEdit()).toBeTruthy();

        const editor = <EditController>{
            commitCurrentEdit: () => false,
            cancelCurrentEdit: () => false
        };

        editorLock.activate(editor);
        editorLock.deactivate(editor);

        expect(editorLock.cancelCurrentEdit()).toBeTruthy();
    });
});

describe('GlobalEditorLock', () => {
    it('should export an already initialized EditorLock for GlobalEditorLock', function () {
        expect(GlobalEditorLock).toBeDefined();
        expect(GlobalEditorLock).toBeInstanceOf(EditorLock);
    });
});
