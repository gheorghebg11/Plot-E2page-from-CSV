using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormNotes : Form
    {
        private Element ElementForWindow;
        private int IndexInArrayOfForms;
        private string[] NamesOfExtendees;

        public FormNotes(Element elem, int indexInArray, List<object[]> extensionsHeader)
        {
            InitializeComponent();
            ElementForWindow = elem;
            IndexInArrayOfForms = indexInArray;

            this.Text = ElementForWindow.AssembleName();
            this.textBox_Notes.Text = ElementForWindow.Notes;

            this.label_elemName.Text = elem.AssembleName();
            this.label_stem.Text = "stem       " + elem.Stem.ToString();
            this.label_filt.Text = "filtration  " + elem.Filtration;
            this.label_weight.Text = "weight    " + elem.Weight;

            this.label_ext.Text = "";

            NamesOfExtendees = new string[extensionsHeader.Count];
            for (int i=0; i < elem.NameOfExtTargets.Length; i++)
            {
                if (i != 0)
                    this.label_ext.Text += "\n\n";

                NamesOfExtendees[i] = (string)extensionsHeader[i][0];
                if(elem.NameOfExtTargets[i] != null)
                {
                    this.label_ext.Text += "* " + NamesOfExtendees[i] + "   =    " + elem.AssembleExtensionName(i);
                    if(elem.PropertyExtTarget[i] != null)
                        this.label_ext.Text += "   with prop " + elem.AssembleExtensionProperties(i);
                }
                else
                    this.label_ext.Text += "* " + NamesOfExtendees[i] + "   =  0 / ? " ;
            }

        }


    
        private void button_SaveExit_Click(object sender, EventArgs e)
        {
            this.ExitForm(sender, e);
        }


        private void ExitForm(object sender, EventArgs e)
        {
            // save the notes
            ElementForWindow.Notes = this.textBox_Notes.Text;

            // put the last opened window from CommentWindowsOpenCoord at this spot, to avoid holes in array which causes trouble when checking if it's already been opened (as it will stop at this hole).
            int otherWindowIndex = FormMainWindow.MaxNbrOpenCommentWindows - 1;
            while (FormMainWindow.CommentWindowsOpenCoord[otherWindowIndex] == null)
                otherWindowIndex--;
            // can do it all uniformly, otherWindowIndex is either another index, or the same as IndexInArrayOfForms. In any case, copy from there, then erase the old one applies in both cases.
            FormMainWindow.CommentWindowsOpenCoord[IndexInArrayOfForms] = FormMainWindow.CommentWindowsOpenCoord[otherWindowIndex];
            FormMainWindow.CommentWindowsOpenCoord[otherWindowIndex] = null;
            FormMainWindow.CommentWindows[IndexInArrayOfForms].Dispose();
            FormMainWindow.CommentWindows[IndexInArrayOfForms] = FormMainWindow.CommentWindows[otherWindowIndex];
            FormMainWindow.CommentWindows[otherWindowIndex] = null;

            // if an element has really been changed, then update his entry in the array
            if(IndexInArrayOfForms != otherWindowIndex)
                FormMainWindow.CommentWindows[IndexInArrayOfForms].IndexInArrayOfForms = IndexInArrayOfForms;
            
        }

        private void FormNotes_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ExitForm(sender, e);
        }
    }
}
