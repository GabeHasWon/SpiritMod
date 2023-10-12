using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.MarbleSet.MarbleArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class MarbleHelm : ModItem
	{
		public override void SetStaticDefaults() => ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 24;
			Item.value = 1100;
			Item.rare = ItemRarityID.Green;
			Item.defense = 6;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<MarbleChest>();

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Marble");
			player.GetSpiritPlayer().marbleSet = true;
		}

		public override void UpdateEquip(Player player)
		{
			player.maxRunSpeed *= 1.1f;
			player.moveSpeed *= 1.1f;
		}

		public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadow = true;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<MarbleChunk>(), 14);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
