using System.Drawing;
using System.Windows.Forms;
using HearThis.Script;
using HearThis.UI;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// Test the tricks we do to limit preceding context when short of space
	/// </summary>
	[TestFixture]
	public class ScriptLinePainterTests
	{

		[Test]
		public void PaintsAllWhenPlentyOfSpace()
		{
			var block = new ScriptLine() {Text = "This is a test", FontName = "Arial", FontSize = 12};
			var rect = new RectangleF(0, 0, 1000, 500);
			using (var testForm = new Form())
			using (var gr = testForm.CreateGraphics())
			{
				var painter = new ScriptControl.ScriptBlockPainter(1.0f, Color.Black, gr, block, rect, 12, true);
				// Given that all the text fits easily, DoMaxHeight should return whatever the paint function returns.
				// The paint function should be passed all the text.
				Assert.That(painter.DoMaxHeight(500, input =>
					{
						Assert.That(input, Is.EqualTo("This is a test"));
						return 79;
					}), Is.EqualTo(79));
			}
		}

		/// <summary>
		/// The font size that the control uses for drawing all context text.
		/// </summary>
		private const float ContextFontSize = 0.9f*12;

		private Font ContextFont
		{
			get { return new Font("Arial", ContextFontSize); }
		}

		[Test]
		public void PaintsNothingWhenNoLineFits()
		{
			var block = new ScriptLine() { Text = "This is a test", FontName = "Arial", FontSize = 12 };
			var rect = new RectangleF(0, 0, 1000, 500);
			using (var testForm = new Form())
			using (var gr = testForm.CreateGraphics())
			{
				var painter = new ScriptControl.ScriptBlockPainter(1.0f, Color.Black, gr, block, rect, 12, true);
				// Below min height, paints nothing.
				Assert.That(painter.DoMaxHeight(5, input =>
				{
					Assert.Fail("Should not try to paint anything");
					return 79;
				}), Is.EqualTo(0));
			}
		}

		[Test]
		public void MakesASimpleSplitAtWordBreak()
		{
			var block = new ScriptLine() { Text = "This is a test with several words", FontName = "Arial", FontSize = 12 };
			using (var testForm = new Form())
			using (var gr = testForm.CreateGraphics())
			using (var font = ContextFont)
			{
				var size = gr.MeasureString("...with several words", font);
				var rect = new RectangleF(0, 0, size.Width + 2, 500); // "...with several words" will just fit
				var painter = new ScriptControl.ScriptBlockPainter(1.0f, Color.Black, gr, block, rect, 12, true);
				// Paints the trailing words that fit.
				Assert.That(painter.DoMaxHeight(size.Height + 2, input =>
				{
					Assert.That(input, Is.EqualTo("...with several words"));
					return 82;
				}), Is.EqualTo(82));
			}
		}

		[Test]
		public void PaintsNothingWhenNoWordFits()
		{
			var block = new ScriptLine() { Text = "This is a test with several words", FontName = "Arial", FontSize = 12 };
			using (var testForm = new Form())
			using (var gr = testForm.CreateGraphics())
			using (var font = ContextFont)
			{
				var size = TextRenderer.MeasureText(gr, "words", font);
				var rect = new RectangleF(0, 0, size.Width - 2, 500); //last word will not fit
				var painter = new ScriptControl.ScriptBlockPainter(1.0f, Color.Black, gr, block, rect, 12, true);
				// Does nothing.
				Assert.That(painter.DoMaxHeight(size.Height + 2, input =>
				{
					Assert.Fail("Should not try to paint anything, but tried to paint " + input);
					return 79;
				}), Is.EqualTo(0));
			}
		}

		[Test]
		public void PaintsNothingWhenNoSplitPossible()
		{
			var block = new ScriptLine() { Text = "Thisisatest", FontName = "Arial", FontSize = 12 };
			using (var testForm = new Form())
			using (var gr = testForm.CreateGraphics())
			using (var font = ContextFont)
			{
				var size = TextRenderer.MeasureText(gr, "Thisisatest", font);
				var rect = new RectangleF(0, 0, size.Width - 2, 500); //nothing will fit
				var painter = new ScriptControl.ScriptBlockPainter(1.0f, Color.Black, gr, block, rect, 12, true);
				// Does nothing.
				Assert.That(painter.DoMaxHeight(size.Height + 2, input =>
				{
					Assert.Fail("Should not try to paint anything");
					return 79;
				}), Is.EqualTo(0));
			}
		}

		[Test]
		public void PaintsNothingWhenNoWordFitsWithDots()
		{
			var block = new ScriptLine() { Text = "This is a test with several words", FontName = "Arial", FontSize = 12 };
			using (var testForm = new Form())
			using (var gr = testForm.CreateGraphics())
			using (var font = ContextFont)
			{
				var size = TextRenderer.MeasureText(gr, "words", font);
				var rect = new RectangleF(0, 0, size.Width +1, 500); //last word will fit, but not with the ...
				var painter = new ScriptControl.ScriptBlockPainter(1.0f, Color.Black, gr, block, rect, 12, true);
				// Does nothing
				Assert.That(painter.DoMaxHeight(size.Height + 2, input =>
				{
					Assert.Fail("Should not try to paint anything");
					return 79;
				}), Is.EqualTo(0));
			}
		}

		[Test]
		public void CanTruncateJustOneWord()
		{
			var block = new ScriptLine() { Text = "This is a test with several words", FontName = "Arial", FontSize = 12 };
			using (var testForm = new Form())
			using (var gr = testForm.CreateGraphics())
			using (var font = ContextFont)
			{
				var size = gr.MeasureString("his is a test with several words", font);
				var rect = new RectangleF(0, 0, size.Width + 1, 500); //all but one letter will fit
				var painter = new ScriptControl.ScriptBlockPainter(1.0f, Color.Black, gr, block, rect, 12, true);
				// Given that all the text fits easily, DoMaxHeight should return whatever the paint function returns.
				// The paint function should be passed all the text.
				Assert.That(painter.DoMaxHeight(size.Height + 2, input =>
				{
					Assert.That(input, Is.EqualTo("... is a test with several words"));
					return 65;
				}), Is.EqualTo(65));
			}
		}

		[Test]
		public void AllFitsOnTwoLines()
		{
			var block = new ScriptLine() { Text = "This is a test with several words", FontName = "Arial", FontSize = 12 };
			using (var testForm = new Form())
			using (var gr = testForm.CreateGraphics())
			using (var font = ContextFont)
			{
				var size = gr.MeasureString("his is a test with several words", font);
				var rect = new RectangleF(0, 0, size.Width + 1, 500); //all but one letter will fit
				var painter = new ScriptControl.ScriptBlockPainter(1.0f, Color.Black, gr, block, rect, 12, true);
				Assert.That(painter.DoMaxHeight(size.Height * 2.5f, input =>
				{
					Assert.That(input, Is.EqualTo("This is a test with several words"));
					return 65;
				}), Is.EqualTo(65));
			}
		}

		/// <summary>
		/// This test may prove somewhat flaky. It's not entirely obvious how much text will fit on the first line;
		/// it might be that a slightly different amount will fit at different resolutions or on Linux.
		/// </summary>
		[Test]
		public void MostFitsOnTwoLines()
		{
			//                                           several words several words
			var block = new ScriptLine() { Text = "This is a test with several words", FontName = "Arial", FontSize = 12 };
			using (var testForm = new Form())
			using (var gr = testForm.CreateGraphics())
			using (var font = ContextFont)
			{
				var size = TextRenderer.MeasureText(gr, "several words", font);
				var rect = new RectangleF(0, 0, size.Width + 1, 500);
				var painter = new ScriptControl.ScriptBlockPainter(1.0f, Color.Black, gr, block, rect, 12, true);
				Assert.That(painter.DoMaxHeight(size.Height * 2.5f, input =>
				{
					Assert.That(input, Is.EqualTo("... a test with several words"));
					return 65;
				}), Is.EqualTo(65));
			}
		}
	}
}
