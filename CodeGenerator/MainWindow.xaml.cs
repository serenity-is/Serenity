using Serenity.Data;
using Serenity.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace Serenity.CodeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BindingList<GeneratorConfig.Connection> _connections;
        private BindingList<string> _tables;
        private GeneratorConfig config;

        public MainWindow()
        {
            InitializeComponent();

            _connections = new BindingList<GeneratorConfig.Connection>();
            _tables = new BindingList<string>();

            this.ConnectionsCombo.DataContext = _connections;
            this.DataContext = this;

            var configFilePath = GetConfigurationFilePath();
            config = JsConfigHelper.LoadConfig<GeneratorConfig>(configFilePath);
            config.Connections = config.Connections ?? new List<GeneratorConfig.Connection>();
            config.RemoveForeignFields = config.RemoveForeignFields ?? new List<string>();

            if (!config.WebProjectFile.IsEmptyOrNull())
            {
                var webConfig = Path.Combine(Path.GetDirectoryName(config.WebProjectFile), "web.config");
                AddConnectionsFromAppConfig(config.Connections, webConfig);
            }
                

            foreach (var connection in config.Connections)
                _connections.Add(connection);
        }
         
        private void AddConnectionsFromAppConfig(List<GeneratorConfig.Connection> connections, string configFilePath)
        {
            if (File.Exists(configFilePath))
            {
                try
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(File.ReadAllText(configFilePath));
                    var nodes = xml.SelectNodes("//configuration/connectionStrings/add");
                    foreach (XmlElement node in nodes)
                    {
                        var name = node.Attributes["name"];
                        var conn = node.Attributes["connectionString"];
                        var prov = node.Attributes["providerName"];
                        if (name != null && 
                            !string.IsNullOrWhiteSpace(name.Value) &&
                            conn != null &&
                            !string.IsNullOrWhiteSpace(conn.Value) &&
                            prov != null &&
                            !string.IsNullOrWhiteSpace(prov.Value))
                        {
                            var connection = config.Connections.FirstOrDefault(x => String.Compare(x.Key, name.Value, StringComparison.OrdinalIgnoreCase) == 0);
                            if (connection == null)
                            {
                                connection = new GeneratorConfig.Connection();
                                connection.Key = name.Value;
                                connections.Add(connection);
                            }
                            connection.ConnectionString = conn.Value;
                            connection.ProviderName = prov.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            }
        }

        public BindingList<string> Tables { get { return _tables; } }

        public string RootNamespace
        {
            get { return config.RootNamespace; }
            set 
            { 
                if (value != config.RootNamespace) {
                    config.RootNamespace = value;
                    SaveConfig();
                    Changed("RootNamespace");
                }
            }
        }

        public string WebProjectFile
        {
            get { return config.WebProjectFile; }
            set
            {
                if (value != config.WebProjectFile)
                {
                    config.WebProjectFile = value;

                    if (!config.WebProjectFile.IsEmptyOrNull())
                    {
                        var webConfig = Path.Combine(Path.GetDirectoryName(config.WebProjectFile), "web.config");
                        AddConnectionsFromAppConfig(config.Connections, webConfig);
                    }

                    Changed("WebProjectFile");
                }
            }
        }

        public string ScriptProjectFile
        {
            get { return config.ScriptProjectFile; }
            set
            {
                if (value != config.ScriptProjectFile)
                {
                    config.ScriptProjectFile = value;
                    Changed("ScriptProjectFile");
                }
            }
        }

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
                    _module = value.TrimToNull();
                    Changed("Module");
                }
            }
        } private string _module;

        public string ConnectionKey
        {
            get { return _connectionKey; }
            set
            {
                if (value != _connectionKey)
                {
                    _connectionKey = value;
                    Changed("ConnectionKey");
                }
            }
        } private string _connectionKey;

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

        private void ScriptProjectFileBrowse(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = string.IsNullOrWhiteSpace(ScriptProjectFile) ? "*.csproj" : ScriptProjectFile;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                ScriptProjectFile = dlg.FileName;
                SaveConfig();
            }
        }

        private void WebProjectFileBrowse(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = string.IsNullOrWhiteSpace(WebProjectFile) ? "*.csproj" : WebProjectFile;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                WebProjectFile = dlg.FileName;
                SaveConfig();
            }
        }

        private void Ekle_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddConnectionStringWindow();
            if (dlg.ShowDialog() == true)
            {
                var cstr = dlg.Key.Text.Trim();
                if (cstr.Length < 0)
                    throw new ArgumentNullException("connectionKey");

                var connection = config.Connections.FirstOrDefault(x => String.Compare(x.Key, cstr, StringComparison.OrdinalIgnoreCase) == 0);
                if (connection == null)
                {
                    connection = new GeneratorConfig.Connection
                    {
                        Key = cstr,
                        ConnectionString = dlg.ConnectionString.Text.Trim(),
                        ProviderName = dlg.Provider.Text.Trim(),
                        Tables = new List<GeneratorConfig.Table>
                        {

                        }
                    };

                    config.Connections.Add(connection);
                    _connections.Clear();
                    _connections.AddRange(config.Connections);
                }
                else
                {
                    connection.ConnectionString = dlg.ConnectionString.Text.Trim();
                    connection.ProviderName = dlg.Provider.Text.Trim();
                }

                this.ConnectionsCombo.SelectedItem = connection;
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            config.Connections.Sort((x, y) => x.Key.CompareTo(y.Key));
            File.WriteAllText(GetConfigurationFilePath(), JSON.StringifyIndented(config));
        }

        private void Sil_Click(object sender, RoutedEventArgs e)
        {
            var connection = (GeneratorConfig.Connection)this.ConnectionsCombo.SelectedItem;
            config.Connections.Remove(connection);
            _connections.Remove(connection);
            SaveConfig();
        }

        private bool IsNugetPackage()
        {
            var parentPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
            return File.Exists(Path.Combine(parentPath, "repositories.config")) &&
                parentPath.EndsWith(@"packages\", StringComparison.OrdinalIgnoreCase);
        }

        private string GetConfigurationFilePath()
        {
            var configPath = AppDomain.CurrentDomain.BaseDirectory;
            if (IsNugetPackage())
                configPath = Path.GetFullPath(Path.Combine(configPath, @"..\..\..\"));

            return Path.Combine(configPath, "Serenity.CodeGenerator.config");
        }

        private void ConnectionsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._tables.Clear();

            if (this.ConnectionsCombo.SelectedItem != null)
            {
                var conn = (GeneratorConfig.Connection)this.ConnectionsCombo.SelectedItem;

                try
                {
                    using (var connection = SqlConnections.New(conn.ConnectionString, conn.ProviderName)) 
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
                !EntitySingular.IsTrimmedEmpty())
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
                    var conn = (GeneratorConfig.Connection)this.ConnectionsCombo.SelectedItem;
                    using (var connection = SqlConnections.New(conn.ConnectionString, conn.ProviderName)) 
                    {
                        connection.Open();
                        this.GeneratedCode.Text = RowGenerator.Generate(connection, tableSchema, table,
                            Module, ConnectionKey, EntitySingular, Permission, config);
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
                var connection = this.ConnectionsCombo.SelectedItem as GeneratorConfig.Connection;
                var table = connection != null ? connection.Tables.FirstOrDefault(x => x.Tablename == tableName) : null;
                EntitySingular = table == null ? "" : table.Identifier;
                Permission = table == null ? "" : table.PermissionKey;
                ConnectionKey = table != null ? table.ConnectionKey : (connection != null ? connection.Key : "");
                Module = table == null ? "" : table.Module;
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
                MessageBox.Show("A connection string and table name must be selected!");
                return;
            }

            if (EntitySingular.IsTrimmedEmpty())
            {
                MessageBox.Show("Entity class identifier must be entered!");
                return;
            }

            if (Permission.IsTrimmedEmpty())
            {
                MessageBox.Show("Permission key must be entered!");
                return;
            }
   
            string tableName = (string)this.TablesCombo.SelectedItem; 
            try
            {
                EntityCodeGenerationModel rowModel;
                var conn = (GeneratorConfig.Connection)this.ConnectionsCombo.SelectedItem;
                using (var connection = SqlConnections.New(conn.ConnectionString, conn.ProviderName)) 
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
                        Module, ConnectionKey, EntitySingular, Permission, config);
                    new EntityCodeGenerator(rowModel, config).Run();

                    MessageBox.Show("Code files for the selected table is generated!");

                    GenerateCodeButton.IsEnabled = false;
                }

                var cnn = this.ConnectionsCombo.SelectedItem as GeneratorConfig.Connection;
                var tableObj = cnn != null ? cnn.Tables.FirstOrDefault(x => x.Tablename == tableName) : null;
                if (tableObj == null && cnn != null)
                {
                    tableObj = new GeneratorConfig.Table();
                    tableObj.Tablename = tableName;
                    cnn.Tables.Add(tableObj);
                }

                if (tableObj != null)
                {
                    tableObj.Identifier = EntitySingular;
                    tableObj.PermissionKey = Permission;
                    tableObj.Module = Module;
                    tableObj.ConnectionKey = ConnectionKey;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            File.WriteAllText(GetConfigurationFilePath(), JSON.StringifyIndented(config));
        }
    }
}