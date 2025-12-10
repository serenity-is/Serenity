import { jsPDF } from "jspdf";
import { applyPlugin } from "jspdf-autotable";

applyPlugin(jsPDF);

/** @internal */
export * from "jspdf";
/** @internal */
export * from "jspdf-autotable";