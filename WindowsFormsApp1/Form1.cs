
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

using System.Drawing;



namespace WindowsFormsApp1
{
    enum GraphSizeMode
    {
        BestFitToScreen,
        A4Format = 1,
        A3Format = 2,
        A2Format = 3,
        A1Format = 4,
        A0Format = 5,
        Custom = 6
    }



    public partial class Form1 : Form
    {
        // -------------------------------------------------------- Fields and Properties --------------------------------------------------------

        #region Global Parameters that the user can change

        // Words for Parsing the .csv
        static private string[][] WordsForNonExtensionAttributes = new string[7][] {
        new string[1] { "name" },
        new string[2] { "s", "stem" },
        new string[3] { "f", "filt", "filtration" },
        new string[2] { "w", "weight" },
        new string[3] { "ttorsion", "tautorsion", "taut" },
        new string[1] { "label" },
        new string[1] { "angle" } };

    static private string[][] WordsForExtensionAttributes = new string[3][] {
        new string[3] { "target", "ext", "extension" },
        new string[1] { "info"},
        new string[1] { "loc" } };


        // Title, Frame and BackColor
        static private string Title = "E2 page in progress";
        static private bool TitleIsShown = true;
        static private int TitleFontSize = 20;

        static private Color FrameColor = Color.Black;
        static private int FrameWidth = 10;
        static private bool FrameIsVisible = false;

        static private Color BackGraphColor = Color.White;

        // Axis, Grid and Margin
        static private float GridLineWidthPen = 1;
        static private int GridInterval = 2;
        static private Color GridColor = Color.LightGray;

        static private float AxisWidth = GridLineWidthPen;
        static private Color AxisColor = GridColor;

        static private int AxisLabelFontsize = 10;
        static private int AxisXspaceToLabel = 40;
        static private int AxisYspaceToLabel = 45;

        static private int MarginToGraphLeft = 50;
        static private int MarginToGraphRight = 50;
        static private int MarginToGraphTop = 50;
        static private int MarginToGraphBottom = 50;

        // Points, Labels and Extensions
        static private float PointsSize = 5;
        static private Color[] ColorTauTorsion = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Brown };

        static private int DeltaBetweenAdjacentPoints = 10; // from center to center, remove PointsSize to get the distance between points

        static private int LabelSize = 12;
        static private int LabelDistance = 15; // from the point to the center of the labelbox

        static private float ExtensionWidthPen = (float)1.0;
        static private Color ExtensionColorDefault = Color.DimGray; // the color of extensions in the case of non-tauTorsion scenarios

        static private float PrecisionClickFindPoint = 2; // the bigger this is, the less precise you have to be when clicking

        // Arrow at the end of an infinite tower of h1 multiples
        static private int ArrowOpensAngle = 25;
        static private int ArrowLength = 6;
        static private int ArrowInsideDiagonal = 4;
        static private float ArrowInLocalizationFactor = (float)0.7;

        // Format
        static private int CustomXresolution = 3508; // change freely for custom format. As a reference : 3508 x 2480 is A4 format with 300 PPI
        static private int CustomYresolution = 2480;

        private const int DefaultXresA4 = 3508; // nbr of pixels of A4 format with 300 PPI
        private const int DefaultYresA4 = 2480;
        private const int ResolutionFactorForBestFitScreen = 25; // increase for better resolution in BestFitScreen mode


        // TO NOT CHANGE
        private const int UpperBoundStem = 150; // change when we got there, if we got there...
        private int UpperBoundFiltration = UpperBoundStem / 2;

        #endregion

        #region Private (internal) Fields
        static private E2data E2data;
        static private E2graph E2graph;
        static private Bitmap BitmapForE2Chart; // the Bitmap contains the graph
        static private PictureBox PictureBox; // the picture box contains the Bitmap
        static private GraphSizeMode E2graphSizeMode;
        static private List<ToolStripMenuItem> ExtensionInMenu;

        static private bool LockedElement;
        static private int[] LockedElementStemFiltNbrinelem;
        static private float ResolutionBitmapToPictureBoxY;
        static private float ResolutionBitmapToPictureBoxX;
        static private bool GoodQuality = true;
        static private bool HideAllLabels = false;
        static private bool ShowWarnings = true;
        #endregion

        // -------------------------------------------------------- Constructors --------------------------------------------------------

        public Form1()
        {
            InitializeComponent();

            comboBox1.Items.Add("Best Fit Screen");
            comboBox1.Items.Add("A4 Format");
            comboBox1.Items.Add("A3 Format");
            comboBox1.Items.Add("A2 Format");
            comboBox1.Items.Add("A1 Format");
            comboBox1.Items.Add("A0 Format");
            comboBox1.Items.Add("Custom");

            comboBox1.SelectedItem = comboBox1.Items[0];
            showFrameToolStripMenuItem.Checked = FrameIsVisible;

            label3.Text = "";
            label5.Text = "";

            Point_radiobutton.Checked = true;
            Label_radiobutton.Checked = false;
            LabelTextBox.Enabled = false;

            trackbar_zoom.Value = trackbar_zoom.Minimum;
            trackbar_resolution.Value = trackbar_resolution.Minimum;

            getInfoOnClickToolStripMenuItem1.Checked = false;
            showWarningToolStripMenuItem.Checked = true;

            PictureBox = new PictureBox();
            PictureBox.BackColor = Color.White;
            PictureBox.Visible = true;
            PictureBox.MouseClick += PictureBox_MouseClick;
            PictureBox.Width = 0;
            PictureBox.Height = 0;

            panel1.Controls.Add(PictureBox);
            PictureBox.Parent = panel1; // It does nothing, is it to link them or something ?
            Controls.Add(panel1);
            

            //this.DoubleBuffered = true;

            // loads the latex fonts for FMathML
            string filePath = Application.StartupPath.ToString();
            filePath = Path.GetFullPath(Path.Combine(filePath, @"..\..\Resources\FMathML"));
            fmath.controls.MathMLFormulaControl.setFolderUrlForFonts(filePath);
            fmath.controls.MathMLFormulaControl.setFolderUrlForGlyphs(filePath);

        }

        // -------------------------------------------------------- Methods --------------------------------------------------------

        #region On Window Buttons/Scrolls/ etc

        /// <summary>
        /// Refresh button which redraws the whole diagram (but does not reload the document)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Refresh_button(object sender, EventArgs e)
        {
            RefreshGraph();
        }

        /// <summary>
        /// Box where you choose the format (and hence the number of pixels) of the document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9); // to make it bigger after opening it for readability

            GraphSizeMode previousMode = E2graphSizeMode;
            switch (comboBox1.Text)
            {
                case "Best Fit Screen":
                    E2graphSizeMode = GraphSizeMode.BestFitToScreen;
                    break;
                case "A4 Format":
                    E2graphSizeMode = GraphSizeMode.A4Format;
                    break;
                case "A3 Format":
                    E2graphSizeMode = GraphSizeMode.A3Format;
                    break;
                case "A2 Format":
                    E2graphSizeMode = GraphSizeMode.A2Format;
                    break;
                case "A1 Format":
                    E2graphSizeMode = GraphSizeMode.A1Format;
                    break;
                case "A0 Format":
                    E2graphSizeMode = GraphSizeMode.A0Format;
                    break;
                case "Custom":
                    E2graphSizeMode = GraphSizeMode.Custom;
                    break;
                default:
                    E2graphSizeMode = GraphSizeMode.BestFitToScreen;
                    break;
            }

            if (E2data != null && previousMode != E2graphSizeMode)
            {
                trackbar_resolution.Value = 1;
                CreateGraph();
            }

            if (E2graphSizeMode == GraphSizeMode.BestFitToScreen)
                trackbar_resolution.Enabled = true;
            else
                trackbar_resolution.Enabled = false;
        }

        /// <summary>
        /// Scrolling button to zoom in/out. The minimum zoom is the default size, the maximum is the biggest zoom which does not loose quality.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zoom_scroll(object sender, EventArgs e)
        {
            if(E2graph != null)
                RescaleImageAndPrintOnScreen();
        }

        /// <summary>
        /// Button to improve the resolution, without changing the format mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resolution_scroll(object sender, EventArgs e) 
        {
            if (E2data != null)
                CreateGraph();
        }

        private void QuickOpen_button(object sender, EventArgs e)
        {
            if (E2data == null)
            {
                string filePath = Application.StartupPath.ToString();
                filePath = Path.GetFullPath(Path.Combine(filePath, @"..\..\...\\E2chartdata132shorter.csv"));
                OpenAndParseFile(filePath);

                PrepareMenus(sender, e);

                CreateGraph();
                
            }
            else
            {
                MessageBox.Show("You already have a file opened, duh, restart the program to open a new file.");
            }

            mathMLViewer = new fmath.controls.MathMLFormulaControl();
            mathMLViewer.latex = true;
            //mathMLViewer.AutoScroll = true;
            //mathMLViewer.BackColor = System.Drawing.Color.Transparent;
            //mathMLViewer.Font = new System.Drawing.Font("Palatino", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //mathMLViewer.ForeColor = System.Drawing.SystemColors.WindowText;
            mathMLViewer.Location = new System.Drawing.Point(6, 7);
            //mathMLViewer.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            //mathMLViewer.Name = "mathMLViewer";
            //mathMLViewer.Size = new System.Drawing.Size(1751, 1399);
            //mathMLViewer.TabIndex = 0;
            mathMLViewer.Contents = @"$\Delta h_1^3 \ \ h_3 \ \ c_0 \ \ Q'$";
            //panel1.Controls.Add(mathMLViewer);

        }

        private void GetInfo_button_Click(object sender, EventArgs e) // To implement
        {
            MessageBox.Show("Not functional yet!");
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (LockedElement)
            {
                if (Point_radiobutton.Checked)
                {
                    {
                        E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetY -= Convert.ToInt32(TextBoxShiftElem.Text); // do something if not a number....

                        //RefreshGraphLocally(LockedElementStemFiltNbrinelem[0], LockedElementStemFiltNbrinelem[1]);

                    }
                }
                else if (Label_radiobutton.Checked)
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetYlabel -= Convert.ToInt32(TextBoxShiftElem.Text);
                
                RefreshGraph();
            }

        }
        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (LockedElement)
            {
                {
                    if (Point_radiobutton.Checked)
                        E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetY += Convert.ToInt32(TextBoxShiftElem.Text); // do something if not a number....
                    else if (Label_radiobutton.Checked)
                        E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetYlabel += Convert.ToInt32(TextBoxShiftElem.Text);

                    RefreshGraph();
                }
            }
        }
        private void buttonLeft_Click(object sender, EventArgs e)
        {
            if (LockedElement)
            {
                if (Point_radiobutton.Checked)
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetX -= Convert.ToInt32(TextBoxShiftElem.Text); // do something if not a number....
                else if (Label_radiobutton.Checked)
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetXlabel -= Convert.ToInt32(TextBoxShiftElem.Text);

                RefreshGraph();
            }
        }
        private void buttonRight_Click(object sender, EventArgs e)
        {
            if (LockedElement)
            {
                if (Point_radiobutton.Checked)
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetX += Convert.ToInt32(TextBoxShiftElem.Text); // do something if not a number....
                else if (Label_radiobutton.Checked)
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetXlabel += Convert.ToInt32(TextBoxShiftElem.Text);

                RefreshGraph();
            }
        }

        private void FarRight_button_Click(object sender, EventArgs e)
        {
            if (LockedElement)
            {
                if (Point_radiobutton.Checked)
                {
                    int nbrInElemToSwapWith = LockedElementStemFiltNbrinelem[2];

                    float point1Xcoord = E2data.GetElement(LockedElementStemFiltNbrinelem).GetGeomXcoord();
                    float point1Ycoord = E2data.GetElement(LockedElementStemFiltNbrinelem).GetGeomYcoord();

                    float smallestXDist = point1Xcoord;

                    for (int i = 0; i < E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]].Count; i++)
                    {
                        if (i != LockedElementStemFiltNbrinelem[2])
                        {
                            if (E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][i].GetGeomXcoord() - point1Xcoord > 0)
                            {
                                if (smallestXDist > E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][i].GetGeomXcoord() - point1Xcoord)
                                {
                                    smallestXDist = E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][i].GetGeomXcoord() - point1Xcoord;
                                    nbrInElemToSwapWith = i;
                                }
                            }
                        }
                    }

                    int point1offsetX = E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetX;
                    int point1offsetY = E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetY;

                    E2data.GetElement(LockedElementStemFiltNbrinelem).SetGeomXcoord(E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].GetGeomXcoord());
                    E2data.GetElement(LockedElementStemFiltNbrinelem).SetGeomYcoord(E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].GetGeomYcoord());
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetX = E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].OffsetX;
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetY = E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].OffsetY;

                    E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].SetGeomXcoord(point1Xcoord);
                    E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].SetGeomYcoord(point1Ycoord);
                    E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].OffsetX = point1offsetX;
                    E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].OffsetY = point1offsetY;

                }
                else if (Label_radiobutton.Checked)
                {
                    E2data.GetElement(LockedElementStemFiltNbrinelem).LabelAngle += 5 * Convert.ToInt32(TextBoxShiftElem.Text);
                }

                RefreshGraph();
            }
        }
        private void FarLeft_button_Click(object sender, EventArgs e)
        {
            if (LockedElement)
            {
                if (Point_radiobutton.Checked)
                {
                    int nbrInElemToSwapWith = LockedElementStemFiltNbrinelem[2];

                    float point1Xcoord = E2data.GetElement(LockedElementStemFiltNbrinelem).GetGeomXcoord();
                    float point1Ycoord = E2data.GetElement(LockedElementStemFiltNbrinelem).GetGeomYcoord();

                    float smallestXDist = point1Xcoord;

                    for (int i = 0; i < E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]].Count; i++)
                    {
                        if (i != LockedElementStemFiltNbrinelem[2])
                        {
                            if (point1Xcoord - E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][i].GetGeomXcoord() > 0)
                            {
                                if (smallestXDist > point1Xcoord - E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][i].GetGeomXcoord())
                                {
                                    smallestXDist = point1Xcoord - E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][i].GetGeomXcoord();
                                    nbrInElemToSwapWith = i;
                                }
                            }
                        }
                    }

                    int point1offsetX = E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetX;
                    int point1offsetY = E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetY;

                    E2data.GetElement(LockedElementStemFiltNbrinelem).SetGeomXcoord(E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].GetGeomXcoord());
                    E2data.GetElement(LockedElementStemFiltNbrinelem).SetGeomYcoord(E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].GetGeomYcoord());
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetX = E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].OffsetX;
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetY = E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].OffsetY;

                    E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].SetGeomXcoord(point1Xcoord);
                    E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].SetGeomYcoord(point1Ycoord);
                    E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].OffsetX = point1offsetX;
                    E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]][nbrInElemToSwapWith].OffsetY = point1offsetY;

                }
                else if (Label_radiobutton.Checked)
                {
                    E2data.GetElement(LockedElementStemFiltNbrinelem).LabelAngle -= 5 * Convert.ToInt32(TextBoxShiftElem.Text);
                }

                RefreshGraph();
            }
        }

        private void Resetbutton_click(object sender, EventArgs e)
        {
            if (LockedElement)
            {
                if (Point_radiobutton.Checked)
                {
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetY = 0;
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetX = 0;
                }
                else if (Label_radiobutton.Checked)
                {
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetXlabel = 0;
                    E2data.GetElement(LockedElementStemFiltNbrinelem).OffsetYlabel = 0;
                    E2data.GetElement(LockedElementStemFiltNbrinelem).LabelAngle = -90;
                }

                RefreshGraph();
            }
        }

        private void ShowHide_button_Click(object sender, EventArgs e)
        {
            if (LockedElement)
            {
                if (Point_radiobutton.Checked)
                {
                    E2data.GetElement(LockedElementStemFiltNbrinelem).IsVisible = !E2data.GetElement(LockedElementStemFiltNbrinelem).IsVisible;
                }
                else if (Label_radiobutton.Checked)
                {
                    E2data.GetElement(LockedElementStemFiltNbrinelem).IsLabelVisible = !E2data.GetElement(LockedElementStemFiltNbrinelem).IsLabelVisible;
                    if (E2data.GetElement(LockedElementStemFiltNbrinelem).LatexLabel == "")
                    {
                        E2data.GetElement(LockedElementStemFiltNbrinelem).LatexLabel = E2data.GetElement(LockedElementStemFiltNbrinelem).AssembleLatexName();
                        LabelTextBox.Text = E2data.GetElement(LockedElementStemFiltNbrinelem).LatexLabel;
                    }
                }

                RefreshGraph();
            }
        }

        private void LabelTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (LockedElement)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    E2data.GetElement(LockedElementStemFiltNbrinelem).LatexLabel = LabelTextBox.Text;

                    if (LabelTextBox.Text != "")
                        E2data.GetElement(LockedElementStemFiltNbrinelem).IsLabelVisible = true;
                    else
                        E2data.GetElement(LockedElementStemFiltNbrinelem).IsLabelVisible = false;

                    RefreshGraph();
                }
            }
        }

        private void Point_radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            Label_radiobutton.Checked = !Point_radiobutton.Checked;
            LabelTextBox.Enabled = false;

            FarLeft_button.Enabled = true;
            FarRight_button.Enabled = true;

            if (LockedElement == true)
            {
                if(E2data.Elements[LockedElementStemFiltNbrinelem[0]][LockedElementStemFiltNbrinelem[1]].Count == 1)
                {
                    if(Point_radiobutton.Enabled == true)
                    {
                        FarLeft_button.Enabled = false;
                        FarRight_button.Enabled = false;
                    }
                }
            }
        }
        private void Label_radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            LabelTextBox.Enabled = true;
            Point_radiobutton.Checked = !Label_radiobutton.Checked;

            FarLeft_button.Enabled = true;
            FarRight_button.Enabled = true;
        }

        #endregion

        #region In Menu Buttons

        private void OpenFile_MenuButton(object sender, EventArgs e)
        {
            if (E2data == null)
            {
                using (OpenFileDialog l_ofd = new OpenFileDialog())
                {
                    l_ofd.InitialDirectory = @"C:\";   // default location where to open the dialog
                    l_ofd.Filter = "Excel Spreadsheet|*.csv";

                    if (l_ofd.ShowDialog() == DialogResult.OK)
                    {
                        OpenAndParseFile(l_ofd.FileName);

                        PrepareMenus(sender, e);
                        CreateGraph();
                    }
                }
            }
            else
            {
                    MessageBox.Show("You already have a file opened, duh, restart the program to open a new file.");
            }
        }

        private void SaveAsPDF_MenuButton(object sender, EventArgs e)
        {
            #region Open, Create the pdf file




            #endregion
        }

        private void BetterQuality_MenuButton(object sender, EventArgs e)
        {
            GoodQuality = !GoodQuality;

            if (E2data != null)
                CreateGraph();
        }

        private void HideAllLabels_MenuButton(object sender, EventArgs e)
        {
            HideAllLabels = !HideAllLabels;
            RefreshGraph();
        }

        private void ShowFrame_MenuButton(object sender, EventArgs e)
        {
            FrameIsVisible = !FrameIsVisible;
            RefreshGraph();
        }

        private void showWarningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWarnings = !ShowWarnings;
        }

        private void getInfoOnClickToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            getInfoOnClickToolStripMenuItem1.Checked = !getInfoOnClickToolStripMenuItem1.Checked;
        }

        private void OpenExtensionMenu_Click(object sender, EventArgs e)
        {
            int nbrInExt = 0;
            for (int i = 0; i < E2data.Extensions.Count; i++)
            {
                if (((ToolStripMenuItem)sender).Text == E2data.Extensions[i].NameElem)
                    nbrInExt = i;
            }

            ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
            E2data.Extensions[nbrInExt].IsVisible = !E2data.Extensions[nbrInExt].IsVisible;

            E2data.MaxStemExtensions = 0;
            E2data.MaxFiltExtensions = 0;

            for(int i=0; i < E2data.Extensions.Count; i++)
            {
                if(E2data.Extensions[i].IsVisible)
                {
                    E2data.MaxStemExtensions = Math.Max(E2data.MaxStemExtensions, E2data.Extensions[i].StemElem);
                    E2data.MaxFiltExtensions = Math.Max(E2data.MaxFiltExtensions, E2data.Extensions[i].FiltElem);
                }
            }

            RefreshGraph();

            /* 
            Form2 formForExtensions = new Form2("Extension Properties for " + E2data.Extensions[nbrInExt].NameElem);
            formForExtensions.Show();

            Extension extension = E2data.Extensions[nbrInExt];

            UserControlForExtensions userControlForExt = new UserControlForExtensions(ref extension);
            formForExtensions.Controls.Add(userControlForExt);
            */
        }

        #endregion

        #region Methods to create and show the graph

        private void OpenAndParseFile(string filepath)
        {
            E2data = new E2data(filepath, UpperBoundStem, UpperBoundFiltration, WordsForNonExtensionAttributes, WordsForExtensionAttributes, ExtensionColorDefault);
        }

        private void CreateExtensionMenu(object sender, EventArgs e)
        {
            ExtensionInMenu = new List<ToolStripMenuItem>();
            extensionsToolStripMenuItem1.DropDownItems.Clear();

            for (int i = 0; i < E2data.Extensions.Count; i++)
            {
                ToolStripMenuItem tempMenuItem = new ToolStripMenuItem(E2data.Extensions[i].NameElem);
                tempMenuItem.Checked = true;

                tempMenuItem.Click += OpenExtensionMenu_Click;
                extensionsToolStripMenuItem1.DropDownItems.Add(tempMenuItem);
                ExtensionInMenu.Add(tempMenuItem);
            }
        }

        private void InitiateGraph()
        {
            int horizontalRes, verticalRes;
            switch (E2graphSizeMode)
            {
                case GraphSizeMode.A4Format:
                case GraphSizeMode.A3Format:
                case GraphSizeMode.A2Format:
                case GraphSizeMode.A1Format:
                case GraphSizeMode.A0Format:
                    int zoomFactor = (int)E2graphSizeMode - 1;
                    horizontalRes = (int)(DefaultXresA4 * Math.Pow(Math.Sqrt(2), zoomFactor));
                    verticalRes = (int)(DefaultYresA4 * Math.Pow(Math.Sqrt(2), zoomFactor));
                    break;
                case GraphSizeMode.Custom:
                    horizontalRes = CustomXresolution;
                    verticalRes = CustomYresolution;
                    break;
                case GraphSizeMode.BestFitToScreen:
                default:
                    horizontalRes = ResolutionFactorForBestFitScreen * E2data.MaxStem + MarginToGraphLeft + MarginToGraphRight;
                    verticalRes = ResolutionFactorForBestFitScreen * E2data.MaxFilt + MarginToGraphTop + MarginToGraphBottom;
                    break;
            }
            horizontalRes *= trackbar_resolution.Value;
            verticalRes *= trackbar_resolution.Value;

            if (BitmapForE2Chart != null)
                BitmapForE2Chart.Dispose();
            BitmapForE2Chart = new Bitmap(horizontalRes, verticalRes);

            Graphics l_gra = Graphics.FromImage(BitmapForE2Chart);            
            E2graph = new E2graph(l_gra, E2data, horizontalRes, verticalRes, E2data.MaxStem, E2data.MaxFilt, GridInterval, MarginToGraphLeft, MarginToGraphRight, MarginToGraphTop, MarginToGraphBottom, AxisXspaceToLabel, AxisYspaceToLabel, DeltaBetweenAdjacentPoints, PointsSize, GoodQuality, ColorTauTorsion);

            E2graph.InitiatePoints(DeltaBetweenAdjacentPoints);
            E2graph.InitiateExtensions(ArrowInLocalizationFactor);
        }

        private void DrawOnGraph()
        {
            if(FrameIsVisible == true)
                E2graph.DrawFrame(FrameColor, FrameWidth);

            E2graph.DrawAxis(AxisColor, AxisWidth);
            E2graph.DrawAxisLabel(AxisXspaceToLabel, AxisYspaceToLabel, AxisLabelFontsize);
            E2graph.DrawGrid(GridColor, GridLineWidthPen);
            
            for (int i = 0; i < E2data.Extensions.Count; i++)
            {
                if (E2data.Extensions[i].IsVisible)
                    E2graph.DrawExtensions(i, ExtensionWidthPen, ArrowOpensAngle, ArrowLength, ArrowInsideDiagonal);
            }

            E2graph.DrawAllPoints(PointsSize);

            if (!HideAllLabels)
                E2graph.DrawLabels(LabelSize, LabelDistance);
        }

        private void RescaleImageAndPrintOnScreen()
        {

            #region Take care of the zoom

            double ratio1 = (double)E2graph.WidthGraph / panel1.Width;
            double ratio2 = (double)E2graph.HeightGraph / panel1.Height;
            double ratio = Math.Max(ratio1, ratio2);

            int widthres = (int)(E2graph.WidthGraph / ratio);
            int heightres = (int)(E2graph.HeightGraph / ratio);

            int zoomFactorInterpolation = trackbar_zoom.Value;
            if (ratio < 1) // if the panel is bigger than the graph, which is somehow a degenerate case, then the zoom will just be normal and zooming up to x4
            {
                widthres *= zoomFactorInterpolation;
                heightres *= zoomFactorInterpolation;
            }
            else // what should be the case as the graph should be way bigger than the panel, then the min zoom is to fit the screen and the max to get a panel of the size of graph * bigImageFactor
            {
                float bigImageFactor = 1;
                if(E2graph.WidthGraph > 8000) // if it's less than 8000, normal zoom is fine, otherwise need to zoom a little more.
                    bigImageFactor = (float)E2graph.WidthGraph / 8000;

                widthres = (int)( 4* panel1.Width - bigImageFactor*E2graph.WidthGraph + trackbar_zoom.Value*(bigImageFactor * E2graph.WidthGraph - panel1.Width) ) / 3;
                heightres = (int)( 4* panel1.Height - bigImageFactor * E2graph.HeightGraph + trackbar_zoom.Value*(bigImageFactor * E2graph.HeightGraph - panel1.Height) ) / 3;
                
                // old one:
                //widthres += Math.Abs(E2graph.WidthGraph - widthres) * zoomFactorInterpolation / trackbar_zoom.Maximum;
                //heightres += Math.Abs(E2graph.HeightGraph - heightres) * zoomFactorInterpolation / trackbar_zoom.Maximum;
            }

            #endregion

            if (PictureBox.Image != null)
                PictureBox.Image.Dispose();

            Bitmap resizedImg = new Bitmap(widthres, heightres);
            using (Graphics g = Graphics.FromImage(resizedImg))
                g.DrawImage(BitmapForE2Chart, 0, 0, widthres, heightres);

            PictureBox.Height = heightres;
            PictureBox.Width = widthres;
            PictureBox.Image = resizedImg;

            ResolutionBitmapToPictureBoxY = (float)E2graph.HeightGraph / PictureBox.Height;
            ResolutionBitmapToPictureBoxX = (float)E2graph.WidthGraph / PictureBox.Width;

            label5.Text = "Pixels : " + E2graph.WidthGraph.ToString() + " x " + E2graph.HeightGraph.ToString();

            if (ResolutionBitmapToPictureBoxY - ResolutionBitmapToPictureBoxX > (float)0.001 && ShowWarnings)
                MessageBox.Show("The X-Resolution ratio and Y-Resolution ratio are not the same (although it shouldn't be a problem)");

            if (E2graph.HadToRescale && ShowWarnings)
                MessageBox.Show("We had to reduce the ratio (# Pixels of the Pen) / (# Pixels of the an Interval of unit 1), so that the max number of " + E2data.MaxNbrElemPerStemFilt + " elements fit.");

        }

        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            label3.Text = "";
            LockedElement = false;

            float IntervalPixelPerUnit = E2graph.GetIntervalBitmapPixelPerUnit();

            int mouseX = e.Location.X;
            int mouseY = e.Location.Y;

            label4.Text = mouseX + " x " + mouseY;

            int stem = (int)Math.Floor(((float)(mouseX * ResolutionBitmapToPictureBoxX - E2graph.Origin.X) + IntervalPixelPerUnit / 2) / IntervalPixelPerUnit);
            int filt = (int)Math.Floor(((float)(E2graph.Origin.Y - mouseY * ResolutionBitmapToPictureBoxX) + IntervalPixelPerUnit / 2) / IntervalPixelPerUnit);

            if (stem >= 0 && filt >= 0 && stem <= E2data.MaxStem && filt <= E2data.MaxFilt)
            {
                if (E2data.Elements[stem][filt] != null)
                {
                    if(E2data.Elements[stem][filt].Count == 1)
                    {
                        if (Point_radiobutton.Checked == true) // if there is only 1 point at that spot and the radiobutton Point is select, disenable the far left and far right buttons
                        {
                            FarLeft_button.Enabled = false;
                            FarRight_button.Enabled = false;
                        }
                    }
                    else if(E2data.Elements[stem][filt].Count > 1)
                    {
                        FarLeft_button.Enabled = true;
                        FarRight_button.Enabled = true;
                    }

                    for (int i = 0; i < E2data.Elements[stem][filt].Count; i++)
                    {
                        if (Math.Abs(mouseX * ResolutionBitmapToPictureBoxX - (E2data.Elements[stem][filt][i].GetGeomXcoord() + E2data.Elements[stem][filt][i].OffsetX)) < DeltaBetweenAdjacentPoints * E2graph.RatioInputToPixelOnBitmap / 2 &&
                            Math.Abs(mouseY * ResolutionBitmapToPictureBoxX - (E2data.Elements[stem][filt][i].GetGeomYcoord() + E2data.Elements[stem][filt][i].OffsetY)) < Math.Max(PointsSize * PrecisionClickFindPoint, DeltaBetweenAdjacentPoints * E2graph.RatioInputToPixelOnBitmap / 2))
                        {
                            LockedElement = true;
                            LockedElementStemFiltNbrinelem = new int[3] { stem, filt, i };
                            label3.Text = E2data.Elements[stem][filt][i].ExcelName;
                            label3.Text += "\n \nStem = " + stem.ToString();
                            label3.Text += "\nFiltration = " + filt.ToString();
                            label3.Text += "\nWeight = " + E2data.Elements[stem][filt][i].Weight.ToString();

                            LabelTextBox.Text = E2data.Elements[stem][filt][i].LatexLabel;

                            if (getInfoOnClickToolStripMenuItem1.Checked)
                                GetInfo_button_Click(sender, e);
                        }
                    }
                }
            }  
        }

        private void RefreshGraph()
        {
            if (E2data != null)
            {
                E2graph.Graph.Clear(Color.Transparent);
                DrawOnGraph();
                RescaleImageAndPrintOnScreen();
            }
        }

        private void RefreshGraphLocally(int stem, int filtration)
        {
            if (E2data != null)
            {
                // draw a white rectangle there and redraw locally
                E2graph.DrawRectangle(BackGraphColor, stem, filtration, E2data.MaxStemExtensions, E2data.MaxFiltExtensions);
                //E2graph.Graph.DrawImage(BitmapForE2Chart, 0, 0, BitmapForE2Chart.Width, BitmapForE2Chart.Height);
                //RescaleImageAndPrintOnScreen();


                //E2graph.Graph.Clear(Color.Transparent);
                DrawOnGraph();
                RescaleImageAndPrintOnScreen();

                //E2graph.Graph.Dispose();
                //BitmapForE2Chart.Dispose();
            }
        }

        private void CreateGraph()
        {
            InitiateGraph();
            DrawOnGraph();
            RescaleImageAndPrintOnScreen();
        }

        private void PrepareMenus(object sender, EventArgs e)
        {
            CreateExtensionMenu(sender, e);
        }

        #endregion

    }


}
