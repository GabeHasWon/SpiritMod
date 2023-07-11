using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using SpiritMod.Items.Accessory.TalismanTree.GrislyTotem;
using SpiritMod.Items.Accessory.TalismanTree.GildedScarab;
using SpiritMod.Items.Accessory.TalismanTree.SlagMedallion;
using SpiritMod.Utilities;
using Terraria.DataStructures;

namespace SpiritMod.GlobalClasses.Players
{
	internal class TalismanPlayer : ModPlayer
	{
		public int totemHealthToRecover;
		public int scarabDefense;
		public float slagDamageMultiplier;
		private bool canActivate;
		public int animTimer;

		public override void UpdateEquips()
		{
			animTimer++;
			if (animTimer > 28)
				animTimer = 0;
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter) 
		{
			if (damageSource.SourceOtherIndex == 3 || damageSource.SourceOtherIndex == 2)
				canActivate = false;
			else
				canActivate = true;
			return true;
		}
		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
		{
			if (Player.HasAccessory<GrislyTotem>() && damage > 8)
			{
				SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);
				totemHealthToRecover = (int)damage / 8;
				for (int i = 0; i < 3; i++)
				{
					int item = Item.NewItem(Player.GetSource_FromThis("grislyTotem"), Player.position, ModContent.ItemType<GrislyBit>());
					Main.item[item].velocity = Main.rand.NextVector2CircularEdge(20f, 10f);
				}
				for (int i = 0; i < 10; i++)
					Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood, 1f, 1f, 100, default, 2f);
			}

			if (Player.HasAccessory<GildedScarab>() && canActivate) 
			{
				if (Player.HasBuff(ModContent.BuffType<GildedScarab_buff>()))
				{
					Player.ClearBuff(ModContent.BuffType<GildedScarab_buff>());
					scarabDefense = (scarabDefense + ((int)damage / 8) >= 50 || scarabDefense >= 50) ? 50 : scarabDefense + ((int)damage / 8);
					Player.AddBuff(ModContent.BuffType<GildedScarab_buff>(), 300);
				}
				else
				{
					SoundEngine.PlaySound(SoundID.Item76);
					scarabDefense = (damage >= 400) ? 50 : 5 + ((int)damage / 8);
					Player.AddBuff(ModContent.BuffType<GildedScarab_buff>(), 300);
				}
			}

			if (Player.HasAccessory<SlagMedallion>() && canActivate)
			{
				if (Player.HasBuff(ModContent.BuffType<SlagFury_buff>()))
				{
					Player.ClearBuff(ModContent.BuffType<SlagFury_buff>());
					slagDamageMultiplier = ((slagDamageMultiplier + (float)(damage / 800)) >= 0.5f || slagDamageMultiplier >= 0.5f) ? 0.5f : slagDamageMultiplier + (float)(damage / 800);
					Player.AddBuff(ModContent.BuffType<SlagFury_buff>(), 300);
				}
				else
				{
					SoundEngine.PlaySound(SoundID.Item74);
					slagDamageMultiplier = (damage >= 400) ? 0.5f : 0.05f + (float)(damage / 800);
					Player.AddBuff(ModContent.BuffType<SlagFury_buff>(), 300);
				}
			}
		}
	}
}
