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
            this.SuspendLayout();
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(0, 548);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(576, 20);
            this.hScrollBar.TabIndex = 1;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(556, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(20, 548);
            this.vScrollBar.TabIndex = 2;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
            // 
            // CodeTextbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.hScrollBar);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CodeTextbox";
            this.Size = new System.Drawing.Size(576, 568);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CodeTextbox_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CodeTextbox_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CodeTextbox_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CodeTextbox_MouseDown);
            this.MouseEnter += new System.EventHandler(this.CodeTextbox_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.CodeTextbox_MouseLeave);
            this.MouseHover += new System.EventHandler(this.CodeTextbox_MouseHover);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CodeTextbox_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CodeTextbox_MouseUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CodeTextbox_PreviewKeyDown);
            this.Resize += new System.EventHandler(this.CodeTextbox_Resize);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.VScrollBar vScrollBar;
    }
}
