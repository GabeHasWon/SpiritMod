using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.DarkfeatherVisage
{
    [AutoloadEquip(EquipType.Head)]
    public class DarkfeatherVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darkfeather Visage");
            Tooltip.SetDefault("Increases magic and summon damage by 7%\nGrants a bonus when worn with a magic robe or fur coat");
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 1, 6, 0);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 2;
        }

		public override bool IsArmorSet(Item head, Item body, Item legs) => (body.type >= ItemID.AmethystRobe && body.type <= ItemID.DiamondRobe || body.type == ItemID.GypsyRobe || body.type == ItemID.FlinxFurCoat);
		public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.MagicSummonHybrid) += .07f;
        }
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = $"Generates exploding darkfeather bolts around the player";
			player.GetSpiritPlayer().darkfeatherVisage = true;
		}
	}
}
