using Terraria.Audio;
using SpiritMod.Items.Accessory.TalismanTree.GildedScarab;
using SpiritMod.Items.Accessory.TalismanTree.SlagMedallion;
using SpiritMod.Utilities;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using SpiritMod.Items.Accessory.TalismanTree.GrislyTongue;
using Microsoft.Xna.Framework;

namespace SpiritMod.GlobalClasses.Players
{
	internal class TalismanPlayer : ModPlayer
	{
		public int totemHealthToRecover;
		public int scarabDefense;
		public float slagDamageMultiplier;
		private bool canActivate;
		public int scarabTimer;
		public int slagTimer;
		public int scarabFadeTimer;
		public int slagFadeTimer;

		public override void UpdateEquips()
		{
			//for animating drawlayer sprites
			if (++scarabTimer >= 32)
				scarabTimer = 0;
			if (++slagTimer >= 20)
				slagTimer = 0;

			if (Player.HasBuff(ModContent.BuffType<GildedScarab_buff>()))
				scarabFadeTimer++;
			if (Player.HasBuff(ModContent.BuffType<SlagFury_buff>()))
				slagFadeTimer++;
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (modifiers.DamageSource.SourceOtherIndex == 3 || modifiers.DamageSource.SourceOtherIndex == 2)
				canActivate = false;
			else
				canActivate = true;
		}

		public override void OnHurt(Player.HurtInfo info)
		{
			if (Player.HasAccessory<GrislyTongue>() && info.Damage > 8)
			{
				SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);
				totemHealthToRecover = (int)info.Damage / 8;
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
				scarabFadeTimer = 0;
				if (Player.HasBuff(ModContent.BuffType<GildedScarab_buff>()))
				{
					Player.ClearBuff(ModContent.BuffType<GildedScarab_buff>());
					scarabDefense = (scarabDefense + (int)(info.Damage / 8f) >= 50 || scarabDefense >= 50) ? 50 : scarabDefense + (int)(info.Damage / 8f);
				}
				else
				{
					SoundEngine.PlaySound(SoundID.Item76);
					scarabDefense = (info.Damage >= 400) ? 50 : 5 + (int)(info.Damage / 8f);
				}
				Player.AddBuff(ModContent.BuffType<GildedScarab_buff>(), 300);
			}

			if (Player.HasAccessory<SlagMedallion>() && canActivate)
			{
				slagFadeTimer = 0;
				if (Player.HasBuff(ModContent.BuffType<SlagFury_buff>()))
				{
					Player.ClearBuff(ModContent.BuffType<SlagFury_buff>());
					slagDamageMultiplier = ((slagDamageMultiplier + (float)(info.Damage / 800f)) >= 0.5f || slagDamageMultiplier >= 0.5f) ? 0.5f : slagDamageMultiplier + (float)(info.Damage / 800f);
				}
				else
				{
					SoundEngine.PlaySound(SoundID.Item74);
					slagDamageMultiplier = (info.Damage >= 400) ? 0.5f : 0.05f + (float)(info.Damage / 800f);
				}
				Player.AddBuff(ModContent.BuffType<SlagFury_buff>(), 300);
			}
		}
	}
}