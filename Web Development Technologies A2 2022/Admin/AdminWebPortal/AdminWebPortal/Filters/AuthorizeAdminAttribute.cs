using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminWebPortal.Filters;

public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter {
    public void OnAuthorization(AuthorizationFilterContext context) {

        string admin = context.HttpContext.Session.GetString("Admin");
        if (admin != "Admin")
            context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}
