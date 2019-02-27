using AspNetCore.Base.APIs;
using AspNetCore.Base.Extensions;
using GeoAPI.Geometries;
using HtmlTags;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;


namespace AspNetCore.Base.MvcExtensions
{
    public static class HtmlHelperExtensions
    {
        public static T GetInstance<T>(this IHtmlHelper html)
        {
            return (T)html.ViewContext.HttpContext.RequestServices.GetService(typeof(T));
        }

        public static object GetInstance(this IHtmlHelper html, Type type)
        {
            return html.ViewContext.HttpContext.RequestServices.GetService(type);
        }

        public static HtmlString ClientIdFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return new HtmlString(
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression)));
        }

        public static string ToString(IHtmlContent content)
        {
            var writer = new System.IO.StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static HtmlString IconLink(this IHtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, String iconName, object htmlAttributes = null)
        {
            var linkMarkup = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes).Render().Replace("%2F", "/");
            var iconMarkup = String.Format("<span class=\"{0}\" aria-hidden=\"true\"></span> ", iconName);
            return new HtmlString(linkMarkup.Insert(linkMarkup.IndexOf(@">") + 1, iconMarkup));
        }

        public static HtmlString IconLink(this IHtmlHelper htmlHelper, string linkText, string actionName, object routeValues, String iconName, object htmlAttributes = null)
        {
            var linkMarkup = htmlHelper.ActionLink(linkText, actionName, routeValues, htmlAttributes).Render().Replace("%2F", "/");
            var iconMarkup = String.Format("<span class=\"{0}\" aria-hidden=\"true\"></span> ", iconName);
            return new HtmlString(linkMarkup.Insert(linkMarkup.IndexOf(@">") + 1, iconMarkup));
        }

        public static HtmlString Grid(this IHtmlHelper<dynamic> html, Boolean details, Boolean edit, Boolean delete, Boolean sorting, Boolean entityActions)
        {
            var div = new TagBuilder("div");
            div.AddCssClass("table-responsive");

            var table = new TagBuilder("table");
            table.AddCssClass("table");
            table.AddCssClass("table-striped");
            table.AddCssClass("table-sm");

            var thead = new TagBuilder("thead");

            var theadTr = new TagBuilder("tr");

            Dictionary<string, List<string>> actions = new Dictionary<string, List<string>>();

            //if (entityActions)
            //{

            //    var actionEvents = new ActionEvents();
            //    actions = actionEvents.GetActionsForDto(html.ViewData.ModelMetadata().ModelType);
            //    entityActions = actions.Count > 0;
            //}

            Boolean hasActions = (details || edit || delete || entityActions);

            var provider = html.GetInstance<IModelMetadataProvider>();

            foreach (var prop in html.ViewData.ModelMetadata(provider).Properties
            .Where(p => (p.ShowForDisplay && !(p.AdditionalValues.ContainsKey("ShowForGrid")) || (p.AdditionalValues.ContainsKey("ShowForGrid") && (Boolean)p.AdditionalValues["ShowForGrid"]))))
            {
                var th = new TagBuilder("th");

                var orderType = "desc";
                if (html.ViewBag.OrderColumn.ToLower() == prop.PropertyName.ToLower() && html.ViewBag.OrderType != "asc")
                {
                    orderType = "asc";
                }


                string linkText = ModelHelperExtensions.DisplayName(html.ViewData.Model, prop.PropertyName, provider).ToString();
                string link;

                Boolean enableSort = sorting;
                if (prop.AdditionalValues.ContainsKey("AllowSortForGrid"))
                {
                    enableSort = (Boolean)prop.AdditionalValues["AllowSortForGrid"];
                }

                if (enableSort)
                {
                    if (html.ViewBag.Collection != null)
                    {
                        link = html.ActionLink(linkText, "Collection", new { id = html.ViewBag.Id, collection = html.ViewBag.Collection, page = html.ViewBag.Page, pageSize = html.ViewBag.PageSize, search = html.ViewBag.Search, orderColumn = prop.PropertyName, orderType = orderType }).Render().Replace("%2F", "/");
                    }
                    else
                    {
                        link = html.ActionLink(linkText, "Index", new { page = html.ViewBag.Page, pageSize = html.ViewBag.PageSize, search = html.ViewBag.Search, orderColumn = prop.PropertyName, orderType = orderType }).Render();
                    }
                }
                else
                {
                    link = linkText;
                }

                // th.InnerHtml = ModelHelperExtensions.DisplayName(html.ViewData.Model, prop.PropertyName).ToString();
                th.InnerHtml.AppendHtml(link);

                theadTr.InnerHtml.AppendHtml(th);
            }

            if (hasActions)
            {
                var thActions = new TagBuilder("th");
                theadTr.InnerHtml.AppendHtml(thActions);
            }

            thead.InnerHtml.AppendHtml(theadTr);

            table.InnerHtml.AppendHtml(thead);

            var tbody = new TagBuilder("tbody");

            foreach (var item in html.ViewData.Model)
            {
                var tbodytr = new TagBuilder("tr");

                foreach (var prop in html.ViewData.ModelMetadata(provider).Properties
               .Where(p => (p.ShowForDisplay && !(p.AdditionalValues.ContainsKey("ShowForGrid")) || (p.AdditionalValues.ContainsKey("ShowForGrid") && (Boolean)p.AdditionalValues["ShowForGrid"]))))
                {
                    var td = new TagBuilder("td");

                    var propertyType = GetNonNullableModelType(prop);
                    var linkToCollection = propertyType.IsCollection() && prop.AdditionalValues.ContainsKey("LinkToCollectionInGrid") && (Boolean)prop.AdditionalValues["LinkToCollectionInGrid"];


                    //Folder, File, Dropdown, Repeater
                    if (prop.AdditionalValues.ContainsKey("IsDatabound"))
                    {
                        if (linkToCollection)
                        {
                            string linkText = "";
                            if (prop.AdditionalValues.ContainsKey("LinkText"))
                            {
                                linkText = (string)prop.AdditionalValues["LinkText"];
                            }

                            if (string.IsNullOrWhiteSpace(linkText))
                            {
                                linkText = HtmlHelperExtensions.ToString(ModelHelperExtensions.Display(html, item, prop.PropertyName));
                            }

                            var collectionLink = html.ActionLink(linkText, "Collection", new { id = item.Id, collection = html.ViewBag.Collection != null ? html.ViewBag.Collection + "/" + prop.PropertyName.ToLower() : prop.PropertyName.ToLower() }).Render().Replace("%2F", "/");
                            HtmlContentBuilderExtensions.SetHtmlContent(td.InnerHtml, collectionLink);
                        }
                        else
                        {
                            if (prop.AdditionalValues.ContainsKey("ActionName"))
                            {
                                var linkText = (string)prop.AdditionalValues["LinkText"];
                                if (string.IsNullOrWhiteSpace(linkText))
                                {
                                    linkText = HtmlHelperExtensions.ToString(ModelHelperExtensions.Display(html, item, prop.PropertyName));
                                }
                                var actionName = (string)prop.AdditionalValues["ActionName"];
                                var controllerName = (string)prop.AdditionalValues["ControllerName"];

                                RouteValueDictionary routeValues = null;
                                if (prop.AdditionalValues.ContainsKey("RouteValueDictionary"))
                                {
                                    routeValues = (RouteValueDictionary)prop.AdditionalValues["RouteValueDictionary"];
                                    SetRouteValues(html, item, routeValues);
                                }

                                string collectionLink = "";
                                if (routeValues == null)
                                {
                                    collectionLink = html.ActionLink(linkText, actionName, controllerName).Render().Replace("%2F", "/");
                                }
                                else
                                {
                                    collectionLink = html.ActionLink(linkText, actionName, controllerName, routeValues).Render().Replace("%2F", "/");
                                }

                                HtmlContentBuilderExtensions.SetHtmlContent(td.InnerHtml, collectionLink);
                            }
                            else
                            {
                                HtmlContentBuilderExtensions.SetHtmlContent(td.InnerHtml, ModelHelperExtensions.Display(html, item, prop.PropertyName));
                            }
                        }
                    }
                    else if (prop.ModelType == typeof(FileInfo))
                    {
                        HtmlContentBuilderExtensions.SetHtmlContent(td.InnerHtml, ModelHelperExtensions.Display(html, item, prop.PropertyName));
                    }
                    else if (prop.ModelType == typeof(Point))
                    {
                        var model = (Point)item.GetType().GetProperty(prop.PropertyName).GetValue(item, null);
                        if (model != null && model.Y != 0 && model.X != 0)
                        {
                            string value = model.Y.ToString("G", CultureInfo.InvariantCulture) + "," + model.X.ToString("G", CultureInfo.InvariantCulture);
                            td.InnerHtml.Append(value);
                        }
                    }
                    else
                    {
                        //String
                        if (linkToCollection)
                        {
                            string linkText = "";
                            if (prop.AdditionalValues.ContainsKey("LinkText"))
                            {
                                linkText = (string)prop.AdditionalValues["LinkText"];
                            }

                            if (string.IsNullOrWhiteSpace(linkText))
                            {
                                linkText = ModelHelperExtensions.DisplayTextSimple(html, item, prop.PropertyName).ToString();
                            }
                            var collectionLink = html.ActionLink(linkText, "Collection", new { id = item.Id, collection = html.ViewBag.Collection != null ? html.ViewBag.Collection + "/" + prop.PropertyName.ToLower() : prop.PropertyName.ToLower() }).Render().Replace("%2F", "/");
                            HtmlContentBuilderExtensions.SetHtmlContent(td.InnerHtml, collectionLink);
                        }
                        else
                        {
                            string value = ModelHelperExtensions.DisplayTextSimple(html, item, prop.PropertyName).ToString();
                            td.InnerHtml.Append(value.Truncate(70));
                        }
                    }

                    tbodytr.InnerHtml.AppendHtml(td);
                }

                if (hasActions)
                {
                    var tdActions = new TagBuilder("td");

                    if (entityActions)
                    {
                        var postUrl = html.Url().Action("TriggerAction", new { id = item.Id });

                        tdActions.InnerHtml.AppendHtml("<div class='btn-group mr-2 mb-2'>");
                        tdActions.InnerHtml.AppendHtml("<form action ='" + postUrl + "' method='POST' />");

                        tdActions.InnerHtml.AppendHtml("<button type='button' class='btn btn-secondary btn-sm dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>");
                        tdActions.InnerHtml.AppendHtml("Actions");
                        tdActions.InnerHtml.AppendHtml("</button>");

                        tdActions.InnerHtml.AppendHtml("<div class='dropdown-menu'>");

                        foreach (var action in actions)
                        {
                            foreach (var actionDescription in action.Value)
                            {
                                var button = "<button type='submit' class='dropdown-item' name='action' value='" + action.Key + "'>" + actionDescription + "</button>";
                                tdActions.InnerHtml.AppendHtml(button);
                            }
                        }

                        tdActions.InnerHtml.AppendHtml(@"</div>");
                        tdActions.InnerHtml.AppendHtml(@"</form>");
                        tdActions.InnerHtml.AppendHtml(@"</div>");
                    }

                    if (details)
                    {
                        if (html.ViewBag.Collection != null)
                        {
                            tdActions.InnerHtml.AppendHtml(html.IconLink("Details", "Collection", new { id = html.ViewBag.Id, collection = html.ViewBag.Collection + "/" + item.Id }, "fa fa-search", new { @class = "btn btn-primary btn-sm mr-2 mb-2" }));
                        }
                        else
                        {
                            tdActions.InnerHtml.AppendHtml(html.IconLink("Details", "Details", new { id = item.Id }, "fa fa-search", new { @class = "btn btn-primary btn-sm mr-2 mb-2" }));
                        }
                    }

                    if (edit)
                    {
                        tdActions.InnerHtml.AppendHtml(html.IconLink("Edit", "Edit", new { id = item.Id }, "fa fa-pencil", new { @class = "btn btn-warning btn-sm mr-2 mb-2" }));
                    }

                    if (delete)
                    {
                        tdActions.InnerHtml.AppendHtml(html.IconLink("Delete", "Delete", new { id = item.Id }, "fa fa-trash", new { @class = "btn btn-danger btn-sm mr-2 mb-2" }));
                    }

                    tbodytr.InnerHtml.AppendHtml(tdActions);
                }

                tbody.InnerHtml.AppendHtml(tbodytr);
            }

            table.InnerHtml.AppendHtml(tbody);

            div.InnerHtml.AppendHtml(table);

            return new HtmlString(div.Render());
        }

        private static void SetRouteValues(IHtmlHelper htmlHelper, dynamic obj, RouteValueDictionary routeValues)
        {
            if (routeValues == null)
                return;

            foreach (var kvp in routeValues.Where(kvp => kvp.Value.GetType() == typeof(string)))
            {
                string value = GetObjectProperty(htmlHelper, obj, kvp.Value.ToString());
                routeValues[kvp.Key] = value;
            }
        }

        private static string GetObjectProperty(IHtmlHelper htmlHelper, dynamic obj, string displayExpression)
        {
            string value = displayExpression;

            if (!value.Contains("{") && !value.Contains(" "))
            {
                value = "{" + value + "}";
            }

            var replacementTokens = GetReplacementTokens(value);
            foreach (var token in replacementTokens)
            {
                var propertyName = token.Substring(1, token.Length - 2);
                if (ObjectExtensions.DynamicHasProperty(obj, propertyName))
                {
                    var displayString = ObjectExtensions.GetPropValue(obj, propertyName) != null ? ObjectExtensions.GetPropValue(obj, propertyName).ToString() : "";
                    value = value.Replace(token, displayString);
                }
                else
                {
                    value = value.Replace(token, propertyName);
                }
            }

            return value;
        }

        private static List<String> GetReplacementTokens(String str)
        {
            Regex regex = new Regex(@"{(.*?)}", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(str);

            // Results include braces (undesirable)
            return matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();
        }

        public static string Controller(this IHtmlHelper htmlHelper)
        {
            var routeValues = htmlHelper.ViewContext.RouteData.Values;

            if (routeValues.ContainsKey("controller"))
                return (string)routeValues["controller"];

            return string.Empty;
        }

        public static string Action(this IHtmlHelper htmlHelper)
        {
            var routeValues = htmlHelper.ViewContext.RouteData.Values;

            if (routeValues.ContainsKey("action"))
                return (string)routeValues["action"];

            return string.Empty;
        }

        public static HtmlString BootstrapPager(this IHtmlHelper html, int currentPageIndex, Func<int, string> action, int totalItems, int pageSize = 10, int numberOfLinks = 5)
        {
            if (totalItems <= 0)
            {
                return HtmlString.Empty;
            }
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var lastPageNumber = (int)Math.Ceiling((double)currentPageIndex / numberOfLinks) * numberOfLinks;
            var firstPageNumber = lastPageNumber - (numberOfLinks - 1);
            var hasPreviousPage = currentPageIndex > 1;
            var hasNextPage = currentPageIndex < totalPages;
            if (lastPageNumber > totalPages)
            {
                lastPageNumber = totalPages;
            }

            var nav = new TagBuilder("nav");
            nav.AddCssClass("pagination-nav");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.AddCssClass("justify-content-center");
            ul.InnerHtml.AppendHtml(AddLink(1, action, currentPageIndex == 1, "disabled", "<<", "First Page", false));
            ul.InnerHtml.AppendHtml(AddLink(currentPageIndex - 1, action, !hasPreviousPage, "disabled", "<", "Previous Page", false));
            for (int i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml.AppendHtml(AddLink(i, action, i == currentPageIndex, "active", i.ToString(), i.ToString(), false));
            }
            ul.InnerHtml.AppendHtml(AddLink(currentPageIndex + 1, action, !hasNextPage, "disabled", ">", "Next Page", true));
            ul.InnerHtml.AppendHtml(AddLink(totalPages, action, currentPageIndex == totalPages, "disabled", ">>", "Last Page", false));

            nav.InnerHtml.AppendHtml(ul);


            return new HtmlString(nav.Render());
        }

        private static Type GetNonNullableModelType(Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;
            Type underlyingType = Nullable.GetUnderlyingType(realModelType);


            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }

            return realModelType;
        }

        //@Html.BootstrapPager(pageIndex, index => Url.Action("Index", "Product", new { pageIndex = index }), Model.TotalCount, numberOfLinks: 10)
        private static TagBuilder AddLink(int index, Func<int, string> action, bool condition, string classToAdd, string linkText, string tooltip, bool nextPage)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");
            li.MergeAttribute("title", tooltip);
            if (condition)
            {
                li.AddCssClass(classToAdd);
            }
            var a = new TagBuilder("a");
            a.AddCssClass("page-link");
            if (nextPage && !condition)
            {
                a.AddCssClass("pagination__next");
            }
            a.MergeAttribute("href", !condition ? action(index) : "javascript:");
            a.InnerHtml.Append(linkText);
            li.InnerHtml.AppendHtml(a);
            return li;
        }

        public static HtmlString DisqusCommentCount(this IHtmlHelper helper, string path)
        {
            HtmlTag a = new HtmlTag("a");
            a.Attr("data-disqus-identifier", path);

            return new HtmlString(a.ToString());
        }

        public static HtmlString DisqusCommentCountScript(this IHtmlHelper helper, string disqusShortname)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var disqus_shortname = '" + disqusShortname + "';");

            sb.AppendLine("(function () {");
            sb.AppendLine("var s = document.createElement('script');");
            sb.AppendLine("s.async = true;");
            sb.AppendLine("s.src = 'http://' + disqus_shortname + '.disqus.com/count.js';");
            sb.AppendLine("(document.getElementsByTagName('HEAD')[0] || document.getElementsByTagName('BODY')[0]).appendChild(s);");
            sb.AppendLine("}");
            sb.AppendLine("());");

            script.AppendHtml(sb.ToString());

            return new HtmlString(script.ToString());
        }

        public static HtmlString FacebookCommentsThread(this IHtmlHelper helper, string siteUrl, string path, string title)
        {
            var url = string.Format("{0}{1}", siteUrl, path);

            HtmlTag div = new HtmlTag("div");
            div.AddClass("fb-comments");
            div.Attr("data-href", url);
            div.Attr("data-numposts", "10");
            div.Attr("width", "100%");
            /*div.Attr("data-order-by", "social"); reverse_time /*/

            return new HtmlString(div.ToString());
        }

        public static HtmlString FacebookCommentCount(this IHtmlHelper helper, string siteUrl, string path)
        {
            var url = string.Format("{0}{1}", siteUrl, path);

            HtmlTag span = new HtmlTag("span");
            span.AddClass("fb-comments-count");
            span.Attr("data-href", url);

            return new HtmlString(span.ToString());
        }

        //ideally right after the opening<body> tag.
        public static HtmlString FacebookCommentsScript(this IHtmlHelper helper, string appid)
        {
            HtmlTag div = new HtmlTag("div");
            div.Id("fb-root");

            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" (function(d, s, id) { ");

            sb.AppendLine(" var js, fjs = d.getElementsByTagName(s)[0]; ");
            sb.AppendLine(" if (d.getElementById(id)) return; ");
            sb.AppendLine(" js = d.createElement(s); js.id = id; ");
            sb.AppendLine(" js.src = '//connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.9&appId=" + appid + "' ");
            sb.AppendLine(" fjs.parentNode.insertBefore(js, fjs); ");

            sb.AppendLine(" } ");
            sb.AppendLine(" (document, 'script', 'facebook-jssdk')); ");

            script.AppendHtml(sb.ToString());

            return new HtmlString(div.ToString() + script.ToString());
        }

        public static HtmlString GoogleAnalyticsScript(this IHtmlHelper helper, string trackingId)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){ ");
            sb.AppendLine(" (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o), ");
            sb.AppendLine(" m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m) ");
            sb.AppendLine(" })(window,document,'script','https://www.google-analytics.com/analytics.js','ga'); ");
            sb.AppendLine(" ga('create', '" + trackingId + "', 'auto'); ");
            sb.AppendLine(" ga('send', 'pageview'); ");

            script.AppendHtml(sb.ToString());

            return new HtmlString(script.ToString());
        }

        public static HtmlString GoogleAdSenseScript(this IHtmlHelper helper, string id)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");
            script.Attr("async", "");
            script.Attr("src", "//pagead2.googlesyndication.com/pagead/js/adsbygoogle.js");

            HtmlTag script2 = new HtmlTag("script");
            script2.Attr("type", "text/javascript");

            StringBuilder sb2 = new StringBuilder();
            sb2.AppendLine(" (adsbygoogle = window.adsbygoogle || []).push({ ");
            sb2.AppendLine(" google_ad_client: '" + id + "', ");
            sb2.AppendLine(" enable_page_level_ads: false ");
            sb2.AppendLine(" }); ");

            script2.AppendHtml(sb2.ToString());

            return new HtmlString(script.ToString() + script2.ToString());
        }

        public static HtmlString DisqusThread(this IHtmlHelper helper, string disqusShortname, string siteUrl, string path, string title)
        {
            HtmlTag div = new HtmlTag("div");
            div.Id("disqus_thread");

            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            var url = string.Format("{0}{1}", siteUrl, path);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var disqus_shortname = '" + disqusShortname + "';");

            sb.AppendLine("var disqus_config = function() {");
            sb.AppendLine("this.page.url = '" + url + "'");
            sb.AppendLine("this.page.identifier = '" + path + "'");
            sb.AppendLine("this.page.title = '" + title + "'");
            sb.AppendLine("};");

            sb.AppendLine("(function() {");
            sb.AppendLine("var dsq = document.createElement('script');");
            sb.AppendLine("dsq.type = 'text/javascript';");
            sb.AppendLine("dsq.async = false;");
            sb.AppendLine("dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js';");
            sb.AppendLine("(document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);");
            sb.AppendLine("})();");

            script.AppendHtml(sb.ToString());

            HtmlTag noscript = new HtmlTag("noscript");
            noscript.AppendHtml("Please enable JavaScript to view the <a href=\"http://disqus.com/?ref_noscript\"> comments powered by Disqus.</a>");

            HtmlTag aDisqus = new HtmlTag("a");
            aDisqus.Attr("href", "http://disqus.com");
            aDisqus.AddClass("dsq-brlink");
            aDisqus.AppendHtml("blog comments powered by<span class=\"logo - disqus\">Disqus</span>");

            var finalHTML = div.ToString() + Environment.NewLine + script.ToString() + Environment.NewLine + noscript.ToString() + Environment.NewLine + aDisqus.ToString() + DisqusCommentCountScript(helper, disqusShortname).ToString();

            return new HtmlString(finalHTML);
        }

        public static HtmlString AddThisLinks(this IHtmlHelper helper, string siteUrl, string path, string title, string description, string imageUrl)
        {
            var url = string.Format("{0}{1}", siteUrl, path);

            HtmlTag div = new HtmlTag("div");
            div.Attr("class", "addthis_inline_share_toolbox_p3ki");
            div.Attr("data-url", url);
            div.Attr("data-title", title);
            div.Attr("data-description", description);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                div.Attr("data-media", imageUrl);
            }

            return new HtmlString(div.ToString());
        }

        public static HtmlString AddThisRelatedPosts(this IHtmlHelper helper)
        {

            HtmlTag div = new HtmlTag("div");
            div.Attr("class", "addthis_relatedposts_inline");

            return new HtmlString(div.ToString());
        }

        public static HtmlString ReturnToTop(this IHtmlHelper helper)
        {
            HtmlTag link = new HtmlTag("a");
            link.Attr("href", "javascript:");
            link.Id("return-to-top");
            link.AppendHtml(@"<i class=""fa fa-chevron-up""></i>");

            return new HtmlString(link.ToString());
        }

        public static HtmlString AddThisScript(this IHtmlHelper helper, string pubid)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("src", "https://s7.addthis.com/js/300/addthis_widget.js#pubid=" + pubid);
            script.Attr("type", "text/javascript");

            return new HtmlString(script.ToString());
        }

        public static HtmlString PostScrollHeight(this IHtmlHelper helper)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" var oldHeight = 0; ");
            sb.AppendLine(" window.setInterval(function() { ");
            sb.AppendLine(" if(oldHeight != document.body.scrollHeight) ");
            sb.AppendLine(" { ");
            sb.AppendLine(" oldHeight = document.body.scrollHeight; ");
            sb.AppendLine(" window.top.postMessage(document.body.scrollHeight, '*'); ");
            sb.AppendLine(" } ");
            sb.AppendLine(" }, 500);  ");

            script.AppendHtml(sb.ToString());

            return new HtmlString(script.ToString());
        }

        public static HtmlString DeferredIFrameLoad(this IHtmlHelper helper)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function init() ");
            sb.AppendLine("{");
            sb.AppendLine("var vidDefer = document.getElementsByTagName('iframe');");
            sb.AppendLine("for (var i = 0; i < vidDefer.length; i++)");
            sb.AppendLine("{");
            sb.AppendLine("if (vidDefer[i].getAttribute('data-src'))");
            sb.AppendLine("{");
            sb.AppendLine("vidDefer[i].setAttribute('src', vidDefer[i].getAttribute('data-src'));");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("window.onload = init;");

            script.AppendHtml(sb.ToString());

            return new HtmlString(script.ToString());
        }

        public static HtmlString GoogleFontAsync(this IHtmlHelper helper, List<string> fonts, bool regular = true, bool bold = false, bool black = false, bool italic = false, bool boldItalic = false, Boolean hideTextWhileLoading = true, int timeout = 5000)
        {
            var html = Google.Font.GetFontScriptAsync(fonts, regular, bold, black, italic, boldItalic, hideTextWhileLoading, timeout);
            return new HtmlString(html);
        }

        public static HtmlString GetFontBodyCSSAsync(this IHtmlHelper helper, string font)
        {
            return new HtmlString(Google.Font.GetFontBodyCSSAsync(font));
        }

        public static HtmlString GetFontNavBarCSSAsync(this IHtmlHelper helper, string font, string styleCSS)
        {
            return new HtmlString(Google.Font.GetFontNavBarCSSAsync(font, styleCSS));
        }

        public static HtmlString GoogleFont(this IHtmlHelper helper, string font, string styleCSS, bool bodyFont, bool navBarFont, bool regular = true, bool bold = false, bool italic = false, bool boldItalic = false)
        {
            var html = Google.Font.GetFontLink(font, regular, bold, italic, boldItalic);
            if (bodyFont)
            {
                html += Environment.NewLine + Google.Font.GetFontBodyCSS(font);
            }

            if (navBarFont)
            {
                html += Environment.NewLine + Google.Font.GetFontNavBarCSS(font, styleCSS);
            }

            return new HtmlString(html);
        }

        public static HtmlString GoogleFontEffects(this IHtmlHelper helper, string[] effects)
        {
            var html = Google.Font.GetEffectsLink(effects);
            return new HtmlString(html);
        }

        public static IHtmlContent Action<TController>(this IHtmlHelper helper, Expression<Action<TController>> expression, Boolean passRouteValues = false) where TController : ControllerBase
        {
            var controllerName = typeof(TController).Name;
            var result = Helpers.ExpressionHelper.GetRouteValuesFromExpression<TController>(expression);
            result.RouteValues.Remove("Action");
            result.RouteValues.Remove("Controller");

            if (passRouteValues)
            {
                return helper.ActionLink(result.Action, result.Controller, result.RouteValues);
            }
            else
            {
                return helper.ActionLink(result.Action, result.Controller);
            }

        }

        public static HtmlString MenuLink(this IHtmlHelper helper, string area, string controllerName, string actionName, string linkText, string classNames, string iconClass = "", string content = "")
        {
            var routeData = helper.ViewContext.RouteData.Values;
            var currentController = routeData["controller"];
            var currentAction = routeData["action"];

            if (String.Equals(actionName, currentAction as string,
                      StringComparison.OrdinalIgnoreCase)
                &&
               String.Equals(controllerName, currentController as string,
                       StringComparison.OrdinalIgnoreCase))

            {
                classNames = classNames + " active";

                HtmlString activeLink = null;

                if (!string.IsNullOrEmpty(iconClass))
                {
                    activeLink = new HtmlString(string.Format(@"<a href=""{0}"" class=""{1}"" title=""{2}""><i class=""fa {3}""></i></a>", helper.Url().Action(actionName, controllerName, new { area }), classNames, linkText, iconClass));
                }
                else if (!string.IsNullOrEmpty(content))
                {
                    activeLink = new HtmlString(string.Format(@"<a href=""{0}"" class=""{1}"" title=""{2}"">{3}{2}</a>", helper.Url().Action(actionName, controllerName , new { area }), classNames, linkText, content));
                }
                else
                {
                    activeLink = new HtmlString(helper.ActionLink(linkText, actionName, controllerName, new { area },
                    new { @class = classNames, title = linkText }
                    ).Render());
                }

                return activeLink;
            }

            HtmlString link = null;

            if (!string.IsNullOrEmpty(iconClass))
            {
                link = new HtmlString(string.Format(@"<a href=""{0}"" class=""{1}"" title=""{2}""><i class=""fa {3}""></i></a>", helper.Url().Action(actionName, controllerName, new { area }), classNames, linkText, iconClass));
            }
            else if (!string.IsNullOrEmpty(content))
            {
                link = new HtmlString(string.Format(@"<a href=""{0}"" class=""{1}"" title=""{2}"">{3}{2}</a>", helper.Url().Action(actionName, controllerName, new { area }), classNames, linkText, content));
            }
            else
            {
                link = new HtmlString(helper.ActionLink(linkText, actionName, controllerName, new { area },
                new { @class = classNames, title = linkText }
                ).Render());
            }

            return link;
        }

        public static IHtmlContent MenuLink(this IHtmlHelper helper, string area, string controllerName, string actionName, string linkText, string classNames)
        {
            if (controllerName != "")
            {
                var routeData = helper.ViewContext.RouteData.Values;
                var currentController = routeData["controller"];
                var currentAction = routeData["action"];

                if (String.Equals(actionName, currentAction as string,
                          StringComparison.OrdinalIgnoreCase)
                    &&
                   String.Equals(controllerName, currentController as string,
                           StringComparison.OrdinalIgnoreCase))

                {
                    classNames = classNames + " active";
                    var activeLink = helper.ActionLink(linkText, actionName, controllerName, new { area },
                        new { @class = classNames, title = linkText, itemprop = "url" }
                        );
                    return activeLink;

                }

                var link = helper.ActionLink(linkText, actionName, controllerName, new { area },
                    new { @class = classNames, title = linkText, itemprop = "url" }
                    );
                return link;
            }
            else
            {
                return new HtmlString(string.Format("<a href='{0}' itemprop='url' class='{1}'>{2}</a>", actionName, classNames, linkText));
            }
        }
        public static UrlHelper Url(this IHtmlHelper html)
        {
            return new UrlHelper(html.ViewContext);
        }

        //https://daveaglick.com/posts/getting-an-htmlhelper-for-an-alternate-model-type
        public static HtmlHelper<TModel> For<TModel>(this IHtmlHelper helper) where TModel : class, new()
        {
            return For<TModel>(helper.ViewContext, helper.ViewData);
        }

        public static HtmlHelper<TModel> For<TModel>(this IHtmlHelper helper, TModel model)
        {
            return For<TModel>(helper.ViewContext, helper.ViewData, model);
        }

        public static HtmlHelper<dynamic> For(this IHtmlHelper helper, dynamic model)
        {
            return For(helper.ViewContext, helper.ViewData, model);
        }

        public static HtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData) where TModel : class, new()
        {
            TModel model = new TModel();
            return For<TModel>(viewContext, viewData, model);
        }

        public static HtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, TModel model)
        {

            var metadataProvider = (IModelMetadataProvider)viewContext.HttpContext.RequestServices.GetService(typeof(IModelMetadataProvider));
            //var metadataProvider = new EmptyModelMetadataProvider();

            var newViewData = new ViewDataDictionary<TModel>(metadataProvider, new ModelStateDictionary()) { Model = model };

            ViewContext newViewContext = new ViewContext(
                viewContext,
                viewContext.View,
                newViewData,
                viewContext.TempData,
                viewContext.Writer,
               new HtmlHelperOptions());

            var helper = new HtmlHelper<TModel>(
                (IHtmlGenerator)viewContext.HttpContext.RequestServices.GetService(typeof(IHtmlGenerator)),
                (ICompositeViewEngine)viewContext.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)),
                (IModelMetadataProvider)viewContext.HttpContext.RequestServices.GetService(typeof(IModelMetadataProvider)),
                (IViewBufferScope)viewContext.HttpContext.RequestServices.GetService(typeof(IViewBufferScope)),
                (HtmlEncoder)viewContext.HttpContext.RequestServices.GetService(typeof(HtmlEncoder)),
                (UrlEncoder)viewContext.HttpContext.RequestServices.GetService(typeof(UrlEncoder)),
                (ExpressionTextCache)viewContext.HttpContext.RequestServices.GetService(typeof(ExpressionTextCache)));

            helper.Contextualize(newViewContext);

            return helper;
        }

        public static HtmlHelper<dynamic> ForDynamic(this IHtmlHelper helper, dynamic model)
        {
            return ForDynamic(helper.ViewContext, helper.ViewData, model);
        }

        public static HtmlHelper<dynamic> ForDynamic(ViewContext viewContext, ViewDataDictionary viewData, dynamic model)
        {

            var newViewData = new ViewDataDictionary<dynamic>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };

            ViewContext newViewContext = new ViewContext(
                viewContext,
                viewContext.View,
                newViewData,
                viewContext.TempData,
                viewContext.Writer,
                new HtmlHelperOptions());

            var helper = new HtmlHelper<dynamic>(
               (IHtmlGenerator)viewContext.HttpContext.RequestServices.GetService(typeof(IHtmlGenerator)),
                (ICompositeViewEngine)viewContext.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)),
                (IModelMetadataProvider)viewContext.HttpContext.RequestServices.GetService(typeof(IModelMetadataProvider)),
                (IViewBufferScope)viewContext.HttpContext.RequestServices.GetService(typeof(IViewBufferScope)),
                (HtmlEncoder)viewContext.HttpContext.RequestServices.GetService(typeof(HtmlEncoder)),
                (UrlEncoder)viewContext.HttpContext.RequestServices.GetService(typeof(UrlEncoder)),
                (ExpressionTextCache)viewContext.HttpContext.RequestServices.GetService(typeof(ExpressionTextCache)));

            helper.Contextualize(newViewContext);

            return helper;
        }

        public static IHtmlContent ActionLink<TController>(this IHtmlHelper html, Expression<Action<TController>> expression, string linkText, object htmlAttributes = null, Boolean passRouteValues = true) where TController : Controller
        {
            var result = Helpers.ExpressionHelper.GetRouteValuesFromExpression<TController>(expression);
            result.RouteValues.Remove("Action");
            result.RouteValues.Remove("Controller");

            IDictionary<string, object> htmlAttributesDict = null;

            if (htmlAttributes != null)
            {
                htmlAttributesDict = (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes);
            }
            else
            {
                htmlAttributesDict = new Dictionary<string, object>();
            }

            if (passRouteValues)
            {
                return html.ActionLink(linkText, result.Action, result.Controller, result.RouteValues, htmlAttributesDict);
            }
            else
            {
                return html.ActionLink(linkText, result.Action, result.Controller, new RouteValueDictionary(), htmlAttributesDict);
            }
        }

        public static Dictionary<string, string> ToDictionary(this Object attributes)
        {
            var props = attributes.GetType().GetProperties();
            var pairs = props.ToDictionary(x => x.Name.Replace("_", "-"), x => x.GetValue(attributes, null).ToString());
            return pairs;
        }

        public static Dictionary<string, string> StringsToDictionary(this Object attributes)
        {
            var props = attributes.GetType().GetProperties();
            var pairs = props.Where(x => x.PropertyType == typeof(string)).ToDictionary(x => x.Name, x => x.GetValue(attributes, null).ToString());
            return pairs;
        }

        public static HtmlString BooleanCheckbox(this IHtmlHelper htmlHelper, string expression, string text, bool isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            HtmlTag div = new HtmlTag("div");
            if (divHtmlAttributes != null)
            {
                foreach (var kvp in divHtmlAttributes.ToDictionary())
                {
                    div.Attr(kvp.Key, kvp.Value);
                }
            }

            var id = htmlHelper.Id(expression);

            var input = htmlHelper.CheckBox(expression, isChecked, inputHtmlAttributes).Render();

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            HtmlTag label = new HtmlTag("label");
            if (labelHtmlAttributes != null)
            {
                foreach (var kvp in labelHtmlAttributes.ToDictionary())
                {
                    label.Attr(kvp.Key, kvp.Value);
                }
            }
            label.AppendHtml(text);

            div.AppendHtml(input);
            if (!string.IsNullOrEmpty(text))
            {
                div.Append(label);
            }

            return new HtmlString(div.ToString());
        }

        public static HtmlString BooleanCheckboxButton(this IHtmlHelper htmlHelper, string expression, string text, bool isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            HtmlTag div = new HtmlTag("div");
            if (divHtmlAttributes != null)
            {
                foreach (var kvp in divHtmlAttributes.ToDictionary())
                {
                    div.Attr(kvp.Key, kvp.Value);
                }
            }

            var input = htmlHelper.CheckBox(expression, isChecked, inputHtmlAttributes).Render();

            HtmlTag label = new HtmlTag("label");
            if (labelHtmlAttributes != null)
            {
                foreach (var kvp in labelHtmlAttributes.ToDictionary())
                {
                    label.Attr(kvp.Key, kvp.Value);
                }
            }

            if (isChecked)
            {
                label.AddClass("active");
            }

            label.AppendHtml(input);
            label.AppendHtml(text);

            div.Append(label);

            return new HtmlString(div.ToString());
        }

        public static HtmlString ValueCheckbox(this IHtmlHelper htmlHelper, string expression, string value, string text, bool isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            HtmlTag div = new HtmlTag("div");
            if (divHtmlAttributes != null)
            {
                foreach (var kvp in divHtmlAttributes.ToDictionary())
                {
                    div.Attr(kvp.Key, kvp.Value);
                }
            }

            var id = htmlHelper.Id(expression);
            var name = htmlHelper.Name(expression);

            HtmlTag input = new HtmlTag("input");
            if (inputHtmlAttributes != null)
            {
                foreach (var kvp in inputHtmlAttributes.ToDictionary())
                {
                    input.Attr(kvp.Key, kvp.Value);
                }
            }
            input.Id(id);
            input.Name(name);
            input.Attr("type", "checkbox");
            input.Value(value);
            if (isChecked)
            {
                input.Attr("checked", "checked");
            }

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            HtmlTag label = new HtmlTag("label");
            if (labelHtmlAttributes != null)
            {
                foreach (var kvp in labelHtmlAttributes.ToDictionary())
                {
                    label.Attr(kvp.Key, kvp.Value);
                }
            }
            label.AppendHtml(text);

            div.Append(input);
            if (!string.IsNullOrEmpty(text))
            {
                div.Append(label);
            }

            return new HtmlString(div.ToString());
        }

        public static HtmlString ValueCheckboxButton(this IHtmlHelper htmlHelper, string expression, string value, string text, bool isChecked, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var id = htmlHelper.Id(expression);
            var name = htmlHelper.Name(expression);

            HtmlTag input = new HtmlTag("input");
            if (inputHtmlAttributes != null)
            {
                foreach (var kvp in inputHtmlAttributes.ToDictionary())
                {
                    input.Attr(kvp.Key, kvp.Value);
                }
            }
            input.Id(id);
            input.Name(name);
            input.Attr("type", "checkbox");
            input.Value(value);
            if (isChecked)
            {
                input.Attr("checked", "checked");
            }

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            HtmlTag label = new HtmlTag("label");
            if (labelHtmlAttributes != null)
            {
                foreach (var kvp in labelHtmlAttributes.ToDictionary())
                {
                    label.Attr(kvp.Key, kvp.Value);
                }
            }
            if (isChecked)
            {
                label.AddClass("active");
            }
            label.Append(input);
            label.AppendHtml(text);

            return new HtmlString(label.ToString());
        }

        public static HtmlString ValueRadio(this IHtmlHelper htmlHelper, string expression, string value, string text, bool isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            // var radioHtml = htmlHelper.RadioButton(expression, value, isChecked, htmlAttributes).Render();

            HtmlTag div = new HtmlTag("div");
            if (divHtmlAttributes != null)
            {
                foreach (var kvp in divHtmlAttributes.ToDictionary())
                {
                    div.Attr(kvp.Key, kvp.Value);
                }
            }

            var input = htmlHelper.RadioButton(expression, value, isChecked, inputHtmlAttributes).Render();

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            HtmlTag label = new HtmlTag("label");
            if (labelHtmlAttributes != null)
            {
                foreach (var kvp in labelHtmlAttributes.ToDictionary())
                {
                    label.Attr(kvp.Key, kvp.Value);
                }
            }
            label.AppendHtml(text);

            div.AppendHtml(input);

            if (!string.IsNullOrEmpty(text))
            {
                div.Append(label);
            }

            return new HtmlString(div.ToString());
        }

        public static HtmlString ValueRadioButton(this IHtmlHelper htmlHelper, string expression, string value, string text, bool isChecked, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var input = htmlHelper.RadioButton(expression, value, isChecked, inputHtmlAttributes).Render();

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            HtmlTag label = new HtmlTag("label");
            if (labelHtmlAttributes != null)
            {
                foreach (var kvp in labelHtmlAttributes.ToDictionary())
                {
                    label.Attr(kvp.Key, kvp.Value);
                }
            }
            if (isChecked)
            {
                label.AddClass("active");
            }

            label.AppendHtml(input);
            label.AppendHtml(text);


            return new HtmlString(label.ToString());
        }

        public static HtmlString ValueCheckboxList(this IHtmlHelper htmlHelper, string expression, IList<SelectListItem> items, bool inline)
        {
            string divClass = "form-check";
            if (inline)
            {
                divClass += " form-check-inline";
            }

            var sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.AppendLine(htmlHelper.ValueCheckbox(expression, item.Value, item.Text, item.Selected, new { @class = divClass }, new { @class = "form-check-input" }, new { @class = "form-check-label" }).Render());
            }
            return new HtmlString(sb.ToString());
        }

        public static HtmlString ValueRadioList(this IHtmlHelper htmlHelper, string expression, IList<SelectListItem> items, bool inline)
        {
            string divClass = "form-check";
            if (inline)
            {
                divClass += " form-check-inline";
            }

            var sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.AppendLine(htmlHelper.ValueRadio(expression, item.Value, item.Text, item.Selected, new { @class = divClass }, new { @class = "form-check-input" }, new { @class = "form-check-label" }).Render());
            }
            return new HtmlString(sb.ToString());
        }

        public static HtmlString ValueCheckboxButtonList(this IHtmlHelper htmlHelper, string expression, IList<SelectListItem> items, object divHtmlAttributes, object labelHtmlAttributes)
        {
            HtmlTag div = new HtmlTag("div");
            if (divHtmlAttributes != null)
            {
                foreach (var kvp in divHtmlAttributes.ToDictionary())
                {
                    div.Attr(kvp.Key, kvp.Value);
                }
            }

            foreach (var item in items)
            {
                div.AppendHtml(htmlHelper.ValueCheckboxButton(expression, item.Value, item.Text, item.Selected, new { @class = "", autocomplete = "off" }, labelHtmlAttributes).Render());
            }

            return new HtmlString(div.ToString());
        }

        public static HtmlString ValueRadioButtonList(this IHtmlHelper htmlHelper, string expression, IList<SelectListItem> items, bool groupRadioButtons, object divHtmlAttributes, object labelHtmlAttributes)
        {
            HtmlTag div = new HtmlTag("div");
            if (divHtmlAttributes != null)
            {
                foreach (var kvp in divHtmlAttributes.ToDictionary())
                {
                    div.Attr(kvp.Key, kvp.Value);
                }
            }
            if (groupRadioButtons)
            {
                div.AddClass("btn-group");
            }

            foreach (var item in items)
            {
                div.AppendHtml(htmlHelper.ValueRadioButton(expression, item.Value, item.Text, item.Selected, new { @class = "", autocomplete = "off" }, labelHtmlAttributes).Render());
            }

            return new HtmlString(div.ToString());
        }

        public static HtmlString TrueFalseRadioButtonList(this IHtmlHelper htmlHelper, string expression, bool isChecked, bool groupRadioButtons, object divHtmlAttributes, object labelHtmlAttributes)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem("True", "true", isChecked, false));
            items.Add(new SelectListItem("False", "false", !isChecked, false));

            HtmlTag div = new HtmlTag("div");
            if (divHtmlAttributes != null)
            {
                foreach (var kvp in divHtmlAttributes.ToDictionary())
                {
                    div.Attr(kvp.Key, kvp.Value);
                }
            }
            if (groupRadioButtons)
            {
                div.AddClass("btn-group");
            }

            foreach (var item in items)
            {
                div.AppendHtml(htmlHelper.ValueRadioButton(expression, item.Value, item.Text, item.Selected, new { @class = "", autocomplete = "off" }, labelHtmlAttributes).Render());
            }

            return new HtmlString(div.ToString());
        }

        public static HtmlString YesNoRadioButtonList(this IHtmlHelper htmlHelper, string expression, bool isChecked, bool groupRadioButtons, object divHtmlAttributes, object labelHtmlAttributes)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem("Yes", "true", isChecked, false));
            items.Add(new SelectListItem("No", "false", !isChecked, false));

            HtmlTag div = new HtmlTag("div");
            if (divHtmlAttributes != null)
            {
                foreach (var kvp in divHtmlAttributes.ToDictionary())
                {
                    div.Attr(kvp.Key, kvp.Value);
                }
            }
            if (groupRadioButtons)
            {
                div.AddClass("btn-group");
            }

            foreach (var item in items)
            {
                div.AppendHtml(htmlHelper.ValueRadioButton(expression, item.Value, item.Text, item.Selected, new { @class = "", autocomplete = "off" }, labelHtmlAttributes).Render());
            }

            return new HtmlString(div.ToString());
        }
    }
}
