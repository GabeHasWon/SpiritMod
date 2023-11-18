using SpiritMod.Mechanics.QuestSystem.Quests;
using SpiritMod.Mechanics.QuestSystem;
using Terraria;
using SpiritMod.Biomes;

namespace SpiritMod.NPCs;

internal class SpiritConditions
{
	public static Condition InBriar = new("Mods.SpiritMod.Condition.InBriar", () => Main.LocalPlayer.InModBiome<BriarSurfaceBiome>() || Main.LocalPlayer.InModBiome<BriarUndergroundBiome>());
	public static Condition InAsteroids = new("Mods.SpiritMod.Condition.InAsteroids", Main.LocalPlayer.InModBiome<AsteroidBiome>);
	public static Condition InSpirit = new("Mods.SpiritMod.Condition.InSpirit", () => Main.LocalPlayer.InModBiome<SpiritSurfaceBiome>() || Main.LocalPlayer.InModBiome<SpiritUndergroundBiome>());
	public static Condition VoyagerDown = new("Mods.SpiritMod.Conditions.VoyagerDown", () => MyWorld.DownedStarplate);
	public static Condition ScarabDown = new("Mods.SpiritMod.Conditions.ScarabDown", () => MyWorld.DownedScarabeus);
	public static Condition MJWDown = new("Mods.SpiritMod.Conditions.ScarabDown", () => MyWorld.DownedMoonWizard);
	public static Condition VinewrathDown = new("Mods.SpiritMod.Conditions.VinewrathDown", () => MyWorld.DownedVinewrath);
	public static Condition DuskingDown = new("Mods.SpiritMod.Conditions.DuskingDown", () => MyWorld.DownedVinewrath);

	//Quest conditions
	public static Condition FirstAdventureFinished = new("Mods.SpiritMod.Conditions.FirstAdventureFinished", () => QuestManager.GetQuest<FirstAdventure>().IsCompleted);
	public static Condition FishyBusinessFinished = new("Mods.SpiritMod.Conditions.FishyBusinessFinished", () => QuestManager.GetQuest<AnglerStatueQuest>().IsCompleted);
	public static Condition SanctuaryNightlightsFinished = new("Mods.SpiritMod.Conditions.SanctuaryNightlightsFinished", () => QuestManager.GetQuest<CritterCaptureFloater>().IsCompleted);
	public static Condition SpaceRocksFinished = new("Mods.SpiritMod.Conditions.SpaceRocksFinished", () => QuestManager.GetQuest<ExplorerQuestAsteroid>().IsCompleted);
	public static Condition RockyRoadFinished = new("Mods.SpiritMod.Conditions.RockyRoadFinished", () => QuestManager.GetQuest<ExplorerQuestGranite>().IsCompleted);
	public static Condition ForgottenCivilizationsFinished = new("Mods.SpiritMod.Conditions.ForgottenCivilizationsFinished", () => QuestManager.GetQuest<ExplorerQuestMarble>().IsCompleted);
	public static Condition HiveHuntingFinished = new("Mods.SpiritMod.Conditions.HiveHuntingFinished", () => QuestManager.GetQuest<ExplorerQuestHive>().IsCompleted);
	public static Condition GlowingAGardenFinished = new("Mods.SpiritMod.Conditions.GlowingAGardenFinished", () => QuestManager.GetQuest<ExplorerQuestMushroom>().IsCompleted);
	public static Condition ManicMageFinished = new("Mods.SpiritMod.Conditions.ManicMageFinished", () => QuestManager.GetQuest<ManicMage>().IsCompleted);
	public static Condition SkyHighFinished = new("Mods.SpiritMod.Conditions.SkyHighFinished", () => QuestManager.GetQuest<SkyHigh>().IsCompleted);
	public static Condition UnholyUndertakingFinished = new("Mods.SpiritMod.Conditions.UnholyUndertakingFinished", () => QuestManager.GetQuest<ZombieOriginQuest>().IsCompleted);
	public static Condition DecrepitDepthsFinished = new("Mods.SpiritMod.Conditions.DecrepitDepthsFinished", () => QuestManager.GetQuest<DecrepitDepths>().IsCompleted);
	public static Condition TowerOrHideoutQuestFinished = new("Mods.SpiritMod.Conditions.TowerOrHideoutQuestFinished", () => 
		QuestManager.GetQuest<BreakingAndEntering>().IsCompleted || QuestManager.GetQuest<FriendSafari>().IsCompleted);
	public static Condition ItsNoSalmonFinished = new("Mods.SpiritMod.Conditions.ItsNoSalmonFinished", () => QuestManager.GetQuest<ItsNoSalmon>().IsCompleted);
	public static Condition SanctuarySporeSalvageFinished = new("Mods.SpiritMod.Conditions.SanctuarySporeSalvageFinished", () => QuestManager.GetQuest<SporeSalvage>().IsCompleted);
	public static Condition WhyZombiesFinished = new("Mods.SpiritMod.Conditions.WhyZombiesFinished", () => QuestManager.GetQuest<SlayerQuestDrBones>().IsCompleted);
	public static Condition BeneathTheIceFinished = new("Mods.SpiritMod.Conditions.BeneathTheIceFinished", () => QuestManager.GetQuest<IceDeityQuest>().IsCompleted);
	public static Condition FloweryFiendsFinished = new("Mods.SpiritMod.Conditions.FloweryFiendsFinished", () => QuestManager.GetQuest<SlayerQuestBriar>().IsCompleted);
}
