using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
	[AutoloadEquip(EquipType.Neck)]
	public class SpellswordCrest : MinionAccessory
	{
		public override void SetDefaults()
		{
            Item.damage = 32;
            Item.DamageType = DamageClass.Summon;
			Item.width = 48;
			Item.height = 49;
            Item.knockBack = 1.25f;
			Item.value = Item.sellPrice(0, 1, 50, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.defense = 1;
			Item.accessory = true;
		}

		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<RogueCrest>(), 1);
            recipe.AddIngredient(ItemID.CrystalShard, 8);
            recipe.AddIngredient(ItemID.SoulofLight, 15);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}
