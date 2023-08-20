using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.SportSystem;

internal class SportsPlayer : ModPlayer //funny name for a class ain't it
{
	public override void OnEnterWorld()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Sports, 5);
			packet.Write((byte)SportMessageType.RequestCourtsFromServer);
			packet.Write((byte)Player.whoAmI);
			packet.Send();
		}
	}
}
