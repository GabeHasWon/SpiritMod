using Terraria;
using Terraria.ModLoader;


namespace SpiritMod.Buffs.Potion
{
	public class FlightPotionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soaring");
			Description.SetDefault("Increases flight duration by 25%.");
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();
			modPlayer.WingTimeMaxMultiplier += 0.25f;
		}
	}
}
