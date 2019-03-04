using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace CmslDesign
{
    public sealed partial class CmslRoundEdgeFlatButton : Button
    {
        private const string Category1 = "설정1";
        private const int BrushCount = 3;

        private byte ModeC;
        private int m_radi;
        private int m_olsize;
        private int m_insetsize;

        private SolidBrush[] m_brushes;
        private Pen m_pen;
        private StringFormat m_stfmt;

        public CmslRoundEdgeFlatButton()
        {
            InitializeComponent();

            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            // Region = new Region(new Rectangle(0, 0, Width + 1, Height + 1));

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = FlatAppearance.MouseOverBackColor = Color.Transparent;
            TabStop = false;

            ModeC = 3;

            MouseEnter += RoundEdgeFlatButton_MouseEnter;
            MouseLeave += RoundEdgeFlatButton_MouseLeave;
            MouseDown += RoundEdgeFlatButton_MouseDown;

            m_radi = 25;
            m_olsize = 4;
            m_insetsize = m_olsize / 2 + m_olsize % 2;

            m_brushes = new SolidBrush[BrushCount] { new SolidBrush(Color.DodgerBlue), new SolidBrush(Color.White), new SolidBrush(Color.FromArgb(128, 128, 255)) };
            m_pen = new Pen(m_brushes[0].Color, m_olsize) { Alignment = PenAlignment.Center };
            m_stfmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        }

        private void RoundEdgeFlatButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                ModeC = 1;
                Invalidate();
            }
        }

        private void RoundEdgeFlatButton_MouseLeave(object sender, EventArgs e)
        {
            ModeC = 3;
            Invalidate();
        }

        private void RoundEdgeFlatButton_MouseEnter(object sender, EventArgs e)
        {
            ModeC = 0;
            Invalidate(); 
        }

        [Category(Category1)]
        public int EdgeRadius
        {
            get => m_radi;
            set
            {
                if (m_radi < 0 || value / 2 >= Width || value / 2 >= Height) return;
                m_radi = value;
                Invalidate();
            }
        }

        [Category(Category1)]
        public int OutLineSize
        {
            get => m_olsize;
            set
            {
                if (m_radi < 0 || value >= Width || value >= Height) return;
                m_pen.Width = m_olsize = value;
                m_insetsize = value / 2 + value % 2;
                Invalidate();
            }
        }

        [Category(Category1)]
        public Color OutLineColor
        {
            get => m_brushes[0].Color;
            set
            {
                m_brushes[0].Color = m_pen.Color = value;
                Invalidate();
            }
        }

        [Category(Category1)]
        public Color FocusForeColor
        {
            get => m_brushes[1].Color;
            set
            {
                m_brushes[1].Color = value;
                Invalidate();
            }
        }

        [Category(Category1)]
        public Color MouseDownColor
        {
            get => m_brushes[2].Color;
            set
            {
                m_brushes[2].Color = value;
                Invalidate();
            }
        }

        private void AddRound(GraphicsPath gp, Rectangle Rect)
        {
            gp.AddArc(Rect.X, Rect.Y, Rect.Height, Rect.Height, 90, 180);
            gp.AddLine(Rect.Height / 2, Rect.Y, Rect.Width - Rect.Height / 2, Rect.Y);
            gp.AddArc(Rect.Width - Rect.Height, Rect.Y, Rect.Height, Rect.Height, -90, 180);
            gp.CloseFigure();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            pe.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            pe.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(pe);

            using (GraphicsPath gp = new GraphicsPath())
            {
                int v = m_insetsize * 2;
                AddRound(gp, new Rectangle(m_insetsize, m_insetsize, Width - v, Height - v));
                switch (ModeC)
                {
                    case 0:
                        pe.Graphics.FillPath(m_brushes[0], gp);
                        goto case 254;
                    case 1:
                        pe.Graphics.FillPath(m_brushes[2], gp);
                        goto case 254;
                    case 254:
                        pe.Graphics.DrawString(Text, Font, m_brushes[1], Width / 2f, Height / 2f, m_stfmt);
                        break;
                    default:
                        pe.Graphics.DrawPath(m_pen, gp);
                        break;
                }
            }
        }

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            for (int x = 0; x < BrushCount; x++)
            {
                m_brushes[x].Dispose();
                m_brushes[x] = null;
            }

            m_pen.Dispose();
            m_pen = null;

            m_stfmt.Dispose();
            m_stfmt = null;

            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
    }
}
