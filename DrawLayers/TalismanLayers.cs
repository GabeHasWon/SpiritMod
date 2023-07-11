using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using SpiritMod.Items.Accessory.TalismanTree.GildedScarab;
using SpiritMod.Items.Accessory.TalismanTree.SlagMedallion;
using SpiritMod.GlobalClasses.Players;
using Terraria.DataStructures;

namespace SpiritMod.DrawLayers
{
	internal class TalismanLayers : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.FrontAccFront);
		protected override void Draw(ref PlayerDrawSet drawInfo) //reference sharkbonesExplosion and use DrawAnimationVerticalRect or reference how its done in inferno potion
		{
			if (drawInfo.shadow != 0f)
				return;

			Player drawPlayer = drawInfo.drawPlayer; 
			if (drawPlayer.HasBuff(ModContent.BuffType<GildedScarab_buff>()))
			{
				Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/Accessory/TalismanTree/GildedScarab/GildedScarab_Player").Value;
				Vector2 size = new Vector2(68, 54);
				Vector2 origin = size / 2;
				Vector2 drawPos = drawPlayer.Center;
	
				DrawData drawData = new DrawData(texture, drawPos - Main.screenPosition, new Rectangle(0, 56 * (drawPlayer.GetModPlayer<TalismanPlayer>().animTimer / 4), 68, 54), Color.White * 0.9f, 0, origin, 1, SpriteEffects.None, 0);
				drawInfo.DrawDataCache.Add(drawData);
			}
		}
	}
}
