using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CryoliteSet
{
	public class CryoHamaxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryolite Hamaxe");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/CryoliteSet/CryoHamaxe_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 40;
			Item.value = Item.sellPrice(0, 0, 40, 0);
			Item.rare = ItemRarityID.Orange;

			Item.axe = 30;
			Item.hammer = 70;

			Item.damage = 18;
			Item.knockBack = 5;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 27;
			Item.useAnimation = 27;

			Item.DamageType = DamageClass.Melee;
			Item.useTurn = true;
			Item.autoReuse = true;

			Item.UseSound = SoundID.Item1;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.06f, .16f, .22f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.DungeonSpirit);
			Main.dust[dust].noGravity = true;
		}

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ModContent.ItemType<CryoliteBar>(), 12);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}
