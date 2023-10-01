using SpiritMod.Mechanics.SportSystem;
using SpiritMod.Mechanics.SportSystem.Volleyball;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.ByBiome.Ocean.Misc.VolleyballContent;

[Sacrifice(1)]
internal class VolleyballCourtChecker : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	public override void SetDefaults()
	{
		Item.width = Item.height = 26;
		Item.rare = ItemRarityID.White;
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
		if (player.whoAmI == Main.myPlayer)
		{
			if (SportCourts.TryAddCourt(new Point(Player.tileTargetX, Player.tileTargetY), new VolleyballGameTracker()) && Main.netMode == NetmodeID.MultiplayerClient)
				SportsSyncing.PlaceOrSyncCourt<VolleyballGameTracker>((byte)player.whoAmI, new(Player.tileTargetX, Player.tileTargetY));
		}
		return true;
	}
}