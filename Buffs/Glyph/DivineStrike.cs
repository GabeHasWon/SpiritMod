using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Glyph
{
	public class DivineStrike : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Divine Strike");
			Description.SetDefault("Your next attack will deal ");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override bool ReApply(Player player, int time, int buffIndex)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();
			if (modPlayer.divineStacks < 6)
				modPlayer.divineStacks++;
			return false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();

			if (modPlayer.glyph == GlyphType.Radiant)
				player.buffTime[buffIndex] = 2;
			else
				player.DelBuff(buffIndex--);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
		{
			MyPlayer modPlayer = Main.LocalPlayer.GetSpiritPlayer();
			string texturePath = "Buffs/Glyph/DivineStrike";
			if (modPlayer.divineStacks > 1)
				texturePath += $"_{modPlayer.divineStacks - 1}";
			spriteBatch.Draw(Mod.Assets.Request<Texture2D>(texturePath).Value, drawParams.Position, drawParams.DrawColor);
			return false;
		}

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			MyPlayer modPlayer = Main.LocalPlayer.GetSpiritPlayer();
			tip += $"{modPlayer.divineStacks * 11}% more damage.";
		}
	}
}