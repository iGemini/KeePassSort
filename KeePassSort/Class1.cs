using System;
using System.Collections.Generic;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePassLib;
using KeePassSort.Utility;

namespace KeePassSort
{
    public class KeePassSortExt : Plugin
    {
        private IPluginHost _host;
        private ToolStripMenuItem _pluginMenuItem;
        private ToolStripSeparator _separator;

        // Set up menu items etc
        public override bool Initialize(IPluginHost host)
        {
            _host = host;

            var tsMenu = host.MainWindow.ToolsMenu.DropDownItems;

            _separator = new ToolStripSeparator();
            tsMenu.Add(_separator);

            _pluginMenuItem = new ToolStripMenuItem {Text = "KeePass Sorter"};
            var menuItemAscending = new ToolStripMenuItem("A -> Z", null, AscendingClicked);
            var menuItemDescending = new ToolStripMenuItem("Z -> A", null, DescendingClicked);

            _pluginMenuItem.DropDownItems.Add(menuItemAscending);
            _pluginMenuItem.DropDownItems.Add(menuItemDescending);
            _pluginMenuItem.DropDownItems.Add(_separator);

            tsMenu.Add(_pluginMenuItem);

            return true;
        }

        private void AscendingClicked(object sender, EventArgs e)
        {
            SortEntries(false);
        }

        private void DescendingClicked(object sender, EventArgs e)
        {
            SortEntries(true);
        }

        private void SortEntries(bool descending)
        {
            if (!_host.Database.IsOpen)
            {
                MessageBox.Show("You first need to open a database.", "KeePass Sort");
                return;
            }

            var comparer = descending
                ? (IComparer<PwEntry>) new CompareDescending("Title", true, true)
                : new PwEntryComparer("Title", true, true);

            var root = _host.Database.RootGroup;

            root.Entries.Sort(comparer);

            foreach (var group in root.Groups) group.Entries.Sort(comparer);

            _host.Database.Modified = true;
            _host.MainWindow.RefreshEntriesList();
        }

        // Handle cleanup here
        public override void Terminate()
        {
            var tsMenu = _host.MainWindow.ToolsMenu.DropDownItems;
            tsMenu.Remove(_pluginMenuItem);
            tsMenu.Remove(_separator);
        }
    }
}