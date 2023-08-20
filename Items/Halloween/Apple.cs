using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace SpiritMod.Items.Halloween
{
	public class Apple : CandyBase
	{
		public override bool IsLoadingEnabled(Mod mod) => false;

		internal override Point Size => new(20, 20);
		public override void StaticDefaults()
		{
			// DisplayName.SetDefault("Apple");
			// Tooltip.SetDefault("'Who the hell gives these out?'");
		}

		public override void Defaults()
		{
			Item.rare = -1;
			Item.maxStack = 30;
			Item.autoReuse = false;
		}
	}
}
