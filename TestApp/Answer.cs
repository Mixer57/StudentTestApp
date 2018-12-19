using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Answer : UserControl
    {
        public Answer()
        {
            InitializeComponent();
        }

        public delegate void CheckedChangedDlg(int idx, bool check);

        public event CheckedChangedDlg CheckedChanged;

        public Question.QValueEnum ValueType { get; set; }
        public Question.QAnswerEnum AnswerType { get; set; }

        private bool _check;
        private Image _chk0;
        private Image _chk1;

        public bool Checked
        {
            get => _check;
            set
            {
                _check = value;
                label1.Image = value ? _chk1 : _chk0;
                label1.ForeColor = value ? Color.DarkOrange : Color.GhostWhite;
                BackColor = value ? Color.FromArgb(54, 78, 114): Color.FromArgb(34, 47, 71);
            } 
        }

        public Image ValueImg { get; set; }
        public string ValueTxt { get; set; }
        public int Idx { get; set; }


        private void Answer_Load(object sender, EventArgs e)
        {
            if (ValueType == Question.QValueEnum.Image)
                label1.BackgroundImage = ValueImg;
            else
                label1.Text = new RichTextBox { Rtf = ValueTxt }.Text;

            if (AnswerType == Question.QAnswerEnum.Check)
            {
                _chk0 = Properties.Resources.check0;
                _chk1 = Properties.Resources.check1;
            }
            else
            {
                _chk0 = Properties.Resources.radio0;
                _chk1 = Properties.Resources.radio1;
            }

            label1.BackgroundImageLayout = ImageLayout.Zoom;
            label1.Image = _chk0;
            Visible = label1.Text.Length > 0 || label1.BackgroundImage != null;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
            CheckedChanged?.Invoke(Idx, Checked);
        }

        private void Answer_FontChanged(object sender, EventArgs e)
        {
            label1.Font = Font;
        }
    }
}
