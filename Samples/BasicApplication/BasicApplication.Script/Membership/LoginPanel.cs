
namespace BasicApplication.Membership
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;
    using System.Html;

    [FormKey("Membership.User"), Panel]
    public class LoginPanel : PropertyDialog<object>
    {
        public LoginPanel()
        {
            this.ById("LoginButton").Click((s, e) => {
                e.PreventDefault();

                if (!ValidateForm())
                    return;

                var request = GetSaveEntity();
                Q.ServiceCall(new ServiceCallOptions
                {
                    Url = Q.ResolveUrl("~/Account/Login"),
                    Request = request.As<ServiceRequest>(),
                    OnSuccess = response =>
                    {
                        var q = Q.Externals.ParseQueryString();
                        var r = q["returnUrl"] ?? q["ReturnUrl"];
                        if (!string.IsNullOrEmpty(r))
                            Window.Location.Href = r;
                        else
                            Window.Location.Href = Q.ResolveUrl("~/");
                    }
                });
                
            });
        }
    }
}