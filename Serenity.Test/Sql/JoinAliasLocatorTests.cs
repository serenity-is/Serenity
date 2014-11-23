using System;
using Xunit;

namespace Serenity.Data.Test
{
    public partial class JoinAliasLocatorTests
    {
        [Fact]
        public void LocateOptimizedWithNullExpressionThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => {
                string singleAlias;
                JoinAliasLocator.LocateOptimized(null, out singleAlias);
            });
        }

        [Fact]
        public void LocateOptimizedWorksWithEmptyString()
        {
            string singleAlias;
            var aliases = JoinAliasLocator.LocateOptimized("", out singleAlias);
            Assert.Equal(null, aliases);
            Assert.Equal(null, singleAlias);
        }

        [Fact]
        public void LocateOptimizedReturnsSingleAliasIfExpressionContainsSingle()
        {
            string singleAlias;
            var aliases = JoinAliasLocator.LocateOptimized("x.y", out singleAlias);
            Assert.Equal(null, aliases);
            Assert.Equal("x", singleAlias);
        }

        [Fact]
        public void LocateOptimizedReturnsSingleAliasIfExpressionContainsSingleAliasDoubleTimes()
        {
            string singleAlias;
            var aliases = JoinAliasLocator.LocateOptimized("x.y + x.z", out singleAlias);
            Assert.Equal(null, aliases);
            Assert.Equal("x", singleAlias);
        }

        [Fact]
        public void LocateOptimizedReturnsHashSetIfExpressionContainsDoubleAliases()
        {
            string singleAlias;
            var aliases = JoinAliasLocator.LocateOptimized("x.y + y.z", out singleAlias);
            Assert.Equal(null, singleAlias);
            Assert.Equal(2, aliases.Count);
            Assert.True(aliases.Contains("x"));
            Assert.True(aliases.Contains("y"));
        }

        [Fact]
        public void LocateOptimizedReturnsHashSetIfExpressionContainsTripleAliases()
        {
            string singleAlias;
            var aliases = JoinAliasLocator.LocateOptimized("x.y + y.z + u.w", out singleAlias);
            Assert.Equal(null, singleAlias);
            Assert.Equal(3, aliases.Count);
            Assert.True(aliases.Contains("x"));
            Assert.True(aliases.Contains("y"));
            Assert.True(aliases.Contains("u"));
        }

        [Fact]
        public void LocateOptimizedReturnsHashSetIfExpressionContainsQuadrupleAliases()
        {
            string singleAlias;
            var aliases = JoinAliasLocator.LocateOptimized("x.y + y.z + u.w + t.v", out singleAlias);
            Assert.Equal(null, singleAlias);
            Assert.Equal(4, aliases.Count);
            Assert.True(aliases.Contains("x"));
            Assert.True(aliases.Contains("y"));
            Assert.True(aliases.Contains("u"));
            Assert.True(aliases.Contains("t"));
        }

        [Fact]
        public void LocateOptimizedReturnsHashSetWithIgnoreCase()
        {
            string singleAlias;
            var aliases = JoinAliasLocator.LocateOptimized("x.y + y.z", out singleAlias);
            Assert.Equal(null, singleAlias);
            Assert.Equal(2, aliases.Count);
            Assert.True(aliases.Contains("X"));
            Assert.True(aliases.Contains("x"));
            Assert.True(aliases.Contains("Y"));
            Assert.True(aliases.Contains("y"));
        }

        [Fact]
        public void EnumerateAliasesDoesntEnumerateBracedAliases()
        {
            string singleAlias;
            var aliases = JoinAliasLocator.LocateOptimized("x.y + [a].b + y.z + [a].[b].d", out singleAlias);
            Assert.Equal(null, singleAlias);
            Assert.Equal(2, aliases.Count);
            Assert.True(aliases.Contains("x"));
            Assert.True(aliases.Contains("y"));
            Assert.Equal(2, aliases.Count);
        }

        [Fact]
        public void ReplaceAliasesIgnoresBracedAliases()
        {
            var result = JoinAliasLocator.ReplaceAliases("x.y + [a].b + y.z + [a].[b].d", x => "_" + x);
            Assert.Equal("_x.y + [a].b + _y.z + [a].[b].d", result);
        }
    }
}