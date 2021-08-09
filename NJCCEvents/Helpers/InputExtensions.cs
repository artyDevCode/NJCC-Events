using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NJCCEvents.Helpers
{
    public static class InputExtensions
    {
        public static IHtmlString RichTextAreaFor(this HtmlHelper helper, string name="", string text ="", bool isread=false)
        {
            return new MvcHtmlString(string.Format("<textarea name='{0}' data-njcc-read='{3}' id='{1}'>{2}</textarea>", name, name, text, isread));
        }
    }
}