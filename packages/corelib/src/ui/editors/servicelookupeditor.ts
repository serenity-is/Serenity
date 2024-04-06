import { ColumnSelection, Criteria, ListRequest, ListResponse, ServiceOptions, resolveServiceUrl, serviceCall } from "../../base";
import { Decorators } from "../../types/decorators";
import { EditorProps } from "../widgets/widget";
import { ComboboxSearchQuery, ComboboxSearchResult } from "./combobox";
import { ComboboxEditor, ComboboxEditorOptions } from "./comboboxeditor";

export interface ServiceLookupEditorOptions extends ComboboxEditorOptions {
    service?: string;
    idField?: string;
    textField?: string;
    pageSize?: number;
    minimumResultsForSearch?: any;
    sort?: string[];
    columnSelection?: ColumnSelection;
    includeColumns?: string[];
    excludeColumns?: string[];
    includeDeleted?: boolean;
    containsField?: string;
    equalityFilter?: any;
    criteria?: any[];
}

@Decorators.registerEditor("Serenity.ServiceLookupEditorBase")
export abstract class ServiceLookupEditorBase<P extends ServiceLookupEditorOptions, TItem> extends ComboboxEditor<P, TItem> {

    protected getDialogTypeKey() {
        var dialogTypeKey = super.getDialogTypeKey();
        if (dialogTypeKey)
            return dialogTypeKey;

        var service = this.getService();
        if (service.startsWith("~/Services/"))
            service = service.substring("~/Services/".length);

        if (service.split('/').length == 3)
            service = service.substring(0, service.lastIndexOf('/'));

        return service.replace("/", ".");
    }

    protected getService(): string {
        return this.options.service;
    }

    protected getServiceUrl() {
        var url = this.getService();
        if (url == null)
            throw new Error("ServiceLookupEditor requires 'service' option to be configured!");

        return resolveServiceUrl(url);
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

            if (this.get_cascadeField()) {
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

    protected getCriteria(query: ComboboxSearchQuery): any[] {
        return Criteria.and(
            Criteria.and(this.getIdListCriteria(query.idList), this.options.criteria),
            Criteria.and(this.getCascadeCriteria(), this.getFilterCriteria()));
    }

    protected getListRequest(query: ComboboxSearchQuery): ListRequest {

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

    protected getServiceCallOptions(query: ComboboxSearchQuery): ServiceOptions<ListResponse<TItem>> {
        return {
            blockUI: false,
            service: this.getServiceUrl(),
            request: this.getListRequest(query),
            signal: query.signal
        }
    }

    protected hasAsyncSource() {
        return true;
    }

    protected canSearch(byId: boolean) {
        if (!byId && this.get_cascadeField()) {
            var val = this.get_cascadeValue();
            if (val == null || val === '')
                return false;
        }

        return true;
    }

    protected override async asyncSearch(query: ComboboxSearchQuery): Promise<ComboboxSearchResult<TItem>> {
        if (!this.canSearch(query.idList != null)) {
            return Promise.resolve({
                items: [],
                more: false
            });
        }

        var opt = this.getServiceCallOptions(query);
        var response = await serviceCall(opt);
        var itemsPlus1 = response.Entities || [];
        var items = itemsPlus1;

        if (query.take && query.checkMore)
            items = items.slice(0, query.take);

        return {
            items: items,
            more: query.checkMore && query.take && itemsPlus1.length > query.take
        };
    }
}

@Decorators.registerEditor('Serenity.ServiceLookupEditor')
export class ServiceLookupEditor<P extends ServiceLookupEditorOptions = ServiceLookupEditorOptions, TItem = any> extends ServiceLookupEditorBase<ServiceLookupEditorOptions, TItem> {
    constructor(props: EditorProps<P>) {
        super(props);
    }
}