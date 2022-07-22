using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Serenity.Tests.Web;

public partial class ImageUploadBehaviorTests
{
    private MockTextLocalizer localizer { get; } = new();

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenRowIsNull()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer);
        var result = sut.ActivateFor(null);

        Assert.False(result);
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenTargetDoesntHave_ImageUploadEditorAttribute()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestRow.Fields.Id
        };
        var result = sut.ActivateFor(new TestRow());

        Assert.False(result);
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenTargetImageUploadEditorAttribute_DisableDefaultBehaviour_IsTrue()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestRow.Fields.ImageUploadEditorDisableDefaultBehavior
        };
        var result = sut.ActivateFor(new TestRow());

        Assert.False(result);
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenTargetImageUploadEditorAttribute_EditorType_IsNotImageUpload()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestRow.Fields.ImageUploadEditorDifferentEditorType
        };
        var result = sut.ActivateFor(new TestRow());

        Assert.False(result);
    }

    [Fact]
    public void ActivateFor_ThrowsArgumentException_WhenTargetType_IsNotStringField()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestIIdRow.Fields.IntegerFieldImageUploadEditor
        };

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new TestIIdRow()));
        Assert.Contains("string", Normalize(exception.Message));
    }

    [Fact]
    public void ActivateFor_ThrowsArgumentException_WhenRow_DoesNotInherits_IIdRow()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestRow.Fields.StringFieldImageUploadEditor
        };

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new TestRow()));
        Assert.Contains("iidrow", Normalize(exception.Message));
    }

    [Fact]
    public void ActivateFor_ReturnsTrue_WhenTargetImageUploadEditorAttribute_EditorTypeIs_ImageUpload()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorCorrectEditorType
        };
        var result = sut.ActivateFor(new TestIIdRow());

        Assert.True(result);
    }

    [Fact]
    public void ActivateFor_Throws_WhenTargetImageUploadEditorAttribute_OriginalName_IsNotStringField()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalNameIntegerField
        };

        Assert.Throws<InvalidCastException>(() => sut.ActivateFor(new TestIIdRow()));
    }

    [Fact]
    public void ActivateFor_Throws_WhenTargetImageUploadEditorAttribute_OriginalName_IsNull()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalNameNoField
        };

        Assert.Throws<ArgumentException>(() => sut.ActivateFor(new TestIIdRow()));
    }

    [Fact]
    public void ActivateFor_ReturnsTrue_WhenTargetImageUploadEditorAttribute_OriginalName_IsStringField()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalName
        };
        var result = sut.ActivateFor(new TestIIdRow());

        Assert.True(result);
    }

    [Fact]
    public void OnPrepareQuery_ShouldAdd_NonTableReplaceFields_ToSelectSqlQuery()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorReplaceField
        };

        var query = new SqlQuery()
            .From("table")
            .Select("field");

        sut.ActivateFor(new TestIIdRow
        {
            ImageUploadEditorReplaceField = "/test.jpg"
        });

        sut.OnPrepareQuery(default(ISaveRequestHandler), query);

        Assert.Equal("select field,(select 'test') as [stringfieldexpression] from table", Normalize(query.DebugText));
    }

    // [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [Theory]
    public void OnPrepareQuery_ShouldNotAdd_NonTableReplaceFields_ToSqlQuery_IfFieldIsAlreadySelected(int selectIndex)
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorReplaceField
        };

        var query = new SqlQuery()
            .From("table");

        var previousSelectTexts = new List<string>();

        for (var i = 0; i < selectIndex; i++)
        {
            var currentSelectField = "field" + i;
            query = query.Select(currentSelectField);
            previousSelectTexts.Add(currentSelectField);
        }

        _ = new SqlQuery.Column(query, "(SELECT 'Test')", "StringFieldExpression", TestIIdRow.Fields.StringFieldExpression);
        query = query.Select("field");

        sut.ActivateFor(new TestIIdRow
        {
            ImageUploadEditorReplaceField = "/test.jpg"
        });

        sut.OnPrepareQuery(default(ISaveRequestHandler), query);

        var joinedSelects = string.Join(",", previousSelectTexts);
        if (previousSelectTexts.Count > 0)
            joinedSelects += ",";

        Assert.Equal("select " + joinedSelects + "(select 'test') as [stringfieldexpression],field from table", Normalize(query.DebugText));
    }

    [Fact]
    public void ParseReplaceFields_ThrowsArgumentException_WhenPipeOperators_AreNextToEachOther()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorInvalidReplaceField
        };

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new TestIIdRow()));
        Assert.Contains("invalid format string", Normalize(exception.Message));
    }

    [Fact]
    public void ParseReplaceFields_ThrowsArgumentException_WhenReplaceField_DoesntExist()
    {
        var sut = new ImageUploadBehavior(MockUploadStorage.Create(), localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorReplaceFieldNoField
        };

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new TestIIdRow()));
        Assert.Contains("no such field is found", Normalize(exception.Message));
    }
    
    [Fact]
    public void OnBeforeSave_DeletesOldFiles_OnCommit()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddDirectory("upload");
        mockFileSystem.AddFile("upload/old.jpg", new MockFileData(""));
        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));
        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = "temporary/new.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new TestIIdRow
            {
                StringFieldImageUploadEditor = "upload/old.jpg"
            },
            Row = row,
            UnitOfWork = uow
        };

        Assert.NotEmpty(mockFileSystem.AllFiles);
        Assert.Collection(mockFileSystem.AllFiles.Select(mockFileSystem.Path.GetFileName),
            x1 => Assert.Equal("old.jpg", x1),
            x2 => Assert.Equal("new.jpg", x2));

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Single(mockFileSystem.AllFiles); // should be a random file name
    }

    [Fact]
    public void OnBeforeSave_DoesntDeleteOldFiles_OnCommit_WhenFileNamesAreTrimmedEqual()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddDirectory("upload");
        mockFileSystem.AddFile("upload/new.jpg", new MockFileData(CreateImage(1000, 1000)));
        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = "upload/new.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new TestIIdRow
            {
                StringFieldImageUploadEditor = "upload/new.jpg"
            },
            Row = row,
            UnitOfWork = uow
        };

        Assert.NotEmpty(mockFileSystem.AllFiles);
        Assert.Collection(mockFileSystem.AllFiles.Select(mockFileSystem.Path.GetFileName),
            x1 => Assert.Equal("new.jpg", x1));

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Collection(mockFileSystem.AllFiles.Select(mockFileSystem.Path.GetFileName),
            x1 => Assert.Equal("new.jpg", x1));
    }

    [Fact]
    public void OnBeforeSave_SetsRowsEditorField_ToNull_OnCommit_WhenNewFileName_IsNull()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddDirectory("upload");
        mockFileSystem.AddFile("upload/old.jpg", new MockFileData(CreateImage(1000, 1000)));
        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = null
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new TestIIdRow
            {
                StringFieldImageUploadEditor = "upload/old.jpg"
            },
            Row = row,
            UnitOfWork = uow
        };

        Assert.NotEmpty(mockFileSystem.AllFiles);
        Assert.Collection(mockFileSystem.AllFiles.Select(mockFileSystem.Path.GetFileName),
            x1 => Assert.Equal("old.jpg", x1));

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Null(row.StringFieldImageUploadEditor);
        Assert.Empty(mockFileSystem.AllFiles);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void OnBeforeSave_DoesntDoAnything_OnCommit_WhenNewFileName_IsNullAnd_OldFileName_IsTrimmedEmpty(string oldFileName)
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = null
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new TestIIdRow
            {
                StringFieldImageUploadEditor = oldFileName
            },
            Row = row,
            UnitOfWork = uow
        };

        Assert.Empty(mockFileSystem.AllFiles);

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Empty(mockFileSystem.AllFiles);
    }

    [Fact]
    public void OnBeforeSave_ThrowsIf_NewFileNameDoesntStartWith_TemporaryFolder()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = "not-a-temporary-file.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new TestIIdRow
            {
                StringFieldImageUploadEditor = null
            },
            Row = row,
            UnitOfWork = uow
        };

        Assert.Throws<InvalidOperationException>(() => sut.OnBeforeSave(requestHandler));
    }

    [Fact]
    public void OnBeforeSave_SetsOriginalFileNameField_ToNull_WhenEditorField_IsNull()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalName
        };

        mockFileSystem.AddDirectory("upload");
        var row = new TestIIdRow
        {
            ImageUploadEditorOriginalName = null,
            Name = "old.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new TestIIdRow
            {
                ImageUploadEditorOriginalName = "upload/old.jpg"
            },
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Null(row.Name);
    }

    [Fact]
    public void OnBeforeSave_SetsOriginalName_ToNull_OnCommit_WhenFileDoesntHaveMeta()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddDirectory("temporary");
        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));
        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = "temporary/new.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = true,
            IsUpdate = false,
            Old = null,
            Row = row,
            UnitOfWork = uow
        };

        Assert.NotEmpty(mockFileSystem.AllFiles);

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Null(row.Name);
    }

    [Fact]
    public void OnBeforeSave_SetsOriginalName_ToOriginalName_OnCommit_WhenFileDoesHaveMeta()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalName
        };

        mockFileSystem.AddDirectory("temporary");
        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));
        mockUploadStorage.SetFileMetadata("temporary/new.jpg", new Dictionary<string, string>()
        {
            [FileMetadataKeys.OriginalName] = "originalFileName.jpg"
        }, true);

        var row = new TestIIdRow
        {
            ImageUploadEditorOriginalName = "temporary/new.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = true,
            IsUpdate = false,
            Old = null,
            Row = row,
            UnitOfWork = uow
        };

        Assert.NotEmpty(mockFileSystem.AllFiles);

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Equal("originalFileName.jpg", row.Name);
    }

    [Fact]
    public void OnBeforeSave_SavesFileToHistory_IfAttributeHasCopyToHistoryTrue()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorCopyToHistory
        };

        mockFileSystem.AddDirectory("upload");
        mockFileSystem.AddFile("upload/new.jpg", new MockFileData(CreateImage(1000, 1000)));

        var row = new TestIIdRow
        {
            ImageUploadEditorCopyToHistory = null
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = false,
            IsUpdate = true,
            Old = new TestIIdRow
            {
                ImageUploadEditorCopyToHistory = "upload/new.jpg"
            },
            Row = row,
            UnitOfWork = uow
        };

        var file = Assert.Single(mockFileSystem.AllFiles);

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        var archivedFile = Assert.Single(mockFileSystem.AllFiles);
        Assert.DoesNotContain("history", file);
        Assert.Contains("history", archivedFile);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnBeforeSave_SetsNewFileNameToField_WhenItsUpdate(bool isUpdate)
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddDirectory("temporary");
        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));

        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = "temporary/new.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = !isUpdate,
            IsUpdate = isUpdate,
            Old = isUpdate ? new TestIIdRow() : null,
            Row = row,
            UnitOfWork = uow
        };

        var fileName = Path.GetFileName(Assert.Single(mockFileSystem.AllFiles));

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        var newFile = Path.GetFileName(Assert.Single(mockFileSystem.AllFiles));
        var rowFileName = Path.GetFileName(row.StringFieldImageUploadEditor);

        if (!isUpdate)
        {
            Assert.Equal(fileName, newFile);
            Assert.NotNull(rowFileName);
        }
        else
            Assert.Equal(rowFileName, newFile);
    }

    [Fact]
    public void OnAfterDelete_DeletesFiles_OnCommit()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddDirectory("upload");
        mockFileSystem.AddFile("upload/new.jpg", "");

        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = "upload/new.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockDeleteHandler<TestIIdRow>
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

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddDirectory("upload");
        mockFileSystem.AddFile("upload/new.jpg", "");

        var row = new TestIIsActiveDeletedRowRow
        {
            StringFieldImageUploadEditor = "upload/new.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockDeleteHandler<TestIIdRow>
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

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddDirectory("upload");
        mockFileSystem.AddFile("upload/new.jpg", "");

        var row = new TestIIsDeletedRow
        {
            StringFieldImageUploadEditor = "upload/new.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockDeleteHandler<TestIIdRow>
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

        var sut = new ImageUploadBehavior(mockUploadStorage, localizer)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddDirectory("upload");
        mockFileSystem.AddFile("upload/new.jpg", "");

        var row = new TestIDeleteLogRow
        {
            StringFieldImageUploadEditor = "upload/new.jpg"
        };

        sut.ActivateFor(row);

        var uow = new MockUnitOfWork();
        var requestHandler = new MockDeleteHandler<TestIIdRow>
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
        using var stream = new MemoryStream();

        image.Save(stream, format ?? PngFormat.Instance);

        return stream.ToArray();
    }

    private static string Normalize(string str)
    {
        return str.ToLowerInvariant().ReplaceLineEndings().Replace(Environment.NewLine, string.Empty);
    }
}