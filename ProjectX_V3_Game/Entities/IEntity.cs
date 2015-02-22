//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// The base interface for entities
	/// </summary>
	public interface IEntity
	{
		uint EntityUID { get; set; }
		string Name { get; set; }
		
		int HP { get; set; }
		int MP { get; set; }
		int MaxHP { get; set; }
		int MaxMP { get; set; }
		
		ushort Strength { get; set; }
		ushort Agility { get; set; }
		ushort Vitality { get; set; }
		ushort Spirit { get; set; }
		
		bool Alive { get; set; }
		
		byte Level { get; set; }
		
		byte Reborns { get; set; }
		
		Enums.Class Class { get; set; }
		
		BaseEntity BaseEntity { get; }
		
		Core.Screen Screen { get; }
		
		Maps.Map Map { get; set; }
		ushort X { get; set; }
		ushort Y { get; set; }
		
		ulong StatusFlag1 { get; set; }
	}
}
