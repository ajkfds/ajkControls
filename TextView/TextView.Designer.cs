namespace ajkControls
{
    partial class TextView
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
            this.dBDrawBox = new ajkControls.DoubleBufferedDrawBox();
            this.SuspendLayout();
            // 
            // dBDrawBox
            // 
            this.dBDrawBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dBDrawBox.Location = new System.Drawing.Point(0, 0);
            this.dBDrawBox.Name = "dBDrawBox";
            this.dBDrawBox.Size = new System.Drawing.Size(505, 475);
            this.dBDrawBox.TabIndex = 0;
            this.dBDrawBox.DoubleBufferedPaint += new ajkControls.DoubleBufferedDrawBox.DoubleBufferedPaintHandler(this.dBDrawBox_DoubleBufferedPaint);
            // 
            // TextView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dBDrawBox);
            this.Name = "TextView";
            this.Size = new System.Drawing.Size(505, 475);
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedDrawBox dBDrawBox;
    }
}
