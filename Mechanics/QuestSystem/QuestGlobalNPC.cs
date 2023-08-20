using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using SpiritMod.Items.Accessory;
using SpiritMod.Items.Ammo.Arrow;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Mechanics.QuestSystem.Quests;
using SpiritMod.NPCs.Town;
using SpiritMod.Items.Consumable.Quest;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Utilities;
using SpiritMod.NPCs;
using SpiritMod.Items.Weapon.Magic.LuminanceSeacone;
using SpiritMod.Items.Sets.MagicMisc.Lightspire;
using SpiritMod.Tiles.Furniture.JadeDragonStatue;
using SpiritMod.Items.Equipment;
using SpiritMod.Items.Sets.DashSwordSubclass.AnimeSword;
using SpiritMod.Items.Weapon.Swung.Punching_Bag;
using SpiritMod.Tiles.Furniture.Critters;
using SpiritMod.Items.Consumable;
using SpiritMod.Items.Weapon.Thrown.CryoKnife;
using SpiritMod.Items.Placeable.IceSculpture;

namespace SpiritMod.Mechanics.QuestSystem
{
	public class QuestGlobalNPC : GlobalNPC
	{
		public static event Action<NPC> OnNPCLoot;

		public static Dictionary<int, QuestPoolData> SpawnPoolMods = new Dictionary<int, QuestPoolData>();
		public static Dictionary<int, int> PoolModsCount = new Dictionary<int, int>();

		public override void OnKill(NPC npc)
		{
			if (npc.type == NPCID.Zombie || npc.type == NPCID.BaldZombie || npc.type == NPCID.SlimedZombie || npc.type == NPCID.SwampZombie || npc.type == NPCID.TwiggyZombie || 
				npc.type == NPCID.ZombieRaincoat || npc.type == NPCID.PincushionZombie || npc.type == NPCID.ZombieEskimo)
			{
				if (!QuestManager.GetQuest<ZombieOriginQuest>().IsUnlocked && QuestManager.GetQuest<FirstAdventure>().IsCompleted && Main.rand.NextBool(40))
				{
					int slot = Item.NewItem(npc.GetSource_Death("Quest"), (int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<OccultistMap>());

					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, slot, 1f);
				}
			}

			if (Main.netMode == NetmodeID.SinglePlayer)
				ClientNPCLoot(npc);
			else if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.Quest, 4);
				packet.Write((byte)QuestMessageType.SyncOnNPCLoot);
				packet.Write(false);
				packet.Write((byte)npc.whoAmI);
				packet.Send(-1, 256);
			}
		}

		/// <summary>Contains everything needed for clients to know what NPC died without needing untoward syncing. If you need to add quest queues, do it here.</summary>
		public void ClientNPCLoot(NPC npc)
		{
			if (npc.type == NPCID.EyeofCthulhu || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.SkeletronHead || npc.type == ModContent.NPCType<NPCs.Boss.Scarabeus.Scarabeus>() || npc.type == NPCID.BrainofCthulhu ||
				npc.type == ModContent.NPCType<NPCs.Boss.AncientFlyer>() || npc.type == ModContent.NPCType<NPCs.Boss.MoonWizard.MoonWizard>() || npc.type == ModContent.NPCType<NPCs.Boss.SteamRaider.SteamRaiderHead>())
			{
				ModContent.GetInstance<QuestWorld>().AddQuestQueue(ModContent.NPCType<Adventurer>(), QuestManager.GetQuest<SlayerQuestOccultist>());
				ModContent.GetInstance<QuestWorld>().AddQuestQueue(ModContent.NPCType<Adventurer>(), QuestManager.GetQuest<UnidentifiedFloatingObjects>());
			}

			if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.BrainofCthulhu)
				ModContent.GetInstance<QuestWorld>().AddQuestQueue(ModContent.NPCType<Adventurer>(), QuestManager.GetQuest<SlayerQuestMarble>());

			if (npc.type == NPCID.SkeletronHead)
			{
				ModContent.GetInstance<QuestWorld>().AddQuestQueue(ModContent.NPCType<Adventurer>(), QuestManager.GetQuest<RaidingTheStars>());
				ModContent.GetInstance<QuestWorld>().AddQuestQueue(ModContent.NPCType<Adventurer>(), QuestManager.GetQuest<StrangeSeas>());
				ModContent.GetInstance<QuestWorld>().AddQuestQueue(ModContent.NPCType<RuneWizard>(), QuestManager.GetQuest<IceDeityQuest>());
			}

			OnNPCLoot?.Invoke(npc);
		}

		public override void ModifyShop(NPCShop shop)
		{
			if (shop.NpcType == ModContent.NPCType<RuneWizard>())
				shop.Add<OccultistMap>(Condition.TimeNight, SpiritConditions.FirstAdventureFinished);
			else if (shop.NpcType == NPCID.Merchant)
				shop.Add<GiantAnglerStatue>(SpiritConditions.FishyBusinessFinished);
			else if (shop.NpcType == NPCID.Dryad)
				shop.Add<LuminanceSeacone>(SpiritConditions.SanctuaryNightlightsFinished);
			else if (shop.NpcType == ModContent.NPCType<Adventurer>())
			{
				shop.Add<DurasilkSheaf>(SpiritConditions.FirstAdventureFinished);
				shop.Add<ExplorerScrollAsteroidFull>(SpiritConditions.SpaceRocksFinished);
				shop.Add<ExplorerScrollGraniteFull>(SpiritConditions.RockyRoadFinished);
				shop.Add<ExplorerScrollMarbleFull>(SpiritConditions.ForgottenCivilizationsFinished);
				shop.Add<ExplorerScrollHiveFull>(SpiritConditions.HiveHuntingFinished);
				shop.Add<ExplorerScrollMushroomFull>(SpiritConditions.GlowingAGardenFinished);
				shop.Add<AkaviriStaff>(SpiritConditions.ManicMageFinished);
				shop.Add<Punching_Bag>(SpiritConditions.UnholyUndertakingFinished);

				shop.Add<DragonStatueItem>(SpiritConditions.SkyHighFinished);
				shop.Add<DynastyFan>(SpiritConditions.SkyHighFinished);
				shop.Add<AnimeSword>(SpiritConditions.SkyHighFinished);

				shop.Add<SepulchreArrow>(SpiritConditions.DecrepitDepthsFinished);
				shop.Add<SepulchreBannerItem>(SpiritConditions.DecrepitDepthsFinished);
				shop.Add<SepulchreChest>(SpiritConditions.DecrepitDepthsFinished);

				shop.Add<PottedSakura>(SpiritConditions.TowerOrHideoutQuestFinished);
				shop.Add<PottedWillow>(SpiritConditions.TowerOrHideoutQuestFinished);

				shop.Add<KoiTotem>(SpiritConditions.ItsNoSalmonFinished);
				shop.Add<VibeshroomJarItem>(SpiritConditions.SanctuarySporeSalvageFinished);
				shop.Add<SeedBag>(SpiritConditions.WhyZombiesFinished);

				shop.Add<CryoKnife>(SpiritConditions.BeneathTheIceFinished);
				shop.Add<IceDeitySculpture>(SpiritConditions.BeneathTheIceFinished);

				shop.Add<FeralConcoction>(SpiritConditions.FloweryFiendsFinished);
			}
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) //Draws the exclamation mark on the NPC when they have a quest
		{
			bool valid = ModContent.GetInstance<SpiritClientConfig>().ShowNPCQuestNotice && npc.CanTalk; //Check if the NPC talks and if the config allows
			if (valid && ModContent.GetInstance<QuestWorld>().NPCQuestQueue.ContainsKey(npc.type) && ModContent.GetInstance<QuestWorld>().NPCQuestQueue[npc.type].Count > 0)
			{
				Texture2D tex = Mod.Assets.Request<Texture2D>("UI/QuestUI/Textures/ExclamationMark").Value;
				float scale = (float)Math.Sin(Main.time * 0.08f) * 0.14f;
				spriteBatch.Draw(tex, new Vector2(npc.Center.X - 2, npc.Center.Y - 40) - Main.screenPosition, new Rectangle(0, 0, 6, 24), Color.White, 0f, new Vector2(3, 12), 1f + scale, SpriteEffects.None, 0f);
			}
		}

		public static void AddToPool(int id, QuestPoolData data)
		{
			if (SpawnPoolMods.ContainsKey(id))
			{
				QuestPoolData currentData = SpawnPoolMods[id];
				if (SpawnPoolMods[id].NewRate is null || (data.NewRate != null && SpawnPoolMods[id].NewRate < data.NewRate))
					currentData.NewRate = data.NewRate;

				PoolModsCount[id]++;
			}
			else
			{
				SpawnPoolMods.Add(id, data);
				PoolModsCount.Add(id, 1);
			}
		}

		public static void RemoveFromPool(int id)
		{
			if (SpawnPoolMods.ContainsKey(id))
			{
				PoolModsCount[id]--;

				if (PoolModsCount[id] == 0)
				{
					SpawnPoolMods.Remove(id);
					PoolModsCount.Remove(id);
				}
			}
		}

		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			foreach (int item in SpawnPoolMods.Keys)
			{
				var currentPool = SpawnPoolMods[item];

				if (!pool.ContainsKey(item))
				{
					if (currentPool.Forced)
					{
						if (((currentPool.Exclusive && !NPC.AnyNPCs(item)) || !currentPool.Exclusive) && (currentPool.Conditions == null || currentPool.Conditions.Invoke(spawnInfo)))
							pool.Add(item, currentPool.NewRate.Value);
					}
					continue;
				}

				if (currentPool.NewRate is null) //We don't have a new rate to set to
					continue;

				if (((currentPool.Exclusive && !NPC.AnyNPCs(item)) || !currentPool.Exclusive) && (currentPool.Conditions == null || currentPool.Conditions.Invoke(spawnInfo)))
					pool[item] = currentPool.NewRate.Value;
			}

			if (QuestManager.GetQuest<SlayerQuestClown>().IsActive)
			{
				if (pool.ContainsKey(NPCID.Clown))
					pool[NPCID.Clown] = 0.1f;
			}

			if (QuestManager.GetQuest<SlayerQuestDrBones>().IsActive)
			{
				if (!Main.dayTime && spawnInfo.Player.ZoneJungle && !spawnInfo.PlayerSafe && spawnInfo.SpawnTileY < Main.worldSurface && !NPC.AnyNPCs(NPCID.DoctorBones) && pool.ContainsKey(NPCID.DoctorBones))
					pool[NPCID.DoctorBones] = 0.1f;
			}
		}
	}
}