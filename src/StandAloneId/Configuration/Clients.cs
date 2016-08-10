using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace StandAloneId.Configuration
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "spa",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:5000/spa/callback.html"
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid", "profile",
                        "api.todo"
                    }
                },
                new Client
                {
                    ClientId = "native",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("mySecret".Sha256())
                    },
                    AllowAccessToAllScopes = true,
                    RequireConsent = false,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:5000/"
                    }
                },
                new Client
                {
                    ClientId = "hybrid.todo",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("mySecret".Sha256())
                    },
                    AllowAccessToAllScopes = true,
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:5000/",
                        "http://www.lubcycle.pl/"
                    }
                }
            };
        }
    }
}
