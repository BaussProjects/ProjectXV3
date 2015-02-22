//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.IO;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// The database for revive spots.
	/// </summary>
	public class ReviveDatabase
	{
		/// <summary>
		/// Loads all the revive spots.
		/// </summary>
		/// <returns>Returns true if the revive spots were loaded.</returns>
		public static bool LoadReviveSpots()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Revive Spots...");
			using (var revive = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(revive, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_ReviveSpots");
				}
				while (revive.Read())
				{
					Maps.MapPoint revivepoint = new ProjectX_V3_Game.Maps.MapPoint(revive.ReadUInt16("ReviveTargetMapID"), revive.ReadUInt16("ReviveX"), revive.ReadUInt16("ReviveY"));

					ushort frommap = revive.ReadUInt16("ReviveMapID");
					if (!Core.Kernel.Maps.Contains(frommap))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load revive spots. Failed at map: {0}, perhaps the map is not existing.", revivepoint.MapID);
						Console.ResetColor();
						return false;
					}
					Core.Kernel.Maps[frommap].RevivePoint = revivepoint;
				}
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded Revive Spots...");
			return true;
		}
	}
}
