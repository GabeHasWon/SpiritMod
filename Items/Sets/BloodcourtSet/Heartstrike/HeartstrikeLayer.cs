using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BloodcourtSet.Heartstrike
{
	internal class HeartstrikeLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Backpacks);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			if (!drawPlayer.GetModPlayer<HeartstrikePlayer>().active)
				return;

			Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/Sets/BloodcourtSet/Heartstrike/Heartstrike_Backpack").Value;

			Vector2 position = new Vector2((int)drawPlayer.position.X - (int)Main.screenPosition.X, (int)drawPlayer.position.Y + (int)(drawPlayer.height / 2) + (int)drawPlayer.gfxOffY - (int)Main.screenPosition.Y);
			if (drawPlayer.direction == -1)
				position.X += (int)(texture.Width / 2);
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

			DrawData data = new DrawData(texture, position, texture.Bounds, Lighting.GetColor((int)(drawPlayer.Center.X / 16), (int)(drawPlayer.Center.Y / 16)), 0f, texture.Size() / 2, 1f, effects, 0);
			drawInfo.DrawDataCache.Add(data);
		}
	}
}
