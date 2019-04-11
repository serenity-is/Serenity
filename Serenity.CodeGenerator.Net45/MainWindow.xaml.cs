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
using System.Windows.Data;

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

            if (!string.IsNullOrEmpty(config.CustomTemplates))
                Templates.TemplatePath = config.CustomTemplates;

            if (config.CustomSettings != null)
            {
                foreach (var pair in config.CustomSettings.OrderBy(x => x.Key))
                {
                    if (pair.Value is Boolean)
                    {
                        var binding = new Binding("CustomSettings[" + pair.Key + "]");
                        binding.Source = this;
                        var checkbox = new CheckBox();
                        checkbox.SetBinding(CheckBox.IsCheckedProperty, binding);
                        checkbox.Content = Inflector.Inflector.Titleize(pair.Key);
                        checkbox.HorizontalAlignment = HorizontalAlignment.Left;
                        checkbox.Margin = new Thickness(10, 0, 10, 0);
                        checkbox.Click += (s, e) =>
                        {
                            CustomSettings[pair.Key] = checkbox.IsChecked;
                            config.Save();
                        };
                        GenerationOptions.Children.Add(checkbox);
                    }
                    else
                    {
                        var dockPanel = new DockPanel();
                        dockPanel.LastChildFill = true;
                        dockPanel.Margin = new Thickness(5);
                        var textBlock = new TextBlock();
                        textBlock.Margin = new Thickness(0, 0, 4, 0);
                        textBlock.Width = 200;
                        textBlock.Text = Inflector.Inflector.Titleize(pair.Key);
                        dockPanel.Children.Add(textBlock);

                        var binding = new Binding("CustomSettings[" + pair.Key + "]");
                        binding.Source = this;
                        var textbox = new TextBox();
                        textbox.SetBinding(TextBox.TextProperty, binding);
                        textbox.TextChanged += (s, e) =>
                        {
                            CustomSettings[pair.Key] = textbox.Text;
                            config.Save();
                        };
                        dockPanel.Children.Add(textbox);
                        GenerationOptions.Children.Add(dockPanel);
                        DockPanel.SetDock(dockPanel, Dock.Top);
                        DockPanel.SetDock(textBlock, Dock.Left);
                        dockPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    }
                }
            }
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

        public Dictionary<string, object> CustomSettings
        {
            get { return config.CustomSettings; }
            set { config.CustomSettings = value; }
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

        public bool GenerateService
        {
            get { return config.GenerateService; }
            set
            {
                if (value != config.GenerateService)
                {
                    config.GenerateService = value;
                    config.Save();
                    Changed("GenerateService");
                }
            }
        }

        public bool GenerateUI
        {
            get { return config.GenerateUI; }
            set
            {
                if (value != config.GenerateUI)
                {
                    config.GenerateUI = value;
                    config.Save();
                    Changed("GenerateUI");
                }
            }
        }

        public bool GenerateCustom
        {
            get { return config.GenerateCustom; }
            set
            {
                if (value != config.GenerateCustom)
                {
                    config.GenerateCustom = value;
                    config.Save();
                    Changed("GenerateCustom");
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

        public string CustomTemplates
        {
            get { return config.CustomTemplates; }
            set
            {
                if (value != config.CustomTemplates)
                {
                    config.CustomTemplates = value;
                    config.Save();
                    Changed("CustomTemplates");
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

        public bool UseDBIdentifiers
        {
            get { return config.UseDBIdentifiers; }
            set
            {
                if (value != config.UseDBIdentifiers)
                {
                    config.UseDBIdentifiers = value;
                    config.Save();
                    Changed("UseDBIdentifiers");
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
                                (Inflector.Inflector.Pascalize(connectionKey) ?? "").Replace(" ", "");

                            var tableItem = new TableItem
                            {
                                IsChecked = false,
                                ConnectionKey = conn.Key,
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

                        var kdiff3Paths = new[]
                        {
                            config.KDiff3Path,
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "KDiff3\\kdiff3.exe"),
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "KDiff3\\kdiff3.exe"),
                        };

                        CodeFileHelper.Kdiff3Path = kdiff3Paths.FirstOrDefault(File.Exists);

                        if (config.TFSIntegration)
                            CodeFileHelper.SetupTFSIntegration(config.TFPath);

                        CodeFileHelper.SetupTSCPath(config.TSCPath);
                        var siteWebProj = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.WebProjectFile));

                        new EntityCodeGenerator(rowModel, config, siteWebProj).Run();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            if (config.GenerateService ||
                config.GenerateUI ||
                config.GenerateCustom)
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

        private void btnCustomTemplates_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (string.IsNullOrWhiteSpace(config.CustomTemplates))
            {
                dlg.FileName = "*.scriban";
                dlg.InitialDirectory = Path.GetDirectoryName(GeneratorConfig.GetConfigurationFilePath());
            }
            else
            {
                dlg.FileName = "*.scriban";
                dlg.InitialDirectory = config.CustomTemplates;
            }

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                CustomTemplates = Path.GetDirectoryName(dlg.FileName);
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