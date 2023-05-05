using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor
{
	public class StoneplateLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.FaceAcc);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (!drawInfo.drawPlayer.GetModPlayer<MyPlayer>().stoneplate)
				return;

			Texture2D texture = Mod.Assets.Request<Texture2D>("Items/Armor/SweatOverlay").Value;
			Vector2 headPos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X) + ((drawInfo.drawPlayer.width - drawInfo.drawPlayer.bodyFrame.Width) / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y) + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4) + drawInfo.drawPlayer.headPosition + drawInfo.rotationOrigin;

			bool steppingUp = (drawInfo.drawPlayer.bodyFrame.Y >= 392 && drawInfo.drawPlayer.bodyFrame.Y <= 504) || (drawInfo.drawPlayer.bodyFrame.Y >= 784 && drawInfo.drawPlayer.bodyFrame.Y <= 896);
			if (steppingUp)
				headPos.Y -= 2 * drawInfo.drawPlayer.gravDir;

			drawInfo.DrawDataCache.Add(new DrawData(
				texture,
				headPos,
				null,
				Lighting.GetColor((drawInfo.drawPlayer.position / 16).ToPoint()) * .7f,
				drawInfo.rotation,
				drawInfo.rotationOrigin,
				1f,
				drawInfo.playerEffect,
				0
			));
		}
	}
}