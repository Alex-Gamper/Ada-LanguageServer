using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdaLanguageServer
{

    [ContentType("Ada")]
    [Export(typeof(ILanguageClient))]
    public class AdaLanguageServer : ILanguageClient
    {
        public string Name => "Ada Language Server";
        public IEnumerable<string> ConfigurationSections => null;
        public object InitializationOptions => null;
        public IEnumerable<string> FilesToWatch => null;
        public event AsyncEventHandler<EventArgs> StartAsync;
        public event AsyncEventHandler<EventArgs> StopAsync;

        public async Task<Connection> ActivateAsync(CancellationToken token)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "lsp-ada_driver.exe";
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = info;

            if (process.Start())
            {
                return new Connection(process.StandardOutput.BaseStream, process.StandardInput.BaseStream);
            }

            return null;
        }

        public async Task OnLoadedAsync()
        {
            await StartAsync.InvokeAsync(this, EventArgs.Empty);
        }

        public Task OnServerInitializeFailedAsync(Exception e)
        {
            return Task.CompletedTask;
        }

        public Task OnServerInitializedAsync()
        {
            return Task.CompletedTask;
        }
    }

    public class AdaContentDefinition
    {
        [Export]
        [Name("Ada")]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static ContentTypeDefinition AdaContentTypeDefinition;

        [Export]
        [FileExtension(".ads")]
        [ContentType("Ada")]
        internal static FileExtensionToContentTypeDefinition AdsFileExtensionDefinition;

        [Export]
        [FileExtension(".adb")]
        [ContentType("Ada")]
        internal static FileExtensionToContentTypeDefinition AdbFileExtensionDefinition;

        [Export]
        [FileExtension(".ada")]
        [ContentType("Ada")]
        internal static FileExtensionToContentTypeDefinition AdaFileExtensionDefinition;

        [Export]
        [FileExtension(".gpr")]
        [ContentType("Ada")]
        internal static FileExtensionToContentTypeDefinition GprFileExtensionDefinition;
    }
}
