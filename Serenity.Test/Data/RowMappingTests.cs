using System;
using Serenity.Data;
using Newtonsoft.Json;
using Xunit;

namespace Serenity.Data
{
    public partial class RowMappingTests
    {
        [Fact]
        public void FaultyRowThrowsException()
        {
            Assert.Throws(typeof(TypeInitializationException), () =>
            {
                new FaultyRow();
            });
        }

        [Fact]
        public void BasicRowHasCorrectMappings()
        {
            Assert.Equal("AString", BasicRow.Fields.AString.Name);
            Assert.Equal("T0.AString", BasicRow.Fields.AString.Expression);
            Assert.Equal("AString", BasicRow.Fields.AString.PropertyName);
            Assert.Equal(FieldFlags.Default, BasicRow.Fields.AString.Flags);
            Assert.Equal(0, BasicRow.Fields.AString.Size);
            Assert.Equal(0, BasicRow.Fields.AString.Scale);
            Assert.Equal(null, BasicRow.Fields.AString.DefaultValue);
            Assert.Equal(SelectLevel.Default, BasicRow.Fields.AString.MinSelectLevel);
            Assert.Equal(null, BasicRow.Fields.AString.ForeignTable);
            Assert.Equal(null, BasicRow.Fields.AString.ForeignField);
            Assert.Equal(null, BasicRow.Fields.AString.Join);
            Assert.Equal(null, BasicRow.Fields.AString.JoinAlias);
            Assert.Equal(null, BasicRow.Fields.AString.ReferencedJoins);
            Assert.Equal(null, BasicRow.Fields.AString.Origin);
            Assert.Equal(0, BasicRow.Fields.AString.NaturalOrder);
            Assert.Equal(null, BasicRow.Fields.AString.TextualField);
            Assert.Equal(null, BasicRow.Fields.AString.Caption);
            Assert.Equal("AString", BasicRow.Fields.AString.Title);
            Assert.Equal(FieldType.String, BasicRow.Fields.AString.Type);
            Assert.Equal(0, BasicRow.Fields.AString.Index);
            Assert.Equal(BasicRow.Fields, BasicRow.Fields.AString.Fields);
        }

    }
}