using Serenity;
using Serenity.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Serenity.CodeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BindingList<string> _connections;
        private BindingList<string> _tables;

        public MainWindow()
        {
            InitializeComponent();
            _connections = new BindingList<string>();
            _tables = new BindingList<string>();
            this.ConnectionsCombo.DataContext = _connections;
            this.DataContext = this;

            var file = ConnectionsFile();
            if (File.Exists(file))
                using (var sr = new StreamReader(file))
                {
                    _connections.Clear();
                    foreach (var s in JsonConvert.DeserializeObject<List<string>>(sr.ReadToEnd()))
                        _connections.Add(s);
                }


            Templates.LocationRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views");
            ProjectRoot = System.IO.Path.GetFullPath(@"..\..\..\..\!!!Beni Sil!!!");
        }

        public BindingList<string> Tables { get { return _tables; } }
        public string ProjectRoot { get; set; }
        public string TemplatesRoot { get { return Templates.LocationRoot; } set { Templates.LocationRoot = value; } }

        private string _entitySingular;

        public string EntitySingular
        {
            get { return _entitySingular; }
            set 
            {
                if (value != _entitySingular)
                {
                    _entitySingular = value; 
                    Changed("EntitySingular");
                    GenerateRowCode();
                }
            }
        }

        public string Module
        {
            get { return _module; }
            set
            {
                if (value != _module)
                {
                    _module = value;
                    Changed("Module");
                }
            }
        } private string _module;

        public string Schema
        {
            get { return _schema; }
            set
            {
                if (value != _schema)
                {
                    _schema = value;
                    Changed("Schema");
                }
            }
        } private string _schema;

        public string Permission
        { 
            get { return _permission; }
            set 
            { 
                if (value != _permission) 
                { 
                    _permission = value; 
                    Changed("Permission");
                } 
            }
        } private string _permission;

        private void Changed(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void Ekle_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddConnectionStringWindow();
            if (dlg.ShowDialog() == true)
            {
                var cstr = dlg.ConnectionString.Text.Trim();
                if (cstr.Length == 0)
                    return;
                if (_connections.IndexOf(cstr) < 0)
                    _connections.Add(cstr);
                this.ConnectionsCombo.SelectedItem = cstr;
            }
        }

        private void Sil_Click(object sender, RoutedEventArgs e)
        {
            _connections.Remove((string)this.ConnectionsCombo.SelectedItem);
        }

        private string ConnectionsFile()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Connections.config");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            using (var sw = new StreamWriter(ConnectionsFile()))
            {
                sw.Write(JsonConvert.SerializeObject(_connections.ToList()));
            }
        }

        private IDbConnection CreateConnection(string connStr)
        {
            string provider = "System.Data.SqlClient";
            var idx = connStr.IndexOf("||");
            if (idx >= 0)
            {
                provider = connStr.Substring(idx + 2).Trim();
                connStr = connStr.Substring(0, idx).Trim();
            }

            return SqlConnections.New(connStr, provider);
        }

        private void ConnectionsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._tables.Clear();

            if (this.ConnectionsCombo.SelectedItem != null)
            {
                string connStr = (string)this.ConnectionsCombo.SelectedItem;

                try
                {
                    using (var connection = CreateConnection(connStr))
                    {
                        connection.Open();

                        foreach (var t in SqlSchemaInfo.GetTableNames(connection))
                            _tables.Add(((t.Item1 != null && t.Item1 != "dbo") ? (t.Item1 + ".") : "") + t.Item2);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void GenerateRowCode()
        {
            if (this.ConnectionsCombo.SelectedItem != null &&
                this.TablesCombo.SelectedItem != null &&
                !EntitySingular.IsTrimmedEmpty() &&
                !this.Module.IsTrimmedEmpty())
            {
                string table = (string)this.TablesCombo.SelectedItem;
                string tableSchema = null;
                if (table.IndexOf('.') > 0)
                {
                    tableSchema = table.Substring(0, table.IndexOf('.'));
                    table = table.Substring(table.IndexOf('.') + 1);
                }
                try
                {
                    using (var connection = CreateConnection((string)this.ConnectionsCombo.SelectedItem))
                    {
                        connection.Open();
                        this.GeneratedCode.Text = RowGenerator.Generate(connection, tableSchema, table,
                            Module, Schema, EntitySingular, Permission);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }

        private void TablesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GeneratedCode.Text = null;

            if (this.ConnectionsCombo.SelectedItem != null &&
                this.TablesCombo.SelectedItem != null)
            {
                string tableName = (string)this.TablesCombo.SelectedItem; 
                EntitySingular = "";
                GenerateCodeButton.IsEnabled = true;
                GenerateRowCode();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GenerateCodes_Click(object sender, RoutedEventArgs e)
        {
            if (this.ConnectionsCombo.SelectedItem == null ||
                this.TablesCombo.SelectedItem == null)
            {
                MessageBox.Show("Bir bağlantı ve tablo adı seçmelisiniz!");
                return;
            }

            if (EntitySingular.IsTrimmedEmpty())
            {
                MessageBox.Show("Entity Sınıfı için değer girmelisiniz!");
                return;
            }

            if (Module.IsTrimmedEmpty())
            {
                MessageBox.Show("Modül için değer girmelisiniz!");
                return;
            }

            if (Permission.IsTrimmedEmpty())
            {
                MessageBox.Show("Erişim Hakkı için değer girmelisiniz!");
                return;
            }
   
            string tableName = (string)this.TablesCombo.SelectedItem; 
            try
            {
                EntityCodeGenerationModel rowModel;
                using (var connection = CreateConnection((string)this.ConnectionsCombo.SelectedItem))
                {
                    connection.Open();
                    var table = (string)this.TablesCombo.SelectedItem;
                    string tableSchema = null;
                    if (table.IndexOf('.') > 0)
                    {
                        tableSchema = table.Substring(0, table.IndexOf('.'));
                        table = table.Substring(table.IndexOf('.') + 1);
                    }
                    rowModel = RowGenerator.GenerateModel(connection, tableSchema, table,
                        Module, Schema, EntitySingular, Permission);
                    new EntityCodeGenerator(rowModel, ProjectRoot).Run();

                    MessageBox.Show("Seçilen tablo için kod üretildi!");

                    GenerateCodeButton.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}