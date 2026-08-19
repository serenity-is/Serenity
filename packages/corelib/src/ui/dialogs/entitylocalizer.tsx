import { cssEscape, EntityDialogTexts, faIcon, Fluent, LanguageList, localText, PropertyItem, SaveRequest, TranslationConfig } from "../../base";
import { PropertyGrid, PropertyGridOptions } from "../widgets/propertygrid";

/**
 * Options for the {@link EntityLocalizer}.
 */
export interface EntityLocalizerOptions {
    /** Resolves an element by id within the dialog. */
    byId: (id: string) => Fluent,
    /** Id prefix used for generated elements. */
    idPrefix: string,
    /** Whether the entity is new (no id). */
    isNew: () => boolean,
    /** Returns the localization toggle button. */
    getButton: () => Fluent;
    /** Returns the current entity. */
    getEntity: () => any;
    /** Returns the list of available languages. */
    getLanguages: () => LanguageList,
    /** Returns the property grid element. */
    getPropertyGrid: () => Fluent,
    /** Returns the toolbar button elements. */
    getToolButtons: () => HTMLElement[]
    /** Options for the localization property grid. */
    pgOptions: PropertyGridOptions,
    /** Retrieves existing localizations for the entity. */
    retrieveLocalizations: () => PromiseLike<{ [languageId: string]: any }>,
    /** Validates the main form before switching modes. */
    validateForm: () => boolean,
}

/**
 * Manages the localization grid for an entity dialog, letting users edit
 * translations of localizable fields for each language.
 */
export class EntityLocalizer {

    declare protected grid: PropertyGrid;
    declare protected pendingValue: any;
    declare protected lastValue: any;
    declare protected targetLanguage: HTMLSelectElement;

    private options: EntityLocalizerOptions;

    /**
     * Creates a localizer and builds the localization grid.
     * @param opt - Options for the localizer.
     */
    constructor(opt: EntityLocalizerOptions) {
        this.options = opt;
        const { pgOptions, idPrefix } = opt;
        const pgDiv = this.options.getPropertyGrid();

        if (!pgDiv?.getNode() ||
            !pgOptions.items.some(x => x.localizable === true))
            return;

        const localGridDiv = <div id={idPrefix + 'LocalizationGrid'} hidden /> as HTMLDivElement;
        pgDiv.after(localGridDiv);
        const langs = this.options.getLanguages() || [];

        this.targetLanguage = <select class="target-language ms-2" hidden>
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
                    skipOnSave: true,
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
                        skipOnSave: false,
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
                                SourceText: this.grid.element.findFirst("[name=" + cssEscape(input.name.split('$')[1]) + "]").val() || input.placeholder,
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
            this.grid.element.findAll('.translation').forEach(x => Fluent.toggle(x.closest(".field"), !val || x.classList.contains('language-' + val)));
        }
        targetLanguageUpdate();

        Fluent.on(this.targetLanguage, "change", () => {
            targetLanguageUpdate();
            localStorage.setItem("EntityLocalizer.TargetLanguage", this.targetLanguage?.value);
        });
    }

    /**
     * Destroys the localization grid.
     */
    destroy() {
        if (this.grid) {
            this.grid.destroy();
            this.grid = null;
        }
    }

    /**
     * Clears pending and last localization values.
     */
    public clearValue(): void {
        this.pendingValue = null;
        this.lastValue = null;
    }

    /**
     * Whether the localization grid is enabled (there are localizable fields).
     * @returns True when enabled.
     */
    public isEnabled(): boolean {
        return !!this.grid;
    }

    /**
     * Whether the dialog is currently in localization mode.
     * @returns True when in localization mode.
     */
    protected isLocalizationMode(): boolean {
        return !!(this.isEnabled() && this.options.getButton()?.hasClass('pressed'));
    }

    /**
     * Whether the localization values changed since the last save.
     * @returns True when changed.
     */
    protected isLocalizationModeAndChanged(): boolean {
        if (!this.isLocalizationMode()) {
            return false;
        }

        var newValue = this.getLocalizationGridValue();
        return JSON.stringify(this.lastValue) != JSON.stringify(newValue);
    }

    /**
     * Toggles localization mode and loads/saves localization values.
     */
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

    /**
     * Loads localization values into the grid.
     */
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
            const copy = Object.assign(Object.create(null), this.options.getEntity());
            Object.entries(localizations ?? {}).forEach(([language, entity]) =>
                Object.entries(entity ?? {}).forEach(([field, value]) =>
                    copy[language + '$' + field] = value));

            this.grid.load(copy);
            this.setLocalizationGridCurrentValues();
            this.pendingValue = null;
            this.lastValue = this.getLocalizationGridValue();
        });
    }

    /**
     * Copies current field values into the localization grid as hints.
     */
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

    /**
     * Returns the localization values from the grid, keyed by language and field.
     * @returns The localization values.
     */
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

    /**
     * Adds pending localizations to a save request.
     * @param req - The save request to modify.
     */
    public editSaveRequest(req: SaveRequest<any>) {
        if (this.pendingValue != null) {
            req.Localizations = this.getPendingLocalizations();
        }
    }

    /**
     * Returns pending localizations grouped by language.
     * @returns The pending localizations.
     */
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

    /**
     * Updates the UI to reflect the current localization mode.
     */
    public updateInterface(): void {

        if (!this.isEnabled())
            return;

        const button = this.options.getButton()?.getNode();
        const locMode = this.isLocalizationMode();
        Fluent.toggle(this.targetLanguage, locMode);
        const inner = button?.querySelector('.button-inner');
        inner && (inner.textContent = ((locMode ? localText(EntityDialogTexts.LocalizationBack) :
            localText(EntityDialogTexts.LocalizationButton))));

        this.options.getPropertyGrid()?.toggle(!locMode);
        Fluent.toggle(this.grid?.domNode, locMode);
        Fluent.toggle(this.targetLanguage, locMode);

        const buttons = this.options.getToolButtons().filter(x => x !== button);
        if (locMode) {
            buttons?.filter(x => !x.classList.contains("localization-hidden")).forEach(el => {
                el.classList.add('localization-hidden');
                el.hidden = true;
            });
        }
        else {
            buttons?.filter(x => x.classList.contains("localization-hidden")).forEach(el => {
                el.classList.remove('localization-hidden');
                el.hidden = false;
            });
        }
    }
}