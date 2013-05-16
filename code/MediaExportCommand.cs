using System.Collections.Specialized;

using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Resources;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

namespace SharedSource.MediaExporterModule
{
    public class MediaExportCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                var parameters = new NameValueCollection();
                parameters["uri"] = item.Uri.ToString();
                Context.ClientPage.Start(this, "Run", parameters);
            }
        }

        protected virtual void Run(ClientPipelineArgs args)
        {
            var uri = ItemUri.Parse(args.Parameters["uri"]);
            Item item = Database.GetItem(uri);
            Error.AssertItemFound(item);

            if (!args.IsPostBack)
            {
                UrlString urlString = ResourceUri.Parse("control:ExportMedia").ToUrlString();
                uri.AddToUrlString(urlString);
                SheerResponse.ShowModalDialog(urlString.ToString(), "280", "220", "", true);
                args.WaitForPostBack();
                return;
            }

            if (string.IsNullOrEmpty(args.Result) || args.Result == "undefined")
                return;

            string[] paramsArray = args.Result.Split('|');
            string exportFileName = paramsArray[0];

            if (string.IsNullOrWhiteSpace(exportFileName))
                return;

            bool recursive = extractRecursiveParam(paramsArray);

            string exportfolderName = Settings.DataFolder + "/" +
                                      Settings.GetSetting("SharedSource.MediaExporterModule.ExportFolderName");

            string exportFileNameWithExtension = exportFileName.EndsWith(".zip") ? exportFileName : exportFileName + ".zip";

            FileUtil.CreateFolder(FileUtil.MapPath(exportfolderName));
        
            string zipPath = FileUtil.MapPath(FileUtil.MakePath(exportfolderName,
                                              exportFileNameWithExtension,
                                              '/'));

            Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBox.Execute(
                "Export Media Items...",
                "Export Media Items",
                new Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBoxMethod(StartProcess),
                new[] { item as object, zipPath, recursive });

            Context.ClientPage.ClientResponse.Download(zipPath);
        }

        private bool extractRecursiveParam(string[] paramsArray)
        {
            bool recursive;
            if (paramsArray.Length > 1)
            {
                bool succeeded = bool.TryParse(paramsArray[1], out recursive);
                if (!succeeded)
                {
                    recursive = true;
                }
            }
            else
            {
                recursive = true;
            }

            return recursive;
        }

        public override CommandState QueryState(CommandContext context)
        {
            Error.AssertObject(context, "context");
            if (context.Items.Length != 1)
            {
                return CommandState.Hidden;
            }
            Item item = context.Items[0];
            if (item == null)
            {
                return CommandState.Hidden;
            }
            return base.QueryState(context);
        }

        public void StartProcess(params object[] parameters)
        {
            var item = (Item) parameters[0];
            var zipPath = (string) parameters[1];
            var recursive = (bool) parameters[2];

            var exporter = new MediaExporter(zipPath);
            foreach (Item i in item.GetChildren())
            { 
                exporter.ProcessMediaItems(i, recursive);   
            }
            exporter.Dispose();
        }
    }
}
