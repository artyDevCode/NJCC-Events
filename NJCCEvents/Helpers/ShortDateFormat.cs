using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace NJCCEvents.Helpers
{
    public static class ShortDateFormat
    {                                       
         public static IHtmlString ShortDate(this HtmlHelper helper, DateTime thisDate, string name,  bool edit)
        {
            var myDate = thisDate.ToString("dd-MM-yyyy");   //thisDate.ToShortDateString().ToString();
          
            if (edit)
                return new MvcHtmlString(string.Format("<input type='text' readonly='readonly' name='{1}' id='{1}' class='datepicker' style='width:100%;' value='{0}'>", myDate, name));
            else
                return new MvcHtmlString(string.Format("{0}", myDate));
            //return new MvcHtmlString(string.Format("<p style='text-align: center;' id='{1}'>{0}</p>", myDate, name));
        }
    }
}