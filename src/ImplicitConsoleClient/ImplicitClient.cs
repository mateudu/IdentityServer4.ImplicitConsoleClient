using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImplicitConsoleClient
{
    public class ImplicitClient
    {
        private string IdentityServerHostUrl { get; set; }
        private string ClientId { get; set; }
        private string RedirectUri { get; set; }
        private string ResponseType { get; set; }
        private string Scope { get; set; }
        private string State { get; set; }
        private string Nonce { get; set; }

        private string _authorizationPageUrl = null;
        private string loginPageUrl = null;
        private HttpResponseMessage _loginPageResponse = null;


        private string AuthorizationPageUrl
        {
            get
            {
                return _authorizationPageUrl ??
                       (_authorizationPageUrl =
                           _authorizationPageUrl = $"{IdentityServerHostUrl}/connect/authorize?client_id={ClientId}"
                                                  + $"&redirect_uri={RedirectUri}&response_type={ResponseType}"
                                                  + $"&scope={Scope}&state={State}&nonce={Nonce}");
            }
        }

        private HttpClient client = new HttpClient();

        public ImplicitClient(string identityServerHostUrl, string clientId, string redirectUri, string responseType,
            string scope, string state, string nonce)
        {
            this.IdentityServerHostUrl = identityServerHostUrl;
            this.ClientId = clientId;
            this.RedirectUri = redirectUri;
            this.ResponseType = responseType;
            this.Scope = scope;
            this.State = state;
            this.Nonce = nonce;
        }

        private async Task<HttpResponseMessage> GetLoginPage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(AuthorizationPageUrl));
            var result = await client.SendAsync(request);
            return _loginPageResponse = result;
        }

        public async Task<ResponseToken> GetResponseTokenAsync(string email, string password)
        {
            if (_loginPageResponse == null)
            {
                _loginPageResponse = await GetLoginPage();
            }

            string requestUrl = GetResponseUrl(_loginPageResponse);
            string cookie = GetAspNetCookie(_loginPageResponse);
            string requestVerificationToken = GetRequestVerificationToken(_loginPageResponse);

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(requestUrl));
            request.Headers.Add("Cookie", cookie);
            var content = new FormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Email", email),
                        new KeyValuePair<string, string>("Password", password),
                        new KeyValuePair<string, string>("__RequestVerificationToken", requestVerificationToken)
                    });
            request.Content = content;

            var response = await client.SendAsync(request);
            var dictionary =
                Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(response.RequestMessage.RequestUri.Fragment);
            var result = new ResponseToken
            {
                AccessToken = dictionary["access_token"],
                IdToken = dictionary["#id_token"],
                TokenType = dictionary["token_type"]
            };
            return result;
        }


        // HELPERS
        private string GetAspNetCookie(HttpResponseMessage msg)
        {
            return msg.Headers.FirstOrDefault(header => header.Key == @"Set-Cookie").Value?.FirstOrDefault();
        }

        private string GetRequestVerificationToken(HttpResponseMessage msg)
        {
            var str = msg.Content.ReadAsStringAsync().Result;
            string query = @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""";
            str = str.Substring(str.IndexOf(query) + query.Length);
            str = str.Substring(0, str.IndexOf(@""""));
            return str;
        }

        private string GetResponseUrl(HttpResponseMessage msg)
        {
            return msg.RequestMessage.RequestUri.AbsoluteUri;
        }
    }

    public class ResponseToken
    {
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public string TokenType { get; set; }
    }
}