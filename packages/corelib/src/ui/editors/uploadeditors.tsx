import { FileUploadTexts, Fluent, PropertyItem, getjQuery, nsSerenity } from "../../base";
import { ValidationHelper, isTrimmedEmpty, replaceAll } from "../../compat";
import { IGetEditValue, IReadOnly, ISetEditValue, IValidateRequired } from "../../interfaces";
import { FileUploadConstraints, UploadHelper, UploadInputOptions, UploadedFile } from "../helpers/uploadhelper";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Options for the {@link FileUploadEditor}.
 */
export interface FileUploadEditorOptions extends FileUploadConstraints {
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
     * Creates a file upload editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        if (!this.options || this.options.allowNonImage == null)
            this.options.allowNonImage = this.getDefaultAllowNonImage();

        this.domNode.classList.add('s-FileUploadEditor');

        if (!this.options.originalNameProperty)
            this.domNode.classList.add('hide-original-name');

        this.toolbar = new Toolbar({
            buttons: this.getToolButtons(),
            element: el => this.domNode.appendChild(el)
        }).init();

        this.progress = this.toolbar.domNode.appendChild(<div class="upload-progress"><div /></div> as HTMLElement);

        var uio = this.getUploadInputOptions();
        this.uploadInput = UploadHelper.addUploadInput(uio);
        if (this.options.readOnly)
            this.set_readOnly(true);

        this.fileSymbols = this.domNode.appendChild(<ul class="file-items" /> as HTMLUListElement);

        if (!this.domNode.id) {
            this.domNode.id = this.uniqueName;
        }

        this.hiddenInput = this.domNode.appendChild(<input class="s-offscreen" type="text" name={this.uniqueName + "_Validator"}
            data-vx-highlight={this.domNode.id} /> as HTMLInputElement);

        this.updateInterface();
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
                if (!UploadHelper.checkImageConstraints(response, this.options)) {
                    return;
                }

                var newEntity: UploadedFile = {
                    OriginalName: name,
                    Filename: response.TemporaryFile
                };

                this.entity = newEntity;
                this.populate();
                this.updateInterface();

                ValidationHelper.validateElement(this.hiddenInput);
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
                onClick: () => {
                    this.entity = null;
                    this.populate();
                    this.updateInterface();

                    ValidationHelper.validateElement(this.hiddenInput);
                }
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
        var displayOriginalName = this.options.displayFileName ||
            !isTrimmedEmpty(this.options.originalNameProperty);

        if (this.entity == null) {
            UploadHelper.populateFileSymbols(this.fileSymbols,
                null, displayOriginalName, this.options.urlPrefix);
        }
        else {
            UploadHelper.populateFileSymbols(
                this.fileSymbols, [this.entity], displayOriginalName,
                this.options.urlPrefix);
        }

        this.hiddenInput.value = ((this.get_value() || {}).Filename)?.trim() || null;
    }

    /**
     * Updates the interface to reflect the current state.
     */
    protected updateInterface(): void {
        var addButton = this.toolbar.findButton('add-file-button');
        var delButton = this.toolbar.findButton('delete-button');
        addButton.toggleClass('disabled', this.get_readOnly());
        delButton.toggleClass('disabled', this.get_readOnly() ||
            this.entity == null);
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
            let $ = getjQuery();
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
        var copy = Object.assign(Object.create(null), this.entity);
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
            var stringValue = (value as string).trim();
            if (stringValue) {
                var idx = stringValue.indexOf('/');
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

    /** Sets the uploaded file. */
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
        var value: UploadedFile = {};
        value.Filename = source[property.name];
        if (!this.options.originalNameProperty) {

            if (this.options.displayFileName) {
                var s = (value.Filename ?? '');
                var idx = replaceAll(s, '\\', '/').lastIndexOf('/');
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

        this.options.allowNonImage = this.getDefaultAllowNonImage();

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

    /**
     * Creates a multiple file upload editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.entities = [];
        this.domNode.classList.add('s-MultipleFileUploadEditor');
        this.toolbar = new Toolbar({
            buttons: this.getToolButtons(),
            element: el => this.domNode.append(el)
        }).init();

        this.progress = this.toolbar.domNode.appendChild(<div class="upload-progress"><div /></div> as HTMLElement);

        this.uploadInput = UploadHelper.addUploadInput(this.getUploadInputOptions());

        this.fileSymbols = this.domNode.appendChild(<ul class="file-items" /> as HTMLUListElement);
        if (!this.domNode.id) {
            this.domNode.id = this.uniqueName;
        }

        this.hiddenInput = <input class="s-offscreen" type="text" name={this.uniqueName + "_Validator"} data-vx-highlight={this.domNode.id} multiple={true} /> as HTMLInputElement;

        this.updateInterface();
    }

    /**
     * Returns the upload input options.
     * @returns Upload input options.
     */
    protected getUploadInputOptions(): UploadInputOptions {
        var addFileButton = this.toolbar.findButton('add-file-button');

        return {
            container: addFileButton,
            zone: this.domNode,
            inputName: this.uniqueName,
            progress: this.progress,
            uploadIntent: this.options.uploadIntent,
            uploadUrl: this.options.uploadUrl,
            allowMultiple: true,
            fileDone: (response, name) => {
                if (!UploadHelper.checkImageConstraints(response, this.options)) {
                    return;
                }
                var newEntity = { OriginalName: name, Filename: response.TemporaryFile };
                this.entities.push(newEntity);
                this.populate();

                ValidationHelper.validateElement(this.hiddenInput);

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
            var x = i;
            Fluent(<a class="delete" />).appendTo(Fluent(e).children().find(x => x.matches('.filename')))
                .on("click", ev => {
                    ev.preventDefault();
                    this.entities.splice(x, 1);
                    this.populate();

                    ValidationHelper.validateElement(this.hiddenInput);
                });
        });

        this.hiddenInput.value = this.get_value()[0]?.Filename || null;
    }

    /**
     * Updates the interface to reflect the current state.
     */
    protected updateInterface(): void {
        var addButton = this.toolbar.findButton('add-file-button');
        addButton.toggleClass('disabled', this.get_readOnly());
        this.fileSymbols.querySelectorAll('a.delete').forEach(x => Fluent(x).toggle(!this.get_readOnly()));
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
            let $ = getjQuery();
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

    /** Sets the uploaded files. */
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
        var val = source[property.name];
        if (typeof val == "string") {
            var json = val.trim();
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
    /** Sets whether the value is JSON-encoded. */
    public set jsonEncodeValue(value) { this.options.jsonEncodeValue = value }
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
        this.domNode.classList.add("s-MultipleImageUploadEditor")
    }
}