using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NCP_Browser.Forms.Controls
{
    /// <summary>
    /// 
    /// </summary>
    //TODO: Disable Paginate Right Button When Current Date

    public partial class ToolStripItemPaginator : ToolStripItem
    {
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        private SendPaginationDate PaginationHandler;
        Rectangle boundingBoxLeft;
        Rectangle boundingBoxRight;
        DateTime PageDate;

        public ToolStripItemPaginator(SendPaginationDate PaginationHandler)
        {
            this.PaginationHandler = PaginationHandler;
            base.AutoSize = false;
            base.Height = 40;
            base.Width = 250;
            base.Font = new System.Drawing.Font("Consolas", 12);
            this.PageDate = DateTime.Now.Date;
            UpdateUI();
            InitializeComponent();
        }

        private void UpdateUI()
        {
            this.Text = this.PageDate.ToString("MM/dd/yyyy");
        }

        Bitmap SavedPixels;
        Form FrozenPixels;
        
        

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Create Bounding Boxes to Detect Clicks
            boundingBoxLeft = new Rectangle(e.ClipRectangle.Left, e.ClipRectangle.Top, 38, 38);
            boundingBoxRight = new Rectangle(e.ClipRectangle.Right - 40, e.ClipRectangle.Top, 38, 38);
            
            // Create a bitmap to store
            //SavedPixels = new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height, System.Drawing.Imaging.PixelFormat.Format64bppPArgb);
            var gfx = e.Graphics;
            
            //gfx.CopyFromScreen(this.Parent.Location.X, this.Parent.Location.Y, this.Parent.Location.X+50, this.Parent.Location.Y+50, new Size(50,50));
            gfx.DrawString(Text, Font, new SolidBrush(ForeColor), e.ClipRectangle.Left + 75, e.ClipRectangle.Top + 12);
            Image x = Image.FromHbitmap(NCP_Browser.Properties.Resources.back.GetHbitmap());
            gfx.DrawImage(x, e.ClipRectangle.Left, e.ClipRectangle.Top, 38, 38);
            Image x2 = Image.FromHbitmap(NCP_Browser.Properties.Resources.forward.GetHbitmap());
            gfx.DrawImage(x2, e.ClipRectangle.Right - 40, e.ClipRectangle.Top, 38, 38);

            //e.Graphics.DrawImage(Image.FromHbitmap(SavedPixels.GetHbitmap()), 0, 0);

            

            

            //e.Graphics.CopyFromScreen(e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Right, e.ClipRectangle.Bottom, new Size(e.ClipRectangle.Width, e.ClipRectangle.Height), CopyPixelOperation.SourceCopy);
        }

        protected override void OnClick(EventArgs e)
        {
            // Determine what button was pressed
            POINT point32;
            GetCursorPos(out point32);
            var x = this.Parent.PointToClient(point32);
            
            // Remove Padd and the well on the left side
            x.X = x.X - 34;
            x.Y = x.Y - 4;

            if(boundingBoxLeft.Contains(x))
            {
                PageDate = PageDate.AddDays(-1);
            }
            else if (boundingBoxRight.Contains(x))
            {
                PageDate = PageDate.AddDays(1);
            }
            else
            {
                // Keep Date, Reload
            }
            this.UpdateUI();
            this.Invalidate();
            PaginationHandler.Invoke(PageDate);
            
        }

        internal void ShowFrozenPixels()
        {
            /*
            FrozenPixels = new Form();
            FrozenPixels.BackColor = Color.White;
            FrozenPixels.FormBorderStyle = FormBorderStyle.None;
            FrozenPixels.Width = SavedPixels.Width;
            FrozenPixels.Height = SavedPixels.Height;
            FrozenPixels.BackgroundImage = Image.FromHbitmap(SavedPixels.GetHbitmap());
            FrozenPixels.TopMost = true;
            FrozenPixels.StartPosition = FormStartPosition.Manual;
            FrozenPixels.Show();
            */
        }

        internal void HideFrozenPixels()
        {
            /*
            FrozenPixels.Close();
            */
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }

    

    public delegate void SendPaginationDate(DateTime ShowThisDate);
}
