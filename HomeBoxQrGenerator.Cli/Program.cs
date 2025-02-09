using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace HomeBoxQrGenerator.Cli {
    internal class Program {
        #region Public methods
        public static async Task<int> Main(string[] args) {
            try {
                var code = await new CommandLineBuilder()
                    .UseDefaults()
                    .AddGeneratorCommand()
                    .Build()
                    .InvokeAsync(args);
                return code;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
        #endregion
    }
}