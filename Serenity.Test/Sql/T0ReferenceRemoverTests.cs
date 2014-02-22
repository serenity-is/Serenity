using Serenity.Testing.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Serenity.Data.Test
{
    public partial class T0ReferenceRemoverTests
    {
        [Fact]
        public void T0ReferenceRemoverWithNullExpressionThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => T0ReferenceRemover.RemoveT0Aliases(null));
        }

        [Fact]
        public void T0ReferenceRemoverWorksWithEmptyString()
        {
            Assert.Equal("", T0ReferenceRemover.RemoveT0Aliases(""));
        }

        [Fact]
        public void T0ReferenceRemoverLeavesExpressionsWithNoT0AliasesAsIs()
        {
            Assert.Equal("ABC", T0ReferenceRemover.RemoveT0Aliases("ABC"));
            Assert.Equal("abc", T0ReferenceRemover.RemoveT0Aliases("abc"));
            Assert.Equal("'!-''.^.&", T0ReferenceRemover.RemoveT0Aliases("'!-''.^.&"));
        }

        [Fact]
        public void T0ReferenceRemoverLeavesT0ExpressionsInStringsAsIs()
        {
            Assert.Equal("'T0.'", T0ReferenceRemover.RemoveT0Aliases("'T0.'"));
            Assert.Equal("'T0.'", T0ReferenceRemover.RemoveT0Aliases("'T0.'"));
            Assert.Equal("'T0.''t0.T1.T0.'", T0ReferenceRemover.RemoveT0Aliases("'T0.''t0.T1.T0.'"));
            Assert.Equal("'!-''T0.^.&", T0ReferenceRemover.RemoveT0Aliases("'!-''T0.^.&"));
        }

        [Fact]
        public void T0ReferenceRemoverDeletesAnyT0ReferenceOutsideStrings()
        {
            Assert.Equal("'T0.'", T0ReferenceRemover.RemoveT0Aliases("t0.'T0.'"));
            //Assert.Equal("Code", T0ReferenceRemover.RemoveT0Aliases("T0.Code"));
            //Assert.Equal("(Code)", T0ReferenceRemover.RemoveT0Aliases("(T0.Code)"));
            //Assert.Equal("(Code+' '+Name)", T0ReferenceRemover.RemoveT0Aliases("(T0.Code + ' ' + T0.Name)"));
        }
    }
}