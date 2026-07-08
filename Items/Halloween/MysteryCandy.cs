using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace SpiritMod.Items.Halloween
{
	public class MysteryCandy : CandyBase
	{
		internal override Point Size => new(24, 24);

		public override void Defaults()
		{
			Item.width = Size.X;
			Item.height = Size.Y;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override bool? UseItem(Player player)
		{
			string line = "";
			Color color = new Color(127, 255, 127);
			if (Main.rand.Next(100) < 66) {
				//Positive effect
				switch (Main.rand.Next(14)) {
					case 0:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.1");
						player.AddBuff(BuffID.Lifeforce, 36000);
						player.AddBuff(BuffID.Regeneration, 36000);
						player.AddBuff(BuffID.RapidHealing, 1800);
						break;
					case 1:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.2");
						player.AddBuff(BuffID.Swiftness, 36000);
						player.AddBuff(BuffID.Panic, 10800);
						break;
					case 2:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.3");
						player.AddBuff(BuffID.Featherfall, 36000);
						player.AddBuff(BuffID.WaterWalking, 36000);
						//player.AddBuff(Gills, 36000);
						break;
					case 3:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.4");
						player.AddBuff(BuffID.Clairvoyance, 36000);
						player.AddBuff(BuffID.MagicPower, 36000);
						player.AddBuff(BuffID.ManaRegeneration, 36000);
						break;
					case 4:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.5");
						player.AddBuff(BuffID.Ironskin, 36000);
						player.AddBuff(BuffID.Endurance, 36000);
						break;
					case 5:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.6");
						player.AddBuff(BuffID.Mining, 36000);
						player.AddBuff(BuffID.Shine, 36000);
						break;
					case 6:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.7");
						player.AddBuff(BuffID.Spelunker, 36000);
						player.AddBuff(BuffID.Hunter, 36000);
						player.AddBuff(BuffID.Dangersense, 36000);
						break;
					case 7:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.8");
						player.AddBuff(BuffID.Archery, 36000);
						player.AddBuff(BuffID.AmmoReservation, 36000);
						break;
					case 8:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.9");
						if (player.statLife < player.statLifeMax) {
							player.HealEffect(Math.Min(200, player.statLifeMax - player.statLife));
							player.statLife += 200;
						}
						player.AddBuff(BuffID.WellFed, 36000);
						player.AddBuff(BuffID.Honey, 1800);
						break;
					case 9:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.10");
						player.AddBuff(BuffID.Calm, 36000);
						player.AddBuff(BuffID.Builder, 36000);
						break;
					case 10:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.11");
						player.AddBuff(BuffID.Summoning, 86400);
						player.AddBuff(BuffID.Bewitched, 86400);
						break;
					case 11:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.12");
						player.AddBuff(BuffID.Rage, 36000);
						player.AddBuff(BuffID.Wrath, 36000);
						break;
					case 12:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.13");
						player.gravDir *= -1;
						player.velocity.Y += player.gravDir * 5;
						player.AddBuff(BuffID.Gravitation, 18000);
						//player.AddBuff(VortexDebuff, 1800);
						break;
					case 13:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Positive.14");
						player.AddBuff(BuffID.Fishing, 54000);
						player.AddBuff(BuffID.Sonar, 54000);
						player.AddBuff(BuffID.Crate, 54000);
						break;
				}
			}
			else {
				color = new Color(255, 127, 127);
				switch (Main.rand.Next(11)) {
					case 0:
						player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral(Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Hurt", player.name))), (int)(player.statLifeMax * .25f), 0);
						if (player.statLife > 0)
							line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.1");
						player.AddBuff(BuffID.Bleeding, 3600);
						break;
					case 1:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.2");
						player.AddBuff(BuffID.Slow, 3600);
						player.AddBuff(BuffID.Dazed, 3600);
						break;
					case 2:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.3");
						player.AddBuff(BuffID.Stoned, 300);
						player.AddBuff(BuffID.Suffocation, 300);
						player.AddBuff(BuffID.OgreSpit, 3600);
						break;
					case 3:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.4");
						player.statMana = 0;
						player.AddBuff(BuffID.ManaSickness, 1200);
						player.AddBuff(BuffID.Silenced, 3600);
						break;
					case 4:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.5");
						player.AddBuff(BuffID.BrokenArmor, 7200);
						player.AddBuff(BuffID.WitheredArmor, 7200);
						player.AddBuff(BuffID.Ichor, 7200);
						break;
					case 5:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.6");
						player.AddBuff(BuffID.Obstructed, 1200);
						player.AddBuff(BuffID.Blackout, 1200);
						player.AddBuff(BuffID.Cursed, 7200);
						break;
					case 6:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.7");
						player.AddBuff(BuffID.Frozen, 600);
						player.AddBuff(BuffID.Frostburn, 600);
						player.AddBuff(BuffID.Chilled, 3600);
						break;
					case 7:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.8");
						player.AddBuff(BuffID.WitheredWeapon, 7200);
						player.AddBuff(BuffID.Weak, 7200);
						break;
					case 8:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.9");
						player.AddBuff(BuffID.Electrified, 1500);
						player.AddBuff(BuffID.Webbed, 300);
						break;
					case 9:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.10");
						player.AddBuff(BuffID.Darkness, 7200);
						player.AddBuff(BuffID.Confused, 7200);
						player.AddBuff(BuffID.Tipsy, 7200);
						player.AddBuff(BuffID.Titan, 7200);
						break;
					case 10:
						line = Language.GetTextValue("Mods.SpiritMod.Items.MysteryCandy.Negative.11");
						player.AddBuff(BuffID.Bleeding, 3600);
						player.AddBuff(BuffID.PotionSickness, 10800);
						break;
				}
			}
			if (line != "")
				Main.NewText(line, color.R, color.G, color.B);

			return null;
		}
	}
}
