using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace SpiritMod.Mechanics.AutoSell
{
	public class AutoSellUIElement : UIElement
	{
		private static Texture2D _backgroundTexture { get { return SpiritModAutoSellTextures.autoSellUIButton.Value; } }
		private static Texture2D sell_NoValueActive { get { return SpiritModAutoSellTextures.sellNoValueButton.Value; } }
		private static Texture2D sell_LockActive { get { return SpiritModAutoSellTextures.sellLockButton.Value; } }
		private static Texture2D sell_WeaponsActive { get { return SpiritModAutoSellTextures.sellWeaponsButton.Value; } }
		
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			
			Player player = Main.LocalPlayer;
			CalculatedStyle dimensions = GetDimensions();
			Point point1 = new Point((int)dimensions.X, (int)dimensions.Y);
			int width = (int)Math.Ceiling(dimensions.Width);
			int height = (int)Math.Ceiling(dimensions.Height);
			
			if (Main.playerInventory && !player.dead && Main.npcShop != 0 && !Main.InReforgeMenu && player.active)
			{
				if (Main.npcShop > 0)
				{
					spriteBatch.Draw(_backgroundTexture, new Rectangle(point1.X, point1.Y, width, height), Color.White);
				}
				
				if (player.GetModPlayer<AutoSellPlayer>().sell_Lock && Main.npcShop > 0)
				{
					spriteBatch.Draw(sell_LockActive, new Rectangle(502, 324, 32, 32), Color.White);
				}

				if (player.GetModPlayer<AutoSellPlayer>().sell_Weapons && Main.npcShop > 0)
				{
					spriteBatch.Draw(sell_WeaponsActive, new Rectangle(502, 362, 32, 32), Color.White);
				}

				if (player.GetModPlayer<AutoSellPlayer>().sell_NoValue && Main.npcShop > 0)
				{
					spriteBatch.Draw(sell_NoValueActive, new Rectangle(502, 400, 32, 32), Color.White);
				}
			}
		}
	}
}