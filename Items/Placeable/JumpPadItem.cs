using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Placeable
{
	[Sacrifice(1)]
	public class JumpPadItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.rare = ItemRarityID.Green;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.createTile = Mod.Find<ModTile>("JumpPadTile").Type;
			Item.maxStack = Item.CommonMaxStack;
			Item.autoReuse = false;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.consumable = true;

		}
	}
}
