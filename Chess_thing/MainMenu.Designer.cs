namespace Chess_thing
{
    partial class MainMenu
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
            this.components = new System.ComponentModel.Container();
            this.AI = new System.Windows.Forms.CheckBox();
            this.TIME = new System.Windows.Forms.CheckBox();
            this.Times = new System.Windows.Forms.ComboBox();
            this.Begin = new System.Windows.Forms.Button();
            this.Comeback = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // AI
            // 
            this.AI.AutoSize = true;
            this.AI.Location = new System.Drawing.Point(12, 12);
            this.AI.Name = "AI";
            this.AI.Size = new System.Drawing.Size(92, 17);
            this.AI.TabIndex = 1;
            this.AI.Text = "Against the AI";
            this.AI.UseVisualStyleBackColor = true;
            this.AI.CheckedChanged += new System.EventHandler(this.Options);
            // 
            // TIME
            // 
            this.TIME.AutoSize = true;
            this.TIME.Location = new System.Drawing.Point(12, 35);
            this.TIME.Name = "TIME";
            this.TIME.Size = new System.Drawing.Size(71, 17);
            this.TIME.TabIndex = 4;
            this.TIME.Text = "Use Time";
            this.TIME.UseVisualStyleBackColor = true;
            this.TIME.CheckedChanged += new System.EventHandler(this.Options);
            // 
            // Times
            // 
            this.Times.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Times.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Times.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Times.FormattingEnabled = true;
            this.Times.Location = new System.Drawing.Point(110, 12);
            this.Times.Name = "Times";
            this.Times.Size = new System.Drawing.Size(77, 33);
            this.Times.TabIndex = 5;
            this.Times.SelectedIndexChanged += new System.EventHandler(this.Times_SelectedIndexChanged);
            // 
            // Begin
            // 
            this.Begin.Location = new System.Drawing.Point(12, 56);
            this.Begin.Name = "Begin";
            this.Begin.Size = new System.Drawing.Size(92, 47);
            this.Begin.TabIndex = 6;
            this.Begin.Text = "BeingGame";
            this.Begin.UseVisualStyleBackColor = true;
            this.Begin.Click += new System.EventHandler(this.Begin_Click);
            // 
            // Comeback
            // 
            this.Comeback.Tick += new System.EventHandler(this.Comeback_Tick);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(198, 115);
            this.Controls.Add(this.Begin);
            this.Controls.Add(this.Times);
            this.Controls.Add(this.TIME);
            this.Controls.Add(this.AI);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainMenu";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main Menu";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox AI;
        private System.Windows.Forms.CheckBox TIME;
        private System.Windows.Forms.ComboBox Times;
        private System.Windows.Forms.Button Begin;
        private System.Windows.Forms.Timer Comeback;
    }
}