using System;
using System.Collections.Generic;
using System.Linq;
using SpiritMod.Items.Sets.RlyehianDrops;
using SpiritMod.Items.Consumable;
using SpiritMod.Items.Placeable.MusicBox;
using SpiritMod.NPCs.MoonjellyEvent;
using SpiritMod.NPCs.Tides;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace SpiritMod.Utilities
{
	/// <summary>
	/// A class containing useful methods for registering <c>BossChecklist</c> information,
	/// along with internal methods for initializing <see cref="SpiritMod"/>-specific data.
	/// </summary>
	public static class BossChecklistDataHandler
	{
		public enum EntryType
		{
			Boss,
			Miniboss,
			Event
		}

		public static Mod BossChecklistMod
		{
			get
			{
				if (ModLoader.TryGetMod("BossChecklist", out Mod result))
					return result;
				return null;
			}
		}

		public static bool BossChecklistIsLoaded => BossChecklistMod != null;

		public class BCIDData
		{
			public readonly List<int> npcIDs;
			public readonly List<int> itemSpawnIDs;
			public readonly List<int> itemCollectionIDs;

			public BCIDData(List<int> npcIDs, List<int> itemSpawnIDs, List<int> itemCollectionIDs)
			{
				this.npcIDs = npcIDs;
				this.itemSpawnIDs = itemSpawnIDs;
				this.itemCollectionIDs = itemCollectionIDs;
			}
		}

		/*public static void LogBoss(this Mod mod, float progression, string npcName, Func<bool> downedCondition,
			BCIDData identificationData, string spawnInfo, string despawnMessage, Action<SpriteBatch, Rectangle, Color> portrait,
			string overrideHeadIconTexture, Func<bool> bossAvailable) =>
			AddBCEntry(EntryType.Boss, mod, progression, npcName, downedCondition, identificationData, spawnInfo,
				despawnMessage, portrait, overrideHeadIconTexture, bossAvailable);

		public static void LogMiniBoss(this Mod mod, float progression, string npcName, Func<bool> downedCondition,
			BCIDData identificationData, string spawnInfo, string despawnMessage, Action<SpriteBatch, Rectangle, Color> portrait,
			string overrideHeadIconTexture, Func<bool> bossAvailable) =>
			AddBCEntry(EntryType.Miniboss, mod, progression, npcName, downedCondition, identificationData, spawnInfo,
				despawnMessage, portrait, overrideHeadIconTexture, bossAvailable);*/

		public static void LogEvent(this Mod mod, float progression, string eventName, Func<bool> downedCondition,
			BCIDData identificationData, LocalizedText spawnInfo, string portrait,
			string overrideHeadIconTexture,
			Func<bool> eventAvailable) =>
			AddBCEntry(EntryType.Event, mod, progression, eventName, downedCondition, identificationData, spawnInfo,
				null, portrait, overrideHeadIconTexture, eventAvailable);

		private static void AddBCEntry(EntryType entryType, Mod mod, float progression, string name,
			Func<bool> downedCondition, BCIDData identificationData, LocalizedText spawnInfo,
			LocalizedText despawnMessage, string portrait, string overrideHeadIconTexture,
			Func<bool> bossAvailable)
		{
			string logType = entryType switch
			{
				EntryType.Boss => "LogBoss",
				EntryType.Miniboss => "LogMiniBoss",
				EntryType.Event => "LogEvent",
				_ => throw new ArgumentOutOfRangeException(nameof(entryType), entryType, null),
			};

			var extraData = new Dictionary<string, object>()
			{
				["spawnItems"] = identificationData.itemSpawnIDs ?? new List<int>(),
				["collectibles"] = identificationData.itemCollectionIDs ?? new List<int>(),
				["availability"] = bossAvailable ?? (() => true),
			};

			if (overrideHeadIconTexture != string.Empty)
				extraData.Add("overrideHeadTextures", overrideHeadIconTexture);
			if (spawnInfo != null)
				extraData.Add("spawnInfo", spawnInfo);
			if (despawnMessage != null)
				extraData.Add("despawnMessage", despawnMessage);
			if (portrait != string.Empty)
			{
				extraData.Add("customPortrait", (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
					Texture2D texture = ModContent.Request<Texture2D>(portrait).Value;
					Vector2 centered = rect.Location.ToVector2() + (rect.Size() / 2) - (texture.Size() / 2);
					spriteBatch.Draw(texture, centered, color);
				});
			}

			BossChecklistMod.Call(
				logType,
				mod,
				name,
				progression,
				downedCondition,
				identificationData.npcIDs ?? new List<int>(),
				extraData
			);
		}
		
		internal static void RegisterSpiritData(Mod spiritMod)
		{
			if (!BossChecklistIsLoaded)
				return;

			RegisterSpiritEvents(spiritMod);
			RegisterInterfaces(spiritMod);
		}

		private static void RegisterSpiritEvents(Mod spiritMod)
		{
			spiritMod.LogEvent(
				2.4f,
				"JellyDeluge",
				() => MyWorld.downedJellyDeluge,
				new BCIDData(
					new List<int> {
						ModContent.NPCType<DreamlightJelly>(), ModContent.NPCType<ExplodingMoonjelly>(),
						ModContent.NPCType<MoonjellyGiant>(), ModContent.NPCType<MoonlightPreserver>(),
						ModContent.NPCType<TinyLunazoa>()
					},
					new List<int> {
						ModContent.ItemType<DistressJellyItem>()
					},
					new List<int> {
						ModContent.ItemType<JellyDelugeBox>()
					}),
				Language.GetText("Mods.SpiritMod.Events.JellyDeluge.BossChecklistIntegration.Condition"),
				"SpiritMod/Textures/BossChecklist/JellyDeluge",
				"SpiritMod/Textures/BossChecklist/JellyDelugeIcon",
				null
			);

			spiritMod.LogEvent(
				6.6f,
				"TheTide",
				() => MyWorld.downedTide,
				new BCIDData(
					new List<int> {
						ModContent.NPCType<Crocomount>(),
						ModContent.NPCType<KakamoraParachuter>(),
						ModContent.NPCType<KakamoraRunner>(),
						ModContent.NPCType<KakamoraShaman>(),
						ModContent.NPCType<KakamoraShielder>(),
						ModContent.NPCType<KakamoraShielderRare>(),
						ModContent.NPCType<LargeCrustecean>(),
						ModContent.NPCType<MangoJelly>(),
						ModContent.NPCType<Rylheian>(),
						ModContent.NPCType<SpearKakamora>(),
						ModContent.NPCType<SwordKakamora>()
					},
					new List<int> {
						ModContent.ItemType<BlackPearl>()
					},
					new List<int> {
						ModContent.ItemType<Trophy10>(),
						ModContent.ItemType<RlyehMask>(),
						ModContent.ItemType<TideBox>()
					}),
				Language.GetText("Mods.SpiritMod.Events.TheTide.BossChecklistIntegration.Condition"),
				"SpiritMod/Textures/BossChecklist/TideTexture",
				"SpiritMod/Textures/InvasionIcons/Depths_Icon",
				null
			);

			spiritMod.LogEvent(
				7.5f,
				"MysticMoon",
				() => MyWorld.downedBlueMoon,
				new BCIDData(
					new List<int> {
						ModContent.NPCType<NPCs.BlueMoon.Bloomshroom.Bloomshroom>(),
						ModContent.NPCType<NPCs.BlueMoon.Glitterfly.Glitterfly>(),
						ModContent.NPCType<NPCs.BlueMoon.GlowToad.GlowToad>(),
						ModContent.NPCType<NPCs.BlueMoon.Lumantis.Lumantis>(),
						ModContent.NPCType<NPCs.BlueMoon.LunarSlime.LunarSlime>(),
						ModContent.NPCType<NPCs.BlueMoon.MadHatter.MadHatter>()
					},
					new List<int> {
						ModContent.ItemType<BlueMoonSpawn>()
					},
					null),
				Language.GetText("Mods.SpiritMod.Events.BlueMoon.BossChecklistIntegration.Condition"),
				"SpiritMod/Textures/BossChecklist/MysticMoonTexture",
				"SpiritMod/Textures/BossChecklist/BlueMoonIcon",
				null
			);
		}

		private static void RegisterInterfaces(Mod spiritMod)
		{
			foreach (Type type in spiritMod.Code.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IBCRegistrable)))) {
				BCIDData identificationData = new BCIDData(null, null, null);
				LocalizedText spawnInfo = null;
				LocalizedText despawnMessage = null;
				string portrait = "";
				string headTextureOverride = "";
				Func<bool> isAvailable = null;

				if (Activator.CreateInstance(type) is not IBCRegistrable registrableType)
					continue;

				registrableType.RegisterToChecklist(out EntryType entryType, out float progression, out string name,
					out Func<bool> downedCondition, ref identificationData, ref spawnInfo, ref despawnMessage,
					ref portrait, ref headTextureOverride, ref isAvailable);

				AddBCEntry(entryType, spiritMod, progression, name, downedCondition, identificationData, spawnInfo, despawnMessage, portrait, headTextureOverride, isAvailable);
			}
		}
	}
}
