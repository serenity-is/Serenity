
/** The MathML namespace URI (`http://www.w3.org/1998/Math/MathML`). */
export const MathMLNamespace = "http://www.w3.org/1998/Math/MathML"
/** Set of tag names that exist only in MathML and never in HTML. Used by the JSX factory to auto-select the MathML namespace. */
export const mathMLOnlyTags: Set<string> = /*#__PURE__*/ new Set(["math", "annotation", "annotation-xml", "maction", "merror", "mfrac", "mi", "mmultiscripts", "mn", "mo", "mover", "mpadded", "mphantom", "mprescripts", "mroot", "mrow", "ms", "mspace", "msqrt", "mstyle", "msub", "msubsup", "msup", "mtable", "mtd", "mtext", "mtr", "munder", "munderover", "semantics", "menclose", "mfenced"]);