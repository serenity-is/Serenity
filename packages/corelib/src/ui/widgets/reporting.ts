import { Fluent, PropertyItem, ServiceRequest, ServiceResponse, serviceCall, serviceRequest } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { postToService } from "../../q";
import { QuickSearchInput } from "../datagrid/quicksearchinput";
import { TemplatedDialog } from "../dialogs/templateddialog";
import { PropertyGrid } from "./propertygrid";
import { Widget, WidgetProps } from "./widget";

export namespace Reporting {
    export interface ReportDialogOptions {
        reportKey?: string;
    }

    @Decorators.registerClass('Serenity.Reporting.ReportDialog')
    export class ReportDialog<P extends ReportDialogOptions = ReportDialogOptions> extends TemplatedDialog<P> {
        constructor(props: WidgetProps<P>) {
            super(props);

            if (this.options.reportKey) {
                this.loadReport(this.options.reportKey);
            }
        }

        protected propertyGrid: PropertyGrid;
        protected propertyItems: PropertyItem[];
        protected reportKey: string;

        protected createPropertyGrid(): void {
            if (this.propertyGrid) {
                this.byId('PropertyGrid').html('').attr('class', '');
                this.propertyGrid = null;
            }
            this.propertyGrid = (new PropertyGrid({
                element: this.findById('PropertyGrid'),
                idPrefix: this.idPrefix,
                useCategories: true,
                items: this.propertyItems,
            })).init();
        }

        loadReport(reportKey: string): void {
            serviceRequest("Report/Retrieve", { ReportKey: reportKey }, (response: ReportRetrieveResponse) => {
                this.reportKey = response.ReportKey ?? reportKey;
                this.propertyItems = response.Properties || [];
                this.dialogTitle = response.Title;
                this.createPropertyGrid();
                var set = response.InitialSettings;
                if (set == null) {
                    set = new Object();
                }
                this.propertyGrid.load(set);
                this.toolbar.findButton('print-preview-button').toggle(!response.IsDataOnlyReport);
                this.toolbar.findButton('export-pdf-button').toggle(!response.IsDataOnlyReport);
                this.toolbar.findButton('export-docx-button').toggle(!response.IsDataOnlyReport);
                this.dialogOpen(null);
            });
    }

    executeReport(targetFrame: string, exportType: string): void {
        if(!this.validateForm()) {
        return;
    }
    var parameters = new Object();
    this.propertyGrid.save(parameters);
    postToService({
        service: 'Report/Execute',
        request: {
            ReportKey: this.reportKey,
            DesignId: 'Default',
            ExportType: exportType,
            Parameters: parameters
        }, target: targetFrame
    });

}

        protected getToolbarButtons() {
    var buttons = [];
    buttons.push({
        title: 'Önizleme', cssClass: 'print-preview-button', onClick: () => {
            this.executeReport('_blank', null);
        }
    });
    buttons.push({
        title: 'PDF', cssClass: 'export-pdf-button', onClick: () => {
            this.executeReport('', 'Pdf');
        }
    });
    buttons.push({
        title: 'Excel', cssClass: 'export-xlsx-button', onClick: () => {
            this.executeReport('', 'Xlsx');
        }
    });
    buttons.push({
        title: 'Word', cssClass: 'export-docx-button', onClick: () => {
            this.executeReport('', 'Docx');
        }
    });
    return buttons;
}
    }

export interface ReportExecuteRequest extends ServiceRequest {
    ExportType?: string;
    ReportKey?: string;
    DesignId?: string;
    Parameters?: any;
}

@Decorators.registerClass('Serenity.Reporting.ReportPage')
export class ReportPage<P = {}> extends Widget<P> {
    constructor(props: WidgetProps<P>) {
        super(props);

        Fluent.on(this.domNode, 'click', '.report-link', this.reportLinkClick.bind(this));
        Fluent.on(this.domNode, 'click', 'div.line', this.categoryClick.bind(this));
        var self = this;
        new QuickSearchInput({
            element: "#QuickSearchInput",
            onSearch: function (field, text, done) {
                self.updateMatchFlags(text);
                done(true);
            }
        });
    }

    protected updateMatchFlags(text: string) {
        var liList = Array.from(document.querySelectorAll<HTMLElement>('#ReportList li'));
        liList.forEach(el => el.classList.remove("non-match"));
        text = text?.trim() || null;
        if (text == null) {
            liList.forEach(li => {
                li.querySelectorAll<HTMLElement>(":scope > ul").forEach(ul => ul.style.display = "none");
                li.style.display = "";
                li.classList.remove("expanded");
            });
            return;
        }
        var parts = text.replace(',', ' ').split(' ');
        for (var i = 0; i < parts.length; i++) {
            parts[i] = Select2.util.stripDiacritics(parts[i]).toUpperCase()?.trim() || null;
        }
        var reportItems = liList.filter(x => x.classList.contains('report-item'));
        reportItems.forEach(e => {
            var title = Select2.util.stripDiacritics((e.textContent ?? '').toUpperCase());
            for (var $t1 = 0; $t1 < parts.length; $t1++) {
                var p = parts[$t1];
                if (p != null && !(title.indexOf(p) !== -1)) {
                    e.classList.add('non-match');
                    break;
                }
            }
        });

        function parents(el: HTMLElement, selector: string) {
            const parents = [];
            while ((el = el.parentNode as HTMLElement) && (el as any) !== document) {
                if (!selector || el.matches(selector))
                    parents.push(el);
            }
            return parents;
        }

        var matchingItems = reportItems.filter(x => !x.classList.contains('.non-match'));
        var visibles = [...matchingItems];
        matchingItems.forEach(x => visibles.push(...parents(x as HTMLElement, 'li')));
        visibles = visibles.filter((x, i) => visibles.indexOf(x) === i);

        var nonVisibles = Array.from(liList).filter(x => visibles.indexOf(x) < 0);
        nonVisibles.forEach(x => {
            x.style.display = "none";
            x.classList.add('non-match')
        });

        visibles.forEach(x => x.style.display = "");

        if (visibles.length <= 100) {
            liList.forEach(x => {
                x.querySelectorAll<HTMLElement>(":scope > ul").forEach(x => x.style.display = "");
                x.classList.add("expanded");
            });
        }
    }

    protected categoryClick(e: Event) {
        var li = (e.target as HTMLElement).closest('li');
        if (li.classList.contains('expanded')) {
            li.querySelectorAll('ul').forEach(x => x.style.display = "none");
            li.classList.remove('expanded');
            li.querySelectorAll("li").forEach(x => x.classList.remove('expanded'));
        }
        else {
            li.classList.add('expanded');
            li.querySelectorAll('ul').forEach(x => x.style.display = "");
            var childLi = li.querySelectorAll(':scope > ul > li');
            if (childLi.length === 1 &&
                !childLi[0].classList.contains('expanded')) {
                childLi[0].querySelector<HTMLElement>(".line")?.click();
            }
        }
    }

    protected reportLinkClick(e: Event) {
        e.preventDefault();
        new ReportDialog({ reportKey: (e.target as HTMLElement).dataset.key });
    }
}

export interface ReportRetrieveRequest extends ServiceRequest {
    ReportKey?: string;
}

export interface ReportRetrieveResponse extends ServiceResponse {
    ReportKey?: string;
    Properties?: PropertyItem[];
    Title?: string;
    InitialSettings?: any;
    IsDataOnlyReport?: boolean;
}
}
