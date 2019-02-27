using DND.Core;
using DND.Domain.CMS.ContentHtmls;
using System;
using System.Linq;

namespace DND.Data
{
    public class DbSeed
    {
        //This will run each time the application launches so make you check to see if data already exists in db.
        public static void Seed(AppContext context)
        {
            AddContentHtml(context);
        }

        private static void AddContentText(AppContext context)
        {

        }

        private static void AddContentHtml(AppContext context)
        {
            AddContentHTML(context, CMS.ContentHtml.About, "<p>About Me</p>");
            AddContentHTML(context, CMS.ContentHtml.SideBarAbout, "<p>About Me</p>");
            AddContentHTML(context, CMS.ContentHtml.WorkWithMe, "<p>Work With Me</p>");
            AddContentHTML(context, CMS.ContentHtml.MyWebsite, "<p>My Website</p>");
            AddContentHTML(context, CMS.ContentHtml.Affiliates, "<p>Affiliates</p>");
            AddContentHTML(context, CMS.ContentHtml.Resume, "<p>Resume</p>");
            AddContentHTML(context, CMS.ContentHtml.Contact, "<p>Contact</p>");
            AddContentHTML(context, CMS.ContentHtml.Head, "");
            AddContentHTML(context, CMS.ContentHtml.Main, "");
            AddContentHTML(context, CMS.ContentHtml.PrivacyPolicy, "");
        }

        private static void AddContentHTML(AppContext context, string id, string content)
        {
            if (!context.ContentHtml.Any(c => c.Id == id))
            {
                context.ContentHtml.Add(new ContentHtml() { Id = id, HTML = content, PreventDelete = true, CreatedOn = DateTime.Now, CreatedBy = "SYSTEM" });
            }
        }
    }
}
