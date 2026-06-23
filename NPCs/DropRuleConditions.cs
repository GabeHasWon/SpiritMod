using SpiritMod.Utilities;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace SpiritMod.NPCs
{
	public class DropRuleConditions
	{
		public class NotDay : IItemDropRuleCondition, IProvideItemConditionDescription
		{
			public bool CanDrop(DropAttemptInfo info)
			{
				if (!info.IsInSimulation)
					return !Main.dayTime;
				return false;
			}

			public bool CanShowItemDropInUI() => true;
			public string GetConditionDescription() => Language.GetTextValue("Mods.SpiritMod.Conditions.NotDay");
		}

		public class Day : IItemDropRuleCondition, IProvideItemConditionDescription
		{
			public bool CanDrop(DropAttemptInfo info)
			{
				if (!info.IsInSimulation)
					return Main.dayTime;
				return false;
			}

			public bool CanShowItemDropInUI() => true;
			public string GetConditionDescription() => Language.GetTextValue("Mods.SpiritMod.Conditions.Day");
		}

		public class BossDowned : IItemDropRuleCondition, IProvideItemConditionDescription
		{
			public enum Bosses : int
			{
				Evil_Boss,
				Any_Mech,
				King_Slime,
				Scarabeus,
				Skeletron,
				SingularCutoff, //Keep this after all non-"the" bosses (i.e. after Skeletron but before THE Eye of Cthulhu)
				Eye_Of_Cthulhu,
				Moon_Jelly_Wizard,
				Queen_Bee,
			}

			readonly Bosses boss = Bosses.King_Slime;

			public BossDowned(Bosses boss)
			{
				this.boss = boss;
			}

			public bool CanDrop(DropAttemptInfo info)
			{
				if (!info.IsInSimulation)
				{
					return boss switch
					{
						Bosses.King_Slime => NPC.downedSlimeKing,
						Bosses.Scarabeus => MyWorld.DownedScarabeus,
						Bosses.Eye_Of_Cthulhu => NPC.downedBoss1,
						Bosses.Evil_Boss => NPC.downedBoss2,
						Bosses.Skeletron => NPC.downedBoss3,
						Bosses.Queen_Bee => NPC.downedQueenBee,
						Bosses.Moon_Jelly_Wizard => MyWorld.DownedMoonWizard,
						Bosses.Any_Mech => NPC.downedMechBossAny,
						_ => false,
					};
				}
				return false;
			}

			public bool CanShowItemDropInUI() => true;

			public string GetConditionDescription()
			{
				string bossstr = GetBossDisplayName();
				string prefix = ShouldUseThePrefix() ? Language.GetTextValue("Mods.SpiritMod.Conditions.PrefixThe") : "";
    
				return Language.GetTextValue("Mods.SpiritMod.Conditions.DefBoss", prefix + bossstr);
			}

			private string GetBossDisplayName()
			{
				return boss switch
				{
					Bosses.Moon_Jelly_Wizard => Language.GetTextValue("Mods.SpiritMod.NPCs.MoonWizard.DisplayName"),
					Bosses.Scarabeus => Language.GetTextValue("Mods.SpiritMod.NPCs.Scarabeus.DisplayName"),
					Bosses.King_Slime => Language.GetTextValue("NPCName.KingSlime"),
					Bosses.Skeletron => Language.GetTextValue("NPCName.SkeletronHead"),
					Bosses.Queen_Bee => Language.GetTextValue("NPCName.QueenBee"),
					Bosses.Eye_Of_Cthulhu => Language.GetTextValue("NPCName.EyeOfCthulhu"),
					Bosses.Evil_Boss => WorldGen.crimson ? Language.GetTextValue("NPCName.BrainofCthulhu") : Language.GetTextValue("NPCName.EaterofWorldsHead"),
					Bosses.Any_Mech => Language.GetTextValue("Mods.SpiritMod.Conditions.AnyMech"),
					_ => boss.ToString().Replace("_", " ")
				};
			}

			private bool ShouldUseThePrefix()
			{
				return boss != Bosses.Any_Mech && (int)boss >= (int)Bosses.SingularCutoff;
			}
		}

		public class InBiome : IItemDropRuleCondition, IProvideItemConditionDescription
		{
			public enum Biome : int
			{
				AnyPurity,
				SurfacePurity,
				UndergroundPurity,
				Snow,
				Asteroid
			}

			readonly Biome biome = Biome.AnyPurity;

			public InBiome(Biome biome)
			{
				this.biome = biome;
			}

			public bool CanDrop(DropAttemptInfo info)
			{
				if (!info.IsInSimulation)
				{
					return biome switch
					{
						Biome.AnyPurity => info.player.ZonePurity,
						Biome.SurfacePurity => info.player.ZonePurity && info.player.ZoneOverworldHeight,
						Biome.UndergroundPurity => info.player.ZonePurity && info.player.ZoneDirtLayerHeight,
						Biome.Asteroid => info.player.ZoneAsteroid(),
						Biome.Snow => info.player.ZoneSnow,
						_ => false,
					};
				}
				return false;
			}

			public bool CanShowItemDropInUI() => true;

			public string GetConditionDescription()
			{
				string def = Language.GetTextValue("Mods.SpiritMod.Conditions.DropFrom");
				if (biome.ToString().Contains("Purity")) 
					return def + Language.GetTextValue("Mods.SpiritMod.Conditions.Purity");
				else
					return def + Language.GetTextValue("Mods.SpiritMod.Conditions." + biome.ToString());
			}
		}

		public class PlayerConditional : IItemDropRuleCondition, IProvideItemConditionDescription
		{
			public bool CanDrop(DropAttemptInfo info)
			{
				if (!info.IsInSimulation)
					return canDrop(info.player);
				return false;
			}

			public Func<Player, bool> canDrop;
			public readonly string condition = "";

			public PlayerConditional(string cond, Func<Player, bool> func)
			{
				condition = cond;
				canDrop = func;
			}

			public bool CanShowItemDropInUI() => true;
			public string GetConditionDescription() => condition;
		}

		public class NPCConditional : IItemDropRuleCondition, IProvideItemConditionDescription
		{
			public bool CanDrop(DropAttemptInfo info)
			{
				if (!info.IsInSimulation)
					return canDrop(info.npc);
				return false;
			}

			public Func<NPC, bool> canDrop;
			public readonly string condition = "";

			public NPCConditional(string cond, Func<NPC, bool> func)
			{
				condition = cond;
				canDrop = func;
			}

			public bool CanShowItemDropInUI() => true;
			public string GetConditionDescription() => condition;
		}
	}
}
