/* 
 * BSD 3-Clause License
 * 
 * Copyright 2019, ekfvoddl3536 (ekfvoddl3535@naver.com)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * 
 * Neither the name of the copyright holder nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *--------------------------------------------------------------------------------*/

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
        
        // m_radi 값은 제거됨 2019 03 05
        // 초기에는 수정할 수 있었으나, radi 값 수정이 굉장히 많은 오류를 내, 수정 불가능하게 바꿈
        private byte ModeC;
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

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = FlatAppearance.MouseOverBackColor = Color.Transparent;
            TabStop = false;

            ModeC = 3;

            MouseEnter += RoundEdgeFlatButton_MouseEnter;
            MouseLeave += RoundEdgeFlatButton_MouseLeave;
            MouseDown += RoundEdgeFlatButton_MouseDown;

            m_olsize = 4;
            m_insetsize = m_olsize / 2 + m_olsize % 2;

            m_brushes = new SolidBrush[BrushCount] { new SolidBrush(Color.DodgerBlue), new SolidBrush(Color.White), new SolidBrush(Color.FromArgb(128, 128, 255)) };
            // Alignment 이거 Center로 안하면, 안티 켜든 끄든 퀄리티 설정하든 말든 전부 흐리게 그려짐
            // 아무튼 Center로 해야됨
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
        public int OutLineSize
        {
            get => m_olsize;
            set
            {
                if (value >= Width || value >= Height) return;
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
            // 만약 m_radi를 사용하고 싶다면, 여기서 사용해야함
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
