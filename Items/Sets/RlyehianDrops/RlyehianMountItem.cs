using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace SpiritMod.Items.Sets.RlyehianDrops
{
	[Sacrifice(1)]
	public class RlyehianMountItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Eldritch Cloak");
			// Tooltip.SetDefault("Summons an eldritch cloak to carry you around\nHolding jump allows you to float higher, damaging enemies who pass below");
		}

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 26;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item106;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.RlyehianMount.RlyehianMount>();
			Item.rare = ItemRarityID.Master;
			Item.master = true;
		}
	}
}