import { faIcon, Fluent, LanguageList, localText, PropertyItem, SaveRequest, TranslationConfig } from "../../base";
import { extend } from "../../compat/system-compat";
import { PropertyGrid, PropertyGridOptions } from "../widgets/propertygrid";

export interface EntityLocalizerOptions {
    byId: (id: string) => Fluent,
    idPrefix: string,
    isNew: () => boolean,
    getButton: () => Fluent;
    getEntity: () => any;
    getLanguages: () => LanguageList,
    getPropertyGrid: () => Fluent,
    getToolButtons: () => HTMLElement[]
    pgOptions: PropertyGridOptions,
    retrieveLocalizations: () => PromiseLike<{ [ languageId: string ]: any}>,
    validateForm: () => boolean,
}

export class EntityLocalizer {

    declare protected grid: PropertyGrid;
    declare protected pendingValue: any;
    declare protected lastValue: any;
    declare protected targetLanguage: HTMLSelectElement;

    private options: EntityLocalizerOptions;

    constructor(opt: EntityLocalizerOptions) {
        this.options = opt;
        const { pgOptions, idPrefix } = opt;
        const pgDiv = this.options.getPropertyGrid();

        if (!pgDiv?.getNode() ||
            !pgOptions.items.some(x => x.localizable === true))
            return;

        const localGridDiv = <div id={idPrefix + 'LocalizationGrid'} style="display: none" /> as HTMLDivElement;
        pgDiv.after(localGridDiv);
        const langs = this.options.getLanguages() || [];

        this.targetLanguage = <select class="target-language ms-2" style="display: none">
            <option value="">--{localText("Site.Translation.TargetLanguage")}--</option>
            {langs.map(x => <option value={x.id}>{x.text}</option>)}
        </select> as HTMLSelectElement;
        this.options.getButton()?.after(this.targetLanguage);

        const targetLang = localStorage.getItem("EntityLocalizer.TargetLanguage");
        if (langs.some(x => x.id == targetLang)) {
            this.targetLanguage.value = targetLang;
        }

        pgOptions.idPrefix = idPrefix + 'Localization_';

        var items: PropertyItem[] = [];
        for (var item of pgOptions.items) {

            if (item.localizable === true) {
                items.push(Object.assign({} as PropertyItem, item, {
                    oneWay: true,
                    readOnly: true,
                    required: false,
                    defaultValue: null
                }));

                for (var lang of langs) {
                    items.push(Object.assign({} as PropertyItem, item, {
                        name: lang.id + '$' + item.name,
                        title: lang.text,
                        cssClass: Fluent.toClassName([item.cssClass, 'translation', 'language-' + lang.id]),
                        insertable: true,
                        updatable: true,
                        oneWay: false,
                        required: false,
                        localizable: false,
                        defaultValue: null
                    }));
                }
            }
        }

        pgOptions.items = items;

        this.grid = (new PropertyGrid({ element: localGridDiv, ...pgOptions })).init();
        localGridDiv.classList.add('s-LocalizationGrid');

        if (TranslationConfig.translateTexts) {
            this.grid.element.findAll<HTMLInputElement>("input[type=text].editor").forEach(input => {
                if (!input.name || input.name.indexOf('$') < 0)
                    return;
                const div = <div class="input-group w-100" /> as HTMLElement;
                Fluent(div).insertBefore(input);
                input.classList.add("form-control");
                div.append(input);
                div.append(
                    <button class="btn btn-primary btn-sm" title={localText("Site.Translation.TranslateText")} onClick={() => {
                        TranslationConfig.translateTexts({
                            Inputs: [{
                                SourceText: this.grid.element.findFirst("[name=" + CSS.escape(input.name.split('$')[1]) + "]").val() || input.placeholder,
                                TargetLanguageID: input.name.split('$')[0],
                            }]
                        }).then(result => {
                            if (result.Translations && result.Translations.length > 0) {
                                input.value = result.Translations[0].TranslatedText;
                            }
                        });
                    }}>
                        <span class={faIcon("language")} />
                    </button>);
            });
        }

        const targetLanguageUpdate = () => {
            const val = this.targetLanguage?.value;
            this.grid.element.findAll('.translation').forEach(x => x.closest(".field")?.classList.toggle('hidden', !!val && !x.classList.contains('language-' + val)));
        }
        targetLanguageUpdate();

        Fluent.on(this.targetLanguage, "change", () => {
            targetLanguageUpdate();
            localStorage.setItem("EntityLocalizer.TargetLanguage", this.targetLanguage?.value);
        });        
    }

    destroy() {
        if (this.grid) {
            this.grid.destroy();
            this.grid = null;
        }
    }

    public clearValue(): void {
        this.pendingValue = null;
        this.lastValue = null;
    }

    public isEnabled(): boolean {
        return !!this.grid;
    }

    protected isLocalizationMode(): boolean {
        return !!(this.isEnabled() && this.options.getButton()?.hasClass('pressed'));
    }

    protected isLocalizationModeAndChanged(): boolean {
        if (!this.isLocalizationMode()) {
            return false;
        }

        var newValue = this.getLocalizationGridValue();
        return JSON.stringify(this.lastValue) != JSON.stringify(newValue);
    }

    public buttonClick(): void {
        if (this.isLocalizationMode() && !this.options.validateForm()) {
            return;
        }

        if (this.isLocalizationModeAndChanged()) {
            var newValue = this.getLocalizationGridValue();
            this.lastValue = newValue;
            this.pendingValue = newValue;
        }

        this.options.getButton()?.toggleClass('pressed');
        this.updateInterface();
        if (this.isLocalizationMode()) {
            this.loadLocalization();
        }
    }

    protected loadLocalization(): void {
        if (this.lastValue == null && this.options.isNew()) {
            this.grid.load({});
            this.setLocalizationGridCurrentValues();
            this.lastValue = this.getLocalizationGridValue();
            return;
        }

        if (this.lastValue != null) {
            this.grid.load(this.lastValue);
            this.setLocalizationGridCurrentValues();
            return;
        }

        this.options.retrieveLocalizations().then(localizations => {
            const copy = extend(new Object(), this.options.getEntity());
            Object.entries(localizations ?? {}).forEach(([language, entity]) => 
                Object.entries(entity ?? {}).forEach(([field, value]) =>
                    copy[language + '$' + field] = value));

            this.grid.load(copy);
            this.setLocalizationGridCurrentValues();
            this.pendingValue = null;
            this.lastValue = this.getLocalizationGridValue();
        });
    }

    protected setLocalizationGridCurrentValues(): void {
        const valueByName: Record<string, any> = {};

        this.grid.enumerateItems((item, widget) => {
            if (item.name.indexOf('$') < 0 && Fluent.isInputLike(widget.domNode)) {
                valueByName[item.name] = this.options.byId(item.name).val();
                widget.element.val(valueByName[item.name]);
            }
        });

        this.grid.enumerateItems((item1, widget1) => {
            var idx = item1.name.indexOf('$');
            if (idx >= 0 && Fluent.isInputLike(widget1.domNode)) {
                var hint = valueByName[item1.name.substring(idx + 1)];
                if (hint != null && hint.length > 0) {
                    widget1.element.attr('title', hint).attr('placeholder', hint);
                }
            }
        });
    }

    protected getLocalizationGridValue(): any {
        var value: any = {};
        this.grid.save(value);

        for (var k of Object.keys(value)) {
            if (k.indexOf('$') < 0) {
                delete value[k];
            }
        }

        return value;
    }

    public editSaveRequest(req: SaveRequest<any>) {
        if (this.pendingValue != null) {
            req.Localizations = this.getPendingLocalizations();
        }
    }

    protected getPendingLocalizations(): any {
        if (this.pendingValue == null) {
            return null;
        }

        var result: { [key: string]: any } = {};
        var langs = this.options.getLanguages();

        for (var lang of langs) {
            var entity: any = {};
            var prefix = lang.id + '$';

            for (var k of Object.keys(this.pendingValue)) {
                if (k.startsWith(prefix))
                    entity[k.substring(prefix.length)] = this.pendingValue[k];
            }

            result[lang.id] = entity;
        }

        return result;
    }

    public updateInterface(): void {

        if (!this.isEnabled())
            return;

        const button = this.options.getButton()?.getNode();
        const locMode = this.isLocalizationMode();
        Fluent.toggle(this.targetLanguage, locMode);
        const inner = button?.querySelector('.button-inner');
        inner && (inner.textContent = ((locMode ? localText('Controls.EntityDialog.LocalizationBack') :
            localText('Controls.EntityDialog.LocalizationButton'))));

        this.options.getPropertyGrid()?.toggle(!locMode);
        Fluent.toggle(this.grid?.domNode, locMode);
        Fluent.toggle(this.targetLanguage, locMode);

        const buttons = this.options.getToolButtons().filter(x => x !== button);
        if (locMode) {
            buttons?.filter(x => !x.classList.contains("localization-hidden")).forEach(el => {
                el.classList.add('localization-hidden', 'hidden');
            });
        }
        else {
            buttons?.filter(x => x.classList.contains("localization-hidden")).forEach(el => {
                el.classList.remove('localization-hidden', 'hidden');
            });
        }
    }
}