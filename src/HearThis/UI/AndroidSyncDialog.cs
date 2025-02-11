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
using System.Threading;
using System.Windows.Forms;
using SIL.Reporting;
using SIL.Windows.Forms.Progress;
using ZXing;

namespace HearThis.UI
{
	/// <summary>
	/// This dialog is responsible to obtain the IP address of the Android we want to sync with, if possible.
	/// Currently this is done by displaying a QR code representing our own IP address, which the client
	/// must set using SetOurIpAddress. The android scans this and sends a packet containing its own IP address.
	/// When this class receives that successfully, it closes (with DialogResult.OK). If instead the user
	/// closes the dialog, it will return another code, I think Cancel.
	/// Enhance JT: handle the possibility that the Android does not have a functional scanner.
	/// Display the Android's IP address on its screen, and tell the user to type it in a new text box (or 4)
	/// here, then click OK.
	/// Enhance JT: possibly the QR code could convey a BlueTooth IP address also as another option.
	/// </summary>
	public partial class AndroidSyncDialog : Form
	{
		private UDPListener _listener;

		public event EventHandler<EventArgs> GotSync;

		public LogBox ProgressBox { get; private set; }
		public AndroidSyncDialog()
		{
			InitializeComponent();
			// This works around a weird behavior of BetterLinkLabel, where the appearance of IsTextSelectable  = false
			// is achieved by making enabled false. But we want the user to be able to click the link!
			// Since the purpose of the "Better" label is to handle multi-line and we don't need that, if a link like this
			// becomes permanent (e.g., a simple link to HTA on Play Store), consider using an ordinary LinkLabel.
			playStoreLinkLabel.Enabled = true;
		}

		/// <summary>
		/// The result, if any, we obtained: the IP address of the Android we should sync with.
		/// </summary>
		public static string AndroidIpAddress { get; set; }

		private string _ourIpAddress;

		/// <summary>
		/// Set the IP address (on the wireless network) of this computer.
		/// This will be displayed as a QR code for the Android to read.
		/// </summary>
		/// <param name="content"></param>
		public void SetOurIpAddress(string content)
		{
			_ourIpAddress = content;
			var writer = new BarcodeWriter
			{
				Format = BarcodeFormat.QR_CODE,
				Options =
				{
					Height = qrBox.Height,
					Width = qrBox.Width
				}
			};
			var matrix = writer.Write(content);
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
				int index = _ourIpAddress.LastIndexOf(".", StringComparison.Ordinal);
				if (index > 0)
				{
					_ipAddressBox.Text = _ourIpAddress.Substring(0, index + 1) + "???";
				}
			}
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			_listener = new UDPListener();
			_listener.NewMessageReceived += (sender, args) =>
			{
				AndroidIpAddress = Encoding.UTF8.GetString(args.data);
				Invoke(new Action(HandleGotIpAddress));
			};
			int index = _ipAddressBox.Text.LastIndexOf(".", StringComparison.Ordinal);
			_ipAddressBox.SelectionStart = index + 1;
			_ipAddressBox.SelectionLength = 3;
			_ipAddressBox.Focus();
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
		private void HandleGotIpAddress()
		{
			qrBox.Hide();
			ProgressBox = new LogBox() ;
			int progressMargin = 10;
			ProgressBox.Location = new Point(progressMargin, qrBox.Top);
			ProgressBox.Size = new Size(this.DisplayRectangle.Width - progressMargin * 2, okButton.Top - ProgressBox.Top - progressMargin);
			ProgressBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
			Controls.Add(ProgressBox);
			_ipAddressBox.Hide();
			_syncButton.Hide();
			_altIpLabel.Hide();
			ProgressBox.Show();
			GotSync?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Helper class to listen for a single packet from the Android. Construct an instance to start
		/// listening (on another thread); hook NewMessageReceived to receive a single packet.
		/// </summary>
		class UDPListener
		{
			private int _portToListen = 11007; // must match HearThisAndroid SyncActivity.desktopPort
			public event EventHandler<MyMessageArgs> NewMessageReceived;
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
					_listener = new UdpClient(_portToListen);
				}
				catch (SocketException)
				{
					//do nothing
				}

				if (_listener != null)
				{
					IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 0);

					try
					{
						byte[] bytes = _listener.Receive(ref groupEP); // waits for packet from Android.

						//raise event
						NewMessageReceived(this, new MyMessageArgs(bytes));
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
		/// Helper class to hold the data we got from the Android, for the NewMessageReceived event of UDPListener
		/// </summary>
		class MyMessageArgs : EventArgs
		{
			public byte[] data { get; set; }

			public MyMessageArgs(byte[] newData)
			{
				data = newData;
			}
		}

		private void _syncButton_Click(object sender, EventArgs e)
		{
			AndroidIpAddress = _ipAddressBox.Text;
			if (AndroidIpAddress.Contains("?"))
			{
				MessageBox.Show(
					"You need to replace the three question marks in the box to the left with the number shown on the device in order to sync manually.",
					"HearThis Sync Problem");
				return;
			}

			if (!ValidateIpAddress())
			{
				MessageBox.Show(
					"The value in the address box does not appear to be a valid device address.",
					"HearThis Sync Problem");
				return;
			}

			if (!IsPlausibleIpAddress())
			{
				if (MessageBox.Show(
					"The device address you entered appears to be on a different local network. This usually won't work. Do you want to try anyway?",
					"Warning", MessageBoxButtons.YesNo) == DialogResult.No)
				{
					return;
				}
			}
			_listener.StopListener();
			HandleGotIpAddress();
		}

		private bool ValidateIpAddress()
		{
			var parts = AndroidIpAddress.Split('.');
			if (parts.Length != 4)
				return false;
			foreach (var part in parts)
			{
				if (!int.TryParse(part, out var val) || val < 0 || val > 255)
					return false;
			}
			return true;
		}

		private bool IsPlausibleIpAddress()
		{
			return getIpPrefix(_ourIpAddress) == getIpPrefix(AndroidIpAddress);
		}

		private string getIpPrefix(string input)
		{
			int index = input.LastIndexOf('.');
			if (index < 0)
				return "";
			return input.Substring(0, index + 1);
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
