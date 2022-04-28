namespace Core.Framework.Model.Helper.Odata
{
    /// <summary>
    ///     Interface IOdataQuery
    /// </summary>
    public interface IOdataQuery
    {
        /// <summary>
        ///     Equals the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string Equal(string input);

        /// <summary>
        ///     Nulls the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string Null(string input);

        /// <summary>
        ///     Nots the equal.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string NotEqual(string input);

        /// <summary>
        ///     Greaters the than.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string GreaterThan(string input);

        /// <summary>
        ///     Greaters the than or equal.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string GreaterThanOrEqual(string input);

        /// <summary>
        ///     Lesses the than.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string LessThan(string input);

        /// <summary>
        ///     Lesses the than or equal.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string LessThanOrEqual(string input);

        /// <summary>
        ///     Ands the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string And(string input);

        /// <summary>
        ///     Ors the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string Or(string input);

        /// <summary>
        ///     Nots the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string Not(string input);

        /// <summary>
        ///     Startswithes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string Startswith(string input);

        /// <summary>
        ///     Endwithes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        string Endwith(string input);

        /// <summary>
        ///     Determines whether [contains] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        string Contains(string input);

        /// <summary>
        ///     Creates the filter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>ResultQuery.</returns>
        ResultQuery CreateFilter(string input);
    }
}