using SpiritMod.Items.Sets.BriarDrops;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.FloranSet.FloranArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class FHelmet : ModItem
	{
		private int timer = 0;

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 22;
			Item.value = Item.sellPrice(0, 0, 12, 0);
			Item.rare = ItemRarityID.Green;
			Item.defense = 4;
		}

		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += .06f;
			player.maxRunSpeed += .03f;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type == ModContent.ItemType<FPlate>() && legs.type == ModContent.ItemType<FLegs>();

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Floran");
			player.GetSpiritPlayer().floranSet = true;

			if (++timer >= 20)
			{
				int d = Dust.NewDust(player.position, player.width, player.height, DustID.JungleGrass);
				Main.dust[d].velocity *= 0f;
				timer = 0;
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FloranBar>(), 8);
			recipe.AddIngredient(ModContent.ItemType<EnchantedLeaf>(), 5);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
