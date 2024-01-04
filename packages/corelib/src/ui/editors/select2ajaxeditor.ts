import { ListRequest, ListResponse, RetrieveResponse, localText } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { ServiceOptions, safeCast, serviceCall } from "../../q";
import { ValidationHelper } from "../helpers/validationhelper";
import { EditorProps, Widget } from "../widgets/widget";
import { WX } from "../widgets/wx";

@Decorators.registerEditor('Serenity.Select2AjaxEditor', [IStringValue])
@Decorators.element('<input type="hidden" />')
export class Select2AjaxEditor<P = {}, TItem = any> extends Widget<P> implements IStringValue {
    pageSize: number = 50;

    constructor(props: EditorProps<P>) {
        super(props);

        let hidden = $(this.domNode);
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
        var txt = this.domNode.getAttribute("placeholder");
        if (txt == null) {
            txt = localText('Controls.SelectEditor.EmptyItemText');
        }
        return txt;
    }

    protected getService(): string {
        throw new Error("Not implemented!");
    }

    protected query(request: ListRequest, callback: (p1: ListResponse<TItem>) => void): void {
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

    protected executeQuery(options: ServiceOptions<ListResponse<TItem>>): void {
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

    protected executeQueryByKey(options: ServiceOptions<RetrieveResponse<TItem>>): void {
        serviceCall(options);
    }

    protected getItemKey(item: TItem): string {
        return null;
    }

    protected getItemText(item: TItem): string {
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
            placeHolder: emptyItemText || null,
            allowClear: emptyItemText != null,
            query: query => {
                var request = {
                    ContainsText: query.term?.trim() || null,
                    Skip: (query.page - 1) * this.pageSize,
                    Take: this.pageSize + 1
                };

                if (queryTimeout !== 0) {
                    window.clearTimeout(queryTimeout);
                }

                var select2 = $(this.domNode).data('select2');
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
                var val = element.val() as string;
                if (!val) {
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
            .attr('title', title).insertAfter(this.domNode).click(function (e) {
                self.inplaceCreateClick(e);
            });

        this.get_select2Container().add(this.domNode)
            .addClass('has-inplace-button');
    }

    protected inplaceCreateClick(e: any): void {
    }

    protected get_select2Container(): JQuery {
        return $(this.domNode).prevAll('.select2-container');
    }

    get_value(): string {
        return safeCast($(this.domNode).select2('val'), String);
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string) {
        if (value !== this.get_value()) {
            var el = $(this.domNode);
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