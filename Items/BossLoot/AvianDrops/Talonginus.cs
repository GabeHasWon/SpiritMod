using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Held;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AvianDrops
{
	public class Talonginus : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Talonginus");
			Tooltip.SetDefault("Extremely quick, but inaccurate");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.sellPrice(0, 1, 30, 0);
			Item.rare = ItemRarityID.Green;
			Item.crit = 6;
			Item.damage = 24;
			Item.knockBack = 2.5f;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 7;
			Item.useAnimation = 7;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<TalonginusProj>();
			Item.shootSpeed = 2f;
			Item.UseSound = SoundID.Item1;
		}

		public override bool CanUseItem(Player player) => !(player.ownedProjectileCounts[Item.shoot] > 0);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
			=> velocity = velocity.RotatedByRandom(0.45f);
	}
}