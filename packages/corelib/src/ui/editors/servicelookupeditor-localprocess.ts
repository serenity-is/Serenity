import { ListRequestCapabilities, type ListRequest } from "../../base";
import { type ComboboxSearchQuery } from "./combobox";
import { ComboboxEditor } from "./comboboxeditor";

export type LocalListProcessor = {
    serverRequest: ListRequest;
    process: (items: any[]) => {
        items: any[];
        more?: boolean;
    }
};

export function normalizePrimitiveItems(items: any[]): any[] {
    if (items.length === 0 || items.some(item =>
        item !== null && typeof item === "object"))
        return items;

    return items.map(Value => ({ Value }));
}

export function createLocalListProcessor(this: void, {
    request, capabilities, query, quickSearchFields, itemText
}: {
    request: ListRequest;
    capabilities: ListRequestCapabilities | null | undefined;
    query: ComboboxSearchQuery;
    quickSearchFields?: string[];
    itemText: (item: any) => string;
}): LocalListProcessor | null {
    if (capabilities == null)
        return null;

    const supports = (capability: ListRequestCapabilities) =>
        (capabilities & capability) === capability;

    const containsFieldUnsupported = request.ContainsText != null &&
        request.ContainsField != null && !supports(ListRequestCapabilities.ContainsField);
    const localContains = request.ContainsText != null &&
        (!supports(ListRequestCapabilities.ContainsText) || containsFieldUnsupported);
    const localEquality = request.EqualityFilter != null &&
        !supports(ListRequestCapabilities.EqualityFilter);
    const localCriteria = request.Criteria != null &&
        !supports(ListRequestCapabilities.Criteria);
    const localSort = request.Sort != null && !supports(ListRequestCapabilities.Sort);
    const localFilter = localContains || localEquality || localCriteria;
    const localPage = (request.Skip != null && !supports(ListRequestCapabilities.Skip)) ||
        (request.Take != null && !supports(ListRequestCapabilities.Take)) || localFilter || localSort;

    const serverRequest: ListRequest = { ...request };

    for (const key of Object.keys(serverRequest)) {
        const capability = ListRequestCapabilities[key as keyof typeof ListRequestCapabilities];
        if (typeof capability !== "number" || !supports(capability))
            delete (serverRequest as any)[key];
    }

    if (localContains)
        delete serverRequest.ContainsText;
    if (containsFieldUnsupported)
        delete serverRequest.ContainsField;
    if (localSort)
        delete serverRequest.Sort;

    if (localPage) {
        delete serverRequest.Skip;
        delete serverRequest.Take;
    }

    return {
        serverRequest,
        process: (items: any[]) => {
            if (localContains) {
                const search = request.ContainsText!;
                const field = request.ContainsField;
                const matches = new Set(ComboboxEditor.filterByText(
                    items, item => field ?
                        String(getItemField(item, field) ?? "") : itemText(item), search));

                if (!field) {
                    for (const name of quickSearchFields ?? []) {
                        for (const item of ComboboxEditor.filterByText(
                            items, item => String(getItemField(item, name) ?? ""), search))
                            matches.add(item);
                    }
                }

                items = items.filter(item => matches.has(item));
            }

            if (localEquality) {
                const equality = request.EqualityFilter!;
                items = items.filter(item => Object.keys(equality).every(name =>
                    getItemField(item, name) === equality[name]));
            }

            if (localCriteria)
                items = items.filter(item => matchesCriteria(item, request.Criteria));

            if (localSort)
                items = sortItems(items, request.Sort!);

            if (localPage) {
                const skip = request.Skip || 0;
                const take = query.take ?? request.Take ?? 0;
                const more = !!query.checkMore && !!query.take && items.length > skip + query.take;
                const end = take > 0 ? skip + take : undefined;
                items = items.slice(skip, end);
                if (query.checkMore && query.take)
                    items = items.slice(0, query.take);
                return { items, more };
            }

            return { items };
        }
    }
}

export function getItemField(item: any, field: string): any {
    if (item == null)
        return undefined;

    if (Object.prototype.hasOwnProperty.call(item, field))
        return item[field];

    const key = Object.keys(item).find(name => name.toLowerCase() === field.toLowerCase());
    return key == null ? undefined : item[key];
}

export function matchesCriteria(item: any, criteria: any): boolean {
    if (criteria == null || !Array.isArray(criteria) || criteria.length === 0)
        return true;

    if (criteria.length === 1 && typeof criteria[0] === "string")
        return !!getItemField(item, criteria[0]);

    if ((criteria[0] === "is null" || criteria[0] === "is not null") &&
        Array.isArray(criteria[1]) && typeof criteria[1][0] === "string") {
        const isNull = getItemField(item, criteria[1][0]) == null;
        return criteria[0] === "is null" ? isNull : !isNull;
    }

    if (criteria.length >= 3 && typeof criteria[1] === "string") {
        const operator = criteria[1].toLowerCase();
        if (operator === "and")
            return matchesCriteria(item, criteria[0]) && matchesCriteria(item, criteria[2]);
        if (operator === "or")
            return matchesCriteria(item, criteria[0]) || matchesCriteria(item, criteria[2]);

        const field = Array.isArray(criteria[0]) && typeof criteria[0][0] === "string" ?
            criteria[0][0] : null;
        if (!field)
            throw new Error("ServiceLookupEditor cannot evaluate this criteria expression locally.");

        const value = getItemField(item, field);
        const expected = criteria[2];
        switch (operator) {
            case "=":
            case "==":
                return value == expected;
            case "!=":
            case "<>":
                return value != expected;
            case ">":
                return value > expected;
            case ">=":
                return value >= expected;
            case "<":
                return value < expected;
            case "<=":
                return value <= expected;
            case "in":
            case "not in": {
                const values = Array.isArray(expected?.[0]) ? expected[0] : expected;
                const hasValue = Array.isArray(values) && values.some(candidate => candidate == value);
                return operator === "in" ? hasValue : !hasValue;
            }
            case "like":
            case "not like": {
                const patternValue = String(expected ?? "");
                const escape = typeof criteria[3] === "string" ? criteria[3] : null;
                let pattern = "";
                for (let index = 0; index < patternValue.length; index++) {
                    const character = patternValue[index];
                    if (escape && character === escape && index + 1 < patternValue.length) {
                        pattern += patternValue[++index].replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
                    }
                    else if (character === "%")
                        pattern += ".*";
                    else if (character === "_")
                        pattern += ".";
                    else
                        pattern += character.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
                }
                const result = new RegExp(`^${pattern}$`, "i").test(String(value ?? ""));
                return operator === "like" ? result : !result;
            }
            default:
                throw new Error(`ServiceLookupEditor cannot evaluate criteria operator '${operator}' locally.`);
        }
    }

    throw new Error("ServiceLookupEditor cannot evaluate this criteria expression locally.");
}

export function sortItems<T>(items: T[], sort: string[]): T[] {
    const descriptors = sort.map(value => {
        const match = /^\s*(.*?)\s*(?:(ASC|DESC))?\s*$/i.exec(value);
        return {
            field: match?.[1] ?? value,
            descending: match?.[2]?.toUpperCase() === "DESC"
        };
    });

    return items.slice().sort((a, b) => {
        for (const descriptor of descriptors) {
            const left = getItemField(a, descriptor.field);
            const right = getItemField(b, descriptor.field);
            let comparison: number;
            if (left == null || right == null)
                comparison = left == null ? (right == null ? 0 : -1) : 1;
            else if (typeof left === "number" && typeof right === "number")
                comparison = left - right;
            else
                comparison = String(left).localeCompare(String(right));

            if (comparison !== 0)
                return descriptor.descending ? -comparison : comparison;
        }
        return 0;
    });
}
