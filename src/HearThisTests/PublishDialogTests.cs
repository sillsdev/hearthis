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

		// ----- Max-side tests -----

		[Test]
		public void MaxPauseValueChanged_UncheckedDownstreamPushedUpThenUpstreamLowered_DoesNotRaiseDownstreamValue()
		{
			// Regression test for the bug the reviewer found: Paragraph's Designer-default Max
			// (0.7, baked into PublishDialog.Designer.cs) is higher than the loaded/current
			// value (0.1, matching PauseData.kDefaultMax for a fresh/unconfigured project).
			// Lowering Clip Max back down must NOT cause Paragraph Max to jump up to 0.7.
			ParagraphCheckBox.Checked = false;
			ClipMax.Value = 0.1m;
			ParagraphMax.Value = 0.1m;

			// Raise Clip Max: existing push-up logic correctly pushes Paragraph Max up to match.
			ClipMax.Value = 0.2m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(0.2m), "Push-up should have raised Paragraph Max to match Clip Max.");

			// Lower Clip Max back down: pull-down branch fires (0.1 < 0.2, unchecked), computing
			// a floor of Math.Max(0.1, designerDefault=0.7) = 0.7. Without the never-increase
			// guard, that would incorrectly raise Paragraph Max from 0.2 up to 0.7.
			ClipMax.Value = 0.1m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(0.2m),
				"Paragraph Max must not be raised above its current value when the computed " +
				"Designer-default floor is higher than where it already sat.");
		}

		[Test]
		public void MaxPauseValueChanged_UncheckedDownstreamAboveDesignerDefault_PullsDownToFloorButNoFurther()
		{
			// Here the downstream tier gets pushed up *above* its own Designer default (0.7),
			// so lowering the upstream value back down should produce a real decrease, floored
			// at 0.7 -- confirming the original pull-down behavior (not just the new guard)
			// still works correctly.
			ParagraphCheckBox.Checked = false;
			ClipMax.Value = 0.1m;
			ParagraphMax.Value = 0.1m;

			ClipMax.Value = 1.0m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(1.0m), "Push-up should have raised Paragraph Max to match Clip Max.");

			ClipMax.Value = 0.3m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(0.7m),
				"Paragraph Max should be pulled down to its own Designer-default floor (0.7).");

			// Lower further, below the floor: Paragraph Max must not be pulled below 0.7.
			ClipMax.Value = 0.05m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(0.7m),
				"Paragraph Max must not be pulled below its own Designer-default floor.");
		}

		[Test]
		public void MaxPauseValueChanged_CheckedDownstream_NeverMovesDuringUpstreamRaiseOrLower()
		{
			// A checked (settled) downstream value must never be auto-reduced by the pull-down
			// branch. (Values are chosen so the pre-existing, unchanged push-up branch also
			// never fires here, isolating the pull-down behavior under test.)
			ParagraphCheckBox.Checked = true;
			ParagraphMax.Value = 1.0m;
			ClipMax.Value = 0.1m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(1.0m));

			ClipMax.Value = 0.3m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(1.0m), "Checked Paragraph Max must not move when Clip Max is raised.");

			ClipMax.Value = 0.1m;
			Assert.That(ParagraphMax.Value, Is.EqualTo(1.0m), "Checked Paragraph Max must not move when Clip Max is lowered.");
		}

		// ----- Min-side tests (symmetric; the reviewer noted the same root cause affects Min) -----

		[Test]
		public void MinPauseValueChanged_UncheckedDownstreamPushedUpThenUpstreamLowered_DoesNotRaiseDownstreamValue()
		{
			// Min-side Designer default is 0.1 for every tier, which is also the Min
			// controls' typical loaded value. To reproduce the same "Designer default is
			// HIGHER than the current cascaded value" bug condition as the Max-side test,
			// both Clip and Paragraph Min start BELOW the 0.1 Designer default (at 0.05,
			// still within the controls' allowed Minimum of 0.01).
			ParagraphCheckBox.Checked = false;
			ClipMin.Value = 0.05m;
			ParagraphMin.Value = 0.05m;

			// Raise Clip Min a little (still below the 0.1 Designer default): push-up
			// correctly pushes Paragraph Min up to match.
			ClipMin.Value = 0.08m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.08m), "Push-up should have raised Paragraph Min to match Clip Min.");

			// Lower Clip Min back down: pull-down fires (0.05 < 0.08, unchecked), computing a
			// floor of Math.Max(0.05, designerDefault=0.1) = 0.1. Without the never-increase
			// guard, that would incorrectly raise Paragraph Min from 0.08 up to 0.1.
			ClipMin.Value = 0.05m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.08m),
				"Paragraph Min must not be raised above its current value when the computed " +
				"Designer-default floor is higher than where it already sat.");
		}

		[Test]
		public void MinPauseValueChanged_UncheckedDownstreamAboveDesignerDefault_PullsDownToFloorButNoFurther()
		{
			// Here the downstream tier gets pushed up *above* its own Designer default (0.1),
			// so lowering the upstream value back down should produce a real decrease, floored
			// at 0.1 -- confirming the original pull-down behavior (not just the new guard)
			// still works correctly.
			ParagraphCheckBox.Checked = false;
			ClipMin.Value = 0.1m;
			ParagraphMin.Value = 0.1m;

			ClipMin.Value = 0.5m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.5m), "Push-up should have raised Paragraph Min to match Clip Min.");

			ClipMin.Value = 0.3m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.3m),
				"Pull-down should lower Paragraph Min to follow Clip Min (still above the 0.1 Designer default).");

			// Now lower Clip Min below Paragraph's Designer default floor (0.1): Paragraph Min
			// should settle at the floor, not below it, and a further lower must not raise it.
			ClipMin.Value = 0.1m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.1m), "Paragraph Min should be pulled down to its Designer-default floor (0.1).");

			ClipMin.Value = 0.02m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(0.1m), "Paragraph Min must not be pulled below its own Designer-default floor.");
		}

		[Test]
		public void MinPauseValueChanged_CheckedDownstream_NeverMovesDuringUpstreamRaiseOrLower()
		{
			ParagraphCheckBox.Checked = true;
			ParagraphMin.Value = 1.0m;
			ClipMin.Value = 0.1m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(1.0m));

			ClipMin.Value = 0.3m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(1.0m), "Checked Paragraph Min must not move when Clip Min is raised.");

			ClipMin.Value = 0.1m;
			Assert.That(ParagraphMin.Value, Is.EqualTo(1.0m), "Checked Paragraph Min must not move when Clip Min is lowered.");
		}
	}
}
