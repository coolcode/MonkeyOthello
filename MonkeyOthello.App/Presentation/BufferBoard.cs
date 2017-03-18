using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonkeyOthello.Presentation
{
    public partial class BufferBoard : UserControl
    {
        //public BoardPainter Painter { get; set; }

        public BufferBoard()
        {
            InitializeComponent();  
        }
         /*
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Painter != null)
            {
                Painter.Paint();
                BackgroundImage = Painter.Buffer;
            }
        }*/
    }
}
