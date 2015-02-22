//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Lib.Cryptography
{
	/// <summary>
	/// Description of SpellEncryption.
	/// </summary>
	public class SpellEncryption
	{
		public static void Decrypt(byte[] packet, uint EntityUID, out ushort SkillId, out ushort TargetX, out ushort TargetY, out uint TargetUID)
		{
			SkillId = Convert.ToUInt16(((long)packet[24] & 0xFF) | (((long)packet[25] & 0xFF) << 8));
			SkillId ^= (ushort)0x915d;
			SkillId ^= (ushort)EntityUID;
			SkillId = (ushort)(SkillId << 0x3 | SkillId >> 0xd);
			SkillId -= 0xeb42;

			TargetUID = ((uint)packet[12] & 0xFF) | (((uint)packet[13] & 0xFF) << 8) | (((uint)packet[14] & 0xFF) << 16) | (((uint)packet[15] & 0xFF) << 24);
			TargetUID = ((((TargetUID & 0xffffe000) >> 13) | ((TargetUID & 0x1fff) << 19)) ^ 0x5F2D2463 ^ EntityUID) - 0x746F4AE6;

			TargetX = 0;
			TargetY = 0;
			long xx = (packet[16] & 0xFF) | ((packet[17] & 0xFF) << 8);
			long yy = (packet[18] & 0xFF) | ((packet[19] & 0xFF) << 8);
			xx = xx ^ (EntityUID & 0xffff) ^ 0x2ed6;
			xx = ((xx << 1) | ((xx & 0x8000) >> 15)) & 0xffff;
			xx |= 0xffff0000;
			xx -= 0xffff22ee;
			yy = yy ^ (EntityUID & 0xffff) ^ 0xb99b;
			yy = ((yy << 5) | ((yy & 0xF800) >> 11)) & 0xffff;
			yy |= 0xffff0000;
			yy -= 0xffff8922;
			TargetX = Convert.ToUInt16(xx);
			TargetY = Convert.ToUInt16(yy);
		}
	}
}
