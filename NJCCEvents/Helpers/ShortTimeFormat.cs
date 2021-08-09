using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NJCCEvents.Helpers
{
    public static class ShortTimeFormat
    {
        public static IHtmlString ShortTime(this HtmlHelper helper, DateTime thisTime, string name, bool edit)
        {
            //DateTime.ParseExact( model.StartDateTime, "yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");)

            var myTime = thisTime.ToString("hh:mm tt");
            
            if (edit)
                return new MvcHtmlString(string.Format("<input type='text'  name='{1}' id='{1}' data-date-format='HH:mm' style='width:100%;' class='input-small' value='{0}'>", myTime, name));
            else
                //return new MvcHtmlString(string.Format("{0}", myTime));
                return new MvcHtmlString(string.Format("<p  id='{1}'>{0}</p>", myTime, name));
        }
    }
}