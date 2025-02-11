namespace HomeBoxQrGenerator.Core.Models {
    /// <summary>
    /// Options used for generating item labels
    /// </summary>
    public class ItemGeneratorOptions : GeneratorOptions {
        #region Public properties
        /// <summary>
        /// A query or item id to use to locate an item on the server
        /// </summary>
        public required string ItemSearchQuery { get; init; }
        #endregion
    }
}