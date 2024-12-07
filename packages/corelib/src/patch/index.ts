import { Fluent } from "../base";
import { jQueryPatch } from "./jquerypatch";

!jQueryPatch() && Fluent.ready(jQueryPatch);
export { jQueryPatch } from "./jquerypatch";