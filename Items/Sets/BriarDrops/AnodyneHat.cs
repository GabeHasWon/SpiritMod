using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BriarDrops
{
	[AutoloadEquip(EquipType.Head)]
	public class AnodyneHat : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Anodyne Hat");
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 1, 50, 0);
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}
	}
}