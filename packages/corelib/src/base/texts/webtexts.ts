import { proxyTexts } from "../localtext";

namespace webTexts {
    export declare namespace Controls {
        export function asKey(): typeof Controls;
        export function asTry(): typeof Controls;
        namespace CheckTreeEditor {
            export function asKey(): typeof CheckTreeEditor;
            export function asTry(): typeof CheckTreeEditor;
            export const SelectAll: string;
        }
        namespace ColumnPickerDialog {
            export function asKey(): typeof ColumnPickerDialog;
            export function asTry(): typeof ColumnPickerDialog;
            export const HiddenColumns: string;
            export const HideHint: string;
            export const RestoreDefaults: string;
            export const ShowHint: string;
            export const Title: string;
            export const VisibleColumns: string;
        }
        namespace DataGrid {
            export function asKey(): typeof DataGrid;
            export function asTry(): typeof DataGrid;
            export const NewButton: string;
        }
        namespace DateTimeEditor {
            export function asKey(): typeof DateTimeEditor;
            export function asTry(): typeof DateTimeEditor;
            export const SetToNow: string;
        }
        namespace EntityDialog {
            export function asKey(): typeof EntityDialog;
            export function asTry(): typeof EntityDialog;
            export const ApplyChangesButton: string;
            export const CloneButton: string;
            export const DeleteButton: string;
            export const DeleteConfirmation: string;
            export const EditButton: string;
            export const EditRecordTitle: string;
            export const LocalizationBack: string;
            export const LocalizationButton: string;
            export const LocalizationConfirmation: string;
            export const NewRecordTitle: string;
            export const SaveButton: string;
            export const SaveSuccessMessage: string;
            export const UndeleteButton: string;
            export const UndeleteConfirmation: string;
            export const UpdateButton: string;
            export const ViewRecordTitle: string;
        }
        namespace EntityGrid {
            export function asKey(): typeof EntityGrid;
            export function asTry(): typeof EntityGrid;

            export const IncludeDeletedToggle: string;
            export const NewButton: string;
            export const RefreshButton: string;
            export const SaveChangesButton: string;
            export const UndoChangesButton: string;
        }
        namespace FilterPanel {
            export function asKey(): typeof FilterPanel;
            export function asTry(): typeof FilterPanel;
            export const AddFilter: string;
            export const All: string;
            export const And: string;
            export const AndInParens: string;
            export const ApplyGroups: string;
            export const ChangeAndOr: string;
            export const ClearGroups: string;
            export const CurrentFilter: string;
            export const DialogTitle: string;
            export const EditFilter: string;
            export const EffectiveEmpty: string;
            export const EffectiveFilter: string;
            export const FixErrorsMessage: string;
            export const GroupBy: string;
            export const InvalidDate: string;
            export const InvalidNumber: string;
            export const InvalidOperator: string;
            namespace OperatorFormats {
                export function asKey(): typeof OperatorFormats;
                export function asTry(): typeof OperatorFormats;
                export const bw: string;
                export const contains: string;
                export const eq: string;
                export const ge: string;
                export const gt: string;
                export const isnotnull: string;
                export const isnull: string;
                export const le: string;
                export const lt: string;
                export const ne: string;
                export const startswith: string;
            }

            namespace OperatorNames {
                export function asKey(): typeof OperatorNames;
                export function asTry(): typeof OperatorNames;

                export const bw: string;
                export const contains: string;
                export const eq: string;
                export const ge: string;
                export const gt: string;
                export const isnotnull: string;
                export const isnull: string;
                export const le: string;
                export const lt: string;
                export const ne: string;
                export const startswith: string;
            }
            export const Or: string;
            export const OrInParens: string;
            export const RemoveField: string;
            export const ResetButton: string;
            export const ResetFilterHint: string;
            export const SearchButton: string;
            export const SelectField: string;
            export const ThenBy: string;
            export const ValueRequired: string;
        }
        namespace Pager {
            export function asKey(): typeof Pager;
            export function asTry(): typeof Pager;
            export const DefaultLoadError: string;
            export const LoadingStatus: string;
            export const NoRowStatus: string;
            export const Page: string;
            export const PageStatus: string;
        }
        namespace PropertyGrid {
            export function asKey(): typeof PropertyGrid;
            export function asTry(): typeof PropertyGrid;
            export const RequiredHint: string;
        }
        namespace QuickSearch {
            export function asKey(): typeof QuickSearch;
            export function asTry(): typeof QuickSearch;
            export const FieldSelection: string;
            export const Hint: string;
            export const Placeholder: string;
        }
        namespace SelectEditor {
            export function asKey(): typeof SelectEditor;
            export function asTry(): typeof SelectEditor;
            export const AjaxError: string;
            export const ClickToDefine: string;
            export const EmptyItemText: string;
            export const InplaceAdd: string;
            export const InplaceEdit: string;
            export const InputTooLong: string;
            export const InputTooShort: string;
            export const LoadMore: string;
            export const MultipleMatches: string;
            export const NoMatches: string;
            export const NoResultsClickToDefine: string;
            export const Searching: string;
            export const SelectionTooBig: string;
            export const SingleMatch: string;
        }
    }
    export declare namespace Dialogs {
        export function asKey(): typeof Dialogs;
        export function asTry(): typeof Dialogs;
        export const AlertTitle: string;
        export const CancelButton: string;
        export const ConfirmationTitle: string;
        export const InformationTitle: string;
        export const MaximizeHint: string;
        export const NoButton: string;
        export const OkButton: string;
        export const RestoreHint: string;
        export const SuccessTitle: string;
        export const WarningTitle: string;
        export const YesButton: string;
    }
    export declare namespace Validation {
        export function asKey(): typeof Validation;
        export function asTry(): typeof Validation;
        export const CaptchaMismatch: string;
        export const DateInvalid: string;
        export const DayHourAndMin: string;
        export const Decimal: string;
        export const Digits: string;
        export const Email: string;
        export const EmailExists: string;
        export const EmailMultiple: string;
        export const HourAndMin: string;
        export const IncorrectPassword: string;
        export const Integer: string;
        export const InvalidFormMessage: string;
        export const MaxDate: string;
        export const MaxLength: string;
        export const MinDate: string;
        export const MinLength: string;
        export const PasswordConfirm: string;
        export const Range: string;
        export const Required: string;
        export const UniqueConstraint: string;
        export const Url: string;
        export const Username: string;
        export const UsernameExists: string;
        export const Xss: string;
    }
}

const textsProxy: typeof webTexts = proxyTexts({}, '', {
    Controls: {
        CheckTreeEditor: {},
        ColumnPickerDialog: {},
        DataGrid: {},
        DateTimeEditor: {},
        EntityDialog: {},
        EntityGrid: {},
        FilterPanel: {
            OperatorFormats: {},
            OperatorNames: {}
        },
        Pager: {},
        PropertyGrid: {},
        QuickSearch: {},
        SelectEditor: {}
    },
    Dialogs: {},
    Validation: {}
}) as any;

export const CheckTreeEditorTexts = textsProxy.Controls.CheckTreeEditor;

export const ColumnPickerDialogTexts = textsProxy.Controls.ColumnPickerDialog;

export const DataGridTexts = textsProxy.Controls.DataGrid;

export const DateTimeEditorTexts = textsProxy.Controls.DateTimeEditor;

export const EntityDialogTexts = textsProxy.Controls.EntityDialog;

export const EntityGridTexts = textsProxy.Controls.EntityGrid;

export const FilterPanelTexts = textsProxy.Controls.FilterPanel;

export const FormValidationTexts = textsProxy.Validation;

export const PagerTexts = textsProxy.Controls.Pager;

export const PropertyGridTexts = textsProxy.Controls.PropertyGrid;

export const QuickSearchTexts = textsProxy.Controls.QuickSearch;

export const SelectEditorTexts = textsProxy.Controls.SelectEditor;