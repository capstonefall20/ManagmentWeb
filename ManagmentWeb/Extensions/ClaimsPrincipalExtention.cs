using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


    public static class ClaimsPrincipalExtention
    {
        public static bool HasClaim(this ClaimsPrincipal claimsPrincipal, string claim)
        {
            if (claimsPrincipal == null)
            {
                return false;
            }

            return claimsPrincipal.HasClaim(c => c.Type == claim);
        }
        public static int GetIntValue(this ClaimsPrincipal claimsPrincipal, string claim)
        {
            if (claimsPrincipal == null)
            {
                return 0;
            }

            if (claimsPrincipal.HasClaim(claim))
            {
                string claimValue = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claim).Value;
                if (!string.IsNullOrEmpty(claimValue))
                {
                    return Int32.Parse(claimValue);
                }
            }
            return 0;
        }
        public static bool IsInRoles(this ClaimsPrincipal claimsPrincipal, params string[] roles)
        {
            if (claimsPrincipal == null)
            {
                return false;
            }
            var rolesList = roles.ToList();

            return rolesList.Any(r => claimsPrincipal.IsInRole(r));
        }
        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                return 0;
            }

            return claimsPrincipal.GetIntValue(CustomClaims.UserId);
        }
    }

