using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoPrinter
{
    public enum PictureSelectionMode
    {
        None,
        All,
        Inverse
    }

    public class PictureViewPanel : FlowLayoutPanel
    {
        PictureViewBox m_lastSelected = null;

        public PictureViewPanel()
        {
            this.MouseClick += new MouseEventHandler((object sender, MouseEventArgs e) => {

            });
        }

        public bool Add(string filename)
        {
            PictureViewBox pvb = PictureViewBox.Create(filename);

            if (pvb != null)
            {
                pvb.SelectionChangeRequested += new PictureViewBoxSelectionChangedEventHandler((PictureViewBox sender) => {
                    HandlePictureViewBoxSelectionChangeEvent(sender);
                });

                this.Controls.Add(pvb);
            }

            return (pvb != null);
        }

        public void Select(PictureSelectionMode selectionMode)
        {
            foreach (Control control in this.Controls)
            {
                PictureViewBox pvb = (PictureViewBox)control;

                switch (selectionMode)
                {
                    case PictureSelectionMode.All:
                        pvb.Select(true);
                        break;
                    case PictureSelectionMode.Inverse:
                        pvb.Select(!pvb.IsSelected);
                        break;
                    case PictureSelectionMode.None:
                        pvb.Select(false);
                        break;
                }
            }
        }

        private void HandlePictureViewBoxSelectionChangeEvent(PictureViewBox pvb)
        {
            if (Control.ModifierKeys == Keys.None || m_lastSelected == null)
            {
                this.Select(PictureSelectionMode.None);
                pvb.Select(true);
            }
            else
            {
                if (Control.ModifierKeys.HasFlag(Keys.Shift))
                {
                    int indexOfLastSelected = this.Controls.IndexOf(m_lastSelected);
                    int indexOfNextSelected = this.Controls.IndexOf(pvb);

                    if (indexOfLastSelected < indexOfNextSelected)
                    {
                        for (int current_pvb_index = indexOfLastSelected + 1; current_pvb_index <= indexOfNextSelected; current_pvb_index++)
                        {
                            PictureViewBox current_pvb = (PictureViewBox)this.Controls[current_pvb_index];
                            current_pvb.Select(true);
                        }
                    }
                    else
                    {
                        for (int current_pvb_index = indexOfLastSelected - 1; current_pvb_index >= indexOfNextSelected; current_pvb_index--)
                        {
                            PictureViewBox current_pvb = (PictureViewBox)this.Controls[current_pvb_index];
                            current_pvb.Select(true);
                        }
                    }
                }
                else if (Control.ModifierKeys.HasFlag(Keys.Control))
                {
                    pvb.Select(true);
                }
            }

            m_lastSelected = pvb;
        }
    }
}
