﻿using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

// http://bitoftech.net/2015/01/21/asp-net-identity-2-with-asp-net-web-api-2-accounts-management/


namespace jwt.Infrastructure {
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider {

        public override Task ValidateClientAuthentication(
               OAuthValidateClientAuthenticationContext context) {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(
               OAuthGrantResourceOwnerCredentialsContext context) {
            var allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", 
                                                     new[] { allowedOrigin });
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null) {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed) {
                context.SetError("invalid_grant", "User did not confirm email.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT"); 
            var ticket = new AuthenticationTicket(oAuthIdentity, null);        
            context.Validated(ticket);
           
        }
    }
}