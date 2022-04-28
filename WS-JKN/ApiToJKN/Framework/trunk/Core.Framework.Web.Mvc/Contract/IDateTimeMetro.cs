using System;

namespace Core.Framework.Web.Mvc.Contract
{
    /// <summary>
    /// Interface IDateTimeMetro
    /// </summary>
    public interface IDateTimeMetro : ICustomControl
    {
        /// <summary>
        /// Dates the format. digunakan untuk mengatur format dari date time yang akan di tampilkan
        /// </summary>
        /// <param name="format">berisikan format dari date time yang akan di tampilan</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  DateFormat        
        /// <code>
        /// @Html.MetroDateTime().DateFormat("dd-mm-yyyy").Render()
        ///</code>
        ///</example>
        IDateTimeMetro DateFormat(string format);

        /// <summary>
        /// Defaults the date. digunakn untuk memberikan nilai default pada date time
        /// </summary>
        /// <param name="date">berisikan tanggal sekarang yang akan di set</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  DefaultDate        
        /// <code>
        /// @Html.MetroDateTime().DefaultDate(DateTime.Now).Render()
        ///</code>
        ///</example>
        IDateTimeMetro DefaultDate(DateTime date);

        /// <summary>
        /// Withes the display. digunakn untuk memberikan label pada date time metro
        /// </summary>
        /// <param name="display">The display name.</param>
        /// <returns>IComboMetro.</returns>
        ///  <example> contoh yang digunakan untuk  memanggil  DisplayName        
        /// <code>
        /// @Html.MetroDateTime().DisplayName("Tanggal Masuk").Render()
        ///</code>
        ///</example>
        IDateTimeMetro DisplayName(string display);

        /// <summary>
        /// Inputs the atribute. digunakan untuk memberikan atribute pada textbox date time
        /// </summary>
        /// <param name="htmlAttribute">berisikan object atribute yang akan di tambahkan</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  InputAtribute        
        /// <code>
        /// @Html.MetroDateTime().DisplayName("Tanggal Masuk").Render()
        ///</code>
        ///</example>
        IDateTimeMetro InputAtribute(object htmlAttribute);

        /// <summary>
        /// Members the specified member name. digunakan untk memberikan tag id dan name pada hasil render 
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Member        
        /// <code>
        /// @Html.MetroDateTime().Member("StartDate").Render()
        ///</code>
        ///</example>
        IDateTimeMetro Member(string memberName);

        /// <summary>
        /// Reads the only. digunakan jika date time tidak bisa dirubah oleh user
        /// </summary>
        /// <param name="readOnly">if set to <c>true</c> [is readOnly]</param>
        /// <returns>IDateTimeMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  ReadOnly        
        /// <code>
        /// @Html.MetroDateTime().ReadOnly(true).Render()
        ///</code>
        ///</example>
        IDateTimeMetro ReadOnly(string readOnly);

        /// <summary>
        /// Requireds the specified is required. digunakan untuk memberikan tanda jika control itu wajib di isi
        /// </summary>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns>IComboMetro.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Required        
        /// <code>
        /// @Html.MetroDateTime().Required(true).Render()
        ///</code>
        ///</example>
        IDateTimeMetro Required(bool isRequired);
    }
}