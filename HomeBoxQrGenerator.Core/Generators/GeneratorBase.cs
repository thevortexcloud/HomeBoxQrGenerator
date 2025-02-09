using HomeBoxQrGenerator.Core.Models;
using QRCoder;
using SkiaSharp;

namespace HomeBoxQrGenerator.Core.Generators {
    /// <summary>
    /// Provides base functionality for generating labels
    /// </summary>
    /// <typeparam name="T">The type of options this generator takes</typeparam>
    public abstract class GeneratorBase<T> where T : GeneratorOptions {
        #region Protected methods
        /// <summary>
        /// Generates a QR code for the given URI, options and with descriptive text to the right of the QR code
        /// </summary>
        /// <param name="uri">The URI to put into the QR code</param>
        /// <param name="options">Options which can influence how the code is generated</param>
        /// <param name="text">Descriptive text to put next to the QR code</param>
        /// <returns>A stream containing the QR code data</returns>
        /// <exception cref="NotSupportedException"></exception>
        protected Stream GenerateQrCode(Uri uri, T options, string text) {
            //This section starts to generate the QR code
            using (var qrGenerator = new QRCodeGenerator()) {
                //Generate the URL QR code 
                using (var qrCodeData = qrGenerator.CreateQrCode(uri.AbsoluteUri, QRCodeGenerator.ECCLevel.Q)) {
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
        }
        #endregion

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