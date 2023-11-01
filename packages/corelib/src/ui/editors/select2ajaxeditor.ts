import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { isEmptyOrNull, isValue, ListRequest, ListResponse, RetrieveResponse, safeCast, serviceCall, ServiceOptions, localText, trimToNull } from "../../q";
import { ValidationHelper } from "../helpers/validationhelper";
import { Widget } from "../widgets/widget";
import { WX } from "../widgets/wx";

@Decorators.registerEditor('Serenity.Select2AjaxEditor', [IStringValue])
@Decorators.element('<input type="hidden" />')
export class Select2AjaxEditor<TOptions, TItem> extends Widget<TOptions> implements IStringValue {
    pageSize: number = 50;

    constructor(hidden: JQuery, opt: TOptions) {
        super(hidden, opt);

        var emptyItemText = this.emptyItemText();
        if (emptyItemText != null)
            hidden.attr("placeholder", emptyItemText);

        hidden.select2(this.getSelect2Options());

        hidden.attr("type", "text"); // jquery validate to work

        hidden.on("change." + this.uniqueName, (e, x) => {
            if (WX.hasOriginalEvent(e) || !x) {
                if (ValidationHelper.getValidator(hidden) != null)
                    hidden.valid();
            }
        });
    }

    protected emptyItemText(): string {
        var txt = this.element.attr('placeholder');
        if (txt == null) {
            txt = localText('Controls.SelectEditor.EmptyItemText');
        }
        return txt;
    }

    protected getService(): string {
        throw new Error("Not implemented!");
    }

    protected query(request: ListRequest, callback: (p1: ListResponse<any>) => void): void {
        var options: ServiceOptions<any> = {
            blockUI: false,
            service: this.getService() + '/List',
            request: request,
            onSuccess: function (response) {
                callback(response);
            }
        };
        this.executeQuery(options);
    }

    protected executeQuery(options: ServiceOptions<ListResponse<any>>): void {
        serviceCall(options);
    }

    protected queryByKey(key: string, callback: (p1: any) => void): void {
        var options: ServiceOptions<any> = {
            blockUI: false,
            service: this.getService() + '/Retrieve',
            request: { EntityId: key },
            onSuccess: function (response) {
                callback(response.Entity);
            }
        };
        this.executeQueryByKey(options);
    }

    protected executeQueryByKey(options: ServiceOptions<RetrieveResponse<any>>): void {
        serviceCall(options);
    }

    protected getItemKey(item: any): string {
        return null;
    }

    protected getItemText(item: any): string {
        return null;
    }

    protected getTypeDelay(): number {
        return 200;
    }

    protected getSelect2Options(): Select2Options {
        var emptyItemText = this.emptyItemText();
        var queryTimeout = 0;
        return {
            minimumResultsForSearch: 10,
            placeHolder: (!isEmptyOrNull(emptyItemText) ? emptyItemText : null),
            allowClear: isValue(emptyItemText),
            query: query => {
                var request = {
                    ContainsText: trimToNull(query.term), Skip: (query.page - 1) * this.pageSize, Take: this.pageSize + 1
                };

                if (queryTimeout !== 0) {
                    window.clearTimeout(queryTimeout);
                }

                var select2 = $(this.element).data('select2');
                select2 && select2.search && select2.search.removeClass('select2-active').parent().removeClass('select2-active');

                queryTimeout = window.setTimeout(() => {
                    select2 && select2.search.addClass('select2-active').parent().addClass('select2-active');

                    this.query(request, response => {
                        query.callback({
                            results: response.Entities.slice(0, this.pageSize).map(x => {
                                return { id: this.getItemKey(x), text: this.getItemText(x), source: x };
                            }), more: response.Entities.length >= this.pageSize
                        });
                    });
                }, this.getTypeDelay());

            },
            initSelection: (element, callback) => {
                var val = element.val();
                if (isEmptyOrNull(val)) {
                    callback(null);
                    return;
                }
                this.queryByKey(val, result => {
                    callback((result == null ? null : {
                        id: this.getItemKey(result),
                        text: this.getItemText(result),
                        source: result
                    }));
                });
            }
        };
    }

    protected addInplaceCreate(title: string): void {
        var self = this;
        $('<a><b/></a>').addClass('inplace-button inplace-create')
            .attr('title', title).insertAfter(this.element).click(function (e) {
                self.inplaceCreateClick(e);
            });

        this.get_select2Container().add(this.element)
            .addClass('has-inplace-button');
    }

    protected inplaceCreateClick(e: any): void {
    }

    protected get_select2Container(): JQuery {
        return this.element.prevAll('.select2-container');
    }

    get_value(): string {
        return safeCast(this.element.select2('val'), String);
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string) {
        if (value !== this.get_value()) {
            var el = this.element;
            el.select2('val', value);
            el.data('select2-change-triggered', true);
            try {
                el.triggerHandler('change', [true]); // valueSet: true
            }
            finally {
                el.data('select2-change-triggered', false);
            }
        }
    }

    set value(v: string) {
        this.set_value(v);
    }
}