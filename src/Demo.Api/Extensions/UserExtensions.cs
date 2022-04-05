using Microsoft.Extensions.Primitives;

namespace Demo.Api.Extensions
{
    public static class UserExtensions
    {
        public static int GetUserId(IHttpContextAccessor httpContextAccessor)
        {
            var userId = 0;
            if (httpContextAccessor?.HttpContext?.Request.HttpContext.Items["UserId"] != null)
            {
                userId = int.Parse(httpContextAccessor.HttpContext?.Request.HttpContext.Items["UserId"].ToString() ??
                                   string.Empty);
            }

            return userId;
        }

        public static string GetUserToken(IHttpContextAccessor httpContextAccessor)
        {
            string userToken;
            if (httpContextAccessor?.HttpContext?.Request.HttpContext.Items["UserToken"] != null)
            {
                userToken = httpContextAccessor.HttpContext?.Request.HttpContext.Items["UserToken"].ToString() ??
                            string.Empty;
            }
            else
            {
                StringValues token = default;
                httpContextAccessor?.HttpContext?.Request.Headers.TryGetValue("Authorization", out token);
                userToken = token.FirstOrDefault();
            }

            return userToken;
        }
    }
}