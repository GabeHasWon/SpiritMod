using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace SpiritMod.Items.BossLoot.StarplateDrops.SteamplateBow
{
	internal class SteamplateBowLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Backpacks);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			if (!drawPlayer.GetModPlayer<SteamplateBowPlayer>().active || drawPlayer.dead)
				return;

			Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/BossLoot/StarplateDrops/SteamplateBow/SteamplateBow_Backpack").Value;
			int frameHeight = texture.Height / 2;

			SteamplateBowPlayer modPlayer = drawPlayer.GetModPlayer<SteamplateBowPlayer>();

			Rectangle rect = new Rectangle(0, modPlayer.negative ? frameHeight : 0, texture.Width, frameHeight - 2);
			Vector2 position = new Vector2((int)drawPlayer.position.X - (int)Main.screenPosition.X, (int)drawPlayer.position.Y + (int)(drawPlayer.height / 2) + (int)drawPlayer.gfxOffY - (int)Main.screenPosition.Y);
			if (drawPlayer.direction == -1)
				position.X += (int)(rect.Width / 2);
			//The player's "step up" frames
			if ((drawPlayer.bodyFrame.Y >= 392 && drawPlayer.bodyFrame.Y <= 504) || (drawPlayer.bodyFrame.Y >= 784 && drawPlayer.bodyFrame.Y <= 896))
				position.Y -= 2;

			SpriteEffects effects;
			//Standard orientation effects under different circumstances
			if ((double)drawPlayer.gravDir == 1.0)
			{
				if (drawPlayer.direction == 1)
					effects = SpriteEffects.None;
				else
					effects = SpriteEffects.FlipHorizontally;
			}
			else
			{
				if (drawPlayer.direction == 1)
					effects = SpriteEffects.FlipVertically;
				else
					effects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
			}

			List<DrawData> data = new List<DrawData>
			{
				new DrawData(texture, position, rect, Lighting.GetColor((int)(drawPlayer.Center.X / 16), (int)(drawPlayer.Center.Y / 16)), 0f, rect.Size() / 2, 1f, effects, 0),
				new DrawData(ModContent.Request<Texture2D>("SpiritMod/Items/BossLoot/StarplateDrops/SteamplateBow/SteamplateBow_Backpack_Glow").Value, position, rect, Color.White, 0f, rect.Size() / 2, 1f, effects, 0)
			};
			if (modPlayer.counter > 0)
			{
				Color color = Color.White * (float)((float)modPlayer.counter / (float)modPlayer.counterMax);
				data.Add(new DrawData(ModContent.Request<Texture2D>("SpiritMod/Items/BossLoot/StarplateDrops/SteamplateBow/SteamplateBow_Backpack_Swap").Value, position, rect, color, 0f, rect.Size() / 2, 1f, effects, 0));
			}
			drawInfo.DrawDataCache.AddRange(data);
		}
	}
}
