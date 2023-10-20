import { Decorators } from "../../decorators";
import { postToService, PropertyItem, serviceCall, ServiceRequest, ServiceResponse, trimToNull } from "../../q";
import { QuickSearchInput } from "../datagrid/quicksearchinput";
import { TemplatedDialog } from "../dialogs/templateddialog";
import { PropertyGrid } from "./propertygrid";
import { Widget } from "./widget";

export namespace Reporting {
    export interface ReportDialogOptions {
        reportKey?: string;
    }

    @Decorators.registerClass('Serenity.Reporting.ReportDialog')
    export class ReportDialog extends TemplatedDialog<ReportDialogOptions> {
        constructor(opt: ReportDialogOptions) {
            super(opt);

            if (opt.reportKey) {
                this.loadReport(opt.reportKey);
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
            this.propertyGrid = (new PropertyGrid(this.byId('PropertyGrid'), {
                idPrefix: this.idPrefix,
                useCategories: true,
                items: this.propertyItems
            })).init(null);
        }

        loadReport(reportKey: string): void {
            serviceCall({
                service: 'Report/Retrieve', request: { ReportKey: reportKey },
                onSuccess: (response: ReportRetrieveResponse) => {
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
                }
            });
        }

        executeReport(targetFrame: string, exportType: string): void {
            if (!this.validateForm()) {
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
    export class ReportPage extends Widget<any> {
        constructor(div: JQuery) {
            super(div);

            $('.report-link').click((e) => this.reportLinkClick(e));
            $('div.line').click((e) => this.categoryClick(e));
            var self = this;
            new QuickSearchInput($('#QuickSearchInput'), {
                onSearch: function (field, text, done) {
                    self.updateMatchFlags(text);
                    done(true);
                }
            });
        }

        protected updateMatchFlags(text: string) {
            var liList = $('#ReportList').find('li').removeClass('non-match');
            text = trimToNull(text);
            if (text == null) {
                liList.children('ul').hide();
                liList.show().removeClass('expanded');
                return;
            }
            var parts = text.replace(',', ' ').split(' ');
            for (var i = 0; i < parts.length; i++) {
                parts[i] = trimToNull(Select2.util.stripDiacritics(parts[i]).toUpperCase());
            }
            var reportItems = liList.filter('.report-item');
            reportItems.each(function (i1, e) {
                var x = $(e);
                var title = Select2.util.stripDiacritics((x.text() ?? '').toUpperCase());
                for (var $t1 = 0; $t1 < parts.length; $t1++) {
                    var p = parts[$t1];
                    if (p != null && !(title.indexOf(p) !== -1)) {
                        x.addClass('non-match');
                        break;
                    }
                }
            });
            var matchingItems = reportItems.not('.non-match');
            var visibles = matchingItems.parents('li').add(matchingItems);
            var nonVisibles = liList.not(visibles);
            nonVisibles.hide().addClass('non-match');
            visibles.show();
            if (visibles.length <= 100) {
                liList.children('ul').show();
                liList.addClass('expanded');
            }
        }

        protected categoryClick(e: JQueryEventObject) {
            var li = $(e.target).closest('li');
            if (li.hasClass('expanded')) {
                li.find('ul').hide('fast');
                li.removeClass('expanded');
                li.find('li').removeClass('expanded');
            }
            else {
                li.addClass('expanded');
                li.children('ul').show('fast');
                if (li.children('ul').children('li').length === 1 && !li.children('ul').children('li').hasClass('expanded')) {
                    li.children('ul').children('li').children('.line').click();
                }
            }
        }

        protected reportLinkClick(e: JQueryEventObject) {
            e.preventDefault();
            var dialog = new ReportDialog({ reportKey: $(e.target).data('key') });
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
