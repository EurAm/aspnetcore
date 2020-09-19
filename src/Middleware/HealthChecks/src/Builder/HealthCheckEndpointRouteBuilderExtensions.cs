// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add health checks.
    /// </summary>
    public static class HealthCheckEndpointRouteBuilderExtensions
    {
        private const string DefaultDisplayName = "Health checks";

        /// <summary>
        /// Adds a health checks endpoint to the <see cref="IEndpointRouteBuilder"/> with the specified template.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the health checks endpoint to.</param>
        /// <param name="pattern">The URL pattern of the health checks endpoint.</param>
        /// <returns>A convention routes for the health checks endpoint.</returns>
        public static IEndpointConventionBuilder MapHealthChecks(
           this IEndpointRouteBuilder endpoints,
           string pattern)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            return MapHealthChecksCore(endpoints, pattern, null);
        }

        /// <summary>
        /// Adds a health checks endpoint to the <see cref="IEndpointRouteBuilder"/> with the specified template and options.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the health checks endpoint to.</param>
        /// <param name="pattern">The URL pattern of the health checks endpoint.</param>
        /// <param name="options">A <see cref="HealthCheckOptions"/> used to configure the health checks.</param>
        /// <returns>A convention routes for the health checks endpoint.</returns>
        public static IEndpointConventionBuilder MapHealthChecks(
           this IEndpointRouteBuilder endpoints,
           string pattern,
           HealthCheckOptions options)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return MapHealthChecksCore(endpoints, pattern, options);
        }

        private static IEndpointConventionBuilder MapHealthChecksCore(IEndpointRouteBuilder endpoints, string pattern, HealthCheckOptions? options)
        {
            if (endpoints.ServiceProvider.GetService(typeof(HealthCheckService)) == null)
            {
                throw new InvalidOperationException(Resources.FormatUnableToFindServices(
                    nameof(IServiceCollection),
                    nameof(HealthCheckServiceCollectionExtensions.AddHealthChecks),
                    "ConfigureServices(...)"));
            }

            var args = options != null ? new[] { Options.Create(options) } : Array.Empty<object>();

            var pipeline = endpoints.CreateApplicationBuilder()
               .UseMiddleware<HealthCheckMiddleware>(args)
               .Build();

            return endpoints.Map(pattern, pipeline).WithDisplayName(DefaultDisplayName);
        }
    }
}
