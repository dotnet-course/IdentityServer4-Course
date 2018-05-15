// Copyright (c) Jeffcky <see cref="http://www.cnblogs.com/createmyself"/> All rights reserved.
using System.Collections.Generic;

namespace IdentityServer4Demo.Models
{
    public class LoginViewModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; set; }
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
    }

    public class ExternalProvider
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }
    }
}
