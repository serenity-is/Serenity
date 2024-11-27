import { Fluent, PropertyItem, getjQuery, localText } from "../../base";
import { IGetEditValue, IReadOnly, ISetEditValue, IValidateRequired } from "../../interfaces";
import { ValidationHelper, extend, isTrimmedEmpty, replaceAll } from "../../q";
import { Decorators } from "../../types/decorators";
import { FileUploadConstraints, UploadHelper, UploadInputOptions, UploadedFile } from "../helpers/uploadhelper";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { EditorProps, EditorWidget } from "./editorwidget";

export interface FileUploadEditorOptions extends FileUploadConstraints {
    displayFileName?: boolean;
    uploadIntent?: string;
    uploadUrl?: string;
    urlPrefix?: string;
}

export interface ImageUploadEditorOptions extends FileUploadEditorOptions {
}

@Decorators.registerEditor('Serenity.FileUploadEditor', [IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired])
export class FileUploadEditor<P extends FileUploadEditorOptions = FileUploadEditorOptions> extends EditorWidget<P>
    implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {

    constructor(props: EditorProps<P>) {
        super(props);

        if (!this.options || this.options.allowNonImage == null)
            this.options.allowNonImage = true;

        this.domNode.classList.add('s-FileUploadEditor');

        if (!this.options.originalNameProperty)
            this.domNode.classList.add('hide-original-name');

        this.toolbar = new Toolbar({
            buttons: this.getToolButtons(),
            element: el => this.domNode.appendChild(el)
        }).init();

        this.progress = this.toolbar.domNode.appendChild(<div class="upload-progress"><div/></div> as HTMLElement);

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

    protected addFileButtonText(): string {
        return localText('Controls.ImageUpload.AddFileButton');
    }

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
                hint: localText('Controls.ImageUpload.DeleteButtonHint'),
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

    protected updateInterface(): void {
        var addButton = this.toolbar.findButton('add-file-button');
        var delButton = this.toolbar.findButton('delete-button');
        addButton.toggleClass('disabled', this.get_readOnly());
        delButton.toggleClass('disabled', this.get_readOnly() ||
            this.entity == null);
    }

    get_readOnly(): boolean {
        return this.uploadInput.attr('disabled') != null;
    }

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

    get_required(): boolean {
        return this.hiddenInput.classList.contains("required");
    }

    set_required(value: boolean): void {
        this.hiddenInput.classList.toggle("required", !!value);
    }

    get_value(): UploadedFile {
        if (this.entity == null) {
            return null;
        }
        var copy = extend({}, this.entity);
        return copy;
    }

    get value(): UploadedFile {
        return this.get_value();
    }

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
            if (value.Filename == null) {
                this.entity = null;
            }
            else {
                this.entity = extend({}, value);
            }
        }
        else {
            this.entity = null;
        }
        this.populate();

        ValidationHelper.validateElement(this.hiddenInput);
        this.updateInterface();
    }

    set value(v: UploadedFile) {
        this.set_value(v);
    }

    getEditValue(property: PropertyItem, target: any) {
        target[property.name] = this.entity?.Filename?.trim() || null;
    }

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

@Decorators.registerEditor('Serenity.ImageUploadEditor')
export class ImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends FileUploadEditor<P> {
    constructor(props: EditorProps<P>) {
        super(props);

        if (this.options.allowNonImage == null)
            this.options.allowNonImage = false;

        this.domNode.classList.add("s-ImageUploadEditor'")
    }
}

export interface MultipleFileUploadEditorOptions extends FileUploadEditorOptions {
    jsonEncodeValue?: boolean;
}

@Decorators.registerEditor('Serenity.MultipleFileUploadEditor', [IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired])
export class MultipleFileUploadEditor<P extends MultipleFileUploadEditorOptions = MultipleFileUploadEditorOptions> extends EditorWidget<P>
    implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {

    declare private entities: UploadedFile[];
    declare private toolbar: Toolbar;
    declare private fileSymbols: HTMLUListElement;
    declare private uploadInput: Fluent;
    declare protected progress: HTMLElement;
    declare protected hiddenInput: HTMLInputElement;

    constructor(props: EditorProps<P>) {
        super(props);

        this.entities = [];
        this.domNode.classList.add('s-MultipleFileUploadEditor');
        this.toolbar = new Toolbar({
            buttons: this.getToolButtons(),
            element: el => this.domNode.append(el)
        }).init();

        this.progress = this.toolbar.domNode.appendChild(<div class="upload-progress"><div/></div> as HTMLElement);

        this.uploadInput = UploadHelper.addUploadInput(this.getUploadInputOptions());

        this.fileSymbols = this.domNode.appendChild(<ul class="file-items" /> as HTMLUListElement);
        if (!this.domNode.id) {
            this.domNode.id = this.uniqueName;
        }

        this.hiddenInput = <input class="s-offscreen" type="text" name={this.uniqueName + "_Validator"} data-vx-highlight={this.domNode.id} multiple={true} /> as HTMLInputElement;

        this.updateInterface();
    }

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

    protected addFileButtonText(): string {
        return localText('Controls.ImageUpload.AddFileButton');
    }

    protected getToolButtons(): ToolButton[] {
        return [{
            title: this.addFileButtonText(),
            action: 'add-file',
            cssClass: 'add-file-button',
            onClick: function () {
            }
        }];
    }

    protected populate(): void {
        UploadHelper.populateFileSymbols(this.fileSymbols, this.entities,
            true, this.options.urlPrefix);

        this.fileSymbols.childNodes.forEach((e, i) => {
            var x = i;
            Fluent(<a class="delete"/>).appendTo(Fluent(e).children().find(x => x.matches('.filename')))
                .on("click", ev => {
                    ev.preventDefault();
                    this.entities.splice(x, 1);
                    this.populate();

                    ValidationHelper.validateElement(this.hiddenInput);
                });
        });

        this.hiddenInput.value = this.get_value()[0]?.Filename || null;
    }

    protected updateInterface(): void {
        var addButton = this.toolbar.findButton('add-file-button');
        addButton.toggleClass('disabled', this.get_readOnly());
        this.fileSymbols.querySelectorAll('a.delete').forEach(x => Fluent(x).toggle(!this.get_readOnly()));
    }

    get_readOnly(): boolean {
        return this.uploadInput.attr('disabled') != null;
    }

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

    get_required(): boolean {
        return this.hiddenInput.classList.contains('required');
    }

    set_required(value: boolean): void {
        this.hiddenInput && this.hiddenInput.classList.toggle('required', !!value);
    }

    get_value(): UploadedFile[] {
        return this.entities.map(function (x) {
            return extend({}, x);
        });
    }

    get value(): UploadedFile[] {
        return this.get_value();
    }

    set_value(value: UploadedFile[]) {
        this.entities = (value || []).map(function (x) {
            return extend({}, x);
        });
        this.populate();
        this.updateInterface();
    }

    set value(v: UploadedFile[]) {
        this.set_value(v);
    }

    getEditValue(property: PropertyItem, target: any) {
        if (this.jsonEncodeValue) {
            target[property.name] = JSON.stringify(this.get_value());
        }
        else {
            target[property.name] = this.get_value();
        }
    }

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

    public get jsonEncodeValue() { return this.options.jsonEncodeValue }
    public set jsonEncodeValue(value) { this.options.jsonEncodeValue = value }
}

@Decorators.registerEditor('Serenity.MultipleImageUploadEditor')
export class MultipleImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends MultipleFileUploadEditor<P> {
    constructor(props: EditorProps<P>) {
        super(props);
        this.domNode.classList.add("s-MultipleImageUploadEditor'")
    }
}