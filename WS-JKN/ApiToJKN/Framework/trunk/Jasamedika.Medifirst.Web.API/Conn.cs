using System.Threading;
using Medifirst.PointOfService.Impl;

namespace Jasamedika.Medifirst.Web.API
{
    public class Connection
    {
        private static string _conn;//=@"Data Source=serveroltp\ss2008_dev;Initial Catalog=medifirst2000;Persist Security Info=True;User ID=sa;Password=inisql2008";

        public static string Conn
        {
            get
            {
                try
                {
                    if (_conn == string.Empty)
                        _conn = new PointService().GetSetting("adoConnectionSIMKES");
                    else
                        if (_conn == null)
                            _conn = new PointService().GetSetting("adoConnectionSIMKES");
                        else
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                try
                                {
                                    _conn = new PointService().GetSetting("adoConnectionSIMKES");
                                }
                                catch (System.Exception)
                                {
                                }
                            });
                        }
                    return _conn;
                }
                catch (System.Exception)
                {
                    return _conn;
                }
            }
            private set
            {
                _conn = value;
            }
        }
    }
}