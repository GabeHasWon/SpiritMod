using SpiritMod.Mechanics.VolleyballSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.ByBiome.Ocean.Misc.VolleyballContent;

[Sacrifice(1)]
internal class VolleyballValidator : ModItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 26;
		Item.rare = ItemRarityID.White;
		Item.value = Item.buyPrice(copper: 20);
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 20;
		Item.noMelee = true;
		Item.autoReuse = true;
		Item.noUseGraphic = true;
		Item.UseSound = SoundID.Item1;
		Item.consumable = false;
	}

	public override bool? UseItem(Player player)
	{
		VolleyballCourts.TryAddCourt(new Point(Player.tileTargetX, Player.tileTargetY));
		return true;
	}
}