import { Fluent, localText, PropertyItem, SaveRequest, serviceCall, ServiceOptions } from "../../base";
import { extend, safeCast } from "../../q/system-compat";
import { PropertyGrid, PropertyGridOptions } from "../widgets/propertygrid";
import { Toolbar } from "../widgets/toolbar";

export interface EntityLocalizerOptions {
    validateForm: () => boolean,
    byId: (id: string) => Fluent,
    idPrefix: string,
    isNew: () => boolean,
    getButton: () => Fluent;
    getEntity: () => any;
    getEntityId: () => any,
    getIdProperty: () => string,
    getLanguages: () => any[],
    getRetrieveServiceMethod: () => string,
    getPropertyGrid: () => Fluent,
    pgOptions: PropertyGridOptions,
    getToolButtons: () => HTMLElement[]
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
        const langs = this.getLangs();

        this.targetLanguage = <select class="target-language ms-2" style="display: none">
            <option value="">--{localText("Site.Translation.TargetLanguage")}--</option>
            {langs.map(x => <option value={x[0]}>{x[1]}</option>)}
        </select> as HTMLSelectElement;
        this.options.getButton()?.after(this.targetLanguage);

        const targetLang = localStorage.getItem("EntityLocalizer.TargetLanguage");
        if (langs.some(x => x[0] == targetLang)) {
            this.targetLanguage.value = targetLang;
        }

        Fluent.on(this.targetLanguage, "change", () => {
            const val = this.targetLanguage?.value;
            this.grid.element.findAll('.translation').forEach(x => x.closest(".field")?.classList.toggle('hidden', !!val && !x.classList.contains('language-' + val)));
            localStorage.setItem("EntityLocalizer.TargetLanguage", val);
        });

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
                        name: lang[0] + '$' + item.name,
                        title: lang[1],
                        cssClass: Fluent.toClassName([item.cssClass, 'translation', 'language-' + lang[0]]),
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

    protected isEnabled(): boolean {
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

    // for compatibility with older getLanguages methods written in Saltaralle
    private getLangs(): string[][] {

        var langsTuple = this.options.getLanguages();
        var langs = safeCast(langsTuple, Array);
        if (langs == null || langs.length === 0 ||
            langs[0] == null || !Array.isArray(langs[0])) {
            langs = Array.prototype.slice.call(langsTuple.map(function (x: any) {
                return [x.item1, x.item2];
            }));
        }

        return langs;
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

        var opt: ServiceOptions<any> = {
            service: this.options.getRetrieveServiceMethod(),
            blockUI: true,
            request: {
                EntityId: this.options.getEntityId(),
                ColumnSelection: 'keyOnly',
                IncludeColumns: ['Localizations']
            },
            onSuccess: response => {
                var copy = extend(new Object(), this.options.getEntity());
                if (response.Localizations) {
                    for (var language of Object.keys(response.Localizations)) {
                        var entity = response.Localizations[language];
                        for (var key of Object.keys(entity)) {
                            (copy as any)[language + '$' + key] = entity[key];
                        }
                    }
                }

                this.grid.load(copy);
                this.setLocalizationGridCurrentValues();
                this.pendingValue = null;
                this.lastValue = this.getLocalizationGridValue();
            }
        };

        serviceCall(opt);
    }

    protected setLocalizationGridCurrentValues(): void {
        var valueByName: Record<string, any> = {};

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

    public adjustSaveRequest(req: SaveRequest<any>) {
        if (this.pendingValue != null) {
            req.Localizations = this.getPendingLocalizations();
        }
    }

    protected getPendingLocalizations(): any {
        if (this.pendingValue == null) {
            return null;
        }

        var result: { [key: string]: any } = {};
        var idField = this.options.getIdProperty();
        var langs = this.getLangs();

        for (var pair of langs) {
            var language = pair[0];
            var entity: any = {};

            if (idField != null) {
                entity[idField] = this.options.getEntityId();
            }

            var prefix = language + '$';

            for (var k of Object.keys(this.pendingValue)) {
                if (k.startsWith(prefix))
                    entity[k.substring(prefix.length)] = this.pendingValue[k];
            }

            result[language] = entity;
        }

        return result;
    }

    public updateInterface(): void {

        const button = this.options.getButton()?.toggle(this.isEnabled());
        if (!this.isEnabled())
            return;


        const locMode = this.isLocalizationMode();
        Fluent.toggle(this.targetLanguage, locMode);
        button?.findFirst('.button-inner').text((locMode ? localText('Controls.EntityDialog.LocalizationBack') :
            localText('Controls.EntityDialog.LocalizationButton')));

        this.options.getPropertyGrid()?.toggle(!locMode);
        Fluent.toggle(this.grid?.domNode, locMode);

        const buttons = this.options.getToolButtons().filter(x => x != button?.getNode());
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