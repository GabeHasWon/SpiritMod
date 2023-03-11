using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.ByBiome.Forest.Consumeable;

public class EnchantedApple : FoodItem
{
	internal override Point Size => new(20, 22);
	public override void StaticDefaults() => Tooltip.SetDefault("Minor improvements to all stats\nIncreases mana regeneration");

	public override bool CanUseItem(Player player)
	{
		player.AddBuff(BuffID.ManaRegeneration, 3600);
		return true;
	}
}
