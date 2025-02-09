namespace HomeBoxQrGenerator.Core.Models {
    public class LocationGeneratorOptions : GeneratorOptions {
        #region Public properties
        /// <summary>
        /// The id of the location to generate a label for
        /// </summary>
        public required Guid LocationId { get; init; }
        #endregion
    }
}