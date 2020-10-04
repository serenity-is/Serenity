using Serenity.Web;
using System;
using Xunit;
using MyException = System.Web.HttpException;

namespace Serenity.Web.Test
{
    public class UploadHelper_CheckFileNameSecurityTests
    {
        [Fact]
        public void ThrowsWhenNull()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity(null);
            });
        }

        [Fact]
        public void ThrowsWhenEmpty()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity(String.Empty);
            });
        }

        [Fact]
        public void ThrowsWhenWhitespace()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("   ");
            });
        }

        [Fact]
        public void ThrowsWhenJustDot()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity(" . ");
            });
        }

        [Fact]
        public void ThrowsWhenJustDoubleDot()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity(" .. ");
            });
        }

        [Fact]
        public void ThrowsWhenDoubleDotSlashAtStart()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("../some");
            });
        }

        [Fact]
        public void ThrowsWhenDoubleDotSlashMiddle()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("a/../b");
            });
        }

        [Fact]
        public void ThrowsWhenDoubleDotSlashEnd()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("a/b../");
            });
        }

        [Fact]
        public void ThrowsWhenContainsColon()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("a:b");
            });
        }

        [Fact]
        public void ThrowsWhenStartsWithSlash()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("/a");
            });
        }

        [Fact]
        public void ThrowsWhenStartsWithSpaceSlash()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity(" /b");
            });
        }

        [Fact]
        public void ThrowsWhenEndsWithSlash()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("a/");
            });
        }

        [Fact]
        public void ThrowsWhenEndsWithSlashSpace()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("b/ ");
            });
        }

        [Fact]
        public void ThrowsWhenStartsWithBackSlash()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("\\a");
            });
        }

        [Fact]
        public void ThrowsWhenStartsWithSpaceBackSlash()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity(" \\b");
            });
        }

        [Fact]
        public void ThrowsWhenEndsWithBackSlash()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("a\\");
            });
        }

        [Fact]
        public void ThrowsWhenEndsWithBackSlashSpace()
        {
            Assert.Throws<MyException>(() =>
            {
                UploadHelper.CheckFileNameSecurity("b\\ ");
            });
        }

        [Fact]
        public void OkForBasicFilenames()
        {
            UploadHelper.CheckFileNameSecurity("test.jpg");
            UploadHelper.CheckFileNameSecurity("x");
            UploadHelper.CheckFileNameSecurity("x.y");
            UploadHelper.CheckFileNameSecurity("a.txt");
            UploadHelper.CheckFileNameSecurity("xyz.png");
        }

        [Fact]
        public void OkForBasicFilenamesWithSubFolders()
        {
            UploadHelper.CheckFileNameSecurity("a/b/test.jpg");
            UploadHelper.CheckFileNameSecurity("x/x");
            UploadHelper.CheckFileNameSecurity("z/u/x.y");
            UploadHelper.CheckFileNameSecurity("a-b/c.txt");
            UploadHelper.CheckFileNameSecurity("a/test folder/xyz.png");
            UploadHelper.CheckFileNameSecurity("a\\b\\test.jpg");
            UploadHelper.CheckFileNameSecurity("x\\x");
            UploadHelper.CheckFileNameSecurity("z\\u\\x.y");
            UploadHelper.CheckFileNameSecurity("a-b\\c.txt");
            UploadHelper.CheckFileNameSecurity("a\\test folder\\xyz.png");
        }

    }
}