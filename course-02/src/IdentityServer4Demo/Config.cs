// Copyright (c) Jeffcky <see cref="http://www.cnblogs.com/createmyself"/> All rights reserved.
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer4Demo
{
    public class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "70F3D0E7-1655-4727-827D-36D21C25A955",
                    ClientName = "MVCClient",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = new List<Secret> {
                        new Secret("5CDCCB776008457BBECC7CDE93BE4AA5".Sha256())},
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = false,
                    AllowedCorsOrigins = {"http://localhost:5001"},
                    RedirectUris = {"http://localhost:5001/signin-oidc" },
                    PostLogoutRedirectUris = {"http://localhost:5001/signout-callback-oidc" },
                    AccessTokenLifetime = 30,
                    IdentityTokenLifetime = 30,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "customMVCAPI.read",
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "customMVCAPI",
                    DisplayName = "IdentityServerDemoMVC API",
                    Description = "IdentityServerDemoMVC Access",
                    ApiSecrets = new List<Secret> {new Secret("AC6FB80A070244EE857BE07930D1D701".Sha256())},
                    Scopes = new List<Scope> {
                        new Scope("customMVCAPI.read"),
                        new Scope("customMVCAPI.write")
                    }
                }
            };
        }
    }
}
