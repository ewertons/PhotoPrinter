using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoPrinter
{
    public partial class Form1 : Form
    {
        private List<string> m_currentFilenames = new List<string>();
        private PictureViewPanel m_pictureViewPanel;

        public Form1()
        {
            InitializeComponent();

            m_pictureViewPanel = new PictureViewPanel();
            m_pictureViewPanel.Dock = DockStyle.Fill;
            m_pictureViewPanel.BackColor = Color.White;
            splitContainer1.Panel2.Controls.Add(m_pictureViewPanel);
        }

        private void loadPicturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK &&
                dialog.FileNames.Length > 0)
            {
                foreach (string filename in dialog.FileNames)
                {
                    m_currentFilenames.Add(filename);
                    filenameListBox.Nodes.Add(Path.GetFileName(filename));

                    if (m_pictureViewPanel.Add(filename))
                    {
                        // Success message.
                    }
                    else
                    {
                        // Error message.
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();

            printDoc.PrintPage += PrintDoc_PrintPage;

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDoc;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        List<PictureViewBox> m_currentPrintSet;
        int m_currentPrintSetItem = 0;

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (m_currentPrintSet == null)
            {
                m_currentPrintSet = new List<PictureViewBox>(m_pictureViewPanel.GetSelected());
                m_currentPrintSetItem = 0;
            }

            PictureViewBox pvb = m_currentPrintSet[m_currentPrintSetItem];
            Image image = pvb.Image;
            e.Graphics.DrawImage(image, 0, 0);
            image.Dispose();

            m_currentPrintSetItem++;

            if (m_currentPrintSetItem < m_currentPrintSet.Count)
            {
                e.HasMorePages = true;
            }
            else
            {
                m_currentPrintSetItem = 0;
                m_currentPrintSet = null;
                e.HasMorePages = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var pvb in m_pictureViewPanel.GetSelected())
            {
                pvb.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
        }

        private void leftToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (var pvb in m_pictureViewPanel.GetSelected())
            {
                pvb.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_pictureViewPanel.Select(PictureSelectionMode.All);
        }

        private void inverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_pictureViewPanel.Select(PictureSelectionMode.Inverse);
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_pictureViewPanel.Select(PictureSelectionMode.None);
        }
    }
}
