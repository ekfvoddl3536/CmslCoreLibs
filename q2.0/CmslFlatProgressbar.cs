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
 
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CmslDesign
{
	public class CmslFlatProgressbar : Control
	{
		// "설정1"을 한영 바꾸면서 입력하기가 힘들다. const로 만들어 놓으면 굉장히 편하다.
		private const string Set = "설정1";
		
		protected int m_max;
		protected int m_min;
		protected int m_val;
		protected Color m_actcolor;
		// 과도한 GC 호출을 방지
		protected SolidBrush m_sb;
		
		public CmslFlatProgressbar()
		{
			m_max = 100;
			m_min = 0;
			m_val = 20;
			m_actcolor = Color.DodgerBlue;
			m_sb = new SolidBrush(m_actcolor);
		}
		
		// set에 Invalidate() 가 있어야만 값이 변경될 때마다 OnPaint가 호출된다.
		// 폼을 디자인 할 때 시각적인 변화를 즉시 볼 수 있다.
		[Category(Set)]
		public Color BarColor
		{
			get => m_actcolor;
			set
			{
				m_actcolor = value;
				m_sb.Color = value;
				Invalidate();
			}
		}
		
		// 1) Max 값이 1보다 작아지지 않도록 한다
		// 2) Max 값이 Min 보다 작으면 Min 값을 Max보다 1작게 설정한다.
		// 3) Max 값이 Val 보다 작으면 Val 값을 Max 와 같게 설정한다.
		[Category(Set)]
		public int MaximumStep
		{
			get => m_max;
			set
			{
				if (value < 1) return;
				if (value < m_min) m_min = value - 1;
				if (value < m_val) m_val = value;
				m_max = value;
				Invalidate();
			}
		}
		
		// 1) Min 값이 0보다 작아지지 않도록 한다 (음수가 되지 않게)
		// 2) Min 값이 Max 보다 크면 Max 값을 Min 보다 1크게 설정한다. checked 블록을 이용하여 오버플로를 검사한다.
		// 3) Min 값이 Val 보다 크면 Val 값을 Min 과 같게 설정한다.
		[Category(Set)]
		public int MinimumStep
		{
			get => m_min;
			set
			{
				if (value < 0) return;
				if (value > m_max) m_max = checked(value + 1);
				if (value > m_val) m_val = value;
				m_min = value;
				Invalidate();
			}
		}
		
		// 1) Val 값이 0 보다 작아지지 않도록 한다 (음수가 되지 않게)
		// 2) Val 값이 Max 보다 크면 Max 값을 Val 과 같게 설정한다.
		// 3) Val 값이 Min 보다 작으면 Min 값을 Val 과 같게 설정한다.
		[Category(Set)]
		public int CurrentStep
		{
			get => m_val;
			set
			{
				if (value < 0) return;
				if (value > m_max) m_max = value;
				if (value < m_min) m_min = value;
				m_val = value;
				Invalidate();
			}
		}
		
		// Val 값에 1을 추가하고 그린다
		public void AddStep() => AddStep(1);
		
		// Val 값이 (int)만큼 추가하고 그린다
		public void AddStep(int val)
		{
			if (checked(m_val + val) < m_max)
			{
				m_val += val;
				Invalidate();
			}
		}
		
		// m_max 또는 m_val을 (float)으로 만들어야만 한다
		// m_val (20) / m_max(100)을 int로 할 경우 0이 나온다.
		// (Visual Studio 2017 기준) '/'에 마우스를 올려보면 float 으로 계산하는지 int로 계산하는지 알 수 있다.
		protected virtual int GetBarWidth => (int)(m_val / (float)m_max * Width);
		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.FillRectangle(m_sb, 0, 0, GetBarWidth, Height);
		}

		protected override void Dispose(bool disposing)
		{
			m_sb.Dispose();
			m_sb = null;
			m_actcolor = Color.Empty;
			base.Dispose(disposing);
		}
	}
}
