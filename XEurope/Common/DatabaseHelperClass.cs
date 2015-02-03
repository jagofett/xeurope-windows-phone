using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope.Common
{
    //This class for perform all database CRUID operations
    public class DatabaseHelperClass
    {
        SQLiteConnection dbConn;
       
        //Create Tabble
        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<Scans>();
                    }
                } 
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Retrieve the specific scans from the database.
        public Scans ReadScan(int contactid)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingScan = dbConn.Query<Scans>("select * from Scans where Id =" + contactid).FirstOrDefault();
                return existingScan;
            }
        }
        public Scans ReadScan(string code)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingScan = dbConn.Table<Scans>().Where(x => x.Code == code).FirstOrDefault();
                return existingScan;
            }
        }
        // Retrieve the all scans list from the database.
        public ObservableCollection<Scans> ReadScans()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Scans> myCollection  = dbConn.Table<Scans>().ToList<Scans>();
                ObservableCollection<Scans> ScansList = new ObservableCollection<Scans>(myCollection);
                return ScansList;
            }
        }
        
        //Update existing scan
        public void UpdateScan(Scans scan)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingScan = dbConn.Table<Scans>().Where(x => x.Code == scan.Code).FirstOrDefault();// dbConn.Query<Scans>("select * from Scans where TeamName =" + scan.TeamName).FirstOrDefault();
                if (existingScan != null)
                {
                    existingScan.TeamName = scan.TeamName;
                    existingScan.ImageName = scan.ImageName;
                    existingScan.Code = scan.Code;
                    existingScan.Voted = scan.Voted;

                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingScan);
                    });
                }
            }
        }
        // Insert the new scan in the Scans table.
        public void Insert(Scans newScan)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                    {
                        dbConn.Insert(newScan);
                    });
            }
        }
       
        //Delete specific scan
        public void DeleteScan(int Id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingScan = dbConn.Query<Scans>("select * from Scans where Id =" + Id).FirstOrDefault();
                if (existingScan != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingScan);
                    });
                }
            }
        }
        //Delete all scanlist or delete Scans table
        public void DeleteAllScan()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                //dbConn.RunInTransaction(() =>
                //   {
                       dbConn.DropTable<Scans>();
                       dbConn.CreateTable<Scans>();
                       dbConn.Dispose();
                       dbConn.Close();
                   //});
            }
        }
    }
}
