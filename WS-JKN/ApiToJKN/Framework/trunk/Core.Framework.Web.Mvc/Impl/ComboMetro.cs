using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Core.Framework.Web.Mvc.Contract;

namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class ComboMetro
    /// </summary>
    public class ComboMetro : IComboMetro
    {
        /// <summary>
        /// The items
        /// </summary>
        private List<SelectListItem> items = new List<SelectListItem>();

        /// <summary>
        /// Withes the display. digunakn untuk memberikan label pada combo box
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  WithDisplay
        ///   <code>
        /// @Html.MetroComboBox().WithDisplay("Jenis kelamin").Render()
        ///   </code>
        ///   </example>
        public IComboMetro WithDisplay(string displayName)
        {
            DisplayName = displayName;
            return this;
        }

        /// <summary>
        /// Adds the item. digunakan untuk menambahkan Item pada combobox
        /// </summary>
        /// <param name="item">berisikan item yang akan di tambahkan pada combobox</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Add Item with Single Item
        ///   <code>
        /// @Html.MetroComboBox().AddItem(new SelectListItem(){Value = "Value",Selected = true,Text = "Text"}).Render()
        ///   </code>
        ///   </example>
        public IComboMetro AddItem(SelectListItem item)
        {
            items.Add(item);
            return this;
        }

        /// <summary>
        /// Adds the item. digunakan untuk menambahkan List Item pada combobox
        /// </summary>
        /// <param name="item">berisikan List item yang akan di tambahkan pada combobox</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Add Item with List Item
        ///   <code>
        /// @Html.MetroComboBox().AddItem(new SelectListItem[] { new SelectListItem() { Value = "Value", Selected = true, Text = "Text" } }).Render()
        ///   </code>
        ///   </example>
        public IComboMetro AddItem(SelectListItem[] item)
        {
            foreach (var selectListItem in item)
            {
                items.Add(selectListItem);    
            }
            return this;
        }

        /// <summary>
        /// URLs the source. digunakan untuk end point dari data yang di tampilkan pada combo box
        /// </summary>
        /// <param name="url">berisikan url endpoint dari list data</param>
        /// <param name="keyMember">berisikan nilai yang akan di ambil</param>
        /// <param name="valueMember">berisikan nilai yang akan di tampilkan di combobox</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil Url Source
        ///   <code>
        // {val} merupakan tanda yang digunakan untuk mengambil text hasil dari text box
        /// @Html.MetroComboBox().UrlSource(Url.Content("/LogBook/Manage/GetListEmploye?name={val}"), "ID", "Name").Render()
        ///   </code>
        ///   </example>
        public IComboMetro UrlSource(string url, string keyMember, string valueMember)
        {
            Url = url;
            Key = keyMember;
            ValueField = valueMember;
            return this;
        }

        /// <summary>
        /// Requireds the specified is required. digunakan untuk memberikan tanda jika control itu wajib di isi
        /// </summary>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Required
        ///   <code>
        /// @Html.MetroComboBox().Required(true).Render()
        ///   </code>
        ///   </example>
        public IComboMetro Required(bool isRequired)
        {
            IsRequired = isRequired;
            return this;
        }

        /// <summary>
        /// Members the specified member name. digunakan untk memberikan tag id dan name pada hasil render
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>IComboMetro.</returns>
        public IComboMetro Member(string memberName)
        {
            MemberName = memberName;
            return this;
        }

        /// <summary>
        /// Attributes the specified HTML attribute. digunakan untuk memberikan atribute pada combobox
        /// </summary>
        /// <param name="htmlAttribute">berisikan object atribute yang akan di tambahkan</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Attribute
        ///   <code>
        /// @Html.MetroComboBox().Attribute(new { display = "Choose Level Difficulty", data_bind = "options: listLevel,optionsText: 'name',optionsValue:'type',optionsCaption: 'Choose...'" }).Render()
        ///   </code>
        ///   </example>
        public IComboMetro Attribute(object htmlAttribute)
        {
            ComboAttribute = htmlAttribute;
            return this;
        }

        /// <summary>
        /// Renders this instance. digunakan untuk menggenerate control
        /// </summary>
        /// <returns>MvcHtmlString.</returns>
        public MvcHtmlString Render()
        {
            var str = new StringBuilder();
            if (!string.IsNullOrEmpty(DisplayName))
            {
                str.Append("<div>");
                str.Append("<b>" + DisplayName + "</b></div>");
            }
            if (string.IsNullOrEmpty(Url))
            {
                if (items.Count != 0)
                {
                    str.Append("<select  " + (string.IsNullOrEmpty(ValueData) ? " " : "data=\"" + ValueData + "\" ") + ((IsRequired) ? "req=\"true\"" : "") + " data-metro=\"true\" name=\"" + MemberName + "\" id=\"" + MemberName + "\" " + Helper.Helper.ConvertToAttribut(ComboAttribute) + " >");
                    foreach (var selectListItem in items)
                    {
                        if (string.IsNullOrEmpty(ValueData))
                            str.Append("<option  value=\"" + selectListItem.Value + "\" >" + selectListItem.Text + "</options>");
                        else
                            if (ValueData.Equals(selectListItem.Value))
                                str.Append("<option selected=\"selected\"  value=\"" + selectListItem.Value + "\" >" + selectListItem.Text + "</options>");
                            else
                                str.Append("<option  value=\"" + selectListItem.Value + "\" >" + selectListItem.Text + "</options>");
                    }
                    str.Append("</select>");
                }
                else
                {
                    str.Append("<select  " + (string.IsNullOrEmpty(ValueData) ? " " : "data=\"" + ValueData + "\" ") + ((IsRequired) ? "req=\"true\"" : "") + " data-metro=\"true\" " + Helper.Helper.ConvertToAttribut(ComboAttribute) + " name=\"" + MemberName + "\" id=\"" + MemberName + "\" >");
                    str.Append("</select>");
                }
            }
            else
            {
                str.Append("<select   " + (string.IsNullOrEmpty(ValueData) ? " " : "data=\"" + ValueData + "\" ") + ((IsRequired) ? "req=\"true\"" : "") + " data-metro=\"true\" " + Helper.Helper.ConvertToAttribut(ComboAttribute) + " name=\"" + MemberName + "\" id=\"" + MemberName + "\" url-source=\"" + Url + "\" key=\"" + Key + "\" val=\"" + ValueField + "\">");
                str.Append("</select>");
            }

            return new MvcHtmlString(str.ToString());
        }

        /// <summary>
        /// Values the specified value. digunakan untuk memberikan nilai dari combo box
        /// </summary>
        /// <param name="value">berisikan nilai dari combo box yang di pilih</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Value
        ///   <code>
        /// @Html.MetroComboBox().Value("01").Render()
        ///   </code>
        ///   </example>
        public IComboMetro Value(string value)
        {
            ValueData = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the value field.
        /// </summary>
        /// <value>The value field.</value>
        public string ValueField { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        public string MemberName { get; set; }

        /// <summary>
        /// Gets or sets the combo attribute.
        /// </summary>
        /// <value>The combo attribute.</value>
        public object ComboAttribute { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value><c>true</c> if this instance is required; otherwise, <c>false</c>.</value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the value data.
        /// </summary>
        /// <value>The value data.</value>
        public string ValueData { get; set; }
    }
}