using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CryoliteSet.CryoliteArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class CryoliteHead : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 26;
			Item.value = Item.sellPrice(0, 0, 80, 0);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 9;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type == ModContent.ItemType<CryoliteBody>() && legs.type == ModContent.ItemType<CryoliteLegs>();

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Cryolite");
			player.GetSpiritPlayer().cryoSet = true;
		}

		public override void UpdateEquip(Player player) => player.GetAttackSpeed(DamageClass.Melee) += .08f;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CryoliteBar>(), 15);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
