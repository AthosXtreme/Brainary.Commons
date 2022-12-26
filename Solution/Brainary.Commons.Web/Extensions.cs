using Microsoft.AspNetCore.Http;

namespace Brainary.Commons.Web
{
    public static class Extensions
    {
        /// <summary>
        /// Obtain remote IP from request
        /// </summary>
        /// <param name="request">Current request</param>
        /// <returns>IP address</returns>
        public static string? GetUserIp(this HttpRequest request)
        {
            var ip = request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(ip)) ip = ip.Split(',').First();

            if (string.IsNullOrWhiteSpace(ip)) ip = Convert.ToString(request.HttpContext.Connection.RemoteIpAddress);

            if (string.IsNullOrWhiteSpace(ip)) ip = request.Headers["REMOTE_ADDR"].FirstOrDefault();

            return ip;
        }
    }
}
