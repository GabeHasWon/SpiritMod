using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SpiritSet
{
	public class SoulShred : ModItem
	{
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true; 
			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.rare = ItemRarityID.Pink;
			Item.width = 14;
			Item.height = 36;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override void PostUpdate() => Lighting.AddLight(Item.Center, new Color(46, 255, 251).ToVector3() * 0.3f * Main.essScale);
		public override Color? GetAlpha(Color lightColor) => new Color(180, 180, 180);
	}
}