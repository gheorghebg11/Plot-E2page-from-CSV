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
            this.label_elemName = new System.Windows.Forms.Label();
            this.label_stem = new System.Windows.Forms.Label();
            this.label_filt = new System.Windows.Forms.Label();
            this.label_weight = new System.Windows.Forms.Label();
            this.label_ext = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox_Notes
            // 
            this.textBox_Notes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Notes.Location = new System.Drawing.Point(5, 170);
            this.textBox_Notes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_Notes.Multiline = true;
            this.textBox_Notes.Name = "textBox_Notes";
            this.textBox_Notes.Size = new System.Drawing.Size(479, 230);
            this.textBox_Notes.TabIndex = 0;
            // 
            // button_SaveExit
            // 
            this.button_SaveExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveExit.Location = new System.Drawing.Point(396, 2);
            this.button_SaveExit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_SaveExit.Name = "button_SaveExit";
            this.button_SaveExit.Size = new System.Drawing.Size(88, 36);
            this.button_SaveExit.TabIndex = 1;
            this.button_SaveExit.Text = "Save and Close";
            this.button_SaveExit.UseVisualStyleBackColor = true;
            this.button_SaveExit.Click += new System.EventHandler(this.button_SaveExit_Click);
            // 
            // label_elemName
            // 
            this.label_elemName.AutoSize = true;
            this.label_elemName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_elemName.Location = new System.Drawing.Point(12, 12);
            this.label_elemName.Name = "label_elemName";
            this.label_elemName.Size = new System.Drawing.Size(69, 15);
            this.label_elemName.TabIndex = 2;
            this.label_elemName.Text = "elemName";
            // 
            // label_stem
            // 
            this.label_stem.AutoSize = true;
            this.label_stem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_stem.Location = new System.Drawing.Point(91, 12);
            this.label_stem.Name = "label_stem";
            this.label_stem.Size = new System.Drawing.Size(44, 15);
            this.label_stem.TabIndex = 3;
            this.label_stem.Text = "stem =";
            // 
            // label_filt
            // 
            this.label_filt.AutoSize = true;
            this.label_filt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_filt.Location = new System.Drawing.Point(91, 34);
            this.label_filt.Name = "label_filt";
            this.label_filt.Size = new System.Drawing.Size(63, 15);
            this.label_filt.TabIndex = 4;
            this.label_filt.Text = "filtration = ";
            // 
            // label_weight
            // 
            this.label_weight.AutoSize = true;
            this.label_weight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_weight.Location = new System.Drawing.Point(91, 56);
            this.label_weight.Name = "label_weight";
            this.label_weight.Size = new System.Drawing.Size(56, 15);
            this.label_weight.TabIndex = 5;
            this.label_weight.Text = "weight = ";
            // 
            // label_ext
            // 
            this.label_ext.AutoSize = true;
            this.label_ext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ext.Location = new System.Drawing.Point(177, 12);
            this.label_ext.Name = "label_ext";
            this.label_ext.Size = new System.Drawing.Size(84, 15);
            this.label_ext.TabIndex = 6;
            this.label_ext.Text = "EXTENSIONS";
            // 
            // FormNotes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 404);
            this.Controls.Add(this.label_ext);
            this.Controls.Add(this.label_weight);
            this.Controls.Add(this.label_filt);
            this.Controls.Add(this.label_stem);
            this.Controls.Add(this.label_elemName);
            this.Controls.Add(this.button_SaveExit);
            this.Controls.Add(this.textBox_Notes);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormNotes";
            this.Text = "Form2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNotes_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Notes;
        private System.Windows.Forms.Button button_SaveExit;
        private System.Windows.Forms.Label label_elemName;
        private System.Windows.Forms.Label label_stem;
        private System.Windows.Forms.Label label_filt;
        private System.Windows.Forms.Label label_weight;
        private System.Windows.Forms.Label label_ext;
    }
}