// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2015-2025, SIL Global.
// <copyright from='2015' to='2025' company='SIL Global'>
//		Copyright (c) 2015-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using L10NSharp;
using SIL.Reporting;
using SIL.Windows.Forms.Extensions;
using SIL.Windows.Forms.Progress;
using ZXing;
using static System.String;
using static System.Windows.Forms.MessageBoxButtons;
using static SIL.Windows.Forms.Extensions.ControlExtensions.ErrorHandlingAction;

namespace HearThis.UI
{
	/// <summary>
	/// This dialog is responsible to obtain the IP address of the Android we want to sync with,
	/// if possible. Currently, this is done by displaying a QR code representing our own IP
	/// address, which the client must set using <see cref="SetOurIpAddress"/>. The Android device
	/// scans this and sends a packet containing its own IP address. When this class receives that
	/// successfully, it closes (with <see cref="DialogResult.OK"/>). If instead the user closes
	/// the dialog, it will return another code, I think Cancel.
	/// Enhance JT: handle the possibility that the Android device does not have a functional
	/// scanner.
	/// Display the Android device's IP address on its screen, and tell the user to type it in a
	/// new text box (or 4) here, then click OK.
	/// Enhance JT: possibly the QR code could alternatively convey a BlueTooth IP address.
	/// </summary>
	public partial class AndroidSyncDialog : Form, ILocalizable
	{
		/// <summary>
		/// This is the placeholder for the last octet of the IP address.
		/// </summary>
		/// <remarks>
		/// ENHANCE: For now, this is hard-coded, but it should probably be localized. (An even
		/// better thing might be to make a custom control that uses good UI cues to make it clear
		/// that this is a placeholder, perhaps along with a tooltip or more info in the preceding
		/// label offering a clear explanation.) Something like this might become more urgent when
		/// we support Arabic, but for now we can maybe wait until someone requests it. Manual sync
		/// is already not the "happy path."</remarks>
		private readonly string _lastOctetPlaceholder = "???";
		private string _uneditedValueInSelectedIPAddressBox;
		private UDPListener _listener;
		private IPAddress _ourIpAddress;

		public event EventHandler<EventArgs> GotSync;
		
		/// <summary>
		/// The result, if any, we obtained: the IP address of the Android we should sync with.
		/// </summary>
		public static string AndroidIpAddress { get; set; }

		public LogBox ProgressBox { get; private set; }
		public AndroidSyncDialog()
		{
			InitializeComponent();
			// This works around a weird behavior of BetterLinkLabel, where the appearance of
			// IsTextSelectable = false is achieved by making enabled false. But we want the user
			// to be able to click the link! Since the purpose of the "Better" label is to handle
			// multi-line and we don't need that, if a link like this becomes permanent (e.g., a
			// simple link to HTA on Play Store), consider using an ordinary LinkLabel.
			_playStoreLinkLabel.Enabled = true;

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
		}

		/// <summary>
		/// Set the IP address (on the wireless network) of this computer.
		/// This will be displayed as a QR code for the Android to read.
		/// </summary>
		/// <param name="ipAddress"></param>
		public void SetOurIpAddress(IPAddress ipAddress)
		{
			if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
				throw new ArgumentException("Only IPv4 IP addresses may be used for Android sync.");

			_ourIpAddress = ipAddress;
			var writer = new BarcodeWriter
			{
				Format = BarcodeFormat.QR_CODE,
				Options =
				{
					Height = qrBox.Height,
					Width = qrBox.Width
				}
			};
			var matrix = writer.Write(ipAddress.ToString());
			var qrBitmap = new Bitmap(matrix);
			qrBox.Image = qrBitmap;
		}

		public void ShowAndroidIpAddress()
		{
			if (AndroidIpAddress != null)
			{
				_ipAddressBox.Text = AndroidIpAddress;
			}
			else if (_ourIpAddress != null)
			{
				// We expect it to be on the same network, so the first three groups should be the same
				var bytes = _ourIpAddress.GetAddressBytes(); // e.g., [192, 168, 1, 10]
				if (bytes.Length == 4)
				{
					_ipAddressBox.Text =
						$@"{bytes[0]}.{bytes[1]}.{bytes[2]}.{_lastOctetPlaceholder}";
				}
			}
		}
		public void HandleStringsLocalized()
		{
			Text = Format(Text, Program.kAndroidAppName);
			_lblAboutHearThisAndroid.Text = Format(_lblAboutHearThisAndroid.Text,
				Program.kAndroidAppName, Program.kProduct);
			_lblSyncInstructions.Text = Format(_lblSyncInstructions.Text,
				Program.kAndroidAppName);
			_playStoreLinkLabel.Text = Format(_playStoreLinkLabel.Text, Program.kAndroidAppName);
			_lblAltIP.Text = Format(_lblAltIP.Text, Program.kAndroidAppName, _syncButton.Text);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			_listener = new UDPListener();
			_listener.NewMessageReceived += (sender, args) =>
			{
				AndroidIpAddress = Encoding.UTF8.GetString(args.Data);
				Invoke(new Action(HandleGotIPAddress));
			};
		}

		protected override void OnClosed(EventArgs e)
		{
			try
			{
				_listener.StopListener(); // currently throws an exception in the code that is waiting for a packet.
			}
			catch (Exception exception)
			{
				// See HT-372
				Logger.WriteError(exception);
			}
			base.OnClosed(e);
		}

		/// <summary>
		/// Invoked by the listener when we receive an IP address from the Android.
		/// Hides the QR code, which has served its purpose, and shows a LogBox to report
		/// progress of the sync.
		/// </summary>
		private void HandleGotIPAddress()
		{
			qrBox.Hide();
			ProgressBox = new LogBox() ;
			int progressMargin = 10;
			ProgressBox.Location = new Point(progressMargin, qrBox.Top);
			ProgressBox.Size = new Size(DisplayRectangle.Width - progressMargin * 2,
				_btnClose.Top - ProgressBox.Top - progressMargin);
			ProgressBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
			Controls.Add(ProgressBox);
			_ipAddressBox.Hide();
			_syncButton.Hide();
			_lblAltIP.Hide();
			ProgressBox.Show();
			GotSync?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Helper class to listen for a single packet from the Android. Construct an instance to start
		/// listening (on another thread); hook NewMessageReceived to receive a single packet.
		/// </summary>
		private class UDPListener
		{
			private const int kPortToListen = 11007; // must match HearThisAndroid SyncActivity.desktopPort
			public event EventHandler<UDPListenerMessageArgs> NewMessageReceived;
			UdpClient _listener;
			private bool _listening;

			//constructor: starts listening.
			public UDPListener()
			{
				var listeningThread = new Thread(ListenForUDPPackages) { IsBackground = true };
				listeningThread.Start();
				_listening = true;
			}

			/// <summary>
			/// Run on a background thread; returns only when done listening.
			/// </summary>
			private void ListenForUDPPackages()
			{
				try
				{
					_listener = new UdpClient(kPortToListen);
				}
				catch (SocketException)
				{
					// Do nothing
				}

				if (_listener != null)
				{
					var groupEndPt = new IPEndPoint(IPAddress.Any, 0);

					try
					{
						// Wait for packet from Android.
						byte[] bytes = _listener.Receive(ref groupEndPt);

						//raise event
						NewMessageReceived?.Invoke(this, new UDPListenerMessageArgs(bytes));
						_listening = false;
						_listener.Close();
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
					}
				}

			}
			public void StopListener()
			{
				if (_listening)
				{
					_listening = false;
					_listener.Close(); // forcibly end communication
				}
			}
		}

		/// <summary>
		/// Helper class to hold the data we got from the Android, for the NewMessageReceived
		/// event of UDPListener
		/// </summary>
		private class UDPListenerMessageArgs : EventArgs
		{
			public byte[] Data { get; }

			public UDPListenerMessageArgs(byte[] newData)
			{
				Data = newData;
			}
		}

		private void _syncButton_Click(object sender, EventArgs e)
		{
			AndroidIpAddress = _ipAddressBox.Text;

			string GetProblemCaption() =>
				Format(LocalizationManager.GetString("AndroidSynchronization.SyncProblemCaption",
					"{0} Sync Problem",
					"MessageBox caption; Param is \"HearThis\" (product name)"),
					Program.kProduct);
			
			if (AndroidIpAddress.Contains("?"))
			{
				MessageBox.Show(
					LocalizationManager.GetString("AndroidSynchronization.ReplaceQuestionMarks",
					"To sync manually, complete the IP address in the address box using the " +
					"number shown on the Android device."),
					GetProblemCaption());
				return;
			}

			if (!IPAddress.TryParse(AndroidIpAddress, out var ipAddress) ||
			    ipAddress.AddressFamily != AddressFamily.InterNetwork)
			{
				MessageBox.Show(
					Format(LocalizationManager.GetString("AndroidSynchronization.InvalidIpAddress",
						"The value in the address box does not appear to be a valid ({0}) " +
						"device address.", "\"IPv4\""), "IPv4"),
					GetProblemCaption());
				return;
			}

			if (!IsPlausibleIPAddress(ipAddress))
			{
				var result = MessageBox.Show(
					LocalizationManager.GetString("AndroidSynchronization.DifferentNetwork",
						"The device address you entered appears to be on a different local network. This usually " +
						"won't work. Do you want to try anyway?"),
					LocalizationManager.GetString("AndroidSynchronization.WarningCaption",
						"Warning"), YesNo);
				if (result == DialogResult.No)
				{
					return;
				}
			}
			_listener.StopListener();
			HandleGotIPAddress();
		}

		private bool IsPlausibleIPAddress(IPAddress ipAddress)
		{
			var ourBytes = _ourIpAddress.GetAddressBytes();
			var otherBytes = ipAddress.GetAddressBytes();

			// Only compare IPv4 addresses (caller should have already validated)
			if (ourBytes.Length != 4 || otherBytes.Length != 4)
				return false;

			// Compare first 3 bytes (prefix)
			for (int i = 0; i < 3; i++)
			{
				if (ourBytes[i] != otherBytes[i])
					return false;
			}
			return true;
		}

		private void _ipAddressBox_Enter(object sender, EventArgs e)
		{
			var regex = new Regex(@"^(?<firstThreeOctets>(?:\d{1,3}\.){3})(?:\?{3})$");
			var match = regex.Match(_ipAddressBox.Text);
			if (match.Success)
			{
				_uneditedValueInSelectedIPAddressBox = _ipAddressBox.Text =
					match.Groups["firstThreeOctets"].Value;
				this.SafeInvoke(() => { _ipAddressBox.Select(_ipAddressBox.Text.Length, 0); },
					nameof(_ipAddressBox_Enter), IgnoreIfDisposed);
			}
		}

		private void _ipAddressBox_Leave(object sender, EventArgs e)
		{
			if (_uneditedValueInSelectedIPAddressBox == _ipAddressBox.Text)
				_ipAddressBox.Text += _lastOctetPlaceholder; // restore the placeholder
		}
	}
}
