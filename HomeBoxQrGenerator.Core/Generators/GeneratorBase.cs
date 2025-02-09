using HomeBoxQrGenerator.Core.Models;

namespace HomeBoxQrGenerator.Core.Generators {
    /// <summary>
    /// Provides base functionality for generating labels
    /// </summary>
    /// <typeparam name="T">The type of options this generator takes</typeparam>
    public abstract class GeneratorBase<T> where T : GeneratorOptions {
        #region Public methods
        /// <summary>
        /// Generates a label with the given options
        /// </summary>
        /// <param name="options">The options to generate the label with</param>
        /// <returns>A stream which contains the generated label</returns>
        public abstract Task<Stream> GenerateAsync(T options);
        #endregion
    }
}