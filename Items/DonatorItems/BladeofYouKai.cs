using Microsoft.Xna.Framework;
using SpiritMod.Items.BossLoot.DuskingDrops;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace SpiritMod.Items.DonatorItems
{
	public class BladeofYouKai : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blade of the You-Kai");
			// Tooltip.SetDefault("Melee hits on enemies may emit Shadowflame Embers\nInflicts Shadowflame");
		}

		public override void SetDefaults()
		{
			Item.damage = 52;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.DamageType = DamageClass.Melee;
			Item.width = 48;
			Item.height = 48;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = 25700;
			Item.rare = ItemRarityID.Pink;
			Item.crit = 7;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true;
		}


		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.ShadowbeamStaff);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].velocity *= 0f;
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.ShadowFlame, 180, true);
			if (Main.rand.NextBool(4)) {
				Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<ShadowEmber>(), damageDone, hit.Knockback, player.whoAmI, 0f, 0f);
				Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center.X, target.Center.Y, 1f, 0f, ModContent.ProjectileType<ShadowEmber>(), damageDone, hit.Knockback, player.whoAmI, 0f, 0f);
				Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center.X, target.Center.Y, -2f, 0f, ModContent.ProjectileType<ShadowEmber>(), damageDone, hit.Knockback, player.whoAmI, 0f, 0f);
			}
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<DuskStone>(), 5);
			recipe.AddIngredient(ItemID.SoulofNight, 30);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}