using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Tencent.Authentication
{
    public class TencentHandler : OAuthHandler<TencentOptions>
    {
        ILogger<TencentHandler> _logger;

        public TencentHandler(HttpClient backchannel, ILoggerFactory loggerFactory)
            : base(backchannel)
        {
            _logger = loggerFactory.CreateLogger<TencentHandler>();
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            var openIdParameters = new Dictionary<string, string>()
            {
                { "access_token", tokens.AccessToken}
            };

            var getUserInfoParameters = new Dictionary<string, string>()
            {
                { "access_token",tokens.AccessToken}
            };

            var requestUrl = QueryHelpers.AddQueryString(Options.OpenIdEndpoint, openIdParameters);
            var response = await this.Backchannel.GetAsync(requestUrl, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"An error occurred when retrieving user information ({response.StatusCode}). Please check if the authentication information is correct and the corresponding Google+ API is enabled.");
            }
            var contentRegex = new Regex(@"callback\((.*)\);", RegexOptions.Compiled);
            var content = await response.Content.ReadAsStringAsync();
            var match = contentRegex.Match(content);

            if (!match.Success)
            {
                var msg = $"获取openid错误,content:{content}";
                _logger.LogError(msg);
                throw new HttpRequestException("获取openid错误");
            }

            var payload = JObject.Parse(match.Groups[1].Value);
            //{“client_id”:”YOUR_APPID”,”openid”:”YOUR_OPENID”}
            var clientId = payload.Value<string>("client_id");
            var openid = payload.Value<string>("openid");

            getUserInfoParameters.Add("oauth_consumer_key", clientId);
            getUserInfoParameters.Add("openid", openid);

            var userInfoRequestUrl = QueryHelpers.AddQueryString(Options.UserInformationEndpoint,
                getUserInfoParameters);

            var userInfoRsp = await this.Backchannel.GetAsync(userInfoRequestUrl, Context.RequestAborted);

            var userInfoPayload = JObject.Parse(await userInfoRsp.Content.ReadAsStringAsync());

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, properties, Options.AuthenticationScheme);
            var context = new OAuthCreatingTicketContext(ticket, Context, Options, Backchannel, tokens, userInfoPayload);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, openid));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, userInfoPayload.Value<string>("nickname")));
            identity.AddClaim(new Claim(ClaimTypes.Gender, userInfoPayload.Value<string>("gender")));


            await Options.Events.CreatingTicket(context);

            return context.Ticket;
        }

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "client_id", Options.ClientId },
                { "redirect_uri", redirectUri },
                { "client_secret", Options.ClientSecret },
                { "code", code },
                { "grant_type", "authorization_code" },
            };

            var requestUrl = QueryHelpers.AddQueryString(Options.TokenEndpoint,
                tokenRequestParameters);

            var response = await this.Backchannel.GetAsync(requestUrl, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return OAuthTokenResponse.Success(ParseQuery(content));
            }
            else
            {
                var error = "OAuth token endpoint failure";
                _logger.LogError(error);
                return OAuthTokenResponse.Failed(new Exception(error));
            }
        }

        private JObject ParseQuery(string query)
        {
            var jObject = new JObject();

            foreach (var kv in query.Split('&'))
            {
                var keyValue = kv.Split('=');

                jObject.Add(keyValue[0], new JValue(keyValue[1]));
            }

            return jObject;
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            var queryStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            queryStrings.Add("response_type", "code");
            queryStrings.Add("client_id", Options.ClientId);
            queryStrings.Add("redirect_uri", redirectUri);

            AddQueryString(queryStrings, properties, "scope", FormatScope());
            AddQueryString(queryStrings, properties, "display");
            AddQueryString(queryStrings, properties, "g_ut");

            var state = Options.StateDataFormat.Protect(properties);
            queryStrings.Add("state", state);

            var authorizationEndpoint = QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, queryStrings);
            return authorizationEndpoint;
        }

        private static void AddQueryString(
            IDictionary<string, string> queryStrings,
            AuthenticationProperties properties,
            string name,
            string defaultValue = null)
        {
            string value;
            if (!properties.Items.TryGetValue(name, out value))
            {
                value = defaultValue;
            }
            else
            {
                // Remove the parameter from AuthenticationProperties so it won't be serialized to state parameter
                properties.Items.Remove(name);
            }

            if (value == null)
            {
                return;
            }

            queryStrings[name] = value;
        }

        protected override string FormatScope()
        {
            return string.Join(",", Options.Scope);
        }
    }
}
