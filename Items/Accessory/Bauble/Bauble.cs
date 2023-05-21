using SpiritMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Bauble
{
	public class Bauble : AccessoryItem, ITimerItem
	{
		public const int shieldTime = 360;
		public const int cooldown = 3600;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Winter's Bauble");
			Tooltip.SetDefault("Defensive and movement abilities are increased when below half health" +
				"\nDropping below half health creates a shield that nullifies projectiles for 6 seconds" +
				"\n1 minute cooldown");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.accessory = true;
		}

		public override void SafeUpdateAccessory(Player player, bool hideVisual)
		{
			if (player.ItemTimer<Bauble>() == (cooldown - shieldTime))
				player.AddBuff(ModContent.BuffType<BaubleCooldown>(), cooldown - shieldTime);

			if (player.statLife < (player.statLifeMax2 / 2))
			{
				player.endurance += .10f;
				player.moveSpeed += .05f;
			}
		}

		public int TimerCount() => 1;
	}
}
