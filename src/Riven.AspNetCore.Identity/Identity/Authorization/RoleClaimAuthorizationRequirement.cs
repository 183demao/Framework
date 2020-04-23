﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Riven.Identity.Authorization
{
    /// <summary>
    /// 基于 Role Claim 的 AuthorizationHandler
    /// </summary>
    public class RoleClaimAuthorizationRequirement : AuthorizationHandler<RoleClaimAuthorizationRequirement>, IAuthorizationRequirement
    {
        readonly IServiceProvider _serviceProvider;

        public RoleClaimAuthorizationRequirement(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            RoleClaimAuthorizationRequirement requirement)
        {
            await Task.Yield();

            if (context.HasSucceeded || context.HasFailed)
            {
                return;
            }

            var routeEndpoint = context.Resource as RouteEndpoint;

            var roleClaimAttributes = routeEndpoint?.Metadata?.GetOrderedMetadata<RoleClaimAttribute>()?.ToList();
            if (roleClaimAttributes == null || roleClaimAttributes.Count == 0)
            {
                context.Succeed(requirement);
                return;
            }

            var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            try
            {
                var identityOptions = serviceProvider.GetService<IOptions<IdentityOptions>>().Value;
                var claimsIdentityUserId = context.User.FindFirst(o => o.Type == identityOptions.ClaimsIdentity.UserIdClaimType);

                Check.NotNull(claimsIdentityUserId, nameof(claimsIdentityUserId));

                var userId = claimsIdentityUserId.Value;

                Check.NotNullOrWhiteSpace(userId, nameof(userId));



            }
            catch (Exception ex)
            {
                context.Fail();

                var logger = serviceProvider.GetService<ILogger<RoleClaimAuthorizationRequirement>>();
                logger.LogError(ex, ex.Message);
            }
            finally
            {
                scope.Dispose();
            }

        }
    }
}