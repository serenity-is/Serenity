namespace Serenity.CodeGenerator
{
    public partial class ApplicationMetadataTests
    {
        private static ApplicationMetadata CreateMetadata(Action<ApplicationMetadata>? onCreated = null)
        {
            var metadata = new ApplicationMetadata(new MockFileSystem(),
                typeof(ApplicationMetadataTests).Assembly.Location);
            onCreated?.Invoke(metadata);
            return metadata;
        }

        [Fact]
        public void GetRowByTablename_Throws_ArgumentNull()
        {
            var metadata = CreateMetadata();
            Assert.Throws<ArgumentNullException>(() => metadata.GetRowByTablename(null!));
        }
    }
}
