
import { ArgsSort, ISleekGrid } from "@serenity-is/sleekgrid";
import { isInstanceOfType, localText, PropertyItem, tryGetText } from "../../base";
import { deepClone } from "../../compat";
import { IRemoteView } from "../../slick";
import { BooleanFiltering } from "../filtering/booleanfiltering";
import { DateFiltering } from "../filtering/datefiltering";
import { DateTimeFiltering } from "../filtering/datetimefiltering";
import { FilteringTypeRegistry } from "../filtering/filteringtyperegistry";
import { FilterOperators } from "../filtering/filteroperator";
import { IFiltering } from "../filtering/ifiltering";
import { IQuickFiltering } from "../filtering/iquickfiltering";
import { ReflectionOptionsSetter } from "../widgets/reflectionoptionssetter";
import { QuickFilter } from "./quickfilter";
import { QuickFilterBar } from "./quickfilterbar";

export function getDefaultSortBy(this: void, sleekGrid: ISleekGrid): string[] {
    if (!sleekGrid)
        return [];

    const columns = sleekGrid.getColumns().filter(function (x) {
        return x.sortOrder && x.sortOrder !== 0 && x.field != null;
    });

    if (columns.length > 0) {
        columns.sort(function (x1, y) {
            return Math.abs(x1.sortOrder) < Math.abs(y.sortOrder) ? -1 : (Math.abs(x1.sortOrder) > Math.abs(y.sortOrder) ? 1 : 0);
        });

        const list = [];
        for (let i = 0; i < columns.length; i++) {
            const col = columns[i];
            list.push(col.field + ((col.sortOrder < 0) ? ' DESC' : ''));
        }

        return list;
    }

    return [];
}

export function getItemCssClass(this: void, item: any, activeFieldName: string, deletedFieldName: string): string {
    if (activeFieldName && deletedFieldName)
        return null;

    if (activeFieldName) {
        const value = (item as any)[activeFieldName];
        if (value == null) {
            return null;
        }

        if (typeof (value) === 'number') {
            if (value < 0) {
                return 'deleted';
            }
            else if (value === 0) {
                return 'inactive';
            }
        }
        else if (typeof (value) === 'boolean') {
            if (value === false) {
                return 'deleted';
            }
        }
    }
    else {
        return (item as any)[deletedFieldName] ? 'deleted' : null;
    }

    return null;
}

export function propertyItemToQuickFilter(item: PropertyItem): QuickFilter<any, any> | null {
    let result: QuickFilter<any, any> = {};
    const name = item.name;
    const title = localText(item.title, item.title ?? name);

    const filteringType = FilteringTypeRegistry.get((item.filteringType ?? 'String'));
    if (filteringType === DateFiltering) {
        // an application can give the date fields a filter of its own, e.g. one that edits the range in one input
        result = (QuickFilterBar.defaults.dateRangeFilter ?? QuickFilterBar.dateRange)(name, title);
    }
    else if (filteringType === DateTimeFiltering) {
        result = QuickFilterBar.dateTimeRange(name, title, item.editorParams?.useUtc);
    }
    else if (filteringType === BooleanFiltering) {
        const qfp = item.quickFilterParams || {};
        const fp = item.filteringParams || {};
        result = QuickFilterBar.boolean(name, title, qfp['trueText'] ?? fp['trueText'], qfp['falseText'] ?? fp['falseText']);
    }
    else {
        const filtering = new (filteringType as any)(item.filteringParams ?? {}) as IFiltering;
        if (isInstanceOfType(filtering, IQuickFiltering)) {
            ReflectionOptionsSetter.set(filtering, item.filteringParams);
            filtering.set_field(item);
            filtering.set_operator({ key: FilterOperators.EQ });
            (filtering as any).initQuickFilter(result);
            result.options = Object.assign(deepClone(result.options ?? {}), item.quickFilterParams ?? {});
            // the placeholder of the quick filter is a local text key, or the text itself, see QuickFilterAttribute
            if (typeof result.options.placeholder === "string")
                result.options.placeholder = tryGetText(result.options.placeholder) ?? result.options.placeholder;
        }
        else {
            return null;
        }
    }

    if (item.quickFilterSeparator) {
        result.separator = true;
    }

    result.cssClass = item.quickFilterCssClass;
    return result;
}

export function sleekGridOnSort(this: void, view: IRemoteView<any>, p: ArgsSort) {
    view.populateLock();
    try {
        const sortBy = [];
        let col: any;
        if (!!p.multiColumnSort) {
            for (let i = 0; i < p.sortCols.length; i++) {
                const x = p.sortCols[i];
                col = x.sortCol;
                if (col == null) {
                    col = {};
                }
                sortBy.push(col.field + (!!x.sortAsc ? '' : ' DESC'));
            }
        }
        else {
            col = p.sortCol;
            if (col == null) {
                col = {};
            }
            sortBy.push(col.field + (!!p.sortAsc ? '' : ' DESC'));
        }

        view.seekToPage = 1;
        view.sortBy = sortBy;
    }
    finally {
        view.populateUnlock();
    }

    if (view.getLocalSort && view.getLocalSort()) {
        view.sort();
    }
    else {
        view.populate();
    }
}