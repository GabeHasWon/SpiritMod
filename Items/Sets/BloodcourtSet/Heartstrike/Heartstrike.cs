using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using SpiritMod.Particles;

namespace SpiritMod.Items.Sets.BloodcourtSet.Heartstrike
{
	public class Heartstrike : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heartstrike");
			Tooltip.SetDefault("Successful hits grant charges, which can fired using right click\nFired charges inflict 'Surging Anguish', slowly depleting enemy life");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 24;
			Item.height = 46;
			Item.useTime = 31;
			Item.useAnimation = 31;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.knockBack = 1.5f;
			Item.value = 22500;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item5;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.channel = true;
			Item.shootSpeed = 8f;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, .42f, .02f, .13f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override bool AltFunctionUse(Player player) => player.GetModPlayer<HeartstrikePlayer>().charges > 0;
		public override void HoldItem(Player player) => player.GetModPlayer<HeartstrikePlayer>().active = true;

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, new Vector2(10, 0).RotatedBy(velocity.ToRotation()), ModContent.ProjectileType<HeartstrikeProj>(), damage, knockback, player.whoAmI, ai1: (player.altFunctionUse == 2) ? 1 : 0);

			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 12f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			if (player.altFunctionUse != 2)
			{
				Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);

				if (proj.TryGetGlobalProjectile(out HeartstrikeGProj globalProj))
					globalProj.heartStruck = true;
			}
			else
			{
				player.GetModPlayer<HeartstrikePlayer>().charges--;
				for (int i = 0; i < 12; i++)
				{
					Dust dust = Dust.NewDustPerfect(position, DustID.LavaMoss, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(1.0f, 1.5f));
					dust.velocity = (velocity * Main.rand.NextFloat(0.2f, 0.5f)).RotatedByRandom(0.8f);
					dust.noGravity = true;
					dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
				}
			}

			return false;
		}

		public static void DoChargeVisual(Player player)
		{
			Vector2 position = player.Center + new Vector2(20 * -player.direction, -10);

			for (int i = 0; i < 12; i++)
			{
				Dust dust = Dust.NewDustPerfect(position + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(18.0f, 30.0f)), DustID.LavaMoss, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(1.0f, 1.5f));
				dust.velocity = dust.position.DirectionTo(position) * (dust.position.Distance(position) / 12);
				dust.noGravity = true;
				dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
				dust.customData = player;

				if (i < 3)
				{
					Vector2 randomPos = position + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(18.0f, 30.0f));
					int timeAlive = Main.rand.Next(14, 24);

					ParticleHandler.SpawnParticle(new ImpactLine(randomPos, randomPos.DirectionTo(position) * (randomPos.Distance(position) / (timeAlive + 2)), Color.Magenta, Vector2.One * 0.8f, timeAlive, player));
				}
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<DreamstrideEssence>(), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}

	public class HeartstrikePlayer : ModPlayer
	{
		public bool active = false;
		public int charges;

		public override void ResetEffects()
		{
			if (!active)
				charges = 0;

			active = false;
		}
	}

	public class HeartstrikeGProj : GlobalProjectile
	{
		public bool heartStruck = false;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.arrow || (entity.ModProjectile != null && entity.ModProjectile.AIType == ProjectileID.WoodenArrowFriendly);

		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{
			if (heartStruck)
			{
				if (!Main.dedServ)
					Heartstrike.DoChargeVisual(Main.player[projectile.owner]);

				Main.player[projectile.owner].GetModPlayer<HeartstrikePlayer>().charges++;
			}
		}
	}
}