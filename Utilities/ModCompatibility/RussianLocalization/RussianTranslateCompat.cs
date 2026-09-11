using SpiritMod.Items.Accessory.DarkfeatherVisage;
using SpiritMod.Items.Armor;
using SpiritMod.Items.Armor.BotanistSet;
using SpiritMod.Items.Armor.Daybloom;
using SpiritMod.Items.Armor.LeatherArmor;
using SpiritMod.Items.Armor.WayfarerSet;
using SpiritMod.Items.BossLoot.AtlasDrops.PrimalstoneArmor;
using SpiritMod.Items.BossLoot.AvianDrops.ApostleArmor;
using SpiritMod.Items.BossLoot.DuskingDrops.DuskArmor;
using SpiritMod.Items.BossLoot.InfernonDrops.InfernonArmor;
using SpiritMod.Items.BossLoot.MoonWizardDrops.JellynautHelmet;
using SpiritMod.Items.BossLoot.ScarabeusDrops.ChitinArmor;
using SpiritMod.Items.BossLoot.StarplateDrops.StarArmor;
using SpiritMod.Items.Sets.BismiteSet.BismiteArmor;
using SpiritMod.Items.Sets.BloodcourtSet.BloodCourt;
using SpiritMod.Items.Sets.CascadeSet.Armor;
using SpiritMod.Items.Sets.CryoliteSet.CryoliteArmor;
using SpiritMod.Items.Sets.FloatingItems.Driftwood.DriftwoodArmor;
using SpiritMod.Items.Sets.FloranSet.FloranArmor;
using SpiritMod.Items.Sets.FrigidSet.FrigidArmor;
using SpiritMod.Items.Sets.GraniteSet.GraniteArmor;
using SpiritMod.Items.Sets.HuskstalkSet.ElderbarkArmor;
using SpiritMod.Items.Sets.MarbleSet.MarbleArmor;
using SpiritMod.Items.Sets.RunicSet.RunicArmor;
using SpiritMod.Items.Sets.SeraphSet.SeraphArmor;
using SpiritMod.Items.Sets.SlagSet.FieryArmor;
using SpiritMod.Items.Sets.SpiritSet.SpiritArmor;
using SpiritMod.Items.Sets.TideDrops.StreamSurfer;
using System;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Utilities.ModCompatibility.RussianLocalization;

internal class RussianTranslateCompat : ModSystem
{
	public static bool IsNotRussianFont { get; private set; }
	public override void PostSetupContent()
	{
		if (Language.ActiveCulture.Name != "ru-RU")
			return;

		IsNotRussianFont = true;

		if (!ModLoader.TryGetMod("CalamityRuTranslate", out Mod tru))
			return;

		Type configType = tru.Code.GetType("CalamityRuTranslate.Core.Config.TRuConfig");
		object configInstance = configType?.GetField("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

		if (configType?.GetField("NewRussianTerrariaFont", BindingFlags.Public | BindingFlags.Instance)?.GetValue(configInstance) is bool value)
			IsNotRussianFont = !value;

		tru.Call("AddFeminineItems", Mod, new[]
		{
			//Accessories
			"FeralConcoction",
			"WinterHat",
			"Bauble",
			"DesertSlab",
			"AssassinMagazine",
			"Unstable_Tesla_Coil",
			"Dartboard",
			"Rabbit_Foot",
			"OpalFrogItem",
			"IchorPendant",
			"HellEater",
			"SwiftRune",
			"WheezerScale",
			"ElectricGuitar",
			"FallenAngel",
			//Weapons
			"MadHat",
			"SoaringScapula",
			"TalonPiercer",
			"Shadowmoor",
			"ShadowSphere",
			"InfernalStaff",
			"InfernalSword",
			"Handball",
			"ChitinPickaxe",
			"AstralLens",
			"BlizzardEdge",
			"HauntingClaw",
			"SpookyScythe",
			"BismitePickaxe",
			"BismiteSpear",
			"Morningtide",
			"Basking_Shark",
			"Reef_Wrath",
			"BoneClub",
			"FloranBludgeon",
			"WoodenClub",
			"CryoPick",
			"ClatterMace",
			"FloranPick",
			"FrostSpine",
			"GraniteFlail",
			"GranitePickaxe",
			"KineticRailgun",
			"LadyLuck",
			"PolymorphGun",
			"Scattergun",
			"LibertyItem",
			"Sharkbones",
			"ArcLash",
			"AkaviriStaff",
			"MagicDeck",
			"NightSkyStaff",
			"NightStaff",
			"TrueDarkStaff",
			"MarblePick",
			"ArtemisHunt",
			"ClawCannon",
			"SpiritRune",
			"BambooHalberd",
			"PhantomArc",
			"SpiritPickaxe",
			"SpiritSaber",
			"ToucaneItem",
			"MimeSummon",
			"SpiritStar",
			"CoconutGun",
			"MagicConch",
			"ThornDevilfish"
		});

		tru.Call("AddNeuterItems", Mod, new[]
		{
			//Accessories
			"ShieldCore",
			"GoldenApple",
			"MagnifyingGlass",
			"Ukelele",
			"SpectreRing",
			"ArcaneNecklace",
			"FourOfAKind",
			//Weapons
			"Earthshatter",
			"Talonginus",
			"InfernalJavelin",
			"DuskfeatherDagger",
			"RageBlazeDecapitator",
			"HeartilleryBeacon",
			"IcySpear",
			"OakHeart",
			"BreathOfTheZephyr",
			"FieryMagicLauncher",
			"SpiritSpear",
			"TikiJavelin",
			"RealityQuill",
			"ValkyrieSpear",
			"ClatterSpear",
			"Tao"
		});

		tru.Call("AddPluralItems", Mod, new[]
		{
			//Accessories
			"FrigidGloves",
			"Matriarch_Wings",
			"SpiritWings",
			"LeatherGlove",
			"SkarmoryWings",
			"NetherWings",
			"LeatherBoots",
			"HighGravityBoots",
			"TechBoots",
			"ExplorerTreads",
			//Weapons
			"FearsomeFork",
			"SevenSins",
			"JadeDao",
			"BetrayersChains"
		});

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<DriftwoodHelmet>(), () =>
		Language.GetTextValue("Mods.SpiritMod.SetBonuses.Driftwood"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<ElderbarkHead>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Elderbark"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<WayfarerHead>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Wayfarer"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<LeatherHood>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Leather"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<BotanistHat>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Botanist"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<DaybloomHead>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Daybloom"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<RogueHood>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Rogue"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<FrigidHelm>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Frigid", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN")));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<BismiteHelmet>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Bismite"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<CascadeHelmet>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Cascade"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<FHelmet>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Floran"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<ChitinHelmet>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Chitin"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<BloodCourtHead>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.BloodCourt", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN")));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<GraniteHelm>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Granite", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN")));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<MarbleHelm>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Marble"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<TalonHeaddress>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Apostle"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<ObsidiusHelm>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Slag", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN")));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<CryoliteHead>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Cryolite"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<StarMask>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Starplate", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"), 12));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<StreamSurferHelmet>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.StreamSurfer", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN")));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<SeraphHelm>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Seraph"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<InfernalVisor>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Infernal"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<DuskHood>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Dusk"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<RunicHood>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Runic"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<SpiritHeadgear>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Spirit"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<PrimalstoneFaceplate>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.Primalstone"));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<JellynautBubble>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.JellynautBubble", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN")));

		tru.Call("AddArmorSetBonusPreview", ModContent.ItemType<DarkfeatherVisage>(), () =>
			Language.GetTextValue("Mods.SpiritMod.SetBonuses.DarkfeatherVisage"));
	}
}
