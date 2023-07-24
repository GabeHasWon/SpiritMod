using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;

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

				if (SportCourts.TryAddCourt(new Point(x, y), Activator.CreateInstance(Type.GetType(fullName)) as CourtGameTracker))
					ModContent.GetInstance<SpiritMod>().Logger.Debug($"Recieved {fullName} court at {x},{y}.");
				else
					ModContent.GetInstance<SpiritMod>().Logger.Debug($"Failed to recieve {fullName} court at {x},{y}.");

				break;
			case SportMessageType.RequestCourtsFromServer:
				if (Main.netMode == NetmodeID.Server)
				{
					byte whoAmI = reader.ReadByte();

					foreach (var court in ModContent.GetInstance<SportCourts>().courts)
					{
						RemoteClient.CheckSection(whoAmI, court.center.ToWorldCoordinates());

						string name = court.tracker.GetType().FullName;

						ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Sports, 4);
						packet.Write((byte)SportMessageType.PlaceCourt);
						packet.Write((short)court.center.X);
						packet.Write((short)court.center.Y + 1);
						packet.Write(name);
						packet.Send(whoAmI, -1);

						SpiritMod.Instance.Logger.Debug($"Sending court {name} to client {whoAmI}.");
					}
				}
				else
					throw null;

				break;
			default:
				throw new Exception("Uh oh! How'd you get here? Invalid sports message type!");
		}
	}

	public static void PlaceOrSyncCourt<T>(byte whoAmI, Point16 loc) where T : CourtGameTracker => PlaceOrSyncCourt(whoAmI, loc, typeof(T).FullName);

	public static void PlaceOrSyncCourt(byte whoAmI, Point16 loc, string trackerName)
	{
		ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Sports, 5);
		packet.Write((byte)SportMessageType.PlaceCourt);
		packet.Write(loc.X);
		packet.Write(loc.Y);
		packet.Write(trackerName);
		packet.Write(whoAmI);
		packet.Send();
	}
}
