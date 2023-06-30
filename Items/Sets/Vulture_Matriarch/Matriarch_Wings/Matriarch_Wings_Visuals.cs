using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.Vulture_Matriarch.Matriarch_Wings
{
	public class Matriarch_Wings_Visuals : ModPlayer
	{
		public float impact;
		public float storedVelocityY;
		public bool wingsEquipped = false;

		public bool IsDiving => (Player.gravDir == -1) ? storedVelocityY < 0 : storedVelocityY > 0;

		public override void ResetEffects() => wingsEquipped = false;

		public override void ModifyScreenPosition()
		{
			if (wingsEquipped)
				impact = MathHelper.Lerp(impact, 0, 0.05f);
			else
				impact = 0;

			Main.screenPosition.Y += impact * ModContent.GetInstance<SpiritClientConfig>().ScreenShake;
		}

		public override void PostUpdate() { }
	}

	public class VultureMatriachWingsLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;

			if (drawPlayer.GetModPlayer<Matriarch_Wings_Visuals>().IsDiving)
			{
				SpriteEffects spriteEffects;
				if (drawPlayer.gravDir == 1.0)
				{
					if (drawPlayer.direction == 1)
						spriteEffects = SpriteEffects.None;
					else
						spriteEffects = SpriteEffects.FlipHorizontally;
				}
				else
				{
					if (drawPlayer.direction == 1)
						spriteEffects = SpriteEffects.FlipVertically;
					else
						spriteEffects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
				}

				Color color = drawPlayer.GetImmuneAlphaPure(Color.Goldenrod with { A = 0 }, drawInfo.shadow);
				Texture2D texture = Mod.Assets.Request<Texture2D>("Items/Sets/Vulture_Matriarch/Matriarch_Wings/Matriarch_Wings_Wings", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Rectangle drawFrame = texture.Frame(1, 4, 0, drawInfo.drawPlayer.wingFrame, 0, 0);
				Vector2 position = new Vector2((int)(drawInfo.Position.X - (9 * drawInfo.drawPlayer.direction) - Main.screenPosition.X), (int)(drawInfo.Position.Y - 8 - Main.screenPosition.Y));
				float scale = 1 + (float)(Math.Sin(Main.timeForVisualEffects / 30) * .05f);

				DrawData drawData = new DrawData(texture, position, drawFrame, color, drawInfo.drawPlayer.headRotation, drawInfo.rotationOrigin + new Vector2(25, 2), scale, spriteEffects, 0);
				drawInfo.DrawDataCache.Add(drawData);
			}
		}
	}
}