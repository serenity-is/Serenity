import { Fluent, ListRequest, ListResponse, RetrieveResponse, ServiceOptions, getjQuery, isArrayLike, localText, serviceCall } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { ValidationHelper } from "../helpers/validationhelper";
import { EditorProps, Widget } from "../widgets/widget";

@Decorators.registerEditor('Serenity.Select2AjaxEditor', [IStringValue])
@Decorators.element('<input type="hidden" />')
export class Select2AjaxEditor<P = {}, TItem = any> extends Widget<P> implements IStringValue {
    pageSize: number = 50;

    declare readonly domNode: HTMLInputElement;

    constructor(props: EditorProps<P>) {
        super(props);

        let hidden = Fluent(this.domNode);
        var emptyItemText = this.emptyItemText();
        if (emptyItemText != null)
            hidden.attr("placeholder", emptyItemText);

        let $ = getjQuery();
        if ($?.fn?.select2)
            $(hidden).select2(this.getSelect2Options());

        hidden.attr("type", "text"); // jquery validate to work

        this.changeSelect2((e) => {
            ValidationHelper.validateElement(hidden);
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

                let $ = getjQuery();
                var select2: any;
                if ($?.fn?.select2) {
                     select2 = $(this.domNode).data('select2');
                }
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
                var val = isArrayLike(element) ? (element[0] as any)?.value : (element as any)?.value;
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
        Fluent("a").append(Fluent("b")).addClass('inplace-button inplace-create')
            .attr('title', title).insertAfter(this.domNode).on("click", function (e) {
                self.inplaceCreateClick(e);
            });

        this.get_select2Container().addClass("has-inplace-button");
        this.domNode.classList.add("has-inplace-button");
    }

    protected inplaceCreateClick(e: any): void {
    }

    protected get_select2Container(): Fluent {
        let container = this.domNode.previousElementSibling;
        while (container && !container.classList.contains("select2-container"))
            container = container.previousElementSibling;
        return Fluent(container);
    }

    get_value(): string {
        var val;
        let $ = getjQuery();
        if ($ && $(this.domNode).data('select2')) {
            val = $(this.domNode).select2('val');
            if (val != null && Array.isArray(val)) {
                return val.join(',');
            }
        }
        else
            val = this.domNode.value;

        return val;        
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string) {
        if (value != this.get_value()) {
            var val: any = value;
            let $ = getjQuery();
            if ($?.fn?.select2) {
                $(this.domNode).select2('val', val);
                this.domNode.dataset.select2settingvalue = "true";
                try {
                    Fluent.trigger(this.domNode, "change");
                } finally {
                    delete this.domNode.dataset.select2settingvalue;
                }
            }
            else {
                this.domNode.value = val;
            }
        }
    }

    set value(v: string) {
        this.set_value(v);
    }
}