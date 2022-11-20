using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using MockFileData = System.IO.Abstractions.TestingHelpers.MockFileData;

namespace Serenity.Tests.Web;

public partial class MultipleFileUploadBehaviorTests
{
    [Fact]
    public void ActivateFor_ReturnsFalse_WhenRowIsNull()
    {
        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create());
        var result = sut.ActivateFor(null);

        Assert.False(result);
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenTargetDoesntHave_MultipleImageUploadEditorAttribute()
    {
        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = MultipleTestRow.Fields.Id
        };
        var result = sut.ActivateFor(new MultipleTestRow());

        Assert.False(result);
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenTargetImageUploadEditorAttribute_DisableDefaultBehaviour_IsTrue()
    {
        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = MultipleTestRow.Fields.ImageUploadEditorDisableDefaultBehavior
        };
        var result = sut.ActivateFor(new MultipleTestRow());

        Assert.False(result);
    }
    

    [Fact]
    public void ActivateFor_ThrowsArgumentException_WhenTargetType_IsNotStringField()
    {
        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = MultipleTestIIdRow.Fields.IntegerFieldImageUploadEditor
        };

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new MultipleTestIIdRow()));
        Assert.Contains("string", Normalize(exception.Message));
    }

    [Fact]
    public void ActivateFor_ThrowsArgumentException_WhenTargetType_IsNotStringFieldAndPropertyNameIsNull()
    {
        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = MultipleTestIIdRow.Fields.IntegerFieldImageUploadEditor
        };

        sut.Target.PropertyName = null;

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new MultipleTestIIdRow()));
        Assert.Contains("string", Normalize(exception.Message));
    }


    [Fact]
    public void ActivateFor_ThrowsArgumentException_WhenRow_DoesNotInherits_IIdRow()
    {
        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = MultipleTestRow.Fields.StringFieldImageUploadEditor
        };

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new MultipleTestRow()));
        Assert.Contains("iidrow", Normalize(exception.Message));
    }

    [Fact]
    public void ActivateFor_ThrowsArgumentException_WhenRow_DoesNotInherits_IIdRowAndPropertyNameIsNull()
    {
        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = MultipleTestRow.Fields.StringFieldImageUploadEditor
        };

        sut.Target.PropertyName = null;

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new MultipleTestRow()));
        Assert.Contains("iidrow", Normalize(exception.Message));
    }

    [Fact]
    public void ActivateFor_ReturnsTrue_WhenTargetImageUploadEditorAttribute_EditorTypeIs_ImageUpload()
    {
        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = MultipleTestIIdRow.Fields.ImageUploadEditorCorrectEditorType
        };
        var result = sut.ActivateFor(new MultipleTestIIdRow());

        Assert.True(result);
    }
    
    [Fact]
    public void ActivateFor_ReturnsTrue_WhenTargetImageUploadEditorAttribute_OriginalName_IsStringField()
    {
        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = MultipleTestIIdRow.Fields.ImageUploadEditorOriginalName
        };
        var result = sut.ActivateFor(new MultipleTestIIdRow());

        Assert.True(result);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("[{\"FileName\":\"\",\"OriginalName\":\"same.jpg\"}]")]
    public void OnBeforeSave_InvalidJson(string newFile)
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = newFile
        };

        sut.ActivateFor(row);
        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new MultipleTestIIdRow
            {
                StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]"
            },
            Row = row,
            UnitOfWork = uow
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => sut.OnBeforeSave(requestHandler));
    }

    [Fact]
    public void OnBeforeSave_ClearField()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = null
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new MultipleTestIIdRow
            {
                StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]"
            },
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Null(row.StringFieldImageUploadEditor);
    }

    [Fact]
    public void OnBeforeSave_DoesNothing_WhenFieldIsNotAssigned()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new MultipleTestIIdRow
            {
                StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]"
            },
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Empty(uow.OnCommitInvocationList);
    }

    [Fact]
    public void OnAfterSave_DoesNothing_WhenFieldIsNotAssigned()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = true,
            IsUpdate = false,
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        sut.OnAfterSave(requestHandler);
        uow.Commit();

        Assert.Empty(uow.OnCommitInvocationList);
    }

    [Fact]
    public void OnAfterSave_DoesNothing_WhenFieldIsEmpty()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = null
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = true,
            IsUpdate = false,
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        sut.OnAfterSave(requestHandler);
        uow.Commit();

        Assert.Empty(uow.OnCommitInvocationList);
    }

    [Fact]
    public void OnAfterSave_ClearField()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = null
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new MultipleTestIIdRow
            {
                StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]"
            },
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        sut.OnAfterSave(requestHandler);
        uow.Commit();

        Assert.Null(row.StringFieldImageUploadEditor);
    }

    [Fact]
    public void OnBeforeSave_Update()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = "[{\"FileName\":\"temporary/new.jpg\",\"OriginalName\":\"new.jpg\"}]"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new MultipleTestIIdRow
            {
                StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]"
            },
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.NotNull(row.StringFieldImageUploadEditor);
        Assert.Contains("\"OriginalName\":\"new.jpg\"", row.StringFieldImageUploadEditor);
    }

    [Fact]
    public void OnBeforeSave_ChangesOnlyChangedFiles()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"},{\"FileName\":\"temporary/new.jpg\",\"OriginalName\":\"new.jpg\"}]"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new MultipleTestIIdRow
            {
                StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]"
            },
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);


        Assert.Contains("{\"Filename\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}", row.StringFieldImageUploadEditor);
        Assert.Contains("\"OriginalName\":\"new.jpg\"", row.StringFieldImageUploadEditor);
    }

    [Fact]
    public void OnBeforeSave_NewImage_NotInTemporaryFolder()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = "[{\"FileName\":\"upload/new.jpg\",\"OriginalName\":\"new.jpg\"}]"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new MultipleTestIIdRow
            {
                StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]"
            },
            Row = row,
            UnitOfWork = uow
        };


        var exp = Assert.Throws<InvalidOperationException>(() => sut.OnBeforeSave(requestHandler));
        Assert.Contains("temporary", exp.Message);
    }

    [Fact]
    public void OnBeforeSave_DoesNothing_When_FilesAreSame()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new MultipleTestIIdRow
            {
                StringFieldImageUploadEditor = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]"
            },
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);

        uow.Commit();

        Assert.Equal("[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}]", row.StringFieldImageUploadEditor);
    }

    [Fact]
    public void OnPrepareQuery_AddsReplaceFieldIntoQuery()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.ImageUploadEditorReplaceField
        };

        var row = new MultipleTestIIdRow
        {
            ImageUploadEditorReplaceField = "[{\"FileName\":\"upload/same.jpg\",\"OriginalName\":\"same.jpg\"}, {\"FileName\":\"temporary/new.jpg\",\"OriginalName\":\"new.jpg\"}]"
        };

        var query = new SqlQuery();

        sut.ActivateFor(row);

        sut.OnPrepareQuery(default(ISaveRequestHandler), query);

        Assert.Contains("(SELECT 'TEST') AS [StringFieldExpression]", query.DebugText);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnAfterSave_CopiesTemporaryFiles_AndDeletesOldFiles_WhenHandlerIsCreate(bool isUpdate)
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));
        mockFileSystem.AddFile("temporary/new2.jpg", new MockFileData(CreateImage(1000, 1000)));

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = "[{\"FileName\":\"temporary/new.jpg\",\"OriginalName\":\"new.jpg\"}, {\"FileName\":\"temporary/new2.jpg\",\"OriginalName\":\"new2.jpg\"}]"
        };

        sut.ActivateFor(row);

        var dbConnection = new MockDbConnection();
        dbConnection.OnExecuteReader(command => new MockDbDataReader(command.CommandText, null));

        var uow = new MockUnitOfWork(dbConnection);
        var requestHandler = new MockSaveHandler<MultipleTestIIdRow>
        {
            IsCreate = !isUpdate,
            IsUpdate = isUpdate,
            Old = isUpdate ? new MultipleTestIIdRow() : null,
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        sut.OnAfterSave(requestHandler);
        uow.Commit();

        var newFile = mockFileSystem.AllFiles.Select(x => mockFileSystem.Path.GetFileName(x)).ToList();
        var rowFileName = JSON.Parse<UploadedFile[]>(row.StringFieldImageUploadEditor).Select(x => mockFileSystem.Path.GetFileName(x.Filename));

        if (!isUpdate)
        {
            Assert.NotNull(rowFileName);
            Assert.Equal(rowFileName, newFile);
        }
        else
            Assert.Equal(rowFileName, newFile);
    }


    [Fact]
    public void OnAfterDelete_DeletesFiles_OnCommit()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddFile("upload/new.jpg", "");

        var row = new MultipleTestIIdRow
        {
            StringFieldImageUploadEditor = "[{\"FileName\":\"upload/new.jpg\",\"OriginalName\":\"new.jpg\"}]"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockDeleteHandler<MultipleTestIIdRow>
        {
            Row = row,
            UnitOfWork = uow
        };

        Assert.NotEmpty(mockFileSystem.AllFiles);

        sut.OnAfterDelete(requestHandler);
        uow.Commit();

        Assert.Empty(mockFileSystem.AllFiles);
    }

    [Fact]
    public void OnAfterDelete_DoesntDeletesFilesOnCommit_WhenRowImplementsIIsActiveDeletedRow()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddFile("upload/new.jpg", "");

        var row = new MultipleTestIIsActiveDeletedRowRow
        {
            StringFieldImageUploadEditor = "[{\"FileName\":\"upload/new.jpg\",\"OriginalName\":\"new.jpg\"}]"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockDeleteHandler<MultipleTestIIdRow>
        {
            Row = row,
            UnitOfWork = uow
        };

        var file = Assert.Single(mockFileSystem.AllFiles);

        sut.OnAfterDelete(requestHandler);
        uow.Commit();

        Assert.Equal(file, Assert.Single(mockFileSystem.AllFiles));
    }

    [Fact]
    public void OnAfterDelete_DoesntDeletesFilesOnCommit_WhenRowImplementsIIsDeletedRow()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIIsDeletedRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddFile("upload/new.jpg", "");

        var row = new MultipleTestIIsDeletedRow
        {
            StringFieldImageUploadEditor = "[{\"FileName\":\"upload/new.jpg\",\"OriginalName\":\"new.jpg\"}]"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockDeleteHandler<MultipleTestIIdRow>
        {
            Row = row,
            UnitOfWork = uow
        };

        var file = Assert.Single(mockFileSystem.AllFiles);

        sut.OnAfterDelete(requestHandler);
        uow.Commit();

        Assert.Equal(file, Assert.Single(mockFileSystem.AllFiles));
    }

    [Fact]
    public void OnAfterDelete_DoesntDeletesFilesOnCommit_WhenRowImplementsIDeleteLogRow()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new MultipleFileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = MultipleTestIDeleteLogRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddFile("upload/new.jpg", "");

        var row = new MultipleTestIDeleteLogRow
        {
            StringFieldImageUploadEditor = "[{\"FileName\":\"upload/new.jpg\",\"OriginalName\":\"new.jpg\"}]"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockDeleteHandler<MultipleTestIIdRow>
        {
            Row = row,
            UnitOfWork = uow
        };

        var file = Assert.Single(mockFileSystem.AllFiles);

        sut.OnAfterDelete(requestHandler);
        uow.Commit();

        Assert.Equal(file, Assert.Single(mockFileSystem.AllFiles));
    }

    private static byte[] CreateImage(int width, int height, IImageFormat format = null, Configuration configuration = null, Rgba32? color = null)
    {
        using var image = new Image<Rgba32>(configuration ?? Configuration.Default, width, height, color ?? new Rgba32(255, 255, 255));
        using var stream = new System.IO.MemoryStream();

        image.Save(stream, format ?? PngFormat.Instance);

        return stream.ToArray();
    }

    private static string Normalize(string str)
    {
        return str.ToLowerInvariant().ReplaceLineEndings().Replace(Environment.NewLine, string.Empty);
    }
}