namespace HomeBoxQrGenerator.Core.Models {
    /// <summary>
    /// Generic base options to be used with any QR code generator
    /// </summary>
    public abstract class GeneratorOptions {
        #region Public properties
        /// <summary>
        /// The credentials to use to authenticate with homebox
        /// </summary>
        public required Credentials Credentials { get; init; }

        /// <summary>
        /// Changes the overall size of the output image
        /// </summary>
        public int Scale { get; init; } = 2;

        /// <summary>
        /// The server to use to generate label information from
        /// </summary>
        public required Uri Server { get; init; }
        #endregion
    }
}