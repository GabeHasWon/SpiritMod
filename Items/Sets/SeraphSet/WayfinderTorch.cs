using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SeraphSet
{
	public class WayfinderTorch : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seraph's Light");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Shuriken);
			Item.width = 32;
			Item.height = 32;
			Item.shoot = ModContent.ProjectileType<Projectiles.Thrown.WayfinderTorch>();
			Item.useAnimation = 21;
			Item.useTime = 21;
			Item.mana = 4;
			Item.shootSpeed = 12f;
			Item.damage = 43;
			Item.knockBack = 1f;
			Item.value = Item.buyPrice(0, 0, 0, 50);
			Item.rare = ItemRarityID.LightRed;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
		}

		public override void HoldItem(Player player)
		{
			Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
			Lighting.AddLight(position, .3f, .2f, .6f);
		}

		public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick) => dryTorch = true;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) 
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(55);
			recipe.AddIngredient(ModContent.ItemType<MoonStone>(), 2);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}