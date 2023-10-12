using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops.InfernonArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class InfernalVisor : ModItem
	{
		private int timer;

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.rare = ItemRarityID.Pink;
			Item.value = 72000;
			Item.defense = 9;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetCritChance(DamageClass.Magic) += 9;
			player.GetDamage(DamageClass.Magic) += 0.15f;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type == ModContent.ItemType<InfernalBreastplate>() && legs.type == ModContent.ItemType<InfernalGreaves>();

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Infernal");
			player.GetSpiritPlayer().infernalSet = true;

			if (++timer >= 20)
			{
				Dust.NewDust(player.position, player.width, player.height, DustID.Torch);
				timer = 0;
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<InfernalAppendage>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
