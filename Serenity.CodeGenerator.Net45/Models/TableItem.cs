using System.ComponentModel;

namespace Serenity.CodeGenerator
{
    public class TableItem : INotifyPropertyChanged
    {
        private bool isChecked;

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (isChecked != value)
                {
                    this.isChecked = value;
                    Change("IsChecked");
                }
            }
        }

        private string fullName;

        public string FullName
        {
            get
            {
                return fullName;
            }

            set
            {
                if (fullName != value)
                {
                    fullName = value;
                    Change("FullName");
                }
            }
        }

        private string identifier;

        public string Identifier
        {
            get
            {
                return identifier;
            }

            set
            {
                if (identifier != value)
                {
                    identifier = value;
                    Change("Identifier");
                }
            }
        }

        private string module;

        public string Module
        {
            get
            {
                return module;
            }

            set
            {
                if (module != value)
                {
                    module = value;
                    Change("Module");
                }
            }
        }

        private string connectionKey;

        public string ConnectionKey
        {
            get
            {
                return connectionKey;
            }

            set
            {
                if (connectionKey != value)
                {
                    connectionKey = value;
                    Change("ConnectionKey");
                }
            }
        }

        private string permissionKey;

        public string PermissionKey
        {
            get
            {
                return permissionKey;
            }

            set
            {
                if (permissionKey != value)
                {
                    permissionKey = value;
                    Change("PermissionKey");
                }
            }
        }

        protected void Change(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}