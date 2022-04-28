using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Model.Attr;
using Microsoft.CSharp;

namespace Core.Framework.Model
{
    [Serializable]
    public class ProfileTable : TableItem
    {
        public ProfileItem Profile
        {
            get
            {
                var repo = BaseDependency.Get<IProfileRepository>();

                if (repo != null)
                    return
                        repo.CurrentProfile();
                return null;
            }
        }
        [Field("KdProfile", true)]
        public Int16 KdProfile
        {
            get
            {
                try
                {
                    var connectionManager = BaseDependency.Get<ISettingRepository>();
                    if (connectionManager == null)
                        return 0;
                    if (connectionManager.CurrentProfile() == null)
                        return 0;
                    return Convert.ToInt16(connectionManager.CurrentProfile().CodeProfile);
                }
                catch (Exception)
                {
                    return Convert.ToInt16(base["KdProfile"]);
                }

            }
            set { base["KdProfile"] = value; }
        }
    }


}
