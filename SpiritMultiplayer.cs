﻿using Microsoft.Xna.Framework;
using SpiritMod.GlobalClasses.Players;
using SpiritMod.GlobalClasses.Projectiles;
using SpiritMod.Items.Pins;
using SpiritMod.Items.Weapon.Summon.StardustBomb;
using SpiritMod.Mechanics.BoonSystem;
using SpiritMod.Mechanics.Fathomless_Chest;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.SportSystem;
using SpiritMod.Mechanics.Trails;
using SpiritMod.NPCs.AuroraStag;
using SpiritMod.NPCs.ExplosiveBarrel;
using SpiritMod.NPCs.Tides.Tide;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod;

public static class SpiritMultiplayer
{
	private struct Wait
	{
		public Func<bool> Condition { get; set; }
		public Action Result { get; set; }
	}

	private static List<Wait> _waits = new List<Wait>();

	public static void Load() => Main.OnTickForThirdPartySoftwareOnly += OnTick;

	public static void Unload()
	{
		Main.OnTickForThirdPartySoftwareOnly -= OnTick;
		_waits = null;
	}

	public static void OnTick()
	{
		if (_waits == null) 
			return;

		for (int i = 0; i < _waits.Count; i++)
		{
			Wait wait = _waits[i];
			if (wait.Condition.Invoke())
			{
				wait.Result?.Invoke();
				_waits.RemoveAt(i--);
			}
		}
	}

	public static void WaitUntil(Func<bool> condition, Action whenTrue) => _waits.Add(new Wait() { Condition = condition, Result = whenTrue });

	// This is deprecated, DO NOT USE IT. Only here for compatability until later stages when we decide to swap it out for the new one.
	[Obsolete]
	public static ModPacket WriteToPacket(ModPacket packet, byte msg, params object[] param)
	{
		packet.Write(msg);

		for (int m = 0; m < param.Length; m++)
		{
			object obj = param[m];
			if (obj is bool) 
				packet.Write((bool)obj);
			else if (obj is byte) 
				packet.Write((byte)obj);
			else if (obj is int) 
				packet.Write((int)obj);
			else if (obj is float) 
				packet.Write((float)obj);
			else if (obj is double) 
				packet.Write((double)obj);
			else if (obj is short) 
				packet.Write((short)obj);
			else if (obj is ushort) 
				packet.Write((ushort)obj);
			else if (obj is sbyte) 
				packet.Write((sbyte)obj);
			else if (obj is uint) 
				packet.Write((uint)obj);
			else if (obj is decimal) 
				packet.Write((decimal)obj);
			else if (obj is long) 
				packet.Write((long)obj);
			else if (obj is string) 
				packet.Write((string)obj);
		}
		return packet;
	}

	public static ModPacket WriteToPacket(int capacity, MessageType type)
	{
		ModPacket packet = SpiritMod.Instance.GetPacket(capacity);
		packet.Write((byte)type);
		return packet;
	}

	public static ModPacket WriteToPacket(int capacity, MessageType type, Action<ModPacket> action)
	{
		ModPacket packet = SpiritMod.Instance.GetPacket(capacity);
		packet.Write((byte)type);
		action?.Invoke(packet);
		return packet;
	}

	public static ModPacket WriteToPacketAndSend(int capacity, MessageType type, Action<ModPacket> beforeSend)
	{
		var packet = WriteToPacket(capacity, type, beforeSend);
		packet.Send();
		return packet;
	}

	public static void HandlePacket(BinaryReader reader, int whoAmI)
	{
		var id = (MessageType)reader.ReadByte();
		byte player;
		int proj;
		byte glyph;

		switch (id)
		{
			case MessageType.AuroraData:
				MyWorld.auroraType = reader.ReadInt32();
				break;
			case MessageType.ProjGlyph:
				proj = reader.ReadInt32();
				glyph = reader.ReadByte();
				byte rarity = reader.ReadByte();
				int parent = reader.ReadInt32();
				int pType = reader.ReadInt32();

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.ProjGlyph, 5);
					packet.Write(proj);
					packet.Write(glyph);
					packet.Write(rarity);
					packet.Write(parent);
					packet.Write(pType);
					packet.Send(ignoreClient: whoAmI);
				}
				if (Main.projectile[proj] is Projectile projectile && projectile.TryGetGlobalProjectile(out GlyphGlobalProjectile gProj))
				{
					gProj.Glyph = (GlyphType)glyph;
					gProj.rarity = rarity;
					gProj.parentData = new(parent, pType);
				}
				break;
			case MessageType.PlaceMapPin:
				int cursorX = reader.ReadInt32();
				int cursorY = reader.ReadInt32();
				string pinValue = reader.ReadString();

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.PlaceMapPin, 3);
					packet.Write(cursorX);
					packet.Write(cursorY);
					packet.Write(pinValue);
					packet.Send(-1, whoAmI);
				}

				ModContent.GetInstance<PinWorld>().SetPin(pinValue, new Vector2(cursorX, cursorY));
				break;
			case MessageType.Dodge:
				player = reader.ReadByte();
				byte type = reader.ReadByte();
				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Dodge, 2);
					packet.Write(player);
					packet.Write(type);
					packet.Send(-1, whoAmI);
				}
				if (type != 1)
					SpiritMod.Instance.Logger.Error("Unknown message (2:" + type + ")");
				break;
			case MessageType.Dash:
				player = reader.ReadByte();
				DashType dash = (DashType)reader.ReadByte();
				sbyte dir = reader.ReadSByte();
				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Dash, 3);
					packet.Write(player);
					packet.Write((byte)dash);
					packet.Write(dir);
					packet.Send(-1, whoAmI);
				}
				Main.player[player].GetModPlayer<DashPlayer>().PerformDash(dash, dir, false);
				break;
			case MessageType.PlayerGlyph:
				player = reader.ReadByte();
				glyph = reader.ReadByte();

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.PlayerGlyph, 2);
					packet.Write(player);
					packet.Write((byte)glyph);
					packet.Send(-1, whoAmI);
				}
				if (player == Main.myPlayer)
					break;
				Main.player[player].GetModPlayer<GlyphPlayer>().Glyph = (GlyphType)glyph;
				break;
			case MessageType.BossSpawnFromClient:
				if (Main.netMode == NetmodeID.Server)
				{
					player = reader.ReadByte();
					int netNPCType = reader.ReadInt32();
					int npcCenterX = reader.ReadInt32();
					int npcCenterY = reader.ReadInt32();

					if (NPC.AnyNPCs(netNPCType))
						return;

					int npcID = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), npcCenterX, npcCenterY, netNPCType);
					Main.npc[npcID].netUpdate = true;
					Main.npc[npcID].netUpdate2 = true;

					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[npcID].GetTypeNetName()), new Color(175, 75, 255));
				}
				break;
			case MessageType.SpawnNPCFromClient:
				if (Main.netMode == NetmodeID.Server)
				{
					int npcIndex = reader.ReadInt32();
					int npcCenterX = reader.ReadInt32();
					int npcCenterY = reader.ReadInt32();

					int npcID = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), npcCenterX, npcCenterY, npcIndex);
					Main.npc[npcID].netUpdate2 = true;
				}
				break;
			case MessageType.StartTide:
				TideWorld.TheTide = true;
				TideWorld.TideWaveIncrease();
				break;
			case MessageType.TideData:
				TideWorld.HandlePacket(reader);
				break;
			case MessageType.TameAuroraStag:
				int stagWhoAmI = reader.ReadInt32();

				if (Main.netMode == NetmodeID.Server)
					WriteToPacket(SpiritMod.Instance.GetPacket(4), (byte)MessageType.TameAuroraStag, stagWhoAmI).Send();

				(Main.npc[stagWhoAmI].ModNPC as AuroraStag).TameAnimationTimer = AuroraStag.TameAnimationLength;
				break;
			case MessageType.SpawnTrail:
				proj = reader.ReadInt32();

				if (Main.netMode == NetmodeID.Server)
				{ 
					//If received by the server, send to all clients instead
					WriteToPacket(SpiritMod.Instance.GetPacket(), (byte)MessageType.SpawnTrail, proj).Send();
					break;
				}

				if (Main.projectile[proj].ModProjectile is IManualTrailProjectile trailProj)
					trailProj.DoTrailCreation(SpiritMod.TrailManager);
				break;
			case MessageType.PlaceSuperSunFlower:
				MyWorld.superSunFlowerPositions.Add(new Point16(reader.ReadUInt16(), reader.ReadUInt16()));
				break;
			case MessageType.DestroySuperSunFlower:
				MyWorld.superSunFlowerPositions.Remove(new Point16(reader.ReadUInt16(), reader.ReadUInt16()));
				break;
			case MessageType.SpawnExplosiveBarrel: // this packet is only meant to be received by the server
				(int x, int y) = (reader.ReadInt32(), reader.ReadInt32());
				NPC.NewNPC(new EntitySource_TileBreak(x / 16, y / 16), x, y, ModContent.NPCType<ExplosiveBarrel>(), 0, 2, 1, 0, 0); // gets forwarded to all clients
				break;
			case MessageType.SpawnStardustBomb:
				if (Main.netMode == NetmodeID.Server)
				{
					player = reader.ReadByte();
					Vector2 velocity = reader.ReadVector2();

					int npcID = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), (int)Main.player[player].Center.X, (int)Main.player[player].Center.Y + 100, ModContent.NPCType<StardustBombNPC>(), 0, player);
					Main.npc[npcID].velocity = velocity;
					Main.npc[npcID].netUpdate2 = true;
				}
				break;
			case MessageType.StarjinxData:
				//TBD in future
				break;
			case MessageType.BoonData:
				ushort npcType = reader.ReadUInt16();
				ushort index = reader.ReadUInt16();
				byte boonType = reader.ReadByte();
				Boon ret = Activator.CreateInstance(BoonLoader.LoadedBoonTypes[boonType]) as Boon;
				ret.npc = Main.npc[index];
				SpiritMod.Instance.Logger.Debug($"received new boon data, index: {index} boonType: {boonType} which is {BoonLoader.LoadedBoonTypes[boonType].Name}");
				// wait until the npc at the specified index becomes the type we expect it to be, then set the boons up
				WaitUntil(() => Main.npc[index].type == npcType, () =>
				{
					Main.npc[index].GetGlobalNPC<BoonNPC>().currentBoon = ret;
					Main.npc[index].GetGlobalNPC<BoonNPC>().currentBoon.SpawnIn();
					Main.npc[index].GetGlobalNPC<BoonNPC>().currentBoon.SetStats();
					SpiritMod.Instance.Logger.Debug($"current boon is now: {Main.npc[index].GetGlobalNPC<BoonNPC>().currentBoon.GetType().Name}");
				});
				break;
			case MessageType.FathomlessData:
				byte effectIndex = reader.ReadByte();
				byte playerIndex = reader.ReadByte();
				(ushort i, ushort j) = (reader.ReadUInt16(), reader.ReadUInt16());

				if (Main.netMode == NetmodeID.Server) //If the server recieves the packet, send to other clients
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.FathomlessData, 4);
					packet.Write(effectIndex);
					packet.Write(playerIndex);
					packet.Write(i);
					packet.Write(j);
					packet.Send(-1, whoAmI);
				}

				ChanceEffectManager.effectIndex[effectIndex].Trigger(Main.player[playerIndex], new Point16(i, j));
				break;
			case MessageType.PlayDeathSoundFromServer:
				index = reader.ReadUInt16();
				string deathSound = reader.ReadString();

				if (Main.netMode == NetmodeID.MultiplayerClient)
					Main.npc[index].PlayDeathSound(deathSound);
				break;
			case MessageType.RequestQuestManager:
				player = reader.ReadByte();
				QuestManager.SyncToClient(player);
				break;
			case MessageType.RecieveQuestManager:
				QuestPlayer.RecieveManager(reader);
				ModContent.GetInstance<SpiritMod>().Logger.Debug("Recieved manager.");
				break;
			case MessageType.Quest:
				QuestMultiplayer.HandlePacket(reader, (QuestMessageType)reader.ReadByte(), reader.ReadBoolean());
				break;
			case MessageType.SyncLuminousOcean:
				byte luminousType = reader.ReadByte();
				bool isActive = reader.ReadBoolean();

				MyWorld.luminousType = luminousType;
				MyWorld.luminousOcean = isActive;

				if (Main.netMode == NetmodeID.Server) //If the server recieves the packet, send to other clients
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.SyncLuminousOcean, 2);
					packet.Write(luminousType);
					packet.Write(isActive);
					packet.Send(-1, whoAmI);
				}

				break;
			case MessageType.Sports:
				SportsSyncing.HandlePacket((SportMessageType)reader.ReadByte(), reader);
				break;

			case MessageType.SearchForFathomless:
				if (Main.netMode == NetmodeID.Server)
				{
					int from = reader.ReadByte();
					int invSlot = reader.ReadByte();
					bool successFromServer = Mystical_Dice.FindFathomlessChest(Main.player[from], Main.player[from].inventory[invSlot]);

					if (!successFromServer)
					{
						ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.SearchForFathomlessFailure, 0);
						packet.Send(from);
					}
				}

				break;

			case MessageType.SearchForFathomlessFailure:
				Mystical_Dice.FailFind(Main.LocalPlayer, Main.LocalPlayer.HeldItem);
				break;

			default:
				SpiritMod.Instance.Logger.Error("Unknown net message (" + id + ")");
				break;
		}
	}

	public static void SpawnBossFromClient(byte whoAmI, int type, int x, int y) => SpiritMod.WriteToPacket(SpiritMod.Instance.GetPacket(), (byte)MessageType.BossSpawnFromClient, whoAmI, type, x, y).Send();
}
