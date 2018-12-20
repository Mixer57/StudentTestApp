using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StudiTest
{
    public partial class ProgressBarEx : UserControl
    {
        public ProgressBarEx()
        {
            InitializeComponent();
        }


        public bool Inverted { get; set; }

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                Refresh();
                UpdateStyles();
            }
        }

        [DefaultValue(100)] public int Maximum { get; set; } = 100;

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            if (Maximum == 0) Maximum = 100;
            var w = (Value * Width / Maximum);

            e.Graphics.Clear(ForeColor);
            e.Graphics.FillRectangle(new SolidBrush(BackColor), 1, 1, Width - 2, Height - 2);

            if (Inverted)
                e.Graphics.FillRectangle(new SolidBrush(ForeColor), Width - w, 0, w, Height);
            else
                e.Graphics.FillRectangle(new SolidBrush(ForeColor), 0, 0, w, Height);
        }
    }
}
