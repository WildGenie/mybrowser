using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NCP_Browser
{
    public class BaseForm : BaseBaseForm
    {
        internal ToolStripMenuItem MenuDock { get; set; }

        public void BaseInit()
        {
        }

        internal void generic_Menu(object sender, EventArgs e)
        {
            ((ToolStripEnhanced)sender).form.Activate();
        }
    }
}
