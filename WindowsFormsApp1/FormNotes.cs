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

        public FormNotes(Element elem, int indexInArray)
        {
            InitializeComponent();
            ElementForWindow = elem;
            IndexInArrayOfForms = indexInArray;

            this.Text = ElementForWindow.AssembleName();
            this.textBox_Notes.Text = ElementForWindow.Notes;
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

            FormMainWindow.CommentWindows[IndexInArrayOfForms].IndexInArrayOfForms = IndexInArrayOfForms;

            // PROBLEMS WITHTHIS!!!!
        }

        private void FormNotes_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ExitForm(sender, e);
        }
    }
}
