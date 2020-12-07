using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagmentWeb.Handler
{
   
    public class RoleAccessRequirement : IAuthorizationRequirement
    {
    }
    public class RoleCheckerHandler : AuthorizationHandler<RoleAccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAccessRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "Roles"))
            {
                return Task.FromResult(0);
            }

            var isHR = context.User.FindFirst(c => c.Type == "Roles").Value;
            context.Succeed(requirement);
            //if (isHR)
            //{
            //    context.Succeed(requirement);
            //}
            return Task.FromResult(0);
        }
    }
}
