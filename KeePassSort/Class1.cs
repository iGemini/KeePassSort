using System;
using System.Collections.Generic;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeePassSort.Utility;

namespace KeePassSort
{
    public class KeePassSortExt : Plugin
    {
        private IPluginHost _host;
        private bool _sortRecursively;
        private const string OptionsRecursiveSort = "KeePassSort_SortRecursively";

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
            tsmi.DropDownItems.Add(menuItemAscending);

            var menuItemDescending = new ToolStripMenuItem {Text = "Z -> A"};
            menuItemDescending.Click += DescendingClicked;
            tsmi.DropDownItems.Add(menuItemDescending);

            tsmi.DropDownItems.Add(new ToolStripSeparator());

            var menuItemSortRecursively = new ToolStripMenuItem {Text = "Sort recursively?"};
            menuItemSortRecursively.Click += RecurseMenuItemClicked;
            tsmi.DropDownItems.Add(menuItemSortRecursively);

            tsmi.DropDownOpening += delegate
            {
                var db = _host.Database;
                var dbOpen = db != null && db.IsOpen;
                menuItemAscending.Enabled = dbOpen;
                menuItemDescending.Enabled = dbOpen;

                UIUtil.SetChecked(menuItemSortRecursively, _sortRecursively);
            };

            return tsmi;
        }

        private void RecurseMenuItemClicked(object sender, EventArgs e)
        {
            _sortRecursively = !_sortRecursively;
        }

        // Set up menu items etc
        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false;
            _host = host;

            _sortRecursively = _host.CustomConfig.GetBool(OptionsRecursiveSort, false);

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

            if (_sortRecursively == true)
            {
                var root = _host.Database.RootGroup;

                SortGroupsRecursively(comparer, root);
            }

            else
            {
                var group = _host.MainWindow.GetSelectedGroup();
                group.Entries.Sort(comparer);
            }

            _host.MainWindow.UpdateUI(false, null, false, _host.MainWindow.GetSelectedGroup(), true, null, false);
            _host.Database.Save(null);
            
        }

        private void SortGroupsRecursively(IComparer<PwEntry> comparer, PwGroup group)
        {
            group.Entries.Sort(comparer);

            foreach (var g in group.Groups) SortGroupsRecursively(comparer, g);
        }

        // Handle cleanup here
        public override void Terminate()
        {
            _host.CustomConfig.SetBool(OptionsRecursiveSort, _sortRecursively);
        }
    }
}