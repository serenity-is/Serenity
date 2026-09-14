namespace Serenity.CodeGenerator
{
    public partial class ApplicationMetadataTests
    {
        [Fact]
        public void Strips_Leading_Slash_From_Route()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("Local")!;

            Assert.Equal("AppMeta/Local/List", row.ListServiceRoute);
        }

        [Fact]
        public void Ignores_Route_Without_Action_Placeholder()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("Plain")!;

            Assert.Equal("PlainRow", row.ClassName);
            Assert.Null(row.ListServiceRoute);
        }
    }
}
