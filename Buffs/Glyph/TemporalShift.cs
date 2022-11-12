using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Glyph
{
	public class TemporalShift : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Temporal Shift");
			Description.SetDefault("Double tap to dash and gain a temporary speed boost");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
			Main.pvpBuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();

			if (player.whoAmI != Main.myPlayer && player.buffTime[buffIndex] > 1) {
				player.buffTime[buffIndex]--;
			}

			if (player.buffTime[buffIndex] > 1) {
				modPlayer.phaseShift = true;
				Main.buffNoTimeDisplay[Type] = false;
			}
			else if (player.GetModPlayer<DashPlayer>().phaseStacks > 0) {
				player.buffTime[buffIndex] = 2;
				Main.buffNoTimeDisplay[Type] = true;
			}
			else if (player.whoAmI == Main.myPlayer) {
				player.DelBuff(buffIndex--);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
		{
			MyPlayer modPlayer = Main.LocalPlayer.GetSpiritPlayer();
			var texture = Mod.Assets.Request<Texture2D>("Buffs/Glyph/TemporalShift_" + (Main.LocalPlayer.GetModPlayer<DashPlayer>().phaseStacks - 1)).Value;
			if (modPlayer.divineStacks == 0)
				texture = Mod.Assets.Request<Texture2D>("Buffs/Glyph/TemporalShift").Value;

			spriteBatch.Draw(texture, drawParams.Position, drawParams.DrawColor);
			return false;
		}

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			MyPlayer modPlayer = Main.LocalPlayer.GetSpiritPlayer();
			if (modPlayer.phaseShift) {
				tip = "High speed and immunity to all movement impairment";
			}
		}
	}
}
