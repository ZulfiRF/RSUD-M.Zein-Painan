using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Core.Framework.Web.Mvc.Contract;

namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class DateTimeMetro
    /// </summary>
    public class DateTimeMetro : IDateTimeMetro
    {

        /// <summary>
        /// Gets or sets the property attribute input.
        /// </summary>
        /// <value>The property attribute input.</value>
        public object PropertyAttributeInput { get; set; }

        #region Implementation of IDateTimeMetro

        /// <summary>
        /// Dates the format. digunakan untuk mengatur format dari date time yang akan di tampilkan
        /// </summary>
        /// <param name="format">berisikan format dari date time yang akan di tampilan</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  DateFormat
        ///   <code>
        /// @Html.MetroDateTime().DateFormat("dd-mm-yyyy").Render()
        ///   </code>
        ///   </example>
        public IDateTimeMetro DateFormat(string format)
        {
            Format = format;
            return this;
        }

        /// <summary>
        /// Defaults the date. digunakn untuk memberikan nilai default pada date time
        /// </summary>
        /// <param name="date">berisikan tanggal sekarang yang akan di set</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  DefaultDate
        ///   <code>
        /// @Html.MetroDateTime().DefaultDate(DateTime.Now).Render()
        ///   </code>
        ///   </example>
        public IDateTimeMetro DefaultDate(DateTime date)
        {
            Date = date;
            return this;
        }

        /// <summary>
        /// Withes the display. digunakn untuk memberikan label pada date time metro
        /// </summary>
        /// <param name="display">The display name.</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  DisplayName
        ///   <code>
        /// @Html.MetroDateTime().DisplayName("Tanggal Masuk").Render()
        ///   </code>
        ///   </example>
        public IDateTimeMetro DisplayName(string display)
        {
            Display = display;
            return this;
        }

        /// <summary>
        /// Requireds the specified is required. digunakan untuk memberikan tanda jika control itu wajib di isi
        /// </summary>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Required
        ///   <code>
        /// @Html.MetroDateTime().Required(true).Render()
        ///   </code>
        ///   </example>
        public IDateTimeMetro Required(bool isRequired)
        {
            IsRequired = isRequired;
            return this;
        }

        /// <summary>
        /// Inputs the atribute. digunakan untuk memberikan atribute pada textbox date time
        /// </summary>
        /// <param name="htmlAttribute">berisikan object atribute yang akan di tambahkan</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  InputAtribute
        ///   <code>
        /// @Html.MetroDateTime().DisplayName("Tanggal Masuk").Render()
        ///   </code>
        ///   </example>
        public IDateTimeMetro InputAtribute(object htmlAttribute)
        {
            PropertyAttributeInput = htmlAttribute;
            return this;
        }

        /// <summary>
        /// Reads the only. digunakan jika date time tidak bisa dirubah oleh user
        /// </summary>
        /// <param name="readOnly">if set to <c>true</c> [is readOnly]</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  ReadOnly
        ///   <code>
        /// @Html.MetroDateTime().ReadOnly(true).Render()
        ///   </code>
        ///   </example>
        public IDateTimeMetro ReadOnly(string readOnly)
        {
            Readonly = readOnly;
            return this;
        }

        /// <summary>
        /// Members the specified member name. digunakan untk memberikan tag id dan name pada hasil render
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Member
        ///   <code>
        /// @Html.MetroDateTime().Member("StartDate").Render()
        ///   </code>
        ///   </example>
        public IDateTimeMetro Member(string memberName)
        {
            MemberName = memberName;
            return this;
        }



        /// <summary>
        /// Renders this instance. digunakan untuk menggenerate control
        /// </summary>
        /// <returns>MvcHtmlString.</returns>
        public MvcHtmlString Render()
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(Display))
            {
                builder.Append("<div>");
                builder.Append("<b>" + Display + "</b></div>");
            }
            builder.Append(
                "<div id=\"input." + MemberName + "\" data-metro=\"date\" class=\"textbox input-append date  dropdown\" ");
            if (!string.IsNullOrEmpty(Format))
            {
                Format = Format.Replace("mm", "MM");
                builder.Append(" data-date-format=\"" + Format.ToLower() + "\" ");
                if (Date.HasValue)
                    builder.Append(" data-date=\"" + Date.Value.ToString(Format) + "\" ");
                else
                {
                    builder.Append(" data-date=\"" + DateTime.Now.ToString(Format) + "\" ");
                }
            }
            else
                builder.Append(" data-date=\"12-02-2012\" ");
            builder.Append(" >");
            builder.Append("<input data-metro=\"true\" id=\"" + MemberName + "\" name=\"" + MemberName + "\"  class=\"date\" size=\"16\" type=\"text\" " + (string.IsNullOrEmpty(Readonly) ? "" : "readonly=\"readonly\"") + " ");
            if (Date.HasValue)
                builder.Append(" value=\"" + Date.Value.ToString(Format) + "\" ");
            else
            {
                builder.Append(" value=\"" + DateTime.Now.ToString(Format) + "\" ");
            }
            if (IsRequired)
                builder.Append(" req=\"true\"");
            builder.Append(Helper.Helper.ConvertToAttribut(PropertyAttributeInput));
            builder.Append(" >");

            builder.Append("<span class=\"ui-button ui-widget boder-blue ui-button-icon-only ui-combobox-toggle right add-on\" style=\"position:absolute;outline:none \" tabindex=\"-1\"  style=\"height: 16px\"><i class=\"icons-date\" >");
            builder.Append("</i></span>");
            builder.Append("</div>");
            return new MvcHtmlString(builder.ToString());
        }

        #endregion Implementation of IDateTimeMetro

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the display.
        /// </summary>
        /// <value>The display.</value>
        public string Display { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        public string MemberName { get; set; }


        /// <summary>
        /// Gets or sets the readonly.
        /// </summary>
        /// <value>The readonly.</value>
        public string Readonly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value><c>true</c> if this instance is required; otherwise, <c>false</c>.</value>
        public bool IsRequired { get; set; }
    }
}