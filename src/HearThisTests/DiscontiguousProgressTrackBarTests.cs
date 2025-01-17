using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using HearThis.UI;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// Test the calculations used in DiscontiguousProgressTrackBarTests
	/// </summary>
	[TestFixture]
	public class DiscontiguousProgressTrackBarTests
	{
		private DiscontiguousProgressTrackBar _sut;

		[SetUp]
		public void Setup()
		{
			_sut = new DiscontiguousProgressTrackBar();
		}

		private void MakeSegWidthLessThanThumbWidth()
		{
			// Thumb width is a constant 20, so for segment width to be less than thumb width,
			// the usable width / 20 must be < the number of segments.
			_sut.BarWidth = 800;
			_sut.SegmentCount = 60;
		}

		private void MakeSegWidthGreaterThanThumbWidth()
		{
			// Thumb width is a constant 20, so for segment width to be less than thumb width,
			// the usable width / 20 must be > the number of segments.
			_sut.BarWidth = 800;
			_sut.SegmentCount = 20;
		}

		private void MakeSegWidthEqualThumbWidth()
		{
			// Thumb width is a constant 20, so for segment width to be less than thumb width,
			// the usable width / 20 must be = the number of segments.
			_sut.BarWidth = 800;
			_sut.SegmentCount = 40;
		}

		[Test]
		public void ThumbRectangle_ThumbOnFirstSegmentAndSegWidthLessThanThumbWidth_StartsAtLeftMargin()
		{
			MakeSegWidthLessThanThumbWidth();
			_sut.Value = 0;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(0));
		}

		[Test]
		public void ThumbRectangle_ThumbOnFirstSegmentAndSegWidthGreaterThanThumbWidth_CenteredOnFirstSegment()
		{
			MakeSegWidthGreaterThanThumbWidth();
			_sut.Value = 0;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(10));
		}

		[Test]
		public void ThumbRectangle_ThumbOnFirstSegmentAndSegWidthEqualsThumbWidth_StartsAtLeftMargin()
		{
			MakeSegWidthEqualThumbWidth();
			_sut.Value = 0;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(0));
		}

		[Test]
		public void ThumbRectangle_ThumbOnLesserMiddleSegmentAndSegWidthLessThanThumbWidth_CenteredOnLesserMiddleSegment()
		{
			MakeSegWidthLessThanThumbWidth();
			_sut.Value = 29;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(383));
		}

		[Test]
		public void ThumbRectangle_ThumbOnLesserMiddleSegmentAndSegWidthGreaterThanThumbWidth_CenteredOnLesserMiddleSegment()
		{
			MakeSegWidthGreaterThanThumbWidth();
			_sut.Value = 9;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(370));
		}

		[Test]
		public void ThumbRectangle_ThumbOnLesserMiddleSegmentAndSegWidthEqualsThumbWidth_StartsAtLeftEdgeOfLesserMiddleSegment()
		{
			MakeSegWidthEqualThumbWidth();
			_sut.Value = 19;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(380));
		}

		[Test]
		public void ThumbRectangle_ThumbOnGreaterMiddleSegmentAndSegWidthLessThanThumbWidth_CenteredOnGreaterMiddleSegment()
		{
			MakeSegWidthLessThanThumbWidth();
			_sut.Value = 30;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(397));
		}

		[Test]
		public void ThumbRectangle_ThumbOnGreaterMiddleSegmentAndSegWidthGreaterThanThumbWidth_CenteredOnGreaterMiddleSegment()
		{
			MakeSegWidthGreaterThanThumbWidth();
			_sut.Value = 10;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(410));
		}

		[Test]
		public void ThumbRectangle_ThumbOnGreaterMiddleSegmentAndSegWidthEqualsThumbWidth_StartsAtMiddleOfBar()
		{
			MakeSegWidthEqualThumbWidth();
			_sut.Value = 20;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(400));
		}

		[Test]
		public void ThumbRectangle_ThumbOnLastSegmentAndSegWidthLessThanThumbWidth_PositionedAtEndOfBar()
		{
			MakeSegWidthLessThanThumbWidth();
			_sut.Value = 59;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(780));
		}

		[Test]
		public void ThumbRectangle_ThumbOnLastSegmentAndSegWidthGreaterThanThumbWidth_CenteredOnLastSegment()
		{
			MakeSegWidthGreaterThanThumbWidth();
			_sut.Value = 19;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(770));
		}

		[Test]
		public void ThumbRectangle_ThumbOnLastSegmentAndSegWidthEqualsThumbWidth_PositionedAtEndOfBar()
		{
			MakeSegWidthEqualThumbWidth();
			_sut.Value = 39;
			Assert.That(_sut.ThumbRectangle.Left, Is.EqualTo(780));
		}

		[TestCase(25, 13, 3, 3, 0, 0)]
		[TestCase(30, 13, 3, 3, 0, 0)]
		[TestCase(19, 10, 3, 3, 0, 0)]
		[TestCase(25, 16, 3, 3, 0, 0)]
		[TestCase(25, 10, 3, 3, 0, 0)]
		[TestCase(25, 13, 3, 3, 2, 1)]
		[TestCase(25, 13, 3, 3, 1, 3)]
		[TestCase(25, 13, 2, 3, 0, 0)]
		[TestCase(25, 13, 0, 1, 0, 0)]
		[TestCase(25, 13, 5, 0, 0, 0)]
		[TestCase(25, 13, 3, 4, 1, 1)]
		[TestCase(25, 13, 3, 0, 1, 1)]
		public void ThumbRectangle_ThumbOnLastSegmentAndSegWidthEqualsThumbWidth_HeightNeverGreaterThanControlHeightMinusPadding(
			int controlHeight, float fontSize, int topMargin, int bottomMargin, int topPadding, int bottomPadding)
		{
			_sut.SegmentCount = 6;
			_sut.Height = controlHeight;
			_sut.Margin = new Padding(0, topMargin, 0, bottomMargin);
			_sut.Padding = new Padding(0, topPadding, 0, bottomPadding);
			using (var font = new Font(_sut.Font.FontFamily, fontSize))
			{
				_sut.Font = font;
				_sut.Value = 3;
				Assert.That(_sut.ThumbRectangle.Top, Is.GreaterThanOrEqualTo(topPadding));
				Assert.That(_sut.ThumbRectangle.Height, Is.LessThanOrEqualTo(controlHeight - topPadding - bottomPadding));
				Assert.That(_sut.ThumbRectangle.Bottom, Is.LessThanOrEqualTo(controlHeight - bottomPadding));
			}
		}

		[Test]
		public void ThumbRectangle_NoSegments_Empty()
		{
			_sut.SegmentCount = 0;
			Assert.That(_sut.ThumbRectangle.IsEmpty, Is.True);
		}

		[Test]
		public void GetSegmentCount_GetSegmentBrushesOverridden_ReturnsActualCountOfSegmentBrushes()
		{
			_sut.GetSegmentBrushesDelegate =
				() => BrushesToPaintInfo(new Brush[] { new HatchBrush(HatchStyle.BackwardDiagonal, Color.AliceBlue), new SolidBrush(Color.Aquamarine) });
			Assert.That(_sut.SegmentCount, Is.EqualTo(2));
		}

		[Test]
		public void SetSegmentCount_GetSegmentBrushesOverridden_ThrowsInvalidOperationException()
		{
			_sut.GetSegmentBrushesDelegate =
				() => BrushesToPaintInfo(new Brush[] { new HatchBrush(HatchStyle.BackwardDiagonal, Color.AliceBlue), new SolidBrush(Color.Aquamarine) });
			Assert.Throws<InvalidOperationException>(() => _sut.SegmentCount = 45);
		}

		SegmentPaintInfo[] BrushesToPaintInfo(Brush[] brushes)
		{
			return brushes.Select(b => new SegmentPaintInfo() {MainBrush = b}).ToArray();
		}
	}
}
