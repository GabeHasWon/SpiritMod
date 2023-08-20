using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Accessory.Leather;
using SpiritMod.Items.Sets.GraniteSet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.ShurikenLauncher
{
	[AutoloadEquip(EquipType.HandsOn)]
	public class ShurikenLauncher : ModItem
	{
		public const int EffectiveDistance = 480;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sharpshooter's Glove");
			// Tooltip.SetDefault("Increases ranged damage at a distance\nConsecutive shots at this range deal increased damage");
		}

		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 38;
			Item.value = Item.buyPrice(gold: 4);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<ShurikenLauncherPlayer>().throwerGlove = true;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<LeatherGlove>());
			recipe.AddIngredient(ModContent.ItemType<GraniteChunk>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}