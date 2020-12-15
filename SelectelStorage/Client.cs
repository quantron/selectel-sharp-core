﻿using System;
using System.Net;
using System.Threading.Tasks;
using SelectelStorage.Requests;

namespace SelectelStorage
{
    public class Client
    {
        public string StorageUrl { get; private set; }
        public string AuthToken { get; private set; }
        public long ExpireAuthToken { get; private set; }

        public Client() { }

        public Client(string proxyUrl = null, string proxyUser = null, string proxyPassword = null)
        {
            var proxy = new WebProxy(proxyUrl, true);
            proxy.Credentials = new NetworkCredential(proxyUser, proxyPassword);
            WebRequest.DefaultWebProxy = proxy;
        }

        public Client(WebProxy proxy = null)
        {
            WebRequest.DefaultWebProxy = proxy;
        }

        public async Task AuthorizeAsync(string user, string key)
        {
            var result = await ExecuteAsync(new AuthRequest(user, key));

            this.StorageUrl = result.StorageUrl;
            this.AuthToken = result.AuthToken;
            this.ExpireAuthToken = result.ExpireAuthToken;
        }

        public async Task<T> ExecuteAsync<T>(BaseRequest<T> request)
        {
            if (!request.AllowAnonymously)
            {
                CheckTokenNotNull();
            }

            await request.Execute(StorageUrl, AuthToken);
            return request.Result;
        }

        private void CheckTokenNotNull()
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                throw new Exception("You should first authorize this client. Call AuthorizeAsync method.");
            }
        }
    }
}
