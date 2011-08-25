using System;
using System.Drawing;
using System.Windows.Forms;
using HearThis.Script;
using Palaso.UI.WindowsForms.Widgets.Flying;

namespace HearThis.UI
{
	public partial class ScriptControl : UserControl
	{
		private Animator _animator;
		private PointF _animationPoint;
		private ScriptLine _outgoingScript;
		private Direction _direction;

		public ScriptControl()
		{
			InitializeComponent();
			Script = new ScriptLine(
				"The king’s scribes were summoned at that time, in the third month, which is the month of Sivan, on the twenty-third day. And an edict was written, according to all that Mordecai commanded concerning the Jews, to the satraps and the governors and the officials of the provinces from India to Ethiopia, 127 provinces");
			SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
		}

		public ScriptLine Script { get; set; }
		private void ScriptControl_Load(object sender, EventArgs e)
		{

		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Script == null)
				return;

				RectangleF r;
				if (_animator == null)
				{
					r = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
					DrawScript(e.Graphics, Script, r, true);
				}
				else
				{
					if (_direction == Direction.Down)
					{
						int virtualTop = Animator.GetValue(_animationPoint.X, 0,
														   0 - Bounds.Height);
						r = new RectangleF(0, virtualTop, Bounds.Width, Bounds.Height*2);
						DrawScript(e.Graphics, _outgoingScript, r, false);

						virtualTop = Animator.GetValue(_animationPoint.X, Bounds.Bottom, 0);
						r = new RectangleF(0, virtualTop, Bounds.Width, Bounds.Height*2);
						DrawScript(e.Graphics, Script, r, true);
					}
					else
					{
						int virtualTop = Animator.GetValue(_animationPoint.X, 0,
														   0 + Bounds.Height);
						r = new RectangleF(0, virtualTop, Bounds.Width, Bounds.Height*2);
						DrawScript(e.Graphics, _outgoingScript, r, false);

						virtualTop = Animator.GetValue(_animationPoint.X, 0 - Bounds.Height, 0);
						r = new RectangleF(0, virtualTop, Bounds.Width, Bounds.Height*2);
						DrawScript(e.Graphics, Script, r, true);
					}
				}

		}

		private static void DrawScript(Graphics graphics, ScriptLine script, RectangleF rectangle, bool enabled)
		{
			if (script == null)
				return;

			FontStyle fontStyle=default(FontStyle);
			if(script.Bold)
				fontStyle = FontStyle.Bold;
			StringFormat alignment = new StringFormat();
			if(script.Centered)
				alignment.Alignment = StringAlignment.Center;


			using (var font = new Font(script.FontName, script.FontSize, fontStyle))
			{
				graphics.DrawString(script.Text, font, enabled?Brushes.Black:Brushes.Gray, rectangle, alignment);
			}
		}

		/* protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			RectangleF r;
			if(_animator ==null)
			{
				r =  new RectangleF(e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height);
				e.Graphics.DrawString(Script, Font, Brushes.Black, r);
			}
			else
			{
				if (_direction == Direction.Down)
				{
					int virtualTop = Animator.GetValue(_animationPoint.X, e.ClipRectangle.Top,
													   e.ClipRectangle.Top - e.ClipRectangle.Height);
					r = new RectangleF(e.ClipRectangle.Left, virtualTop, e.ClipRectangle.Width, e.ClipRectangle.Height*2);
					e.Graphics.DrawString(_outgoingScript, Font, Brushes.Gray, r);

					virtualTop = Animator.GetValue(_animationPoint.X, e.ClipRectangle.Bottom, e.ClipRectangle.Top);
					r = new RectangleF(e.ClipRectangle.Left, virtualTop, e.ClipRectangle.Width, e.ClipRectangle.Height*2);
					e.Graphics.DrawString(Script, Font, Brushes.Black, r);
				}
				else
				{
					int virtualTop = Animator.GetValue(_animationPoint.X, e.ClipRectangle.Top,
													   e.ClipRectangle.Top + e.ClipRectangle.Height);
					r = new RectangleF(e.ClipRectangle.Left, virtualTop, e.ClipRectangle.Width, e.ClipRectangle.Height*2);
					e.Graphics.DrawString(_outgoingScript, Font, Brushes.Gray, r);

					virtualTop = Animator.GetValue(_animationPoint.X, e.ClipRectangle.Top - e.ClipRectangle.Height, e.ClipRectangle.Top);
					r = new RectangleF(e.ClipRectangle.Left, virtualTop, e.ClipRectangle.Width, e.ClipRectangle.Height*2);
					e.Graphics.DrawString(Script, Font, Brushes.Black, r);
				}
			}

		}
*/
		public enum Direction
		{
			Up,
			Down
		}

		public void GoToScript(Direction direction, ScriptLine script)
		{
			_direction = direction;
			_outgoingScript = Script;
			_animator = new Animator();
			_animator.Animate += new Animator.AnimateEventDelegate(animator_Animate);
			_animator.Finished += new EventHandler((x, y) => { _animator = null;
																 _outgoingScript = null;
			});
			_animator.Start();
			Script = script;
			Invalidate();
		}

		void animator_Animate(object sender, Animator.AnimatorEventArgs e)
		{
			_animationPoint = e.Point;
			Invalidate();
		}
	}
}
