using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Equipment
{
	[Sacrifice(1)]
	internal class KelpHook : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Kelp Hook");
			Tooltip.SetDefault("Provides faster mobility when underwater");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.shootSpeed = 12f;
			Item.shoot = ModContent.ProjectileType<KelpHookProj>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Sets.FloatingItems.Kelp>(), 20);
			recipe.AddIngredient(ItemID.Hook, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}

	internal class KelpHookProj : ModProjectile
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Kelp Hook");

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
			Projectile.timeLeft = 1200;
		}

		public override bool? CanUseGrapple(Player player)
		{
			int hooksOut = 0;

			for (int l = 0; l < 1000; l++)
				if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type)
					hooksOut++;

			return hooksOut <= 1;
		}

		public override float GrappleRange() => Main.player[Projectile.owner].wet ? 425 : 325;
		public override void NumGrappleHooks(Player player, ref int numHooks) => numHooks = 1;
		public override void GrappleRetreatSpeed(Player player, ref float speed) => speed = Main.player[Projectile.owner].wet ? 24 : 12;
		public override void GrapplePullSpeed(Player player, ref float speed) => speed = Main.player[Projectile.owner].wet ? 20 : 10;

		public override void PostDraw(Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/Equipment/KelpHookChain", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Vector2 vector = Projectile.Center;
			Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
			Rectangle? sourceRectangle = null;
			Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			float num = texture.Height;
			Vector2 vector2 = mountedCenter - vector;
			float rotation = (float)Math.Atan2(vector2.Y, vector2.X) - 1.57f;
			bool flag = true;
			if (float.IsNaN(vector.X) && float.IsNaN(vector.Y)) {
				flag = false;
			}
			if (float.IsNaN(vector2.X) && float.IsNaN(vector2.Y)) {
				flag = false;
			}
			while (flag) {
				if (vector2.Length() < num + 1.0) {
					flag = false;
				}
				else {
					Vector2 value = vector2;
					value.Normalize();
					vector += value * num;
					vector2 = mountedCenter - vector;
					Color color = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16.0));
					color = Projectile.GetAlpha(color);
					Main.spriteBatch.Draw(texture, vector - Main.screenPosition, sourceRectangle, color, rotation, origin, 1f, SpriteEffects.None, 0f);
				}
			}
		}
	}
}