using System.Net.Http.Headers;
using HomeBoxQrGenerator.Core.Models;

namespace HomeBoxQrGenerator.Core.Client {
    public class HomeBoxConnection : IDisposable, IAsyncDisposable {
        #region Private readonly variables
        private readonly Credentials _creds;
        #endregion

        #region Public properties
        public HomeBoxClient Client { get; init; }
        #endregion

        #region Constructors
        public HomeBoxConnection(Uri host, Credentials creds) {
            this._creds = creds;
            //Create a HttpClient to connect to homebox with
            var http = new HttpClient();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var client = new HomeBoxClient(http);

            client.BaseUrl = $"{host.AbsoluteUri}api";

            this.Client = client;
        }
        #endregion

        #region Public methods
        public async Task ConnectAsync() {
            //Login to homebox
            var loginForm = new LoginForm { Username = this._creds.Username, Password = this._creds.Password, StayLoggedIn = false };

            var login = await this.Client.LoginAsync(loginForm);
            //Try to check if the login actually worked
            if (login is null || login.Result is null) {
                throw new InvalidOperationException();
            }
        }
        #endregion

        #region IAsyncDisposable Members
        public async ValueTask DisposeAsync() {
            await this.Client.LogoutAsync();
        }
        #endregion

        #region IDisposable Members
        public void Dispose() {
            this.Client.LogoutAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        #endregion
    }
}