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

        private void AscendingClicked(object sender, EventArgs e)
        {
            SortEntries(false);
        }

        private void DescendingClicked(object sender, EventArgs e)
        {
            SortEntries(true);
        }

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

            SortGroups(comparer, root);
            
            // root.Entries.Sort(comparer);

            // foreach (var group in root.Groups) SortGroups(comparer, group);

            _host.Database.Modified = true;
        }

        private void SortGroups(IComparer<PwEntry> comparer, PwGroup group)
        {
            group.Entries.Sort(comparer);

            foreach (var g in group.Groups) SortGroups(comparer, g);
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