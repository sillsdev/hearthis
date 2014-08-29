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
			Assert.AreEqual(0, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnFirstSegmentAndSegWidthGreaterThanThumbWidth_CenteredOnFirstSegment()
		{
			MakeSegWidthGreaterThanThumbWidth();
			_sut.Value = 0;
			Assert.AreEqual(10, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnFirstSegmentAndSegWidthEqualsThumbWidth_StartsAtLeftMargin()
		{
			MakeSegWidthEqualThumbWidth();
			_sut.Value = 0;
			Assert.AreEqual(0, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnLesserMiddleSegmentAndSegWidthLessThanThumbWidth_CenteredOnLesserMiddleSegment()
		{
			MakeSegWidthLessThanThumbWidth();
			_sut.Value = 29;
			Assert.AreEqual(383, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnLesserMiddleSegmentAndSegWidthGreaterThanThumbWidth_CenteredOnLesserMiddleSegment()
		{
			MakeSegWidthGreaterThanThumbWidth();
			_sut.Value = 9;
			Assert.AreEqual(370, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnLesserMiddleSegmentAndSegWidthEqualsThumbWidth_StartsAtLeftEdgeOfLesserMiddleSegment()
		{
			MakeSegWidthEqualThumbWidth();
			_sut.Value = 19;
			Assert.AreEqual(380, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnGreaterMiddleSegmentAndSegWidthLessThanThumbWidth_CenteredOnGreaterMiddleSegment()
		{
			MakeSegWidthLessThanThumbWidth();
			_sut.Value = 30;
			Assert.AreEqual(397, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnGreaterMiddleSegmentAndSegWidthGreaterThanThumbWidth_CenteredOnGreaterMiddleSegment()
		{
			MakeSegWidthGreaterThanThumbWidth();
			_sut.Value = 10;
			Assert.AreEqual(410, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnGreaterMiddleSegmentAndSegWidthEqualsThumbWidth_StartsAtMiddleOfBar()
		{
			MakeSegWidthEqualThumbWidth();
			_sut.Value = 20;
			Assert.AreEqual(400, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnLastSegmentAndSegWidthLessThanThumbWidth_PositionedAtEndOfBar()
		{
			MakeSegWidthLessThanThumbWidth();
			_sut.Value = 59;
			Assert.AreEqual(780, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnLastSegmentAndSegWidthGreaterThanThumbWidth_CenteredOnLastSegment()
		{
			MakeSegWidthGreaterThanThumbWidth();
			_sut.Value = 19;
			Assert.AreEqual(770, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_ThumbOnLastSegmentAndSegWidthEqualsThumbWidth_PositionedAtEndOfBar()
		{
			MakeSegWidthEqualThumbWidth();
			_sut.Value = 39;
			Assert.AreEqual(780, _sut.ThumbRectangle.Left);
		}

		[Test]
		public void ThumbRectangle_Finished_Empty()
		{
			_sut.SegmentCount = 2;
			_sut.Finished = true;
			Assert.IsTrue(_sut.ThumbRectangle.IsEmpty);
		}

		[Test]
		public void ThumbRectangle_NoSegments_Empty()
		{
			_sut.SegmentCount = 0;
			Assert.IsTrue(_sut.ThumbRectangle.IsEmpty);
		}
	}
}
