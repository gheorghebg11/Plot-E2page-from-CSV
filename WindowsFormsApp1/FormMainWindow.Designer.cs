namespace WindowsFormsApp1
{
    partial class FormMainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainWindow));
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.trackbar_zoom = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.trackbar_resolution = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsPdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.showFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.betterQualityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideAllLabelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showWarningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getInfoOnClickToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.extensionsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonRight = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonLeft = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.GetInfo_button = new System.Windows.Forms.Button();
            this.TextBoxShiftElem = new System.Windows.Forms.TextBox();
            this.Reset_button = new System.Windows.Forms.Button();
            this.LabelTextBox = new System.Windows.Forms.TextBox();
            this.FarRight_button = new System.Windows.Forms.Button();
            this.FarLeft_button = new System.Windows.Forms.Button();
            this.Point_radiobutton = new System.Windows.Forms.RadioButton();
            this.Label_radiobutton = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Remove_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_zoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_resolution)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Refresh_button);
            // 
            // comboBox1
            // 
            this.comboBox1.AllowDrop = true;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.DropDown += new System.EventHandler(this.ChooseFormat_SelectedIndexChanged);
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ChooseFormat_SelectedIndexChanged);
            // 
            // trackbar_zoom
            // 
            resources.ApplyResources(this.trackbar_zoom, "trackbar_zoom");
            this.trackbar_zoom.Maximum = 4;
            this.trackbar_zoom.Minimum = 1;
            this.trackbar_zoom.Name = "trackbar_zoom";
            this.trackbar_zoom.Value = 1;
            this.trackbar_zoom.Scroll += new System.EventHandler(this.Zoom_scroll);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // trackbar_resolution
            // 
            resources.ApplyResources(this.trackbar_resolution, "trackbar_resolution");
            this.trackbar_resolution.Maximum = 4;
            this.trackbar_resolution.Minimum = 1;
            this.trackbar_resolution.Name = "trackbar_resolution";
            this.trackbar_resolution.Value = 1;
            this.trackbar_resolution.Scroll += new System.EventHandler(this.Resolution_scroll);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // button4
            // 
            resources.ApplyResources(this.button4, "button4");
            this.button4.Name = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.QuickOpen_button);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertiesToolStripMenuItem,
            this.saveAsPdfToolStripMenuItem});
            this.FileMenu.Name = "FileMenu";
            resources.ApplyResources(this.FileMenu, "FileMenu");
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.OpenFile_MenuButton);
            // 
            // saveAsPdfToolStripMenuItem
            // 
            this.saveAsPdfToolStripMenuItem.Name = "saveAsPdfToolStripMenuItem";
            resources.ApplyResources(this.saveAsPdfToolStripMenuItem, "saveAsPdfToolStripMenuItem");
            this.saveAsPdfToolStripMenuItem.Click += new System.EventHandler(this.SaveAsPDF_MenuButton);
            // 
            // OptionsMenu
            // 
            this.OptionsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showFrameToolStripMenuItem,
            this.betterQualityToolStripMenuItem,
            this.hideAllLabelsToolStripMenuItem});
            this.OptionsMenu.Name = "OptionsMenu";
            resources.ApplyResources(this.OptionsMenu, "OptionsMenu");
            // 
            // showFrameToolStripMenuItem
            // 
            this.showFrameToolStripMenuItem.CheckOnClick = true;
            this.showFrameToolStripMenuItem.Name = "showFrameToolStripMenuItem";
            resources.ApplyResources(this.showFrameToolStripMenuItem, "showFrameToolStripMenuItem");
            this.showFrameToolStripMenuItem.Click += new System.EventHandler(this.ShowFrame_MenuButton);
            // 
            // betterQualityToolStripMenuItem
            // 
            this.betterQualityToolStripMenuItem.Checked = true;
            this.betterQualityToolStripMenuItem.CheckOnClick = true;
            this.betterQualityToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.betterQualityToolStripMenuItem.Name = "betterQualityToolStripMenuItem";
            resources.ApplyResources(this.betterQualityToolStripMenuItem, "betterQualityToolStripMenuItem");
            this.betterQualityToolStripMenuItem.Click += new System.EventHandler(this.BetterQuality_MenuButton);
            // 
            // hideAllLabelsToolStripMenuItem
            // 
            this.hideAllLabelsToolStripMenuItem.CheckOnClick = true;
            this.hideAllLabelsToolStripMenuItem.Name = "hideAllLabelsToolStripMenuItem";
            resources.ApplyResources(this.hideAllLabelsToolStripMenuItem, "hideAllLabelsToolStripMenuItem");
            this.hideAllLabelsToolStripMenuItem.Click += new System.EventHandler(this.HideAllLabels_MenuButton);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.OptionsMenu,
            this.optionsToolStripMenuItem,
            this.extensionsToolStripMenuItem1});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showWarningToolStripMenuItem,
            this.getInfoOnClickToolStripMenuItem1});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            // 
            // showWarningToolStripMenuItem
            // 
            this.showWarningToolStripMenuItem.Name = "showWarningToolStripMenuItem";
            resources.ApplyResources(this.showWarningToolStripMenuItem, "showWarningToolStripMenuItem");
            this.showWarningToolStripMenuItem.Click += new System.EventHandler(this.showWarningToolStripMenuItem_Click);
            // 
            // getInfoOnClickToolStripMenuItem1
            // 
            this.getInfoOnClickToolStripMenuItem1.Name = "getInfoOnClickToolStripMenuItem1";
            resources.ApplyResources(this.getInfoOnClickToolStripMenuItem1, "getInfoOnClickToolStripMenuItem1");
            this.getInfoOnClickToolStripMenuItem1.Click += new System.EventHandler(this.getInfoOnClickToolStripMenuItem1_Click);
            // 
            // extensionsToolStripMenuItem1
            // 
            this.extensionsToolStripMenuItem1.Name = "extensionsToolStripMenuItem1";
            resources.ApplyResources(this.extensionsToolStripMenuItem1, "extensionsToolStripMenuItem1");
            // 
            // buttonUp
            // 
            resources.ApplyResources(this.buttonUp, "buttonUp");
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonRight
            // 
            resources.ApplyResources(this.buttonRight, "buttonRight");
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.UseVisualStyleBackColor = true;
            this.buttonRight.Click += new System.EventHandler(this.buttonRight_Click);
            // 
            // buttonDown
            // 
            resources.ApplyResources(this.buttonDown, "buttonDown");
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonLeft
            // 
            resources.ApplyResources(this.buttonLeft, "buttonLeft");
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.UseVisualStyleBackColor = true;
            this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // GetInfo_button
            // 
            resources.ApplyResources(this.GetInfo_button, "GetInfo_button");
            this.GetInfo_button.Name = "GetInfo_button";
            this.GetInfo_button.UseVisualStyleBackColor = true;
            this.GetInfo_button.Click += new System.EventHandler(this.GetInfo_button_Click);
            // 
            // TextBoxShiftElem
            // 
            resources.ApplyResources(this.TextBoxShiftElem, "TextBoxShiftElem");
            this.TextBoxShiftElem.Name = "TextBoxShiftElem";
            // 
            // Reset_button
            // 
            resources.ApplyResources(this.Reset_button, "Reset_button");
            this.Reset_button.Name = "Reset_button";
            this.Reset_button.UseVisualStyleBackColor = true;
            this.Reset_button.Click += new System.EventHandler(this.Resetbutton_click);
            // 
            // LabelTextBox
            // 
            resources.ApplyResources(this.LabelTextBox, "LabelTextBox");
            this.LabelTextBox.Name = "LabelTextBox";
            this.LabelTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LabelTextBox_KeyDown);
            // 
            // FarRight_button
            // 
            resources.ApplyResources(this.FarRight_button, "FarRight_button");
            this.FarRight_button.Name = "FarRight_button";
            this.FarRight_button.UseVisualStyleBackColor = true;
            this.FarRight_button.Click += new System.EventHandler(this.FarRight_button_Click);
            // 
            // FarLeft_button
            // 
            resources.ApplyResources(this.FarLeft_button, "FarLeft_button");
            this.FarLeft_button.Name = "FarLeft_button";
            this.FarLeft_button.UseVisualStyleBackColor = true;
            this.FarLeft_button.Click += new System.EventHandler(this.FarLeft_button_Click);
            // 
            // Point_radiobutton
            // 
            resources.ApplyResources(this.Point_radiobutton, "Point_radiobutton");
            this.Point_radiobutton.Checked = true;
            this.Point_radiobutton.Name = "Point_radiobutton";
            this.Point_radiobutton.TabStop = true;
            this.Point_radiobutton.UseVisualStyleBackColor = true;
            this.Point_radiobutton.CheckedChanged += new System.EventHandler(this.Point_radiobutton_CheckedChanged);
            // 
            // Label_radiobutton
            // 
            resources.ApplyResources(this.Label_radiobutton, "Label_radiobutton");
            this.Label_radiobutton.Name = "Label_radiobutton";
            this.Label_radiobutton.TabStop = true;
            this.Label_radiobutton.UseVisualStyleBackColor = true;
            this.Label_radiobutton.CheckedChanged += new System.EventHandler(this.Label_radiobutton_CheckedChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // Remove_button
            // 
            resources.ApplyResources(this.Remove_button, "Remove_button");
            this.Remove_button.Name = "Remove_button";
            this.Remove_button.UseVisualStyleBackColor = true;
            this.Remove_button.Click += new System.EventHandler(this.ShowHide_button_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.Remove_button);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackbar_zoom);
            this.Controls.Add(this.trackbar_resolution);
            this.Controls.Add(this.Label_radiobutton);
            this.Controls.Add(this.Point_radiobutton);
            this.Controls.Add(this.GetInfo_button);
            this.Controls.Add(this.LabelTextBox);
            this.Controls.Add(this.FarLeft_button);
            this.Controls.Add(this.FarRight_button);
            this.Controls.Add(this.Reset_button);
            this.Controls.Add(this.TextBoxShiftElem);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonLeft);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.buttonRight);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_zoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_resolution)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TrackBar trackbar_zoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackbar_resolution;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsPdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OptionsMenu;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem betterQualityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extensionsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem hideAllLabelsToolStripMenuItem;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonRight;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonLeft;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem showFrameToolStripMenuItem;
        private System.Windows.Forms.Button GetInfo_button;
        private System.Windows.Forms.TextBox TextBoxShiftElem;
        private System.Windows.Forms.Button Reset_button;
        private System.Windows.Forms.TextBox LabelTextBox;
        private System.Windows.Forms.Button FarRight_button;
        private System.Windows.Forms.Button FarLeft_button;
        private System.Windows.Forms.RadioButton Point_radiobutton;
        private System.Windows.Forms.RadioButton Label_radiobutton;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showWarningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getInfoOnClickToolStripMenuItem1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Remove_button;
    }
}

