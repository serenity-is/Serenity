import { BaseDialog, DataGrid, DeleteResponse, Dictionary, EditorProps, EmailAddressEditor, EntityDialog, EntityGrid, Formatter, IGetEditValue, ISetEditValue, IconClassName, ListRequest, ListResponse, PasswordEditor, PrefixedContext, PropertyDialog, PropertyItem, SaveResponse, ServiceError, ServiceOptions, ServiceRequest, ServiceResponse, SettingStorage, ToolButton, Widget, WidgetProps } from '@serenity-is/corelib';
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
export declare const Texts: typeof texts;
export declare const BasicProgressDialogTexts: typeof texts.Site.BasicProgressDialog;
export declare const BulkServiceActionTexts: typeof texts.Site.BulkServiceAction;
export declare const ChangePasswordFormTexts: typeof texts.Forms.Membership.ChangePassword;
export declare const DialogUtilsTexts: typeof texts.Site.Dialogs;
export declare const ExtensionsTexts: typeof texts;
export declare const ForgotPasswordFormTexts: typeof texts.Forms.Membership.ForgotPassword;
export declare const PasswordStrengthValidationTexts: typeof texts.Validation;
export declare const ResetPasswordFormTexts: typeof texts.Forms.Membership.ResetPassword;
export declare const SetPasswordFormTexts: typeof texts.Forms.Membership.SetPassword;
export declare const TranslationTexts: typeof texts.Site.Translation;
export declare class BasicProgressDialog<P = {}> extends BaseDialog<P> {
	constructor(props?: WidgetProps<P>);
	cancelled: boolean;
	get max(): number;
	set max(value: number);
	get value(): number;
	set value(value: number);
	get title(): string;
	set title(value: string);
	cancelTitle: string;
	getDialogButtons(): {
		text: string;
		class: string;
		click: () => void;
	}[];
	getDialogOptions(): import("@serenity-is/corelib").DialogOptions;
	initDialog(): void;
	renderContents(): any;
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
export declare abstract class GridEditorBase<TEntity, P = {}> extends EntityGrid<TEntity, P> implements IGetEditValue, ISetEditValue {
	protected getIdProperty(): string;
	protected nextId: number;
	constructor(props: EditorProps<P>);
	protected id(entity: TEntity): any;
	protected getNextId(): string;
	protected setNewId(entity: TEntity): void;
	protected save(opt: ServiceOptions<any>, callback: (r: ServiceResponse) => void): void;
	protected deleteEntity(id: number): boolean;
	protected validateEntity(row: TEntity, id: number): boolean;
	protected setEntities(items: TEntity[]): void;
	protected getNewEntity(): TEntity;
	protected getButtons(): ToolButton[];
	protected editItem(entityOrId: any): void;
	getEditValue(property: any, target: any): void;
	setEditValue(source: any, property: any): void;
	get value(): TEntity[];
	set value(value: TEntity[]);
	protected getGridCanLoad(): boolean;
	protected usePager(): boolean;
	protected getInitialTitle(): any;
	protected createQuickSearchInput(): void;
}
export declare abstract class GridEditorDialog<TEntity, P = {}> extends EntityDialog<TEntity, P> {
	protected getIdProperty(): string;
	onSave: (options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void) => void;
	onDelete: (options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void) => void;
	destroy(): void;
	protected updateInterface(): void;
	protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void): void;
	protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): void;
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
