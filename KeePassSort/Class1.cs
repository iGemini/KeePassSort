using System;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePassLib;

namespace KeePassSort
{
    public class KeePassSortExt : Plugin
    {
        private IPluginHost _host;
        private ToolStripMenuItem _tsmiMenuItem;
        private ToolStripSeparator _tsSeparator;

        // Set up menu items etc
        public override bool Initialize(IPluginHost host)
        {
            _host = host;

            var tsMenu = host.MainWindow.ToolsMenu.DropDownItems;

            _tsSeparator = new ToolStripSeparator();
            tsMenu.Add(_tsSeparator);

            _tsmiMenuItem = new ToolStripMenuItem {Text = "Sort Alphabetically"};
            _tsmiMenuItem.Click += OnMenuDoSomething;
            tsMenu.Add(_tsmiMenuItem);

            return true;
        }

        private void OnMenuDoSomething(object sender, EventArgs e)
        {
            if (!_host.Database.IsOpen)
            {
                MessageBox.Show("You first need to open a database.", "KeePass Sort");
                return;
            }

            var comparer = new PwEntryComparer("Title", true, true);
            var root = _host.Database.RootGroup;

            root.Entries.Sort(comparer);

            foreach (var group in root.Groups) group.Entries.Sort(comparer);

            _host.Database.Modified = true;
        }

        // Handle cleanup here
        public override void Terminate()
        {
            var tsMenu = _host.MainWindow.ToolsMenu.DropDownItems;
            _tsmiMenuItem.Click -= OnMenuDoSomething;
            tsMenu.Remove(_tsmiMenuItem);
            tsMenu.Remove(_tsSeparator);
        }
    }
}