using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZXing;
using ZXing.QrCode;

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
		private UDPListener m_listener;
		public AndroidSyncDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// The result, if any, we obtained: the IP address of the Android we should sync with.
		/// </summary>
		public string AndroidIpAddress { get; set; }

		/// <summary>
		/// Set the IP address (on the wireless network) of this computer.
		/// This will be displayed as a QR code for the Android to read.
		/// </summary>
		/// <param name="content"></param>
		public void SetOurIpAddress(string content)
		{
			var writer = new BarcodeWriter() {Format = BarcodeFormat.QR_CODE};
			writer.Options.Height = qrBox.Height;
			writer.Options.Width = qrBox.Width;
			var matrix = writer.Write(content);
			var qrBitmap = new Bitmap(matrix);
			qrBox.Image = qrBitmap;

		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			m_listener = new UDPListener();
			m_listener.NewMessageReceived += (sender, args) =>
			{
				AndroidIpAddress = Encoding.UTF8.GetString(args.data);
				this.Invoke(new Action(() => CloseWithOk()));
			};
		}

		protected override void OnClosed(EventArgs e)
		{
			m_listener.StopListener(); // currently throws an exception in the code that is waiting for a packet.
			base.OnClosed(e);
		}

		/// <summary>
		/// Invoked by the listener when we receive an IP address from the Android.
		/// </summary>
		private void CloseWithOk()
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>
		/// Helper class to listen for a single packet from the Android. Construct an instance to start
		/// listening (on another thread); hook NewMessageReceived to receive a single packet.
		/// </summary>
		class UDPListener
		{
			private int m_portToListen = 11007; // must match HearThisAndroid SyncActivity.desktopPort
			Thread m_ListeningThread;
			public event EventHandler<MyMessageArgs> NewMessageReceived;
			UdpClient m_listener = null;
			private bool _listening;

			//constructor: starts listening.
			public UDPListener()
			{
				m_ListeningThread = new Thread(ListenForUDPPackages);
				m_ListeningThread.IsBackground = true;
				m_ListeningThread.Start();
				_listening = true;
			}

			/// <summary>
			/// Run on a background thread; returns only when done listening.
			/// </summary>
			public void ListenForUDPPackages()
			{
				try
				{
					m_listener = new UdpClient(m_portToListen);
				}
				catch (SocketException)
				{
					//do nothing
				}

				if (m_listener != null)
				{
					IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 0);

					try
					{
						byte[] bytes = m_listener.Receive(ref groupEP); // waits for packet from Android.

						//raise event
						NewMessageReceived(this, new MyMessageArgs(bytes));
						_listening = false;
						m_listener.Close();
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
					m_listener.Close(); // forcibly end communication
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
	}
}
