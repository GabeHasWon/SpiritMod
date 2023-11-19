using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.RabbitFoot
{
	[AutoloadEquip(EquipType.Waist)]
	public class Rabbit_Foot : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.sellPrice(copper: 1);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;
		}

		public override void UpdateEquip(Player player) => player.GetCritChance(DamageClass.Generic) = 1;

		public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips) 
		{
			var b = new TooltipLine(Mod, "SpiritMod:Rabbit_Foot", "'This must be Lucky!'");
			tooltips.Insert(2, b);
		}
	}
}