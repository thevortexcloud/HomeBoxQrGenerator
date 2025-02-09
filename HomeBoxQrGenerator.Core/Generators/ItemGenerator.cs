using HomeBoxQrGenerator.Core.Client;
using HomeBoxQrGenerator.Core.Models;

namespace HomeBoxQrGenerator.Core.Generators {
    /// <summary>
    /// Generates label for an item
    /// </summary>
    public class ItemGenerator : GeneratorBase<ItemGeneratorOptions> {
        #region Public methods
        /// <inheritdoc />
        public override async Task<Stream> GenerateAsync(ItemGeneratorOptions options) {
            await using (var conn = new HomeBoxConnection(options.Server, options.Credentials)) {
                await conn.ConnectAsync();
                string? itemName = null;

                //Check if we are searching for an item or looking up a specific item. If we are doing a specific item
                //we need to just look up the item name. If we are searching we need to look up the id and name
                if (Guid.TryParse(options.ItemSearchQuery, out var itemId)) {
                    var item = await conn.Client.ItemsGET2Async(options.ItemSearchQuery);
                    itemName = item.Result.Name;
                } else {
                    //We only support generating a label for a single item at a time
                    var searchResult = await conn.Client.ItemsGETAsync(options.ItemSearchQuery);
                    if (searchResult.Result.Items.Count != 1) {
                        throw new ArgumentException($"No matches/Too many matches were found for '{options.ItemSearchQuery}'. Either use the id directly or use a more specific search option");
                    }

                    var item = searchResult.Result.Items.First();
                    itemName = item.Name;
                    itemId = Guid.Parse(item.Id);
                }


                var text = itemName;
                return this.GenerateQrCode(new Uri($"{options.Server}/item/{itemId}"), options, text);
            }
        }
        #endregion
    }
}