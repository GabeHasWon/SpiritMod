using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ThrownMisc.FlaskofGore
{
	public class CrimsonSkull : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crimson Skull");
			Tooltip.SetDefault("You shouldn't see this");
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 7));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
		}
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 32;
			Item.maxStack = 1;
			Item.alpha = 50;
			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

		public override bool ItemSpace(Player player) => true;
		public override void GrabRange(Player player, ref int grabRange) => grabRange = 0;

		public override bool OnPickup(Player player)
		{
			player.AddBuff(Mod.Find<ModBuff>("CrimsonSkullBuff").Type, 240);
			SoundEngine.PlaySound(SoundID.DD2_DrakinDeath with { Volume = 0.8f }, player.Center);
			return false;
		}
	}
}
