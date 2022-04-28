using System.Collections.Generic;

namespace Core.Framework.Helper.Contracts
{
    public interface ISetting
    {
        string PathFile { get; }
        string Save(string key, string value);
        string Save(string key, string value, bool useEncryte);
        string Log(string key, string value);
        string Update(string key, string newValue);
        string Delete(string key);
        string GetValue(string key);
        IEnumerable<string> GetValues(string key);
        string Save(string key,IEnumerable<string> values);
        string[] GetAllKey();
        string GetPathFile();
    }
}