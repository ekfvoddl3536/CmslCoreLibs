using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CmslDesign
{
    public class CmslFlatCheckbox : Control
    {
        private const int single = 3;
        private const int penCount = 2;
        private const int brushCount = 2;
        private const string Set = "설정1";

        private bool m_md;
        private bool m_chk;
        private int m_size;
        private SolidBrush[] m_brushes;
        private Pen[] m_pens;

        public CmslFlatCheckbox() : base()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.StandardDoubleClick, false);

            BackColor = Color.White;
            Click += MetroStyleCheckbox_Click;
            MouseDown += MetroStyleCheckbox_MouseDown;
            MouseUp += MetroStyleCheckbox_MouseUp;
            Resize += MetroStyleCheckbox_Resize;

            m_brushes = new SolidBrush[brushCount]
            {
                new SolidBrush(Color.DodgerBlue),
                new SolidBrush(Color.Gray)
            };

            m_size = 2;
            m_pens = new Pen[penCount]
            {
                new Pen(m_brushes[0].Color, m_size) { Alignment = PenAlignment.Inset },
                new Pen(Color.FromArgb(70, 70, 70), m_size) { Alignment = PenAlignment.Inset }
            };

            Size = new Size(25, 25);
        }

        private void MetroStyleCheckbox_Click(object sender, EventArgs e) => Checked ^= true;

        private void MetroStyleCheckbox_Resize(object sender, EventArgs e) => Region = new Region(new Rectangle(0, 0, Width + 1, Height + 1));

        private void MetroStyleCheckbox_MouseDown(object sender, MouseEventArgs e)
        {
            m_md = true;
            Invalidate();
        }

        private void MetroStyleCheckbox_MouseUp(object sender, MouseEventArgs e)
        {
            m_md = false;
            Invalidate();
        }

        [Category(Set)]
        public Color ActiveColor { get => m_brushes[0].Color; set { m_pens[0].Color = m_brushes[0].Color = value; Invalidate(); } }

        [Category(Set)]
        public Color MouseDownColor { get => m_brushes[1].Color; set { m_brushes[1].Color = value; Invalidate(); } }

        [Category(Set)]
        public Color DefaultColor { get => m_pens[1].Color; set { m_pens[1].Color = value; Invalidate(); } }

        [Category(Set)]
        public int BorderSize
        {
            get => m_size;
            set
            {
                m_size = value;
                for (int x = 0; x < penCount; x++)
                    m_pens[x].Width = value;

                Invalidate();
            }
        }

        [Category(Set)]
        public bool Checked { get => m_chk; set { m_chk = value; Invalidate(); } }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (m_md)
                pe.Graphics.FillRectangle(m_brushes[1], 0, 0, Width, Height);
            else
            {
                Pen p = m_pens[Checked ? 0 : 1];

                pe.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                if (Checked)
                {
                    int temp = m_size + single;
                    int xtmp = (temp * 2) - 1;

                    pe.Graphics.FillRectangle(m_brushes[0], temp - 1, temp - 1, Width - xtmp, Height - xtmp);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            int x;
            for (x = 0; x < penCount; x++)
            {
                m_pens[x].Dispose();
                m_pens[x] = null;
            }
            for (x = 0; x < brushCount; x++)
            {
                m_brushes[x].Dispose();
                m_brushes[x] = null;
            }
            m_pens = null;
            m_brushes = null;

            base.Dispose(disposing);
        }
    }
}
