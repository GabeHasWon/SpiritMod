using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AtlasDrops.PrimalstoneArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class PrimalstoneFaceplate : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 30;
			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Cyan;
			Item.defense = 14;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<PrimalstoneBreastplate>() && legs.type == ModContent.ItemType<PrimalstoneLeggings>();

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Primalstone");
			player.GetSpiritPlayer().primalSet = true;
			player.moveSpeed -= .10F;
			Dust.NewDustDirect(player.position, player.width, player.height - 38, DustID.UnusedWhiteBluePurple).scale = 2f;
		}

		public override void UpdateEquip(Player player)
		{
			player.endurance += 0.08f;
			player.statManaMax2 += 60;
			player.GetDamage(DamageClass.Melee) += .2f;
			player.GetDamage(DamageClass.Magic) += .2f;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<ArcaneGeyser>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}