using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.RlyehianDrops
{
	public class TentacleChain : ModItem
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Brine Barrage");

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 44;
			Item.rare = ItemRarityID.Orange;
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 16;
			Item.useTime = 16;
			Item.knockBack = 0;
			Item.value = Item.sellPrice(0, 1, 20, 0);
			Item.damage = 23;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<TentacleChainProj>();
			Item.shootSpeed = 22f;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.channel = true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			float radius = 20f;
			float direction = Main.rand.NextFloat(0.25f, 1f) * Main.rand.NextBool().ToDirectionInt() * MathHelper.ToRadians(radius);
			Projectile.NewProjectile(source, player.RotatedRelativePoint(player.MountedCenter), velocity, type, damage, knockback, player.whoAmI, 0f, direction);

			return false;
		}
	}
}