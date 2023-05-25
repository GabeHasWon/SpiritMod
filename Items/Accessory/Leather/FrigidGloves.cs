using SpiritMod.Items.Sets.FrigidSet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Leather
{
	[AutoloadEquip(EquipType.HandsOn)]
	public class FrigidGloves : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frigid Wraps");
			Tooltip.SetDefault("Melee critical strikes inflict Frostburn\nGrants 4% increased attack speed for every nearby enemy\nThis effect stacks up to five times");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.Blue;
			Item.value = 1200;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetAttackSpeed(DamageClass.Melee) += 0.05f * player.GetModPlayer<MyPlayer>().frigidGloveStacks;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<LeatherGlove>(), 1);
			recipe.AddIngredient(ModContent.ItemType<FrigidFragment>(), 6);
			recipe.AddRecipeGroup("SpiritMod:PHMEvilMaterial", 4);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
