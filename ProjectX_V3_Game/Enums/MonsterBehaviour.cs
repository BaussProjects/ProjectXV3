//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
	/// <summary>
	/// Different kinds of monster behaviours.
	/// </summary>
	public enum MonsterBehaviour
	{
		/// <summary>
		/// Normal monster behaviour. Attacks if you are in range, but does not follow out of area.
		/// </summary>
		Normal = 0,
		
		/// <summary>
		/// Aggressive monster behaviour. Attacks if you are in range, moves out of area if you do. (Max 50 x or 50 y)
		/// </summary>
		Aggresive = 1,
		
		/// <summary>
		/// Peaceful monster behaviour. Does not attack, not even if you attack.
		/// </summary>
		Peaceful = 2,
		
		/// <summary>
		/// Magic guard. Attacks with a high damage magic attack within their range.
		/// These are regular guards.
		/// </summary>
		MagicGuard = 3,
		
		/// <summary>
		/// Physical guard. Attacks with a high damage physical attack. Follows target, once dead or out of range it will teleport back.
		/// </summary>
		PhysicalGuard = 4,
		
		/// <summary>
		/// Death guard. Attacks and kills with one hit no matter gear. Attacks only in range.
		/// </summary>
		DeathGuard = 5,
		
		/// <summary>
		/// Reviver guard 1. Revives dead players in range and teleports to revive spawn if there is any.
		/// </summary>
		ReviverGuard1 = 6,
		
		/// <summary>
		/// Reviver guard 2. Revives dead players in range at their spot.
		/// </summary>
		ReviverGuard2 = 7,
	}
}
