using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Core.Framework.Web.Mvc.Contract;

namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class MetroGridKnockoutJs
    /// </summary>
    public class MetroGridKnockoutJs : IMetroGridKnockoutJs
    {
        /// <summary>
        /// The list column
        /// </summary>
        private readonly List<IMetroGridKnockoutJs> listColumn = new List<IMetroGridKnockoutJs>();

        /// <summary>
        /// Gets or sets the click method.
        /// </summary>
        /// <value>The click method.</value>
        public string ClickMethod { get; set; }

        /// <summary>
        /// Gets or sets the list source.
        /// </summary>
        /// <value>The list source.</value>
        public string ListSource { get; set; }

        /// <summary>
        /// Gets or sets the type click.
        /// </summary>
        /// <value>The type click.</value>
        public ClickType TypeClick { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is template.
        /// </summary>
        /// <value><c>true</c> if this instance is template; otherwise, <c>false</c>.</value>
        protected bool IsTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        protected IColumn Template { get; set; }
        #region Implementation of IMetroGridKnockoutJs

        /// <summary>
        /// The is double click
        /// </summary>
        private bool isDoubleClick;
        /// <summary>
        /// The type grid metro grid knockout
        /// </summary>
        private TypeGridMetroGridKnockoutJs typeGridMetroGridKnockout;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetroGridKnockoutJs"/> class.
        /// </summary>
        public MetroGridKnockoutJs()
        {
            typeGridMetroGridKnockout = TypeGridMetroGridKnockoutJs.Inline;
        }

        /// <summary>
        /// Gets or sets the head title. digunakan untuk title pada  grid
        /// </summary>
        /// <value>berisikan title pada grid</value>
        public string HeadTitle { get; set; }

        /// <summary>
        /// Gets or sets the HTML attribute. digunakan untuk menambahkan Html Atribute pada cell
        /// </summary>
        /// <value>berisikan atribute yang akan di tambahkan</value>
        public object HtmlAttribute { get; set; }

        /// <summary>
        /// Gets or sets the HTML attribute search.
        /// </summary>
        /// <value>The HTML attribute search.</value>
        public object HtmlAttributeSearch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is page.
        /// </summary>
        /// <value><c>true</c> if this instance is page; otherwise, <c>false</c>.</value>
        public bool IsPage { get; set; }

        /// <summary>
        /// Gets or sets the name value. digunakan untuk bindang data saat menggunakan knockout Js
        /// </summary>
        /// <value>berisikan property dari data yang akan di binding</value>
        public string NameValue { get; set; }

        /// <summary>
        /// Gets or sets the place holder search.
        /// </summary>
        /// <value>The place holder search.</value>
        public string PlaceHolderSearch { get; set; }
        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="title">berisikan title pada coloum</param>
        /// <param name="databind">berisikan object yang akan di tambahkan pada atribute cell.</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddBindingColumn
        ///   <code>
        /// @Html.GridMetro().AddBindingColumn("Name",new{style="width=100%"}).Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs AddBindingColumn(string title, object databind)
        {
            var newColumn = new MetroGridKnockoutJs();
            newColumn.HeadTitle = title;
            newColumn.HtmlAttribute = databind;
            newColumn.IsCustom = true;
            listColumn.Add(newColumn);
            return this;
        }

        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="headTitle">berisikan title pada coloum</param>
        /// <param name="nameValue">The name value.</param>
        /// <param name="htmlAttribute">berisikan object yang akan di tambahkan pada atribute cell</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddColumn
        ///   <code>
        /// @Html.GridMetro().AddColumn(new ButtonHeaderTemplate().HtmlAttribute(new { style = "width:143px" }).Header(new HeaderTemplate().Header("")).Column(new ButtonTemplate().Content("Manage Question").CssClass("btn").Atribute(new { data_bind = "click : $parent.ManageQuestion" }))).Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs AddColumn(string headTitle, string nameValue, object htmlAttribute)
        {
            var newColumn = new MetroGridKnockoutJs();
            newColumn.HeadTitle = headTitle;
            newColumn.NameValue = nameValue;
            newColumn.HtmlAttribute = htmlAttribute;
            listColumn.Add(newColumn);
            return this;
        }

        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="template">nerisikan template colum yang akan di gunakan</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddColumn
        ///   <code>
        /// @Html.GridMetro().AddColumn(new ButtonHeaderTemplate().HtmlAttribute(new { style = "width:143px" }).Header(new HeaderTemplate().Header("")).Column(new ButtonTemplate().Content("Manage Question").CssClass("btn").Atribute(new { data_bind = "click : $parent.ManageQuestion" }))).Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs AddColumn(IColumn template)
        {
            return AddColumn(template, null);
        }

        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="htmlAttribute">berisikan object yang akan di tambahkan pada atribute cell</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddColumn
        ///   <code>
        /// @Html.GridMetro().AddBindingColumn("Name",new{style="width=100%"}).Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs AddColumn(IColumn template, object htmlAttribute)
        {
            var newColoum = new MetroGridKnockoutJs();
            newColoum.IsTemplate = true;
            newColoum.HtmlAttribute = htmlAttribute;
            newColoum.Template = template;
            listColumn.Add(newColoum);
            return this;
        }

        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="headTitle">berisikan title pada coloum</param>
        /// <param name="nameValue">berisikan property dari data yang akan di binding</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddColumn
        ///   <code>
        /// @Html.GridMetro().AddColumn("Name","Name").Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs AddColumn(string headTitle, string nameValue)
        {
            return AddColumn(headTitle, nameValue, null);
        }

        /// <summary>
        /// Clicks the specified type. digunakn untuk menentukan type dari event pada grid
        /// </summary>
        /// <param name="type">berisikan jenis click type yang di gunakan</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Click
        ///   <code>
        /// @Html.GridMetro().Click(ClickType.None).Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs Click(ClickType type)
        {
            if (!isDoubleClick)
                TypeClick = type;
            return this;
        }

        /// <summary>
        /// Clicks the event. digunakan untuk delegate event pada javascript
        /// </summary>
        /// <param name="clickMethod">berisikan nama function</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  ClickEvent
        ///   <code>
        /// @Html.GridMetro().ClickEvent("showDetail").Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs ClickEvent(string clickMethod)
        {
            ClickMethod = clickMethod;
            return this;
        }

        /// <summary>
        /// Datas the source knockout js. digunakan untuk mengatur data source dari grid jika menggunakan knockout JS
        /// </summary>
        /// <param name="list">berisikan nama property yang digunakan sebagai data source</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  DataSourceKnockoutJs
        ///   <code>
        /// @Html.GridMetro().DataSourceKnockoutJs("FamilyStructures").Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs DataSourceKnockoutJs(string list)
        {
            ListSource = list;
            return this;
        }

        /// <summary>
        /// Renders this instance. digunakan untuk menggenerate control
        /// </summary>
        /// <returns>MvcHtmlString.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public MvcHtmlString Render()
        {
            var str = new StringBuilder();
            str.Append("<table class=\"table\">");
            str.Append("<thead>");
            str.Append("<tr>");
            foreach (IMetroGridKnockoutJs metroGridKnockoutJse in listColumn)
            {
                if (metroGridKnockoutJse is MetroGridKnockoutJs)
                {
                    if ((metroGridKnockoutJse as MetroGridKnockoutJs).IsTemplate)
                    {
                        var baseContentTemplate = (metroGridKnockoutJse as MetroGridKnockoutJs).Template as BaseContentTemplate;
                        if (baseContentTemplate != null)
                            str.Append("<th " + Helper.Helper.ConvertToAttribut(baseContentTemplate.Attribute) + ">" +
                                       (metroGridKnockoutJse as MetroGridKnockoutJs).Template.HeaderTemplateProperty.Render() +
                                       "</th>");
                        else
                        {
                            str.Append("<th></th>");
                        }
                    }
                    else
                    {
                        //if (metroGridKnockoutJse.IsCustom)
                        //{
                        //    str.Append("<th ");
                        //    str.Append(Helper.ConvertToAttribut(metroGridKnockoutJse.HtmlAttribute));
                        //    str.Append(">");
                        //}
                        //else
                        str.Append("<th>");
                        str.Append(metroGridKnockoutJse.HeadTitle);
                        str.Append("</th>");
                    }
                }
            }
            str.Append("</tr>");
            str.Append("</thead>");
            if (string.IsNullOrEmpty(ListSource))
            {
                if (IsPage)
                    str.Append("<tbody data-bind=\"foreach:pagedRows\">");
                else
                    str.Append("<tbody data-bind=\"foreach:listModel\">");
            }
            else str.Append("<tbody data-bind=\"foreach:" + ListSource + "\">");
            switch (typeGridMetroGridKnockout)
            {
                case TypeGridMetroGridKnockoutJs.Inline:
                    if (!isDoubleClick)
                        str.Append("<tr data-bind=\"click:$parent.editMode\">");
                    else
                        str.Append("<tr data-bind=\"event :{ dblclick :$parent.editMode}\">");
                    break;

                case TypeGridMetroGridKnockoutJs.Javascript:
                    str.Append("<tr data-bind=\"click:" + ClickMethod + "\">");
                    break;

                case TypeGridMetroGridKnockoutJs.Form:
                    switch (TypeClick)
                    {
                        case ClickType.DoubleClick:
                            str.Append("<tr data-bind=\"event : { dblclick :$parent.editMode}\">");

                            break;

                        case ClickType.OneClick:
                            str.Append("<tr data-bind=\"click:$parent.editMode\">");
                            break;

                        case ClickType.None:
                            str.Append("<tr>");
                            break;

                        default:
                            str.Append("<tr>");
                            break;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            foreach (IMetroGridKnockoutJs metroGridKnockoutJse in listColumn)
            {
                if (metroGridKnockoutJse is MetroGridKnockoutJs)
                {
                    if ((metroGridKnockoutJse as MetroGridKnockoutJs).IsTemplate)
                    {
                        str.Append("<td " + Helper.Helper.ConvertToAttribut(HtmlAttribute) + " >" +
                                   (metroGridKnockoutJse as MetroGridKnockoutJs).Template.ColumnTemplateProperty.Render());
                        str.Append("</td>");
                    }
                    else if (!metroGridKnockoutJse.IsCustom && !string.IsNullOrEmpty(metroGridKnockoutJse.NameValue))
                    {
                        str.Append("<td " + Helper.Helper.ConvertToAttribut(metroGridKnockoutJse.HtmlAttribute) + " data-bind=\"text :" + metroGridKnockoutJse.NameValue + "\"/>");
                    }
                    else
                    {
                        str.Append("<td " + Helper.Helper.ConvertToAttribut(metroGridKnockoutJse.HtmlAttribute) + " />");
                    }
                }
            }
            str.Append("</tr>");
            str.Append("</tbody>");
            str.Append("</table>");
            if (IsPage)
            {
                str.Append("<div class=\"pagination\">");
                str.Append("<div style=\"margin-top: 2px; width: 100%\">");
                str.Append("<ul>");
                str.Append(
                    "<li data-bind=\"css: { disabled: pageIndex() === 0 }\"><a  data-bind=\"click: previousPage\">");
                str.Append("Previous</a></li></ul>");
                str.Append("<ul data-bind=\"foreach: allPages\">");
                str.Append("<li data-bind=\"css: { disabled: $data.pageNumber === ($root.pageIndex() + 1) }\"><a ");
                str.Append(
                    "data-bind=\"text: $data.title, click: function() { $root.moveToPage($data.pageNumber-1); }\">");
                str.Append("</a></li>");
                str.Append("</ul>");
                str.Append("<ul>");
                str.Append(
                    "<li data-bind=\"css: { disabled: pageIndex() == maxPageIndex()-1  ||( maxPageIndex()-1<0) }\">");
                str.Append("<a href=\"#\" data-bind=\"click: nextPage\">Next</a></li></ul>");
                str.Append("<div class=\"right\" style=\"margin-top: 3px;position:relative\">");
                if (!string.IsNullOrEmpty(PlaceHolderSearch))
                {
                    //str.Append(CoreExtenssion.MetroTextBox(null).Attribute(HtmlAttributeSearch).Render().ToHtmlString());
                    str.Append("<span class=\"uiSearchInput\"><span>");
                    str.Append("<input " + Helper.Helper.ConvertToAttribut(HtmlAttributeSearch) +
                               " style=\"width: 260px\" type=\"text\" maxlength=\"100\" placeholder=\"Search\">");
                    str.Append("<button type=\"submit\" title=\"Search\" data-bind=\"click:searchData\">");
                    str.Append("</button>");
                    str.Append("</span></span>");
                }
                str.Append("</div>");
                str.Append("</div>");
                str.Append("</div>");
            }

            //else
            //    if (!string.IsNullOrEmpty(PlaceHolderSearch) && false)
            //    {
            //        str.Append("<div class=\"right\" style=\"margin-top: 3px;\">");
            //        str.Append(CoreExtenssion.MetroTextBox(null).Attribute(HtmlAttributeSearch).Render().ToHtmlString());
            //        str.Append("</div>");
            //    }

            return new MvcHtmlString(str.ToString());
        }
        /// <summary>
        /// Searches the specified placeholder. digunakan untuk memberikan watermark dan atribute pada search box
        /// </summary>
        /// <param name="placeholder">The placeholder.</param>
        /// <param name="htmlAttribute">berisikan object yang akan di tambahkan pada textbox search</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Search
        ///   <code>
        /// @Html.GridMetro().Search("Searching Template Question...", new { data_bind = "event: { change: searchData }"}).Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs Search(string placeholder, object htmlAttribute)
        {
            HtmlAttributeSearch = htmlAttribute;
            PlaceHolderSearch = placeholder;
            return this;
        }

        /// <summary>
        /// Types the grid. digunakan untuk menentukan type event dari grid
        /// </summary>
        /// <param name="type">berisikan type dari grid</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  TypeGrid
        ///   <code>
        /// @Html.GridMetro().TypeGrid(TypeGridMetroGridKnockoutJs.Form).Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs TypeGrid(TypeGridMetroGridKnockoutJs type)
        {
            typeGridMetroGridKnockout = type;
            return this;
        }

        /// <summary>
        /// Uses the double click. digunakan jika ingen menggunakan event doble click
        /// </summary>
        /// <returns>IMetroGridKnockoutJs.</returns>
        public IMetroGridKnockoutJs UseDoubleClick()
        {
            isDoubleClick = true;
            TypeClick = ClickType.DoubleClick;
            return this;
        }

        /// <summary>
        /// Uses the paged. digunakan jika akan mengunakan pager
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  UsePaged
        ///   <code>
        /// @Html.GridMetro().UsePaged(true).Render()
        ///   </code>
        ///   </example>
        public IMetroGridKnockoutJs UsePaged(bool value)
        {
            IsPage = value;
            return this;
        }
        #endregion Implementation of IMetroGridKnockoutJs

        #region IMetroGridKnockoutJs Members

        /// <summary>
        /// Gets or sets a value indicating whether this instance is custom. digunakan untuk memberikan tanda jika menggunakan tamplate custom
        /// </summary>
        /// <value><c>true</c> if this instance is custom; otherwise, <c>false</c>.</value>
        public bool IsCustom { get; set; }

        #endregion IMetroGridKnockoutJs Members
    }
}