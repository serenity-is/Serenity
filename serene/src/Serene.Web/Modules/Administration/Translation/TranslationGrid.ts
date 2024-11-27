import { confirmDialog, Decorators, EntityGrid, Fluent, GridUtils, isEmptyOrNull, isTrimmedEmpty, LookupEditor, LookupEditorOptions, notifySuccess, outerHtml, stripDiacritics, ToolButton, trimToEmpty, trimToNull, Widget } from "@serenity-is/corelib";
import { TranslationItem, TranslationTexts } from "@serenity-is/extensions";
import { Column } from "@serenity-is/sleekgrid";
import { TranslationService } from "../../ServerTypes/Administration";

@Decorators.registerClass()
export class TranslationGrid extends EntityGrid<TranslationItem, any> {
    protected getIdProperty() { return "Key"; }
    protected getLocalTextPrefix() { return "Administration.Translation"; }
    protected getService() { return TranslationService.baseUrl; }

    private hasChanges: boolean;
    private searchText: string;
    private sourceLanguage: LookupEditor;
    private targetLanguage: LookupEditor;
    private targetLanguageKey: string;

    constructor(props: any) {
        super(props);

        this.element.on('keyup.' + this.uniqueName + ' change.' + this.uniqueName,
            'input.custom-text', e =>
        {
            var value = trimToNull(Fluent(e.target).val());
            if (value === '') {
                value = null;
            }
            this.view.getItemById(Fluent(e.target).data('key')).CustomText = value;
            this.hasChanges = true;
        });
    }

    protected onClick(e: MouseEvent, row: number, cell: number): any {
        super.onClick(e, row, cell);

        if (e.defaultPrevented || (e as any)?.isDefaultPrevented?.()) {
            return;
        }

        let item = this.itemAt(row);
        let done: () => void;

        if (Fluent(e.target).hasClass('source-text')) {
            e.preventDefault();

            done = () => {
                item.CustomText = item.SourceText;
                this.view.updateItem(item.Key, item);
                this.hasChanges = true;
            };

            if (isTrimmedEmpty(item.CustomText) ||
                (trimToEmpty(item.CustomText) === trimToEmpty(item.SourceText))) {
                done();
                return;
            }

            confirmDialog(TranslationTexts.OverrideConfirmation, done);
            return;
        }

        if (Fluent(e.target).hasClass('target-text')) {
            e.preventDefault();

            done = () => {
                item.CustomText = item.TargetText;
                this.view.updateItem(item.Key, item);
                this.hasChanges = true;
            };

            if (isTrimmedEmpty(item.CustomText) ||
                (trimToEmpty(item.CustomText) === trimToEmpty(item.TargetText))) {
                done();
                return;
            }

            confirmDialog(TranslationTexts.OverrideConfirmation, done);
            return;
        }
    }

    protected getColumns(): Column[] {

        var columns: Column[] = [];
        columns.push({
            field: 'Key',
            name: TranslationTexts.Key,
            width: 300,
            sortable: false
        });

        columns.push({
            field: 'SourceText',
            name: TranslationTexts.SourceText,
            width: 300,
            sortable: false,
            format: ctx => {
                return outerHtml(Fluent('a')
                    .addClass('source-text')
                    .text(ctx.value || ''));
            }
        });

        columns.push({
            field: 'CustomText',
            name: TranslationTexts.CustomText,
            width: 300,
            sortable: false,
            format: ctx => outerHtml(Fluent('input')
                .addClass('custom-text')
                .attr('value', ctx.value)
                .attr('type', 'text')
                .attr('placeholder', ctx.item.TargetText)
                .attr('data-key', ctx.item.Key))
        });

        return columns;
    }

    protected createToolbarExtensions(): void {
        super.createToolbarExtensions();

        let opt: LookupEditorOptions = {
            lookupKey: 'Administration.Language'
        };

        this.sourceLanguage = Widget.create({
            type: LookupEditor,
            element: el => el.appendTo(this.toolbar.element).attr('placeholder', '--- ' +
                TranslationTexts.SourceLanguage + ' ---'),
            options: opt
        });

        this.sourceLanguage.changeSelect2(e => {
            if (this.hasChanges) {
                this.saveChanges(this.targetLanguageKey).then(() => this.refresh());
            }
            else {
                this.refresh();
            }
        });

        this.targetLanguage = Widget.create({
            type: LookupEditor,
            element: el => el.appendTo(this.toolbar.element).attr('placeholder', '--- ' +
                TranslationTexts.TargetLanguage + ' ---'),
            options: opt
        });

        this.targetLanguage.changeSelect2(e => {
            if (this.hasChanges) {
                this.saveChanges(this.targetLanguageKey).then(() => this.refresh());
            }
            else {
                this.refresh();
            }
        });
    }

    protected saveChanges(language: string): PromiseLike<any> {
        var translations: { [key: string]: string } = {};
        for (let item of this.getItems()) {
            translations[item.Key] = item.CustomText;
        }

        return Promise.resolve(TranslationService.Update({
            TargetLanguageID: language,
            Translations: translations
        })).then(() => {
            this.hasChanges = false;
            language = trimToNull(language) || 'invariant';
            notifySuccess('User translations in "' + language +
                '" language are saved to "user.texts.' +
                language + '.json" ' + 'file under "~/App_Data/texts/"', '');
        });
    }

    protected onViewSubmit(): boolean {
        var request = this.view.params;
        request.SourceLanguageID = this.sourceLanguage.value;
        this.targetLanguageKey = this.targetLanguage.value || '';
        request.TargetLanguageID = this.targetLanguageKey;
        this.hasChanges = false;
        return super.onViewSubmit();
    }

    protected getButtons(): ToolButton[] {
        return [{
            title: TranslationTexts.SaveChangesButton,
            onClick: e => this.saveChanges(this.targetLanguageKey).then(() => this.refresh()),
            cssClass: 'apply-changes-button'
        }];
    }

    protected createQuickSearchInput() {
        GridUtils.addQuickSearchInputCustom(this.toolbar.element,
            (field, searchText) => {
                this.searchText = searchText;
                this.view.setItems(this.view.getItems(), true);
            });
    }

    protected onViewFilter(item: TranslationItem) {
        if (!super.onViewFilter(item)) {
            return false;
        }

        if (!this.searchText) {
            return true;
        }

        var sd = stripDiacritics;
        var searching = sd(this.searchText).toLowerCase();

        function match(str: string) {
            if (!str)
                return false;

            return str.toLowerCase().indexOf(searching) >= 0;
        }

        return isEmptyOrNull(searching) || match(item.Key) || match(item.SourceText) ||
            match(item.TargetText) || match(item.CustomText);
    }

    protected usePager() {
        return false;
    }
}