using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SeraphSet
{
	public class MoonStone : ModItem
	{
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = Item.height = 24;
			Item.value = 1000;
			Item.rare = ItemRarityID.LightRed;
			Item.scale = .8f;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100);
	}
}