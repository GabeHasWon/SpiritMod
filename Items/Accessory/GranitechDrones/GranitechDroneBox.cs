using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.GranitechDrones
{
	public class GranitechDroneBox : MinionAccessory
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("G-TEK Drone Box");
			Tooltip.SetDefault("Summons 3 drones to aid you\nCan cycle between 3 modes\nThese drones do not take up minion slots");
		}

		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Summon;
			Item.knockBack = 1.5f;
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.buyPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}
	}
}
