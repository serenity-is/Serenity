using Xunit;

namespace Serenity.Test.Data
{
    public class RowTests
    {
        [Fact]
        public void NoNullReferenceException_FieldAssignedValue_PropertyChangedExists()
        {
            var row = new RowMappingTests.BasicRow();
            row.PropertyChanged += (sender, e) => { };
            Assert.Null(Record.Exception(() => row.AString = "AString"));
        }
    }
}