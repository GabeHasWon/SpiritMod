using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Summon;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CoilSet
{
	public class CoilSummonStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Coiled Rod");
			Tooltip.SetDefault("Summons a stationary electric turret\nThis turret shoots beams of lightning that jump from enemy to enemy");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.QueenSpiderStaff);
			Item.damage = 19;
			Item.knockBack = 2.5f;
			Item.shootSpeed = 0f;
			Item.width = 40;
			Item.height = 40;
			Item.mana = 12;
			Item.value = Item.sellPrice(0, 0, 80, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item25;
			Item.sentry = true;
			Item.shoot = ModContent.ProjectileType<CoilSentrySummon>();
		}

		public override bool CanUseItem(Player player)
		{
			player.FindSentryRestingSpot(Item.shoot, out int worldX, out int worldY, out _);
			worldX /= 16;
			worldY /= 16;
			worldY--;
			return !WorldGen.SolidTile(worldX, worldY);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			player.FindSentryRestingSpot(type, out int worldX, out int worldY, out int pushYUp);

			Projectile.NewProjectile(Item.GetSource_ItemUse(Item), worldX, worldY - pushYUp, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
			player.UpdateMaxTurrets();
			return false;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.08f, .4f, .28f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ModContent.ItemType<TechDrive>(), 6);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}
