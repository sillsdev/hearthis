using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.Widgets.Flying;

namespace HearThis
{
	public partial class ScriptControl : UserControl
	{
		private Animator _animator;
		private PointF _animationPoint;

		public ScriptControl()
		{
			InitializeComponent();
			Script =
				"The king’s scribes were summoned at that time, in the third month, which is the month of Sivan, on the twenty-third day. And an edict was written, according to all that Mordecai commanded concerning the Jews, to the satraps and the governors and the officials of the provinces from India to Ethiopia, 127 provinces";
			SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
		}

		public string Script { get; set; }
		private void ScriptControl_Load(object sender, EventArgs e)
		{

		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			RectangleF r;
			if(_animator ==null)
				r =  new RectangleF(e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height);
			else
			{
				int virtualTop = e.ClipRectangle.Bottom - Animator.GetValue(_animationPoint.X, 0, e.ClipRectangle.Height);
				//Debug.WriteLine(virtualTop);
				r = new RectangleF(e.ClipRectangle.Left, virtualTop, e.ClipRectangle.Width, e.ClipRectangle.Height);
			}
			e.Graphics.DrawString(Script, Font, Brushes.Black, r);
		}

		public void GoToScript(string selectedVerseText)
		{
			_animator = new Animator();
			_animator.Animate += new Animator.AnimateEventDelegate(animator_Animate);
			_animator.Finished+=new EventHandler((x,y)=>_animator = null);
			_animator.Start();
			Script = selectedVerseText;
			Invalidate();
		}

		void animator_Animate(object sender, Animator.AnimatorEventArgs e)
		{
			_animationPoint = e.Point;
			Invalidate();
		}
	}
}
