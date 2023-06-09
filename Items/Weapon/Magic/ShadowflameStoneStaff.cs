using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Magic;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Magic
{
	public class ShadowflameStoneStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shadowbreak Wand");
			Tooltip.SetDefault("Shoots out erratic shadowflame wisps");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Weapon/Magic/ShadowflameStoneStaff_Glow");
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 46;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.damage = 12;
			Item.knockBack = 4;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.channel = true;
			Item.UseSound = SoundID.Item8;
			Item.autoReuse = false;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ShadowflameStoneBolt>();
			Item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			for (int i = 0; i < Main.rand.Next(1, 3); i++)
			{
				float angle = Main.rand.NextFloat(MathHelper.PiOver4, -MathHelper.Pi - MathHelper.PiOver4);
				Vector2 spawnPlace = Vector2.Normalize(new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle))) * 20f;
				if (Collision.CanHit(position, 0, 0, position + spawnPlace, 0, 0))
					position += spawnPlace;

				velocity = position.DirectionTo(Main.MouseWorld) * Item.shootSpeed;
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
				
				for (float num2 = 0.0f; (double)num2 < 10; ++num2)
				{
					Dust dust = Dust.NewDustDirect(position, 2, 2, DustID.ShadowbeamStaff, Scale: .8f);
					dust.velocity = Vector2.Normalize(spawnPlace.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi))) * 1.6f;
					dust.noGravity = true;
				}
			}

			return false;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
