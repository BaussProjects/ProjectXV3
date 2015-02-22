//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of Travel.
	/// </summary>
	public class Travel
	{
		public static void BeginTravel(Entities.GameClient client, ushort mapid, ushort x, ushort y, int secs = 40)
		{
			uint dynamicid;
			Core.Kernel.Maps.selectorCollection1[999].CreateDynamic(out dynamicid);
			client.TeleportDynamic(dynamicid, 70, 60);
			
			int travelTime = (secs * 1000);
			
			/*for (int i = 0; i < (travelTime / 1000); i++)
			{
				ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() =>
				                                                       {
				                                                       	using (var msg = Packets.Message.MessageCore.CreateCenter(
				                                                       		string.Format("Hitting destination in {0}", ((travelTime / 1000) - i))))
				                                                       	       {
				                                                       	       	client.Send(msg);
				                                                       	       }
				                                                       	}, 1000 * i);
			}*/
			
			int j = 0;
			for (int i = secs; i > 0; i--)
			{
				int waitTime = (j * 1000);
				string message = string.Format(Core.MessageConst.TRAVEL_TIME, i);
				ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() =>
				                                                       {
				                                                       	using (var msg = Packets.Message.MessageCore.CreateCenter(
				                                                       		message))
				                                                       	{
				                                                       		client.Send(msg);
				                                                       	}
				                                                       }, waitTime);
				j++;
			}
			
			ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() =>
			                                                       {
			                                                       	using (var msg = Packets.Message.MessageCore.CreateCenter(
			                                                       		Core.MessageConst.TRAVEL_REACH))
			                                                       	{
			                                                       		client.Send(msg);
			                                                       	}
			                                                       }, travelTime);
			
			ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() =>
			                                                       {
			                                                       	client.Teleport(mapid, x, y);
			                                                       	using (var str = new Packets.StringPacket(new Packets.StringPacker("autorun_end")))
			                                                       	{
			                                                       		str.Action = Enums.StringAction.RoleEffect;
			                                                       		str.Data = client.EntityUID;
			                                                       		client.Send(str);
			                                                       	}
			                                                       }, travelTime);
		}
	}
}
