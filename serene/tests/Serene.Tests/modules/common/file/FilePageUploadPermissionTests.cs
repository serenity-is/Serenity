using Serene.Common.Pages;
using System.IO;
using System.Text;

namespace Serene.Common;

public class FilePageUploadPermissionTests
{
    private class RecordingUploadProcessor : IUploadProcessor
    {
        public int Calls { get; private set; }

        public ProcessedUploadInfo Process(Stream fileContent, string filename, IUploadOptions options)
        {
            Calls++;
            return new ProcessedUploadInfo { TemporaryFile = "temporary/test.txt", FileSize = 4 };
        }
    }

    private static (FilePage page, RecordingUploadProcessor processor) CreatePage(Func<string, bool> hasPermission,
        string? uploadPermission = SpecialPermissionKeys.LoggedIn)
    {
        var processor = new RecordingUploadProcessor();
        var settings = new UploadSettings { TemporaryUploadPermission = uploadPermission };
        var page = new FilePage(new MockUploadStorage(), processor, new MockPermissions(hasPermission),
            Options.Create(settings), new MockTextLocalizer());

        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "multipart/form-data; boundary=x";
        var content = Encoding.UTF8.GetBytes("test");
        context.Request.Form = new FormCollection([], new FormFileCollection
        {
            new FormFile(new MemoryStream(content), 0, content.Length, "file", "test.txt")
        });
        page.ControllerContext = new ControllerContext { HttpContext = context };
        return (page, processor);
    }

    [Fact]
    public void TemporaryUpload_Is_Denied_Without_Permission()
    {
        var checkedPermissions = new List<string>();
        var (page, processor) = CreatePage(p => { checkedPermissions.Add(p); return false; });

        var result = page.TemporaryUpload();

        Assert.Equal(0, processor.Calls);
        Assert.Equal([SpecialPermissionKeys.LoggedIn], checkedPermissions);
        // Result<UploadResponse>, where UploadResponse is a private class in FilePage
        var data = result.GetType().GetProperty(nameof(Result<ServiceResponse>.Data))?.GetValue(result);
        var error = Assert.IsAssignableFrom<ServiceResponse>(data).Error;
        Assert.Equal("AccessDenied", error?.Code);
    }

    [Fact]
    public void TemporaryUpload_Is_Processed_With_Permission()
    {
        var (page, processor) = CreatePage(_ => true);

        page.TemporaryUpload();

        Assert.Equal(1, processor.Calls);
    }

    [Fact]
    public void TemporaryUploadCK_Is_Denied_Without_Permission()
    {
        var (page, processor) = CreatePage(_ => false);

        var result = Assert.IsType<JsonResult>(page.TemporaryUploadCK());

        Assert.Equal(0, processor.Calls);
        Assert.Contains("uploaded = 0", result.Value?.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void Uses_Configured_Permission()
    {
        var checkedPermissions = new List<string>();
        var (page, processor) = CreatePage(p => { checkedPermissions.Add(p); return p == "Custom:Upload"; },
            uploadPermission: "Custom:Upload");

        page.TemporaryUpload();

        Assert.Equal(1, processor.Calls);
        Assert.Equal(["Custom:Upload"], checkedPermissions);
    }
}
