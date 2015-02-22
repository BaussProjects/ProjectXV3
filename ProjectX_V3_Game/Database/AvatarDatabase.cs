//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Generic;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Database handler for avatars.
	/// </summary>
	public class AvatarDatabase
	{
		/// <summary>
		/// The male avatar holder.
		/// </summary>
		private static ushort[] MaleAvatars;
		
		/// <summary>
		/// The female avatar holder.
		/// </summary>
		private static ushort[] FemaleAvatars;
		
		/// <summary>
		/// Loads all the avatar id's.
		/// </summary>
		public static void LoadAvatars()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading avatars...");
			#region maleavatars
			List<ushort> avatars = new List<ushort>();
			
			for (ushort i = 1; i <= 107; i++)
				avatars.Add(i);
			for (ushort i = 109; i <= 113; i++)
				avatars.Add(i);
			for (ushort i = 129; i <= 153; i++)
				avatars.Add(i);
			
			MaleAvatars = avatars.ToArray();
			avatars.Clear();
			#endregion
			
			#region femaleavatars
			avatars = new List<ushort>();
			
			for (ushort i = 201; i <= 295; i++)
				avatars.Add(i);
			for (ushort i = 300; i <= 304; i++)
				avatars.Add(i);
			for (ushort i = 320; i <= 344; i++)
				avatars.Add(i);
			avatars.Add(2511);
			
			FemaleAvatars = avatars.ToArray();
			avatars.Clear();
			#endregion
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} avatars ({1} male avatars & {2} female avatars)...", (MaleAvatars.Length + FemaleAvatars.Length), MaleAvatars.Length, FemaleAvatars.Length);
		}
		
		/// <summary>
		/// Generates a random avatar. [Used at character creation.]
		/// </summary>
		/// <param name="female">Set to true if female, false if male.</param>
		/// <returns>Returns the random generated avatar.</returns>
		public static ushort GenerateAvatar(bool female)
		{
			if (female)
			{
				int gen = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(FemaleAvatars.Length);
				return FemaleAvatars[gen];
			}
			else
			{
				int gen = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(MaleAvatars.Length);
				return MaleAvatars[gen];
			}
		}
	}
}
