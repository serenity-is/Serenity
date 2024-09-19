import { Fluent } from "../base";
import { jQueryPatch } from "./jquerypatch";
import { reactPatch } from "./reactpatch";

!jQueryPatch() && Fluent.ready(jQueryPatch);
reactPatch();

export { jQueryPatch } from "./jquerypatch";
export { reactPatch } from "./reactpatch";