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
            this.codeTextbox = new ajkControls.CodeTextbox();
            this.candidatesTreeView = new ajkControls.TreeView();
            this.SuspendLayout();
            // 
            // codeTextbox
            // 
            this.codeTextbox.BackColor = System.Drawing.Color.White;
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
            this.codeTextbox.TabIndex = 0;
            // 
            // candidatesTreeView
            // 
            this.candidatesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.candidatesTreeView.Location = new System.Drawing.Point(0, 27);
            this.candidatesTreeView.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.candidatesTreeView.Name = "candidatesTreeView";
            this.candidatesTreeView.Size = new System.Drawing.Size(283, 382);
            this.candidatesTreeView.TabIndex = 1;
            // 
            // SelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 409);
            this.Controls.Add(this.candidatesTreeView);
            this.Controls.Add(this.codeTextbox);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SelectionForm";
            this.Text = "SelectionForm";
            this.ResumeLayout(false);

        }

        #endregion

        private CodeTextbox codeTextbox;
        private TreeView candidatesTreeView;
    }
}