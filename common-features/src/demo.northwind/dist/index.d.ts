import { BaseDialog, BooleanEditor, CaptureOperationType, ColumnsBase, DateEditor, DecimalEditor, DeleteRequest, DeleteResponse, DialogButton, EditorProps, EditorWidget, EmailAddressEditor, EntityDialog, EntityGrid, EnumEditor, Formatter, IGetEditValue, ISetEditValue, ImageUploadEditor, IntegerEditor, ListRequest, ListResponse, LookupEditor, LookupEditorBase, LookupEditorOptions, PrefixedContext, PropertyItem, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse, ServiceOptions, StringEditor, ToolButton, WidgetProps } from '@serenity-is/corelib';
import { GetNextNumberRequest, GetNextNumberResponse, GridEditorBase, GridEditorDialog } from '@serenity-is/extensions';
import { Column, FormatterContext, FormatterResult } from '@serenity-is/sleekgrid';

export interface CategoryRow {
	CategoryID?: number;
	CategoryName?: string;
	Description?: string;
	Picture?: number[];
}
export declare abstract class CategoryRow {
	static readonly idProperty = "CategoryID";
	static readonly nameProperty = "CategoryName";
	static readonly localTextPrefix = "Northwind.Category";
	static readonly lookupKey = "Northwind.Category";
	/** @deprecated use getLookupAsync instead */
	static getLookup(): import("@serenity-is/corelib").Lookup<CategoryRow>;
	static getLookupAsync(): Promise<import("@serenity-is/corelib").Lookup<CategoryRow>>;
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof CategoryRow, string>>;
}
export interface CategoryColumns {
	CategoryID: Column<CategoryRow>;
	CategoryName: Column<CategoryRow>;
	Description: Column<CategoryRow>;
}
export declare class CategoryColumns extends ColumnsBase<CategoryRow> {
	static readonly columnsKey = "Northwind.Category";
	static readonly Fields: Readonly<Record<keyof CategoryColumns, string>>;
}
export interface CategoryForm {
	CategoryName: StringEditor;
	Description: StringEditor;
}
export declare class CategoryForm extends PrefixedContext {
	static readonly formKey = "Northwind.Category";
	private static init;
	constructor(prefix: string);
}
export interface CategoryLangRow {
	Id?: number;
	CategoryId?: number;
	LanguageId?: number;
	CategoryName?: string;
	Description?: string;
}
export declare abstract class CategoryLangRow {
	static readonly idProperty = "Id";
	static readonly nameProperty = "CategoryName";
	static readonly localTextPrefix = "Northwind.CategoryLang";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof CategoryLangRow, string>>;
}
export declare namespace CategoryLangService {
	const baseUrl = "Serenity.Demo.Northwind/CategoryLang";
	function Create(request: SaveRequest<CategoryLangRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<CategoryLangRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<CategoryLangRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<CategoryLangRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<CategoryLangRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<CategoryLangRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/CategoryLang/Create";
		readonly Update: "Serenity.Demo.Northwind/CategoryLang/Update";
		readonly Delete: "Serenity.Demo.Northwind/CategoryLang/Delete";
		readonly Retrieve: "Serenity.Demo.Northwind/CategoryLang/Retrieve";
		readonly List: "Serenity.Demo.Northwind/CategoryLang/List";
	};
}
export declare namespace CategoryService {
	const baseUrl = "Serenity.Demo.Northwind/Category";
	function Create(request: SaveRequest<CategoryRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<CategoryRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<CategoryRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<CategoryRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<CategoryRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<CategoryRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/Category/Create";
		readonly Update: "Serenity.Demo.Northwind/Category/Update";
		readonly Delete: "Serenity.Demo.Northwind/Category/Delete";
		readonly Retrieve: "Serenity.Demo.Northwind/Category/Retrieve";
		readonly List: "Serenity.Demo.Northwind/Category/List";
	};
}
export interface NoteRow {
	NoteId?: number;
	EntityType?: string;
	EntityId?: number;
	Text?: string;
	InsertUserId?: number;
	InsertDate?: string;
	InsertUserDisplayName?: string;
}
export declare abstract class NoteRow {
	static readonly idProperty = "NoteId";
	static readonly nameProperty = "EntityType";
	static readonly localTextPrefix = "Northwind.Note";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof NoteRow, string>>;
}
export interface CustomerRow {
	ID?: number;
	CustomerID?: string;
	CompanyName?: string;
	ContactName?: string;
	ContactTitle?: string;
	Address?: string;
	City?: string;
	Region?: string;
	PostalCode?: string;
	Country?: string;
	Phone?: string;
	Fax?: string;
	NoteList?: NoteRow[];
	Representatives?: number[];
	LastContactDate?: string;
	LastContactedBy?: number;
	Email?: string;
	SendBulletin?: boolean;
}
export declare abstract class CustomerRow {
	static readonly idProperty = "ID";
	static readonly nameProperty = "CompanyName";
	static readonly localTextPrefix = "Northwind.Customer";
	static readonly lookupKey = "Northwind.Customer";
	/** @deprecated use getLookupAsync instead */
	static getLookup(): import("@serenity-is/corelib").Lookup<CustomerRow>;
	static getLookupAsync(): Promise<import("@serenity-is/corelib").Lookup<CustomerRow>>;
	static readonly deletePermission = "Northwind:Customer:Delete";
	static readonly insertPermission = "Northwind:Customer:Modify";
	static readonly readPermission = "Northwind:Customer:View";
	static readonly updatePermission = "Northwind:Customer:Modify";
	static readonly Fields: Readonly<Record<keyof CustomerRow, string>>;
}
export interface CustomerColumns {
	CustomerID: Column<CustomerRow>;
	CompanyName: Column<CustomerRow>;
	ContactName: Column<CustomerRow>;
	ContactTitle: Column<CustomerRow>;
	Region: Column<CustomerRow>;
	PostalCode: Column<CustomerRow>;
	Country: Column<CustomerRow>;
	City: Column<CustomerRow>;
	Phone: Column<CustomerRow>;
	Fax: Column<CustomerRow>;
	Representatives: Column<CustomerRow>;
}
export declare class CustomerColumns extends ColumnsBase<CustomerRow> {
	static readonly columnsKey = "Northwind.Customer";
	static readonly Fields: Readonly<Record<keyof CustomerColumns, string>>;
}
export interface CustomerCustomerDemoRow {
	ID?: number;
	CustomerID?: string;
	CustomerTypeID?: string;
	CustomerCompanyName?: string;
}
export declare abstract class CustomerCustomerDemoRow {
	static readonly idProperty = "ID";
	static readonly nameProperty = "CustomerID";
	static readonly localTextPrefix = "Northwind.CustomerCustomerDemo";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof CustomerCustomerDemoRow, string>>;
}
export interface CustomerDemographicRow {
	ID?: number;
	CustomerTypeID?: string;
	CustomerDesc?: string;
}
export declare abstract class CustomerDemographicRow {
	static readonly idProperty = "ID";
	static readonly nameProperty = "CustomerTypeID";
	static readonly localTextPrefix = "Northwind.CustomerDemographic";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof CustomerDemographicRow, string>>;
}
export interface CustomerDetailsRow {
	Id?: number;
	LastContactDate?: string;
	LastContactedBy?: number;
	Email?: string;
	SendBulletin?: boolean;
	LastContactedByFullName?: string;
}
export declare abstract class CustomerDetailsRow {
	static readonly idProperty = "Id";
	static readonly nameProperty = "Email";
	static readonly localTextPrefix = "Northwind.CustomerDetails";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof CustomerDetailsRow, string>>;
}
export declare class NotesEditor<P = {}> extends EditorWidget<P> implements IGetEditValue, ISetEditValue {
	private isDirty;
	private items;
	private noteList;
	protected renderContents(): any;
	protected updateContent(): void;
	protected addClick(): void;
	protected editClick(e: any): void;
	deleteClick(e: any): void;
	get value(): NoteRow[];
	set value(value: NoteRow[]);
	getEditValue(prop: PropertyItem, target: any): void;
	setEditValue(source: any, prop: PropertyItem): void;
	get_isDirty(): boolean;
	set_isDirty(value: any): void;
	onChange: () => void;
}
export interface CustomerForm {
	CustomerID: StringEditor;
	CompanyName: StringEditor;
	ContactName: StringEditor;
	ContactTitle: StringEditor;
	Representatives: LookupEditor;
	Address: StringEditor;
	Country: LookupEditor;
	City: LookupEditor;
	Region: StringEditor;
	PostalCode: StringEditor;
	Phone: StringEditor;
	Fax: StringEditor;
	NoteList: NotesEditor;
	LastContactDate: DateEditor;
	LastContactedBy: LookupEditor;
	Email: EmailAddressEditor;
	SendBulletin: BooleanEditor;
}
export declare class CustomerForm extends PrefixedContext {
	static readonly formKey = "Northwind.Customer";
	private static init;
	constructor(prefix: string);
}
export interface CustomerGrossSalesRow {
	CustomerId?: string;
	ContactName?: string;
	ProductId?: number;
	ProductName?: string;
	GrossAmount?: number;
}
export declare abstract class CustomerGrossSalesRow {
	static readonly nameProperty = "ContactName";
	static readonly localTextPrefix = "Northwind.CustomerGrossSales";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof CustomerGrossSalesRow, string>>;
}
export interface CustomerRepresentativesRow {
	RepresentativeId?: number;
	CustomerId?: number;
	EmployeeId?: number;
}
export declare abstract class CustomerRepresentativesRow {
	static readonly idProperty = "RepresentativeId";
	static readonly localTextPrefix = "Northwind.CustomerRepresentatives";
	static readonly deletePermission = "Northwind:Customer:View";
	static readonly insertPermission = "Northwind:Customer:View";
	static readonly readPermission = "Northwind:Customer:View";
	static readonly updatePermission = "Northwind:Customer:View";
	static readonly Fields: Readonly<Record<keyof CustomerRepresentativesRow, string>>;
}
export declare namespace CustomerService {
	const baseUrl = "Serenity.Demo.Northwind/Customer";
	function Create(request: SaveRequest<CustomerRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<CustomerRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function GetNextNumber(request: GetNextNumberRequest, onSuccess?: (response: GetNextNumberResponse) => void, opt?: ServiceOptions<any>): PromiseLike<GetNextNumberResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<CustomerRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<CustomerRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<CustomerRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<CustomerRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/Customer/Create";
		readonly Update: "Serenity.Demo.Northwind/Customer/Update";
		readonly Delete: "Serenity.Demo.Northwind/Customer/Delete";
		readonly GetNextNumber: "Serenity.Demo.Northwind/Customer/GetNextNumber";
		readonly Retrieve: "Serenity.Demo.Northwind/Customer/Retrieve";
		readonly List: "Serenity.Demo.Northwind/Customer/List";
	};
}
export declare enum Gender {
	Male = 1,
	Female = 2
}
export interface EmployeeRow {
	EmployeeID?: number;
	LastName?: string;
	FirstName?: string;
	FullName?: string;
	Title?: string;
	TitleOfCourtesy?: string;
	BirthDate?: string;
	HireDate?: string;
	Address?: string;
	City?: string;
	Region?: string;
	PostalCode?: string;
	Country?: string;
	HomePhone?: string;
	Extension?: string;
	Photo?: number[];
	Notes?: string;
	ReportsTo?: number;
	PhotoPath?: string;
	ReportsToFullName?: string;
	Gender?: Gender;
}
export declare abstract class EmployeeRow {
	static readonly idProperty = "EmployeeID";
	static readonly nameProperty = "FullName";
	static readonly localTextPrefix = "Northwind.Employee";
	static readonly lookupKey = "Northwind.Employee";
	/** @deprecated use getLookupAsync instead */
	static getLookup(): import("@serenity-is/corelib").Lookup<EmployeeRow>;
	static getLookupAsync(): Promise<import("@serenity-is/corelib").Lookup<EmployeeRow>>;
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof EmployeeRow, string>>;
}
export interface EmployeeTerritoryRow {
	EmployeeID?: number;
	TerritoryID?: string;
	EmployeeFullName?: string;
	TerritoryDescription?: string;
}
export declare abstract class EmployeeTerritoryRow {
	static readonly idProperty = "EmployeeID";
	static readonly nameProperty = "TerritoryID";
	static readonly localTextPrefix = "Northwind.EmployeeTerritory";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof EmployeeTerritoryRow, string>>;
}
export interface OrderDetailRow {
	DetailID?: number;
	OrderID?: number;
	ProductID?: number;
	UnitPrice?: number;
	Quantity?: number;
	Discount?: number;
	OrderCustomerID?: string;
	OrderEmployeeID?: number;
	OrderDate?: string;
	ProductName?: string;
	LineTotal?: number;
}
export declare abstract class OrderDetailRow {
	static readonly idProperty = "DetailID";
	static readonly localTextPrefix = "Northwind.OrderDetail";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof OrderDetailRow, string>>;
}
export declare enum OrderShippingState {
	NotShipped = 0,
	Shipped = 1
}
export interface OrderRow {
	OrderID?: number;
	CustomerID?: string;
	EmployeeID?: number;
	OrderDate?: string;
	RequiredDate?: string;
	ShippedDate?: string;
	ShipVia?: number;
	Freight?: number;
	ShipName?: string;
	ShipAddress?: string;
	ShipCity?: string;
	ShipRegion?: string;
	ShipPostalCode?: string;
	ShipCountry?: string;
	CustomerCompanyName?: string;
	CustomerContactName?: string;
	CustomerContactTitle?: string;
	CustomerCity?: string;
	CustomerRegion?: string;
	CustomerCountry?: string;
	CustomerPhone?: string;
	CustomerFax?: string;
	EmployeeFullName?: string;
	EmployeeGender?: Gender;
	ShipViaCompanyName?: string;
	ShippingState?: OrderShippingState;
	DetailList?: OrderDetailRow[];
}
export declare abstract class OrderRow {
	static readonly idProperty = "OrderID";
	static readonly nameProperty = "CustomerID";
	static readonly localTextPrefix = "Northwind.Order";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof OrderRow, string>>;
}
export interface OrderColumns {
	OrderID: Column<OrderRow>;
	CustomerCompanyName: Column<OrderRow>;
	OrderDate: Column<OrderRow>;
	EmployeeFullName: Column<OrderRow>;
	RequiredDate: Column<OrderRow>;
	ShippingState: Column<OrderRow>;
	ShippedDate: Column<OrderRow>;
	ShipViaCompanyName: Column<OrderRow>;
	ShipCountry: Column<OrderRow>;
	ShipCity: Column<OrderRow>;
	Freight: Column<OrderRow>;
}
export declare class OrderColumns extends ColumnsBase<OrderRow> {
	static readonly columnsKey = "Northwind.Order";
	static readonly Fields: Readonly<Record<keyof OrderColumns, string>>;
}
export interface OrderDetailColumns {
	ProductName: Column<OrderDetailRow>;
	UnitPrice: Column<OrderDetailRow>;
	Quantity: Column<OrderDetailRow>;
	Discount: Column<OrderDetailRow>;
	LineTotal: Column<OrderDetailRow>;
}
export declare class OrderDetailColumns extends ColumnsBase<OrderDetailRow> {
	static readonly columnsKey = "Northwind.OrderDetail";
	static readonly Fields: Readonly<Record<keyof OrderDetailColumns, string>>;
}
export interface OrderDetailForm {
	ProductID: LookupEditor;
	UnitPrice: DecimalEditor;
	Quantity: IntegerEditor;
	Discount: DecimalEditor;
}
export declare class OrderDetailForm extends PrefixedContext {
	static readonly formKey = "Northwind.OrderDetail";
	private static init;
	constructor(prefix: string);
}
export declare namespace OrderDetailService {
	const baseUrl = "Serenity.Demo.Northwind/OrderDetail";
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<OrderDetailRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<OrderDetailRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<OrderDetailRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<OrderDetailRow>>;
	const Methods: {
		readonly Retrieve: "Serenity.Demo.Northwind/OrderDetail/Retrieve";
		readonly List: "Serenity.Demo.Northwind/OrderDetail/List";
	};
}
export declare class CustomerEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, CustomerRow> {
	constructor(props: EditorProps<P>);
	protected getLookupKey(): string;
	protected getItemText(item: any, lookup: any): string;
}
export declare class OrderDetailDialog extends GridEditorDialog<OrderDetailRow> {
	protected getFormKey(): string;
	protected getLocalTextPrefix(): string;
	protected form: OrderDetailForm;
	constructor(props: WidgetProps<any>);
}
export declare class OrderDetailsEditor<P = {}> extends GridEditorBase<OrderDetailRow, P> {
	protected getColumnsKey(): string;
	protected getDialogType(): typeof OrderDetailDialog;
	protected getLocalTextPrefix(): string;
	validateEntity(row: any, id: any): boolean;
}
export interface OrderForm {
	CustomerID: CustomerEditor;
	OrderDate: DateEditor;
	RequiredDate: DateEditor;
	EmployeeID: LookupEditor;
	DetailList: OrderDetailsEditor;
	ShippedDate: DateEditor;
	ShipVia: LookupEditor;
	Freight: DecimalEditor;
	ShipName: StringEditor;
	ShipAddress: StringEditor;
	ShipCity: StringEditor;
	ShipRegion: StringEditor;
	ShipPostalCode: StringEditor;
	ShipCountry: StringEditor;
}
export declare class OrderForm extends PrefixedContext {
	static readonly formKey = "Northwind.Order";
	private static init;
	constructor(prefix: string);
}
export interface OrderListRequest extends ListRequest {
	ProductID?: number;
}
export declare namespace OrderService {
	const baseUrl = "Serenity.Demo.Northwind/Order";
	function Create(request: SaveRequest<OrderRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<OrderRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<OrderRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<OrderRow>>;
	function List(request: OrderListRequest, onSuccess?: (response: ListResponse<OrderRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<OrderRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/Order/Create";
		readonly Update: "Serenity.Demo.Northwind/Order/Update";
		readonly Delete: "Serenity.Demo.Northwind/Order/Delete";
		readonly Retrieve: "Serenity.Demo.Northwind/Order/Retrieve";
		readonly List: "Serenity.Demo.Northwind/Order/List";
	};
}
export declare namespace PermissionKeys {
	const General = "Northwind:General";
	namespace Customer {
		const Delete = "Northwind:Customer:Delete";
		const Modify = "Northwind:Customer:Modify";
		const View = "Northwind:Customer:View";
	}
}
export interface ProductRow {
	ProductID?: number;
	ProductName?: string;
	ProductImage?: string;
	Discontinued?: boolean;
	SupplierID?: number;
	CategoryID?: number;
	QuantityPerUnit?: string;
	UnitPrice?: number;
	UnitsInStock?: number;
	UnitsOnOrder?: number;
	ReorderLevel?: number;
	SupplierCompanyName?: string;
	SupplierCountry?: string;
	CategoryName?: string;
}
export declare abstract class ProductRow {
	static readonly idProperty = "ProductID";
	static readonly nameProperty = "ProductName";
	static readonly localTextPrefix = "Northwind.Product";
	static readonly lookupKey = "Northwind.Product";
	/** @deprecated use getLookupAsync instead */
	static getLookup(): import("@serenity-is/corelib").Lookup<ProductRow>;
	static getLookupAsync(): Promise<import("@serenity-is/corelib").Lookup<ProductRow>>;
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof ProductRow, string>>;
}
export interface ProductColumns {
	ProductID: Column<ProductRow>;
	ProductName: Column<ProductRow>;
	Discontinued: Column<ProductRow>;
	SupplierCompanyName: Column<ProductRow>;
	CategoryName: Column<ProductRow>;
	QuantityPerUnit: Column<ProductRow>;
	UnitPrice: Column<ProductRow>;
	UnitsInStock: Column<ProductRow>;
	UnitsOnOrder: Column<ProductRow>;
	ReorderLevel: Column<ProductRow>;
}
export declare class ProductColumns extends ColumnsBase<ProductRow> {
	static readonly columnsKey = "Northwind.Product";
	static readonly Fields: Readonly<Record<keyof ProductColumns, string>>;
}
export interface ProductForm {
	ProductName: StringEditor;
	ProductImage: ImageUploadEditor;
	Discontinued: BooleanEditor;
	SupplierID: LookupEditor;
	CategoryID: LookupEditor;
	QuantityPerUnit: StringEditor;
	UnitPrice: DecimalEditor;
	UnitsInStock: IntegerEditor;
	UnitsOnOrder: IntegerEditor;
	ReorderLevel: IntegerEditor;
}
export declare class ProductForm extends PrefixedContext {
	static readonly formKey = "Northwind.Product";
	private static init;
	constructor(prefix: string);
}
export interface ProductLangRow {
	Id?: number;
	ProductId?: number;
	LanguageId?: number;
	ProductName?: string;
}
export declare abstract class ProductLangRow {
	static readonly idProperty = "Id";
	static readonly nameProperty = "ProductName";
	static readonly localTextPrefix = "Northwind.ProductLang";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof ProductLangRow, string>>;
}
export declare namespace ProductLangService {
	const baseUrl = "Serenity.Demo.Northwind/ProductLang";
	function Create(request: SaveRequest<ProductLangRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<ProductLangRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<ProductLangRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<ProductLangRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<ProductLangRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<ProductLangRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/ProductLang/Create";
		readonly Update: "Serenity.Demo.Northwind/ProductLang/Update";
		readonly Delete: "Serenity.Demo.Northwind/ProductLang/Delete";
		readonly Retrieve: "Serenity.Demo.Northwind/ProductLang/Retrieve";
		readonly List: "Serenity.Demo.Northwind/ProductLang/List";
	};
}
export interface ProductLogRow {
	ProductLogID?: number;
	OperationType?: CaptureOperationType;
	ChangingUserId?: number;
	ValidFrom?: string;
	ValidUntil?: string;
	ProductID?: number;
	ProductName?: string;
	ProductImage?: string;
	Discontinued?: boolean;
	SupplierID?: number;
	CategoryID?: number;
	QuantityPerUnit?: string;
	UnitPrice?: number;
	UnitsInStock?: number;
	UnitsOnOrder?: number;
	ReorderLevel?: number;
}
export declare abstract class ProductLogRow {
	static readonly idProperty = "ProductLogID";
	static readonly localTextPrefix = "Northwind.ProductLog";
	static readonly deletePermission: any;
	static readonly insertPermission: any;
	static readonly readPermission = "";
	static readonly updatePermission: any;
	static readonly Fields: Readonly<Record<keyof ProductLogRow, string>>;
}
export declare namespace ProductService {
	const baseUrl = "Serenity.Demo.Northwind/Product";
	function Create(request: SaveRequest<ProductRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<ProductRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<ProductRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<ProductRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<ProductRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<ProductRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/Product/Create";
		readonly Update: "Serenity.Demo.Northwind/Product/Update";
		readonly Delete: "Serenity.Demo.Northwind/Product/Delete";
		readonly Retrieve: "Serenity.Demo.Northwind/Product/Retrieve";
		readonly List: "Serenity.Demo.Northwind/Product/List";
	};
}
export interface RegionRow {
	RegionID?: number;
	RegionDescription?: string;
}
export declare abstract class RegionRow {
	static readonly idProperty = "RegionID";
	static readonly nameProperty = "RegionDescription";
	static readonly localTextPrefix = "Northwind.Region";
	static readonly lookupKey = "Northwind.Region";
	/** @deprecated use getLookupAsync instead */
	static getLookup(): import("@serenity-is/corelib").Lookup<RegionRow>;
	static getLookupAsync(): Promise<import("@serenity-is/corelib").Lookup<RegionRow>>;
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof RegionRow, string>>;
}
export interface RegionColumns {
	RegionID: Column<RegionRow>;
	RegionDescription: Column<RegionRow>;
}
export declare class RegionColumns extends ColumnsBase<RegionRow> {
	static readonly columnsKey = "Northwind.Region";
	static readonly Fields: Readonly<Record<keyof RegionColumns, string>>;
}
export interface RegionForm {
	RegionID: IntegerEditor;
	RegionDescription: StringEditor;
}
export declare class RegionForm extends PrefixedContext {
	static readonly formKey = "Northwind.Region";
	private static init;
	constructor(prefix: string);
}
export declare namespace RegionService {
	const baseUrl = "Serenity.Demo.Northwind/Region";
	function Create(request: SaveRequest<RegionRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<RegionRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<RegionRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<RegionRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<RegionRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<RegionRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/Region/Create";
		readonly Update: "Serenity.Demo.Northwind/Region/Update";
		readonly Delete: "Serenity.Demo.Northwind/Region/Delete";
		readonly Retrieve: "Serenity.Demo.Northwind/Region/Retrieve";
		readonly List: "Serenity.Demo.Northwind/Region/List";
	};
}
export interface SalesByCategoryRow {
	CategoryId?: number;
	CategoryName?: string;
	ProductName?: string;
	ProductSales?: number;
}
export declare abstract class SalesByCategoryRow {
	static readonly nameProperty = "CategoryName";
	static readonly localTextPrefix = "Northwind.SalesByCategory";
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof SalesByCategoryRow, string>>;
}
export interface SalesByCategoryColumns {
	CategoryName: Column<SalesByCategoryRow>;
	ProductName: Column<SalesByCategoryRow>;
	ProductSales: Column<SalesByCategoryRow>;
}
export declare class SalesByCategoryColumns extends ColumnsBase<SalesByCategoryRow> {
	static readonly columnsKey = "Northwind.SalesByCategory";
	static readonly Fields: Readonly<Record<keyof SalesByCategoryColumns, string>>;
}
export declare namespace SalesByCategoryService {
	const baseUrl = "Serenity.Demo.Northwind/SalesByCategory";
	function List(request: ListRequest, onSuccess?: (response: ListResponse<SalesByCategoryRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<SalesByCategoryRow>>;
	const Methods: {
		readonly List: "Serenity.Demo.Northwind/SalesByCategory/List";
	};
}
export interface ShipperRow {
	ShipperID?: number;
	CompanyName?: string;
	Phone?: string;
}
export declare abstract class ShipperRow {
	static readonly idProperty = "ShipperID";
	static readonly nameProperty = "CompanyName";
	static readonly localTextPrefix = "Northwind.Shipper";
	static readonly lookupKey = "Northwind.Shipper";
	/** @deprecated use getLookupAsync instead */
	static getLookup(): import("@serenity-is/corelib").Lookup<ShipperRow>;
	static getLookupAsync(): Promise<import("@serenity-is/corelib").Lookup<ShipperRow>>;
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof ShipperRow, string>>;
}
export interface ShipperColumns {
	ShipperID: Column<ShipperRow>;
	CompanyName: Column<ShipperRow>;
	Phone: Column<ShipperRow>;
}
export declare class ShipperColumns extends ColumnsBase<ShipperRow> {
	static readonly columnsKey = "Northwind.Shipper";
	static readonly Fields: Readonly<Record<keyof ShipperColumns, string>>;
}
export interface PhoneEditorOptions {
	multiple?: boolean;
}
export declare class PhoneEditor<P extends PhoneEditorOptions = PhoneEditorOptions> extends StringEditor<P> {
	constructor(props: WidgetProps<P>);
	protected formatValue(): void;
	protected getFormattedValue(): string;
	get_value(): string;
	set_value(value: string): void;
	static validate(phone: string, isMultiple: boolean): string;
	static isValidPhone(phone: string): boolean;
	static formatPhone(phone: any): any;
	static formatMulti(phone: string, format: (s: string) => string): string;
	static isValidMulti(phone: string, check: (s: string) => boolean): boolean;
}
export interface ShipperForm {
	CompanyName: StringEditor;
	Phone: PhoneEditor;
}
export declare class ShipperForm extends PrefixedContext {
	static readonly formKey = "Northwind.Shipper";
	private static init;
	constructor(prefix: string);
}
export declare namespace ShipperService {
	const baseUrl = "Serenity.Demo.Northwind/Shipper";
	function Create(request: SaveRequest<ShipperRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<ShipperRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<ShipperRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<ShipperRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<ShipperRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<ShipperRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/Shipper/Create";
		readonly Update: "Serenity.Demo.Northwind/Shipper/Update";
		readonly Delete: "Serenity.Demo.Northwind/Shipper/Delete";
		readonly Retrieve: "Serenity.Demo.Northwind/Shipper/Retrieve";
		readonly List: "Serenity.Demo.Northwind/Shipper/List";
	};
}
export interface SupplierRow {
	SupplierID?: number;
	CompanyName?: string;
	ContactName?: string;
	ContactTitle?: string;
	Address?: string;
	City?: string;
	Region?: string;
	PostalCode?: string;
	Country?: string;
	Phone?: string;
	Fax?: string;
	HomePage?: string;
}
export declare abstract class SupplierRow {
	static readonly idProperty = "SupplierID";
	static readonly nameProperty = "CompanyName";
	static readonly localTextPrefix = "Northwind.Supplier";
	static readonly lookupKey = "Northwind.Supplier";
	/** @deprecated use getLookupAsync instead */
	static getLookup(): import("@serenity-is/corelib").Lookup<SupplierRow>;
	static getLookupAsync(): Promise<import("@serenity-is/corelib").Lookup<SupplierRow>>;
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof SupplierRow, string>>;
}
export interface SupplierColumns {
	SupplierID: Column<SupplierRow>;
	CompanyName: Column<SupplierRow>;
	ContactName: Column<SupplierRow>;
	ContactTitle: Column<SupplierRow>;
	Phone: Column<SupplierRow>;
	Region: Column<SupplierRow>;
	Country: Column<SupplierRow>;
	City: Column<SupplierRow>;
}
export declare class SupplierColumns extends ColumnsBase<SupplierRow> {
	static readonly columnsKey = "Northwind.Supplier";
	static readonly Fields: Readonly<Record<keyof SupplierColumns, string>>;
}
export interface SupplierForm {
	CompanyName: StringEditor;
	ContactName: StringEditor;
	ContactTitle: StringEditor;
	Address: StringEditor;
	Region: StringEditor;
	PostalCode: StringEditor;
	Country: StringEditor;
	City: StringEditor;
	Phone: StringEditor;
	Fax: StringEditor;
	HomePage: StringEditor;
}
export declare class SupplierForm extends PrefixedContext {
	static readonly formKey = "Northwind.Supplier";
	private static init;
	constructor(prefix: string);
}
export declare namespace SupplierService {
	const baseUrl = "Serenity.Demo.Northwind/Supplier";
	function Create(request: SaveRequest<SupplierRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<SupplierRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<SupplierRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<SupplierRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<SupplierRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<SupplierRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/Supplier/Create";
		readonly Update: "Serenity.Demo.Northwind/Supplier/Update";
		readonly Delete: "Serenity.Demo.Northwind/Supplier/Delete";
		readonly Retrieve: "Serenity.Demo.Northwind/Supplier/Retrieve";
		readonly List: "Serenity.Demo.Northwind/Supplier/List";
	};
}
export interface TerritoryRow {
	ID?: number;
	TerritoryID?: string;
	TerritoryDescription?: string;
	RegionID?: number;
	RegionDescription?: string;
}
export declare abstract class TerritoryRow {
	static readonly idProperty = "ID";
	static readonly nameProperty = "TerritoryID";
	static readonly localTextPrefix = "Northwind.Territory";
	static readonly lookupKey = "Northwind.Territory";
	/** @deprecated use getLookupAsync instead */
	static getLookup(): import("@serenity-is/corelib").Lookup<TerritoryRow>;
	static getLookupAsync(): Promise<import("@serenity-is/corelib").Lookup<TerritoryRow>>;
	static readonly deletePermission = "Northwind:General";
	static readonly insertPermission = "Northwind:General";
	static readonly readPermission = "Northwind:General";
	static readonly updatePermission = "Northwind:General";
	static readonly Fields: Readonly<Record<keyof TerritoryRow, string>>;
}
export interface TerritoryColumns {
	TerritoryID: Column<TerritoryRow>;
	TerritoryDescription: Column<TerritoryRow>;
	RegionDescription: Column<TerritoryRow>;
}
export declare class TerritoryColumns extends ColumnsBase<TerritoryRow> {
	static readonly columnsKey = "Northwind.Territory";
	static readonly Fields: Readonly<Record<keyof TerritoryColumns, string>>;
}
export interface TerritoryForm {
	TerritoryID: StringEditor;
	TerritoryDescription: StringEditor;
	RegionID: LookupEditor;
}
export declare class TerritoryForm extends PrefixedContext {
	static readonly formKey = "Northwind.Territory";
	private static init;
	constructor(prefix: string);
}
export declare namespace TerritoryService {
	const baseUrl = "Serenity.Demo.Northwind/Territory";
	function Create(request: SaveRequest<TerritoryRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Update(request: SaveRequest<TerritoryRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
	function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
	function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<TerritoryRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<TerritoryRow>>;
	function List(request: ListRequest, onSuccess?: (response: ListResponse<TerritoryRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<TerritoryRow>>;
	const Methods: {
		readonly Create: "Serenity.Demo.Northwind/Territory/Create";
		readonly Update: "Serenity.Demo.Northwind/Territory/Update";
		readonly Delete: "Serenity.Demo.Northwind/Territory/Delete";
		readonly Retrieve: "Serenity.Demo.Northwind/Territory/Retrieve";
		readonly List: "Serenity.Demo.Northwind/Territory/List";
	};
}
export declare class CategoryDialog<P = {}> extends EntityDialog<CategoryRow, P> {
	protected getFormKey(): string;
	protected getRowDefinition(): typeof CategoryRow;
	protected getService(): string;
	protected form: CategoryForm;
}
export declare class CategoryGrid<P = {}> extends EntityGrid<CategoryRow, P> {
	protected getColumnsKey(): string;
	protected getDialogType(): any;
	protected getRowDefinition(): typeof CategoryRow;
	protected getService(): string;
}
export declare class CustomerDialog<P = {}> extends EntityDialog<CustomerRow, P> {
	protected getFormKey(): string;
	protected getRowDefinition(): typeof CustomerRow;
	protected getService(): string;
	protected form: CustomerForm;
	private ordersGrid;
	private loadedState;
	constructor(props: WidgetProps<P>);
	initDialog(): void;
	getSaveState(): string;
	loadResponse(data: any): void;
	loadEntity(entity: CustomerRow): void;
	onSaveSuccess(response: any): void;
	renderContents(): any;
}
export declare class CustomerGrid<P = {}> extends EntityGrid<CustomerRow, P> {
	protected getColumnsKey(): string;
	protected getDialogType(): any;
	protected getRowDefinition(): typeof CustomerRow;
	protected getService(): string;
}
export declare class OrderDialog<P = {}> extends EntityDialog<OrderRow, P> {
	protected getFormKey(): string;
	protected getRowDefinition(): typeof OrderRow;
	protected getService(): string;
	protected form: OrderForm;
	getToolbarButtons(): import("@serenity-is/corelib").ToolButton[];
	protected updateInterface(): void;
}
export declare class CustomerOrderDialog extends OrderDialog {
	updateInterface(): void;
}
export declare class OrderGrid<P = {}> extends EntityGrid<OrderRow, P> {
	protected getColumnsKey(): string;
	protected getDialogType(): any;
	protected getRowDefinition(): typeof OrderRow;
	protected getService(): string;
	protected shippingStateFilter: EnumEditor;
	protected getQuickFilters(): import("@serenity-is/corelib").QuickFilter<import("@serenity-is/corelib").Widget<any>, any>[];
	protected createQuickFilters(): void;
	protected getButtons(): ToolButton[];
	protected getColumns(): import("@serenity-is/sleekgrid").Column<OrderRow>[];
	protected onClick(e: Event, row: number, cell: number): void;
	set_shippingState(value: number): void;
	protected addButtonClick(): void;
}
export declare class CustomerOrdersGrid<P = {}> extends OrderGrid<P> {
	protected getDialogType(): typeof CustomerOrderDialog;
	protected getColumns(): Column[];
	protected initEntityDialog(itemType: any, dialog: any): void;
	protected getButtons(): import("@serenity-is/corelib").ToolButton[];
	protected addButtonClick(): void;
	protected getInitialTitle(): any;
	protected getGridCanLoad(): boolean;
	private _customerID;
	get customerID(): string;
	set customerID(value: string);
}
export declare class EmployeeListFormatter implements Formatter {
	format(ctx: FormatterContext): FormatterResult;
}
export declare class EmployeeFormatter implements Formatter {
	readonly props: {
		genderProperty?: string;
	};
	constructor(props?: {
		genderProperty?: string;
	});
	format(ctx: FormatterContext): FormatterResult;
	initializeColumn(column: Column): void;
}
export declare class NoteDialog<P = {}> extends BaseDialog<P> {
	private textEditor;
	protected renderContents(): any;
	protected getDialogButtons(): DialogButton[];
	get text(): string;
	set text(value: string);
	okClick: () => void;
}
export declare class FreightFormatter implements Formatter {
	format(ctx: FormatterContext): string;
}
export declare class ProductDialog<P = {}> extends EntityDialog<ProductRow, P> {
	protected getFormKey(): string;
	protected getRowDefinition(): typeof ProductRow;
	protected getService(): string;
	protected form: ProductForm;
}
export declare class ProductGrid<P = {}> extends EntityGrid<ProductRow, P> {
	protected getColumnsKey(): string;
	protected getDialogType(): any;
	protected getRowDefinition(): typeof ProductRow;
	protected getService(): string;
	private pendingChanges;
	constructor(props: WidgetProps<P>);
	protected getButtons(): import("@serenity-is/corelib").ToolButton[];
	protected onViewProcessData(response: any): import("@serenity-is/corelib").ListResponse<ProductRow>;
	/**
	 * It would be nice if we could use autonumeric, Serenity editors etc. here, to control input validation,
	 * but it's not supported by SlickGrid as we are only allowed to return a string, and should attach
	 * no event handlers to rendered cell contents
	 */
	private numericInputFormatter;
	private stringInputFormatter;
	/**
	 * Sorry but you cannot use LookupEditor, e.g. Select2 here, only possible is a SELECT element
	 */
	private selectFormatter;
	private getEffectiveValue;
	protected getColumns(): Column<ProductRow>[];
	private inputsChange;
	private setSaveButtonState;
	private saveClick;
	protected getQuickFilters(): import("@serenity-is/corelib").QuickFilter<import("@serenity-is/corelib").Widget<any>, any>[];
}
export declare class RegionDialog<P = {}> extends EntityDialog<RegionRow, P> {
	protected getFormKey(): string;
	protected getRowDefinition(): typeof RegionRow;
	protected getService(): string;
	protected form: RegionForm;
}
export declare class RegionGrid<P = {}> extends EntityGrid<RegionRow, P> {
	protected getColumnsKey(): string;
	protected getDialogType(): any;
	protected getRowDefinition(): typeof RegionRow;
	protected getService(): string;
}
export declare class ShipperDialog<P = {}> extends EntityDialog<ShipperRow, P> {
	protected getFormKey(): string;
	protected getRowDefinition(): typeof ShipperRow;
	protected getService(): string;
	protected form: ShipperForm;
}
export declare class ShipperFormatter implements Formatter {
	format(ctx: FormatterContext): FormatterResult;
}
export declare class ShipperGrid<P = {}> extends EntityGrid<ShipperRow, P> {
	protected getColumnsKey(): string;
	protected getDialogType(): any;
	protected getRowDefinition(): typeof ShipperRow;
	protected getService(): string;
}
export declare class SupplierDialog<P = {}> extends EntityDialog<SupplierRow, P> {
	protected getFormKey(): string;
	protected getRowDefinition(): typeof SupplierRow;
	protected getService(): string;
	protected form: SupplierForm;
}
export declare class SupplierGrid<P = {}> extends EntityGrid<SupplierRow, P> {
	protected getColumnsKey(): string;
	protected getDialogType(): any;
	protected getRowDefinition(): typeof SupplierRow;
	protected getService(): string;
}
export declare class TerritoryDialog<P = {}> extends EntityDialog<TerritoryRow, P> {
	protected getFormKey(): string;
	protected getRowDefinition(): typeof TerritoryRow;
	protected getService(): string;
	protected form: TerritoryForm;
}
export declare class TerritoryGrid<P = {}> extends EntityGrid<TerritoryRow, P> {
	protected getColumnsKey(): string;
	protected getDialogType(): any;
	protected getRowDefinition(): typeof TerritoryRow;
	protected getService(): string;
}

export {};
