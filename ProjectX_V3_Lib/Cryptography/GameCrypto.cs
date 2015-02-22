//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Lib.Cryptography
{
	/// <summary>
	/// The cryptographer used for the game server.
	/// </summary>
	public class GameCrypto : ConquerCrypto
	{
		BlowfishCryptography _blowfish;
		
		internal bool Exchanged = false;
		
		public GameCrypto(byte[] key)
		{
			DHKeySequence = new DHKeyExchange.ServerKeyExchange();
			_blowfish = new BlowfishCryptography(BlowfishAlgorithm.CFB64);
			lock(key)
				_blowfish.SetKey(key);
		}
		
		public override void Encrypt(byte[] buffer)
		{
			//lock (buffer)
			//{
				byte[] encrypted = _blowfish.Encrypt(buffer);
				//lock (buffer)
				encrypted.memcpy(buffer);
			//}
		}
		
		public override void Decrypt(byte[] buffer)
		{
			//lock (buffer)
			//{
				byte[] decrypted = _blowfish.Decrypt(buffer);
				//lock (buffer)
				decrypted.memcpy(buffer);
			//}
		}
		
		public void SetKey(byte[] k)
		{
			_blowfish.SetKey(k);
		}
		
		public void SetIvs(byte[] i1, byte[] i2)
		{
			_blowfish.EncryptIV = i1;
			_blowfish.DecryptIV = i2;
		}
		
		private DHKeyExchange.ServerKeyExchange DHKeySequence;
		
		public ProjectX_V3_Lib.Network.DataPacket GetExchangePacket()
		{
			return new ProjectX_V3_Lib.Network.DataPacket(DHKeySequence.CreateServerKeyPacket());
		}
		
		public static bool HandleExchange(GameCrypto Crypto, ProjectX_V3_Lib.Network.SocketClient sClient, ProjectX_V3_Lib.Network.DataPacket Packet)
		{
			try
			{
				byte[] packet = Packet.Copy();
				
				ushort position = 7;
				uint PacketLen = BitConverter.ToUInt32(packet, position);
				position += 4;
				int JunkLen = BitConverter.ToInt32(packet, position);
				position += 4;
				position += (ushort)JunkLen;
				int Len = BitConverter.ToInt32(packet, position);
				position += 4;
				byte[] pubKey = new byte[Len];
				for (int x = 0; x < Len; x++)
					pubKey[x] = packet[x + position];
				string PubKey = pubKey.GetString(); // System.Text.ASCIIEncoding.ASCII.GetString(pubKey);
				Crypto = Crypto.DHKeySequence.HandleClientKeyPacket(PubKey, Crypto);
				Crypto.Exchanged = true;
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
