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
			Tooltip.SetDefault("Extremely quick, but innacurate");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.sellPrice(0, 1, 30, 0);
			Item.rare = ItemRarityID.Green;
			Item.crit = 6;
			Item.damage = 21;
			Item.knockBack = 2.5f;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 9;
			Item.useAnimation = 9;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<TalonginusProj>();
			Item.shootSpeed = 15f;
			Item.UseSound = SoundID.Item1;
		}

		public override bool CanUseItem(Player player) => !(player.ownedProjectileCounts[Item.shoot] > 0);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 origVect = velocity;
			Vector2 newVect;
			if (Main.rand.NextBool(2))
				newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(82, 1800) / 10));
			else
				newVect = origVect.RotatedBy(-System.Math.PI / (Main.rand.Next(82, 1800) / 10));

			velocity.X = newVect.X;
			velocity.Y = newVect.Y;
		}
	}
}