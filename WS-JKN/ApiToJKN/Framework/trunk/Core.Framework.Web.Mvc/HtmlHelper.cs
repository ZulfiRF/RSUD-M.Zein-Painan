using System;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Core.Framework.Web.Mvc.Contract;
using Core.Framework.Web.Mvc.Impl;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace Core.Framework.Web.Mvc
{
    public class MvcRazorToPdf
    {
        public byte[] GeneratePdfOutput(ControllerContext context, object model = null, string viewName = null,
            Action<PdfWriter, Document> configureSettings = null)
        {
            if (viewName == null)
            {
                viewName = context.RouteData.GetRequiredString("action");
            }

            context.Controller.ViewData.Model = model;

            byte[] output;
            using (var document = new Document())
            {
                using (var workStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, workStream);
                    writer.CloseStream = false;

                    if (configureSettings != null)
                    {
                        configureSettings(writer, document);
                    }
                    document.Open();


                    using (var reader = new StringReader(RenderRazorView(context, viewName)))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, reader);

                        document.Close();
                        output = workStream.ToArray();
                    }
                }
            }
            return output;
        }

        public string RenderRazorView(ControllerContext context, string viewName)
        {
            IView viewEngineResult = ViewEngines.Engines.FindView(context, viewName, null).View;
            var sb = new StringBuilder();


            using (TextWriter tr = new StringWriter(sb))
            {
                var viewContext = new ViewContext(context, viewEngineResult, context.Controller.ViewData,
                    context.Controller.TempData, tr);
                viewEngineResult.Render(viewContext, tr);
            }
            return sb.ToString();
        }
    }
    public static class CoreExtenssion
    {
        //public static MvcHtmlString MetroTextbox(this HtmlHelper helper, string placeHolder)
        //{
        //    var conbtrol = new MvcHtmlString(String.Format("<div class=\"textbox\"><input  placeholder=\"" + placeHolder + "\" type=\"text\" /></div>"));
        //    return conbtrol;
        //}

        //public static MvcHtmlString MetroTextbox(this HtmlHelper helper, string placeHolder, object htmlAttribute)
        //{
        //    var builder = new StringBuilder();
        //    if (htmlAttribute != null)
        //        builder.Append(Helper.ConvertToAttribut(htmlAttribute));
        //    var conbtrol = new MvcHtmlString("<div class=\"textbox\"><input  placeholder=\"" + placeHolder + "\" type=\"text\" " + builder + " /></div>");
        //    return conbtrol;
        //}

        #region TextBox

        public static MvcHtmlString MetroTextboxKnockoutWithDisplayFor(this HtmlHelper helper, string memberName, string displayName)
        {
            var conbtrol = MetroTextboxKnockoutWithDisplayFor(helper, memberName, displayName, new { data_bind = "value : " + memberName });
            return conbtrol;
        }

        public static MvcHtmlString MetroTextboxKnockoutWithDisplayFor(this HtmlHelper helper, string memberName, string displayName, object htmlAttribute)
        {
            var conbtrol = BaseMetroTexBox(memberName, displayName, htmlAttribute);
            var htmlStrig = conbtrol.ToHtmlString();
            var builder = new StringBuilder();
            builder.Append("<div>");
            builder.Append("<b>" + displayName + "</b></div>");
            builder.Append(htmlStrig);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString MetroTextboxKnockoutFor(this HtmlHelper helper, string memberName)
        {
            var conbtrol = BaseMetroTexBox(memberName, null);

            return conbtrol;
        }

        public static MvcHtmlString MetroTextboxKnockoutFor(this HtmlHelper helper, string memberName, object htmlAttribute)
        {
            var conbtrol = BaseMetroTexBox(memberName, htmlAttribute);
            return conbtrol;
        }

        public static IMetroTextBox MetroTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return new MetroTextBox(expression);
        }

        public static IMetroTextBox MetroTextBox(this HtmlHelper helper)
        {
            return new MetroTextBox();
        }

        private static MvcHtmlString BaseMetroTexBox(string memberName, object htmlAttribute)
        {
            return BaseMetroTexBox(memberName, "", htmlAttribute);
        }

        private static MvcHtmlString BaseMetroTexBox(string memberName, string displayName, object htmlAttribute)
        {
            var builder = new StringBuilder();
            builder.Append("<div class=\"{textbox}\">");
            builder.Append("<input autocomplete=\"off\"   ");
            builder.Append(" id=\"" + memberName + "\"");
            builder.Append(" name=\"" + memberName + "\"");
            bool hasDisable = false;
            if (htmlAttribute != null)
                builder.Append(Helper.Helper.ConvertToAttribut(htmlAttribute));
            if (builder.ToString().Contains("disabled"))
                hasDisable = true;
            if (!builder.ToString().ToLower().Contains("placeholder") && !hasDisable)
                builder.Append(" placeholder=\"Type " + displayName + "...\" ");
            builder.Append("/>");
            builder.Append("</div>");
            string html = builder.ToString();
            html = html.Replace("{textbox}", hasDisable ? "textbox-disabled" : "textbox");
            var conbtrol = new MvcHtmlString(html);
            return conbtrol;
        }

        #endregion TextBox

        #region Password

        public static MvcHtmlString MetroPasswordKnockoutWithDisplayFor(this HtmlHelper helper, string memberName, string displayName)
        {
            var control = MetroPasswordKnockoutWithDisplayFor(helper, memberName, displayName, new { data_bind = "value : " + memberName });
            return control;
        }

        public static MvcHtmlString MetroPasswordKnockoutWithDisplayFor(this HtmlHelper helper, string memberName, string displayName, object htmlAttribute)
        {
            var conbtrol = BaseMetroPassword(memberName, htmlAttribute);
            var htmlStrig = conbtrol.ToHtmlString();
            var builder = new StringBuilder();
            builder.Append("<div>");
            builder.Append("<b>" + displayName + "</b></div>");
            builder.Append(htmlStrig);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString MetroPasswordKnockoutFor(this HtmlHelper helper, string memberName)
        {
            var conbtrol = BaseMetroPassword(memberName, null);
            return conbtrol;
        }

        public static MvcHtmlString MetroPasswordKnockoutFor(this HtmlHelper helper, string memberName, object htmlAttribute)
        {
            var conbtrol = BaseMetroTexBox(memberName, htmlAttribute);
            return conbtrol;
        }

        private static MvcHtmlString BaseMetroPassword(string memberName, object htmlAttribute)
        {
            return BaseMetroPassword(memberName, "", htmlAttribute);
        }

        private static MvcHtmlString BaseMetroPassword(string memberName, string displayName, object htmlAttribute)
        {
            var builder = new StringBuilder();
            builder.Append("<div class=\"textbox\">");
            builder.Append("<input   type=\"password\"");
            builder.Append(" id=\"" + memberName + "\"");
            builder.Append(" name=\"" + memberName + "\"");
            builder.Append(" data-bind=\" value :  " + memberName + "\"");
            if (htmlAttribute != null)
                foreach (var propertyInfo in htmlAttribute.GetType().GetProperties())
                {
                    builder.Append(" " + propertyInfo.Name.Replace("_", "-") + "=\"" + propertyInfo.GetValue(htmlAttribute, null) +
                                   "\"");
                }
            builder.Append("/>");
            builder.Append("</div>");

            var conbtrol = new MvcHtmlString(builder.ToString());
            return conbtrol;
        }

        #endregion Password

        #region MetroGrid

        public static IMetroGridKnockoutJs GridMetro(this HtmlHelper helper)
        {
            return new MetroGridKnockoutJs();
        }

        #endregion MetroGrid

        #region Date

        public static IDateTimeMetro MetroDateTime(this HtmlHelper helper)
        {
            return new DateTimeMetro();
        }

        public static IComboMetro MetroComboBox(this HtmlHelper helper)
        {
            return new ComboMetro();
        }

        #endregion Date
    }
}