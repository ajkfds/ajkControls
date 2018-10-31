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
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr IParam);

        private const int WM_SETFONT = 0x30;
        private const int WM_FONTCHANGE = 0x1d;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.OnFontChanged(EventArgs.Empty);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            IntPtr hFont = this.Font.ToHfont();
            SendMessage(this.Handle, WM_SETFONT, hFont, (IntPtr) (- 1));
            SendMessage(this.Handle, WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);
            this.UpdateStyles();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //TabControlの背景を塗る
            e.Graphics.FillRectangle(SystemBrushes.Control, this.ClientRectangle);

            if (this.TabPages.Count == 0)
                return;

            //TabPageの枠を描画する
            System.Windows.Forms.TabPage page = this.TabPages[this.SelectedIndex];
            Rectangle pageRect = new Rectangle(
                page.Bounds.X - 2,
                page.Bounds.Y - 2,
                page.Bounds.Width + 5,
                page.Bounds.Height + 5);
            TabRenderer.DrawTabPage(e.Graphics, pageRect);

            //タブを描画する
            for (int i = 0; i < this.TabPages.Count; i++)
            {
                page = this.TabPages[i];
                Rectangle tabRect = this.GetTabRect(i);

                //表示するタブの状態を決定する
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

                //選択されたタブとページの間の境界線を消すために、
                //描画する範囲を大きくする
                if (this.SelectedIndex == i)
                {
                    if (this.Alignment == TabAlignment.Top)
                    {
                        tabRect.Height += 1;
                    }
                    else if (this.Alignment == TabAlignment.Bottom)
                    {
                        tabRect.Y -= 2;
                        tabRect.Height += 2;
                    }
                    else if (this.Alignment == TabAlignment.Left)
                    {
                        tabRect.Width += 1;
                    }
                    else if (this.Alignment == TabAlignment.Right)
                    {
                        tabRect.X -= 2;
                        tabRect.Width += 2;
                    }
                }

                //画像のサイズを決定する
                Size imgSize;
                if (this.Alignment == TabAlignment.Left ||
                    this.Alignment == TabAlignment.Right)
                {
                    imgSize = new Size(tabRect.Height, tabRect.Width);
                }
                else
                {
                    imgSize = tabRect.Size;
                }

                //Bottomの時はTextを表示しない（Textを回転させないため）
                string tabText = page.Text;
                if (this.Alignment == TabAlignment.Bottom)
                {
                    tabText = "";
                }

                //タブの画像を作成する
                Bitmap bmp = new Bitmap(imgSize.Width, imgSize.Height);
                Graphics g = Graphics.FromImage(bmp);
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    //                    g = Graphics.FromImage(bmp);
                    //                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //                    
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(page.Text,
                        page.Font,
                        SystemBrushes.ControlText,
                        new RectangleF(0, 0, bmp.Width, bmp.Height + 1),
                        sf);
                    g.Dispose();
                    sf.Dispose();
                }
                //高さに1足しているのは、下にできる空白部分を消すため
                //TabRenderer.DrawTabItem(g,
                //    new Rectangle(0, 0, bmp.Width, bmp.Height + 1),
                //    tabText,
                //    page.Font,
                //    false,
                //    state);
                //g.Dispose();

                //画像を回転する
                if (this.Alignment == TabAlignment.Bottom)
                {
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                else if (this.Alignment == TabAlignment.Left)
                {
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
                else if (this.Alignment == TabAlignment.Right)
                {
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }

                //Bottomの時はTextを描画する
                if (this.Alignment == TabAlignment.Bottom)
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g = Graphics.FromImage(bmp);
                    g.DrawString(page.Text,
                        page.Font,
                        SystemBrushes.ControlText,
                        new RectangleF(0, 0, bmp.Width, bmp.Height),
                        sf);
                    g.Dispose();
                    sf.Dispose();
                }

                //画像を描画する
                e.Graphics.DrawImage(bmp, tabRect.X, tabRect.Y, bmp.Width, bmp.Height);

                bmp.Dispose();
            }
        }
    }
}
