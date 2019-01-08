namespace ajkControls
{
    partial class CodeTextbox
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.dbDrawBox = new ajkControls.DoubleBufferedDrawBox();
            this.SuspendLayout();
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(0, 537);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(493, 20);
            this.hScrollBar.TabIndex = 1;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(473, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(20, 537);
            this.vScrollBar.TabIndex = 2;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
            // 
            // dbDrawBox
            // 
            this.dbDrawBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbDrawBox.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dbDrawBox.Location = new System.Drawing.Point(0, 0);
            this.dbDrawBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dbDrawBox.Name = "dbDrawBox";
            this.dbDrawBox.Size = new System.Drawing.Size(473, 537);
            this.dbDrawBox.TabIndex = 0;
            this.dbDrawBox.DoubleBufferedPaint += new ajkControls.DoubleBufferedDrawBox.DoubleBufferedPaintHandler(this.dbDrawBox_DoubleBufferedPaint);
            this.dbDrawBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dbDrawBox_KeyDown);
            this.dbDrawBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dbDrawBox_KeyPress);
            this.dbDrawBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dbDrawBox_KeyUp);
            this.dbDrawBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dbDrawBox_MouseDoubleClick);
            this.dbDrawBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dbDrawBox_MouseDown);
            this.dbDrawBox.MouseLeave += new System.EventHandler(this.dbDrawBox_MouseLeave);
            this.dbDrawBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dbDrawBox_MouseMove);
            this.dbDrawBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dbDrawBox_MouseUp);
            // 
            // CodeTextbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dbDrawBox);
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.hScrollBar);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CodeTextbox";
            this.Size = new System.Drawing.Size(493, 557);
            this.Resize += new System.EventHandler(this.CodeTextbox_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedDrawBox dbDrawBox;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.VScrollBar vScrollBar;
    }
}
