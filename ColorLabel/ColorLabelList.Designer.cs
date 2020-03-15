namespace ajkControls
{
    partial class ColorLabelList
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.doubleBufferedDrawBox = new ajkControls.DoubleBufferedDrawBox();
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(593, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(26, 497);
            this.vScrollBar.TabIndex = 1;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            // 
            // doubleBufferedDrawBox
            // 
            this.doubleBufferedDrawBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleBufferedDrawBox.Location = new System.Drawing.Point(0, 0);
            this.doubleBufferedDrawBox.Margin = new System.Windows.Forms.Padding(4);
            this.doubleBufferedDrawBox.Name = "doubleBufferedDrawBox";
            this.doubleBufferedDrawBox.Size = new System.Drawing.Size(593, 497);
            this.doubleBufferedDrawBox.TabIndex = 0;
            this.doubleBufferedDrawBox.DoubleBufferedPaint += new ajkControls.DoubleBufferedDrawBox.DoubleBufferedPaintHandler(this.doubleBufferedDrawBox_DoubleBufferedPaint);
            this.doubleBufferedDrawBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.doubleBufferedDrawBox_KeyDown);
            this.doubleBufferedDrawBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.doubleBufferedDrawBox_KeyPress);
            this.doubleBufferedDrawBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.doubleBufferedDrawBox_KeyUp);
            this.doubleBufferedDrawBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.doubleBufferedDrawBox_MouseDown);
            this.doubleBufferedDrawBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.doubleBufferedDrawBox_PreviewKeyDown);
            // 
            // ColorLabelList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.doubleBufferedDrawBox);
            this.Controls.Add(this.vScrollBar);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ColorLabelList";
            this.Size = new System.Drawing.Size(619, 497);
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedDrawBox doubleBufferedDrawBox;
        private System.Windows.Forms.VScrollBar vScrollBar;
    }
}
