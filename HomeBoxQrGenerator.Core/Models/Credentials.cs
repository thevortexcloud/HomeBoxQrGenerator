namespace HomeBoxQrGenerator.Core.Models {
    public class Credentials {
        #region Public properties
        /// <summary>
        /// The password to use to connect to Homebox
        /// </summary>
        public required string Password { get; init; }

        /// <summary>
        /// The username to use to connect to Homebox, likely an email address
        /// </summary>
        public required string Username { get; init; }
        #endregion
    }
}