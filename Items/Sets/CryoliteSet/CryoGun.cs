using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CryoliteSet
{
	public class CryoGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Winter's Wake");
			Tooltip.SetDefault("Converts rockets into flake rockets, which inflict 'Cryo Crush'\nCryo Crush deals increased damage to weakened enemies");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 42;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.rare = ItemRarityID.Orange;
			Item.width = 50;
            Item.height = 26;
            Item.value = Item.sellPrice(0, 0, 70, 0);
            Item.useAnimation = 60;
			Item.useTime = 60;
			Item.knockBack = 2.5f;
			Item.crit = 6;
			Item.UseSound = SoundID.Item96;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.RocketI;
			Item.shootSpeed = 10f;
			Item.useAmmo = AmmoID.Rocket;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-8, 0);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => type = ModContent.ProjectileType<Projectiles.Bullet.FlakeRocketProj>();

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.06f, .16f, .22f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Handgun);
			recipe.AddIngredient(ModContent.ItemType<CryoliteBar>(), 15);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}