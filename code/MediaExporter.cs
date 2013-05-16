using System;
using System.IO;
using System.Threading;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Zip;

namespace SharedSource.MediaExporterModule
{
    public class MediaExporter : IDisposable
    {
        public string File { get; private set; }
        protected ZipWriter ZipWriter { get; private set; }

        public MediaExporter(string file)
        {
            File = file;    
            ZipWriter = new ZipWriter(file);
        }

        public void Dispose()
        {
            ZipWriter.Dispose();
        }

        public void ProcessMediaItems(Item rootMediaItem, bool recursive)
        {
            if (rootMediaItem.TemplateID != TemplateIDs.MediaFolder 
                && rootMediaItem.TemplateID != TemplateIDs.MainSection)
            {
                // Update the job status
                Context.Job.Status.Processed++;
                Context.Job.Status.Messages.Add(
                    "Processed " + 
                    Context.Job.Status.Processed +
                    " items");

                var mediaItem = new MediaItem(rootMediaItem);
                AddMediaItemToZip(mediaItem);
            }
            else if (recursive)
            {
                foreach (Item item in rootMediaItem.GetChildren())
                {
                    ProcessMediaItems(item, true);
                }
            }
        }

        private void AddMediaItemToZip(MediaItem mediaItem)
        {
            Assert.ArgumentNotNull(mediaItem, "mediaItem");

            Stream stream = mediaItem.GetMediaStream();
            if (stream == null)
            {
                Log.Warn(string.Format("Cannot find media data for item '{0}'", 
                    mediaItem.MediaPath), 
                    typeof(object));
                return;
            }

            string mediaExtension = string.IsNullOrEmpty(mediaItem.Extension) 
                ? "" 
                : "." + mediaItem.Extension;

            ZipWriter.AddEntry(
                mediaItem.MediaPath.Substring(1) + 
                mediaItem.Name + 
                mediaExtension, stream);
        }
    }
}
