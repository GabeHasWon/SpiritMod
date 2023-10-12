using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using SpiritMod.Dusts;
using SpiritMod.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BloodcourtSet.BloodCourt
{
	[AutoloadEquip(EquipType.Head)]
	public class BloodCourtHead : ModItem
	{
		public override void SetStaticDefaults() => ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = 3000;
			Item.rare = ItemRarityID.Green;
			Item.defense = 3;
		}

        public override void UpdateEquip(Player player)
		{
			BloodCourtEye(player);

			if (Main.timeForVisualEffects % 40 == 0)
				Projectile.NewProjectile(player.GetSource_FromThis(), player.Center.X + Main.rand.Next(-30, 30), player.Center.Y - Main.rand.Next(40, 50), Main.rand.Next(-1, 1), -1, ModContent.ProjectileType<BloodRuneEffect>(), 0, 0, player.whoAmI, 0.0f, 1);

			player.GetDamage(DamageClass.Generic) += 0.04f;
			player.maxMinions += 1;
		}

		public override void UpdateArmorSet(Player player)
		{
			string tapDir = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.BloodCourt", tapDir);
			player.GetSpiritPlayer().bloodcourtSet = true;
		}

		public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadow = true;

		public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<BloodCourtChestplate>() && legs.type == ModContent.ItemType<BloodCourtLeggings>();

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<DreamstrideEssence>(), 6);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

		public static void DoubleTapEffect(Player player)
		{
			player.AddBuff(ModContent.BuffType<CourtCooldown>(), 500);
			Vector2 mouse = Main.MouseScreen + Main.screenPosition;
			Vector2 dir = Vector2.Normalize(mouse - player.Center) * 12;
			player.statLife -= (int)(player.statLifeMax * .08f);

			for (int i = 0; i < 18; i++)
			{
				int num = Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<NightmareDust>(), 0f, -2f, 0, default, 2f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].scale *= .85f;
				if (Main.dust[num].position != player.Center)
					Main.dust[num].velocity = player.DirectionTo(Main.dust[num].position) * 6f;
			}

			SoundEngine.PlaySound(SoundID.Item109);

			Projectile.NewProjectile(player.GetSource_FromThis("DoubleTap"), player.Center, dir, ModContent.ProjectileType<DarkAnima>(), 70, 0, player.whoAmI);
		}

		private static void BloodCourtEye(Player player)
		{
			int index = 0 + player.bodyFrame.Y / 56;
			if (index >= Main.OffsetsPlayerHeadgear.Length)
				index = 0;

			Vector2 vector2_1 = Vector2.Zero;
			if (player.mount.Active && player.mount.Cart)
			{
				int num = Math.Sign(player.velocity.X);
				if (num == 0)
					num = player.direction;

				vector2_1 = new Vector2(MathHelper.Lerp(0.0f, -8f, player.fullRotation / 0.7853982f), MathHelper.Lerp(0.0f, 2f, Math.Abs(player.fullRotation / 0.7853982f))).RotatedBy(player.fullRotation, new Vector2());
				if (num == Math.Sign(player.fullRotation))
					vector2_1 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(player.fullRotation / 0.7853982f));
			}

			Vector2 spinningpoint1 = new Vector2(3 * player.direction - (player.direction == 1 ? 1 : 0), -11.5f * player.gravDir) + Vector2.UnitY * player.gfxOffY + player.Size / 2f + Main.OffsetsPlayerHeadgear[index];
			Vector2 spinningpoint2 = new Vector2(3 * player.shadowDirection[1] - (player.direction == 1 ? 1 : 0), -11.5f * player.gravDir) + player.Size / 2f + Main.OffsetsPlayerHeadgear[index];
			if (player.fullRotation != 0.0)
			{
				spinningpoint1 = spinningpoint1.RotatedBy(player.fullRotation, player.fullRotationOrigin);
				spinningpoint2 = spinningpoint2.RotatedBy(player.fullRotation, player.fullRotationOrigin);
			}

			float offset = 0.0f;
			if (player.mount.Active)
				offset = player.mount.PlayerOffset;

			Vector2 vector2_2 = player.position + spinningpoint1 + vector2_1;
			vector2_2.Y -= offset / 2f;

			Vector2 vector2_3 = player.oldPosition + spinningpoint2 + vector2_1;
			vector2_3.Y -= offset / 2f;

			int num3 = (int)Vector2.Distance(vector2_2, vector2_3) / 4 + 1;
			if (Vector2.Distance(vector2_2, vector2_3) % 3.0 != 0.0)
				++num3;

			for (float num4 = 1f; num4 <= (double)num3; ++num4)
			{
				Dust dust = Main.dust[Dust.NewDust(player.Center, 0, 0, ModContent.DustType<NightmareDust>(), 0.0f, 0.0f, 0, new Color(), 1f)];
				dust.position = Vector2.Lerp(vector2_3, vector2_2, num4 / num3);
				dust.noGravity = true;
				dust.velocity = Vector2.Zero;
				dust.customData = player;
				dust.shader = GameShaders.Armor.GetSecondaryShader(player.cYorai, player);
			}
		}
	}
}
