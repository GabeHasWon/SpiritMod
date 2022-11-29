using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.StarplateDrops.StarplateSummon
{
	public class DroneController : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Drone Controller");
			Tooltip.SetDefault("Summons a Starplate Fighter Drone to fight for you");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.width = 34;
			Item.height = 32;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.mana = 12;
			Item.knockBack = 3f;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.holdStyle = ItemHoldStyleID.HoldFront;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<StarplateDrone>();
			Item.UseSound = SoundID.Item44;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		public override void HoldStyle(Player player, Rectangle heldItemFrame)
		{
			player.itemLocation.X -= 12 * player.direction;
			player.itemLocation.Y += 10 * (int)player.gravDir;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			position = Main.MouseWorld;
			Projectile.NewProjectile(source, position, Main.rand.NextVector2Circular(3, 3), type, damage, knockback, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CosmiliteShard>(), 18);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}