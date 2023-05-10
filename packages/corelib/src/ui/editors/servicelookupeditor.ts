import { Decorators } from "../../decorators";
import { ColumnSelection, Criteria, isEmptyOrNull, ListRequest, ListResponse, resolveUrl, serviceCall, ServiceOptions, startsWith } from "@serenity-is/corelib/q";
import { Select2Editor, Select2EditorOptions, Select2SearchPromise, Select2SearchQuery, Select2SearchResult } from "./select2editor";

export interface ServiceLookupEditorOptions extends Select2EditorOptions {
    service?: string;
    idField: string;
    textField: string;
    pageSize?: number;
    minimumResultsForSearch?: any;
    sort: string[];
    columnSelection?: ColumnSelection;
    includeColumns?: string[];
    excludeColumns?: string[];
    includeDeleted?: boolean;
    containsField?: string;
    equalityFilter?: any;
    criteria?: any[];
}

@Decorators.registerEditor("Serenity.ServiceLookupEditorBase")
export abstract class ServiceLookupEditorBase<TOptions extends ServiceLookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {

    constructor(input: JQuery, opt?: TOptions) {
        super(input, opt);
    }

    protected getDialogTypeKey() {
        var dialogTypeKey = super.getDialogTypeKey();
        if (dialogTypeKey)
            return dialogTypeKey;

        var service = this.getService();
        if (startsWith(service, "~/Services/"))
            service = service.substr("~/Services/".length);

        if (service.split('/').length == 3)
            service = service.substr(0, service.lastIndexOf('/'));

        return service.replace("/", ".");
    }

    protected getService(): string {
        return this.options.service;
    }

    protected getServiceUrl() {
        var url = this.getService();
        if (url == null)
            throw new Error("ServiceLookupEditor requires 'service' option to be configured!");

        if (!startsWith(url, "~") && !startsWith(url, "/") && url.indexOf('://') < 0)
            url = "~/Services/" + url;

        if (startsWith(url, "~"))
            url = resolveUrl(url);

        return url;
    }

    protected getIncludeColumns() {
        var include = this.options.includeColumns || [];
        var idField = this.getIdField();

        if (idField && include.indexOf(idField) < 0)
            include.push(idField);

        var textField = this.getTextField();
        if (textField && include.indexOf(textField) < 0)
            include.push(textField);

        return include;
    }

    protected getSort() {
        return this.options.sort || (this.getTextField() ? [this.getTextField()] : null);
    }

    protected getCascadeCriteria(): any[] {

        var val = this.get_cascadeValue();

        if (val == null || val === '') {

            if (!isEmptyOrNull(this.get_cascadeField())) {
                return ['0', '=', '1'];
            }

            return null;
        }

        var fld = this.get_cascadeField();

        return Criteria(fld).eq(val);
    }

    protected getFilterCriteria(): any[] {
        var val = this.get_filterValue();

        if (val == null || val === '') {
            return null;
        }

        var fld = this.get_filterField();
        return [[fld], '=', val];
    }

    protected getIdListCriteria(idList: any[]): any[] {
        if (idList == null)
            return null;

        if (idList.length == 0)
            return ['0', '=', '1'];

        var idField = this.getIdField();
        if (idField == null)
            throw new Error("ServiceLookupEditor requires 'idField' option to be configured!");

        return Criteria(idField).in(idList);
    }

    protected getCriteria(query: Select2SearchQuery): any[] {
        return Criteria.and(
            Criteria.and(this.getIdListCriteria(query.idList), this.options.criteria),
            Criteria.and(this.getCascadeCriteria(), this.getFilterCriteria()));
    }

    protected getListRequest(query: Select2SearchQuery): ListRequest {

        var request: ListRequest = {};

        if (query.searchTerm)
            request.ContainsText = query.searchTerm;

        request.Sort = this.getSort();
        request.ColumnSelection = this.options.columnSelection || ColumnSelection.KeyOnly;
        request.IncludeColumns = this.getIncludeColumns();
        request.ExcludeColumns = this.options.excludeColumns;
        request.ContainsField = this.options.containsField;
        request.EqualityFilter = this.options.equalityFilter;
        request.Criteria = this.getCriteria(query);
        request.Skip = query.skip || 0;
        request.Take = query.take ? (query.checkMore ? query.take + 1 : query.take) : 0;
        request.IncludeDeleted = this.options.includeDeleted;
        request.ExcludeTotalCount = true;

        return request;
    }

    protected getServiceCallOptions(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): ServiceOptions<ListResponse<TItem>> {
        return {
            blockUI: false,
            url: this.getServiceUrl(),
            request: this.getListRequest(query),
            onSuccess: response => {
                var items = response.Entities || [];

                if (items && query.take && query.checkMore && response.Entities.length > items.length)
                    items = items.slice(0, query.take);

                results({
                    items: items.slice(0, query.take),
                    more: query.checkMore && query.take && items.length > query.take
                });
            }
        };
    }

    protected hasAsyncSource() {
        return true;
    }

    protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise {
        var opt = this.getServiceCallOptions(query, results);
        return serviceCall(opt);
    }
}

@Decorators.registerEditor('Serenity.ServiceLookupEditor')
export class ServiceLookupEditor extends ServiceLookupEditorBase<ServiceLookupEditorOptions, any> {
    constructor(hidden: JQuery, opt?: ServiceLookupEditorOptions) {
        super(hidden, opt);
    }
}