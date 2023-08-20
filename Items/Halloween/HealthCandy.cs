using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Candy;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Halloween
{
	public class HealthCandy : CandyBase
	{
		internal override Point Size => new(24, 24);
		public override void StaticDefaults()
		{
			// DisplayName.SetDefault("Health Candy");
			// Tooltip.SetDefault("Increases health");
		}

		public override void Defaults()
		{
			Item.width = Size.X;
			Item.height = Size.Y;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 30;
			Item.buffType = ModContent.BuffType<HealthBuffC>();
			Item.buffTime = 14400;
		}
	}
}
