using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace NJCCEvents.Helpers
{
    public static class EventDisplay
    {
        public static IHtmlString EventDescription(this HtmlHelper helper, string target, string text, string text1)
          {
              return new MvcHtmlString(string.Format("<p style='text-align: center' id='{1}'>{0} ---- {2}</p>", target, text, text1));
          }
    }
}
