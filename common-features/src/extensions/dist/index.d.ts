import { BaseDialog, DataGrid, DeleteResponse, DialogType, Dictionary, EditorProps, EmailAddressEditor, EntityDialog, EntityGrid, Formatter, IGetEditValue, ISetEditValue, IconClassName, ListRequest, ListResponse, PasswordEditor, PrefixedContext, PropertyDialog, PropertyItem, SaveInitiator, SaveResponse, ServiceError, ServiceOptions, ServiceRequest, ServiceResponse, SettingStorage, ToolButton, Widget, WidgetProps } from '@serenity-is/corelib';
import { FormatterContext, Grid, GridOptions } from '@serenity-is/sleekgrid';

export interface ChangePasswordForm {
	OldPassword: PasswordEditor;
	NewPassword: PasswordEditor;
	ConfirmPassword: PasswordEditor;
}
export declare class ChangePasswordForm extends PrefixedContext {
	static readonly formKey = "Serenity.Extensions.ChangePasswordRequest";
	private static init;
	constructor(prefix: string);
}
export interface ChangePasswordRequest extends ServiceRequest {
	OldPassword?: string;
	NewPassword?: string;
	ConfirmPassword?: string;
}
export interface ExcelImportRequest extends ServiceRequest {
	FileName?: string;
}
export interface ExcelImportResponse extends ServiceResponse {
	Inserted?: number;
	Updated?: number;
	ErrorList?: string[];
}
export interface ForgotPasswordForm {
	Email: EmailAddressEditor;
}
export declare class ForgotPasswordForm extends PrefixedContext {
	static readonly formKey = "Serenity.Extensions.ForgotPasswordRequest";
	private static init;
	constructor(prefix: string);
}
export interface ForgotPasswordRequest extends ServiceRequest {
	Email?: string;
}
export interface GetNextNumberRequest extends ServiceRequest {
	Prefix?: string;
	Length?: number;
}
export interface GetNextNumberResponse extends ServiceResponse {
	Number?: number;
	Serial?: string;
}
export interface PasswordStrengthRules {
	MinPasswordLength?: number;
	RequireDigit?: boolean;
	RequireLowercase?: boolean;
	RequireNonAlphanumeric?: boolean;
	RequireUppercase?: boolean;
}
export interface ResetPasswordForm {
	NewPassword: PasswordEditor;
	ConfirmPassword: PasswordEditor;
}
export declare class ResetPasswordForm extends PrefixedContext {
	static readonly formKey = "Serenity.Extensions.ResetPasswordRequest";
	private static init;
	constructor(prefix: string);
}
export interface ResetPasswordRequest extends ServiceRequest {
	Token?: string;
	NewPassword?: string;
	ConfirmPassword?: string;
}
export interface ResetPasswordResponse extends ServiceResponse {
	RedirectHome?: boolean;
}
export interface SendResetPasswordResponse extends ServiceResponse {
	DemoLink?: string;
}
export interface TranslateTextInput {
	TextKey?: string;
	TargetLanguageID?: string;
	SourceText?: string;
}
export interface TranslateTextOutput {
	TextKey?: string;
	TargetLanguageID?: string;
	TranslatedText?: string;
}
export interface TranslateTextRequest extends ServiceRequest {
	SourceLanguageID?: string;
	Inputs?: TranslateTextInput[];
}
export interface TranslateTextResponse extends ServiceResponse {
	Translations?: TranslateTextOutput[];
}
export interface TranslationItem {
	Key?: string;
	SourceText?: string;
	TargetText?: string;
	CustomText?: string;
	HasTranslation?: boolean;
	UserTranslated?: boolean;
}
export interface TranslationListRequest extends ListRequest {
	SourceLanguageID?: string;
	TargetLanguageID?: string;
}
export interface TranslationListResponse extends ListResponse<TranslationItem> {
	KeysByAssembly?: {
		[key: string]: string[];
	};
}
export interface TranslationUpdateRequest extends ServiceRequest {
	TargetLanguageID?: string;
	Translations?: {
		[key: string]: string;
	};
}
export interface TranslationUpdateResponse extends ServiceResponse {
	SavedPath?: string;
}
export interface UserPreferenceRetrieveRequest extends ServiceRequest {
	PreferenceType?: string;
	Name?: string;
}
export interface UserPreferenceRetrieveResponse extends ServiceResponse {
	Value?: string;
}
export interface UserPreferenceRow {
	UserPreferenceId?: number;
	UserId?: number;
	PreferenceType?: string;
	Name?: string;
	Value?: string;
}
export declare abstract class UserPreferenceRow {
	static readonly idProperty = "UserPreferenceId";
	static readonly nameProperty = "Name";
	static readonly localTextPrefix = "Common.UserPreference";
	static readonly deletePermission = "";
	static readonly insertPermission = "";
	static readonly readPermission = "";
	static readonly updatePermission = "";
	static readonly Fields: Readonly<Record<keyof UserPreferenceRow, string>>;
}
export interface UserPreferenceUpdateRequest extends ServiceRequest {
	PreferenceType?: string;
	Name?: string;
	Value?: string;
}
export declare namespace UserPreferenceService {
	const baseUrl = "Extensions/UserPreference";
	function Update(request: UserPreferenceUpdateRequest, onSuccess?: (response: ServiceResponse) => void, opt?: ServiceOptions<any>): PromiseLike<ServiceResponse>;
	function Retrieve(request: UserPreferenceRetrieveRequest, onSuccess?: (response: UserPreferenceRetrieveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<UserPreferenceRetrieveResponse>;
	const Methods: {
		readonly Update: "Extensions/UserPreference/Update";
		readonly Retrieve: "Extensions/UserPreference/Retrieve";
	};
}
export interface ReportRetrieveResult extends ServiceResponse {
	ReportKey?: string;
	Title?: string;
	Properties?: PropertyItem[];
	InitialSettings?: any;
	IsDataOnlyReport?: boolean;
	IsExternalReport?: boolean;
}
declare namespace texts {
	namespace Db {
		namespace Common {
			namespace UserPreference {
				const Name: string;
				const PreferenceType: string;
				const UserId: string;
				const UserPreferenceId: string;
				const Value: string;
			}
		}
	}
	namespace Forms {
		namespace Membership {
			namespace ChangePassword {
				const FormTitle: string;
				const SubmitButton: string;
				const Success: string;
			}
			namespace ForgotPassword {
				const FormInfo: string;
				const FormTitle: string;
				const SubmitButton: string;
				const SuccessMessage: string;
			}
			namespace ResetPassword {
				const EmailSubject: string;
				const FormTitle: string;
				const SubmitButton: string;
				const Success: string;
			}
			namespace SetPassword {
				const ElevatedActionsMessage: string;
				const EmailSentMessage: string;
				const EmailToSetPasswordMessage: string;
				const PageTitle: string;
				const SendEmailButton: string;
			}
		}
	}
	namespace Site {
		namespace BasicProgressDialog {
			const CancelTitle: string;
			const PleaseWait: string;
		}
		namespace BulkServiceAction {
			const AllHadErrorsFormat: string;
			const AllSuccessFormat: string;
			const ConfirmationFormat: string;
			const ErrorCount: string;
			const NothingToProcess: string;
			const SomeHadErrorsFormat: string;
			const SuccessCount: string;
		}
		namespace Dialogs {
			const PendingChangesConfirmation: string;
			const PendingChangesUnloadWarning: string;
		}
		namespace Translation {
			const AllTextsAlreadyTranslated: string;
			const Assembly: string;
			const CopyFailMessage: string;
			const CopySourceTranslations: string;
			const CopySuccessMessage: string;
			const CopyTargetTranslations: string;
			const CustomText: string;
			const EntityPlural: string;
			const HasTranslation: string;
			const Key: string;
			const OverrideConfirmation: string;
			const SaveChangesButton: string;
			const SaveSuccessMessage: string;
			const SourceLanguage: string;
			const SourceTargetLanguageSame: string;
			const SourceText: string;
			const TargetLanguage: string;
			const TargetLanguageRequired: string;
			const TargetText: string;
			const TranslateAllText: string;
			const TranslateText: string;
			const TranslateTextConfirmation: string;
			const TranslateTextDisabled: string;
			const UserTranslated: string;
		}
	}
	namespace Validation {
		const InvalidResetToken: string;
		const MinRequiredPasswordLength: string;
		const PasswordConfirmMismatch: string;
		const PasswordStrengthRequireDigit: string;
		const PasswordStrengthRequireLowercase: string;
		const PasswordStrengthRequireNonAlphanumeric: string;
		const PasswordStrengthRequireUppercase: string;
	}
}
export declare const BasicProgressDialogTexts: typeof texts.Site.BasicProgressDialog;
export declare const BulkServiceActionTexts: typeof texts.Site.BulkServiceAction;
export declare const ChangePasswordFormTexts: typeof texts.Forms.Membership.ChangePassword;
export declare const ChangePasswordValidationTexts: typeof texts.Validation;
export declare const DialogUtilsTexts: typeof texts.Site.Dialogs;
export declare const ExtensionsTexts: typeof texts;
export declare const ForgotPasswordFormTexts: typeof texts.Forms.Membership.ForgotPassword;
export declare const PasswordStrengthValidationTexts: typeof texts.Validation;
export declare const ResetPasswordFormTexts: typeof texts.Forms.Membership.ResetPassword;
export declare const SetPasswordFormTexts: typeof texts.Forms.Membership.SetPassword;
export declare const TranslationTexts: typeof texts.Site.Translation;
export declare class BasicProgressDialog<P = {}> extends BaseDialog<P> {
	private progressBar;
	constructor(props?: WidgetProps<P>);
	cancelled: boolean;
	get max(): number;
	set max(value: number);
	get value(): number;
	set value(value: number);
	get title(): string;
	set title(value: string);
	cancelTitle: string;
	protected getDialogButtons(): {
		text: string;
		class: string;
		click: () => void;
	}[];
	protected getDialogOptions(): import("@serenity-is/corelib").DialogOptions;
	protected initDialog(): void;
	protected renderContents(): any;
}
export declare class BulkServiceAction {
	protected keys: string[];
	protected queue: string[];
	protected queueIndex: number;
	protected progressDialog: BasicProgressDialog;
	protected pendingRequests: number;
	protected completedRequests: number;
	protected errorByKey: Dictionary<ServiceError>;
	private successCount;
	private errorCount;
	done: () => void;
	protected createProgressDialog(): void;
	protected getConfirmationFormat(): string;
	protected getConfirmationMessage(targetCount: any): string;
	protected confirm(targetCount: any, action: any): void;
	protected getNothingToProcessMessage(): string;
	protected nothingToProcess(): void;
	protected getParallelRequests(): number;
	protected getBatchSize(): number;
	protected startParallelExecution(): void;
	protected serviceCallCleanup(): void;
	protected executeForBatch(batch: string[]): void;
	protected executeNextBatch(): void;
	protected getAllHadErrorsFormat(): string;
	protected showAllHadErrors(): void;
	protected getSomeHadErrorsFormat(): string;
	protected showSomeHadErrors(): void;
	protected getAllSuccessFormat(): string;
	protected showAllSuccess(): void;
	protected showResults(): void;
	execute(keys: string[]): void;
	get_successCount(): number;
	set_successCount(value: number): void;
	get_errorCount(): number;
	set_errorCount(value: number): void;
}
export interface ExcelExportOptions {
	grid: DataGrid<any, any>;
	service: string;
	onViewSubmit: () => boolean;
	editRequest?: (request: ListRequest) => ListRequest;
	title?: string;
	hint?: string;
	separator?: boolean;
}
export declare namespace ExcelExportHelper {
	function createToolButton(options: ExcelExportOptions): ToolButton;
}
declare global {
	var jspdf: any;
	var jsPDF: any;
	interface jsPDF {
		autoTableEndPosY?: number;
		autoTableHtmlToJson(table: HTMLElement): any;
		autoTable(columns: string[] | jsPDF.AutoTableColumn[], data: any[], options: jsPDF.AutoTableOptions): any;
		autoTableText(text: string, x: number, y: number, styles: jsPDF.AutoTableStyles): any;
	}
	namespace jsPDF {
		interface AutoTableColumn {
			title?: string;
			dataKey?: string;
		}
		interface AutoTableOptions {
			tableWidth?: "wrap";
			theme?: "striped" | "grid" | "plain";
			startY?: number;
			styles?: AutoTableStyles;
			headerStyles?: AutoTableStyles;
			bodyStyles?: AutoTableStyles;
			columnStyles?: {
				[dataKey: string]: AutoTableStyles;
			};
			margin?: AutoTableMargin;
			didDrawCell?: (data: CellHookData) => void;
			didDrawPage?: (data: HookData) => void;
			head?: [
				AutoTableColumn[]
			];
			body?: {}[];
		}
		interface HookData {
			table?: any;
			pageNumber?: number;
			pageCount?: number;
			settings?: {};
			doc?: any;
			cursor?: {
				x?: number;
				y?: number;
			};
		}
		interface CellHookData extends HookData {
			cell?: {
				x?: number;
				y?: number;
			};
			row?: any;
			column?: AutoTableColumn;
			section?: "head" | "body" | "foot";
		}
		interface AutoTableMargin {
			horizontal?: number;
			top?: number;
			left?: number;
			right?: number;
			bottom?: number;
		}
		interface AutoTableStyles {
			cellPadding?: number;
			fontSize?: number;
			font?: string;
			lineColor?: number | number[];
			lineWidth?: number;
			lineHeight?: number;
			fontStyle?: string;
			fillColor?: number | number[];
			textColor?: number | number[];
			halign?: "left" | "center" | "right";
			valign?: "top" | "middle" | "bottom";
			fillStyle?: "S" | "F" | "DF";
			rowHeight?: number;
			columnWidth?: "auto" | "wrap" | number;
			cellWidth?: "auto" | "wrap" | number;
			overflow?: "linebreak";
		}
	}
}
export interface PdfExportOptions {
	grid: DataGrid<any, any>;
	onViewSubmit: () => boolean;
	title?: string;
	hint?: string;
	separator?: boolean;
	reportTitle?: string;
	titleTop?: number;
	titleFontSize?: number;
	fileName?: string;
	pageNumbers?: boolean;
	columnTitles?: {
		[key: string]: string;
	};
	tableOptions?: jsPDF.AutoTableOptions;
	output?: string;
	autoPrint?: boolean;
	printDateTimeHeader?: boolean;
}
export declare namespace PdfExportHelper {
	function exportToPdf(options: PdfExportOptions): void;
	function createToolButton(options: PdfExportOptions): ToolButton;
}
export declare class EnumSelectFormatter implements Formatter {
	readonly props: {
		enumKey?: string;
		allowClear?: boolean;
		emptyItemText?: string;
	};
	constructor(props?: {
		enumKey?: string;
		allowClear?: boolean;
		emptyItemText?: string;
	});
	format(ctx: FormatterContext): string;
	get enumKey(): string;
	set enumKey(value: string);
	get allowClear(): boolean;
	set allowClear(value: boolean);
	get emptyItemText(): string;
	set emptyItemText(value: string);
}
export declare class SingleLineTextFormatter implements Formatter {
	format(ctx: FormatterContext): string;
	static formatValue(value: string): string;
}
export declare abstract class GridEditorDialog<TEntity, P = {}> extends EntityDialog<TEntity, P> {
	protected getIdProperty(): string;
	onSave: (options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void, initiator: SaveInitiator) => void;
	onDelete: (options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void) => void;
	destroy(): void;
	protected updateInterface(): void;
	protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void, initiator: SaveInitiator): void;
	protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): void;
}
export declare abstract class GridEditorBase<TEntity, P = {}> extends EntityGrid<TEntity, P> implements IGetEditValue, ISetEditValue {
	/** Gets the id property name. Returns it from getRowDefinition() if available, or the default __id.
	 * For connected mode, this should be the actual id property name of the entity, or getRowDefinition
	 * should be implemented.
	 */
	protected getIdProperty(): string;
	/**
	 * Gets the dialog type to be used for editing entities. This method must be overridden in the grid editor class to return
	 * the dialog type that extends GridEditorDialog<TEntity> and has the same TEntity type with the grid editor.
	 */
	protected getDialogType(): DialogType | PromiseLike<DialogType>;
	/** Next ID value for in memory editing mode */
	protected nextId: number;
	constructor(props: EditorProps<P>);
	/**
	 * Gets the id value of the entity. If the entity is null, returns null.
	 * @param entity
	 * @deprecated Use itemId method instead
	 */
	protected id(entity: TEntity): any;
	/**
	 * Gets next id value for in-memory editing mode. It is a number prefixed with a backtick.
	 */
	getNextId(): string;
	/**
	 * Sets the id value of the entity for in-memory editing mode.
	 * @param entity Entity to set the id value
	 */
	setNewId(entity: TEntity): TEntity;
	/**
	 * This is called from the editor dialog's save handler to save the entity.
	 * @param opt Save options
	 * @param callback An optional callback to call after the entity is saved, usually same with the opt.onSuccess
	 */
	protected save(opt: ServiceOptions<any>, callback: (r: ServiceResponse) => void): Promise<void>;
	/**
	 * This is called from the editor dialog's delete handler to delete the entity.
	 * @param opt Delete service call options
	 * @param callback An optional callback to call after the entity is deleted, usually same with the opt.onSuccess
	 * @returns
	 */
	protected delete(opt: ServiceOptions<any>, callback: (r: ServiceResponse) => void): Promise<void>;
	protected deleteEntity(id: any): (boolean | Promise<boolean>);
	/**
	 * Called before saving an entity from the dialog. If the function returns false,
	 * the entity will not be saved. Row is the entity to be saved and id is the id of the entity.
	 * If the id is null, it is a new entity (insert mode), otherwise it is an existing entity (update mode).
	 */
	protected validateEntity(row: TEntity, id: any): (boolean | Promise<boolean>);
	/**
	 * Gets a new entity instance to be used for creating new entities.
	 */
	protected getNewEntity(): TEntity;
	protected getButtons(): ToolButton[];
	/**
	 * This method is overridden to intercept the dialog creation and pass the handlers for save/delete operations.
	 * @param entityOrId Entity or id of the entity to be edited
	 */
	protected editItem(entityOrId: any): void;
	/**
	 * Sets the editor value in target object. If connected mode is on the value is not set.
	 * @param property Property item
	 * @param target Target object
	 */
	getEditValue(property: PropertyItem, target: any): void;
	/**
	 * Gets the editor value from source object. If connected mode is on the value is not read.
	 */
	setEditValue(source: any, property: PropertyItem): void;
	/**
	 * Gets the value of the grid editor. Unlike getEditValue, this method returns the actual array of entities
	 * whether connected mode is on or off.
	 */
	get value(): TEntity[];
	/**
	 * Sets the value of the grid editor. Unlike setEditValue, this method sets entities
	 * whether connected mode is on or off.
	 */
	set value(value: TEntity[]);
	/**
	 * Returns true only in connected mode, otherwise false.
	 */
	protected getGridCanLoad(): boolean;
	/**
	 * As grid editor works in memory editing mode by default, it does not use pager.
	 */
	protected usePager(): boolean;
	/**
	 * No title by default for grid editors
	 */
	protected getInitialTitle(): any;
	private _connectedMode;
	get connectedMode(): boolean;
	/**
	 * Sets the connected mode of the grid editor. By default it is false, e.g. it is disconnected/in-memory editing mode.
	 * Connected mode should only be enabled when the dialog containing grid editor is in edit mode, e.g.
	 * a master entity ID is available. In connected mode, the grid editor will load and save data from/to
	 * services directly, instead of in-memory editing, and validateEntity, deleteEntity, save, delete methods
	 * will still be called but they should check the connectedMode property before trying to perform in-memory logic.
	 * In the form, set [MinSelectLevel(SelectLevel.Explicit)] attribute to the grid editor property so that it is not
	 * loaded by MasterDetailRelation behavior when the master dialog is in edit mode.
	 */
	set connectedMode(value: boolean);
	/**
	 * Override this method to perform additional operations when connected mode is switched on/off.
	 * By default it sets the items to empty array when connected mode is switched on/off.
	 */
	protected connectedModeChanged(): void;
	/**
	 * Update UI elements based on the connected mode. In connected mode, quick search and refresh buttons are visible,
	 * and the grid editor has "connected-mode" css class.
	 */
	updateInterface(): void;
	/**
	 * Validates the connected mode support for the grid editor. Checks that the id property is not "__id" and
	 * one of getService, getServiceMethod, getServiceUrl methods are overridden to return the service URL of the entity.
	 */
	protected checkConnectedModeSupport(): void;
	/**
	 * Validates the dialog type returned from getDialogType method of the grid editor.
	 * It must be a subclass of GridEditorDialog<TEntity>, its id property must match with the grid editor's id property, and
	 * its getService() method must return the same service URL with the grid editor's getService() method.
	 *
	 * @param dlg Dialog
	 */
	protected checkDialogType(dlg: any): GridEditorDialog<TEntity>;
}
export declare class ReportDialog<P extends ReportDialogOptions = ReportDialogOptions> extends BaseDialog<P> {
	private report;
	private propertyGrid;
	constructor(props: WidgetProps<P>);
	protected getDialogButtons(): any;
	protected createPropertyGrid(): void;
	protected loadReport(reportKey: string): void;
	protected updateInterface(): void;
	executeReport(target: string, ext: string, download: boolean): void;
	getToolbarButtons(): ({
		title: string;
		cssClass: string;
		onClick: () => void;
		icon?: undefined;
	} | {
		title: string;
		cssClass: string;
		icon: string;
		onClick: () => void;
	})[];
	renderContents(): any;
}
export interface ReportDialogOptions {
	reportKey: string;
}
export interface ReportExecuteOptions {
	reportKey: string;
	download?: boolean;
	extension?: "pdf" | "htm" | "html" | "xlsx" | "docx";
	getParams?: () => any;
	params?: {
		[key: string]: any;
	};
	target?: string;
}
export interface ReportButtonOptions extends ReportExecuteOptions {
	title?: string;
	cssClass?: string;
	icon?: IconClassName;
}
export declare namespace ReportHelper {
	function createToolButton(options: ReportButtonOptions): ToolButton;
	function execute(options: ReportExecuteOptions): void;
}
export declare class ReportPage<P = {}> extends Widget<P> {
	constructor(props: WidgetProps<P>);
	protected updateMatchFlags(text: string): void;
	protected reportLinkClick(e: Event): void;
}
export declare class UserPreferenceStorage implements SettingStorage {
	getItem(key: string): Promise<string>;
	setItem(key: string, data: string): Promise<void>;
}
export declare namespace DialogUtils {
	function pendingChangesConfirmation(element: ArrayLike<HTMLElement> | HTMLElement, hasPendingChanges: () => boolean): void;
}
export interface PromptDialogOptions {
	cssClass?: string;
	editorType?: string | {
		new (props?: any): Widget;
	};
	editorOptions?: any;
	title?: string;
	message?: string;
	isHtml?: boolean;
	value?: any;
	required?: boolean;
	validateValue: (v: any) => boolean;
}
export declare class PromptDialog<P extends PromptDialogOptions = PromptDialogOptions> extends PropertyDialog<any, P> {
	constructor(props: WidgetProps<P>);
	protected getDialogButtons(): import("@serenity-is/corelib").DialogButton[];
	protected loadInitialEntity(): void;
	protected getPropertyItems(): {
		name: string;
		editorType: string;
		required: boolean;
		editorParams: any;
	}[];
	get value(): any;
	set value(v: any);
	static prompt(title: string, message: string, value: string, validateValue: (string: any) => boolean): void;
}
export declare class SelectableEntityGrid<TItem, TOptions> extends EntityGrid<TItem, TOptions> {
	protected getSlickOptions(): GridOptions;
	protected createSlickGrid(): Grid;
}
/**
	* This is an editor widget but it only displays a text, not edits it.
	*
	*/
export declare class StaticTextBlock<P extends StaticTextBlockOptions = StaticTextBlockOptions> extends Widget<P> implements ISetEditValue {
	private value;
	constructor(props: WidgetProps<P>);
	private updateElementContent;
	/**
		* By implementing ISetEditValue interface, we allow this editor to display its field value.
		* But only do this when our text content is not explicitly set in options
		*/
	setEditValue(source: any, property: PropertyItem): void;
}
export interface StaticTextBlockOptions {
	text: string;
	isHtml: boolean;
	isLocalText: boolean;
	hideLabel: boolean;
}
export declare function getPasswordStrengthRules(): Promise<PasswordStrengthRules>;
export declare function addPasswordStrengthValidation(passwordEditor: PasswordEditor, uniqueName?: string): void;

export {};
