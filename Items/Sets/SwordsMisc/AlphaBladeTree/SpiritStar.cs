using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SwordsMisc.AlphaBladeTree
{
	public class SpiritStar : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Soul Star");
			// Tooltip.SetDefault("Rains down multiple starry bolts from the sky that inflict Star Fracture\nThese stars explode into multiple souls that inflict Soul Burn\n'The convergence of souls and the cosmos'");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/SwordsMisc/AlphaBladeTree/SpiritStar_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 112;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.DamageType = DamageClass.Melee;
			Item.width = 56;
			Item.height = 56;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = Item.sellPrice(0, 16, 0, 0);
			Item.rare = ItemRarityID.Cyan;
			Item.shootSpeed = 8;
			Item.UseSound = SoundID.Item69;
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.SpiritStar>();
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) => Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Electric).scale *= .23f;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			for (int i = 0; i < 3; ++i)
				if (Main.myPlayer == player.whoAmI)
					Projectile.NewProjectile(source, Main.MouseWorld.X + Main.rand.Next(-80, 80), player.Center.Y - 1000 + Main.rand.Next(-50, 50), 0, Main.rand.Next(11, 23), type, damage, knockback, player.whoAmI);
			
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Starblade>(), 1);
			recipe.AddIngredient(ModContent.ItemType<SpiritSet.SpiritSaber>(), 1);
			recipe.AddIngredient(ItemID.Ectoplasm, 15);
			recipe.AddIngredient(ItemID.FragmentSolar, 4);
			recipe.AddIngredient(ItemID.FragmentVortex, 4);
			recipe.AddIngredient(ItemID.FragmentNebula, 4);
			recipe.AddIngredient(ItemID.FragmentStardust, 4);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}
	}
}
