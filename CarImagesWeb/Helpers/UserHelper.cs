using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CarImagesWeb.Helpers
{
    public static class UserHelper
    {
        public static List<string> GetRolesOfUser(ClaimsPrincipal user)
        {
            return user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        }
    }
}