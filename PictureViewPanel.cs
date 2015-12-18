using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoPrinter
{
    public class PictureViewPanel : FlowLayoutPanel
    {
        public PictureViewPanel()
        {
            this.MouseClick += new MouseEventHandler((object sender, MouseEventArgs e) => {
                Control control = this.GetChildAtPoint(e.Location);

                if (control != null)
                {
                    PictureViewBox pvb = (PictureViewBox)control;
                    pvb.Select();
                }
            });
        }

        public bool Add(string filename)
        {
            PictureViewBox pvb = PictureViewBox.Create(filename);

            if (pvb != null)
            {
                this.Controls.Add(pvb);
            }

            return (pvb != null);
        }
    }
}
