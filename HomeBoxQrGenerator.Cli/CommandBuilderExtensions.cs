using System.CommandLine;
using System.CommandLine.Builder;
using HomeBoxQrGenerator.Core.Generators;
using HomeBoxQrGenerator.Core.Models;

namespace HomeBoxQrGenerator.Cli {
    public static class CommandBuilderExtensions {
        #region Private methods
        /// <summary>
        /// Handles a request to generate an item label
        /// </summary>
        /// <param name="options">The options to generate the item label</param>
        /// <param name="output">Where to place the file output of the generation</param>
        private static async Task HandleItemGenerationCommand(ItemGeneratorOptions options, FileInfo output) {
            using (var fs = output.Open(FileMode.Create)) {
                var generator = new ItemGenerator();
                using (var result = await generator.GenerateAsync(options)) {
                    await result.CopyToAsync(fs);
                }
            }
        }
        #endregion

        #region Public methods
        public static CommandLineBuilder AddGeneratorCommand(this CommandLineBuilder builder) {
            var root = builder.Command;
            var typeOption = new Option<string>("--type", () => "item", "Allows for generating QR codes for the specific type. Currently only supports 'item'") { IsRequired = true };
            var queryOption = new Option<string>("--query", "The query string. Can either be a search value or an id") { IsRequired = true };
            var hostOption = new Option<string>("--host", "The host address of the server") { IsRequired = true };
            var usernameOption = new Option<string>("--username", "The username used to login to the server") { IsRequired = true };
            var passwordOption = new Option<string>("--password", "The password used to login to the server") { IsRequired = true };
            var outputPathOption = new Option<FileInfo>("--output", "The output location to put the file");

            root.AddOption(typeOption);
            root.AddOption(queryOption);
            root.AddOption(hostOption);
            root.AddOption(usernameOption);
            root.AddOption(passwordOption);
            root.AddOption(outputPathOption);

            root.SetHandler(async (type, query, host, username, password, output) => {
                switch (type) {
                    case "item":
                        var itemOptions = new ItemGeneratorOptions {
                            Server = new Uri(host),
                            Credentials = new Credentials {
                                Username = username,
                                Password = password
                            },
                            ItemSearchQuery = query
                        };

                        await HandleItemGenerationCommand(itemOptions, output);
                        break;
                    default: throw new NotImplementedException();
                }
            }, typeOption, queryOption, hostOption, usernameOption, passwordOption, outputPathOption);
            return builder;
        }
        #endregion
    }
}