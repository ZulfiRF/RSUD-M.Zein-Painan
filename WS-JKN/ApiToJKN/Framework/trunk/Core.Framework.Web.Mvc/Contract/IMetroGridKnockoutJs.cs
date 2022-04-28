using Core.Framework.Web.Mvc.Impl;

namespace Core.Framework.Web.Mvc.Contract
{
    /// <summary>
    /// Interface IMetroGridKnockoutJs
    /// </summary>
    public interface IMetroGridKnockoutJs : ICustomControl
    {
        /// <summary>
        /// Gets or sets the head title. digunakan untuk title pada  grid
        /// </summary>
        /// <value>berisikan title pada grid</value>
        string HeadTitle { get; set; }

        /// <summary>
        /// Gets or sets the HTML attribute. digunakan untuk menambahkan Html Atribute pada cell
        /// </summary>
        /// <value>berisikan atribute yang akan di tambahkan</value>
        object HtmlAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is custom. digunakan untuk memberikan tanda jika menggunakan tamplate custom
        /// </summary>
        /// <value><c>true</c> if this instance is custom; otherwise, <c>false</c>.</value>
        bool IsCustom { get; set; }

        /// <summary>
        /// Gets or sets the name value. digunakan untuk bindang data saat menggunakan knockout Js
        /// </summary>
        /// <value>berisikan property dari data yang akan di binding</value>
        string NameValue { get; set; }

        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="title">berisikan title pada coloum</param>
        /// <param name="databind">berisikan object yang akan di tambahkan pada atribute cell.</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddBindingColumn        
        /// <code>
        /// @Html.GridMetro().AddBindingColumn("Name",new{style="width=100%"}).Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs AddBindingColumn(string title, object databind);

        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="headTitle">berisikan title pada coloum</param>
        /// <param name="nameValue">The name value.</param>
        /// <param name="htmlAttribute">berisikan object yang akan di tambahkan pada atribute cell</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddColumn        
        /// <code>
        /// @Html.GridMetro().AddColumn(new ButtonHeaderTemplate().HtmlAttribute(new { style = "width:143px" }).Header(new HeaderTemplate().Header("")).Column(new ButtonTemplate().Content("Manage Question").CssClass("btn").Atribute(new { data_bind = "click : $parent.ManageQuestion" }))).Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs AddColumn(string headTitle, string nameValue, object htmlAttribute);

        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="template">nerisikan template colum yang akan di gunakan</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddColumn        
        /// <code>
        /// @Html.GridMetro().AddColumn(new ButtonHeaderTemplate().HtmlAttribute(new { style = "width:143px" }).Header(new HeaderTemplate().Header("")).Column(new ButtonTemplate().Content("Manage Question").CssClass("btn").Atribute(new { data_bind = "click : $parent.ManageQuestion" }))).Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs AddColumn(IColumn template);

        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="htmlAttribute">berisikan object yang akan di tambahkan pada atribute cell</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddColumn        
        /// <code>
        /// @Html.GridMetro().AddBindingColumn("Name",new{style="width=100%"}).Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs AddColumn(IColumn template, object htmlAttribute);

        /// <summary>
        /// Adds the binding column. digunakan untuk membinding data
        /// </summary>
        /// <param name="headTitle">berisikan title pada coloum</param>
        /// <param name="nameValue">berisikan property dari data yang akan di binding</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  AddColumn        
        /// <code>
        /// @Html.GridMetro().AddColumn("Name","Name").Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs AddColumn(string headTitle, string nameValue);

        /// <summary>
        /// Clicks the specified type. digunakn untuk menentukan type dari event pada grid
        /// </summary>
        /// <param name="type">berisikan jenis click type yang di gunakan</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Click        
        /// <code>
        /// @Html.GridMetro().Click(ClickType.None).Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs Click(ClickType type);

        /// <summary>
        /// Clicks the event. digunakan untuk delegate event pada javascript
        /// </summary>
        /// <param name="clickMethod">berisikan nama function</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  ClickEvent        
        /// <code>
        /// @Html.GridMetro().ClickEvent("showDetail").Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs ClickEvent(string clickMethod);

        /// <summary>
        /// Datas the source knockout js. digunakan untuk mengatur data source dari grid jika menggunakan knockout JS
        /// </summary>
        /// <param name="list">berisikan nama property yang digunakan sebagai data source</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  DataSourceKnockoutJs        
        /// <code>
        /// @Html.GridMetro().DataSourceKnockoutJs("FamilyStructures").Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs DataSourceKnockoutJs(string list);

        /// <summary>
        /// Searches the specified placeholder. digunakan untuk memberikan watermark dan atribute pada search box
        /// </summary>
        /// <param name="placeholder">The placeholder.</param>
        /// <param name="htmlAttribute">berisikan object yang akan di tambahkan pada textbox search</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Search        
        /// <code>
        /// @Html.GridMetro().Search("Searching Template Question...", new { data_bind = "event: { change: searchData }"}).Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs Search(string placeholder, object htmlAttribute);

        /// <summary>
        /// Types the grid. digunakan untuk menentukan type event dari grid
        /// </summary>
        /// <param name="type">berisikan type dari grid</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  TypeGrid        
        /// <code>
        /// @Html.GridMetro().TypeGrid(TypeGridMetroGridKnockoutJs.Form).Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs TypeGrid(TypeGridMetroGridKnockoutJs type);

        /// <summary>
        /// Uses the double click. digunakan jika ingen menggunakan event doble click
        /// </summary>
        /// <returns>IMetroGridKnockoutJs.</returns>
        IMetroGridKnockoutJs UseDoubleClick();

        /// <summary>
        /// Uses the paged. digunakan jika akan mengunakan pager
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>IMetroGridKnockoutJs.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  UsePaged        
        /// <code>
        /// @Html.GridMetro().UsePaged(true).Render()
        ///</code>
        ///</example>
        IMetroGridKnockoutJs UsePaged(bool value);
    }
}