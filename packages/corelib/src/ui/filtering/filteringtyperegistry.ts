import { htmlEncode, isAssignableFrom, notifyError } from "../../base";
import { commonTypeRegistry } from "../../types/commontyperegistry";
import { IFiltering } from "./ifiltering";

export namespace FilteringTypeRegistry {

    const registry = commonTypeRegistry<Function>({
        attrKey: null,
        isMatch: type => isAssignableFrom(IFiltering, type),
        kind: "filtering",
        suffix: "Filtering",
        loadError: function (key: string) {
            const message = `"${htmlEncode(key)}" filtering handler class not found!`;
            notifyError(message);
        }
    });

    export let get = registry.get;
    export let getOrLoad = registry.getOrLoad;
    export let reset = registry.reset;
    export let tryGet = registry.tryGet;
    export let tryGetOrLoad = registry.tryGetOrLoad;
}