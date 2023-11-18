using SpiritMod.Items.Glyphs;
using SpiritMod.NPCs.Boss;
using SpiritMod.NPCs.Boss.Atlas;
using SpiritMod.NPCs.Boss.Dusking;
using SpiritMod.NPCs.Boss.Infernon;
using SpiritMod.NPCs.Boss.MoonWizard;
using SpiritMod.NPCs.Boss.ReachBoss;
using SpiritMod.NPCs.Boss.Scarabeus;
using SpiritMod.NPCs.Boss.SteamRaider;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.NPCs;

/// <summary>
/// Tracks if bosses are downed and automatically applies them to a list.<br/>
/// Use <see cref="IsBossDowned(NPC)"/> (or its overload) to check if any boss in any mod has been flagged as down.
/// </summary>
internal class BossDownedTracker : GlobalNPC
{
	internal static Dictionary<string, bool> Downed = new();

	public static bool IsBossDowned(NPC npc) => Downed.ContainsKey(GetBossKey(npc)) && Downed[GetBossKey(npc)];
	public static bool IsBossDowned<T>() where T: ModNPC => Downed.ContainsKey(GetBossKey<T>()) && Downed[GetBossKey<T>()];

	public static string GetBossKey(NPC npc) 
	{
		if (npc.type < NPCID.Count)
			return npc.type.ToString();

		return npc.ModNPC.Mod.Name + "/" + npc.ModNPC.Name;
	}

	public static string GetBossKey<T>() where T : ModNPC
	{
		NPC npc = ContentSamples.NpcsByNetId[ModContent.NPCType<T>()];
		return GetBossKey(npc);
	}

	/// <summary>
	/// Adds the glyph drop to all bosses that have never been downed.
	/// </summary>
	/// <param name="globalLoot"></param>
	public override void ModifyGlobalLoot(GlobalLoot globalLoot)
	{
		LeadingConditionRule isBoss = new(new Conditions.LegacyHack_IsABoss());
		isBoss.OnSuccess(ItemDropRule.ByCondition(new CanDropGlyph(), ModContent.ItemType<Glyph>()));
		globalLoot.Add(isBoss);
	}

	/// <summary>
	/// Sets the boss as downed and syncs the kill.
	/// </summary>
	public override void OnKill(NPC npc)
	{
		if (npc.boss)
		{
			Downed[GetBossKey(npc)] = true;

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.WorldData);
		}
	}
}

public class BossDownedTrackingIO : ModSystem
{
	public override void LoadWorldData(TagCompound tag)
	{
		int count = BossDownedTracker.Downed.Where(x => x.Value).Count();
		tag.Add("downedCount", count);

		int index = 0;

		foreach (var pair in BossDownedTracker.Downed)
		{
			if (pair.Value)
				tag.Add("downed" + index++, pair.Key);
		}
	}

	public override void SaveWorldData(TagCompound tag)
	{
		int count = tag.GetInt("downedCount");

		for (int i = 0; i < count; ++i)
			BossDownedTracker.Downed.Add(tag.GetString("downed" + i), true);
	}

	/// <summary>
	/// Syncs boss downed, ported from old impl. in MyWorld.NetRecieve
	/// </summary>
	public static void HandleBossSyncing(BitsByte bosses1)
	{
		BossDownedTracker.Downed[BossDownedTracker.GetBossKey<Scarabeus>()] = bosses1[0];
		BossDownedTracker.Downed[BossDownedTracker.GetBossKey<AncientFlyer>()] = bosses1[1];
		BossDownedTracker.Downed[BossDownedTracker.GetBossKey<SteamRaiderHead>()] = bosses1[2];
		BossDownedTracker.Downed[BossDownedTracker.GetBossKey<Infernon>()] = bosses1[3];
		BossDownedTracker.Downed[BossDownedTracker.GetBossKey<Dusking>()] = bosses1[4];
		BossDownedTracker.Downed[BossDownedTracker.GetBossKey<Atlas>()] = bosses1[5];
		BossDownedTracker.Downed[BossDownedTracker.GetBossKey<ReachBoss1>()] = bosses1[6];
		BossDownedTracker.Downed[BossDownedTracker.GetBossKey<MoonWizard>()] = bosses1[7];
	}
}

/// <summary>
/// Simple condition for checking if the boss has ever been downed.
/// </summary>
public class CanDropGlyph : IItemDropRuleCondition, IProvideItemConditionDescription
{
	public bool CanDrop(DropAttemptInfo info) => !BossDownedTracker.IsBossDowned(info.npc);
	public bool CanShowItemDropInUI() => true;
	public string GetConditionDescription() => Language.GetTextValue("Mods.SpiritMod.Condition.FirstDown");
}