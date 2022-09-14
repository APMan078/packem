using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Packem.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packem.WebApi.Common.Authorization
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public RoleEnum[] AuthorizedRoles { get; set; }

        public CustomAuthorizeAttribute(params RoleEnum[] roles)
        {
            AuthorizedRoles = roles;
        }

    //    protected override AuthorizeViewCore(HttpContextBase
    //httpContext)
    //    {
    //        if (httpContext == null)
    //        {
    //            throw new ArgumentNullException(nameof(httpContext));
    //        }

    //        if (AuthorizedRoles.Any(r =>
    //        httpContext.User.IsInRole(r.ToString())))
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    }
}