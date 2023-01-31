using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CoilSet
{
	public class CoilSword : ModItem
	{
		private int charger;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Coiled Blade");
			Tooltip.SetDefault("Every six successful hits on an enemy releases an electrical explosion");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 22;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.sellPrice(0, 0, 40, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.08f, .12f, .52f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			charger++;
			if (charger >= 6) {
				SoundEngine.PlaySound(SoundID.Item14, target.Center);
				Projectile.NewProjectile(Item.GetSource_OnHit(target), target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<CoiledExplosion>(), damage, knockback, player.whoAmI);
				charger = 0;
			}
		}

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ModContent.ItemType<TechDrive>(), 4);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}
