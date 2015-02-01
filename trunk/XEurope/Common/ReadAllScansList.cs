using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEurope.Common;

namespace XEurope.Common
{
    public class ReadAllContactsList
    {
        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
        public  ObservableCollection<Scans> GetAllContacts()
        {
            return Db_Helper.ReadScans();
        }
    }
}
