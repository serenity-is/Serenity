using Serenity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Serenity.CodeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BindingList<GeneratorConfig.Connection> _connections;
        private BindingList<TableItem> _tables;
        private GeneratorConfig config;

        public MainWindow()
        {
            InitializeComponent();

            var loadProviderDLLs = (ConfigurationManager.AppSettings["LoadProviderDLLs"] ?? "")
                .Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var dll in loadProviderDLLs)
                try
                {
                    Assembly.LoadFrom(dll);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't load: " + dll + "\n" + ex.ToString());
                }

            _connections = new BindingList<GeneratorConfig.Connection>();
            _tables = new BindingList<TableItem>();

            this.ConnectionsCombo.DataContext = _connections;
            this.DataContext = this;

            this.config = GeneratorConfig.Load();

            foreach (var connection in config.Connections)
                _connections.Add(connection);

            if (!config.WebProjectFile.IsEmptyOrNull())
                config.UpdateConnectionsFrom(GetWebConfigLocation(), x => _connections.Add(x));
        }

        public BindingList<TableItem> Tables { get { return _tables; } }

        public string RootNamespace
        {
            get { return config.RootNamespace; }
            set
            {
                if (value != config.RootNamespace)
                {
                    config.RootNamespace = value;
                    config.Save();
                    Changed("RootNamespace");
                }
            }
        }

        public string GetWebConfigLocation()
        {
            return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.WebProjectFile))),
                    "web.config");
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
                        config.UpdateConnectionsFrom(GetWebConfigLocation(), x => _connections.Add(x));
                        Changed("Connections");
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
                }
            }
        }
       
        private void Changed(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void ScriptProjectFileBrowse(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (string.IsNullOrWhiteSpace(ScriptProjectFile))
            {
                dlg.FileName = "*.csproj";
                dlg.InitialDirectory = Path.GetDirectoryName(GeneratorConfig.GetConfigurationFilePath());
            }
            else
            {
                var scriptProjectFile = Path.GetFullPath(ScriptProjectFile);
                dlg.FileName = Path.GetFileName(scriptProjectFile);
                dlg.InitialDirectory = Path.GetDirectoryName(scriptProjectFile);
            }

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                ScriptProjectFile = GeneratorConfig.GetRelativePath(dlg.FileName, AppDomain.CurrentDomain.BaseDirectory);
                config.Save();
            }
        }

        private void WebProjectFileBrowse(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (string.IsNullOrWhiteSpace(WebProjectFile))
            {
                dlg.FileName = "*.csproj";
                dlg.InitialDirectory = Path.GetDirectoryName(GeneratorConfig.GetConfigurationFilePath());
            }
            else
            {
                var webProjectFile = Path.GetFullPath(WebProjectFile);
                dlg.FileName = Path.GetFileName(webProjectFile);
                dlg.InitialDirectory = Path.GetDirectoryName(webProjectFile);
            }

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                WebProjectFile = GeneratorConfig.GetRelativePath(dlg.FileName, AppDomain.CurrentDomain.BaseDirectory);
                config.Save();
            }
        }

        public bool GenerateTSCode
        {
            get { return config.GenerateTSCode; }
            set
            {
                if (value != config.GenerateTSCode)
                {
                    config.GenerateTSCode = value;
                    config.Save();
                    Changed("GenerateTSCode");
                }
            }
        }

        public bool GenerateTSTypings
        {
            get { return config.GenerateTSTypings; }
            set
            {
                if (value != config.GenerateTSTypings)
                {
                    config.GenerateTSTypings = value;
                    config.Save();
                    Changed("GenerateTSTypings");
                }
            }
        }

        public bool GenerateSSImports
        {
            get { return config.GenerateSSImports; }
            set
            {
                if (value != config.GenerateSSImports)
                {
                    config.GenerateSSImports = value;
                    config.Save();
                    Changed("GenerateSSImports");
                }
            }
        }

        public List<GeneratorConfig.BaseRowClass> BaseRowClasses
        {
            get { return config.BaseRowClasses; }
            set
            {
                if (value != config.BaseRowClasses)
                {
                    config.BaseRowClasses = value;
                    config.Save();
                    Changed("BaseRowClasses");
                }
            }
        }

        public List<string> RemoveForeignFields
        {
            get { return config.RemoveForeignFields; }
            set
            {
                if (value != config.RemoveForeignFields)
                {
                    config.RemoveForeignFields = value;
                    config.Save();
                    Changed("RemoveForeignFields");
                }
            }
        }

        public bool RowFieldsSurroundWithRegion
        {
            get { return config.RowFieldsSurroundWithRegion; }
            set
            {
                if (value != config.RowFieldsSurroundWithRegion)
                {
                    config.RowFieldsSurroundWithRegion = value;
                    config.Save();
                    Changed("RowFieldsSurroundWithRegion");
                }
            }
        }

        public bool GenerateLookupEditor
        {
            get { return config.GenerateLookupEditor; }
            set
            {
                if (value != config.GenerateLookupEditor)
                {
                    config.GenerateLookupEditor = value;
                    config.Save();
                    Changed("GenerateLookupEditor");
                }
            }
        }

        public bool MaximizableDialog
        {
            get { return config.MaximizableDialog; }
            set
            {
                if (value != config.MaximizableDialog)
                {
                    config.MaximizableDialog = value;
                    config.Save();
                    Changed("MaximizableDialog");
                }
            }
        }

        public bool FieldDecriptionasPlaceholder
        {
            get { return config.FieldDecriptionasPlaceholder; }
            set
            {
                if (value != config.FieldDecriptionasPlaceholder)
                {
                    config.FieldDecriptionasPlaceholder = value;
                    config.Save();
                    Changed("FieldDecriptionasPlaceholder");
                }
            }
        }

        

        public string KDiff3Path
        {
            get { return config.KDiff3Path; }
            set
            {
                if (value != config.KDiff3Path)
                {
                    config.KDiff3Path = value;
                    config.Save();
                    Changed("KDiff3Path");
                }
            }
        }

        public string TSCPath
        {
            get { return config.TSCPath; }
            set
            {
                if (value != config.TSCPath)
                {
                    config.TSCPath = value;
                    config.Save();
                    Changed("TSCPath");
                }
            }
        }

        public bool GenerateRow
        {
            get { return config.GenerateRow; }
            set
            {
                if (value != config.GenerateRow)
                {
                    config.GenerateRow = value;
                    config.Save();
                    Changed("GenerateRow");
                }
            }
        }
        public bool GenerateColumn
        {
            get { return config.GenerateColumn; }
            set
            {
                if (value != config.GenerateColumn)
                {
                    config.GenerateColumn = value;
                    config.Save();
                    Changed("GenerateColumn");
                }
            }
        }
        public bool GenerateForm
        {
            get { return config.GenerateForm; }
            set
            {
                if (value != config.GenerateForm)
                {
                    config.GenerateForm = value;
                    config.Save();
                    Changed("GenerateForm");
                }
            }
        }
        public bool GenerateEndpoint
        {
            get { return config.GenerateEndpoint; }
            set
            {
                if (value != config.GenerateEndpoint)
                {
                    config.GenerateEndpoint = value;
                    config.Save();
                    Changed("GenerateEndpoint");
                }
            }
        }
        public bool GenerateRepository
        {
            get { return config.GenerateRepository; }
            set
            {
                if (value != config.GenerateRepository)
                {
                    config.GenerateRepository = value;
                    config.Save();
                    Changed("GenerateRepository");
                }
            }
        }
        public bool GeneratePage
        {
            get { return config.GeneratePage; }
            set
            {
                if (value != config.GeneratePage)
                {
                    config.GeneratePage = value;
                    config.Save();
                    Changed("GeneratePage");
                }
            }
        }

        public bool GenerateGrid
        {
            get { return config.GenerateGrid; }
            set
            {
                if (value != config.GenerateGrid)
                {
                    config.GenerateGrid = value;
                    config.Save();
                    Changed("GenerateGrid");
                }
            }
        }
        public bool GenerateDialog
        {
            get { return config.GenerateDialog; }
            set
            {
                if (value != config.GenerateDialog)
                {
                    config.GenerateDialog = value;
                    config.Save();
                    Changed("GenerateDialog");
                }
            }
        }
        public bool GenerateGridEditor
        {
            get { return config.GenerateGridEditor; }
            set
            {
                if (value != config.GenerateGridEditor)
                {
                    config.GenerateGridEditor = value;
                    config.Save();
                    Changed("GenerateGridEditor");
                }
            }
        }
        public bool GenerateGridEditorDialog
        {
            get { return config.GenerateGridEditorDialog; }
            set
            {
                if (value != config.GenerateGridEditorDialog)
                {
                    config.GenerateGridEditorDialog = value;
                    config.Save();
                    Changed("GenerateGridEditorDialog");
                }
            }
        }
        private void AddConnection_Click(object sender, RoutedEventArgs e)
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
                config.Save();
            }
        }

        private void DeleteConnection_Click(object sender, RoutedEventArgs e)
        {
            var connection = (GeneratorConfig.Connection)this.ConnectionsCombo.SelectedItem;
            config.Connections.Remove(connection);
            _connections.Remove(connection);
            config.Save();
        }

        private void ConnectionsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._tables.Clear();
            GenerateCodeButton.IsEnabled = false;

            if (this.ConnectionsCombo.SelectedItem != null)
            {
                var conn = (GeneratorConfig.Connection)this.ConnectionsCombo.SelectedItem;

                try
                {
                    using (var connection = SqlConnections.New(conn.ConnectionString, conn.ProviderName))
                    {
                        connection.Open();

                        var schemaProvider = SchemaHelper.GetSchemaProvider(connection.GetDialect().ServerType);
                        foreach (var t in schemaProvider.GetTableNames(connection))
                        {
                            var table = conn != null ? conn.Tables.FirstOrDefault(x => x.Tablename == t.Tablename) : null;
                            var identifier = (table == null || table.Identifier.IsEmptyOrNull()) ?
                                RowGenerator.ClassNameFromTableName(t.Table) : table.Identifier;
                            var permission = table == null ? "Administration:General" : table.PermissionKey;
                            var connectionKey = (table != null && !table.ConnectionKey.IsEmptyOrNull()) ?
                                table.ConnectionKey : conn.Key;

                            var module = (table != null && table.Module != null) ? table.Module :
                                Inflector.Inflector.Capitalize(connectionKey);

                            var tableItem = new TableItem
                            {
                                IsChecked = false,
                                //ConnectionKey = conn.Key,
                                ConnectionKey = connectionKey,
                                Module = module,
                                Identifier = identifier,
                                PermissionKey = permission,
                                FullName = t.Tablename
                            };

                            _tables.Add(tableItem);
                            tableItem.PropertyChanged += (s, e2) =>
                            {
                                var t2 = conn.Tables.FirstOrDefault(x => x.Tablename == tableItem.FullName);
                                if (t2 == null)
                                {
                                    t2 = new GeneratorConfig.Table();
                                    t2.Tablename = tableItem.FullName;
                                    conn.Tables.Add(t2);
                                }
                                t2.Identifier = tableItem.Identifier;
                                t2.Module = tableItem.Module;
                                t2.ConnectionKey = tableItem.ConnectionKey;
                                t2.PermissionKey = tableItem.PermissionKey;
                                this.config.Save();

                                GenerateCodeButton.IsEnabled = _tables.Any(x => x.IsChecked);
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GenerateCodes_Click(object sender, RoutedEventArgs e)
        {
            var conn = (GeneratorConfig.Connection)this.ConnectionsCombo.SelectedItem;
            if (conn == null)
            {
                MessageBox.Show("A connection must be selected!");
                return;
            }

            var tables = this._tables.Where(x => x.IsChecked == true);
            if (this.ConnectionsCombo.SelectedItem == null)
            {
                MessageBox.Show("Please select at least one table!");
                return;
            };

            var noIdentifier = tables.FirstOrDefault(x => x.Identifier.IsTrimmedEmpty());
            if (noIdentifier != null)
            {
                MessageBox.Show("Identifier for table " + noIdentifier.FullName + " is empty!");
                return;
            };

            foreach (var table in tables)
            {
                try
                {
                    EntityModel rowModel;
                    
                    using (var connection = SqlConnections.New(conn.ConnectionString, conn.ProviderName))
                    {
                        connection.Open();
                        var tableName = table.FullName;
                        string schema = null;
                        if (tableName.IndexOf('.') > 0)
                        {
                            schema = tableName.Substring(0, tableName.IndexOf('.'));
                            tableName = tableName.Substring(tableName.IndexOf('.') + 1);
                        }

                        rowModel = RowGenerator.GenerateModel(connection, schema, tableName,
                            table.Module, table.ConnectionKey, table.Identifier, table.PermissionKey, config);

                        new EntityCodeGenerator(rowModel, config).Run();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            if (config.GenerateTSCode ||
                config.GenerateTSTypings)
            {
                var siteWebProj = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.WebProjectFile));
                var siteWebPath = Path.GetDirectoryName(siteWebProj);
                CodeFileHelper.ExecuteTSC(Path.Combine(siteWebPath, @"Scripts\"), "");
            }

            MessageBox.Show("Code files for the selected table is generated. Please REBUILD SOLUTION before running application, otherwise you may have script errors!");
            GenerateCodeButton.IsEnabled = false;
        }

        private void btnKDiff3PathBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (string.IsNullOrWhiteSpace(config.KDiff3Path))
            {
                dlg.FileName = "*.exe";
                dlg.InitialDirectory = Path.GetDirectoryName(GeneratorConfig.GetConfigurationFilePath());
            }
            else
            {
                dlg.FileName = Path.GetFileName(config.KDiff3Path);
                dlg.InitialDirectory = Path.GetDirectoryName(config.KDiff3Path);
            }

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                KDiff3Path = dlg.FileName;
                config.Save();
            }
        }

        private void btnTSCPathBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (string.IsNullOrWhiteSpace(config.TSCPath))
            {
                dlg.FileName = "*.exe";
                dlg.InitialDirectory = Path.GetDirectoryName(GeneratorConfig.GetConfigurationFilePath());
            }
            else
            {
                dlg.FileName = Path.GetFileName(config.TSCPath);
                dlg.InitialDirectory = Path.GetDirectoryName(config.TSCPath);
            }

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                TSCPath = dlg.FileName;
                config.Save();
            }
        }

        private void TablesGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space && TablesGrid.SelectedItem != null)
                (TablesGrid.SelectedItem as TableItem).IsChecked = !(TablesGrid.SelectedItem as TableItem).IsChecked;
        }
    }
}