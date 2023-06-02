namespace DemonstratorArray
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.demonstratorListBox = new System.Windows.Forms.ListBox();
            this.demonstratorWorker = new System.ComponentModel.BackgroundWorker();
            this.demonstratorTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // demonstratorListBox
            // 
            this.demonstratorListBox.BackColor = System.Drawing.Color.Gold;
            this.demonstratorListBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.demonstratorListBox.FormattingEnabled = true;
            this.demonstratorListBox.ItemHeight = 15;
            this.demonstratorListBox.Items.AddRange(new object[] {
            "(empty)"});
            this.demonstratorListBox.Location = new System.Drawing.Point(1, 0);
            this.demonstratorListBox.Name = "demonstratorListBox";
            this.demonstratorListBox.Size = new System.Drawing.Size(799, 450);
            this.demonstratorListBox.TabIndex = 0;
            // 
            // demonstratorWorker
            // 
            this.demonstratorWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.StartDemo);
            this.demonstratorWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.StopDemo);
            // 
            // demonstratorTimer
            // 
            this.demonstratorTimer.Interval = 500;
            this.demonstratorTimer.Tick += new System.EventHandler(this.HandleTick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.demonstratorListBox);
            this.Name = "Form1";
            this.Text = "DemonstratorArray";
            this.ResumeLayout(false);

        }

        #endregion

        private ListBox demonstratorListBox;
        private System.ComponentModel.BackgroundWorker demonstratorWorker;
        private System.Windows.Forms.Timer demonstratorTimer;
    }
}