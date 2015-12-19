using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoPrinter
{
    public delegate void PictureViewBoxSelectionChangedEventHandler(PictureViewBox sender);

    public class PictureViewBox : Panel
    {
        #region Data

        private string m_filename;
        private Image m_thumbnail;
        private bool m_isSelected;

        #endregion Data

        #region Properties

        public Image Image
        {
            get { return m_filename != null ? Image.FromFile(m_filename) : null; }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
        }

        #endregion Properties

        #region Events

        public event PictureViewBoxSelectionChangedEventHandler SelectionChangeRequested;

        private void OnSelectionChangeRequested()
        {
            if (SelectionChangeRequested != null)
            {
                SelectionChangeRequested(this);
            }
        }

        #endregion Events

        #region Constructors

        private PictureViewBox()
        {
            m_isSelected = false;

            this.Size = new Size(170, 170);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            this.MouseClick += new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    OnSelectionChangeRequested();
                }
            });

            AttachContextMenu();
        }

        #endregion Constructors

        #region Public Methods

        public void Select(bool isSelected)
        {
            if (isSelected)
            {
                this.BackColor = Color.Blue;
                m_isSelected = true;
            }
            else
            {
                this.BackColor = Color.White;
                m_isSelected = false;
            }
        }

        public bool RotateFlip(RotateFlipType rotateFlipOption)
        {
            bool result = false;

            if (File.Exists(m_filename))
            {
                Image image = this.Image;
                image.RotateFlip(rotateFlipOption);
                image.Save(m_filename);
                image.Dispose();

                m_thumbnail.RotateFlip(rotateFlipOption);
                this.Refresh();
                result = true;
            }

            return result;
        }

        public static PictureViewBox Create(string filePath)
        {
            PictureViewBox pvb = new PictureViewBox();
            pvb.LoadPictureAsync(filePath);
            return pvb;
        }

        #endregion Public Methods

        #region Internals

        private void AttachContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem miRotate = new MenuItem();
            miRotate.Text = "&Rotate";
            miRotate.Click += new EventHandler((object s, EventArgs ea) =>
            {
                this.RotateFlip(RotateFlipType.Rotate90FlipNone);
            });
            contextMenu.MenuItems.Add(miRotate);

            this.ContextMenu = contextMenu;
        }

        private Task LoadPictureAsync(string filename)
        {
            return Task.Factory.StartNew(() => {
                m_filename = filename;

                Image image = Image.FromFile(m_filename);
                m_thumbnail = image.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                image.Dispose();

                this.Invoke(new Action(() => {
                    this.InvokePaint(this, new PaintEventArgs(this.CreateGraphics(), this.DisplayRectangle));
                }));
            });
        }

        public Font DefaultTextFont = new Font(FontFamily.GenericSerif, 10, FontStyle.Regular);

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_thumbnail != null)
            {
                e.Graphics.DrawImage(m_thumbnail, 10, 10);
            }

            string displayText = null;
            SizeF displayTextSize;

            do
            {
                if (displayText == null)
                {
                    displayText = Path.GetFileName(m_filename);
                }
                else
                {
                    displayText = String.Concat(displayText.Substring(0, displayText.Length - 4), "...");
                }

                displayTextSize = e.Graphics.MeasureString(displayText, DefaultTextFont);
            }
            while (displayTextSize.Width > this.Width);

            e.Graphics.DrawString(displayText, DefaultTextFont, Brushes.Black, 
                new PointF((this.Width - displayTextSize.Width)/2, this.Height - displayTextSize.Height));

            base.OnPaint(e);
        }

        #endregion Internals
    }
}
