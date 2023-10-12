using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.HuskstalkSet.ElderbarkArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class ElderbarkHead : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 24;
			Item.value = Item.sellPrice(0, 0, 0, 0);
			Item.rare = ItemRarityID.White;
			Item.defense = 1;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<ElderbarkChest>() && legs.type == ModContent.ItemType<ElderbarkLegs>();
		
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Elderbark");
			player.GetDamage(DamageClass.Generic).Flat++;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<AncientBark>(), 20);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
