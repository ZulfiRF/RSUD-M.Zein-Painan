using System;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Core.Framework.Model.Impl.WepApi
{
    public class WebApiReader : IDataReader
    {
        private int indexCount;

        public WebApiReader(string result)
        {
            // TODO: Complete member initialization
            Result = result;
            var resultObject = JsonConvert.DeserializeObject(Result) as JObject;

            if (resultObject != null)
            {
                var jProperty = resultObject.Children().ToArray()[1] as JProperty;
                if (jProperty != null)
                    ListResult = jProperty.Children();
                if (ListResult != null)
                    foreach (var child in ListResult)
                    {
                        var count = 0;
                        foreach (var jObject in child.OfType<JObject>())
                        {
                            count = jObject.Count;
                        }
                        FieldCount = count;
                        break;
                    }
            }
            indexCount = 0;
        }

        protected string Result { get; set; }
        protected IJEnumerable<JToken> ListResult { get; set; }

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        #endregion

        #region Implementation of IDataRecord

        /// <summary>
        ///     Gets the name for the field to find.
        /// </summary>
        /// <returns>
        ///     The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public string GetName(int i)
        {
            var jArray = CurrentItem as JArray;
            if (jArray != null)
            {
                var jObject = jArray[0] as JObject;
                if (jObject != null)
                {
                    var jProperty = jObject.Children().ToArray()[i] as JProperty;
                    if (jProperty != null)
                    {
                        return jProperty.Name;
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets the data type information for the specified field.
        /// </summary>
        /// <returns>
        ///     The data type information for the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that
        ///     would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        /// </summary>
        /// <returns>
        ///     The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would
        ///     be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Return the value of the specified field.
        /// </summary>
        /// <returns>
        ///     The <see cref="T:System.Object" /> which will contain the field value upon return.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets all the attribute fields in the collection for the current record.
        /// </summary>
        /// <returns>
        ///     The number of instances of <see cref="T:System.Object" /> in the array.
        /// </returns>
        /// <param name="values">
        ///     An array of <see cref="T:System.Object" /> to copy the attribute fields into.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Return the index of the named field.
        /// </summary>
        /// <returns>
        ///     The index of the named field.
        /// </returns>
        /// <param name="name">
        ///     The name of the field to find.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <returns>
        ///     The value of the column.
        /// </returns>
        /// <param name="i">
        ///     The zero-based column ordinal.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <returns>
        ///     The 8-bit unsigned integer value of the specified column.
        /// </returns>
        /// <param name="i">
        ///     The zero-based column ordinal.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer
        ///     offset.
        /// </summary>
        /// <returns>
        ///     The actual number of bytes read.
        /// </returns>
        /// <param name="i">
        ///     The zero-based column ordinal.
        /// </param>
        /// <param name="fieldOffset">
        ///     The index within the field from which to start the read operation.
        /// </param>
        /// <param name="buffer">
        ///     The buffer into which to read the stream of bytes.
        /// </param>
        /// <param name="bufferoffset">
        ///     The index for <paramref name="buffer" /> to start the read operation.
        /// </param>
        /// <param name="length">
        ///     The number of bytes to read.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the character value of the specified column.
        /// </summary>
        /// <returns>
        ///     The character value of the specified column.
        /// </returns>
        /// <param name="i">
        ///     The zero-based column ordinal.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given
        ///     buffer offset.
        /// </summary>
        /// <returns>
        ///     The actual number of characters read.
        /// </returns>
        /// <param name="i">
        ///     The zero-based column ordinal.
        /// </param>
        /// <param name="fieldoffset">
        ///     The index within the row from which to start the read operation.
        /// </param>
        /// <param name="buffer">
        ///     The buffer into which to read the stream of bytes.
        /// </param>
        /// <param name="bufferoffset">
        ///     The index for <paramref name="buffer" /> to start the read operation.
        /// </param>
        /// <param name="length">
        ///     The number of bytes to read.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns the GUID value of the specified field.
        /// </summary>
        /// <returns>
        ///     The GUID value of the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        ///     The 16-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        ///     The 32-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <returns>
        ///     The 64-bit signed integer value of the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <returns>
        ///     The single-precision floating point number of the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <returns>
        ///     The double-precision floating point number of the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the string value of the specified field.
        /// </summary>
        /// <returns>
        ///     The string value of the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <returns>
        ///     The fixed-position numeric value of the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the date and time data value of the specified field.
        /// </summary>
        /// <returns>
        ///     The date and time data value of the specified field.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns an <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Data.IDataReader" />.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Return whether the specified field is set to null.
        /// </summary>
        /// <returns>
        ///     true if the specified field is set to null; otherwise, false.
        /// </returns>
        /// <param name="i">
        ///     The index of the field to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the number of columns in the current row.
        /// </summary>
        /// <returns>
        ///     When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is
        ///     -1.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int FieldCount { get; private set; }

        /// <summary>
        ///     Gets the column located at the specified index.
        /// </summary>
        /// <returns>
        ///     The column located at the specified index as an <see cref="T:System.Object" />.
        /// </returns>
        /// <param name="i">
        ///     The zero-based index of the column to get.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        object IDataRecord.this[int i]
        {
            get
            {
                var jArray = CurrentItem as JArray;
                if (jArray != null)
                {
                    var jObject = jArray[0] as JObject;
                    if (jObject != null)
                    {
                        var jProperty = jObject.Children().ToArray()[i] as JProperty;
                        if (jProperty != null)
                        {
                            var jValue = jProperty.Value as JValue;
                            if (jValue != null)
                            {
                                return jValue.Value;
                            }
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        ///     Gets the column with the specified name.
        /// </summary>
        /// <returns>
        ///     The column with the specified name as an <see cref="T:System.Object" />.
        /// </returns>
        /// <param name="name">
        ///     The name of the column to find.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     No column with the specified name was found.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        object IDataRecord.this[string name]
        {
            get { return null; }
        }

        #endregion

        #region Implementation of IDataReader

        /// <summary>
        ///     Closes the <see cref="T:System.Data.IDataReader" /> Object.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Close()
        {
        }

        /// <summary>
        ///     Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the
        ///     <see cref="T:System.Data.IDataReader" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The <see cref="T:System.Data.IDataReader" /> is closed.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        ///     true if there are more rows; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Advances the <see cref="T:System.Data.IDataReader" /> to the next record.
        /// </summary>
        /// <returns>
        ///     true if there are more rows; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool Read()
        {
            if (ListResult == null)
                return false;
            CurrentItem = ListResult.ToArray()[indexCount];
            indexCount++;
            return ListResult.Count() + 1 != indexCount;
        }

        protected IJEnumerable<JToken> CurrentItem { get; set; }

        /// <summary>
        ///     Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <returns>
        ///     The level of nesting.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int Depth { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <returns>
        ///     true if the data reader is closed; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsClosed { get; private set; }

        /// <summary>
        ///     Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        /// <returns>
        ///     The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for
        ///     SELECT statements.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int RecordsAffected { get; private set; }

        #endregion
    }
}