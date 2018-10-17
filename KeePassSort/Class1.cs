using System;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePassLib;
using KeePassSort.Utility;

namespace KeePassSort
{
    public class KeePassSortExt : Plugin
    {
        private IPluginHost _host;
        //private ToolStripMenuItem _tsmiCase;
        private ToolStripMenuItem _tsmiMenuItem;
        private ToolStripSeparator _tsSeparator;

        // Set up menu items etc
        public override bool Initialize(IPluginHost host)
        {
            _host = host;

            var tsMenu = host.MainWindow.ToolsMenu.DropDownItems;

            _tsSeparator = new ToolStripSeparator();
            tsMenu.Add(_tsSeparator);

            _tsmiMenuItem = new ToolStripMenuItem {Text = "KeePass Sorter"};
            var _tsmiAZ = new ToolStripMenuItem("A -> Z", null, AZClicked);
            var _tsmiZA = new ToolStripMenuItem("Z -> A", null, ZAClicked);

            /*
            var _tsmiCase = new ToolStripMenuItem("Case Sensitive?")
            {
                CheckOnClick = true
            };
            */

            _tsmiMenuItem.DropDownItems.Add(_tsmiAZ);
            _tsmiMenuItem.DropDownItems.Add(_tsmiZA);
            _tsmiMenuItem.DropDownItems.Add(_tsSeparator);
            //_tsmiMenuItem.DropDownItems.Add(_tsmiCase);

            tsMenu.Add(_tsmiMenuItem);

            return true;
        }

        private void AZClicked(object sender, EventArgs e)
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
            _host.MainWindow.RefreshEntriesList();
        }

        private void ZAClicked(object sender, EventArgs e)
        {
            if (!_host.Database.IsOpen)
            {
                MessageBox.Show("You first need to open a database.", "KeePass Sort");
                return;
            }

            var comparer = new CompareZA("Title", true, true);
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
            tsMenu.Remove(_tsmiMenuItem);
            tsMenu.Remove(_tsSeparator);
        }
    }
}