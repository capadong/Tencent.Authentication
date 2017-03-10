using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Tencent.Authentication;

namespace Microsoft.AspNetCore.Builder
{
    public static class TencentAppBuilderExtensions
    {
        public static IApplicationBuilder UseTencentQQAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<TencentMiddleware>();
        }

        public static IApplicationBuilder UseTencentQQAuthentication(this IApplicationBuilder app, TencentOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<TencentMiddleware>(Options.Create(options));
        }
    }
}
