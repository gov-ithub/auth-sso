using Microsoft.AspNetCore.Authorization;

namespace GovITHub.Auth.Admin.Services
{
    public static class AuthorizationHandlerContextExtensions
    {
        public static bool TryGetRouteValue<T>(this AuthorizationHandlerContext context, string key, out T value)
        {
            Microsoft.AspNetCore.Routing.RouteValueDictionary routeValues = ((Microsoft.AspNetCore.Mvc.ActionContext)context.Resource).RouteData.Values;

            object returnValue;
            if (routeValues.TryGetValue(key, out returnValue))
            {
                value = (T)System.Convert.ChangeType(returnValue, typeof(T));

                return true;
            }
            else
            {
                value = default(T);

                return false;
            }
        }
    }
}