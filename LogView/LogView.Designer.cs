namespace ajkControls.LogView
{
    partial class LogView
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
            this.components = new System.ComponentModel.Container();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.dbDrawBox = new ajkControls.Primitive.DoubleBufferedDrawBox();
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(500, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(26, 370);
            this.vScrollBar.TabIndex = 1;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // dbDrawBox
            // 
            this.dbDrawBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbDrawBox.Location = new System.Drawing.Point(0, 0);
            this.dbDrawBox.Name = "dbDrawBox";
            this.dbDrawBox.Size = new System.Drawing.Size(500, 370);
            this.dbDrawBox.TabIndex = 0;
            this.dbDrawBox.DoubleBufferedPaint += new ajkControls.Primitive.DoubleBufferedDrawBox.DoubleBufferedPaintHandler(this.dbDrawBox_DoubleBufferedPaint);
            this.dbDrawBox.Resize += new System.EventHandler(this.dbDrawBox_Resize);
            // 
            // LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dbDrawBox);
            this.Controls.Add(this.vScrollBar);
            this.Name = "LogView";
            this.Size = new System.Drawing.Size(526, 370);
            this.ResumeLayout(false);

        }

        #endregion

        private ajkControls.Primitive.DoubleBufferedDrawBox dbDrawBox;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.Timer timer;
    }
}
