using Microsoft.Xna.Framework;
using SpiritMod.Items.Material;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.LeatherArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class LeatherHood : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Marksman's Hood");
			Tooltip.SetDefault("Increases ranged weapon damage by 1");
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 12;
			Item.value = 3200;
			Item.rare = ItemRarityID.Blue;
			Item.defense = 1;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type == ModContent.ItemType<LeatherPlate>() && legs.type == ModContent.ItemType<LeatherLegs>();

		public override void UpdateEquip(Player player) => player.GetDamage(DamageClass.Ranged).Flat++;

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "Wearing Marksman's Armor builds up concentration\nWhile concentrated, your next strike is a critical strike and deals more damage\nConcentration is disrupted when hurt, but charges faster while standing still";
			player.GetSpiritPlayer().leatherSet = true;

			if (player.GetSpiritPlayer().concentrated)
				Yoraiz0rEye(player);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<OldLeather>(), 6);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		private static void Yoraiz0rEye(Player player)
		{
			int index = 0 + player.bodyFrame.Y / 56;
			if (index >= Main.OffsetsPlayerHeadgear.Length)
				index = 0;

			Vector2 vector2_1 = Vector2.Zero;
			if (player.mount.Active && player.mount.Cart)
			{
				int sign = Math.Sign(player.velocity.X);
				if (sign == 0)
					sign = player.direction;

				vector2_1 = new Vector2(MathHelper.Lerp(0.0f, -8f, player.fullRotation / 0.7853982f), MathHelper.Lerp(0.0f, 2f, Math.Abs(player.fullRotation / 0.7853982f))).RotatedBy(player.fullRotation, new Vector2());
				if (sign == Math.Sign(player.fullRotation))
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

			int num3 = (int)Vector2.Distance(vector2_2, vector2_3) / 3 + 1;
			if (Vector2.Distance(vector2_2, vector2_3) % 3.0 != 0.0)
				++num3;

			for (float num4 = 1f; num4 <= (double)num3; ++num4)
			{
				Dust dust = Main.dust[Dust.NewDust(player.Center, 0, 0, DustID.GoldCoin, 0.0f, 0.0f, 0, new Color(), 1f)];
				dust.position = Vector2.Lerp(vector2_3, vector2_2, num4 / num3);
				dust.noGravity = true;
				dust.velocity = Vector2.Zero;
				dust.customData = player;
				dust.shader = GameShaders.Armor.GetSecondaryShader(player.cYorai, player);
			}
		}
	}
}
