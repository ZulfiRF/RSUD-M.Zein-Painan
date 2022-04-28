
using System.Web.Mvc;

namespace Core.Framework.Web.Mvc.Contract
{
    /// <summary>
    /// Interface IComboMetro
    /// </summary>
    public interface IComboMetro : ICustomControl
    {
        /// <summary>
        /// Adds the item. digunakan untuk menambahkan Item pada combobox
        /// </summary>
        /// <param name="item">berisikan item yang akan di tambahkan pada combobox</param>
        /// <returns>IComboMetro.</returns>
        ///  <example> contoh yang digunakan untuk  memanggil  Add Item with Single Item   
        /// <code>
        /// @Html.MetroComboBox().AddItem(new SelectListItem(){Value = "Value",Selected = true,Text = "Text"}).Render()
        ///</code>
        ///</example>
        IComboMetro AddItem(SelectListItem item);

        /// <summary>
        /// Adds the item. digunakan untuk menambahkan List Item pada combobox
        /// </summary>
        /// <param name="item">berisikan List item yang akan di tambahkan pada combobox</param>
        /// <returns>IComboMetro.</returns>
        ///  <example> contoh yang digunakan untuk  memanggil  Add Item with List Item         
        /// <code>
        /// @Html.MetroComboBox().AddItem(new SelectListItem[] { new SelectListItem() { Value = "Value", Selected = true, Text = "Text" } }).Render()
        ///</code>
        ///</example>
        IComboMetro AddItem(SelectListItem[] item);

        /// <summary>
        /// Attributes the specified HTML attribute. digunakan untuk memberikan atribute pada combobox
        /// </summary>
        /// <param name="htmlAttribute">berisikan object atribute yang akan di tambahkan</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Attribute        
        /// <code>
        /// @Html.MetroComboBox().Attribute(new { display = "Choose Level Difficulty", data_bind = "options: listLevel,optionsText: 'name',optionsValue:'type',optionsCaption: 'Choose...'" }).Render()
        ///</code>
        ///</example>
        IComboMetro Attribute(object htmlAttribute);

        /// <summary>
        /// Members the specified member name. digunakan untk memberikan tag id dan name pada hasil render 
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>IComboMetro.</returns>
        IComboMetro Member(string memberName);



        /// <summary>
        /// Requireds the specified is required. digunakan untuk memberikan tanda jika control itu wajib di isi
        /// </summary>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Required        
        /// <code>
        /// @Html.MetroComboBox().Required(true).Render()
        ///</code>
        ///</example>
        IComboMetro Required(bool isRequired);

        /// <summary>
        /// URLs the source. digunakan untuk end point dari data yang di tampilkan pada combo box
        /// </summary>
        /// <param name="url">berisikan url endpoint dari list data</param>
        /// <param name="keyMember">berisikan nilai yang akan di ambil</param>
        /// <param name="valueMember">berisikan nilai yang akan di tampilkan di combobox</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil Url Source        
        /// <code>
        /// // {val} merupakan tanda yang digunakan untuk mengambil text hasil dari text box
        /// @Html.MetroComboBox().UrlSource(Url.Content("/LogBook/Manage/GetListEmploye?name={val}"), "ID", "Name").Render()
        ///</code>
        ///</example>
        IComboMetro UrlSource(string url, string keyMember, string valueMember);

        /// <summary>
        /// Values the specified value. digunakan untuk memberikan nilai dari combo box
        /// </summary>
        /// <param name="value">berisikan nilai dari combo box yang di pilih</param>
        /// <returns>IComboMetro.</returns>
        ///  <example> contoh yang digunakan untuk  memanggil  Value        
        /// <code>
        /// @Html.MetroComboBox().Value("01").Render()
        ///</code>
        ///</example>
        IComboMetro Value(string value);

        /// <summary>
        /// Withes the display. digunakn untuk memberikan label pada combo box
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns>IComboMetro.</returns>
        ///  <example> contoh yang digunakan untuk  memanggil  WithDisplay        
        /// <code>
        /// @Html.MetroComboBox().WithDisplay("Jenis kelamin").Render()
        ///</code>
        ///</example>
        IComboMetro WithDisplay(string displayName);
    }
}