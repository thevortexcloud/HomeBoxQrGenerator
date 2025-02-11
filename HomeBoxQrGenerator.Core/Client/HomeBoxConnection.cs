using System.Net.Http.Headers;
using HomeBoxQrGenerator.Core.Models;

namespace HomeBoxQrGenerator.Core.Client {
    /// <summary>
    /// Represents a "connection" to a HomeBox instance
    /// </summary>
    /// <remarks>
    /// HomeBox's API is just a simple HTTP REST API. But it does some weird things such as
    /// requiring explicit login and log out. So this just wraps it to make it easier to manage
    /// </remarks>
    public class HomeBoxConnection : IDisposable, IAsyncDisposable {
        #region Private readonly variables
        private readonly Credentials _creds;
        #endregion

        #region Public properties
        /// <summary>
        /// The client which exposes functionality for talking to HomeBox
        /// </summary>
        /// <remarks>This should not be used without first calling <see cref="ConnectAsync"/></remarks>
        public HomeBoxClient Client { get; init; }
        #endregion

        #region Constructors
        /// <summary>
        /// Allows for easily establishing a connection to a HomeBox instance
        /// </summary>
        /// <param name="host">The location of the HomeBox instance</param>
        /// <param name="creds">The credentials used to authenticate with the instance</param>
        public HomeBoxConnection(Uri host, Credentials creds) {
            this._creds = creds;
            //Create a HttpClient to connect to HomeBox with
            var http = new HttpClient();
            //We only speak JSON here
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var client = new HomeBoxClient(http);

            //Set the base URL on the actual client so calls work
            client.BaseUrl = $"{host.AbsoluteUri}api";

            this.Client = client;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Logins in to the currently configured instance
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if login was not successful</exception>
        public async Task ConnectAsync() {
            //Login to HomeBox
            var loginForm = new LoginForm {
                Username = this._creds.Username,
                Password = this._creds.Password,
                StayLoggedIn = false
            };

            var login = await this.Client.LoginAsync(loginForm);
            //Try to check if the login actually worked
            if (login is null || login.Result is null) {
                throw new InvalidOperationException();
            }
        }
        #endregion

        #region IAsyncDisposable Members
        /// <inheritdoc />
        public async ValueTask DisposeAsync() {
            await this.Client.LogoutAsync();
        }
        #endregion

        #region IDisposable Members
        /// <inheritdoc />
        public void Dispose() {
            this.Client.LogoutAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        #endregion
    }
}