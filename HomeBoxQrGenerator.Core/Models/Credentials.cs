namespace HomeBoxQrGenerator.Core.Models {
    public class Credentials {
        #region Public properties
        /// <summary>
        /// The password to use to connect to HomeBox
        /// </summary>
        public required string Password { get; init; }

        /// <summary>
        /// The username to use to connect to HomeBox, likely an email address
        /// </summary>
        public required string Username { get; init; }
        #endregion
    }
}