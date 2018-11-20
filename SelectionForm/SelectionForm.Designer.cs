namespace ajkControls
{
    partial class SelectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ajkControls.CodeDrawStyle codeDrawStyle1 = new ajkControls.CodeDrawStyle();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.doubleBufferedDrawBox = new ajkControls.DoubleBufferedDrawBox();
            this.codeTextbox = new ajkControls.CodeTextbox();
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(257, 27);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(26, 382);
            this.vScrollBar.TabIndex = 1;
            // 
            // doubleBufferedDrawBox
            // 
            this.doubleBufferedDrawBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleBufferedDrawBox.Location = new System.Drawing.Point(0, 27);
            this.doubleBufferedDrawBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.doubleBufferedDrawBox.Name = "doubleBufferedDrawBox";
            this.doubleBufferedDrawBox.Size = new System.Drawing.Size(257, 382);
            this.doubleBufferedDrawBox.TabIndex = 2;
            this.doubleBufferedDrawBox.DoubleBufferedPaint += new ajkControls.DoubleBufferedDrawBox.DoubleBufferedPaintHandler(this.doubleBufferedDrawBox_DoubleBufferedPaint);
            // 
            // codeTextbox
            // 
            this.codeTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.codeTextbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.codeTextbox.Document = null;
            this.codeTextbox.Editable = true;
            this.codeTextbox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.codeTextbox.Location = new System.Drawing.Point(0, 0);
            this.codeTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.codeTextbox.MultiLine = false;
            this.codeTextbox.Name = "codeTextbox";
            this.codeTextbox.ScrollBarVisible = false;
            this.codeTextbox.Size = new System.Drawing.Size(283, 27);
            this.codeTextbox.Style = codeDrawStyle1;
            this.codeTextbox.TabIndex = 0;
            this.codeTextbox.AfterKeyPressed += new System.Windows.Forms.KeyPressEventHandler(this.codeTextbox_AfterKeyPressed);
            this.codeTextbox.BeforeKeyDown += new System.Windows.Forms.KeyEventHandler(this.codeTextbox_BeforeKeyDown);
            this.codeTextbox.VisibleChanged += new System.EventHandler(this.codeTextbox_VisibleChanged);
            // 
            // SelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 409);
            this.Controls.Add(this.doubleBufferedDrawBox);
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.codeTextbox);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SelectionForm";
            this.Text = "SelectionForm";
            this.Deactivate += new System.EventHandler(this.SelectionForm_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private CodeTextbox codeTextbox;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private DoubleBufferedDrawBox doubleBufferedDrawBox;
    }
}