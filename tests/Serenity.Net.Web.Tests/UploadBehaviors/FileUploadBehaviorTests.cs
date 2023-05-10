using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using MockFileData = System.IO.Abstractions.TestingHelpers.MockFileData;

namespace Serenity.Tests.Web;

public partial class FileUploadBehaviorTests
{
    [Fact]
    public void ActivateFor_ReturnsFalse_WhenRowIsNull()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create());
        var result = sut.ActivateFor(null);

        Assert.False(result);
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenTargetDoesntHave_ImageUploadEditorAttribute()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = TestRow.Fields.Id
        };
        var result = sut.ActivateFor(new TestRow());

        Assert.False(result);
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenTargetImageUploadEditorAttribute_DisableDefaultBehaviour_IsTrue()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = TestRow.Fields.ImageUploadEditorDisableDefaultBehavior
        };
        var result = sut.ActivateFor(new TestRow());

        Assert.False(result);
    }

    [Fact]
    public void ActivateFor_ThrowsArgumentException_WhenTargetType_IsNotStringField()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = TestIIdRow.Fields.IntegerFieldImageUploadEditor
        };

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new TestIIdRow()));
        Assert.Contains("string", Normalize(exception.Message));
    }

    [Fact]
    public void ActivateFor_ThrowsArgumentException_WhenRow_DoesNotInherits_IIdRow()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = TestRow.Fields.StringFieldImageUploadEditor
        };

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new TestRow()));
        Assert.Contains("iidrow", Normalize(exception.Message));
    }

    [Fact]
    public void ActivateFor_Throws_WhenTargetImageUploadEditorAttribute_OriginalName_IsNotStringField()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalNameIntegerField
        };

        Assert.Throws<InvalidCastException>(() => sut.ActivateFor(new TestIIdRow()));
    }

    [Fact]
    public void ActivateFor_Throws_WhenTargetImageUploadEditorAttribute_OriginalName_IsNull()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalNameNoField
        };

        Assert.Throws<ArgumentException>(() => sut.ActivateFor(new TestIIdRow()));
    }

    [Fact]
    public void ActivateFor_ReturnsTrue_WhenTargetImageUploadEditorAttribute_OriginalName_IsStringField()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalName
        };
        var result = sut.ActivateFor(new TestIIdRow());

        Assert.True(result);
    }

    [Fact]
    public void OnPrepareQuery_ShouldAdd_NonTableReplaceFields_ToSelectSqlQuery()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
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
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
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
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
        {
            Target = TestIIdRow.Fields.ImageUploadEditorInvalidReplaceField
        };

        var exception = Assert.Throws<ArgumentException>(() => sut.ActivateFor(new TestIIdRow()));
        Assert.Contains("invalid format string", Normalize(exception.Message));
    }

    [Fact]
    public void ParseReplaceFields_ThrowsArgumentException_WhenReplaceField_DoesntExist()
    {
        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), MockUploadStorage.Create())
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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

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
        Assert.Collection(mockFileSystem.AllFiles.Select(mockFileSystem.GetFileName),
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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

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
        Assert.Collection(mockFileSystem.AllFiles.Select(mockFileSystem.GetFileName),
            x1 => Assert.Equal("new.jpg", x1));

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        Assert.Collection(mockFileSystem.AllFiles.Select(mockFileSystem.GetFileName),
            x1 => Assert.Equal("new.jpg", x1));
    }

    [Fact]
    public void OnBeforeSave_SetsRowsEditorField_ToNull_OnCommit_WhenNewFileName_IsNull()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

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
        Assert.Collection(mockFileSystem.AllFiles.Select(mockFileSystem.GetFileName),
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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalName
        };

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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorOriginalName
        };

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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.ImageUploadEditorCopyToHistory
        };

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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

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

        var fileName = mockFileSystem.GetFileName(Assert.Single(mockFileSystem.AllFiles));

        sut.OnBeforeSave(requestHandler);
        uow.Commit();

        var newFile = mockFileSystem.GetFileName(Assert.Single(mockFileSystem.AllFiles));
        var rowFileName = mockFileSystem.GetFileName(row.StringFieldImageUploadEditor);

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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIsDeletedRow.Fields.StringFieldImageUploadEditor
        };

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

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIDeleteLogRow.Fields.StringFieldImageUploadEditor
        };

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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnAfterSave_CopiesTemporaryFiles_AndDeletesOldFiles_WhenHandlerIsCreate(bool isUpdate)
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));

        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = "temporary/new.jpg"
        };

        sut.ActivateFor(row);

        var dbConnection = new MockDbConnection();
        dbConnection.OnExecuteReader(command => new MockDbDataReader(command.CommandText, null));
        
        var uow = new MockUnitOfWork(dbConnection);
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = !isUpdate,
            IsUpdate = isUpdate,
            Old = isUpdate ? new TestIIdRow() : null,
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        sut.OnAfterSave(requestHandler);
        uow.Commit();

        var newFile = mockFileSystem.GetFileName(Assert.Single(mockFileSystem.AllFiles));
        var rowFileName = mockFileSystem.GetFileName(row.StringFieldImageUploadEditor);

        if (!isUpdate)
        {
            Assert.NotNull(rowFileName);
            Assert.Equal(rowFileName, newFile);
        }
        else
            Assert.Equal(rowFileName, newFile);
    }

    [Fact]
    public void OnAfterSave_UpdatesRowFileNameFromDatabase_WhenHandlerIsCreate()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));

        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = "temporary/new.jpg"
        };

        sut.ActivateFor(row);

        IDbCommand dbCommand = null;
        var dbConnection = new MockDbConnection();
        
        dbConnection.OnExecuteNonQuery(command =>
        {
            dbCommand = command;
            return 1;
        });
        
        var uow = new MockUnitOfWork(dbConnection);
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = true,
            IsUpdate = false,
            Old = null,
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        row.Id = 1234;
        sut.OnAfterSave(requestHandler);
        uow.Commit();

        var newFile = mockFileSystem.GetFileName(Assert.Single(mockFileSystem.AllFiles));
        
        Assert.NotNull(dbCommand);
        var updateStatement = (TSQL.Statements.TSQLUpdateStatement)Assert.Single(TSQL.TSQLStatementReader.ParseStatements(dbCommand.CommandText));
        
        Assert.Equal("dbo", updateStatement.Update.Tokens[1].Text);
        Assert.Equal(".", updateStatement.Update.Tokens[2].Text);
        Assert.Equal("[Test]", updateStatement.Update.Tokens[3].Text);
        
        Assert.Equal(nameof(TestIIdRow.StringFieldImageUploadEditor), updateStatement.Set.Tokens[1].Text);
        Assert.Equal("=", updateStatement.Set.Tokens[2].Text);
        Assert.Equal("@p1", updateStatement.Set.Tokens[3].Text);
        Assert.Equal(newFile, mockFileSystem.GetFileName(((IDbDataParameter)dbCommand.Parameters[0]!).Value as string));
        
        Assert.Equal("Id", updateStatement.Where.Tokens[2].Text);
        Assert.Equal("=", updateStatement.Where.Tokens[3].Text);
        Assert.Equal("@p2", updateStatement.Where.Tokens[4].Text);
        Assert.Equal(row.Id, ((IDbDataParameter)dbCommand.Parameters[1]!).Value);
    }
    
     [Fact]
    public void OnAfterSave_DoesntDoAnything_WhenHandlerIsCreate_AndFileNameIsNull()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = null
        };

        sut.ActivateFor(row);

        IDbCommand dbCommand = null;
        var dbConnection = new MockDbConnection();
        
        dbConnection.OnExecuteNonQuery(command =>
        {
            dbCommand = command;
            return 1;
        });
        
        var uow = new MockUnitOfWork(dbConnection);
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = true,
            IsUpdate = false,
            Old = null,
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        sut.OnAfterSave(requestHandler);
        uow.Commit();

        Assert.Null(dbCommand);
    }

    [Fact]
    public void ProcessReplaceFields_DirectlyReturns_WhenReplaceFieldsAreNull()
    {
        var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
        var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

        var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage)
        {
            Target = TestIIdRow.Fields.StringFieldImageUploadEditor
        };

        mockFileSystem.AddFile("temporary/new.jpg", new MockFileData(CreateImage(1000, 1000)));

        var row = new TestIIdRow
        {
            StringFieldImageUploadEditor = "temporary/new.jpg"
        };

        sut.ActivateFor(row);

        var dbConnection = new MockDbConnection();

        var uow = new MockUnitOfWork(dbConnection);
        var requestHandler = new MockSaveHandler<TestIIdRow>
        {
            IsCreate = true,
            IsUpdate = false,
            Old = null,
            Row = row,
            UnitOfWork = uow
        };

        sut.OnBeforeSave(requestHandler);
        row.Id = 1234;
        sut.OnAfterSave(requestHandler);
        uow.Commit();
    }

    [Fact]
    public void ProcessReplaceFields_FormatsZeroAs_RowIdentityValue()
    {
        var processResult = TestProcessReplaceFields("{0}", row: new TestIIdRow()
        {
            Id = 1111
        });
        
        Assert.Equal("1111.jpg", processResult);
    }
    
    [Theory]
    [InlineData(null, "_")]
    [InlineData(1000, "1")]
    [InlineData(2000, "2")]
    [InlineData(20000, "20")]
    [InlineData(200000, "200")]
    public void ProcessReplaceFields_FormatsOneAs_IdentityValueDividedByThousand(object id, string groupKey)
    {
        var processResult = TestProcessReplaceFields("{1}", row: new TestIIdRow()
        {
            Id = id as int?
        });
        
        Assert.Equal($"{groupKey}.jpg", processResult);
    }
    
    [Fact]
    public void ProcessReplaceFields_FormatsTwoAs_RandomString()
    {
        var processResult = TestProcessReplaceFields("{2}");
        
        Assert.NotNull(processResult);
        Assert.NotEmpty(processResult);
        Assert.EndsWith(".jpg", processResult);
        Assert.Equal(13 + ".jpg".Length, processResult.Length);
    }
    
    [Fact]
    public void ProcessReplaceFields_CanReplaceFieldFromTable()
    {
        var processResult = TestProcessReplaceFields("|Name|", row: new TestIIdRow()
        {
            Name = "test"
        });
        
        Assert.Equal("test.jpg", processResult);
    }
    
    [Fact]
    public void ProcessReplaceFields_UsesStringFormat_When_CurlyBracesContainsAColon()
    {
        var processResult = TestProcessReplaceFields("|Id:D5|", row: new TestIIdRow()
        {
            Id = 200
        });
        
        Assert.Equal("00200.jpg", processResult);
    }

    [Fact]
    public void ProcessReplaceFields_FormatsThreeAs_DateTimeNow()
    {
        var processResult = TestProcessReplaceFields("{3:yyyyMMddTHHmmssZzz}");
        
        Assert.Matches(@"^\d{8}T\d{6}Z[+-]\d{2}\.jpg$", processResult);
    }
    
    [Fact]
    public void ProcessReplaceFields_FormatsFourAs_OriginalFileName()
    {
        var processResult = TestProcessReplaceFields("{4}", row: new TestIIdRow()
        {
            StringFieldImageUploadEditor = "temporary/originalFileName.jpg"
        });
        
        Assert.Equal("originalFileName.jpg", processResult);
    }

    [Fact]
    public void ProcessReplaceFields_UsesDefaultFormat_WhenFilenameFormatIsNull()
    {
        var processResult = TestProcessReplaceFields(null, row: new TestIIdRow
        {
            Id = 200_000
        });
        
        Assert.Matches(@"^TestIId/00200/00200000_[\d\D]{13}.jpg$", processResult);
    }
    
    [Fact]
    public void ProcessReplaceFields_ReplacesTilde_WithDefaultFormatWithoutRowName()
    {
        var processResult = TestProcessReplaceFields("Foobar/~", row: new TestIIdRow
        {
            Id = 200_000
        });
        
        Assert.Matches(@"^Foobar/00200/00200000_[\d\D]{13}.jpg$", processResult);
    }
    
    [Fact]
    public void ProcessReplaceFields_ReplacesDotWithUnderscore_WhenReplaceFieldValueEndsWithADot()
    {
        var processResult = TestProcessReplaceFields("a-|Name|", row: new TestIIdRow
        {
            Name = "foo."
        });
        
        Assert.Equal("a-foo_.jpg", processResult);
    }

    [Fact]
    public void ProcessReplaceFields_SelectsNonTableFieldsFromDatabase()
    {
        var processResult = TestProcessReplaceFields("|StringFieldExpression|", foreignFieldQueryResults: new Dictionary<string, object>()
        {
            ["StringFieldExpression"] = "test-string-field-expression"
        });
        
        Assert.Equal("test-string-field-expression.jpg", processResult);
    }
    
    [Fact]
    public void ProcessReplaceFields_CanSelectMultipleNonTableFieldsFromDatabase()
    {
        var processResult = TestProcessReplaceFields("|StringFieldExpression|-|IntegerFieldExpression:D3|", foreignFieldQueryResults: new Dictionary<string, object>()
        {
            ["StringFieldExpression"] = "test-string-field-expression",
            ["IntegerFieldExpression"] = 12
        });
        
        Assert.Equal("test-string-field-expression-012.jpg", processResult);
    }
    
    [Fact]
    public void ProcessReplaceFields_ReplacesSlashesInReplaceFields_ToUnderscores()
    {
        var processResult = TestProcessReplaceFields("a-|Name|", row: new TestIIdRow
        {
            Name = "//"
        });
        
        Assert.Equal("a-__.jpg", processResult);
    }
    
    [Fact]
    public void ProcessReplaceFields_ReplacesDoubleSlashes_ToSlashUnderscoreSlash_WhenThereAreAnyReplaceFields()
    {
        var processResult = TestProcessReplaceFields("a-|Name|-//", row: new TestIIdRow()
        {
            Name = "test"
        });
        
        Assert.Equal("a-test-/_/.jpg", processResult);
    }

    private class IgnoreSlashEmptySanitizer : IFilenameFormatSanitizer
    {
        public string SanitizePlaceholder(string key, string value)
        {
            value = StringHelper.SanitizeFilePath((value ?? "").Replace('\\', '/'));
            value = value.Replace("..", "_", StringComparison.Ordinal);
            return value;
        }

        public string SanitizeResult(string result)
        {
            if (string.IsNullOrEmpty(result))
                return result;

            result = result.Replace('\\', '/');

            while (result.Contains("//"))
                result = result.Replace("//", "/", StringComparison.Ordinal);

            return result;
        }
    }

    [Fact]
    public void ProcessReplaceFields_Uses_Registered_IgnoreSlashSanitizer()
    {
        var processResult = TestProcessReplaceFields("x/|Name|/|Empty|/y", row: new TestIIdRow
        {
            Name = "a/b/c/d"
        }, sanitizer: new IgnoreSlashEmptySanitizer());

        Assert.Equal("x/a/b/c/d/y.jpg", processResult);
    }

    private string TestProcessReplaceFields(string fileNameFormat, IDictionary<string, object> foreignFieldQueryResults = null, TestIIdRow  row = null, bool? isUpdate = null,
        IFilenameFormatSanitizer sanitizer = null)
    {
        isUpdate ??= foreignFieldQueryResults == null;
        
        row ??= new TestIIdRow
        {
            Id = isUpdate.Value ? 1234: null
        };

        row.StringFieldImageUploadEditor ??= "temporary/new.jpg";

        var attr = row.GetFields().StringFieldImageUploadEditor.GetAttribute<ImageUploadEditorAttribute>();
        var oldFormat = attr.FilenameFormat;
        try
        {
            attr.FilenameFormat = fileNameFormat;

            var mockUploadStorage = (MockUploadStorage)MockUploadStorage.Create();
            var mockFileSystem = (MockFileSystem)mockUploadStorage.MockFileSystem;

            var sut = new FileUploadBehavior(new MockUploadValidator(), new MockImageProcessor(), mockUploadStorage,
                formatSanitizer: sanitizer)
            {
                Target = TestIIdRow.Fields.StringFieldImageUploadEditor
            };

            mockFileSystem.AddFile(row.StringFieldImageUploadEditor, new MockFileData(CreateImage(1000, 1000)));

            sut.ActivateFor(row);

            var dbConnection = new MockDbConnection();
            dbConnection.OnExecuteReader(command =>
            {
                var selectFields = MockDbDataReader.ParseSelectFieldAliases(command.CommandText).ToList();
                var retRow = new TestIIdRow();

                foreach (var field in selectFields)
                {
                    object value = field + "_value";

                    if (foreignFieldQueryResults?.TryGetValue(field, out var dictValue) == true)
                        value = dictValue;

                    ((IRow)retRow)[field] = value;
                }

                return new MockDbDataReader(command.CommandText, retRow);
            });

            var uow = new MockUnitOfWork(dbConnection);
            var requestHandler = new MockSaveHandler<TestIIdRow>
            {
                IsCreate = !isUpdate.Value,
                IsUpdate = isUpdate.Value,
                Old = isUpdate.Value ? new TestIIdRow { Id = 1234 } : null,
                Row = row,
                UnitOfWork = uow,
                Connection = uow.Connection
            };

            sut.OnBeforeSave(requestHandler);
            if (!isUpdate.Value)
                row.Id = 1234;
            sut.OnAfterSave(requestHandler);
            uow.Commit();

            var file = Assert.Single(mockFileSystem.AllFiles);
            return file[mockFileSystem.Path.GetPathRoot(file).Length..].Replace('\\', '/'); // remove drive letter
        }
        finally
        {
            attr.FilenameFormat = oldFormat;
        }
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