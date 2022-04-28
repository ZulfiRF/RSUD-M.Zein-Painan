using System;
using System.Runtime.Serialization;

namespace Core.Framework.Helper.Services
{
    [DataContract]
    public class Message
    {
        #region Enums

        public enum TypeMessage
        {
            NewPasien,

            UpdatePasien,

            CountingPasien,

            Other
        }

        #endregion

        #region Public Properties

        [DataMember]
        public string Address { get; set; }

        public string JenisPasien { get; set; }
        public ICallBackService CallBack { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Module { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public TypeMessage Type { get; set; }

        #endregion
    }
}