using ServerTypingsTest.SomeModule.Entities;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Emits_UndeletePermission_For_SoftDeleteRow()
        {
            var generator = CreateGenerator(typeof(SoftDeleteWithPermissionsRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "SomeModule/SoftDeleteWithPermissionsRow.ts").Text;
            Assert.Contains("undeletePermission = 'Undelete'", code);
        }

        [Fact]
        public void Emits_UndeletePermission_Falling_Back_To_Modify()
        {
            var generator = CreateGenerator(typeof(SoftDeleteWithoutUndeletePermissionRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "SomeModule/SoftDeleteWithoutUndeletePermissionRow.ts").Text;
            Assert.Contains("undeletePermission = 'Modify'", code);
        }

        [Fact]
        public void Does_Not_Emit_UndeletePermission_For_NonSoftDeleteRow()
        {
            var generator = CreateGenerator(typeof(NonSoftDeleteWithPermissionsRow));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "SomeModule/NonSoftDeleteWithPermissionsRow.ts").Text;
            Assert.Contains("deletePermission = 'Delete'", code);
            Assert.DoesNotContain("undeletePermission", code);
        }
    }
}

namespace ServerTypingsTest.SomeModule.Entities
{
    [ReadPermission("Read")]
    [ModifyPermission("Modify")]
    [DeletePermission("Delete")]
    [UndeletePermission("Undelete")]
    public class SoftDeleteWithPermissionsRow : Row<SoftDeleteWithPermissionsRow.RowFields>, IIsActiveDeletedRow
    {
        public short? IsActive
        {
            get { return fields.IsActive[this]; }
            set { fields.IsActive[this] = value; }
        }

        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
            public Int16Field IsActive;
        }
    }

    [ReadPermission("Read")]
    [ModifyPermission("Modify")]
    public class SoftDeleteWithoutUndeletePermissionRow : Row<SoftDeleteWithoutUndeletePermissionRow.RowFields>, IIsDeletedRow
    {
        public bool? IsDeleted
        {
            get { return fields.IsDeleted[this]; }
            set { fields.IsDeleted[this] = value; }
        }

        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public BooleanField IsDeleted;
        }
    }

    [ReadPermission("Read")]
    [ModifyPermission("Modify")]
    [DeletePermission("Delete")]
    public class NonSoftDeleteWithPermissionsRow : Row<NonSoftDeleteWithPermissionsRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }
}
