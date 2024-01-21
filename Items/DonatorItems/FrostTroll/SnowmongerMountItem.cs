using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace SpiritMod.Items.DonatorItems.FrostTroll
{
	[Sacrifice(1)]
	public class SnowmongerMountItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 23;
			Item.height = 30;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item106;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.SnowMongerMount.SnowmongerMount>();
			Item.rare = ItemRarityID.Master;
			Item.master = true;
		}
	}
}