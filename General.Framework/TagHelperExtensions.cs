using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using General.Framework.Datatable;

namespace General.Framework
{
    //public class TagHelperExtensions : TagHelper
    //{
    //}

    [HtmlTargetElement("a", Attributes = ClaimAttributeName)]
    [HtmlTargetElement("input")]
    [HtmlTargetElement("button", Attributes = ClaimAttributeName)]
    public class ClaimTagHelper : TagHelper
    {
        private const string ClaimAttributeName = "asp-own-route";

        public ClaimTagHelper()
        {

        }

        [HtmlAttributeName(ClaimAttributeName)]
        public string OwnRoute { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // output.SuppressOutput();
        }
    }


    /// <summary>
    /// 分页控件
    /// </summary>
    public class PagerTagHelper : TagHelper
    {
        private const string PageValueAttributeName = "page-value";

        /// <summary>
        /// 分页参数
        /// </summary>
        [HtmlAttributeName(PageValueAttributeName)]
        public Pagination Paging { get; set; }
        
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //分页model为空
            if (Paging == null)
                return;
            //没有数据
            if (Paging.TotalCount <= 0)
                return;
            string routeName = Paging.RouteName;
            if (routeName == null)
                if (ViewContext.ActionDescriptor.AttributeRouteInfo != null)
                    routeName = ViewContext.ActionDescriptor.AttributeRouteInfo.Name;
            if (routeName == null)
                return;
            UrlHelper urlHelper = new UrlHelper(ViewContext);
            //连接地址
            string urlString = urlHelper.RouteUrl(routeName, Paging.RouteArg as object);
            urlString = urlString.Any(o => o == '?') ? urlString + "&page={0}&size={1}" : urlString + "?page={0}&size={1}";
            StringBuilder sb = new StringBuilder();

            //默认显示最多7个连续按钮
            int display = 7;

            int minDisplay = 1;
            int maxDisplay = 7;
            if (Paging.TotalPages > display)
            {
                if (Paging.PageIndex + display / 2 >= Paging.TotalPages)
                {
                    maxDisplay = Paging.TotalPages;
                    minDisplay = Paging.TotalPages - display;
                }
                else if (Paging.PageIndex > display / 2)
                {
                    minDisplay = Paging.PageIndex - display / 2;
                    maxDisplay = Paging.PageIndex + display / 2;
                }
            }
            else
            {
                minDisplay = 1;
                maxDisplay = Paging.TotalPages;
            }


            sb.Append("<nav><ul class=\"pagination\">");

            sb.AppendFormat("<li class=\"disabled\"><a>当前{0}/{1}页 共{2}条</a></li>", Paging.PageIndex, Paging.TotalPages, Paging.TotalCount);

            #region 上一页

            if (Paging.HasPreviousPage)
            {
                sb.AppendFormat("<li><a href=\"{0}\">上一页</a></li>", String.Format(urlString, Paging.PageIndex - 1,Paging.PageSize));
            }
            else
            {
                sb.Append("<li class=\"disabled\"><a href=\"javascript: \">上一页</a></li>");
            }

            #endregion

            if (Paging.PageIndex > display / 2 + 2)
            {
                sb.AppendFormat("<li>< a href = \"{0}\" >1</a ></ li>", String.Format(urlString, 1,Paging.PageSize));
                sb.Append("<li><a href=\"javascript:\">...</ a ></li>");
            }
            else if (Paging.PageIndex == display / 2 + 2)
            {
                sb.AppendFormat("<li><a href=\"{0}\">1</a></li>", String.Format(urlString, 1,Paging.PageSize));
            }
            for (int i = minDisplay; i <= maxDisplay; i++)
            {
                if (i == Paging.PageIndex)
                {
                    sb.AppendFormat("<li class=\"active\"><a href = \"javascript:\" >{0}</a></li>", i);
                }
                else
                {
                    sb.AppendFormat("<li><a href = \"{0}\">{1}</ a ></li>", String.Format(urlString, i,Paging.PageSize), i);
                }
            }
            if (maxDisplay + 1 < Paging.TotalPages)
            {
                sb.Append("<li><a href=\"javascript:\" >...</ a ></li>");
                sb.AppendFormat("<li><a href = \"{0}\" >{1}</a></li>", String.Format(urlString, Paging.TotalPages,Paging.PageSize), Paging.TotalPages);
            }
            else if (maxDisplay + 1 == Paging.TotalPages)
            {
                sb.AppendFormat("<li><a href = \"{0}\" >{1}</a></li>", String.Format(urlString, Paging.TotalPages,Paging.PageSize), Paging.TotalPages);
            }


            #region 下一页

            if (Paging.HasNextPage)
            {
                sb.AppendFormat("<li class=\"next\"><a href=\"{0}\">下一页</a></li>", String.Format(urlString, Paging.PageIndex + 1, Paging.PageSize));
            }
            else
            {
                sb.Append("<li class=\"disabled\"><a href=\"javascript:\">下一页</a></li>");
            }
            #endregion

            sb.Append("<li>&nbsp;&nbsp;</li>");

            #region 每页条数选择按钮
            sb.Append("<li> ");
            sb.Append("<div class=\"btn-group\">");
            sb.Append("<button type=\"button\" class=\"btn btn-white dropdown-toggle btn-sm\" style=\"padding:6px 10px;\" data-toggle=\"dropdown\">");
            sb.AppendFormat("每页{0}条 <span class=\"ace-icon fa fa-caret-down icon-on-right\"></span>", Paging.PageSize);
            sb.Append("</button>");
            sb.Append("<ul class=\"dropdown-menu\" role=\"menu\">");
            sb.AppendFormat("<li><a href=\"{0}\">每页20条</a></li>", String.Format(urlString, Paging.PageIndex, 20));
            sb.AppendFormat("<li><a href=\"{0}\">每页30条</a></li>", String.Format(urlString, Paging.PageIndex, 30));
            sb.AppendFormat("<li><a href=\"{0}\">每页50条</a></li>", String.Format(urlString, Paging.PageIndex, 50));
            sb.AppendFormat("<li><a href=\"{0}\">每页100条</a></li>", String.Format(urlString, Paging.PageIndex, 100));
            sb.Append("</ul>");

            sb.Append("</div>");
            sb.Append("</li>");
            #endregion

            sb.Append("</ul></nav>");
            output.Content.SetHtmlContent(sb.ToString());
        }
    }

}
