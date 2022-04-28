namespace Core.Framework.Helper.Contracts
{
    using System.Collections.Generic;

    public interface ISetting
    {
        #region Public Properties

        string PathFile { get; }

        #endregion

        #region Public Methods and Operators

        string Delete(string key);

        string[] GetAllKey();

        string GetPathFile();

        string GetValue(string key);
        string GetKey(string value);

        IEnumerable<string> GetValues(string key);

        string Log(string key, string value);

        string Save(string key, string value);

        string Save(string key, string value, bool useEncryte);

        string Save(string key, IEnumerable<string> values);

        string Update(string key, string newValue);

        #endregion
    }
}