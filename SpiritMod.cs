using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.NPCs.Town;
using SpiritMod.NPCs.Boss.Atlas;
using SpiritMod.NPCs.Tides.Tide;
using SpiritMod.Skies;
using SpiritMod.Skies.Overlays;
using SpiritMod.Utilities;
using SpiritMod.Dusts;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.UI;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.Utilities;
using Terraria.UI.Chat;
using SpiritMod.Prim;
using SpiritMod.Particles;
using SpiritMod.UI.QuestUI;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.BoonSystem;
using System.Collections.Concurrent;
using SpiritMod.Effects.Stargoop;
using SpiritMod.Mechanics.PortraitSystem;
using SpiritMod.Mechanics.Boids;
using SpiritMod.Mechanics.AutoSell;
using SpiritMod.Buffs.Summon;
using System.Linq;
using SpiritMod.Items.Weapon.Magic.Rhythm;
using SpiritMod.Items.Weapon.Magic.Rhythm.Anthem;
using SpiritMod.Mechanics.EventSystem;
using SpiritMod.Skies.Starjinx;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Items.Sets.OlympiumSet;
using SpiritMod.Mechanics.Coverings;
using static Terraria.ModLoader.Core.TmodFile;
using SpiritMod.Items.Sets.DyesMisc.HairDye;
using SpiritMod.Items.Glyphs;
using ReLogic.Content;
using SpiritMod.Items.Books.UI.MaterialUI;
using SpiritMod.Mechanics.Fathomless_Chest;

namespace SpiritMod
{
	public class SpiritMod : Mod
	{
		internal UserInterface BookUserInterface;
		public static QuestBookUI QuestBookUIState;
		public static QuestHUD QuestHUD;

		public static CoveringsManager Coverings;

		public static ModKeybind QuestBookHotkey;
		public static ModKeybind QuestHUDHotkey;

		internal UserInterface SlotUserInterface;

		public static SpiritMod Instance;
		public UnifiedRandom spiritRNG;
		public static Effect auroraEffect;
		public static BasicEffect basicEffect;
		public static Effect GSaber;
		public static TrailManager TrailManager;
		public static PrimTrailManager primitives;
		public static StargoopManager Metaballs;
		public static BoidHost Boids;

		public static Effect stardustOverlayEffect;
		public static Effect glitchEffect;
		public static Effect starjinxBorderEffect;
		public static Effect vignetteEffect;
		public static Effect StarjinxNoise;
		public static Effect CircleNoise;
		public static Effect StarfirePrims;
		public static Effect ScreamingSkullTrail;
		public static Effect RipperSlugShader;
		public static Effect RepeatingTextureShader;
		public static Effect PrimitiveTextureMap;
		public static Effect EyeballShader;
		public static Effect ArcLashShader;
		public static Effect ConicalNoise;
		public static Effect JemShaders;
		public static Effect SunOrbShader;
		public static Effect ThyrsusShader;
		public static Effect JetbrickTrailShader;
		public static Effect OutlinePrimShader;
		public static Effect AnthemCircle;
		public static Effect TeslaShader;

		public static IDictionary<string, Effect> ShaderDict = new Dictionary<string, Effect>();

		public static PerlinNoise GlobalNoise;
		public static GlitchScreenShader glitchScreenShader;
		public static StarjinxBorderShader starjinxBorderShader;
		public static Vignette vignetteShader;
		public static Texture2D noise;

		public AutoSellUI AutoSellUI_SHORTCUT;
		public Mechanics.AutoSell.Sell_NoValue.Sell_NoValue SellNoValue_SHORTCUT;
		public Mechanics.AutoSell.Sell_Lock.Sell_Lock SellLock_SHORTCUT;
		public Mechanics.AutoSell.Sell_Weapons.Sell_Weapons SellWeapons_SHORTCUT;

		public UserInterface AutoSellUI_INTERFACE;
		public UserInterface SellNoValue_INTERFACE;
		public UserInterface SellLock_INTERFACE;
		public UserInterface SellWeapons_INTERFACE;

		public static event Action<SpriteViewMatrix> OnModifyTransformMatrix;
		//public static Dictionary<int, Texture2D> Portraits = new Dictionary<int, Texture2D>(); //Portraits dict - Gabe

		public const string EMPTY_TEXTURE = "SpiritMod/Empty";

		public static Texture2D EmptyTexture
		{
			get;
			private set;
		}

		public static int GlyphCurrencyID;

		internal static float deltaTime;

		private Vector2 _lastScreenSize;
		private Vector2 _lastViewSize;
		private Viewport _lastViewPort;

		public static int OlympiumCurrencyID = 0;

		/// <summary>Automatically returns false for every NPC ID inside of this list in <seealso cref="NPCs.GNPC.AllowTrickOrTreat(NPC)"/>.
		/// Note that this should only be used in edge cases where an NPC is neither homeless nor has homeTileX/Y set.</summary>
		public readonly List<int> NPCCandyBlacklist = new List<int>();

		public bool FinishedContentSetup { get; private set; }

		public SpiritMod()
		{
			Instance = this;
			spiritRNG = new UnifiedRandom();
		}

		public ModPacket GetPacket(MessageType type, int capacity)
		{
			ModPacket packet = GetPacket(capacity + 1);
			packet.Write((byte)type);
			return packet;
		}

		// this is alright, and i'll expand it so it can still be used, but really this shouldn't be used
		public static ModPacket WriteToPacket(ModPacket packet, byte msg, params object[] param)
		{
			packet.Write(msg);

			for (int m = 0; m < param.Length; m++)
			{
				object obj = param[m];
				if (obj is bool) packet.Write((bool)obj);
				else if (obj is byte) packet.Write((byte)obj);
				else if (obj is int) packet.Write((int)obj);
				else if (obj is float) packet.Write((float)obj);
				else if (obj is double) packet.Write((double)obj);
				else if (obj is short) packet.Write((short)obj);
				else if (obj is ushort) packet.Write((ushort)obj);
				else if (obj is sbyte) packet.Write((sbyte)obj);
				else if (obj is uint) packet.Write((uint)obj);
				else if (obj is decimal) packet.Write((decimal)obj);
				else if (obj is long) packet.Write((long)obj);
				else if (obj is string) packet.Write((string)obj);
			}
			return packet;
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) => SpiritMultiplayer.HandlePacket(reader, whoAmI);

		public override object Call(params object[] args)
		{
			if (args.Length < 1)
			{
				var stack = new System.Diagnostics.StackTrace(true);
				Logger.Error("Call Error: No arguments given:\n" + stack.ToString());
				return null;
			}

			CallContext context;
			int? contextNum = args[0] as int?;
			if (contextNum.HasValue)
				context = (CallContext)contextNum.Value;
			else
				context = ParseCallName(args[0] as string);

			if (context == CallContext.Invalid && !contextNum.HasValue)
			{ //Check if it has a valid value
				var stack = new System.Diagnostics.StackTrace(true);
				Logger.Error("Call Error: Context invalid or null:\n" + stack.ToString());
				return null;
			}

			if (context <= CallContext.Invalid || context >= CallContext.Limit)
			{ //Check if value is in-bounds
				var stack = new System.Diagnostics.StackTrace(true);
				Logger.Error("Call Error: Context invalid:\n" + stack.ToString());
				return null;
			}

			try
			{
				if (context == CallContext.Downed) //Gets if a boss has been downed
					return BossDowned(args);
				else if (context == CallContext.GlyphGet) //Gets an item's glyph
					return GetGlyph(args);
				else if (context == CallContext.GlyphSet) //Sets an item's glyph
				{
					SetGlyph(args);
					return null;
				}
				else if (context == CallContext.AddQuest) //Adds a quest
					return QuestManager.ModCallAddQuest(args);
				else if (context == CallContext.UnlockQuest) //Unlocks a quest
				{
					QuestManager.ModCallUnlockQuest(args);
					return null;
				}
				else if (context == CallContext.GetQuestIsUnlocked) //Self explanatory until...
					return QuestManager.ModCallGetQuestValueFromContext(args, 0);
				else if (context == CallContext.GetQuestIsCompleted)
					return QuestManager.ModCallGetQuestValueFromContext(args, 2);
				else if (context == CallContext.GetQuestIsActive)
					return QuestManager.ModCallGetQuestValueFromContext(args, 1);
				else if (context == CallContext.GetQuestRewardsGiven) //...here
					return QuestManager.ModCallGetQuestValueFromContext(args, 3);
				else if (context == CallContext.Portrait) //Adds a new portrait from another mod
				{
					PortraitManager.ModCallAddPortrait(args);
					return null;
				}
				else if (context == CallContext.Events) //Gets or sets event bools
					return EventCall(args);
			}
			catch (Exception e)
			{
				Logger.Error("Call Error: " + e.Message + "\n" + e.StackTrace);
			}
			return null;
		}

		private object EventCall(object[] args)
		{
			if (args[1] is not bool get)
			{
				var stack = new System.Diagnostics.StackTrace(true);
				Logger.Error("Call Error: Invalid argument for Event call:\n" + stack.ToString());
				return null;
			}

			if (get)
				return GetEventFromCall(args[2]);
			else
				return SetEventFromCall(args[2], args[3], args.Length > 4 ? args[4] : null);
		}


		private static bool? GetEventFromCall(object nameVal)
		{
			if (nameVal is not string name)
				return null;

			return name.ToUpper() switch
			{
				"THETIDE" => TideWorld.TheTide,
				"TIDE" => TideWorld.TheTide,
				"CALMNIGHT" => MyWorld.calmNight,
				"BLUEMOON" => MyWorld.blueMoon,
				_ => null,
			};
		}

		private static bool? SetEventFromCall(object nameVal, object valueVal, object optionalVal)
		{
			if (nameVal is not string name)
				return null;

			if (valueVal is not bool value)
				return null;

			name = name.ToUpper();

			if (name == "THETIDE" || name == "TIDE")
			{
				if (optionalVal is not bool additional)
					additional = false;

				if (!value && TideWorld.TheTide)
				{
					TideWorld.TideWave = 5;
					TideWorld.TideWaveIncrease(additional);
					return true;
				}
				else if (value && !TideWorld.TheTide)
				{
					if (Main.netMode == NetmodeID.MultiplayerClient)
						WriteToPacket(Instance.GetPacket(), (byte)MessageType.StartTide).Send();
					else
					{
						TideWorld.TheTide = true;
						TideWorld.TideWaveIncrease();
					}
					return true;
				}
				return false;
			}
			else if (name == "CALMNIGHT")
			{
				bool oldCalmNight = MyWorld.calmNight;
				MyWorld.calmNight = value;
				return oldCalmNight != value;
			}
			else if (name == "BLUEMOON")
			{
				bool oldBlueMoon = MyWorld.blueMoon;
				MyWorld.blueMoon = value;
				return oldBlueMoon != value;
			}
			return null;
		}

		private static CallContext ParseCallName(string context)
		{
			if (context == null)
				return CallContext.Invalid;

			return context switch
			{
				"downed" => CallContext.Downed,
				"getGlyph" => CallContext.GlyphGet,
				"setGlyph" => CallContext.GlyphSet,
				"AddQuest" => CallContext.AddQuest,
				"UnlockQuest" => CallContext.UnlockQuest,
				"IsQuestUnlocked" => CallContext.GetQuestIsUnlocked,
				"IsQuestActive" => CallContext.GetQuestIsActive,
				"IsQuestCompleted" => CallContext.GetQuestIsCompleted,
				"QuestRewardsGiven" => CallContext.GetQuestRewardsGiven,
				"Portrait" => CallContext.Portrait,
				_ => CallContext.Invalid,
			};
		}

		private static bool BossDowned(object[] args)
		{
			if (args.Length < 2)
				throw new ArgumentException("No boss name specified");

			string name = args[1] as string;

			return name switch
			{
				"Scarabeus" => MyWorld.downedScarabeus,
				"Moon Jelly Wizard" => MyWorld.downedMoonWizard,
				"Vinewrath Bane" => MyWorld.downedReachBoss,
				"Ancient Avian" => MyWorld.downedAncientFlier,
				"Starplate Raider" => MyWorld.downedRaider,
				"Infernon" => MyWorld.downedInfernon,
				"Dusking" => MyWorld.downedDusking,
				"Atlas" => MyWorld.downedAtlas,
				_ => throw new ArgumentException("Invalid boss name:" + name),
			};
		}

		private static void SetGlyph(object[] args)
		{
			if (args.Length < 2)
				throw new ArgumentException("Missing argument: Item");
			else if (args.Length < 3)
				throw new ArgumentException("Missing argument: Glyph");
			if (args[1] is not Item item)
				throw new ArgumentException("First argument must be of type Item");
			int? glyphID = args[2] as int?;
			if (!glyphID.HasValue)
				throw new ArgumentException("Second argument must be of type int");
			GlyphType glyph = (GlyphType)glyphID;
			if (glyph < GlyphType.None || glyph >= GlyphType.Count)
				throw new ArgumentException("Glyph must be in range [" +
					(int)GlyphType.None + "," + (int)GlyphType.Count + ")");
			item.GetGlobalItem<Items.GItem>().SetGlyph(item, glyph);
		}

		private static int GetGlyph(object[] args)
		{
			if (args.Length < 2)
				throw new ArgumentException("Missing argument: Item");
			if (args[1] is not Item item)
				throw new ArgumentException("First argument must be of type Item");
			return (int)item.GetGlobalItem<Items.GItem>().Glyph;
		}

		public override void Load()
		{
			//Always keep this call in the first line of Load!
			LoadReferences();

			QuestBookHotkey = KeybindLoader.RegisterKeybind(this, "SpiritMod:QuestBookToggle", Microsoft.Xna.Framework.Input.Keys.Q);
			QuestHUDHotkey = KeybindLoader.RegisterKeybind(this, "SpiritMod:QuestHUDToggle", Microsoft.Xna.Framework.Input.Keys.V);

			QuestManager.Load();
			if (!Main.dedServ)
			{
				ParticleHandler.RegisterParticles();

				BookUserInterface = new UserInterface();
				BookUserInterface.SetState(new UIBismitePageStsate());
				BookUserInterface.SetState(null);

				QuestBookUIState = new QuestBookUI();
				QuestHUD = new QuestHUD();

				Boids = new BoidHost();
				EventManager.Load();
				_lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
				_lastViewSize = Main.ViewSize;
				_lastViewPort = Main.graphics.GraphicsDevice.Viewport;
			}

			BoonLoader.Load();
			SpiritMultiplayer.Load();
			SpiritDetours.Initialize();
			ChanceEffectManager.Load();
			//Coverings = new CoveringsManager();
			//Coverings.Load(this);

			GlobalNoise = new PerlinNoise(Main.rand.Next());
			Main.rand ??= new UnifiedRandom();

			Items.Halloween.CandyBag.Initialize();

			OlympiumCurrencyID = CustomCurrencyManager.RegisterCurrency(new OlympiumCurrency(ModContent.ItemType<OlympiumToken>(), 999));

			#region configLabels
			void AddTranslation(string name, string defaultValue)
			{
				ModTranslation config = LocalizationLoader.CreateTranslation(this, name);
				config.SetDefault(defaultValue);
				LocalizationLoader.AddTranslation(config);
			}

			//Regular Config labels
			AddTranslation("Screenshake", $"[i:" + Find<ModItem>("BlueNeonSign").Type + "]  Visuals: Screenshake");
			AddTranslation("Distortion", $"[i:" + Find<ModItem>("BlueNeonSign").Type + "]  Visuals: Screen Distortion");
			AddTranslation("Particles", $"[i:" + Find<ModItem>("BlueNeonSign").Type + "]  Visuals: Foreground Particles");
			AddTranslation("Quicksell", $"[i:" + Find<ModItem>("SeedBag").Type + "]  QoL: Quick-Sell Feature");
			AddTranslation("Autoswing", $"[i:" + Find<ModItem>("PurpleNeonSign").Type + "]  QoL: Auto-Reuse Tooltip");
			AddTranslation("AmbientSounds", $"[i:" + Find<ModItem>("SurrenderBell").Type + "]  Ambience: Ambient Sounds");
			AddTranslation("LeafFallAmbience", $"[i:" + Find<ModItem>("EnchantedLeaf").Type + "]  Ambience: Falling Leaf Effects");
			AddTranslation("QuestButton", $"[i:" + Find<ModItem>("Book_Slime").Type + "]  Quests: Quest Book Button Location");
			AddTranslation("QuestBookSwitching", $"[i:" + Find<ModItem>("Book_Slime").Type + "]  Quests: Quest Book Tab Switching");
			AddTranslation("QuestIcons", $"[i:" + Find<ModItem>("Brightbulb").Type + "]  Quests: Town NPC Quest Icons");
			AddTranslation("ArcaneHideoutGen", $"[i:" + Find<ModItem>("JellyCandle").Type + "]  Worldgen: Arcane Tower and Bandit Hideout Generation");
			AddTranslation("OceanShape", $"[i:" + Find<ModItem>("BlackPearl").Type + "]  Oceans: Ocean Generation Shape");
			AddTranslation("OceanVents", $"[i:" + Find<ModItem>("BlackPearl").Type + "]  Oceans: Hydothermal Vent Ecosystems");
			AddTranslation("OceanWater", $"[i:" + Find<ModItem>("BlackPearl").Type + "]  Oceans: Ocean Water Transparency");
			AddTranslation("WaterEnemies", $"[i:" + Find<ModItem>("SpiritKoi").Type + "]  Fishing: Fishing Encounters");

			//Music Config
			void AddMusicTranslation(string name, string addedValue)
			{
				ModTranslation config = LocalizationLoader.CreateTranslation(this, name);
				config.SetDefault($"[i:" + Find<ModItem>("NeonMusicBox").Type + "]  " + addedValue);
				LocalizationLoader.AddTranslation(config);
			}

			AddMusicTranslation("BlizzardMusic", "Adds a unique track for Blizzards");
			AddMusicTranslation("SnowNightMusic", "Adds a unique track for the Snow biome at night");
			AddMusicTranslation("DesertNightMusic", "Adds a unique track for the Desert biome at night");
			AddMusicTranslation("HallowNightMusic", "Adds a unique track for The Hallow at night");
			AddMusicTranslation("CorruptNightMusic", "Adds a unique track for The Corruption at night");
			AddMusicTranslation("CrimsonNightMusic", "Adds a unique track for The Crimson at night");
			AddMusicTranslation("GraniteMusic", "Adds a unique track for the Granite biome");
			AddMusicTranslation("MarbleMusic", "Adds a unique track for the Marble biome");
			AddMusicTranslation("SpiderMusic", "Adds a unique track for the Spider caverns");
			AddMusicTranslation("MeteorMusic", "Adds a unique track for the Meteorite biome");
			AddMusicTranslation("FrostLegionMusic", "Adds a unique track for the Frost Legion invasion");
			AddMusicTranslation("SkeletonPrimeMusic", "Adds a unique track for Skeletron Prime");
			AddMusicTranslation("AuroraMusic", "Adds a unique track for the Aurora");
			AddMusicTranslation("LuminousMusic", "Adds a unique track for Luminous Oceans");
			AddMusicTranslation("CalmNightMusic", "Adds a unique track for Calm Night mini-events");
			AddMusicTranslation("OceanDepthsMusic", "Adds a unique track for the Ocean depths");
			AddMusicTranslation("HyperspaceMusic", "Adds two unique tracks for the artificial Hyperspace biome");
			AddMusicTranslation("AshfallMusic", "Adds a unique track for the Ashfall weather in the Underworld");
			AddMusicTranslation("VictoryNightMusic", "Adds a unique track for the Lantern Night event");
			#endregion

			if (Main.netMode != NetmodeID.Server)
			{
				Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("SpiritMod/Effects/ShockwaveEffect", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "Shockwave"), EffectPriority.VeryHigh);
				Filters.Scene["Shockwave"].Load();

				Filters.Scene["PulsarShockwave"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("SpiritMod/Effects/PulsarShockwave", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "PulsarShockwave"), EffectPriority.VeryHigh);
				Filters.Scene["PulsarShockwave"].Load();

				SlotUserInterface = new UserInterface();

				Filters.Scene["ShockwaveTwo"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("SpiritMod/Effects/ShockwaveTwo", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "ShockwaveTwo"), EffectPriority.VeryHigh);
				Filters.Scene["ShockwaveTwo"].Load();

				Filters.Scene["SpiritMod:AshRain"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("SpiritMod/Effects/AshRain", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "AshRain"), EffectPriority.VeryLow);
				Filters.Scene["SpiritMod:AshRain"].Load();
			}

			Filters.Scene["SpiritMod:ReachSky"] = new Filter(new ScreenShaderData("FilterBloodMoon").UseColor(0.05f, 0.05f, .05f).UseOpacity(0.4f), EffectPriority.High);

			Filters.Scene["CystalTower"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.149f, 0.142f, 0.207f).UseOpacity(0.5f), EffectPriority.VeryHigh);
			Filters.Scene["CystalBloodMoon"] = new Filter(new ScreenShaderData("FilterBloodMoon").UseColor(0.149f, 0.142f, 0.207f).UseOpacity(2f), EffectPriority.VeryHigh);

			Filters.Scene["SpiritMod:SpiritUG1"] = new Filter(new ScreenShaderData("FilterBloodMoon").UseColor(0.2f, 0.2f, .2f).UseOpacity(0.8f), EffectPriority.High);
			Filters.Scene["SpiritMod:SpiritUG2"] = new Filter(new ScreenShaderData("FilterBloodMoon").UseColor(0.45f, 0.45f, .45f).UseOpacity(0.9f), EffectPriority.High);

			Filters.Scene["SpiritMod:WindEffect"] = new Filter((new BlizzardShaderData("FilterBlizzardForeground")).UseColor(0.4f, 0.4f, 0.4f).UseSecondaryColor(0.2f, 0.2f, 0.2f).UseImage("Images/Misc/noise", 0, null).UseOpacity(0.149f).UseImageScale(new Vector2(3f, 0.75f), 0), EffectPriority.High);
			Filters.Scene["SpiritMod:WindEffect2"] = new Filter((new BlizzardShaderData("FilterBlizzardForeground")).UseColor(0.4f, 0.4f, 0.4f).UseSecondaryColor(0.2f, 0.2f, 0.2f).UseImage("Images/Misc/noise", 0, null).UseOpacity(0.549f).UseImageScale(new Vector2(3f, 0.75f), 0), EffectPriority.High);

			GlyphCurrencyID = CustomCurrencyManager.RegisterCurrency(new Currency(ModContent.ItemType<Glyph>(), 999L));

			AutoloadMinionDictionary.AddBuffs(Code);

			if (Main.netMode != NetmodeID.Server)
			{
				TrailManager = new TrailManager(this);
				EmptyTexture = ModContent.Request<Texture2D>("SpiritMod/Empty", AssetRequestMode.ImmediateLoad).Value; 
				auroraEffect = ModContent.Request<Effect>("SpiritMod/Effects/aurora", AssetRequestMode.ImmediateLoad).Value;

				ShaderDict = new Dictionary<string, Effect>();
				var tmodfile = (TmodFile)typeof(SpiritMod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Instance);
				var files = (IDictionary<string, FileEntry>)typeof(TmodFile).GetField("files", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(tmodfile);
				foreach (KeyValuePair<string, FileEntry> kvp in files.Where(x => x.Key.Contains("Effects/") && x.Key.Contains(".xnb")))
					ShaderDict.Add(kvp.Key.Remove(kvp.Key.Length - ".xnb".Length, ".xnb".Length).Remove(0, "Effects/".Length), ModContent.Request<Effect>("SpiritMod/" + kvp.Key.Remove(kvp.Key.Length - ".xnb".Length, ".xnb".Length), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);

				int width = Main.graphics.GraphicsDevice.Viewport.Width;
				int height = Main.graphics.GraphicsDevice.Viewport.Height;
				Vector2 zoom = Main.GameViewMatrix.Zoom;
				Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
				Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);

				Main.QueueMainThreadAction(() =>
				{
					basicEffect = new BasicEffect(Main.graphics.GraphicsDevice)
					{
						VertexColorEnabled = true,
						View = view,
						Projection = projection
					};
				});

				noise = ModContent.Request<Texture2D>("SpiritMod/Textures/noise", AssetRequestMode.ImmediateLoad).Value;

				SpiritModAutoSellTextures.Load();

				GameShaders.Hair.BindShader(ModContent.ItemType<SeafoamDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(Color.Cyan, Color.White, MathHelper.Lerp(0.2f, 1f, (float)((Math.Sin(3f + Main.GlobalTimeWrappedHourly * 1.3f) + 1f) * 0.5f)))));
				GameShaders.Hair.BindShader(ModContent.ItemType<BrightbloodDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.IndianRed));
				GameShaders.Hair.BindShader(ModContent.ItemType<MeteorDye>(), new HairShaderData(Main.PixelShaderRef, "ArmorHades")).UseImage("Images/Misc/noise").UseColor(Color.Orange).UseSecondaryColor(Color.DarkOrange).UseSaturation(5.3f);
				GameShaders.Hair.BindShader(ModContent.ItemType<ViciousDye>(), new HairShaderData(Main.PixelShaderRef, "ArmorVortex")).UseImage("Images/Misc/noise").UseColor(Color.Crimson).UseSaturation(3.3f);
				GameShaders.Hair.BindShader(ModContent.ItemType<CystalDye>(), new HairShaderData(Main.PixelShaderRef, "ArmorNebula")).UseImage("Images/Misc/Perlin").UseColor(Color.Plum).UseSaturation(5.3f);
				GameShaders.Hair.BindShader(ModContent.ItemType<SnowMirageDye>(), new HairShaderData(Main.PixelShaderRef, "ArmorMirage")).UseImage("Images/Misc/Perlin").UseColor(Color.PaleTurquoise).UseSaturation(2.3f);

				PortraitManager.Load();

				AutoSellUI_INTERFACE = new UserInterface();
				SellNoValue_INTERFACE = new UserInterface();
				SellLock_INTERFACE = new UserInterface();
				SellWeapons_INTERFACE = new UserInterface();

				AutoSellUI_SHORTCUT = new AutoSellUI();
				SellNoValue_SHORTCUT = new Mechanics.AutoSell.Sell_NoValue.Sell_NoValue();
				SellLock_SHORTCUT = new Mechanics.AutoSell.Sell_Lock.Sell_Lock();
				SellWeapons_SHORTCUT = new Mechanics.AutoSell.Sell_Weapons.Sell_Weapons();

				AutoSellUI_SHORTCUT.Activate();
				SellNoValue_SHORTCUT.Activate();
				SellLock_SHORTCUT.Activate();
				SellWeapons_SHORTCUT.Activate();

				stardustOverlayEffect = ModContent.Request<Effect>("SpiritMod/Effects/StardustOverlay", AssetRequestMode.ImmediateLoad).Value;

				glitchEffect = ModContent.Request<Effect>("SpiritMod/Effects/glitch", AssetRequestMode.ImmediateLoad).Value;
				glitchScreenShader = new GlitchScreenShader(glitchEffect);
				Filters.Scene["SpiritMod:Glitch"] = new Filter(glitchScreenShader, (EffectPriority)50);

				starjinxBorderEffect = ModContent.Request<Effect>("SpiritMod/Effects/StarjinxBorder", AssetRequestMode.ImmediateLoad).Value;
				starjinxBorderShader = new StarjinxBorderShader(starjinxBorderEffect, "MainPS");
				Filters.Scene["SpiritMod:StarjinxBorder"] = new Filter(starjinxBorderShader, (EffectPriority)50);

				Filters.Scene["SpiritMod:StarjinxBorderFade"] = new Filter(new StarjinxBorderShader(starjinxBorderEffect, "FadePS"), (EffectPriority)70);

				vignetteEffect = ModContent.Request<Effect>("SpiritMod/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value;
				vignetteShader = new Vignette(vignetteEffect, "MainPS");
				Filters.Scene["SpiritMod:Vignette"] = new Filter(vignetteShader, (EffectPriority)100);

				StarjinxNoise = ModContent.Request<Effect>("SpiritMod/Effects/StarjinxNoise", AssetRequestMode.ImmediateLoad).Value;
				CircleNoise = ModContent.Request<Effect>("SpiritMod/Effects/CircleNoise", AssetRequestMode.ImmediateLoad).Value;
				StarfirePrims = ModContent.Request<Effect>("SpiritMod/Effects/StarfirePrims", AssetRequestMode.ImmediateLoad).Value;
				ScreamingSkullTrail = ModContent.Request<Effect>("SpiritMod/Effects/ScreamingSkullTrail", AssetRequestMode.ImmediateLoad).Value;
				RipperSlugShader = ModContent.Request<Effect>("SpiritMod/Effects/RipperSlugShader", AssetRequestMode.ImmediateLoad).Value;
				RepeatingTextureShader = ModContent.Request<Effect>("SpiritMod/Effects/RepeatingTextureShader", AssetRequestMode.ImmediateLoad).Value;
				PrimitiveTextureMap = ModContent.Request<Effect>("SpiritMod/Effects/PrimitiveTextureMap", AssetRequestMode.ImmediateLoad).Value;
				EyeballShader = ModContent.Request<Effect>("SpiritMod/Effects/EyeballShader", AssetRequestMode.ImmediateLoad).Value;
				ArcLashShader = ModContent.Request<Effect>("SpiritMod/Effects/ArcLashShader", AssetRequestMode.ImmediateLoad).Value;
				ConicalNoise = ModContent.Request<Effect>("SpiritMod/Effects/ConicalNoise", AssetRequestMode.ImmediateLoad).Value;
				JemShaders = ModContent.Request<Effect>("SpiritMod/Effects/JemShaders", AssetRequestMode.ImmediateLoad).Value;
				SunOrbShader = ModContent.Request<Effect>("SpiritMod/Effects/SunOrbShader", AssetRequestMode.ImmediateLoad).Value;
				ThyrsusShader = ModContent.Request<Effect>("SpiritMod/Effects/ThyrsusShader", AssetRequestMode.ImmediateLoad).Value;
				JetbrickTrailShader = ModContent.Request<Effect>("SpiritMod/Effects/JetbrickTrailShader", AssetRequestMode.ImmediateLoad).Value;
				OutlinePrimShader = ModContent.Request<Effect>("SpiritMod/Effects/OutlinePrimShader", AssetRequestMode.ImmediateLoad).Value;
				GSaber = ModContent.Request<Effect>("SpiritMod/Effects/GSaber", AssetRequestMode.ImmediateLoad).Value;
				AnthemCircle = ModContent.Request<Effect>("SpiritMod/Effects/AnthemCircle", AssetRequestMode.ImmediateLoad).Value;
				TeslaShader = ModContent.Request<Effect>("SpiritMod/Effects/TeslaShader", AssetRequestMode.ImmediateLoad).Value;

				SkyManager.Instance["SpiritMod:AuroraSky"] = new AuroraSky();
				Filters.Scene["SpiritMod:AuroraSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);
				Overlays.Scene["SpiritMod:AuroraSky"] = new AuroraOverlay();

				Filters.Scene["SpiritMod:BlueMoonSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.1f, 0.2f, 0.5f).UseOpacity(0.53f), EffectPriority.High);
				SkyManager.Instance["SpiritMod:BlueMoonSky"] = new BlueMoonSky();

				SkyManager.Instance["SpiritMod:StarjinxSky"] = new StarjinxSky();
				Filters.Scene["SpiritMod:StarjinxSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);

				SkyManager.Instance["SpiritMod:MeteorSky"] = new MeteorSky();
				SkyManager.Instance["SpiritMod:AsteroidSky2"] = new MeteorBiomeSky2();
				Filters.Scene["SpiritMod:MeteorSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);
				Filters.Scene["SpiritMod:AsteroidSky2"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);

				SkyManager.Instance["SpiritMod:SpiritBiomeSky"] = new SpiritBiomeSky();
				Filters.Scene["SpiritMod:SpiritBiomeSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);

				SkyManager.Instance["SpiritMod:JellySky"] = new JellySky();
				Filters.Scene["SpiritMod:JellySky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);

				SkyManager.Instance["SpiritMod:PurpleAlgaeSky"] = new PurpleAlgaeSky();
				Filters.Scene["SpiritMod:PurpleAlgaeSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);

				SkyManager.Instance["SpiritMod:GreenAlgaeSky"] = new GreenAlgaeSky();
				Filters.Scene["SpiritMod:GreenAlgaeSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);

				SkyManager.Instance["SpiritMod:OceanFloorSky"] = new OceanFloorSky();
				Filters.Scene["SpiritMod:OceanFloorSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);

				SkyManager.Instance["SpiritMod:BlueAlgaeSky"] = new BlueAlgaeSky();
				Filters.Scene["SpiritMod:BlueAlgaeSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);

				SkyManager.Instance["SpiritMod:BloodMoonSky"] = new BloodMoonSky();
				Filters.Scene["SpiritMod:BloodMoonSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryLow);

				Filters.Scene["SpiritMod:Atlas"] = new Filter(new AtlasScreenShaderData("FilterMiniTower").UseColor(0.5f, 0.5f, 0.5f).UseOpacity(0.6f), EffectPriority.VeryHigh);
				SkyManager.Instance["SpiritMod:Atlas"] = new AtlasSky();

				Filters.Scene["SpiritMod:SynthwaveSky"] = new Filter(new AtlasScreenShaderData("FilterMiniTower").UseColor(0.158f, 0.083f, 0.212f).UseOpacity(0.43f), EffectPriority.VeryHigh);
				SkyManager.Instance["SpiritMod:SynthwaveSky"] = new VaporwaveSky();

				Filters.Scene["SpiritMod:MeteoriteSky"] = new Filter(new AtlasScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
				SkyManager.Instance["SpiritMod:MeteoriteSky"] = new MeteoriteSky();

				//Music Boxes
				LoadMusicBox("CrimsonNightBox", "Sounds/Music/CrimsonNight");
				LoadMusicBox("OceanDepthsBox", "Sounds/Music/UnderwaterMusic");
				LoadMusicBox("TranquilWindsBox", "Sounds/Music/TranquilWinds");
				LoadMusicBox("NeonMusicBox", "Sounds/Music/NeonTech");
				LoadMusicBox("HyperspaceDayBox", "Sounds/Music/NeonTech1");
				LoadMusicBox("SpiritBox1", "Sounds/Music/SpiritOverworld");
				LoadMusicBox("SpiritBox2", "Sounds/Music/SpiritLayer1");
				LoadMusicBox("SpiritBox3", "Sounds/Music/SpiritLayer2");
				LoadMusicBox("SpiritBox4", "Sounds/Music/SpiritLayer3");
				LoadMusicBox("ReachBox", "Sounds/Music/Reach");
				LoadMusicBox("BriarNightBox", "Sounds/Music/ReachNighttime");
				LoadMusicBox("AsteroidBox", "Sounds/Music/Asteroids");
				LoadMusicBox("StarplateBox", "Sounds/Music/Starplate");
				LoadMusicBox("MJWBox", "Sounds/Music/MoonJelly");
				LoadMusicBox("ScarabBox", "Sounds/Music/Scarabeus");
				LoadMusicBox("AtlasBox", "Sounds/Music/Atlas");
				LoadMusicBox("VinewrathBox", "Sounds/Music/ReachBoss");
				LoadMusicBox("AvianBox", "Sounds/Music/AncientAvian");
				LoadMusicBox("InfernonBox", "Sounds/Music/Infernon");
				LoadMusicBox("BlizzardBox", "Sounds/Music/Blizzard");
				LoadMusicBox("AuroraBox", "Sounds/Music/AuroraSnow");
				LoadMusicBox("SnowNightBox", "Sounds/Music/SnowNighttime");
				LoadMusicBox("DesertNightBox", "Sounds/Music/DesertNighttime");
				LoadMusicBox("LuminousNightBox", "Sounds/Music/OceanNighttime");
				LoadMusicBox("HallowNightBox", "Sounds/Music/HallowNight");
				LoadMusicBox("CalmNightBox", "Sounds/Music/CalmNight");
				LoadMusicBox("CorruptNightBox", "Sounds/Music/CorruptNight");
				LoadMusicBox("MeteorBox", "Sounds/Music/Meteor");
				LoadMusicBox("MarbleBox", "Sounds/Music/MarbleBiome");
				LoadMusicBox("GraniteBox", "Sounds/Music/GraniteBiome");
				LoadMusicBox("SpiderCaveBox", "Sounds/Music/SpiderCave");
				LoadMusicBox("BlueMoonBox", "Sounds/Music/BlueMoon");
				LoadMusicBox("TideBox", "Sounds/Music/DepthInvasion");
				LoadMusicBox("JellyDelugeBox", "Sounds/Music/JellySky");
				LoadMusicBox("FrostLegionBox", "Sounds/Music/FrostLegion");
				LoadMusicBox("AshfallBox", "Sounds/Music/AshStorm");
				LoadMusicBox("VictoryDayBox", "Sounds/Music/VictoryDay");

				AutoSellUI.visible = false;
				Mechanics.AutoSell.Sell_NoValue.Sell_NoValue.visible = false;
				Mechanics.AutoSell.Sell_Lock.Sell_Lock.visible = false;
				Mechanics.AutoSell.Sell_Weapons.Sell_Weapons.visible = false;

				AutoSellUI_INTERFACE.SetState(AutoSellUI_SHORTCUT);
				SellNoValue_INTERFACE.SetState(SellNoValue_SHORTCUT);
				SellLock_INTERFACE.SetState(SellLock_SHORTCUT);
				SellWeapons_INTERFACE.SetState(SellWeapons_SHORTCUT);

				Main.OnPreDraw += DrawStarGoopTarget;

				primitives = new PrimTrailManager();
				primitives.LoadContent(Main.graphics.GraphicsDevice);

				InitStargoop();
				Boids.LoadContent();
				AdditiveCallManager.Load();

				RhythmMinigame.LoadStatic();
				GuitarMinigame.LoadStatic();
			}

			// using a mildly specific name to avoid mod clashes
			ChatManager.Register<UI.Chat.QuestTagHandler>(new string[] { "sq", "spiritQuest" });
		}

		private void LoadMusicBox(string name, string path) => MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, path), Find<ModItem>(name).Type, Find<ModTile>(name).Type);

		public void CheckScreenSize()
		{
			if (!Main.dedServ && !Main.gameMenu)
			{
				Main.QueueMainThreadAction(() =>
				{
					if (_lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight) && primitives != null)
						primitives.InitializeTargets(Main.graphics.GraphicsDevice);

					if (_lastViewSize != Main.ViewSize && Metaballs is not null)
						Metaballs.Initialize(Main.graphics.GraphicsDevice);

					if ((_lastViewPort.Bounds != Main.graphics.GraphicsDevice.Viewport.Bounds || _lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight) || _lastViewSize != Main.ViewSize)
						&& basicEffect != null && primitives != null)
					{
						Helpers.SetBasicEffectMatrices(ref basicEffect, Main.GameViewMatrix.Zoom);
						Helpers.SetBasicEffectMatrices(ref primitives.pixelEffect, Main.GameViewMatrix.Zoom);
						Helpers.SetBasicEffectMatrices(ref primitives.galaxyEffect, new Vector2(1));
					}

					_lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
					_lastViewSize = Main.ViewSize;
					_lastViewPort = Main.graphics.GraphicsDevice.Viewport;
				});
			}
		}

		/// <summary>
		/// Finds additional textures attached to things
		/// Puts the textures in _textures array
		/// </summary>
		private void LoadReferences()
		{
			foreach (Type type in Code.GetTypes())
			{
				if (type.IsAbstract)
					continue;

				var types = new[]{ typeof(ModItem), typeof(ModNPC), typeof(ModProjectile), typeof(ModDust), typeof(ModTile), typeof(ModWall), typeof(ModBuff), typeof(ModMount) };
				bool modType = types.Any(x => type.IsSubclassOf(x));

				if (Main.dedServ || !modType)
					continue;

				FieldInfo _texField = type.GetField("_textures");
				if (_texField == null || !_texField.IsStatic || _texField.FieldType != typeof(Texture2D[]))
					continue;

				string path = type.FullName.Replace('.', '/');
				int texCount = 0;

				while (ModContent.RequestIfExists<Texture2D>(path + "_" + (texCount + 1), out _))
					texCount++;

				Texture2D[] textures = new Texture2D[texCount + 1];

				if (ModContent.RequestIfExists(path, out Asset<Texture2D> texture, AssetRequestMode.ImmediateLoad))
					textures[0] = texture.Value;

				for (int i = 1; i <= texCount; i++)
					textures[i] = ModContent.Request<Texture2D>(path + "_" + i, AssetRequestMode.ImmediateLoad).Value;

				_texField.SetValue(null, textures);
			}
		}

		public static void InitStargoop()
		{
			Metaballs = new StargoopManager();
			Metaballs.LoadContent();
			Metaballs.Initialize(Main.graphics.GraphicsDevice);

			var friendlyDust = (FriendlyStargoopDust)ModContent.GetModDust(ModContent.DustType<FriendlyStargoopDust>());
			var enemyDust = (EnemyStargoopDust)ModContent.GetModDust(ModContent.DustType<EnemyStargoopDust>());

			friendlyDust.Reset();
			enemyDust.Reset();
		}

		public override void Unload()
		{
			BoonLoader.Unload();
			spiritRNG = null;
			auroraEffect = null;
			StarjinxNoise = null;
			CircleNoise = null;
			StarfirePrims = null;
			ScreamingSkullTrail = null;
			RipperSlugShader = null;
			EyeballShader = null;
			RepeatingTextureShader = null;
			PrimitiveTextureMap = null;
			ArcLashShader = null;
			ConicalNoise = null;
			JemShaders = null;
			ThyrsusShader = null;
			JetbrickTrailShader = null;
			OutlinePrimShader = null;
			SunOrbShader = null;
			noise = null;

			AutoSellUI_INTERFACE = null;
			SellNoValue_INTERFACE = null;
			SellWeapons_INTERFACE = null;
			SellLock_INTERFACE = null;

			SpiritModAutoSellTextures.Unload();

			QuestManager.Unload();
			QuestBookUIState = null;
			QuestHUD = null;
			QuestBookHotkey = null;
			QuestHUDHotkey = null;
			EventManager.Unload();
			//Coverings.Unload();
			//Coverings = null;

			SpiritMultiplayer.Unload();
			AdditiveCallManager.Unload();
			SpiritGlowmask.Unload();
			ParticleHandler.Unload();
			AutoloadMinionDictionary.Unload();
			Mechanics.BackgroundSystem.BackgroundItemManager.Unload();

			if (Boids != null)
				Boids.UnloadContent();

			glitchEffect = null;
			glitchScreenShader = null;
			TrailManager = null;
			GlobalNoise = null;
			GlyphBase.UninitGlyphLookup();
			primitives = null;
			Metaballs = null;
			ShaderDict = new Dictionary<string, Effect>();
			SpiritDetours.Unload();

			PortraitManager.Unload(); //Idk if this is necessary but it seems like a good move - Gabe
									  //UnloadDetours();

			// remove any custom chat tag handlers
			var handlerDict = (ConcurrentDictionary<string, ITagHandler>)typeof(ChatManager).GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			handlerDict.TryRemove("spiritQuest", out var _);
		}

		private void DrawStarGoopTarget(GameTime obj)
		{
			if (!Main.gameMenu && Metaballs != null && Main.graphics.GraphicsDevice != null && Main.spriteBatch != null)
				Metaballs.DrawToTarget(Main.spriteBatch, Main.graphics.GraphicsDevice);
		}

		public static Color StarjinxColor(float Timer)
		{
			float timer = Timer % 10;
			var yellow = new Color(238, 207, 91);
			var orange = new Color(255, 131, 91);
			var pink = new Color(230, 55, 166);

			if (timer < 3)
				return Color.Lerp(yellow, orange, timer / 3);
			else if (timer < 6)
				return Color.Lerp(orange, pink, (timer - 3) / 3);
			else
				return Color.Lerp(pink, yellow, (timer - 6) / 3);
		}

		internal static string GetWeatherRadioText(string key)
		{
			if (MyWorld.ashRain) return "Ashfall";
			else if (MyWorld.aurora) return "Aurora";
			else if (MyWorld.blueMoon) return "Mystic Moon";
			else if (MyWorld.jellySky) return "Jelly Deluge";
			else if (MyWorld.luminousOcean) return "Luminous Seas";
			else if (MyWorld.calmNight) return "Calm Night";
			else if (MyWorld.rareStarfallEvent) return "Starfall";

			return LanguageManager.Instance.GetText(key).Value;
		}

		public override void PostSetupContent()
		{
			GlyphBase.InitializeGlyphLookup();

			BossChecklistDataHandler.RegisterSpiritData(this);
			CrossModContent();

			FinishedContentSetup = true;
		}

		private static void CrossModContent()
		{
			if (ModLoader.TryGetMod("Census", out Mod census))
			{
				census.Call("TownNPCCondition", ModContent.NPCType<Adventurer>(), "Rescue the Adventurer from the Briar after completing your first quest.");
				census.Call("TownNPCCondition", ModContent.NPCType<Gambler>(), "Rescue the Gambler from a Goblin Tower\nIf your world does not have a Goblin Tower, have at least 1 Gold in your inventory");
				census.Call("TownNPCCondition", ModContent.NPCType<Rogue>(), "Rescue the Bandit from the Bandit Hideout\nIf your world does not have a Goblin Tower, have at least 1 Gold in your inventory");
				census.Call("TownNPCCondition", ModContent.NPCType<RuneWizard>(), "Have a Blank Glyph in your inventory");
			}

			if (ModLoader.TryGetMod("Fargowiltas", out Mod fargos))
			{
				// AddSummon, order or value in terms of vanilla bosses, your mod internal name, summon   
				//item internal name, inline method for retrieving downed value, price to sell for in copper
				fargos.Call("AddSummon", 1.4f, "SpiritMod", "ScarabIdol", () => MyWorld.downedScarabeus, 100 * 200);
				fargos.Call("AddSummon", 4.2f, "SpiritMod", "JewelCrown", () => MyWorld.downedAncientFlier, 100 * 200);
				fargos.Call("AddSummon", 5.9f, "SpiritMod", "StarWormSummon", () => MyWorld.downedRaider, 100 * 400);
				fargos.Call("AddSummon", 6.5f, "SpiritMod", "CursedCloth", () => MyWorld.downedInfernon, 100 * 500);
				fargos.Call("AddSummon", 7.3f, "SpiritMod", "DuskCrown", () => MyWorld.downedDusking, 100 * 500);
				fargos.Call("AddSummon", 12.4f, "SpiritMod", "StoneSkin", () => MyWorld.downedAtlas, 100 * 800);
			}
		}

		internal bool _questBookHover;
		internal bool _questBookToggle = false;

		public static void InvokeModifyTransform(SpriteViewMatrix matrix) => OnModifyTransformMatrix?.Invoke(matrix);

		public static float tremorTime;
		public int screenshakeTimer = 0;

		internal static void DrawUpdateToggles()
		{
			Point mousePoint = new Point(Main.mouseX, Main.mouseY);

			Rectangle AutoSellUI_TOGGLERECTANGLE = new Rectangle(494, 280, 39, 39);
			bool AutoSellUI_TOGGLE = AutoSellUI_TOGGLERECTANGLE.Contains(mousePoint);

			if (AutoSellUI_TOGGLE && Main.playerInventory && Main.npcShop > 0)
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.HoverItem = new Item();
				Main.hoverItemName = "Click to quick-sell your items";
			}

			Rectangle Sell_Lock_TOGGLERECTANGLE = new Rectangle(502, 324, 32, 32);
			bool Sell_Lock_TOGGLE = Sell_Lock_TOGGLERECTANGLE.Contains(mousePoint);

			if (Sell_Lock_TOGGLE && Main.playerInventory && Main.npcShop > 0)
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.HoverItem = new Item();
				Main.hoverItemName = "Toggle this to lock quick-sell mechanic\nYou won't be able to use quick-sell while this is toggled";
			}

			Rectangle Sell_Weapons_TOGGLERECTANGLE = new Rectangle(502, 362, 32, 32);
			bool Sell_Weapons_TOGGLE = Sell_Weapons_TOGGLERECTANGLE.Contains(mousePoint);

			if (Sell_Weapons_TOGGLE && Main.playerInventory && Main.npcShop > 0)
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.HoverItem = new Item();
				Main.hoverItemName = "Toggle this to disable the selling of weapons";
			}

			Rectangle Sell_NoValue_TOGGLERECTANGLE = new Rectangle(502, 400, 32, 32);
			bool Sell_NoValue_TOGGLE = Sell_NoValue_TOGGLERECTANGLE.Contains(mousePoint);

			if (Sell_NoValue_TOGGLE && Main.playerInventory && Main.npcShop > 0)
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.HoverItem = new Item();
				Main.hoverItemName = "Toggle this to sell 'no value' items with quick-sell";
			}
		}

		public void DrawEventUI(SpriteBatch spriteBatch)
		{
			if (TideWorld.TheTide && Main.LocalPlayer.ZoneBeach)
			{
				const float Scale = 0.875f;
				const float Alpha = 0.5f;
				const int InternalOffset = 6;
				const int OffsetX = 20;
				const int OffsetY = 20;

				Texture2D EventIcon = Assets.Request<Texture2D>("Textures/InvasionIcons/Depths_Icon", AssetRequestMode.ImmediateLoad).Value;
				Color descColor = new Color(77, 39, 135);
				Color waveColor = new Color(255, 241, 51);

				int width = (int)(200f * Scale);
				int height = (int)(46f * Scale);

				Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 23f), new Vector2(width, height));
				Utils.DrawInvBG(spriteBatch, waveBackground, new Color(63, 65, 151, 255) * 0.785f);

				string waveText = "Wave " + TideWorld.TideWave + " : " + TideWorld.TidePoints + "%";
				Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.Center.X, waveBackground.Y + 5), Color.White, Scale, 0.5f, -0.1f);
				Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.Center.X, waveBackground.Y + waveBackground.Height * 0.75f), TextureAssets.ColorBar.Size());

				var waveProgressAmount = new Rectangle(0, 0, (int)(TextureAssets.ColorBar.Width() * 0.01f * MathHelper.Clamp(TideWorld.TidePoints, 0f, 100f)), TextureAssets.ColorBar.Height());
				var offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * Scale)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * Scale)) * 0.5f);
				spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, null, Color.White * Alpha, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);

				Vector2 descSize = new Vector2(154, 40) * Scale;
				Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 19f), new Vector2(width, height));
				Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), descSize * 0.9f);
				Utils.DrawInvBG(spriteBatch, descBackground, descColor * Alpha);

				int descOffset = (descBackground.Height - (int)(32f * Scale)) / 2;
				var icon = new Rectangle(descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(32 * Scale), (int)(32 * Scale));
				spriteBatch.Draw(EventIcon, icon, Color.White);
				Utils.DrawBorderString(spriteBatch, "The Tide", new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
			}
		}
	}

	internal enum CallContext
	{
		Invalid = -1,
		Downed,
		GlyphGet,
		GlyphSet,
		AddQuest,
		UnlockQuest,
		GetQuestIsUnlocked,
		GetQuestIsActive,
		GetQuestIsCompleted,
		GetQuestRewardsGiven,
		Portrait,
		Events,
		Limit
	}
}