using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NCP_Browser
{
    public class BaseBaseForm : Form
    {
        public int WindowValue = 0;
        public string Title { get; set; }
        public object Browser { get; set; }
        public ToolStrip webNav { get; set; }
        public void SafeClose()
        {
            System.Threading.Thread t = new System.Threading.Thread(Closer);
            t.Start();
            return;
        }

        private void Closer()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(Closer));
            }
            else
            {
                this.Close();
            }
        }

        public ToolStripButton backButton { get; set; }

        public ToolStripButton forwardButton { get; set; }

        public ToolStripButton haltButton { get; set; }

        public ToolStripButton homeButton { get; set; }

        public ToolStripButton reloadButton { get; set; }

        public string URL { get; set; }
    }
}
