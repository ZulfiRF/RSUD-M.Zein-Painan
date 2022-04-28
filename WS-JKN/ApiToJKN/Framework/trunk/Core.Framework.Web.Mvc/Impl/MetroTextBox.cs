using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Core.Framework.Model;
using Core.Framework.Model.Attr;
using Core.Framework.Web.Mvc.Contract;

namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class MetroTextBox
    /// </summary>
    public class MetroTextBox : IMetroTextBox
    {
        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public dynamic Expression { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetroTextBox"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public MetroTextBox(object expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetroTextBox"/> class.
        /// </summary>
        public MetroTextBox()
        {
        }

        #region Implementation of IMetroTextBox

        /// <summary>
        /// Inputs the atribute. digunakan untuk memberikan atribute pada textbox
        /// </summary>
        /// <param name="htmlAttribute">berisikan object atribute yang akan di tambahkan</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Attribute
        ///   <code>
        /// @Html.MetroTextBox().Attribute(new{style="width:100%"}).Render()
        ///   </code>
        ///   </example>
        public IMetroTextBox Attribute(object htmlAttribute)
        {
            HtmlAttribute = htmlAttribute;
            return this;
        }

        /// <summary>
        /// Cols the row text box. digunakan jika menggunakan type multiline
        /// </summary>
        /// <param name="col">berisikan column dari text box</param>
        /// <param name="row">berisikan row dari text box.</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  ColRowTextBox
        ///   <code>
        /// @Html.MetroTextBox().ColRowTextBox(4, 3).Render()
        ///   </code>
        ///   </example>
        public IMetroTextBox ColRowTextBox(int col, int row)
        {
            Col = col;
            Row = row;
            return this;
        }

        /// <summary>
        /// Maxes the length. digunakan untuk membatasi panjang inputan pada textbox
        /// </summary>
        /// <param name="max">berisikan panjang karakter</param>
        /// <returns>IMetroTextBox.</returns>
        public IMetroTextBox MaxLength(int max)
        {
            Max = max;
            return this;
        }

        /// <summary>
        /// Members the specified member name. digunakan untk memberikan tag id dan name pada hasil render
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Member
        ///   <code>
        /// @Html.MetroTextBox().Member("Description").Render()
        ///   </code>
        ///   </example>
        public IMetroTextBox Member(string memberName)
        {
            MemberName = memberName;
            return this;
        }

        /// <summary>
        /// Mins the length.digunakan untuk membatasi minimal panjang inputan pada textbox
        /// </summary>
        /// <param name="min">berisikan minimal panjang karakter.</param>
        /// <returns>IMetroTextBox.</returns>
        public IMetroTextBox MinLength(int min)
        {
            Min = min;
            return this;
        }

        /// <summary>
        /// Masks the specified val. digunakan untuk memberikan mask pada textbox
        /// </summary>
        /// <param name="val">mask yang digunakan</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Mask
        ///   <code>
        //a - Represents an alpha character (A-Z,a-z)
        //9 - Represents a numeric character (0-9)
        //* - Represents an alphanumeric character (A-Z,a-z,0-9)
        /// @Html.MetroTextBox().Mask("99:99").Render()
        ///   </code>
        ///   </example>
        public IMetroTextBox Mask(string val)
        {
            MaskValue = val;
            return this;
        }

        /// <summary>
        /// Nots the border. digunakan jika textbox tidak menggunakan border
        /// </summary>
        /// <returns>IMetroTextBox.</returns>
        public IMetroTextBox NotBorder()
        {
            Border = true;
            return this;
        }

        /// <summary>
        /// Renders this instance. digunakan untuk menggenerate control
        /// </summary>
        /// <returns>MvcHtmlString.</returns>
        public MvcHtmlString Render()
        {
            var str = new StringBuilder();
            dynamic data = Expression;

            if (!string.IsNullOrEmpty(DisplayName))
            {
                str.Append("<div>");
                str.Append("<b>" + DisplayName + "</b></div>");
            }
            var builder = new StringBuilder();
            if (!Border)
                builder.Append("<div class=\"{textbox}\">");
            else
                builder.Append("<div >");
            if (IsRequired)
            {
                builder.Append("<span class=\"icon-remove-text\">*</span>");
            }
            if (Type != MetroTextBoxType.MultiLine)
            {
                builder.Append("<input data-metro=\"true\" autocomplete=\"off\"   ");
                if (!string.IsNullOrEmpty(Val))
                {
                    builder.Append("value=\"" + Val + "\" ");
                }
            }
            else
            {
                builder.Append("<textarea data-metro=\"true\"  cols=\"" + ((Col.HasValue) ? Col.Value.ToString(CultureInfo.InvariantCulture) : "1") + "\" rows=\"" + ((Row.HasValue) ? Row.Value.ToString(CultureInfo.InvariantCulture) : "1") + "\" ");
                if (IsRequired)
                    builder.Append(" req=\"true\"");
            }

            if (data != null)
            {
                try
                {
                    object a = "s";
                    var model = data.Type.GenericTypeArguments;
                    var obj = Activator.CreateInstance(model[0]);
                    var property = data.Body.Member.Name;
                    var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    object val = null;

                    val = prop.GetCustomAttributes(true)[0];
                    if (val is FieldAttribute)
                    {
                        var atribute = val as FieldAttribute;
                        if (atribute.Length != 0)
                            builder.Append(" maxlength=\"" + atribute.Length + "\"");
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                if (Max != 0)
                {
                    builder.Append(" maxlength=\"" + Max + "\"");
                }
            }

            if (Min != 0)
            {
                builder.Append(" minlength=\"" + Min + "\"");
            }
            if (!string.IsNullOrEmpty(MaskValue))
                builder.Append("mask=\"" + MaskValue + "\"");
            if (!string.IsNullOrEmpty(MemberName))
            {
                builder.Append(" id=\"" + MemberName.Replace(".", "_") + "\"");
                builder.Append(" name=\"" + MemberName + "\"");
            }
            if (IsRequired)
                builder.Append(" req=\"true\"");
            switch (Type)
            {
                case MetroTextBoxType.SingleLine:
                    builder.Append(" type=\"text\"");
                    break;

                case MetroTextBoxType.MultiLine:
                    break;

                case MetroTextBoxType.Password:
                    builder.Append(" type=\"password\"");
                    break;

                case MetroTextBoxType.Url:
                    builder.Append(" type=\"url\"");
                    break;

                case MetroTextBoxType.Time:
                    builder.Append(" type=\"Time\"");
                    break;

                case MetroTextBoxType.Week:
                    builder.Append(" type=\"Week\"");
                    break;

                case MetroTextBoxType.Date:
                    builder.Append(" type=\"Date\"");
                    break;

                case MetroTextBoxType.DateTime:
                    builder.Append(" type=\"DateTime\"");
                    break;

                case MetroTextBoxType.Email:
                    builder.Append(" type=\"Email\"");
                    if (IsRequired)
                        builder.Append(" required=\"required\"");
                    break;

                case MetroTextBoxType.Month:
                    builder.Append(" type=\"Month\"");
                    break;

                case MetroTextBoxType.Number:
                    builder.Append(" type=\"Number\"");
                    break;

                case MetroTextBoxType.Range:
                    builder.Append(" type=\"range\"");
                    break;

                case MetroTextBoxType.Search:
                    builder.Append(" type=\"Search\"");
                    break;

                case MetroTextBoxType.File:
                    builder.Append(" type=\"file\"");
                    break;

                default:
                    builder.Append(" type=\"text\"");
                    break;
            }
            bool hasDisable = false;
            if (HtmlAttribute != null)
                builder.Append(Helper.Helper.ConvertToAttribut(HtmlAttribute));
            if (builder.ToString().Contains("disabled"))
                hasDisable = true;
            if (!builder.ToString().ToLower().Contains("placeholder"))
                builder.Append(" placeholder=\"Type " + DisplayName + "...\" ");
            if (Type != MetroTextBoxType.MultiLine)
                builder.Append("/>");
            else
            {
                if (string.IsNullOrEmpty(Val))
                    builder.Append("></textarea>");
                else
                    builder.Append(">" + Val + "</textarea>");
            }
            builder.Append("</div>");
            string html = builder.ToString();
            html = html.Replace("{textbox}", hasDisable ? "textbox-disabled" : "textbox");
            str.Append(html);
            return new MvcHtmlString(str.ToString());
        }

        /// <summary>
        /// Requireds the specified is required. digunakan untuk memberikan tanda jika control itu wajib di isi
        /// </summary>
        /// <param name="value">if set to <c>true</c> [is required].</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Required
        ///   <code>
        /// @Html.MetroTextBox().Required(true).Render()
        ///   </code>
        ///   </example>
        public IMetroTextBox Required(bool value)
        {
            IsRequired = value;
            return this;
        }

        /// <summary>
        /// Types the text box. digunakan untuk memberikan type pada textbox
        /// </summary>
        /// <param name="type">berisikan type dari textbox</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Member
        ///   <code>
        /// @Html.MetroTextBox().TypeTextBox(MetroTextBoxType.MultiLine).Render()
        ///   </code>
        ///   </example>
        public IMetroTextBox TypeTextBox(MetroTextBoxType type)
        {
            Type = type;
            return this;
        }

        /// <summary>
        /// Values the specified val. digunakan untuk memberikan nilai value pada textbox
        /// </summary>
        /// <param name="val">berisikan value yang akan di tampilkan</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Value
        ///   <code>
        /// @Html.MetroTextBox().Value("Nilai Value").Render()
        ///   </code>
        ///   </example>
        public IMetroTextBox Value(string val)
        {
            Val = val;
            return this;
        }

        /// <summary>
        /// Withes the display. digunakn untuk memberikan label pada TEXTBOX
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  WithDisplay
        ///   <code>
        /// @Html.MetroTextBox().WithDisplay("Tanggal Masuk").Render()
        ///   </code>
        ///   </example>
        public IMetroTextBox WithDisplay(string displayName)
        {
            DisplayName = displayName;
            return this;
        }

        #endregion Implementation of IMetroTextBox

        /// <summary>
        /// Gets or sets the max.
        /// </summary>
        /// <value>The max.</value>
        public int Max { get; set; }

        /// <summary>
        /// Gets or sets the min.
        /// </summary>
        /// <value>The min.</value>
        public int Min { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MetroTextBox"/> is border.
        /// </summary>
        /// <value><c>true</c> if border; otherwise, <c>false</c>.</value>
        protected bool Border { get; set; }

        /// <summary>
        /// Gets or sets the col.
        /// </summary>
        /// <value>The col.</value>
        protected int? Col { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        protected string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the HTML attribute.
        /// </summary>
        /// <value>The HTML attribute.</value>
        protected object HtmlAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value><c>true</c> if this instance is required; otherwise, <c>false</c>.</value>
        protected bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        protected string MemberName { get; set; }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row.</value>
        protected int? Row { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        protected MetroTextBoxType Type { get; set; }

        /// <summary>
        /// Gets or sets the val.
        /// </summary>
        /// <value>The val.</value>
        protected string Val { get; set; }

        /// <summary>
        /// Gets or sets the mask value.
        /// </summary>
        /// <value>The mask value.</value>
        public string MaskValue { get; set; }
    }
}