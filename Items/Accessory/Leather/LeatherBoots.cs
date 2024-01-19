using SpiritMod.Items.Material;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Leather;

[AutoloadEquip(EquipType.Shoes)]
public class LeatherBoots : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 20;
		Item.value = Item.buyPrice(0, 0, 4, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<LeatherBootsPlayer>().hasBoots = true;

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(1);
		recipe.AddIngredient(ModContent.ItemType<OldLeather>(), 8);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}

	private class LeatherBootsPlayer : ModPlayer
	{
		public bool hasBoots = false;

		public override void ResetEffects() => hasBoots = false;

		public override void PostUpdateRunSpeeds()
		{
			if (hasBoots)
			{
				Player.runAcceleration *= 1.25f;
				Player.maxRunSpeed += 0.1f;
				Player.accRunSpeed += 0.05f;
			}
		}
	}
}
