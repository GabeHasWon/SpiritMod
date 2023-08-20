using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Candy;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Halloween
{
	public class Taffy : CandyBase
	{
		internal override Point Size => new(20, 20);
		public override void StaticDefaults()
		{
			// DisplayName.SetDefault("Taffy");
			// Tooltip.SetDefault("Increases defense");
		}

		public override void Defaults()
		{
			Item.width = Size.X;
			Item.height = Size.Y;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 30;
			Item.buffType = ModContent.BuffType<TaffyBuff>();
			Item.buffTime = 14400;
		}
	}
}
