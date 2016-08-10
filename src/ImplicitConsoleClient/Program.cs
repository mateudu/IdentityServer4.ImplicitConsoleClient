using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImplicitConsoleClient
{
    public class Program
    {
        public const string IdentityServerHostUrl = @"http://localhost:5000";
        public const string ClientId = @"spa";
        public const string RedirectUri = @"http://localhost:5000/spa/callback.html";
        public const string ResponseType = @"id_token token";
        public const string Scope = @"openid profile api.todo";
        public static string State = Guid.NewGuid().GetGuidString();
        public static string Nonce = Guid.NewGuid().GetGuidString();

        public static void Main(string[] args)
        {
            var client = new ImplicitClient(IdentityServerHostUrl, ClientId, RedirectUri, ResponseType, Scope, State, Nonce);
            var result = client.GetResponseTokenAsync(@"username@example.com", @"P@ssw0rd").Result;
        }
    }

    public static class Extensions
    {
        public static string GetGuidString(this Guid guid)
        {
            string result = "";
            foreach (var c in guid.ToString().ToLowerInvariant())
            {
                if (c != '-')
                {
                    result += c;
                }
            }
            return result;
        }
    }
}
