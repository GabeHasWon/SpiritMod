using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GraniteSet.GraniteArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class GraniteHelm : ModItem
	{
		public override void SetStaticDefaults() => SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Head_Glow");

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 24;
			Item.value = 1100;
			Item.rare = ItemRarityID.Green;
			Item.defense = 9;
		}

		public override void UpdateEquip(Player player) => Player.jumpSpeed += 1;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) => glowMaskColor = Color.White;

		public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<GraniteChest>() && legs.type == ModContent.ItemType<GraniteLegs>();

		public override void UpdateArmorSet(Player player)
		{
			string tapDir = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Granite", tapDir);
			player.GetSpiritPlayer().graniteSet = true;
		}

		public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadow = true;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<GraniteChunk>(), 14);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
