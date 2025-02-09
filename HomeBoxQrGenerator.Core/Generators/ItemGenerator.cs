using System.Net.Http.Headers;
using HomeBoxQrGenerator.Core.Client;
using HomeBoxQrGenerator.Core.Models;
using QRCoder;
using SkiaSharp;

namespace HomeBoxQrGenerator.Core.Generators {
    /// <summary>
    /// Generates label for an item
    /// </summary>
    public class ItemGenerator : GeneratorBase<ItemGeneratorOptions> {
        #region Public methods
        /// <inheritdoc />
        public override async Task<Stream> GenerateAsync(ItemGeneratorOptions options) {
            //Create a HttpClient to connect to homebox with
            var http = new HttpClient();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var client = new HomeBoxClient(http);

            client.BaseUrl = $"{options.Server.AbsoluteUri}api";

            //Login to homebox
            var loginForm = new LoginForm { Username = options.Credentials.Username, Password = options.Credentials.Password, StayLoggedIn = false };

            var login = await client.LoginAsync(loginForm);
            //Try to check if the login actually worked
            if (login is null || login.Result is null) {
                throw new InvalidOperationException();
            }

            try {
                string? itemName = null;

                //Check if we are searching for an item or looking up a specific item. If we are doing a specific item
                //we need to just look up the item name. If we are searching we need to look up the id and name
                if (Guid.TryParse(options.ItemSearchQuery, out var itemId)) {
                    var item = await client.ItemsGET2Async(options.ItemSearchQuery);
                    itemName = item.Result.Name;
                } else {
                    //We only support generating a label for a single item at a time
                    var searchResult = await client.ItemsGETAsync(options.ItemSearchQuery);
                    if (searchResult.Result.Items.Count != 1) {
                        throw new ArgumentException($"No matches/Too many matches were found for '{options.ItemSearchQuery}'. Either use the id directly or use a more specific search option");
                    }

                    var item = searchResult.Result.Items.First();
                    itemName = item.Name;
                    itemId = Guid.Parse(item.Id);
                }


                var text = itemName;

                //This section starts to generate the QR code
                using (var qrGenerator = new QRCodeGenerator()) {
                    //Generate the URL QR code 
                    using (var qrCodeData = qrGenerator.CreateQrCode($"{options.Server}/item/{itemId}", QRCodeGenerator.ECCLevel.Q)) {
                        using (var qrCode = new PngByteQRCode(qrCodeData)) {
                            var qrCodeImage = qrCode.GetGraphic(options.Scale);
                            //We use Skia to make some tweaks to the QR code (such as by putting text next to it)
                            using (var qrbitmap = SKBitmap.Decode(qrCodeImage)) {
                                using (var font = new SKFont(SKTypeface.FromFamilyName("Mono"), 20)) {
                                    using (var bitmap = new SKBitmap(300, qrbitmap.Height)) {
                                        using (var canvas = new SKCanvas(bitmap)) {
                                            //Make sure the background of the image is white to make it easier to read
                                            canvas.DrawColor(SKColor.Parse("FFFFFF"));

                                            var fontWidth = font.GetGlyphWidths(text);

                                            var fontXOffset = qrbitmap.Width + 10;
                                            var fontYOffset = 20;
                                            //If we run out of horizontal space, we need to do some math to work out how to wrap the text
                                            if (fontWidth.Sum() + fontXOffset > bitmap.Width) {
                                                //var requiredSpace = (fontWidth.Sum() + fontXOffset) - bitmap.Width;
                                                var firstPossibleBreak = font.BreakText(text, bitmap.Width - fontXOffset);
                                                var brokenLines = new List<string>();
                                                var firstSpace = text.LastIndexOf(" ", firstPossibleBreak);
                                                if (firstSpace != -1 && firstSpace <= firstPossibleBreak) {
                                                    //TODO: Currently this only supports word wrapping across two lines, even if more vertical space is actually available
                                                    brokenLines.Add(text[..firstSpace].Trim());
                                                    brokenLines.Add(text[firstSpace..].Trim());

                                                    //Draw each line offset from the previous line
                                                    foreach (var split in brokenLines) {
                                                        canvas.DrawText(split, fontXOffset, fontYOffset, SKTextAlign.Left, font, new SKPaint());

                                                        fontYOffset += 20;
                                                    }
                                                } else {
                                                    throw new NotSupportedException();
                                                }
                                            } else {
                                                //No wrapping required, we can just write the text directly
                                                canvas.DrawText(text, fontXOffset, fontYOffset, SKTextAlign.Left, font, new SKPaint());
                                            }

                                            canvas.DrawBitmap(qrbitmap, 10, 0);

                                            //Generate the final output and return it to the caller
                                            var resultStream = new MemoryStream();
                                            bitmap.Encode(SKEncodedImageFormat.Png, 100).AsStream().CopyTo(resultStream);
                                            resultStream.Position = 0;
                                            return resultStream;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } finally {
                await client.LogoutAsync();
            }
        }
        #endregion
    }
}