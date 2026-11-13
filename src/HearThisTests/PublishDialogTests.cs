// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2026, SIL Global.
// <copyright from='2026' to='2026' company='SIL Global'>
//		Copyright (c) 2026, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using NUnit.Framework;

namespace HearThisTests
{
	/// <summary>
	/// Tests for the Min/Max pause-cascade logic in PublishDialog (MinPauseValueChanged /
	/// MaxPauseValueChanged), in particular the invariant that a downstream (broader-scope)
	/// pause tier's value is never smaller than an upstream (narrower-scope) tier's, and the
	/// symmetric rule that an *unsettled* (checkbox-unchecked) downstream value is allowed to
	/// shrink back down when the upstream value is reduced, floored at the downstream
	/// control's own Designer-default value, but a *settled* (checkbox-checked) downstream
	/// value is never auto-reduced.
	///
	/// These tests instantiate the real PublishDialog (using the same TestScriptProvider /
	/// Project construction pattern as ProjectTests.cs) and drive the actual private
	/// NumericUpDown/CheckBox controls via reflection, so the production ValueChanged event
	/// handlers wired up in PublishDialog.Designer.cs are the code under test -- not a
	/// reimplementation of the cascade logic.
	/// </summary>
	[TestFixture]
	public class PublishDialogTests
	{
		// TestScriptProvider.EthnologueCode (from ProjectTests.cs) throws NotImplementedException,
		// which PublishDialog's constructor needs (via PublishingModel's constructor) in order to
		// construct successfully. Override it here rather than changing the shared test double.
		private class TestScriptProviderWithEthnologueCode : TestScriptProvider
		{
			// Must call the base overload that runs Initialize() (which populates
			// ProjectSettings with defaults), not TestScriptProvider's true parameterless
			// constructor, which leaves ProjectSettings null.
			public TestScriptProviderWithEthnologueCode() : base(null) { }

			public override string EthnologueCode => "eng";
		}

		private PublishDialog _dialog;

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			// PublishDialog's constructor calls LocalizationManager.GetString (e.g. for the
			// "paragraph pause unavailable" tooltip, since TestScriptProvider's
			// TracksParagraphStarts is false), which throws unless at least one
			// LocalizationManager has been created in this process.
			LocalizationManager.StrictInitializationMode = false;
			try
			{
				var tempDir = Path.Combine(Path.GetTempPath(), "HearThisTests_L10NSharp_" + Guid.NewGuid());
				Directory.CreateDirectory(tempDir);
				LocalizationManager.Create("en", "HearThisTests", "HearThisTests", "1.0.0", tempDir,
					Path.Combine("SIL", "HearThisTests"), null, "", new[] { "HearThis" }, null);
			}
			catch (ArgumentException)
			{
				// Another fixture running in this process already created a LocalizationManager
				// (e.g. with the same appId); that's sufficient -- GetString only needs at least
				// one LocalizationManager to exist anywhere in the process.
			}
		}

		[SetUp]
		public void Setup()
		{
			var project = new Project(new TestScriptProviderWithEthnologueCode());
			_dialog = new PublishDialog(project, false);
		}

		[TearDown]
		public void TearDown()
		{
			_dialog?.Dispose();
			_dialog = null;
		}

		private static T GetField<T>(object target, string fieldName)
		{
			var fieldInfo = typeof(PublishDialog).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(fieldInfo, Is.Not.Null, $"Expected a private field named '{fieldName}' on PublishDialog.");
			return (T)fieldInfo.GetValue(target);
		}

		private NumericUpDown ClipMax => GetField<NumericUpDown>(_dialog, "_numericClipPauseMax");
		private NumericUpDown ParagraphMax => GetField<NumericUpDown>(_dialog, "_numericParagraphPauseMax");
		private CheckBox ParagraphCheckBox => GetField<CheckBox>(_dialog, "_chkParagraphPauses");

		private NumericUpDown ClipMin => GetField<NumericUpDown>(_dialog, "_numericClipPauseMin");
		private NumericUpDown ParagraphMin => GetField<NumericUpDown>(_dialog, "_numericParagraphPauseMin");
		private NumericUpDown ChapterMin => GetField<NumericUpDown>(_dialog, "_numericChapterPauseMin");
		private NumericUpDown ChapterMax => GetField<NumericUpDown>(_dialog, "_numericChapterPauseMax");
		private NumericUpDown PostHeadingMin => GetField<NumericUpDown>(_dialog, "_numericPostHeadingPauseMin");
		private NumericUpDown PostHeadingMax => GetField<NumericUpDown>(_dialog, "_numericPostHeadingPauseMax");

		// ----- Max-side tests -----

		[Test]
		public void MaxPauseValueChanged_UncheckedDownstreamPushedUpThenUpstreamLowered_DoesNotRaiseDownstreamValue()
		{
			ParagraphCheckBox.Checked = false;
			ClipMax.Value = 0.60m;
			ParagraphMax.Value = 0.60m;

			ClipMax.Value = 0.70m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(0.70m), "Push-up should have raised Paragraph Max to match Clip Max.");

			ClipMax.Value = 0.60m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(0.70m),
				"Paragraph Max must not be raised above its current value when the computed " +
				"Designer-default floor (0.90) is higher than where it already sat.");
		}

		[Test]
		public void MaxPauseValueChanged_UncheckedDownstreamAboveDesignerDefault_PullsDownToFloorButNoFurther()
		{
			ParagraphCheckBox.Checked = false;
			ClipMax.Value = 0.60m;
			ParagraphMax.Value = 0.60m;

			ClipMax.Value = 1.20m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(1.20m), "Push-up should have raised Paragraph Max to match Clip Max.");

			ClipMax.Value = 0.50m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(0.90m),
				"Paragraph Max should be pulled down to its own Designer-default floor (0.90).");

			ClipMax.Value = 0.10m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(0.90m),
				"Paragraph Max must not be pulled below its own Designer-default floor.");
		}

		[Test]
		public void MaxPauseValueChanged_CheckedDownstream_NeverMovesDuringUpstreamRaiseOrLower()
		{
			ParagraphCheckBox.Checked = true;
			ParagraphMax.Value = 1.50m;
			ClipMax.Value = 0.60m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(1.50m));

			ClipMax.Value = 1.00m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(1.50m), "Checked Paragraph Max must not move when Clip Max is raised.");

			ClipMax.Value = 0.60m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(1.50m), "Checked Paragraph Max must not move when Clip Max is lowered.");
		}

		// ----- Min-side tests (symmetric; the reviewer noted the same root cause affects Min) -----

		[Test]
		public void MinPauseValueChanged_UncheckedDownstreamPushedUpThenUpstreamLowered_DoesNotRaiseDownstreamValue()
		{
			ParagraphCheckBox.Checked = false;
			ClipMin.Value = 0.50m;
			ParagraphMin.Value = 0.50m;

			ClipMin.Value = 0.60m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.60m), "Push-up should have raised Paragraph Min to match Clip Min.");

			ClipMin.Value = 0.50m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.60m),
				"Paragraph Min must not be raised above its current value when the computed " +
				"Designer-default floor (0.75) is higher than where it already sat.");
		}

		[Test]
		public void MinPauseValueChanged_UncheckedDownstreamAboveDesignerDefault_PullsDownToFloorButNoFurther()
		{
			ParagraphCheckBox.Checked = false;
			ClipMin.Value = 0.75m;
			ParagraphMin.Value = 0.75m;

			ClipMin.Value = 1.20m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(1.20m), "Push-up should have raised Paragraph Min to match Clip Min.");

			ClipMin.Value = 0.90m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.90m),
				"Pull-down should lower Paragraph Min to follow Clip Min (still above the 0.75 Designer default).");

			ClipMin.Value = 0.75m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.75m), "Paragraph Min should be pulled down to its Designer-default floor (0.75).");

			ClipMin.Value = 0.60m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.75m), "Paragraph Min must not be pulled below its own Designer-default floor.");
		}

		[Test]
		public void MinPauseValueChanged_CheckedDownstream_NeverMovesDuringUpstreamRaiseOrLower()
		{
			ParagraphCheckBox.Checked = true;
			ParagraphMin.Value = 1.50m;
			ClipMin.Value = 0.75m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(1.50m));

			ClipMin.Value = 1.00m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(1.50m), "Checked Paragraph Min must not move when Clip Min is raised.");

			ClipMin.Value = 0.75m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(1.50m), "Checked Paragraph Min must not move when Clip Min is lowered.");
		}

		[Test]
		public void PostHeadingPauseRow_Exists_WithDesignerDefaultsMatchingSpec()
		{
			var checkBox = GetField<CheckBox>(_dialog, "_chkPostHeadingPauses");
			var min = GetField<NumericUpDown>(_dialog, "_numericPostHeadingPauseMin");
			var max = GetField<NumericUpDown>(_dialog, "_numericPostHeadingPauseMax");

			Assert.That(checkBox.Checked, Is.False);
			Assert.That(min.Value, Is.EqualTo(1.45m));
			Assert.That(max.Value, Is.EqualTo(1.55m));
		}

		[Test]
		public void AllPauseRows_DesignerDefaults_MatchSpecTable()
		{
			Assert.That(GetField<NumericUpDown>(_dialog, "_numericClipPauseMin").Value, Is.EqualTo(0.40m));
			Assert.That(GetField<NumericUpDown>(_dialog, "_numericClipPauseMax").Value, Is.EqualTo(0.75m));
			Assert.That(GetField<NumericUpDown>(_dialog, "_numericParagraphPauseMin").Value, Is.EqualTo(0.75m));
			Assert.That(GetField<NumericUpDown>(_dialog, "_numericParagraphPauseMax").Value, Is.EqualTo(0.90m));
			Assert.That(GetField<NumericUpDown>(_dialog, "_numericSectionPauseMin").Value, Is.EqualTo(1.15m));
			Assert.That(GetField<NumericUpDown>(_dialog, "_numericSectionPauseMax").Value, Is.EqualTo(1.25m));
			Assert.That(GetField<NumericUpDown>(_dialog, "_numericChapterPauseMin").Value, Is.EqualTo(1.25m));
			Assert.That(GetField<NumericUpDown>(_dialog, "_numericChapterPauseMax").Value, Is.EqualTo(2.00m));
		}

		[Test]
		public void ChapterPauseMax_RaisedOrLowered_DoesNotMovePostHeadingPauseMax()
		{
			var original = PostHeadingMax.Value;

			ChapterMax.Value = ChapterMax.Value + 1.0m;
			Assert.That(PostHeadingMax.Value, Is.EqualTo(original), "Raising Chapter Max must not move PostHeading Max.");

			ChapterMax.Value = ChapterMax.Minimum;
			Assert.That(PostHeadingMax.Value, Is.EqualTo(original), "Lowering Chapter Max must not move PostHeading Max.");
		}

		[Test]
		public void PostHeadingPauseMax_RaisedOrLowered_DoesNotMoveChapterPauseMax()
		{
			var original = ChapterMax.Value;

			PostHeadingMax.Value = PostHeadingMax.Value + 1.0m;
			Assert.That(ChapterMax.Value, Is.EqualTo(original), "Raising PostHeading Max must not move Chapter Max.");
		}

		[Test]
		public void ParagraphPauseMax_RaisedAbovePostHeadingPauseMax_RaisesPostHeadingPauseMax()
		{
			ParagraphMax.Value = PostHeadingMax.Value + 1.0m;

			Assert.That(PostHeadingMax.Value, Is.EqualTo(ParagraphMax.Value),
				"PostHeading Max must never sit below Paragraph Max.");
		}

		[Test]
		public void PostHeadingPauseMax_LoweredBelowParagraphPauseMax_IsClampedToParagraphPauseMax()
		{
			PostHeadingMax.Value = ParagraphMax.Value - 0.01m;

			Assert.That(PostHeadingMax.Value, Is.EqualTo(ParagraphMax.Value),
				"PostHeading Max must be clamped up to Paragraph Max, not left below it.");
		}

		[Test]
		public void ParagraphPauseMin_RaisedAbovePostHeadingPauseMin_RaisesPostHeadingPauseMin_RegardlessOfApplyState()
		{
			ParagraphCheckBox.Checked = false;
			ParagraphMin.Value = PostHeadingMin.Value + 1.0m;
			Assert.That(PostHeadingMin.Value, Is.EqualTo(ParagraphMin.Value));

			ParagraphCheckBox.Checked = true;
			ParagraphMin.Value = PostHeadingMin.Value + 1.0m;
			Assert.That(PostHeadingMin.Value, Is.EqualTo(ParagraphMin.Value));
		}
	}
}
