using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Banners;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Banners
{
	public class BannerTile : ModTile
	{
		public readonly static (string item, string npc)[] BannersIndex = new (string, string)[]
		{
			("OccultistBanner", "OccultistBoss"),
			("BeholderBanner", "Beholder"),
			("BottomFeederBanner", "BottomFeeder"),
			("ValkyrieBanner", "Valkyrie"),
			("YureiBanner", "PagodaGhostHostile"),
			("SporeWheezerBanner", "SporeWheezer"),
			("WheezerBanner", "Wheezer"),
			("AstralAmalgamBanner", "AstralAmalgam"),
			("ShockhopperBanner", "DeepspaceHopper"),
			("AncientApostleBanner", "BoneHarpy"),
			("LostMimeBanner", "LostMime"),
			("StardancerBanner", "CogTrapperHead"),
			("CavernCrawlerBanner", "CavernCrawler"),
			("OrbititeBanner", "Mineroid"),
			("GladiatorSpiritBanner", "GladiatorSpirit"),
			("AntlionAssassinBanner", "AntlionAssassin"),
			("GoldCrateMimicBanner", "GoldCrateMimic"),
			("IronCrateMimicBanner", "IronCrateMimic"),
			("WoodCrateMimicBanner", "WoodCrateMimic"),
			("GraniteSlimeBanner", "GraniteSlime"),
			("PLACEHOLDER", "NONE"), //Old BlazingRattler
			("GhastBanner", "Illusionist"),
			("PLACEHOLDER", "NONE"), //Old SpectralSkull
			("GreenDungeonCubeBanner", "DungeonCubeGreen"),
			("PinkDungeonCubeBanner", "DungeonCubePink"),
			("BlueDungeonCubeBanner", "DungeonCubeBlue"),
			("WinterbornBanner", "WinterbornMelee"),
			("WinterbornHeraldBanner", "WinterbornMagic"),
			("DiseasedSlimeBanner", "DiseasedSlime"),
			("PLACEHOLDER", "NONE"), //Old DiseasedBat
			("CoconutSlimeBanner", "OceanSlime"),
			("BloaterBanner", "Spewer"),
			("ArterialGrasperBanner", "CrimsonTrapper"),
			("FesterflyBanner", "Vilemoth"),
			("PutromaBanner", "Teratoma"),
			("MasticatorBanner", "Masticator"),
			("BubbleBruteBanner", "LargeCrustecean"),
			("GluttonousDevourerBanner", "HellEater"),
			("ElectricEelBanner", "ElectricEel"),
			("BlossomHoundBanner", "BlossomHound"),
			("RlyehianBanner", "Rylheian"),
			("MangoWarBanner", "MangoJelly"),
			("CrocosaurBanner", "Crocomount"),
			("KakamoraGliderBanner", "KakamoraParachuter"),
			("KakamoraThrowerBanner", "SpearKakamora"),
			("KakamoraBruteBanner", "SwordKakamora"),
			("KakamoraShielderBanner", "KakamoraShielder"),
			("KakamoraShielderBanner1", "KakamoraShielderRare"),
			("KakamoraShamanBanner", "KakamoraShaman"),
			("BriarthornSlimeBanner", "ReachSlime"),
			("PLACEHOLDER", "NONE"), //Old Draseran Trapper/"GrassVine"
			("GladeWraithBanner", "ForestWraith"),
			("PLACEHOLDER", "NONE"), //Old CaptiveMask
			("DarkAlchemistBanner", "PlagueDoctor"),
			("BloatfishBanner", "SwollenFish"),
			("MechromancerBanner", "Mecromancer"),
			("KakamoraBanner", "KakamoraRunner"),
			("GloopBanner", "GloopGloop"),
			("ThornStalkerBanner", "ThornStalker"),
			("PLACEHOLDER", "NONE"), //Old ForgottenOne
			("DeadeyeMarksmanBanner", "DeadArcher"),
			("PhantomSamuraiBanner", "SamuraiHostile"),
			("FleshHoundBanner", "FleshHound"),
			("CracklingCoreBanner", "GraniteCore"),
			("CavernBanditBanner", "CavernBandit"),
			("ReachmanBanner", "Reachman"),
			("HemaphoraBanner", "Hemophora"),
			("MyceliumBotanistBanner", "MycelialBotanist"),
			("MoonlightPreserverBanner", "MoonlightPreserver"),
			("MoonlightRupturerBanner", "ExplodingMoonjelly"),
			("GiantJellyBanner", "MoonjellyGiant"),
			("BloomshroomBanner", "Bloomshroom"),
			("GlitterflyBanner", "Glitterfly"),
			("GlowToadBanner", "GlowToad"),
			("LumantisBanner", "Lumantis"),
			("LunarSlimeBanner", "LunarSlime"),
			("BlizzardBanditBanner", "BlizzardBandit"),
			("CrystalDrifterBanner", "CrystalDrifter"),
			("PLACEHOLDER", "NONE"), //Old BloodGazer
			("CystalBanner", "Cystal"),
			("WildwoodWatcherBanner", "ReachObserver"),
			("MoltenCoreBanner", "Molten_Core"),
			("PokeyBanner", "Pokey_Body"),
			("ScreechOwlBanner", "ScreechOwl"),
			("ArachmatonBanner", "AutomataCreeper"),
			("AstralAdventurerBanner", "AstralAdventurer"),
			("TrochmatonBanner", "AutomataSpinner"),
			("ChestZombieBanner", "Chest_Zombie"),
			("BoulderBehemothBanner", "Boulder_Termagant"),
			("FallingAsteroidBanner", "Falling_Asteroid"),
			("GoblinGrenadierBanner", "Goblin_Grenadier"),
			("BlazingSkullBanner", "BlazingSkull"),
			("StymphalianBatBanner", "StymphalianBat"),
			("SkeletonBruteBanner", "Skeleton_Brute"),
			("DraugrBanner", "Enchanted_Armor"),
			("PirateLobberBanner", "PirateLobber"),
			(nameof(GranitecTurretBanner), "GraniteSentry"),
			(nameof(HauntedTomeBanner), "HauntedTome"),
			(nameof(AlienBanner), "Alien"),
			(nameof(SpiritTomeBanner), "HauntedBook"),
			(nameof(AncientSpectreBanner), "AncientDemon"),
			(nameof(BlizzardNimbusBanner), "BlizzardNimbus"),
		};

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.StyleWrapLimit = 111;
			TileObjectData.addTile(Type);

			DustType = -1;
			TileID.Sets.DisableSmartCursor[Type] = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Banner");
			AddMapEntry(new Color(13, 88, 130), name);
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			int height = tile.TileFrameY == 36 ? 18 : 16;
			Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Banners/BannerTile_Glow").Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White * .8f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			var banner = GetBannerItem(frameX);
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 48, Mod.Find<ModItem>(banner).Type);
		}

		private static string GetBannerItem(int frameX)
		{
			int style = frameX / 18;
			return BannersIndex[style].item;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			int style = Main.tile[i, j].TileFrameX / 18;
			var npc = BannersIndex[style].npc;

			Main.SceneMetrics.NPCBannerBuff[Mod.Find<ModNPC>(npc).Type] = true;
			Main.SceneMetrics.hasBanner = true;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			if (i % 2 == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;
		}
	}
}