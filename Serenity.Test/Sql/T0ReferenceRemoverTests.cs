using System;
using Xunit;

namespace Serenity.Data.Test
{
    public partial class T0ReferenceRemoverTests
    {
        [Fact]
        public void RemoveT0AliasesWithNullExpressionThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => T0ReferenceRemover.RemoveT0Aliases(null));
        }

        [Fact]
        public void RemoveT0AliasesWorksWithEmptyString()
        {
            Assert.Equal("", T0ReferenceRemover.RemoveT0Aliases(""));
        }

        [Fact]
        public void RemoveT0AliasesLeavesExpressionsWithNoT0AliasesAsIs()
        {
            Assert.Equal("ABC", T0ReferenceRemover.RemoveT0Aliases("ABC"));
            Assert.Equal("abc", T0ReferenceRemover.RemoveT0Aliases("abc"));
            Assert.Equal("'!-''.^.&", T0ReferenceRemover.RemoveT0Aliases("'!-''.^.&"));
        }

        [Fact]
        public void RemoveT0AliasesLeavesT0ExpressionsInStringsAsIs()
        {
            Assert.Equal("'T0.'", T0ReferenceRemover.RemoveT0Aliases("'T0.'"));
            Assert.Equal("'T0.'", T0ReferenceRemover.RemoveT0Aliases("'T0.'"));
            Assert.Equal("'T0.''t0.T1.T0.'", T0ReferenceRemover.RemoveT0Aliases("'T0.''t0.T1.T0.'"));
            Assert.Equal("'!-''T0.^.&", T0ReferenceRemover.RemoveT0Aliases("'!-''T0.^.&"));
        }

        [Fact]
        public void RemoveT0AliasesDeletesAnyT0ReferenceOutsideStrings()
        {
            Assert.Equal("'T0.'", T0ReferenceRemover.RemoveT0Aliases("t0.'T0.'"));
            Assert.Equal("Code", T0ReferenceRemover.RemoveT0Aliases("T0.Code"));
            Assert.Equal("(Code)", T0ReferenceRemover.RemoveT0Aliases("(T0.Code)"));
            Assert.Equal("(Code + ' ' + Name)", T0ReferenceRemover.RemoveT0Aliases("(T0.Code + ' ' + T0.Name)"));
        }
    }
}