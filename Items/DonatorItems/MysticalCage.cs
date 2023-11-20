using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Pet;
using SpiritMod.Projectiles.Pet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.DonatorItems
{
	[Sacrifice(1)]
	public class MysticalCage : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.value = 25000;
			Item.rare = ItemRarityID.Orange;
			Item.shoot = ModContent.ProjectileType<CultfishPet>();
			Item.buffType = ModContent.BuffType<CaltfistPetBuff>();
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
				player.AddBuff(Item.buffType, 3600, true);
		}

		public override bool CanUseItem(Player player) => player.miscEquips[1].IsAir;
	}
}