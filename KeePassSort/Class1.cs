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

        private void AscendingClicked(object sender, EventArgs e)
        {
            SortEntries(false);
        }

        private void DescendingClicked(object sender, EventArgs e)
        {
            SortEntries(true);
        }

        public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
        {
            if (t != PluginMenuType.Main) return null;

            var tsmi = new ToolStripMenuItem {Text = "KeePass Sorter"};

            var menuItemAscending = new ToolStripMenuItem {Text = "A -> Z"};
            menuItemAscending.Click += AscendingClicked;

            var menuItemDescending = new ToolStripMenuItem {Text = "Z -> A"};
            menuItemDescending.Click += DescendingClicked;

            tsmi.DropDownItems.Add(menuItemAscending);
            tsmi.DropDownItems.Add(menuItemDescending);
            tsmi.DropDownItems.Add(new ToolStripSeparator());

            return tsmi;
        }

        // Set up menu items etc
        public override bool Initialize(IPluginHost host)
        {
            _host = host;
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

            _host.Database.Modified = true;
            _host.Database.Save(null);
        }

        private void SortGroups(IComparer<PwEntry> comparer, PwGroup group)
        {
            group.Entries.Sort(comparer);

            foreach (var g in group.Groups) SortGroups(comparer, g);
        }

        // Handle cleanup here
        public override void Terminate()
        {
        }
    }
}