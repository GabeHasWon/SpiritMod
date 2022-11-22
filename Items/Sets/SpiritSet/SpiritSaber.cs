using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SpiritSet
{
	public class SpiritSaber : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spirit Saber");
			Tooltip.SetDefault("Shoots out a homing bolt of souls that inflicts Soul Burn");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/SpiritSet/SpiritSaber_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 38;
			Item.value = Item.sellPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.crit += 4;
			Item.damage = 44;
			Item.knockBack = 5f;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SoulSpirit>();
			Item.shootSpeed = 12f;
			Item.UseSound = SoundID.Item1;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.06f, .16f, .22f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe(1);
			modRecipe.AddIngredient(ModContent.ItemType<SpiritBar>(), 12);
			modRecipe.AddIngredient(ModContent.ItemType<SoulShred>(), 6);
			modRecipe.AddTile(TileID.MythrilAnvil);
			modRecipe.Register();
		}
		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(2)) {
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.BlueCrystalShard);
			}
		}
	}
}
