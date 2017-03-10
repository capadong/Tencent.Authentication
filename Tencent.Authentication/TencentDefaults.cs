using System;
using System.Collections.Generic;
using System.Text;

namespace Tencent.Authentication
{
    public static class TencentDefaults
    {
        public const string AuthenticationScheme = "TencentQQ";

        public static readonly string AuthorizationEndpoint = "https://graph.qq.com/oauth2.0/authorize";

        public static readonly string TokenEndpoint = "https://graph.qq.com/oauth2.0/token";

        public static readonly string OpenIdEndpoint = "https://graph.qq.com/oauth2.0/me";

        public static readonly string UserInformationEndpoint = "https://graph.qq.com/user/get_user_info";
    }
}
