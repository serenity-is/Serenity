import { alertDialog, confirmDialog, informationDialog, successDialog, warningDialog } from "../base";

/**
 * Legacy `Q.alert` alias.
 * @deprecated Use {@link alertDialog} from `"@serenity-is/corelib"` instead. This re-export is retained for compat with code that imports `Q.alert` / `Serenity.alert`.
 * @see {@link alertDialog}
 */
export const alert = alertDialog;

/**
 * Legacy `Q.confirm` alias.
 * @deprecated Use {@link confirmDialog} instead.
 * @see {@link confirmDialog}
 */
export const confirm = confirmDialog;

/**
 * Legacy `Q.information` alias.
 * @deprecated Use {@link informationDialog} instead.
 * @see {@link informationDialog}
 */
export const information = informationDialog;

/**
 * Legacy `Q.success` alias.
 * @deprecated Use {@link successDialog} instead.
 * @see {@link successDialog}
 */
export const success = successDialog;

/**
 * Legacy `Q.warning` alias.
 * @deprecated Use {@link warningDialog} instead.
 * @see {@link warningDialog}
 */
export const warning = warningDialog;