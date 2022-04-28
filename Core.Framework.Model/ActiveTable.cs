using System;
using System.ComponentModel;
using Core.Framework.Model.Attr;

namespace Core.Framework.Model
{
    [Serializable]
    public class ActiveTable : ProfileTable
    {
        
        [Field("StatusEnabled", SpesicicationType.NotAllowNull)]
        [DefaultValue(1)]
        public byte StatusEnabled
        {

            get
            {
                //if (IsNew) return 1;
                return Convert.ToByte(base["StatusEnabled"]);
            }
            set { base["StatusEnabled"] = value; }
        }
    }
}
