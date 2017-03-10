using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Builder;

namespace Tencent.Authentication
{
    public class TencentMiddleware : OAuthMiddleware<TencentOptions>
    {
        ILoggerFactory _loggerFactory;

        public TencentMiddleware(RequestDelegate next, 
            IDataProtectionProvider dataProtectionProvider, 
            ILoggerFactory loggerFactory, 
            UrlEncoder encoder, 
            IOptions<SharedAuthenticationOptions> sharedOptions, 
            IOptions<TencentOptions> options) : base(next, dataProtectionProvider, loggerFactory, encoder, sharedOptions, options)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            if (sharedOptions == null)
            {
                throw new ArgumentNullException(nameof(sharedOptions));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _loggerFactory = loggerFactory;
        }

        protected override AuthenticationHandler<TencentOptions> CreateHandler()
        {
            return new TencentHandler(this.Backchannel, _loggerFactory);
        }
    }
}
