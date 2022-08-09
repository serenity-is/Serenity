export * from "./index"
import { promisePatch } from './Patch/PromisePatch'
import { vuePatch } from './Patch/VuePatch'

promisePatch();

// @ts-ignore
if (typeof Vue == "function")
// @ts-ignore
    vuePatch(Vue);