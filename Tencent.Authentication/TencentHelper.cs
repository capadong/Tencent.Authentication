using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tencent.Authentication
{
    public static class TencentHelper
    {
        public static string GetNickName(JObject user) {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("nickname");
        }
    }
}
