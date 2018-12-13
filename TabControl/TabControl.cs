using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ajkControls
{
    public class TabControl : System.Windows.Forms.TabControl
    {
        public TabControl()
            : base()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.Padding = new Point(this.Padding.X + FontHeight / 2, this.Padding.Y);
            this.ImageList = paddingImages;
            paddingImages.ImageSize = new Size(FontHeight, FontHeight);
            paddingImages.Images.Add(new Bitmap(4, 4));
        }

        System.Windows.Forms.ImageList paddingImages = new ImageList();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr IParam);

        private const int WM_SETFONT = 0x30;
        private const int WM_FONTCHANGE = 0x1d;

        public static Icon closeButtonIcon = new Icon(Properties.Resources.closeBottun);

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.OnFontChanged(EventArgs.Empty);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            IntPtr hFont = this.Font.ToHfont();
            SendMessage(this.Handle, WM_SETFONT, hFont, (IntPtr)(-1));
            SendMessage(this.Handle, WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);
            this.UpdateStyles();
            paddingImages.ImageSize = new Size(FontHeight, FontHeight);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(SystemBrushes.Control, this.ClientRectangle);

            if (this.TabPages.Count == 0)
                return;

            System.Windows.Forms.TabPage page = this.TabPages[this.SelectedIndex];
            ajkControls.TabPage tabPage = page as ajkControls.TabPage;

            Rectangle pageRect = new Rectangle(
                page.Bounds.X - 2,
                page.Bounds.Y - 2,
                page.Bounds.Width + 5,
                page.Bounds.Height + 5);
            TabRenderer.DrawTabPage(e.Graphics, pageRect);

            for (int i = 0; i < this.TabPages.Count; i++)
            {
                page = this.TabPages[i];
                tabPage = page as ajkControls.TabPage;

                Rectangle tabRect = this.GetTabRect(i);

                System.Windows.Forms.VisualStyles.TabItemState state;
                if (!this.Enabled)
                {
                    state = System.Windows.Forms.VisualStyles.TabItemState.Disabled;
                }
                else if (this.SelectedIndex == i)
                {
                    state = System.Windows.Forms.VisualStyles.TabItemState.Selected;
                }
                else
                {
                    state = System.Windows.Forms.VisualStyles.TabItemState.Normal;
                }

                Bitmap bmp = new Bitmap(tabRect.Width, tabRect.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    using (StringFormat sf = new StringFormat())
                    {
                        int left = 0;
                        int width = bmp.Width;

                        if (tabPage != null && tabPage.Icon != null)
                        {
                            g.DrawImage(tabPage.Icon.GetImage(FontHeight, Icon.ColorStyle.Blue), new Point(0, 1));
                            left = left + FontHeight;
                            width = width - FontHeight;
                        }
                        if (tabPage != null && tabPage.CloseButtonEnable)
                        {
                            g.DrawImage(closeButtonIcon.GetImage(FontHeight, Icon.ColorStyle.White), new Point(bmp.Width - FontHeight, 1));
                            width = width - FontHeight;
                        }

                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        g.DrawString(page.Text,
                            page.Font,
                            SystemBrushes.ControlText,
                            new RectangleF(left, 0, width, bmp.Height + 1),
                            sf);
                        g.DrawRectangle(SystemPens.WindowFrame, 0, 0, bmp.Width - 1, bmp.Height);
                    }
                }

                e.Graphics.DrawImage(bmp, tabRect.X, tabRect.Y, bmp.Width, bmp.Height);

                bmp.Dispose();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TabControl_MouseClick);
            this.ResumeLayout(false);
        }

        private void TabControl_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < this.TabPages.Count; i++)
            {
                Rectangle tabRect = this.GetTabRect(i);
                if(e.X > tabRect.Left && e.Y > tabRect.Top && e.X < tabRect.Width+tabRect.Left && e.Y < tabRect.Height + tabRect.Top)
                {
                    System.Windows.Forms.TabPage page = this.TabPages[i];
                    ajkControls.TabPage tabPage = page as ajkControls.TabPage;
                    if (tabPage == null) break;
                    if (!tabPage.CloseButtonEnable) break;
                    if(e.X > tabRect.Left + tabRect.Width - FontHeight)
                    {
                        tabPage.CloseButtonClicked();
                        break;
                    }
                    break;
                }
            }
        }
    }
}
