using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Banners;
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
			(nameof(OccultistBanner), "OccultistBoss"),
			(nameof(BeholderBanner), "Beholder"),
			(nameof(BottomFeederBanner), "BottomFeeder"),
			(nameof(ValkyrieBanner), "Valkyrie"),
			(nameof(YureiBanner), "PagodaGhostHostile"),
			(nameof(SporeWheezerBanner), "SporeWheezer"),
			(nameof(WheezerBanner), "Wheezer"),
			(nameof(AstralAmalgamBanner), "AstralAmalgam"),
			(nameof(ShockhopperBanner), "DeepspaceHopper"),
			(nameof(AncientApostleBanner), "BoneHarpy"),
			(nameof(LostMimeBanner), "LostMime"),
			(nameof(StardancerBanner), "CogTrapperHead"),
			(nameof(CavernCrawlerBanner), "CavernCrawler"),
			(nameof(OrbititeBanner), "Mineroid"),
			(nameof(GladiatorSpiritBanner), "GladiatorSpirit"),
			(nameof(AntlionAssassinBanner), "AntlionAssassin"),
			(nameof(GoldCrateMimicBanner), "GoldCrateMimic"),
			(nameof(IronCrateMimicBanner), "IronCrateMimic"),
			(nameof(WoodCrateMimicBanner), "WoodCrateMimic"),
			(nameof(GraniteSlimeBanner), "GraniteSlime"),
			("PLACEHOLDER", "NONE"), //Old BlazingRattler
			(nameof(GhastBanner), "Illusionist"),
			("PLACEHOLDER", "NONE"), //Old SpectralSkull
			(nameof(GreenDungeonCubeBanner), "DungeonCubeGreen"),
			(nameof(PinkDungeonCubeBanner), "DungeonCubePink"),
			(nameof(BlueDungeonCubeBanner), "DungeonCubeBlue"),
			(nameof(WinterbornBanner), "WinterbornMelee"),
			(nameof(WinterbornHeraldBanner), "WinterbornMagic"),
			(nameof(DiseasedSlimeBanner), "DiseasedSlime"),
			("PLACEHOLDER", "NONE"), //Old DiseasedBat
			(nameof(CoconutSlimeBanner), "OceanSlime"),
			(nameof(BloaterBanner), "Spewer"),
			(nameof(ArterialGrasperBanner), "CrimsonTrapper"),
			(nameof(FesterflyBanner), "Vilemoth"),
			(nameof(PutromaBanner), "Teratoma"),
			(nameof(MasticatorBanner), "Masticator"),
			(nameof(BubbleBruteBanner), "LargeCrustecean"),
			(nameof(GluttonousDevourerBanner), "HellEater"),
			(nameof(ElectricEelBanner), "ElectricEel"),
			(nameof(BlossomHoundBanner), "BlossomHound"),
			(nameof(RlyehianBanner), "Rylheian"),
			(nameof(MangoWarBanner), "MangoJelly"),
			(nameof(CrocosaurBanner), "Crocomount"),
			(nameof(KakamoraGliderBanner), "KakamoraParachuter"),
			(nameof(KakamoraThrowerBanner), "SpearKakamora"),
			(nameof(KakamoraBruteBanner), "SwordKakamora"),
			(nameof(KakamoraShielderBanner), "KakamoraShielder"),
			(nameof(KakamoraShielderBanner1), "KakamoraShielderRare"),
			(nameof(KakamoraShamanBanner), "KakamoraShaman"),
			(nameof(BriarthornSlimeBanner), "ReachSlime"),
			("PLACEHOLDER", "NONE"), //Old Draseran Trapper/"GrassVine"
			(nameof(GladeWraithBanner), "ForestWraith"),
			("PLACEHOLDER", "NONE"), //Old CaptiveMask
			(nameof(DarkAlchemistBanner), "PlagueDoctor"),
			(nameof(BloatfishBanner), "SwollenFish"),
			(nameof(MechromancerBanner), "Mecromancer"),
			(nameof(KakamoraBanner), "KakamoraRunner"),
			(nameof(GloopBanner), "GloopGloop"),
			(nameof(ThornStalkerBanner), "ThornStalker"),
			("PLACEHOLDER", "NONE"), //Old ForgottenOne
			(nameof(DeadeyeMarksmanBanner), "DeadArcher"),
			(nameof(PhantomSamuraiBanner), "SamuraiHostile"),
			(nameof(FleshHoundBanner), "FleshHound"),
			(nameof(CracklingCoreBanner), "GraniteCore"),
			(nameof(CavernBanditBanner), "CavernBandit"),
			(nameof(ReachmanBanner), "Reachman"),
			(nameof(HemaphoraBanner), "Hemophora"),
			(nameof(MyceliumBotanistBanner), "MycelialBotanist"),
			(nameof(MoonlightPreserverBanner), "MoonlightPreserver"),
			(nameof(MoonlightRupturerBanner), "ExplodingMoonjelly"),
			(nameof(GiantJellyBanner), "MoonjellyGiant"),
			(nameof(BloomshroomBanner), "Bloomshroom"),
			(nameof(GlitterflyBanner), "Glitterfly"),
			(nameof(GlowToadBanner), "GlowToad"),
			(nameof(LumantisBanner), "Lumantis"),
			(nameof(LunarSlimeBanner), "LunarSlime"),
			(nameof(BlizzardBanditBanner), "BlizzardBandit"),
			(nameof(CrystalDrifterBanner), "CrystalDrifter"),
			("PLACEHOLDER", "NONE"), //Old BloodGazer
			(nameof(CystalBanner), "Cystal"),
			(nameof(WildwoodWatcherBanner), "ReachObserver"),
			(nameof(MoltenCoreBanner), "Molten_Core"),
			(nameof(PokeyBanner), "Pokey_Body"),
			(nameof(ScreechOwlBanner), "ScreechOwl"),
			(nameof(ArachmatonBanner), "AutomataCreeper"),
			(nameof(AstralAdventurerBanner), "AstralAdventurer"),
			(nameof(TrochmatonBanner), "AutomataSpinner"),
			(nameof(ChestZombieBanner), "Chest_Zombie"),
			(nameof(BoulderBehemothBanner), "Boulder_Termagant"),
			(nameof(FallingAsteroidBanner), "Falling_Asteroid"),
			(nameof(GoblinGrenadierBanner), "Goblin_Grenadier"),
			(nameof(BlazingSkullBanner), "BlazingSkull"),
			(nameof(StymphalianBatBanner), "StymphalianBat"),
			(nameof(SkeletonBruteBanner), "Skeleton_Brute"),
			(nameof(DraugrBanner), "Enchanted_Armor"),
			(nameof(PirateLobberBanner), "PirateLobber"),
			(nameof(GranitecTurretBanner), "GraniteSentry"),
			(nameof(HauntedTomeBanner), "HauntedTome"),
			(nameof(AlienBanner), "Alien"),
			(nameof(SpiritTomeBanner), "HauntedBook"),
			(nameof(AncientSpectreBanner), "AncientDemon"),
			(nameof(BlizzardNimbusBanner), "BlizzardNimbus"),
			(nameof(FallenAngelBanner), "FallenAngel"),
			(nameof(NetherbaneBanner), "NetherBane"),
			(nameof(HydraGreenBanner), "HydraHead"), //All three Hydra banners benefit the same NPC
			(nameof(HydraPurpleBanner), "HydraHead"), //^
			(nameof(HydraRedBanner), "HydraHead"), //^^
			(nameof(PhantomBanner), "Phantom"),
			(nameof(SpiritGhoulBanner), "SpiritGhoul"),
			(nameof(SoulCrusherBanner), "SoulCrusher"),
			(nameof(SpiritFloaterBanner), "SpiritVulture"),
			(nameof(SpiritBatBanner), "SeerBat"),
			(nameof(SpiritSkullBanner), "SpiritSkull"),
			(nameof(WanderingSoulBanner), "WanderingSoul"),
			(nameof(SpiritMummyBanner), "SpiritMummy")
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
			TileObjectData.newTile.StyleWrapLimit = 115;
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