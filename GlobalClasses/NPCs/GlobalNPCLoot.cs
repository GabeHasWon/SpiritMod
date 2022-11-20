using SpiritMod.Items.Accessory;
using SpiritMod.Items.Accessory.MageTree;
using SpiritMod.Items.Armor.Masks;
using SpiritMod.Items.Consumable;
using SpiritMod.Items.DonatorItems;
using SpiritMod.Items.Equipment;
using SpiritMod.Items.Glyphs;
using SpiritMod.Items.Pets;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Items.Placeable.IceSculpture;
using SpiritMod.Items.Sets.FrigidSet;
using SpiritMod.Items.Sets.PirateStuff;
using SpiritMod.Items.Sets.ReefhunterSet;
using SpiritMod.Items.Sets.SummonsMisc.PigronStaff;
using SpiritMod.Items.Weapon.Magic;
using SpiritMod.Items.Weapon.Yoyo;
using SpiritMod.NPCs;
using SpiritMod.NPCs.Boss.ReachBoss;
using SpiritMod.NPCs.Winterborn;
using SpiritMod.NPCs.WinterbornHerald;
using SpiritMod.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

using ContentItems = SpiritMod.Items; //Clears up Items and GlobalClasses.Items issue

namespace SpiritMod.GlobalClasses.NPCs
{
	internal class GlobalNPCLoot : GlobalNPC
	{
		public override void ModifyGlobalLoot(GlobalLoot globalLoot)
		{
			LeadingConditionRule glyphChance = new LeadingConditionRule(new DropRuleConditions.NPCConditional("Rarely", (npc) => !npc.SpawnedFromStatue && npc.CanDamage() && npc.type != ModContent.NPCType<ExplodingSpore>()));
			glyphChance.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Glyph>(), 750));
			globalLoot.Add(glyphChance);

			LeadingConditionRule inAsteroids = new LeadingConditionRule(new DropRuleConditions.InBiome(DropRuleConditions.InBiome.Biome.Asteroid));
			inAsteroids.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ContentItems.Sets.GunsMisc.Blaster.Blaster>(), 60));
			globalLoot.Add(inAsteroids);

			LeadingConditionRule wearingArcaneNecklace = new LeadingConditionRule(new DropRuleConditions.PlayerConditional("Wearing the Arcane Necklace and is using a magic weapon", (player) => player.HasAccessory<ArcaneNecklace>() && player.HeldItem.IsMagic() && player.statMana < player.statManaMax2));
			wearingArcaneNecklace.OnSuccess(ItemDropRule.Common(ItemID.Star, 5));
			globalLoot.Add(wearingArcaneNecklace);

			LeadingConditionRule floranSet = new LeadingConditionRule(new DropRuleConditions.PlayerConditional("Wearing the full Floran set", (player) => player.GetSpiritPlayer().floranSet));
			floranSet.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RawMeat>(), 9));
			globalLoot.Add(floranSet);

			LeadingConditionRule vitaStoneEquipped = new LeadingConditionRule(new DropRuleConditions.PlayerConditional("Wearing the Vitality Stone (or an upgrade)", (player) => player.GetSpiritPlayer().vitaStone));
			vitaStoneEquipped.OnSuccess(ItemDropRule.Common(ItemID.Heart, 9));
			globalLoot.Add(vitaStoneEquipped);
		}

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			DropLoot(npcLoot, 50, 50, ModContent.ItemType<SolarRattle>(), npc, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider);
			DropLoot(npcLoot, 50, 50, ModContent.ItemType<ContentItems.Weapon.Summon.EngineeringRod>(), npc, NPCID.GrayGrunt, NPCID.RayGunner, NPCID.BrainScrambler);
			DropLoot(npcLoot, 5, 5, ModContent.ItemType<ContentItems.Sets.BowsMisc.Eyeshot.Eyeshot>(), npc, NPCID.EyeofCthulhu);
			DropLoot(npcLoot, 2, 2, ModContent.ItemType<Martian>(), npc, NPCID.MartianSaucer);
			DropLoot(npcLoot, 1, 1, Main.rand.NextBool(2) ? ModContent.ItemType<Ancient>() : ModContent.ItemType<CultistScarf>(), npc, NPCID.CultistBoss);
			DropLoot(npcLoot, 50, 50, ModContent.ItemType<IchorPendant>(), npc, NPCID.IchorSticker);
			DropLoot(npcLoot, 1, 1, ModContent.ItemType<Typhoon>(), npc, NPCID.DukeFishron);
			DropLoot(npcLoot, 6, 4, ModContent.ItemType<PigronStaffItem>(), npc, NPCID.PigronCorruption, NPCID.PigronHallow, NPCID.PigronCrimson);
			DropLoot(npcLoot, 18, 18, ModContent.ItemType<TheFireball>(), npc, NPCID.FireImp);
			DropLoot(npcLoot, 50, 50, ModContent.ItemType<CursedPendant>(), npc, NPCID.Clinger);
			DropLoot(npcLoot, 50, 50, ModContent.ItemType<MagnifyingGlass>(), npc, NPCID.DemonEye, NPCID.DemonEye2, NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship);
			DropLoot(npcLoot, 1, 1, ModContent.ItemType<PirateKey>(), npc, NPCID.PirateShip);
			DropLoot(npcLoot, 6, 6, ModContent.ItemType<ContentItems.Sets.SummonsMisc.SanguineFlayer.SanguineFlayerItem>(), npc, NPCID.BigMimicCrimson);
			DropLoot(npcLoot, 6, 5, ModContent.ItemType<ContentItems.Accessory.OpalFrog.OpalFrogItem>(), npc, NPCID.BigMimicHallow);
			DropLoot(npcLoot, 15, 15, ModContent.ItemType<ContentItems.Sets.SwordsMisc.CurseBreaker.CurseBreaker>(), npc, NPCID.RedDevil);
			DropLoot(npcLoot, 50, 50, ModContent.ItemType<ChaosCrystal>(), npc, NPCID.ChaosElemental);
			DropLoot(npcLoot, 100, 80, ModContent.ItemType<ContentItems.Weapon.Thrown.PiecesOfEight.PiecesOfEight>(), npc, NPCID.PirateDeckhand);
			DropLoot(npcLoot, 23, 23, ModContent.ItemType<SaucerBeacon>(), npc, NPCID.MartianOfficer);
			DropLoot(npcLoot, 35, 35, ModContent.ItemType<SnapperHat>(), npc, NPCID.Crawdad, NPCID.Crawdad2);
			DropLoot(npcLoot, 50, 50, ModContent.ItemType<TrapperGlove>(), npc, NPCID.ManEater);
			DropLoot(npcLoot, 500, 500, ModContent.ItemType<SnakeStaff>(), npc, NPCID.Lihzahrd, NPCID.LihzahrdCrawler);
			DropLoot(npcLoot, 1, 1, ModContent.ItemType<Glyph>(), npc, NPCID.Tim, NPCID.RuneWizard);
			DropLoot(npcLoot, 100, 100, ModContent.ItemType<ContentItems.Consumable.Potion.BottomlessAle>(), npc, NPCID.Pixie);
			DropLoot(npcLoot, 250, 200, ModContent.ItemType<ContentItems.Accessory.Ukelele.Ukelele>(), npc, NPCID.AngryNimbus);
			DropLoot(npcLoot, 100, 95, ModContent.ItemType<ContentItems.Accessory.BowSummonItem.BowSummonItem>(), npc, NPCID.GoblinArcher);
			DropLoot(npcLoot, 100, 100, ModContent.ItemType<ContentItems.Accessory.FlyingFishFin.Flying_Fish_Fin>(), npc, NPCID.FlyingFish);
			DropLoot(npcLoot, 3, 3, ModContent.ItemType<ContentItems.Accessory.SeaSnailVenom.Sea_Snail_Poison>(), npc, NPCID.SeaSnail);
			DropLoot(npcLoot, 100, 100, ModContent.ItemType<ContentItems.Sets.SlingHammerSubclass.PossessedHammer>(), npc, NPCID.PossessedArmor);
			DropLoot(npcLoot, 90, 75, ModContent.ItemType<GoblinSorcererStaff>(), npc, NPCID.GoblinSorcerer);
			DropLoot(npcLoot, 110, 95, ModContent.ItemType<ContentItems.Sets.ClubSubclass.BoneClub>(), npc, NPCID.AngryBones, NPCID.AngryBonesBig, NPCID.AngryBonesBigMuscle);
			DropLoot(npcLoot, 45, 45, ModContent.ItemType<ContentItems.Sets.BowsMisc.StarSpray.StarlightBow>(), npc, NPCID.Harpy);
			DropLoot(npcLoot, 45, 45, ModContent.ItemType<ContentItems.Sets.MagicMisc.ZephyrBreath.BreathOfTheZephyr>(), npc, NPCID.Harpy);
			DropLoot(npcLoot, 1, 1, ModContent.ItemType<TimScroll>(), npc, NPCID.Tim);
			DropLoot(npcLoot, 30, 30, ItemID.SnowGlobe, npc, NPCID.IcyMerman);
			DropLoot(npcLoot, 14, 10, ModContent.ItemType<SweetThrow>(), npc, NPCID.QueenBee);
			DropLoot(npcLoot, 80, 60, ModContent.ItemType<InfernalPact>(), npc, NPCID.Lavabat, NPCID.RedDevil);
			DropLoot(npcLoot, 150, 150, ModContent.ItemType<IceVikingSculpture>(), npc, NPCID.UndeadViking);
			DropLoot(npcLoot, 150, 150, ModContent.ItemType<IceFlinxSculpture>(), npc, NPCID.SnowFlinx);
			DropLoot(npcLoot, 150, 150, ModContent.ItemType<IceBatSculpture>(), npc, NPCID.IceBat);
			DropLoot(npcLoot, 120, 110, ModContent.ItemType<ContentItems.Accessory.RabbitFoot.Rabbit_Foot>(), npc, NPCID.Bunny);
			DropLoot(npcLoot, 150, 150, ModContent.ItemType<WinterbornSculpture>(), npc, ModContent.NPCType<WinterbornMelee>(), ModContent.NPCType<WinterbornMagic>());
			DropLoot(npcLoot, 5, 5, ModContent.ItemType<ContentItems.Consumable.Potion.BottomlessHealingPotion>(), npc, NPCID.Mimic);
			DropLoot(npcLoot, 42, 42, ModContent.ItemType<ContentItems.Sets.MagicMisc.TerraStaffTree.DungeonStaff>(), npc, NPCID.DarkCaster);
			DropLoot(npcLoot, 200, 200, ModContent.ItemType<ContentItems.Sets.GunsMisc.Swordsplosion.Swordsplosion>(), npc, NPCID.RustyArmoredBonesAxe, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesSwordNoArmor, NPCID.BlueArmoredBones, NPCID.BlueArmoredBonesMace,
				NPCID.BlueArmoredBonesNoPants, NPCID.BlueArmoredBonesSword, NPCID.HellArmoredBones, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSword);
			DropLoot(npcLoot, 20, 16, ModContent.ItemType<FrostGiantBelt>(), npc, NPCID.UndeadViking);
			DropLoot(npcLoot, 13, 13, ModContent.ItemType<ContentItems.Sets.GunsMisc.CaptainsRegards.CaptainsRegards>(), npc, NPCID.PirateShip, NPCID.PirateCaptain);
			DropLoot(npcLoot, 13, 13, ModContent.ItemType<PirateCrate>(), npc, NPCID.PirateShip);
			DropLoot(npcLoot, 175, 175, ModContent.ItemType<ContentItems.Sets.BowsMisc.Morningtide.Morningtide>(), npc, NPCID.HellArmoredBones, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSword);

			DownedBossLoot(npcLoot, DropRuleConditions.BossDowned.Bosses.Scarabeus, 40, 40, ModContent.ItemType<DesertSlab>(), npc, NPCID.TombCrawlerHead);
			DownedBossLootCommon(npcLoot, DropRuleConditions.BossDowned.Bosses.Skeletron, 40, ModContent.ItemType<ContentItems.Sets.SlagSet.CarvedRock>(), 1, 3, npc, NPCID.Demon);

			DropLoot(npcLoot, 1, 1, ModContent.ItemType<PrintPrime>(), 1, 2, npc, NPCID.SkeletronPrime);
			DropLoot(npcLoot, 1, 1, ModContent.ItemType<BlueprintTwins>(), 1, 2, npc, NPCID.Retinazer, NPCID.Spazmatism);
			DropLoot(npcLoot, 1, 1, ModContent.ItemType<PrintProbe>(), 1, 2, npc, NPCID.TheDestroyer);
			DropLoot(npcLoot, 3, 3, ModContent.ItemType<FrigidFragment>(), 1, 2, npc, NPCID.ZombieEskimo, NPCID.IceSlime, NPCID.IceBat, NPCID.ArmoredViking);
			DropLoot(npcLoot, 1, 1, ModContent.ItemType<FrigidFragment>(), 1, 2, npc, NPCID.SpikedIceSlime, NPCID.ArmedZombieEskimo);
			DropLoot(npcLoot, 2, 2, ModContent.ItemType<ContentItems.Material.OldLeather>(), 1, 2, npc, NPCID.Zombie, NPCID.BaldZombie, NPCID.SlimedZombie, NPCID.SwampZombie, NPCID.TwiggyZombie, NPCID.ZombieRaincoat, NPCID.PincushionZombie);
			DropLoot(npcLoot, 1, 1, ModContent.ItemType<IridescentScale>(), 1, 2, npc, NPCID.Shark);

			if (new int[] { NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail }.Contains(npc.type)) //Drops for EoW here
			{
				LeadingConditionRule leadingConditionRule = new(new Conditions.LegacyHack_IsABoss());
				leadingConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ContentItems.Sets.SpearsMisc.RotScourge.EoWSpear>(), 1));
				npcLoot.Add(leadingConditionRule);
			}
		}

		/// <summary>Adds loot that only drops after a certain boss has been defeated. Uses NormalvsExpert if the condition is true.</summary>
		/// <param name="npcLoot"></param>
		/// <param name="boss"></param>
		/// <param name="normal"></param>
		/// <param name="expert"></param>
		/// <param name="itemID"></param>
		/// <param name="npc"></param>
		/// <param name="types"></param>
		private void DownedBossLoot(NPCLoot npcLoot, DropRuleConditions.BossDowned.Bosses boss, int normal, int expert, int itemID, NPC npc, params int[] types)
		{
			if (types.Contains(npc.type))
			{
				LeadingConditionRule rule = new LeadingConditionRule(new DropRuleConditions.BossDowned(boss));
				rule.OnSuccess(ItemDropRule.NormalvsExpert(itemID, normal, expert));
				npcLoot.Add(rule);
			}
		}

		/// <summary>Adds loot that only drops after a certain boss has been defeated. Uses NormalvsExpert if the condition is true.</summary>
		/// <param name="npcLoot"></param>
		/// <param name="boss"></param>
		/// <param name="normal"></param>
		/// <param name="expert"></param>
		/// <param name="itemID"></param>
		/// <param name="npc"></param>
		/// <param name="types"></param>
		private void DownedBossLoot(NPCLoot npcLoot, DropRuleConditions.BossDowned.Bosses boss, int normal, int expert, int itemID, int minStack, int maxStack, NPC npc, params int[] types)
		{
			if (types.Contains(npc.type))
			{
				LeadingConditionRule rule = new LeadingConditionRule(new DropRuleConditions.BossDowned(boss));
				rule.OnSuccess(DropRules.NormalvsExpertStacked(itemID, normal, expert, minStack, maxStack));
				npcLoot.Add(rule);
			}
		}

		private void DownedBossLootCommon(NPCLoot npcLoot, DropRuleConditions.BossDowned.Bosses boss, int normal, int itemID, int minStack, int maxStack, NPC npc, params int[] types)
		{
			if (types.Contains(npc.type))
			{
				LeadingConditionRule rule = new LeadingConditionRule(new DropRuleConditions.BossDowned(boss));
				rule.OnSuccess(ItemDropRule.Common(itemID, normal, minStack, maxStack));
				npcLoot.Add(rule);
			}
		}

		/// <summary>Drops an item given the specific conditions. Uses NormalvsExpert.</summary>
		/// <param name="npc">NPC to check against.</param>
		/// <param name="chance">Chance in not-expert mode (1/x).</param>
		/// <param name="expertChance">Chance in expert mode (1/x).</param>
		/// <param name="itemID">The item to drop.</param>
		/// <param name="types">The NPC IDs to drop from.</param>
		public void DropLoot(NPCLoot loot, int chance, int expertChance, int itemID, NPC npc, params int[] types)
		{
			if (types.Contains(npc.type))
				loot.Add(ItemDropRule.NormalvsExpert(itemID, chance, expertChance));
		}

		/// <summary>Drops an item given the specific conditions. Uses DropRules.NormalvsExpertStacked.</summary>
		/// <param name="npc">NPC to check against.</param>
		/// <param name="chance">Chance in not-expert mode (1/x).</param>
		/// <param name="expertChance">Chance in expert mode (1/x).</param>
		/// <param name="itemID">The item to drop.</param>
		/// <param name="minStack">The minimum stack size of the dropped item.</param>
		/// <param name="maxStack">The maximum stack size of the dropped item.</param>
		/// <param name="types">The NPC IDs to drop from.</param>
		public void DropLoot(NPCLoot loot, int chance, int expertChance, int itemID, int minStack, int maxStack, NPC npc, params int[] types)
		{
			if (types.Contains(npc.type))
				loot.Add(DropRules.NormalvsExpertStacked(itemID, chance, expertChance, minStack, maxStack));
		}
	}
}
