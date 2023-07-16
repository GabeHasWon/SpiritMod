using System.Linq;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace SpiritMod.Mechanics.SportSystem;

internal static class SportsSyncing
{
	internal static void HandlePacket(SportMessageType sportMessageType, BinaryReader reader)
	{
		switch (sportMessageType)
		{
			case SportMessageType.PlaceCourt:
				short x = reader.ReadInt16();
				short y = reader.ReadInt16();
				string fullName = reader.ReadString();

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Sports, 4);
					packet.Write((byte)SportMessageType.PlaceCourt);
					packet.Write(x);
					packet.Write(y);
					packet.Write(fullName);
					packet.Send(-1, reader.ReadByte());
				}

				SportCourts.TryAddCourt(new Point(x, y), Activator.CreateInstance(Type.GetType(fullName)) as CourtGameTracker);
				break;
			default:
				throw new Exception("Uh oh! How'd you get here? Invalid sports message type!");
		}
	}
}
