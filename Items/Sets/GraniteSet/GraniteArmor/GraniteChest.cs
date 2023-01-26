using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GraniteSet.GraniteArmor
{
	[AutoloadEquip(EquipType.Body)]
	public class GraniteChest : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Granite Breastplate");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
			Tooltip.SetDefault("Increases jump height slightly");
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 24;
			Item.value = 1100;
			Item.rare = ItemRarityID.Green;
			Item.defense = 11;
		}

		public override void UpdateEquip(Player player) => Player.jumpSpeed += 1;

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) => glowMaskColor = Color.White;
		
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<GraniteChunk>(), 12);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
