using Budgeting.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Budgeting.Web
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString DropDownListEx(this HtmlHelper helper, string name, IEnumerable<ILookup> list, object selectedValue, object htmlAttributes, bool isOptional = false)
        {

            TagBuilder dropdown = new TagBuilder("select");

            //Setting the name and id attribute with name parameter passed to this method.
            dropdown.Attributes.Add("name", name);
            dropdown.Attributes.Add("id", name);

            StringBuilder options = new StringBuilder();
            //Iterated over the IEnumerable list.
            if (isOptional)
            {
                options = options.Append(string.Format("<option value='{0}' {1}>{2}</option>",
                    null,
                    string.Empty,
                    string.Empty));
            }

            foreach (var item in list)
            {
                options = options.Append(string.Format("<option value='{0}' {1}>{2}</option>",
                    item.DataValue,
                    item.DataValue.Equals(selectedValue == null ? string.Empty : selectedValue.ToString()) ? "selected" : string.Empty,
                    item.DataText));

            }


            dropdown.InnerHtml = options.ToString();
            //Assigning the attributes passed as a htmlAttributes object.
            dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(dropdown.ToString(TagRenderMode.Normal));
        }

    }
}