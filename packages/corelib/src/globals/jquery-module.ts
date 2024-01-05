/// <reference types="jquery" />
export default typeof jQuery === "function" ? jQuery : typeof $ === "function" && $.fn ? $ : undefined;