using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace HearThis.Communication
{
	class BluetoothLink : IAndroidLink
	{
		private BluetoothClient _client;
		private BluetoothAddress _address;
		private Stream _connectStream;
		private readonly Guid _bluetoothServiceId = Guid.Parse("bd17ec40-1475-4f66-a661-4c4a0a65e92d");

		public BluetoothLink()
		{
			_bluetoothServiceId = Guid.Parse("bd17ec40-1475-4f66-a661-4c4a0a65e92d");
			//if (BitConverter.IsLittleEndian)
			//{
			//	// Guid.Parse will have incorrectly made a little-endian guid. But we need a binary match
			//	// on a standard UUID, where Java parses the above string in the standard way using
			//	// big-endian numbers throughout, MS uses native-endian for three of the parts.
			//	// Correct for this if needed. See https://en.wikipedia.org/wiki/Globally_unique_identifier
			//	// Conversion taken from http://stackoverflow.com/questions/5745512/how-to-read-a-net-guid-into-a-java-uuid
			//	_bluetoothServiceId = ToBigEndian(_bluetoothServiceId);
			//}
		}

		/// <summary>
		/// Converts little-endian .NET guids to big-endian Java guids:
		/// </summary>
		[CLSCompliant(true)]
		public static Guid ToBigEndian(Guid netGuid)
		{
			byte[] java = new byte[16];
			byte[] net = netGuid.ToByteArray();
			for (int i = 8; i < 16; i++)
			{
				java[i] = net[i];
			}
			java[0] = net[3];
			java[1] = net[2];
			java[2] = net[1];
			java[3] = net[0];
			java[4] = net[5];
			java[5] = net[4];
			java[6] = net[7];
			java[7] = net[6];
			return new Guid(java);
		}

		public bool InitBluetooth()
		{
			var radio = BluetoothRadio.PrimaryRadio;
			if (radio == null)
				return false;
			if (radio.LocalAddress == null)
				return false;
			var client = new BluetoothClient();
			foreach (var device in client.DiscoverDevices())
			{
				if (!device.Authenticated)
					continue; // enhance: could try to authenticate if not paired
				try
				{
					var ep = new BluetoothEndPoint(device.DeviceAddress, _bluetoothServiceId);
					_client = new BluetoothClient(ep);
					_connectStream = _client.GetStream();
					return true; // Enhance: if we find more than one possible device, may need to ask user to choose
				}
				catch (Exception ex)
				{
					// probably some device not offering the HT Android sync service
				}
			}
			return false;
		}

		public string GetDeviceName()
		{
			throw new NotImplementedException();
		}

		public bool GetFile(string androidPath, string destPath)
		{
			PutMessage("getfile;" + androidPath);
			var content = GetResponse();
			if (content.Length == 0)
				return false;
			Directory.CreateDirectory(Path.GetDirectoryName(destPath));
			File.WriteAllBytes(destPath, content);
			return true;
		}

		void PutMessage(string message)
		{
			// Review: should we send, and have Android expect, a length first? Or a message terminator?
			var msg = Encoding.UTF8.GetBytes(message);
			_connectStream.Write(msg, 0, msg.Length);
		}

		byte[] GetResponse()
		{
			var countBytes = new byte[4];
			ReadBytes(countBytes);
			var count = BitConverter.ToInt32(countBytes,0);
			var result = new byte[count];
			ReadBytes(result);
			return result;
		}

		void ReadBytes(byte[] buffer)
		{
			// Enhance: timeout, etc.
			int gotBytes = 0;
			while (gotBytes < buffer.Length)
			{
				int readBytes = _connectStream.Read(buffer, gotBytes, buffer.Length - gotBytes);
				gotBytes += readBytes;
			}
		}

		public bool TryGetData(string androidPath, out byte[] data)
		{
			PutMessage("getfile;" + androidPath);
			data = GetResponse();
			return data.Length > 0;
		}

		public bool PutFile(string androidPath, byte[] data)
		{
			PutMessage("putfile;" + androidPath + ";" + data.Length + ";");
			_connectStream.Write(data, 0, data.Length);
			return true;
		}

		public bool TryListFiles(string androidPath, out string list)
		{
			PutMessage("listfiles;"+androidPath);
			list = Encoding.UTF8.GetString(GetResponse());
			return true;
		}

		public void DeleteFile(string androidPath)
		{
			throw new NotImplementedException();
		}

		public bool SendNotification(string message)
		{
			PutMessage("notify;" + message);
			return true;
		}
	}
}
