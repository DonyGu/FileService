using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Comm100.Framework.Common
{
    public static class JwtTokenConstants
    {
        /// <summary>
        /// JWT Token Id claim name
        /// </summary>
        public const string Jti = "jti";
        /// <summary>
        /// Agent Id claim name
        /// </summary>
        public const string AgentId = "agentId";
        /// <summary>
        /// Site Id claim name
        /// </summary>
        public const string SiteId = "siteId";
        /// <summary>
        /// If logined successfully
        /// </summary>
        public const string Success = "success";
        /// <summary>
        /// Email claim name
        /// </summary>
        public const string Email = "email";
        /// <summary>
        /// LoginInfo claim name
        /// </summary>
        public const string LoginInfo = "loginInfo";
        /// <summary>
        /// Thumbprint of the certificate
        /// </summary>
        public const string Thumbprint = "thumbprint";
    }

    public static class JwtExtensions
    {
        public static string GetThumbprint(this JwtSecurityToken jwtToken)
        {
            return jwtToken.GetTypeValue(JwtTokenConstants.Thumbprint);
        }

        public static int GetSiteId(this JwtSecurityToken jwtToken)
        {
            int.TryParse(jwtToken.GetTypeValue(JwtTokenConstants.SiteId), out int siteId); // Default value is 0.
            return siteId;
        }

        public static int GetAgentId(this JwtSecurityToken jwtToken)
        {
            Int32.TryParse(jwtToken.GetTypeValue(JwtTokenConstants.AgentId), out int agentId); // Default value is 0.
            return agentId;
        }

        public static bool GetIfSuccess(this JwtSecurityToken jwtToken)
        {
            bool.TryParse(jwtToken.GetTypeValue(JwtTokenConstants.Success), out bool success); // Default value is false.
            return success;
        }

        public static string GetEmail(this JwtSecurityToken jwtToken)
        {
            return jwtToken.GetTypeValue(JwtTokenConstants.Email);
        }

        public static string GetTypeValue(this JwtSecurityToken jwtToken, string type)
        {
            return jwtToken.Claims.FirstOrDefault(c => c.Type == type)?.Value;
        }

        public static string GetThumbprint(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.GetTypeValue(JwtTokenConstants.Thumbprint);
        }

        public static int GetSiteId(this ClaimsPrincipal claimsPrincipal)
        {
            int.TryParse(claimsPrincipal.GetTypeValue(JwtTokenConstants.SiteId), out int siteId); // Default value is 0.
            return siteId;
        }

        public static int GetAgentId(this ClaimsPrincipal claimsPrincipal)
        {
            int.TryParse(claimsPrincipal.GetTypeValue(JwtTokenConstants.AgentId), out int agentId); // Default value is 0.
            return agentId;
        }

        public static bool GetIfSuccess(this ClaimsPrincipal claimsPrincipal)
        {
            bool.TryParse(claimsPrincipal.GetTypeValue(JwtTokenConstants.Success), out bool success); // Default value is false.
            return success;
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.GetTypeValue(JwtTokenConstants.Email);
        }

        public static string GetTypeValue(this ClaimsPrincipal claimsPrincipal, string type)
        {
            return claimsPrincipal.FindFirst(type)?.Value;
        }
    }
}
