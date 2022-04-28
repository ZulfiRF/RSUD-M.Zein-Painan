using System.Web.Mvc;

namespace Core.Framework.Web.Mvc.Contract
{
    /// <summary>
    /// Interface IMetroTextBox digunakn sebagai contrac ketika akan membuat custom text box baru
    /// </summary>
    public interface IMetroTextBox : ICustomControl
    {
        /// <summary>
        /// Requireds the specified is required. digunakan untuk memberikan tanda jika control itu wajib di isi
        /// </summary>
        /// <param name="value">if set to <c>true</c> [is required].</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Required        
        /// <code>
        /// @Html.MetroTextBox().Required(true).Render()
        ///</code>
        ///</example>
        IMetroTextBox Required(bool value);

        /// <summary>
        /// Inputs the atribute. digunakan untuk memberikan atribute pada textbox 
        /// </summary>
        /// <param name="htmlAttribute">berisikan object atribute yang akan di tambahkan</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Attribute        
        /// <code>
        /// @Html.MetroTextBox().Attribute(new{style="width:100%"}).Render()
        ///</code>
        ///</example>
        IMetroTextBox Attribute(object htmlAttribute);

        /// <summary>
        /// Cols the row text box. digunakan jika menggunakan type multiline
        /// </summary>
        /// <param name="col">berisikan column dari text box</param>
        /// <param name="row">berisikan row dari text box.</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  ColRowTextBox        
        /// <code>
        /// @Html.MetroTextBox().ColRowTextBox(4, 3).Render()
        ///</code>
        ///</example>
        IMetroTextBox ColRowTextBox(int col, int row);

        /// <summary>
        /// Withes the display. digunakn untuk memberikan label pada TEXTBOX
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns>IMetroTextBox.</returns>
        ///  <example> contoh yang digunakan untuk  memanggil  WithDisplay        
        /// <code>
        /// @Html.MetroTextBox().WithDisplay("Tanggal Masuk").Render()
        ///</code>
        ///</example>
        IMetroTextBox WithDisplay(string displayName);

        /// <summary>
        /// Members the specified member name. digunakan untk memberikan tag id dan name pada hasil render 
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Member        
        /// <code>
        /// @Html.MetroTextBox().Member("Description").Render()
        ///</code>
        ///</example>
        IMetroTextBox Member(string memberName);

        /// <summary>
        /// Types the text box. digunakan untuk memberikan type pada textbox
        /// </summary>
        /// <param name="type">berisikan type dari textbox</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Member        
        /// <code>
        /// @Html.MetroTextBox().TypeTextBox(MetroTextBoxType.MultiLine).Render()
        ///</code>
        ///</example>
        /// 
        IMetroTextBox TypeTextBox(MetroTextBoxType type);


        /// <summary>
        /// Values the specified val. digunakan untuk memberikan nilai value pada textbox
        /// </summary>
        /// <param name="val">berisikan value yang akan di tampilkan</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Value        
        /// <code>
        /// @Html.MetroTextBox().Value("Nilai Value").Render()
        ///</code>
        ///</example>
        IMetroTextBox Value(string val);

        /// <summary>
        /// Masks the specified val. digunakan untuk memberikan mask pada textbox
        /// </summary>
        /// <param name="val">mask yang digunakan</param>
        /// <returns>IMetroTextBox.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Mask        
        /// <code>
        ///  //a - Represents an alpha character (A-Z,a-z) 
        /// //9 - Represents a numeric character (0-9) 
        /// //* - Represents an alphanumeric character (A-Z,a-z,0-9)
        /// @Html.MetroTextBox().Mask("99:99").Render()
        ///</code>
        ///</example>
        IMetroTextBox Mask(string val);

        /// <summary>
        /// Nots the border. digunakan jika textbox tidak menggunakan border
        /// </summary>
        /// <returns>IMetroTextBox.</returns>
        IMetroTextBox NotBorder();

        /// <summary>
        /// Maxes the length. digunakan untuk membatasi panjang inputan pada textbox
        /// </summary>
        /// <param name="max">berisikan panjang karakter</param>
        /// <returns>IMetroTextBox.</returns>
        IMetroTextBox MaxLength(int max);

        /// <summary>
        /// Mins the length.digunakan untuk membatasi minimal panjang inputan pada textbox
        /// </summary>
        /// <param name="min">berisikan minimal panjang karakter.</param>
        /// <returns>IMetroTextBox.</returns>
        IMetroTextBox MinLength(int min);
    }
}