namespace WindowsFormsApp1
{
    partial class FormNotes
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
            this.textBox_Notes = new System.Windows.Forms.TextBox();
            this.button_SaveExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_Notes
            // 
            this.textBox_Notes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Notes.Location = new System.Drawing.Point(12, 155);
            this.textBox_Notes.Multiline = true;
            this.textBox_Notes.Name = "textBox_Notes";
            this.textBox_Notes.Size = new System.Drawing.Size(560, 283);
            this.textBox_Notes.TabIndex = 0;
            // 
            // button_SaveExit
            // 
            this.button_SaveExit.Location = new System.Drawing.Point(454, 12);
            this.button_SaveExit.Name = "button_SaveExit";
            this.button_SaveExit.Size = new System.Drawing.Size(118, 44);
            this.button_SaveExit.TabIndex = 1;
            this.button_SaveExit.Text = "Save and Close";
            this.button_SaveExit.UseVisualStyleBackColor = true;
            this.button_SaveExit.Click += new System.EventHandler(this.button_SaveExit_Click);
            // 
            // FormNotes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 450);
            this.Controls.Add(this.button_SaveExit);
            this.Controls.Add(this.textBox_Notes);
            this.Name = "FormNotes";
            this.Text = "Form2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNotes_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Notes;
        private System.Windows.Forms.Button button_SaveExit;
    }
}