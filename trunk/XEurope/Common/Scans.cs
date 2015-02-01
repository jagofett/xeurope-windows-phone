using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope.Common
{
    public class Scans : INotifyPropertyChanged
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id
        {
            get;
            set;
        }
        //The Id property is marked as the Primary Key
        private int idValue;

        private string teamNameValue = String.Empty;
        private string imageNameValue = String.Empty;
        private string codeValue = String.Empty;
        private string votedValue = String.Empty;

        #region Public properties
        public string TeamName
        {
            get { return this.teamNameValue; }

            set
            {
                if (value != this.teamNameValue)
                {
                    this.teamNameValue = value;
                    NotifyPropertyChanged("TeamName");
                }
            }
        }
        
        public string ImageName
        {
            get { return this.imageNameValue; }

            set
            {
                if (value != this.imageNameValue)
                {
                    this.imageNameValue = value;
                    NotifyPropertyChanged("ImageName");
                }
            }
        }

        public string Code
        {
            get { return this.codeValue; }

            set
            {
                if (value != this.codeValue)
                {
                    this.codeValue = value;
                    NotifyPropertyChanged("Code");
                }
            }
        }

        public string Voted
        {
            get { return this.votedValue; }

            set
            {
                if (value != this.votedValue)
                {
                    this.votedValue = value;
                    NotifyPropertyChanged("Voted");
                }
            }
        }
        #endregion

        public Scans() { }
        public Scans(string teamName, string imageName, string code, string voted)
        {
            TeamName = teamName;
            ImageName = imageName;
            Code = code;
            Voted = voted;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    } 
}
