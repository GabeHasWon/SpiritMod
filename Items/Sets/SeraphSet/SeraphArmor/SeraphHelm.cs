using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SeraphSet.SeraphArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class SeraphHelm : ModItem
	{
		public override void SetStaticDefaults()
			=> SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/SeraphSet/SeraphArmor/SeraphHelm_Glow");

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 24;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.defense = 10;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type == ModContent.ItemType<SeraphArmor>() && legs.type == ModContent.ItemType<SeraphLegs>();

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
			=> glowMaskColor = Color.White;

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Seraph");
			player.GetSpiritPlayer().astralSet = true;
		}

		public override void UpdateEquip(Player player) => player.GetDamage(DamageClass.Melee) += .12f;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<MoonStone>(), 8);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}