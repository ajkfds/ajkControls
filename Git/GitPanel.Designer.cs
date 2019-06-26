namespace ajkControls.Git
{
    partial class GitPanel
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
            this.components = new System.ComponentModel.Container();
            this.refreshBtn = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel = new System.Windows.Forms.Panel();
            this.fetchBtn = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tableView = new ajkControls.TableView.TableView();
            this.logView = new ajkControls.LogView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeView = new ajkControls.TreeView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // refreshBtn
            // 
            this.refreshBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshBtn.Location = new System.Drawing.Point(3, 4);
            this.refreshBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(91, 96);
            this.refreshBtn.TabIndex = 1;
            this.refreshBtn.Text = "Refresh";
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.PullBtn_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(184, 568);
            this.splitter1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(575, 4);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // panel
            // 
            this.panel.Controls.Add(this.fetchBtn);
            this.panel.Controls.Add(this.refreshBtn);
            this.panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(759, 104);
            this.panel.TabIndex = 4;
            // 
            // fetchBtn
            // 
            this.fetchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fetchBtn.Location = new System.Drawing.Point(100, 4);
            this.fetchBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fetchBtn.Name = "fetchBtn";
            this.fetchBtn.Size = new System.Drawing.Size(91, 96);
            this.fetchBtn.TabIndex = 2;
            this.fetchBtn.Text = "fetch";
            this.fetchBtn.UseVisualStyleBackColor = true;
            this.fetchBtn.Click += new System.EventHandler(this.FetchBtn_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // tableView
            // 
            this.tableView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.tableView.Columns = 5;
            this.tableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableView.Font = new System.Drawing.Font("Meiryo UI", 8F);
            this.tableView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.tableView.HeaderHeight = 0;
            this.tableView.Location = new System.Drawing.Point(184, 104);
            this.tableView.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tableView.Name = "tableView";
            this.tableView.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(91)))), ((int)(((byte)(125)))), ((int)(((byte)(159)))));
            this.tableView.Size = new System.Drawing.Size(575, 464);
            this.tableView.StretchableCoulmn = 4;
            this.tableView.TabIndex = 0;
            // 
            // logView
            // 
            this.logView.BackColor = System.Drawing.Color.Silver;
            this.logView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logView.Font = new System.Drawing.Font("Consolas", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.logView.Location = new System.Drawing.Point(184, 572);
            this.logView.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.logView.MaxLogs = 200;
            this.logView.Name = "logView";
            this.logView.Size = new System.Drawing.Size(575, 107);
            this.logView.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.treeView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 104);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(181, 575);
            this.panel1.TabIndex = 5;
            // 
            // treeView
            // 
            this.treeView.BackColor = System.Drawing.Color.White;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HScrollBarVisible = true;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeView.Name = "treeView";
            this.treeView.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(224)))), ((int)(((byte)(232)))));
            this.treeView.Size = new System.Drawing.Size(181, 575);
            this.treeView.TabIndex = 0;
            this.treeView.VScrollBarVisible = true;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(181, 104);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 575);
            this.splitter2.TabIndex = 6;
            this.splitter2.TabStop = false;
            // 
            // GitPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableView);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.logView);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "GitPanel";
            this.Size = new System.Drawing.Size(759, 679);
            this.panel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableView.TableView tableView;
        private System.Windows.Forms.Button refreshBtn;
        private LogView logView;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button fetchBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter2;
        private TreeView treeView;
    }
}
