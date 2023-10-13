using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SlagSet.FieryArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class ObsidiusHelm : ModItem
	{
		public override void SetStaticDefaults() => SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Head_Glow");

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = Item.sellPrice(0, 0, 35, 0);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 6;
		}

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
			=> glowMaskColor = new Color(100, 100, 100, 100);

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, .4f, .12f, .028f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void UpdateEquip(Player player)
		{
			player.maxTurrets += 1;
			player.GetDamage(DamageClass.Summon) += .05f;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type == ModContent.ItemType<ObsidiusPlate>() && legs.type == ModContent.ItemType<ObsidiusGreaves>();

		public override void UpdateArmorSet(Player player)
		{
			string tapDir = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Slag", tapDir);
			player.GetSpiritPlayer().fierySet = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CarvedRock>(), 14);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
