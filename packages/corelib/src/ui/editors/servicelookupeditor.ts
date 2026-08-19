import { ColumnSelection, Criteria, ListRequest, ListResponse, ServiceOptions, nsSerenity, resolveServiceUrl, serviceCall } from "../../base";
import { ComboboxSearchQuery, ComboboxSearchResult } from "./combobox";
import { ComboboxEditor, ComboboxEditorOptions } from "./comboboxeditor";
import { EditorProps } from "./editorwidget";

/**
 * Options for the {@link ServiceLookupEditor}.
 */
export interface ServiceLookupEditorOptions extends ComboboxEditorOptions {
    /** Service endpoint to load items from. */
    service?: string;
    /** Id field name. */
    idField?: string;
    /** Text field name. */
    textField?: string;
    /** Page size for paged searches. */
    pageSize?: number;
    /** Minimum results required to show the search box. */
    minimumResultsForSearch?: any;
    /** Sort order for results. */
    sort?: string[];
    /** Column selection mode. */
    columnSelection?: ColumnSelection;
    /** Columns to include. */
    includeColumns?: string[];
    /** Columns to exclude. */
    excludeColumns?: string[];
    /** Whether to include deleted rows. */
    includeDeleted?: boolean;
    /** Field used for contains-text search. */
    containsField?: string;
    /** Equality filter applied to the request. */
    equalityFilter?: any;
    /** Criteria applied to the request. */
    criteria?: any[];
}

/**
 * Base editor that renders a combobox over service list results.
 * @typeParam P - Widget props type.
 * @typeParam TItem - The item type.
 */
export abstract class ServiceLookupEditorBase<P extends ServiceLookupEditorOptions, TItem> extends ComboboxEditor<P, TItem> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    /**
     * Returns the dialog type key for in-place add.
     * @returns The dialog type key.
     */
    protected override getDialogTypeKey() {
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

    /**
     * Returns the service endpoint path.
     * @returns The service path.
     */
    protected getService(): string {
        return this.options.service;
    }

    /**
     * Returns the resolved service URL.
     * @returns The service URL.
     */
    protected getServiceUrl() {
        var url = this.getService();
        if (url == null)
            throw new Error("ServiceLookupEditor requires 'service' option to be configured!");

        return resolveServiceUrl(url);
    }

    /**
     * Returns the columns to include in the request.
     * @returns The include columns.
     */
    protected getIncludeColumns() {
        var include = this.options.includeColumns?.slice() || [];
        var idField = this.getIdField();

        if (idField && include.indexOf(idField) < 0)
            include.push(idField);

        var textField = this.getTextField();
        if (textField && include.indexOf(textField) < 0)
            include.push(textField);

        return include;
    }

    /**
     * Returns the sort order for results.
     * @returns The sort descriptors.
     */
    protected getSort() {
        return this.options.sort || (this.getTextField() ? [this.getTextField()] : null);
    }

    /**
     * Returns the cascade criteria for the request.
     * @returns The cascade criteria.
     */
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

    /**
     * Returns the filter criteria for the request.
     * @returns The filter criteria.
     */
    protected getFilterCriteria(): any[] {
        var val = this.get_filterValue();

        if (val == null || val === '') {
            return null;
        }

        var fld = this.get_filterField();
        return [[fld], '=', val];
    }

    /**
     * Returns the criteria for the given id list.
     * @param idList - The id list.
     * @returns The criteria.
     */
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

    /**
     * Returns the combined criteria for the request.
     * @param query - The search query.
     * @returns The criteria.
     */
    protected getCriteria(query: ComboboxSearchQuery): any[] {
        return Criteria.and(
            Criteria.and(this.getIdListCriteria(query.idList), this.options.criteria),
            Criteria.and(this.getCascadeCriteria(), this.getFilterCriteria()));
    }

    /**
     * Returns the list request for the given query.
     * @param query - The search query.
     * @returns The list request.
     */
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

    /**
     * Returns the service call options for the given query.
     * @param query - The search query.
     * @returns Service options.
     */
    protected getServiceCallOptions(query: ComboboxSearchQuery): ServiceOptions<ListResponse<TItem>> {
        return {
            blockUI: false,
            service: this.getServiceUrl(),
            request: this.getListRequest(query),
            signal: query.signal
        }
    }

    /**
     * Whether the editor has an asynchronous item source.
     * @returns True.
     */
    protected override hasAsyncSource() {
        return true;
    }

    /**
     * Whether a search can be performed.
     * @param byId - Whether the search is by id.
     * @returns True when searchable.
     */
    protected canSearch(byId: boolean) {
        if (!byId && this.get_cascadeField()) {
            var val = this.get_cascadeValue();
            if (val == null || val === '')
                return false;
        }

        return true;
    }

    /**
     * Performs an asynchronous search over the service results.
     * @param query - The search query.
     * @returns A promise resolving to the search result.
     */
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

/**
 * An editor that renders a combobox over service list results.
 * @typeParam P - Widget props type.
 * @typeParam TItem - The item type.
 */
export class ServiceLookupEditor<P extends ServiceLookupEditorOptions = ServiceLookupEditorOptions, TItem = any> extends ServiceLookupEditorBase<ServiceLookupEditorOptions, TItem> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    /**
     * Creates a service lookup editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);
    }
}