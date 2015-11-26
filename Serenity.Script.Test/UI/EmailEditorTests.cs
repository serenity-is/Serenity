using jQueryApi;
using QUnit;
using System;

namespace Serenity.Test
{
    [TestFixture]
    public class EmailEditorTests : ScriptContext
    {
        private static jQueryValidationObject CreateValidatedForm()
        {
            var div = Q.NewBodyDiv();
            var form = J("<form/>").AppendTo(div).As<jQueryValidationObject>();
            form.Validate(Q.Externals.ValidateOptions(new jQueryValidatorOptions()));
            return form;
        }

        private static object CreateDummyValidator()
        {
            return new
            {
                optional = new Func<string>(() => null)
            };
        }

        [Test]
        public void EmailValidationClassWorks()
        {
            var form = CreateValidatedForm();
            var validator = form.Validate();
            var input = J("<input type='text'/>").Attribute("name", "dummy").AddClass("email").AppendTo(form);

            input.Value("valid.email@address.com");
            Assert.IsTrue(validator.ValidateElement(input[0]));

            input.Value("invalid.email@");
            Assert.IsFalse(validator.ValidateElement(input[0]));

            form.Parent().Remove();
        }

        [Test]
        public void EmailValidationMethodWorks()
        {
            var oldAscii = Q.Config.EmailAllowOnlyAscii;
            try
            {
                var form = CreateValidatedForm();
                var validator = form.Validate();
                var input = J("<input type='text'/>").Attribute("name", "dummy").AppendTo(form);
                var jq = jQuery.Instance;

                foreach (var onlyAscii in new bool[] { false, true })
                {
                    Q.Config.EmailAllowOnlyAscii = onlyAscii;

                    Assert.IsTrue(jq.validator.methods.email.call(validator, "x@y.com", input.Value("x@y.com")[0]));
                    Assert.IsTrue(jq.validator.methods.email.call(validator, "some.user@somedomain.com", input.Value("some.user@somedomain.com")[0]));
                    Assert.IsTrue(jq.validator.methods.email.call(validator, "some_user@some.domain.com.tr", input.Value("some_user@some.domain.com.tr")[0]));

                    Assert.IsFalse(jq.validator.methods.email.call(validator, "abcdef", input.Value("abcdef")[0]));
                    Assert.IsFalse(jq.validator.methods.email.call(validator, "xyz@", input.Value("xyz@")[0]));
                    Assert.IsFalse(jq.validator.methods.email.call(validator, "@xyz", input.Value("@xyz")[0]));
                    Assert.IsFalse(jq.validator.methods.email.call(validator, "@xyz.com", input.Value("@xyz.com")[0]));

                    if (onlyAscii)
                        Assert.AreEqual(false, jq.validator.methods.email.call(validator, "êığş@ädomaın.com", input.Value("êığş@ädomaın.com")[0]));
                }
                
                form.Parent().Remove();
            }
            finally
            {
                Q.Config.EmailAllowOnlyAscii = oldAscii;
            }
        }

        [Test]
        public void EmailUserValidationMethodWorks()
        {
            var oldAscii = Q.Config.EmailAllowOnlyAscii;
            try
            {
                var form = CreateValidatedForm();
                var validator = form.Validate();
                var user = J("<input type='text' class='emailuser'/>").AppendTo(form);
                var domain = J("<input type='text' class='emaildomain'/>").AppendTo(form);
                var jq = jQuery.Instance;

                EmailEditor.RegisterValidationMethods();

                foreach (var domainReadOnly in new bool[] { false, true })
                    foreach (var onlyAscii in new bool[] { false, true })
                    {
                        var title = (domainReadOnly ? "domainreadonly_" : "");
                        title += onlyAscii ? "ascii_" : "unicode_";

                        if (domainReadOnly)
                            domain.Attribute("readonly", "readonly");
                        else
                            domain.RemoveAttr("readonly");

                        domain.Value(onlyAscii ? "ascii.domain.com" : "unıcõde.domaîn.com");
                        Q.Config.EmailAllowOnlyAscii = onlyAscii;

                        Assert.IsTrue(jq.validator.methods.emailuser.call(validator, "x", user.Value("x")[0]), title + "1");
                        Assert.IsTrue(jq.validator.methods.emailuser.call(validator, "some.user", user.Value("some.user")[0]), title + "2");

                        domain.Value("");
                        Assert.IsFalse(jq.validator.methods.emailuser.call(validator, " ", user.Value(" ")[0]), title + "3");

                        domain.Value(onlyAscii ? "ascii.domain.co.uk" : "unıcõde.domaîn.co.uk");

                        Assert.IsTrue(jq.validator.methods.emailuser.call(validator, "some_user", user.Value("some_user")[0]), title + "4");

                        Assert.IsFalse(jq.validator.methods.emailuser.call(validator, "xyz@", user.Value("xyz@")[0]), title + "5");
                        Assert.IsFalse(jq.validator.methods.emailuser.call(validator, "@xyz", user.Value("@xyz")[0]), title + "6");
                        Assert.IsFalse(jq.validator.methods.emailuser.call(validator, "@@", user.Value("@@")[0]), title + "7");

                        Assert.AreEqual(!onlyAscii, jq.validator.methods.emailuser.call(validator, "êığş", user.Value("êığş")[0]), title + "8");
                    }

                form.Parent().Remove();
            }
            finally
            {
                Q.Config.EmailAllowOnlyAscii = oldAscii;
            }
        }

        [Test]
        public void EmailEditorUserOnlyValidationWorks()
        {
            var form = CreateValidatedForm();
            var validator = form.Validate();
            var input = J("<input type='text'/>").Attribute("name", "dummy").AddClass("email").AppendTo(form);

            input.Value("valid.email@address.com");
            Assert.IsTrue(validator.ValidateElement(input[0]));

            input.Value("invalid.email@");
            Assert.IsFalse(validator.ValidateElement(input[0]));

            form.Parent().Remove();
        }
    }
}
