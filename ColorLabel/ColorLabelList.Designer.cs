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
            this.doubleBufferedDrawBox = new ajkControls.DoubleBufferedDrawBox();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // doubleBufferedDrawBox
            // 
            this.doubleBufferedDrawBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleBufferedDrawBox.Location = new System.Drawing.Point(0, 0);
            this.doubleBufferedDrawBox.Name = "doubleBufferedDrawBox";
            this.doubleBufferedDrawBox.Size = new System.Drawing.Size(450, 373);
            this.doubleBufferedDrawBox.TabIndex = 0;
            this.doubleBufferedDrawBox.DoubleBufferedPaint += new ajkControls.DoubleBufferedDrawBox.DoubleBufferedPaintHandler(this.doubleBufferedDrawBox_DoubleBufferedPaint);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(450, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(26, 373);
            this.vScrollBar.TabIndex = 1;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            // 
            // ColorLabelList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.doubleBufferedDrawBox);
            this.Controls.Add(this.vScrollBar);
            this.Name = "ColorLabelList";
            this.Size = new System.Drawing.Size(476, 373);
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedDrawBox doubleBufferedDrawBox;
        private System.Windows.Forms.VScrollBar vScrollBar;
    }
}
