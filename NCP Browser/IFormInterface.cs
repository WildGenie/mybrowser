using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCP_Browser
{
    interface IFormInterface
    {
        bool TopLevel {get; set;}
        void Activate();
        System.Windows.Forms.FormWindowState WindowState {get; set;}
    }
}
