using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Tencent.Authentication;

namespace Microsoft.AspNetCore.Builder
{
    public class TencentOptions : OAuthOptions
    {
        public TencentOptions()
        {
            this.AuthenticationScheme = TencentDefaults.AuthenticationScheme;
            this.AuthorizationEndpoint = TencentDefaults.AuthorizationEndpoint;
            this.UserInformationEndpoint = TencentDefaults.UserInformationEndpoint;
            this.TokenEndpoint = TencentDefaults.TokenEndpoint;
            this.DisplayName = "QQ";
            this.CallbackPath = new PathString("/signin-qq");
            this.OpenIdEndpoint = TencentDefaults.OpenIdEndpoint;
            //集成identity
            this.SignInScheme = "Identity.External";
          
            Scope.Add("get_user_info");
            //Scope.Add("list_album");
            //Scope.Add("upload_pic");
            //Scope.Add("do_like");
        }

        /// <summary>
        /// 仅PC网站接入时使用。 
        /// 用于展示的样式。不传则默认展示为PC下的样式。
        /// 如果传入“mobile”，则展示为mobile端下的样式。
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        /// 可选
        /// 仅WAP网站接入时使用。 
        /// QQ登录页面版本（1：wml版本； 2：xhtml版本），默认值为1。
        /// </summary>
        public string Gut { get; set; }

        public string OpenIdEndpoint { get; set; }
    }
}
