using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.RunicSet
{
	public class Rune : ModItem
	{
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 5));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;
			ItemID.Sets.ItemIconPulse[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 42;
			Item.value = 100;
			Item.rare = ItemRarityID.Pink;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}