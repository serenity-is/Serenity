import { FileUploadTexts, Fluent, PropertyItem, getjQuery, localText, nsSerenity } from "../../base";
import { ValidationHelper } from "../../compat";
import { IGetEditValue, IReadOnly, ISetEditValue, IValidateRequired } from "../../interfaces";
import { FileUploadConstraints, UploadHelper, UploadInputOptions, UploadedFile } from "../helpers/uploadhelper";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * The box that an empty upload editor shows in place of its list of files, which says that files can be dropped on
 * it, and opens the file chooser when clicked, like the add file button.
 * @param props.text - The text, e.g. "Drop a file here or click to choose".
 * @param props.detail - An optional line under the text, e.g. the kind or the size of the files.
 * @param props.choose - Opens the file chooser.
 */
function FileDropHint(props: { text: string, detail?: string, choose: () => void }): HTMLButtonElement {
    return <button type="button" class="file-drop-hint" hidden onClick={() => props.choose()}>
        <span class="file-drop-icon" aria-hidden="true" />
        <span class="file-drop-text">{props.text}</span>
        {props.detail ? <span class="file-drop-detail">{props.detail}</span> : null}
    </button> as HTMLButtonElement;
}

/**
 * Shows the drop hint in place of the list of files while there are none and files can be added.
 */
function updateFileDropHint(hint: HTMLElement, fileSymbols: HTMLElement, empty: boolean, readOnly: boolean) {
    if (!hint || !fileSymbols)
        return;
    const show = empty && !readOnly;
    hint.hidden = !show;
    fileSymbols.hidden = show;
}

/** The actions of the toolbar of the upload editors that the drop box and the buttons by the files replace. */
const builtInToolActions = ["add-file", "delete"];

/**
 * Whether an upload editor shows its toolbar: the option of the editor, else the one of the defaults of its class,
 * else only when the toolbar has buttons other than the built-in ones, e.g. ones that a subclass adds.
 */
function shouldUseToolbar(option: boolean, defaultOption: boolean, buttons: ToolButton[]) {
    return option ?? defaultOption ?? (buttons ?? []).some(x => !builtInToolActions.includes(x?.action));
}

/**
 * Options for the {@link FileUploadEditor}.
 */
export interface FileUploadEditorOptions extends FileUploadConstraints {
    /**
     * Whether the editor shows its toolbar with the select file (and delete) buttons, as it did before, instead of a
     * drop box while it has no file and buttons by its files. By default it shows it only when its toolbar has buttons
     * of its own besides the built-in ones, e.g. ones that a subclass adds in getToolButtons. The default can be changed
     * for all the editors with `FileUploadEditor.defaults.toolbar` and `MultipleFileUploadEditor.defaults.toolbar`.
     */
    toolbar?: boolean;
    /**
     * The text of the drop box while the editor has no file, in place of "Drop a file here or click to choose" (or
     * "Drop files here..." for a multiple editor), as a text or a local text key.
     */
    dropHint?: string;
    /** A line under the text of the drop box, e.g. the kind or the size of the files, as a text or a local text key. */
    dropHintDetail?: string;
    /** Whether to display the original file name. */
    displayFileName?: boolean;
    /** Upload intent for the upload service. */
    uploadIntent?: string;
    /** Upload URL. */
    uploadUrl?: string;
    /** URL prefix for file links. */
    urlPrefix?: string;
}

/**
 * Options for the {@link ImageUploadEditor}.
 */
export interface ImageUploadEditorOptions extends FileUploadEditorOptions {
}

/**
 * An editor that uploads and displays a single file.
 * @typeParam P - Widget props type.
 */
export class FileUploadEditor<P extends FileUploadEditorOptions = FileUploadEditorOptions> extends EditorWidget<P>
    implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired]);

    /**
     * The defaults of the options of the single file and image upload editors, e.g. `toolbar`, which an option of an
     * editor overrides.
     */
    static defaults: Pick<FileUploadEditorOptions, "toolbar"> = {};

    /**
     * Creates a file upload editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.options.allowNonImage ??= this.getDefaultAllowNonImage();

        this.domNode.classList.add('s-FileUploadEditor');

        if (!this.options.originalNameProperty)
            this.domNode.classList.add('hide-original-name');

        const buttons = this.getToolButtons();
        this.toolbar = new Toolbar({
            buttons,
            element: el => this.domNode.appendChild(el)
        }).init();

        // without the toolbar, it is kept hidden, as the file input is in its add button, and subclasses may use it
        const compact = this.compact = !this.useToolbar(buttons);
        this.toolbar.domNode.hidden = compact;
        this.domNode.classList.toggle('s-upload-compact', compact);

        this.progress = (compact ? this.domNode : this.toolbar.domNode).appendChild(<div class="upload-progress"><div /></div> as HTMLElement);

        const uio = this.getUploadInputOptions();
        this.uploadInput = UploadHelper.addUploadInput(uio);
        if (this.options.readOnly)
            this.set_readOnly(true);

        this.fileSymbols = this.domNode.appendChild(<ul class="file-items" /> as HTMLUListElement);
        this.dropHint = this.domNode.appendChild(<FileDropHint text={this.dropHintText()} detail={this.dropHintDetail()} choose={() => this.chooseFile()} /> as HTMLButtonElement);

        if (!this.domNode.id) {
            this.domNode.id = this.uniqueName;
        }

        this.hiddenInput = this.domNode.appendChild(<input class="s-offscreen" type="text" name={this.uniqueName + "_Validator"}
            data-vx-highlight={this.domNode.id} /> as HTMLInputElement);

        this.updateInterface();
    }

    /**
     * Returns whether the editor shows its toolbar instead of the drop box and the buttons by the file, see
     * {@link FileUploadEditorOptions.toolbar}.
     * @param buttons - The buttons of the toolbar, from getToolButtons.
     * @returns True to show the toolbar.
     */
    protected useToolbar(buttons: ToolButton[]): boolean {
        return shouldUseToolbar(this.options.toolbar, FileUploadEditor.defaults?.toolbar, buttons);
    }

    /**
     * Removes the file, as the delete button and the remove button by the file do.
     */
    protected removeFile(): void {
        this.entity = null;
        this.populate();
        this.updateInterface();

        ValidationHelper.validateElement(this.hiddenInput);
        // e.g. a form that tracks its unsaved changes listens to the change event, as for the other editors
        Fluent.trigger(this.domNode, "change");
    }

    /**
     * Returns the upload input options.
     * @returns Upload input options.
     */
    protected getUploadInputOptions(): UploadInputOptions {
        return {
            container: this.toolbar.findButton('add-file-button'),
            zone: this.domNode,
            inputName: this.uniqueName,
            progress: this.progress,
            uploadIntent: this.options.uploadIntent,
            uploadUrl: this.options.uploadUrl,
            fileDone: (response, name, data) => {
                // a file that was uploading when the editor became read-only
                if (this.get_readOnly())
                    return;

                if (!UploadHelper.checkImageConstraints(response, this.options)) {
                    return;
                }

                const newEntity: UploadedFile = {
                    OriginalName: name,
                    Filename: response.TemporaryFile
                };

                this.entity = newEntity;
                this.populate();
                this.updateInterface();

                ValidationHelper.validateElement(this.hiddenInput);
                // e.g. a form that tracks its unsaved changes listens to the change event, as for the other editors
                Fluent.trigger(this.domNode, "change");
            }
        }
    }

    /**
     * Returns the text for the add-file button.
     * @returns The button text.
     */
    protected addFileButtonText(): string {
        return FileUploadTexts.AddFileButton;
    }

    /**
     * Returns the text that the editor shows while it has no file.
     * @returns The text of the drop hint.
     */
    protected dropHintText(): string {
        return this.options.dropHint ? localText(this.options.dropHint) : FileUploadTexts.DropFileHint;
    }

    /**
     * Returns the line under the text of the drop box, if any.
     * @returns The detail of the drop hint, from the dropHintDetail option.
     */
    protected dropHintDetail(): string {
        return this.options.dropHintDetail ? localText(this.options.dropHintDetail) : null;
    }

    /**
     * Opens the file chooser, as the add file button does.
     */
    protected chooseFile(): void {
        if (!this.get_readOnly())
            this.uploadInput?.getNode()?.click();
    }

    /**
     * Returns the toolbar buttons for the editor.
     * @returns Tool button definitions.
     */
    protected getToolButtons(): ToolButton[] {
        return [
            {
                title: this.addFileButtonText(),
                action: 'add-file',
                cssClass: 'add-file-button',
                onClick: function () {
                }
            },
            {
                title: '',
                hint: FileUploadTexts.DeleteButtonHint,
                action: 'delete',
                cssClass: 'delete-button',
                onClick: () => this.removeFile()
            }
        ];
    }

    /**
     * Returns whether non-image files are allowed by default.
     * @returns True when non-image files are allowed.
     */
    protected getDefaultAllowNonImage(): boolean {
        return true;
    }

    /**
     * Populates the file symbols from the current entity.
     */
    protected populate(): void {
        const displayOriginalName = this.options.displayFileName ||
            !!this.options.originalNameProperty?.trim();

        if (this.entity == null) {
            UploadHelper.populateFileSymbols(this.fileSymbols,
                null, displayOriginalName, this.options.urlPrefix);
        }
        else {
            UploadHelper.populateFileSymbols(
                this.fileSymbols, [this.entity], displayOriginalName,
                this.options.urlPrefix);

            // without the toolbar, a menu on the corner of the file replaces and removes it
            if (this.compact)
                this.fileSymbols.querySelector("li")?.appendChild(this.renderFileMenu());
        }

        this.hiddenInput.value = ((this.get_value() || {}).Filename)?.trim() || null;
    }

    /**
     * Renders the menu on the corner of the file, when the editor has no toolbar, which replaces and removes it. It is a
     * dropdown of Bootstrap, like the field selection of the quick search.
     * @returns The menu.
     */
    protected renderFileMenu(): HTMLElement {
        return <div class="dropdown file-menu" hidden={this.get_readOnly()}>
            <button type="button" class="file-menu-toggle" data-bs-toggle="dropdown" aria-expanded="false"
                data-bs-popper-config='{"strategy":"fixed"}' title={FileUploadTexts.FileMenuHint} aria-label={FileUploadTexts.FileMenuHint}>
                <span class="file-menu-icon" aria-hidden="true" />
            </button>
            <ul class="dropdown-menu dropdown-menu-end">
                <li><button type="button" class="dropdown-item file-replace" onClick={() => this.chooseFile()}>
                    <span class="file-replace-icon" aria-hidden="true" />{FileUploadTexts.ReplaceButton}</button></li>
                <li><button type="button" class="dropdown-item text-danger file-remove" onClick={() => this.removeFile()}>
                    <span class="file-remove-icon" aria-hidden="true" />{FileUploadTexts.DeleteButtonHint}</button></li>
            </ul>
        </div> as HTMLElement;
    }

    /**
     * Updates the interface to reflect the current state.
     */
    protected updateInterface(): void {
        const addButton = this.toolbar.findButton('add-file-button');
        const delButton = this.toolbar.findButton('delete-button');
        addButton.toggleClass('disabled', this.get_readOnly());
        delButton.toggleClass('disabled', this.get_readOnly() ||
            this.entity == null);
        // there is nothing to remove without a file
        delButton.toggle(this.entity != null);
        updateFileDropHint(this.dropHint, this.fileSymbols, this.entity == null, this.get_readOnly());
        this.fileSymbols?.querySelector<HTMLElement>('.file-menu')?.toggleAttribute('hidden', this.get_readOnly());
    }

    /**
     * Returns whether the editor is read-only.
     * @returns True when read-only.
     */
    get_readOnly(): boolean {
        return this.uploadInput.attr('disabled') != null;
    }

    /**
     * Sets whether the editor is read-only.
     * @param value - True to enable read-only mode.
     */
    set_readOnly(value: boolean): void {
        if (this.get_readOnly() !== value) {
            const $ = getjQuery();
            if (value) {
                this.uploadInput.attr('disabled', 'disabled');
                try {
                    $ && $(this.uploadInput).fileupload?.('disable');
                }
                catch {
                }
            }
            else {
                this.uploadInput.removeAttr('disabled');
                try {
                    $ && $(this.uploadInput).fileupload?.('enable');
                } catch {
                }
            }
            this.updateInterface();
        }
    }

    /**
     * Returns whether the field is required.
     * @returns True when required.
     */
    get_required(): boolean {
        return this.hiddenInput.classList.contains("required");
    }

    /**
     * Sets whether the field is required.
     * @param value - True when required.
     */
    set_required(value: boolean): void {
        this.hiddenInput.classList.toggle("required", !!value);
    }

    /**
     * Returns the current uploaded file.
     * @returns The uploaded file, or null.
     */
    get_value(): UploadedFile {
        if (this.entity == null) {
            return null;
        }
        const copy = Object.assign(Object.create(null), this.entity);
        return copy;
    }

    /**
     * Returns the current uploaded file.
     * @returns The uploaded file.
     */
    get value(): UploadedFile {
        return this.get_value();
    }

    /**
     * Sets the uploaded file.
     * @param value - The uploaded file to set.
     */
    set_value(value: UploadedFile): void {
        if (typeof value === "string") {
            const stringValue = (value as string).trim();
            if (stringValue) {
                let idx = stringValue.indexOf('/');
                if (idx < 0)
                    idx = stringValue.indexOf('\\');
                value = {
                    Filename: value,
                    OriginalName: stringValue.substring(idx + 1)
                }
            }
            else
                value = null;
        }
        else if (!value?.Filename?.trim())
            value = null;

        if (value != null) {
            this.entity = Object.assign(Object.create(null), value);
        }
        else {
            this.entity = null;
        }
        this.populate();

        ValidationHelper.validateElement(this.hiddenInput);
        this.updateInterface();
    }

    /** Sets the uploaded file.
     * @param v - The uploaded file to set. */
    set value(v: UploadedFile) {
        this.set_value(v);
    }

    /**
     * Gets the edit value into a target object.
     * @param property - The property item.
     * @param target - The target object.
     */
    getEditValue(property: PropertyItem, target: any) {
        target[property.name] = this.entity?.Filename?.trim() || null;
    }

    /**
     * Sets the edit value from a source object.
     * @param source - The source object.
     * @param property - The property item.
     */
    setEditValue(source: any, property: PropertyItem) {
        const value: UploadedFile = {};
        value.Filename = source[property.name];
        if (!this.options.originalNameProperty) {

            if (this.options.displayFileName) {
                const s = (value.Filename ?? '');
                const idx = (s ?? '').replaceAll('\\', '/').lastIndexOf('/');
                if (idx >= 0) {
                    value.OriginalName = s.substring(idx + 1);
                }
                else {
                    value.OriginalName = s;
                }
            }
        }
        else {
            value.OriginalName = source[this.options.originalNameProperty];
        }

        this.set_value(value);
    }

    declare protected entity: UploadedFile;
    declare protected toolbar: Toolbar;
    declare protected progress: HTMLElement;
    declare protected fileSymbols: HTMLElement;
    declare protected uploadInput: Fluent;
    declare protected hiddenInput: HTMLInputElement;
    declare protected dropHint: HTMLButtonElement;
    /** Whether the editor has no toolbar, but the drop box and the menu on the corner of the file. */
    declare protected compact: boolean;
}

/**
 * An editor that uploads and displays a single image.
 * @typeParam P - Widget props type.
 */
export class ImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends FileUploadEditor<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    /**
     * Creates an image upload editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.classList.add("s-ImageUploadEditor")
    }

    /**
     * Whether non-image files are allowed.
     * @returns False for image editors.
     */
    protected override getDefaultAllowNonImage(): boolean {
        return false;
    }
}

/**
 * Options for the {@link MultipleFileUploadEditor}.
 */
export interface MultipleFileUploadEditorOptions extends FileUploadEditorOptions {
    /** Whether to JSON-encode the value. */
    jsonEncodeValue?: boolean;
}

/**
 * An editor that uploads and displays multiple files.
 * @typeParam P - Widget props type.
 */
export class MultipleFileUploadEditor<P extends MultipleFileUploadEditorOptions = MultipleFileUploadEditorOptions> extends EditorWidget<P>
    implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired]);

    declare private entities: UploadedFile[];
    declare private toolbar: Toolbar;
    declare private fileSymbols: HTMLUListElement;
    declare private uploadInput: Fluent;
    declare protected progress: HTMLElement;
    declare protected hiddenInput: HTMLInputElement;
    declare protected dropHint: HTMLButtonElement;
    declare private compact: boolean;

    /**
     * The defaults of the options of the multiple file and image upload editors, e.g. `toolbar`, which an option of an
     * editor overrides.
     */
    static defaults: Pick<MultipleFileUploadEditorOptions, "toolbar"> = {};

    /**
     * Creates a multiple file upload editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.options.allowNonImage ??= this.getDefaultAllowNonImage();

        this.entities = [];
        this.domNode.classList.add('s-MultipleFileUploadEditor');
        const buttons = this.getToolButtons();
        this.toolbar = new Toolbar({
            buttons,
            element: el => this.domNode.append(el)
        }).init();

        // without the toolbar, it is kept hidden, as the file input is in its add button, and subclasses may use it
        this.compact = !this.useToolbar(buttons);
        this.toolbar.domNode.hidden = this.compact;
        this.domNode.classList.toggle('s-upload-compact', this.compact);

        this.progress = (this.compact ? this.domNode : this.toolbar.domNode).appendChild(<div class="upload-progress"><div /></div> as HTMLElement);

        this.uploadInput = UploadHelper.addUploadInput(this.getUploadInputOptions());

        this.fileSymbols = this.domNode.appendChild(<ul class="file-items" /> as HTMLUListElement);
        this.dropHint = this.domNode.appendChild(<FileDropHint text={this.dropHintText()} detail={this.dropHintDetail()} choose={() => this.chooseFile()} /> as HTMLButtonElement);
        if (!this.domNode.id) {
            this.domNode.id = this.uniqueName;
        }

        this.hiddenInput = <input class="s-offscreen" type="text" name={this.uniqueName + "_Validator"} data-vx-highlight={this.domNode.id} multiple={true} /> as HTMLInputElement;

        this.updateInterface();
    }

    /**
     * Returns whether the editor shows its toolbar instead of the drop box and the tile that adds more files, see
     * {@link FileUploadEditorOptions.toolbar}.
     * @param buttons - The buttons of the toolbar, from getToolButtons.
     * @returns True to show the toolbar.
     */
    protected useToolbar(buttons: ToolButton[]): boolean {
        return shouldUseToolbar(this.options.toolbar, MultipleFileUploadEditor.defaults?.toolbar, buttons);
    }

    /**
     * Returns the upload input options.
     * @returns Upload input options.
     */
    protected getUploadInputOptions(): UploadInputOptions {
        const addFileButton = this.toolbar.findButton('add-file-button');

        return {
            container: addFileButton,
            zone: this.domNode,
            inputName: this.uniqueName,
            progress: this.progress,
            uploadIntent: this.options.uploadIntent,
            uploadUrl: this.options.uploadUrl,
            allowMultiple: true,
            fileDone: (response, name) => {
                // a file that was uploading when the editor became read-only
                if (this.get_readOnly())
                    return;

                if (!UploadHelper.checkImageConstraints(response, this.options)) {
                    return;
                }
                const newEntity = { OriginalName: name, Filename: response.TemporaryFile };
                this.entities.push(newEntity);
                this.populate();

                ValidationHelper.validateElement(this.hiddenInput);
                // e.g. a form that tracks its unsaved changes listens to the change event, as for the other editors
                Fluent.trigger(this.domNode, "change");

                this.updateInterface();
            }
        }
    }

    /**
     * Returns the text for the add-file button.
     * @returns The button text.
     */
    protected addFileButtonText(): string {
        return FileUploadTexts.AddFileButton;
    }

    /**
     * Returns the text that the editor shows while it has no files.
     * @returns The text of the drop hint.
     */
    protected dropHintText(): string {
        return this.options.dropHint ? localText(this.options.dropHint) : FileUploadTexts.DropFilesHint;
    }

    /**
     * Returns the line under the text of the drop box, if any.
     * @returns The detail of the drop hint, from the dropHintDetail option.
     */
    protected dropHintDetail(): string {
        return this.options.dropHintDetail ? localText(this.options.dropHintDetail) : null;
    }

    /**
     * Opens the file chooser, as the add file button does.
     */
    protected chooseFile(): void {
        if (!this.get_readOnly())
            this.uploadInput?.getNode()?.click();
    }

    /**
     * Returns the toolbar buttons for the editor.
     * @returns Tool button definitions.
     */
    protected getToolButtons(): ToolButton[] {
        return [{
            title: this.addFileButtonText(),
            action: 'add-file',
            cssClass: 'add-file-button',
            onClick: function () {
            }
        }];
    }

    /**
     * Populates the file symbols from the current entities.
     */
    protected populate(): void {
        UploadHelper.populateFileSymbols(this.fileSymbols, this.entities,
            true, this.options.urlPrefix);

        this.fileSymbols.childNodes.forEach((e, i) => {
            const x = i;
            Fluent(<a class="delete" />).appendTo(Fluent(e).children().find(x => x.matches('.filename')))
                .on("click", ev => {
                    ev.preventDefault();
                    this.entities.splice(x, 1);
                    this.populate();
                    this.updateInterface();

                    ValidationHelper.validateElement(this.hiddenInput);
                    // e.g. a form that tracks its unsaved changes listens to the change event, as for the other editors
                    Fluent.trigger(this.domNode, "change");
                });
        });

        // without the toolbar, a tile after the files adds more
        if (this.compact && this.entities.length) {
            this.fileSymbols.appendChild(<li class="file-add-tile" hidden={this.get_readOnly()}>
                <button type="button" onClick={() => this.chooseFile()}>
                    <span class="file-drop-icon" aria-hidden="true" />
                    <span>{FileUploadTexts.AddMoreButton}</span>
                </button>
            </li>);
        }

        this.hiddenInput.value = this.get_value()[0]?.Filename || null;
    }

    /**
     * Updates the interface to reflect the current state.
     */
    protected updateInterface(): void {
        const addButton = this.toolbar.findButton('add-file-button');
        addButton.toggleClass('disabled', this.get_readOnly());
        this.fileSymbols.querySelectorAll('a.delete').forEach(x => Fluent(x).toggle(!this.get_readOnly()));
        updateFileDropHint(this.dropHint, this.fileSymbols, !this.entities.length, this.get_readOnly());
        this.fileSymbols.querySelector<HTMLElement>('.file-add-tile')?.toggleAttribute('hidden', this.get_readOnly());
    }

    /**
     * Returns whether the editor is read-only.
     * @returns True when read-only.
     */
    get_readOnly(): boolean {
        return this.uploadInput.attr('disabled') != null;
    }

    /**
     * Sets whether the editor is read-only.
     * @param value - True to enable read-only mode.
     */
    set_readOnly(value: boolean): void {
        if (this.get_readOnly() !== value) {
            const $ = getjQuery();
            if (value) {
                this.uploadInput.attr('disabled', 'disabled');
                try {
                    $ && $(this.uploadInput).fileupload?.('disable');
                }
                catch {
                }
            }
            else {
                this.uploadInput.removeAttr('disabled');
                try {
                    $ && $(this.uploadInput)?.fileupload('enable');
                } catch {
                }
            }
            this.updateInterface();
        }
    }

    /**
     * Returns whether the field is required.
     * @returns True when required.
     */
    get_required(): boolean {
        return this.hiddenInput.classList.contains('required');
    }

    /**
     * Sets whether the field is required.
     * @param value - True when required.
     */
    set_required(value: boolean): void {
        this.hiddenInput && this.hiddenInput.classList.toggle('required', !!value);
    }

    /**
     * Returns the current uploaded files.
     * @returns The uploaded files.
     */
    get_value(): UploadedFile[] {
        return this.entities.map(function (x) {
            return Object.assign(Object.create(null), x);
        });
    }

    /**
     * Returns the current uploaded files.
     * @returns The uploaded files.
     */
    get value(): UploadedFile[] {
        return this.get_value();
    }

    /**
     * Sets the uploaded files.
     * @param value - The uploaded files to set.
     */
    set_value(value: UploadedFile[]) {
        this.entities = (value || []).map(function (x) {
            return Object.assign(Object.create(null), x);
        });
        this.populate();
        this.updateInterface();
    }

    /** Sets the uploaded files.
     * @param v - The uploaded files to set. */
    set value(v: UploadedFile[]) {
        this.set_value(v);
    }

    /**
     * Gets the edit value into a target object.
     * @param property - The property item.
     * @param target - The target object.
     */
    getEditValue(property: PropertyItem, target: any) {
        if (this.jsonEncodeValue) {
            target[property.name] = JSON.stringify(this.get_value());
        }
        else {
            target[property.name] = this.get_value();
        }
    }

    /**
     * Sets the edit value from a source object.
     * @param source - The source object.
     * @param property - The property item.
     */
    setEditValue(source: any, property: PropertyItem) {
        const val = source[property.name];
        if (typeof val == "string") {
            const json = val.trim();
            if (json.startsWith('[') && json.endsWith(']')) {
                this.set_value(JSON.parse(json));
            }
            else {
                this.set_value([{
                    Filename: json,
                    OriginalName: 'UnknownFile'
                }]);
            }
        }
        else {
            this.set_value(val as any);
        }
    }

    /**
     * Whether the value is JSON-encoded.
     * @returns True when JSON-encoded.
     */
    public get jsonEncodeValue() { return this.options.jsonEncodeValue }
    /** Sets whether the value is JSON-encoded.
     * @param value - True to JSON-encode the value. */
    public set jsonEncodeValue(value) { this.options.jsonEncodeValue = value }

    /**
     * Whether non-image files are allowed.
     * @returns True for multiple file editors.
     */
    protected getDefaultAllowNonImage(): boolean {
        return true;
    }
}

/**
 * An editor that uploads and displays multiple images.
 * @typeParam P - Widget props type.
 */
export class MultipleImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends MultipleFileUploadEditor<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates a multiple image upload editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        if (this.options.allowNonImage == null)
            this.options.allowNonImage = this.getDefaultAllowNonImage();

        this.domNode.classList.add("s-MultipleImageUploadEditor")
    }

    /**
     * Whether non-image files are allowed.
     * @returns False for multiple image editors.
     */
    protected override getDefaultAllowNonImage(): boolean {
        return false;
    }
}