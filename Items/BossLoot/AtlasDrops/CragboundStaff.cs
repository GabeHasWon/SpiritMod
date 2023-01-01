using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace SpiritMod.Items.BossLoot.AtlasDrops
{
	public class CragboundStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cragbound Staff");
			Tooltip.SetDefault("A tiny Earthen Guardian rains down energy for you \nOccasionally inflicts foes with 'Unstable Affliction'");
		}


		public override void SetDefaults()
		{
			Item.height = Item.width = 54;
			Item.value = Item.sellPrice(0, 8, 45, 0);
			Item.rare = ItemRarityID.Cyan;
			Item.mana = 20;
			Item.damage = 112;
			Item.knockBack = 7;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<CragboundMinion>();
			Item.UseSound = SoundID.Item44;
			Item.sentry = true; 
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.FindSentryRestingSpot(type, out int worldX, out int worldY, out int pushYUp);
			worldY -= 16;
			Projectile.NewProjectile(source, worldX, worldY - pushYUp, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
			player.UpdateMaxTurrets();
			return false;
		}
	}
}