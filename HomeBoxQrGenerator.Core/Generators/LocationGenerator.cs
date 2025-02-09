using HomeBoxQrGenerator.Core.Client;
using HomeBoxQrGenerator.Core.Models;

namespace HomeBoxQrGenerator.Core.Generators {
    public class LocationGenerator : GeneratorBase<LocationGeneratorOptions> {
        #region Public methods
        public override async Task<Stream> GenerateAsync(LocationGeneratorOptions options) {
            await using (var conn = new HomeBoxConnection(options.Server, options.Credentials)) {
                await conn.ConnectAsync();

                var location = await conn.Client.LocationsGETAsync(options.LocationId.ToString());
                if (location.StatusCode == 200) {
                    return this.GenerateQrCode(new Uri($"{options.Server}location/{location.Result.Id}"), options, location.Result.Name);
                }

                throw new ApplicationException($"The server returned '{location.StatusCode}'");
            }
        }
        #endregion
    }
}