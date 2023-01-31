using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GraniteSet
{
	public class GraniteWand : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Unstable Conduit");
			Tooltip.SetDefault("Creates a fountain of energy at the ground near the Cursor Position\nKilling enemies with this weapon causes them to explode into damaging energy wisps");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 22;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 15;
			Item.width = 44;
			Item.height = 44;
			Item.useTime = 31;
			Item.useAnimation = 31;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.staff[Item.type] = true;
			Item.noMelee = true;
			Item.knockBack = 4;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.crit = 10;
			Item.UseSound = SoundID.Item109;
			Item.shoot = ModContent.ProjectileType<GraniteWandProj>();
			Item.shootSpeed = 8f;
			Item.autoReuse = false;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.08f, .12f, .52f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			for (int i = 0; i < Main.projectile.Length; i++) {
				Projectile p = Main.projectile[i];
				if (p.active && p.type == Item.shoot && p.owner == player.whoAmI) {
					p.active = false;
				}
			}
			Projectile.NewProjectile(source, Main.MouseWorld.X, Main.MouseWorld.Y, 0f, 100f, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<GraniteChunk>(), 18);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
