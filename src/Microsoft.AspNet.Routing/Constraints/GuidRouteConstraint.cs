// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Routing.Constraints
{
    /// <summary>
    /// Constrains a route parameter to represent only <see cref="Guid"/> values.
    /// Matches values specified in any of the five formats "N", "D", "B", "P", or "X",
    /// supported by Guid.ToString(string) and Guid.ToString(String, IFormatProvider) methods.
    /// </summary>
    public class GuidRouteConstraint : IRouteConstraint
    {
        /// <inheritdoc />
        public bool Match([NotNull] HttpContext httpContext,
                          [NotNull] IRouter route,
                          [NotNull] string routeKey,
                          [NotNull] IDictionary<string, object> values,
                          RouteDirection routeDirection)
        {
            object value;
            if (values.TryGetValue(routeKey, out value) && value != null)
            {
                if (value is Guid)
                {
                    return true;
                }

                Guid result;
                var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                return Guid.TryParse(valueString, out result);
            }

            return false;
        }
    }
}