import { NoteRow } from "../ServerTypes/Demo";
import { Authorization, Decorators, EditorWidget, IGetEditValue, ISetEditValue, PropertyItem, Toolbar, confirmDialog, formatDate, formatISODateTimeUTC, insert } from "@serenity-is/corelib";
import DOMPurify from 'dompurify';
import { NoteDialog } from "./NoteDialog";
import "./NotesEditor.css";

@Decorators.registerEditor('Serenity.Demo.Northwind.NotesEditor', [IGetEditValue, ISetEditValue])
@Decorators.element("<div/>")
export class NotesEditor<P = {}> extends EditorWidget<P>
    implements IGetEditValue, ISetEditValue {

    declare private isDirty: boolean;
    declare private items: NoteRow[];
    declare private noteList: HTMLUListElement;

    protected renderContents(): any {
        let id = this.useIdPrefix();
        return (<div>
            <Toolbar id={id.Toolbar} buttons={[{
                title: 'Add Note',
                cssClass: 'add-button',
                onClick: (e: Event) => {
                    e.preventDefault();
                    this.addClick();
                }
            }]} />
            <ul id={id.NoteList} ref={el => this.noteList = el}></ul>
        </div>);
    }

    protected updateContent() {
        this.noteList.innerHTML = '';
        this.noteList.append(<>{(this.items || []).map((item, index) => 
            <li>
                <div class="note-text" dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(item.Text ?? '') }}></div>
                <a href="#" class="note-date" data-index={index} onClick={this.editClick.bind(this)}>
                    {item.InsertUserDisplayName + ' - ' + formatDate(item.InsertDate, 'g')}
                </a>
                <a href="#" class="note-delete" data-index={index} title="delete note" onClick={this.deleteClick.bind(this)} />
            </li>
        )}</>);
    }

    protected addClick() {
        var dlg = new NoteDialog({});
        dlg.dialogTitle = 'Add Note';
        dlg.okClick = () => {
            var text = dlg.text?.trim();
            if (!text)
                return;

            this.items = this.items || [];
            insert(this.items, 0, {
                Text: text,
                InsertUserDisplayName: Authorization.userDefinition.DisplayName,
                InsertDate: formatISODateTimeUTC(new Date())
            });

            this.updateContent();
            dlg.dialogClose("ok");
            this.set_isDirty(true);
            this.onChange && this.onChange();
        };
        dlg.dialogOpen();
    }

    protected editClick(e) {
        e.preventDefault();
        var index = e.target.dataset.index;
        var old = this.items[index];
        var dlg = new NoteDialog({});
        dlg.dialogTitle = 'Edit Note';
        dlg.text = old.Text;
        dlg.okClick = () => {
            var text = dlg.text?.trim();;
            if (!text)
                return;

            this.items[index].Text = text;
            this.updateContent();
            dlg.dialogClose("ok");
            this.set_isDirty(true);
            this.onChange && this.onChange();
        };
        dlg.dialogOpen();
    }

    public deleteClick(e) {
        e.preventDefault();
        var index = e.target.dataset.index;
        confirmDialog('Delete this note?', () => {
            this.items.splice(index, 1);
            this.updateContent();
            this.set_isDirty(true);
            this.onChange && this.onChange();
        });
    }

    public get value() {
        return this.items;
    }

    public set value(value: NoteRow[]) {
        this.items = value || [];
        this.set_isDirty(false);
        this.updateContent();
    }

    public getEditValue(prop: PropertyItem, target) {
        target[prop.name] = this.value;
    }

    public setEditValue(source, prop: PropertyItem) {
        this.value = source[prop.name] || [];
    }

    public get_isDirty(): boolean {
        return this.isDirty;
    }

    public set_isDirty(value): void {
        this.isDirty = value;
    }

    declare public onChange: () => void;
}
