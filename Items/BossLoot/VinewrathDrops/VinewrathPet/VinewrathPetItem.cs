using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Pet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.VinewrathDrops.VinewrathPet
{
	internal class VinewrathPetItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Withered Petal");
			Tooltip.SetDefault("Summons an angry Seedling companion");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 32;
			Item.height = 20;
			Item.shoot = ModContent.ProjectileType<VinewrathPetProjectile>();
			Item.buffType = ModContent.BuffType<VinewrathPetBuff>();
			Item.UseSound = SoundID.NPCDeath6;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
				player.AddBuff(Item.buffType, 3600, true);
		}

		public override bool CanUseItem(Player player) => player.miscEquips[0].IsAir;
	}
}
