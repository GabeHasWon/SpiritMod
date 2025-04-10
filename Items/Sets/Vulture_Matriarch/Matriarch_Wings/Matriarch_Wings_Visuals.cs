using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SpiritMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.Vulture_Matriarch.Matriarch_Wings;

public class Matriarch_Wings_Visuals : ModPlayer
{
	public float impact;
	public float storedVelocityY;
	public bool wingsEquipped = false;

	public bool IsDiving => wingsEquipped && ((Player.gravDir == -1) ? storedVelocityY < 0 : storedVelocityY > 0);
	public override void ResetEffects() => wingsEquipped = false;

	public override void ModifyScreenPosition()
	{
		if (wingsEquipped)
			impact = MathHelper.Lerp(impact, 0, 0.05f);
		else
			impact = 0;

		Main.screenPosition.Y += impact * ModContent.GetInstance<SpiritClientConfig>().ScreenShake;
	}
}

public class VultureMatriachWingsLayer : PlayerDrawLayer
{
	private static Asset<Texture2D> WingsGlow;

	public override void Load() => WingsGlow = Mod.Assets.Request<Texture2D>("Items/Sets/Vulture_Matriarch/Matriarch_Wings/Matriarch_Wings_Wings");
	public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);

	protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		var player = drawInfo.drawPlayer;

		if (!player.active || player.dead || drawInfo.hideEntirePlayer)
			return;

		if (player.GetModPlayer<Matriarch_Wings_Visuals>().IsDiving && player.wings == Matriarch_Wings.EquipSlot)
		{
			Vector2 directions = player.Directions;
			var vector = drawInfo.Position - Main.screenPosition + new Vector2(player.width / 2, player.height - player.bodyFrame.Height / 2) + new Vector2(0, 7);

			Color color = player.GetImmuneAlphaPure(Color.Goldenrod with { A = 0 }, drawInfo.shadow);
			Texture2D texture = WingsGlow.Value;

			var frame = texture.Frame(1, 4, 0, drawInfo.drawPlayer.wingFrame, 0, 0);
			var origin = frame.Size() / 2;

			DrawData drawData = new DrawData(texture, (vector + new Vector2(-9, 2) * directions).Floor(), frame, color, drawInfo.drawPlayer.bodyRotation, origin, 1, drawInfo.playerEffect, 0);
			drawInfo.DrawDataCache.Add(drawData);
		}
	}
}