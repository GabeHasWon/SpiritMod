using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Candy;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Halloween
{
	public class ChocolateBar : CandyBase
	{
		internal override Point Size => new(20, 26);

		public override void Defaults()
		{
			Item.width = Size.X;
			Item.height = Size.Y;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
			Item.buffType = ModContent.BuffType<ChocolateBuff>();
			Item.buffTime = 14400;
		}
	}
}
