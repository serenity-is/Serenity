namespace Serenity.CodeGenerator;

public partial class EntityCodeGeneratorTests
{
    private const string DefaultSergenJson = """
        {
          "Extends": "defaults@9.0.0",
          "RootNamespace": "TestNamespace"
        }
        """;

    private static readonly Dictionary<string, string> DefaultGoldenFiles = new()
    {
        ["Modules/TestModule/Customer/CustomerRow.cs"] = /*lang=c#*/ """
            namespace TestNamespace.TestModule;
            
            [ConnectionKey("TestConnection"), Module("TestModule"), TableName("[test].[Customer]")]
            [DisplayName("Customer"), InstanceName("Customer")]
            [ReadPermission("TestPermission")]
            [ModifyPermission("TestPermission")]
            [ServiceLookupPermission("TestPermission")]
            public sealed class CustomerRow : Row<CustomerRow.RowFields>, IIdRow, INameRow
            {
                const string jCity = nameof(jCity);
            
                [DisplayName("Customer Id"), Identity, IdProperty]
                public int? CustomerId { get => fields.CustomerId[this]; set => fields.CustomerId[this] = value; }
            
                [DisplayName("Customer Name"), Size(50), NotNull, QuickSearch, NameProperty]
                public string CustomerName { get => fields.CustomerName[this]; set => fields.CustomerName[this] = value; }
            
                [DisplayName("City"), ForeignKey("[test].[City]", "CityId"), LeftJoin(jCity), TextualField(nameof(CityName))]
                public int? CityId { get => fields.CityId[this]; set => fields.CityId[this] = value; }
            
                [DisplayName("City City Name"), Expression($"{jCity}.[CityName]")]
                public string CityName { get => fields.CityName[this]; set => fields.CityName[this] = value; }
            
                public class RowFields : RowFieldsBase
                {
                    public Int32Field CustomerId;
                    public StringField CustomerName;
                    public Int32Field CityId;
            
                    public StringField CityName;
                }
            }
            """,
        ["Modules/TestModule/Customer/CustomerColumns.cs"] = /*lang=c#*/ """
            namespace TestNamespace.TestModule.Columns;
            
            [ColumnsScript("TestModule.Customer")]
            [BasedOnRow(typeof(CustomerRow), CheckNames = true)]
            public class CustomerColumns
            {
                [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
                public int? CustomerId { get; set; }
                [EditLink]
                public string CustomerName { get; set; }
                public string CityName { get; set; }
            }
            """,
        ["Modules/TestModule/Customer/CustomerForm.cs"] = /*lang=c#*/ """
            namespace TestNamespace.TestModule.Forms;
            
            [FormScript("TestModule.Customer")]
            [BasedOnRow(typeof(CustomerRow), CheckNames = true)]
            public class CustomerForm
            {
                public string CustomerName { get; set; }
                public int? CityId { get; set; }
            }
            """,
        ["Modules/TestModule/Customer/CustomerPage.cs"] = /*lang=c#*/ """
            namespace TestNamespace.TestModule.Pages;
            
            [PageAuthorize(typeof(CustomerRow))]
            public class CustomerPage : Controller
            {
                [Route("TestModule/Customer")]
                public ActionResult Index()
                {
                    return this.GridPage<CustomerRow>("@/TestModule/Customer/CustomerPage");
                }
            }
            """,
        ["Modules/TestModule/Customer/CustomerDialog.tsx"] = /*lang=typescript*/ """
            import { EntityDialog } from '@serenity-is/corelib';
            import { CustomerForm, CustomerRow, CustomerService } from '../../ServerTypes/TestModule';
            
            export class CustomerDialog extends EntityDialog<CustomerRow, any> {
                static override [Symbol.typeInfo] = this.registerClass("TestNamespace.TestModule.");
            
                protected override getFormKey() { return CustomerForm.formKey; }
                protected override getRowDefinition() { return CustomerRow; }
                protected override getService() { return CustomerService.baseUrl; }
            
                protected form = new CustomerForm(this);
            }
            """,
        ["Modules/TestModule/Customer/CustomerGrid.tsx"] = /*lang=typescript*/ """
            import { EntityGrid } from '@serenity-is/corelib';
            import { CustomerColumns, CustomerRow, CustomerService } from '../../ServerTypes/TestModule';
            import { CustomerDialog } from './CustomerDialog';
            
            export class CustomerGrid extends EntityGrid<CustomerRow> {
                static override [Symbol.typeInfo] = this.registerClass("TestNamespace.TestModule.");
            
                protected override getColumnsKey() { return CustomerColumns.columnsKey; }
                protected override getDialogType() { return CustomerDialog; }
                protected override getRowDefinition() { return CustomerRow; }
                protected override getService() { return CustomerService.baseUrl; }
            }
            """,
        ["Modules/TestModule/Customer/CustomerPage.tsx"] = /*lang=typescript*/ """
            import { gridPageInit } from '@serenity-is/corelib';
            import { CustomerGrid } from './CustomerGrid';
            
            export default () => gridPageInit(CustomerGrid);
            """,
        ["Modules/TestModule/Customer/RequestHandlers/CustomerDeleteHandler.cs"] = """
            using MyRow = TestNamespace.TestModule.CustomerRow;
            
            namespace TestNamespace.TestModule;
            
            public interface ICustomerDeleteHandler : IDeleteHandlerAsync<MyRow, DeleteRequest, DeleteResponse> { }
            
            public class CustomerDeleteHandler(IRequestContext context) :
                DeleteRequestHandlerAsync<MyRow, DeleteRequest, DeleteResponse>(context),
                ICustomerDeleteHandler
            {
            }
            """,
        ["Modules/TestModule/Customer/RequestHandlers/CustomerListHandler.cs"] = /*lang=c#*/ """
            using MyRow = TestNamespace.TestModule.CustomerRow;
            
            namespace TestNamespace.TestModule;
            
            public interface ICustomerListHandler : IListHandlerAsync<MyRow, ListRequest, ListResponse<MyRow>> { }
            
            public class CustomerListHandler(IRequestContext context) :
                ListRequestHandlerAsync<MyRow, ListRequest, ListResponse<MyRow>>(context),
                ICustomerListHandler
            {
            }
            """,
        ["Modules/TestModule/Customer/RequestHandlers/CustomerRetrieveHandler.cs"] = /*lang=c#*/ """
            using MyRow = TestNamespace.TestModule.CustomerRow;
            
            namespace TestNamespace.TestModule;
            
            public interface ICustomerRetrieveHandler : IRetrieveHandlerAsync<MyRow, RetrieveRequest, RetrieveResponse<MyRow>> { }
            
            public class CustomerRetrieveHandler(IRequestContext context) :
                RetrieveRequestHandlerAsync<MyRow, RetrieveRequest, RetrieveResponse<MyRow>>(context),
                ICustomerRetrieveHandler
            {
            }
            """,
        ["Modules/TestModule/Customer/RequestHandlers/CustomerSaveHandler.cs"] = /*lang=c#*/ """
            using MyRow = TestNamespace.TestModule.CustomerRow;
            
            namespace TestNamespace.TestModule;
            
            public interface ICustomerSaveHandler : ISaveHandlerAsync<MyRow, SaveRequest<MyRow>, SaveResponse> { }
            
            public class CustomerSaveHandler(IRequestContext context) :
                SaveRequestHandlerAsync<MyRow, SaveRequest<MyRow>, SaveResponse>(context),
                ICustomerSaveHandler
            {
            }
            """,
        ["Modules/TestModule/Customer/CustomerEndpoint.cs"] = /*lang=c#*/ """
            using Serenity.Reporting;
            using System.Data;
            using System.Globalization;
            using MyRow = TestNamespace.TestModule.CustomerRow;
            
            namespace TestNamespace.TestModule.Endpoints;
            
            [Route("Services/TestModule/Customer/[action]")]
            [ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
            public class CustomerEndpoint : ServiceEndpoint
            {
                [HttpPost, AuthorizeCreate(typeof(MyRow))]
                public Task<SaveResponse> Create(IUnitOfWork uow, SaveRequest<MyRow> request,
                    [FromServices] ICustomerSaveHandler handler, CancellationToken cancellationToken = default)
                {
                    return handler.CreateAsync(uow, request, cancellationToken);
                }
            
                [HttpPost, AuthorizeUpdate(typeof(MyRow))]
                public Task<SaveResponse> Update(IUnitOfWork uow, SaveRequest<MyRow> request,
                    [FromServices] ICustomerSaveHandler handler, CancellationToken cancellationToken = default)
                {
                    return handler.UpdateAsync(uow, request, cancellationToken);
                }
             
                [HttpPost, AuthorizeDelete(typeof(MyRow))]
                public Task<DeleteResponse> Delete(IUnitOfWork uow, DeleteRequest request,
                    [FromServices] ICustomerDeleteHandler handler, CancellationToken cancellationToken = default)
                {
                    return handler.DeleteAsync(uow, request, cancellationToken);
                }
            
                [HttpPost, AuthorizeRetrieve(typeof(MyRow))]
                public Task<RetrieveResponse<MyRow>> Retrieve(IDbConnection connection, RetrieveRequest request,
                    [FromServices] ICustomerRetrieveHandler handler, CancellationToken cancellationToken = default)
                {
                    return handler.RetrieveAsync(connection, request, cancellationToken);
                }
            
                [HttpPost, AuthorizeList(typeof(MyRow))]
                public Task<ListResponse<MyRow>> List(IDbConnection connection, ListRequest request,
                    [FromServices] ICustomerListHandler handler, CancellationToken cancellationToken = default)
                {
                    return handler.ListAsync(connection, request, cancellationToken);
                }
            
                [HttpPost, AuthorizeList(typeof(MyRow))]
                public async Task<FileContentResult> ListExcel(IDbConnection connection, ListRequest request,
                    [FromServices] ICustomerListHandler handler,
                    [FromServices] IExcelExporter exporter, CancellationToken cancellationToken = default)
                {
                    var data = (await List(connection, request, handler, cancellationToken)).Entities;
                    var bytes = exporter.Export(data, typeof(Columns.CustomerColumns), request.ExportColumns);
                    return ExcelContentResult.Create(bytes, "CustomerList_" +
                        DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx");
                }
            }
            """,
        ["Modules/ServerTypes/TestModule.ts"] = /*lang=typescript*/ """
            export * from "./TestModule/CustomerRow"
            export * from "./TestModule/CustomerService"
            export * from "./TestModule/CustomerForm"
            export * from "./TestModule/CustomerColumns"
            """,
        ["Modules/ServerTypes/TestModule/CustomerRow.ts"] = """
            import { fieldsProxy } from '@serenity-is/corelib';
            
            export interface CustomerRow {
                CustomerId?: number;
                CustomerName?: string;
                CityId?: number;
                CityName?: string;
            }
            
            export abstract class CustomerRow {
                static readonly idProperty = 'CustomerId';
                static readonly nameProperty = 'CustomerName';
                static readonly localTextPrefix = 'TestModule.Customer';
            
                static readonly deletePermission = 'TestPermission';
                static readonly insertPermission = 'TestPermission';
                static readonly readPermission = 'TestPermission';
                static readonly updatePermission = 'TestPermission';
            
                static readonly Fields = fieldsProxy<CustomerRow>();
            }
            """,
        ["Modules/ServerTypes/TestModule/CustomerColumns.ts"] = /*lang=typescript*/ """
            import { ColumnsBase, fieldsProxy } from '@serenity-is/corelib';
            import { Column } from '@serenity-is/sleekgrid';
            import { CustomerRow } from './CustomerRow';
            
            export interface CustomerColumns {
                CustomerId: Column<CustomerRow>;
                CustomerName: Column<CustomerRow>;
                CityId: Column<CustomerRow>;
            }
            
            export class CustomerColumns extends ColumnsBase<CustomerRow> {
                static readonly columnsKey = 'TestModule.Customer';
                static readonly Fields = fieldsProxy<CustomerColumns>();
            }
            """,
        ["Modules/ServerTypes/TestModule/CustomerForm.ts"] = /*lang=typescript*/ """
            import { PrefixedContext, initFormType, StringEditor, IntegerEditor } from '@serenity-is/corelib';
            
            export interface CustomerForm {
                CustomerName: StringEditor;
                CityId: IntegerEditor;
            }
            
            export class CustomerForm extends PrefixedContext {
                static readonly formKey = 'TestModule.Customer';
                private static init: boolean;
                
                constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
                    super(...args);
            
                    if (!CustomerForm.init)  {
                        CustomerForm.init = true;
            
                        initFormType(CustomerForm, [
                            'CustomerName', StringEditor,
                            'CityId', IntegerEditor,
                        ]);
                    }
                }
            }
            """,
        ["Modules/ServerTypes/TestModule/CustomerService.ts"] = /*lang=typescript*/ """
            import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from '@serenity-is/corelib';
            import { CustomerRow } from './CustomerRow';
            
            export namespace CustomerService {
                export const baseUrl = 'TestModule/Customer';
            
                export declare function Create(request: SaveRequest<CustomerRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
                export declare function Update(request: SaveRequest<CustomerRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
                export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
                export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<CustomerRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<CustomerRow>>;
                export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<CustomerRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<CustomerRow>>;
            
                export const Methods = {
                    Create: "TestModule/Customer/Create",
                    Update: "TestModule/Customer/Update",
                    Delete: "TestModule/Customer/Delete",
                    Retrieve: "TestModule/Customer/Retrieve",
                    List: "TestModule/Customer/List"
                } as const;
            
                [
                    'Create', 
                    'Update', 
                    'Delete', 
                    'Retrieve', 
                    'List'
                ].forEach(x => {
                    (<any>CustomerService)[x] = function (r, s, o) { 
                        return serviceRequest(baseUrl + '/' + x, r, s, o); 
                    };
                });
            }
            """,
    };

    private static readonly Dictionary<string, string> NullableGoldenOverrides = new()
    {
        ["Modules/TestModule/Customer/CustomerColumns.cs"] = """
            namespace TestNamespace.TestModule.Columns;

            [ColumnsScript("TestModule.Customer")]
            [BasedOnRow(typeof(CustomerRow), CheckNames = true)]
            public class CustomerColumns
            {
                [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
                public int? CustomerId { get; set; }
                [EditLink]
                public string? CustomerName { get; set; }
                public string CityName { get; set; }
            }
            """,
        ["Modules/TestModule/Customer/CustomerForm.cs"] = """
            namespace TestNamespace.TestModule.Forms;

            [FormScript("TestModule.Customer")]
            [BasedOnRow(typeof(CustomerRow), CheckNames = true)]
            public class CustomerForm
            {
                public string? CustomerName { get; set; }
                public int? CityId { get; set; }
            }
            """,
        ["Modules/TestModule/Customer/CustomerRow.cs"] = """
            namespace TestNamespace.TestModule;

            [ConnectionKey("TestConnection"), Module("TestModule"), TableName("[test].[Customer]")]
            [DisplayName("Customer"), InstanceName("Customer")]
            [ReadPermission("TestPermission")]
            [ModifyPermission("TestPermission")]
            [ServiceLookupPermission("TestPermission")]
            public sealed class CustomerRow : Row<CustomerRow.RowFields>, IIdRow, INameRow
            {
                const string jCity = nameof(jCity);

                [DisplayName("Customer Id"), Identity, IdProperty]
                public int? CustomerId { get => fields.CustomerId[this]; set => fields.CustomerId[this] = value; }

                [DisplayName("Customer Name"), Size(50), NotNull, QuickSearch, NameProperty]
                public string? CustomerName { get => fields.CustomerName[this]; set => fields.CustomerName[this] = value; }

                [DisplayName("City"), ForeignKey("[test].[City]", "CityId"), LeftJoin(jCity), TextualField(nameof(CityName))]
                public int? CityId { get => fields.CityId[this]; set => fields.CityId[this] = value; }

                [DisplayName("City City Name"), Expression($"{jCity}.[CityName]")]
                public string? CityName { get => fields.CityName[this]; set => fields.CityName[this] = value; }

                public class RowFields : RowFieldsBase
                {
                    public Int32Field CustomerId = null!;
                    public StringField CustomerName = null!;
                    public Int32Field CityId = null!;

                    public StringField CityName = null!;
                }
            }
            """,
    };

    [Fact]
    public void Run_With_Defaults9_And_Nullable_Generates_All_Files_Exactly()
    {
        var generator = CreateDefaults(out var fileSystem, out var writer, out _, DefaultSergenJson,
            nullableRefTypes: true);

        generator.Run();

        Assert.Equal(16, writer.Files.Count);

        foreach (var pair in DefaultGoldenFiles)
        {
            var path = "/app/" + pair.Key;
            Assert.True(fileSystem.FileExists(path), "Missing file: " + pair.Key);
            var expected = NullableGoldenOverrides.TryGetValue(pair.Key, out var overriden) ?
                overriden : pair.Value;
            var actual = fileSystem.ReadAllText(path);
            Assert.True(expected.ReplaceLineEndings() == actual.ReplaceLineEndings(),
                "Content mismatch in file: " + pair.Key);
        }
    }

    [Fact]
    public void Run_With_Defaults9_Generates_All_Files_Exactly()
    {
        var generator = CreateDefaults(out var fileSystem, out var writer, out _, DefaultSergenJson);

        generator.Run();

        Assert.Equal(16, writer.Files.Count);

        foreach (var pair in DefaultGoldenFiles)
        {
            var path = "/app/" + pair.Key;
            Assert.True(fileSystem.FileExists(path), "Missing file: " + pair.Key);
            var actual = fileSystem.ReadAllText(path);
            Assert.True(pair.Value.ReplaceLineEndings() == actual.ReplaceLineEndings(),
                "Content mismatch in file: " + pair.Key);
        }
    }
}
