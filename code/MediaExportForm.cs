using System;

using Sitecore.Collections;
using Sitecore.Web.UI.Sheer;

namespace SharedSource.MediaExporterModule
{
    public class MediaExportForm : Sitecore.Web.UI.Sheer.BaseForm
    {
        protected Sitecore.Web.UI.HtmlControls.Edit Filename;
        protected Sitecore.Web.UI.HtmlControls.Checkbox Recursive;

        protected void OnSubmitButton()
        {       
                SheerResponse.SetDialogValue(
                Filename.Value 
                + "|" + 
                (Recursive.Value == "1" ? "true" : "false") 
            );

            SheerResponse.CloseWindow();
        }
    }
}
