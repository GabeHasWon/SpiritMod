using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using SpiritMod.Buffs.DoT;
using SpiritMod.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class SpiritGlobalProjectile : GlobalProjectile
	{
		public override bool InstancePerEntity => true;

		public List<SpiritProjectileEffect> effects = new();

		public bool witherLeaf = false;
		public bool shotFromBismiteBow = false;
		public bool shotFromHolyBurst = false;
		public bool shotFromTrueHolyBurst = false;
		public bool shock = false;

		public bool throwerGloveBoost = false;

		public int storedUseTime = 0;

		public override bool PreDraw(Projectile projectile, ref Color lightColor)
		{
			if (throwerGloveBoost)
			{
				Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[projectile.type].Value.Width * 0.5f, projectile.height * 0.5f);
				for (int k = 0; k < projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
					Color color = projectile.GetAlpha(Color.Cyan with { A = 0 }) * ((float)(projectile.oldPos.Length - k) / projectile.oldPos.Length);

					Main.EntitySpriteDraw(TextureAssets.Projectile[projectile.type].Value, drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0);
				}
			}

			return base.PreDraw(projectile, ref lightColor);
		}

		public override void PostDraw(Projectile projectile, Color lightColor)
		{
			if (throwerGloveBoost)
				projectile.QuickDraw(Main.spriteBatch, drawColor: projectile.GetAlpha(Color.Cyan with { A = 0 }));
		}

		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			if (projectile.friendly)
			{
				MyPlayer modPlayer = Main.player[projectile.owner].GetModPlayer<MyPlayer>();

				if (projectile.IsRanged() && modPlayer.throwerGlove && modPlayer.throwerStacks >= 7) //Thrower glove functionality
				{
					projectile.extraUpdates += 1;
					projectile.scale *= 1.1f;
					projectile.damage = (int)(projectile.damage * 1.25f);
					projectile.knockBack += 2;

					throwerGloveBoost = true;
					modPlayer.throwerStacks = 0;
				}
			}

			if (source is EntitySource_ItemUse item)
				storedUseTime = item.Item.useTime;
		}

		public override bool PreAI(Projectile projectile)
		{
			foreach (var effect in effects)
			{
				if (effect != null && !effect.ProjectilePreAI(projectile))
					return false;
			}

			if (witherLeaf)
			{
				projectile.rotation = projectile.velocity.ToRotation() + 1.57f;
				if (Main.rand.NextBool())
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Plantera_Green);
					return true;
				}
			}

			if (shock)
			{
				for (int k = 0; k < 3; k++)
				{
					Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.Electric, Scale: .58f);
					dust.noGravity = true;
					dust.velocity = Vector2.Zero;
				}
			}

			if (throwerGloveBoost && Main.rand.NextBool(3))
				Dust.NewDustPerfect(projectile.Center, DustID.Electric, (projectile.velocity * .5f).RotatedByRandom(.1f), 100, default, Main.rand.NextFloat(.2f, .5f)).noGravity = true;

			if (projectile.owner >= Main.maxPlayers || projectile.owner <= -1) //Check if owner is invalid
				return true;

			return BowEffects(projectile);
		}

		private bool BowEffects(Projectile projectile)
		{
			if (shotFromHolyBurst || shotFromTrueHolyBurst)
			{
				for (int i = 0; i < 4; i++)
				{
					int num = Dust.NewDust(projectile.Center + new Vector2(0, (float)Math.Cos(Main.timeForVisualEffects / 8.2f) * 9.2f).RotatedBy(projectile.rotation), 6, 6, DustID.Clentaminator_Purple, 0f, 0f, 0, default, 1f);
					Main.dust[num].velocity *= .1f;
					Main.dust[num].scale *= .7f;
					Main.dust[num].noGravity = true;
				}
				return false;
			}
			else if (shotFromBismiteBow)
			{
				if (Main.rand.NextBool(20))
					DustHelper.DrawTriangle(projectile.Center, 167, 1, .8f, 1.1f);
			}
			return true;
		}

		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{
			foreach (var effect in effects)
				effect.ProjectileOnHitNPC(projectile, target, damage, knockback, crit);

			Player player = Main.player[projectile.owner];
			MyPlayer modPlayer = player.GetSpiritPlayer();

			if (shock)
			{
				if (Main.rand.NextBool(12) || (player.GetSpiritPlayer().starSet && Main.rand.NextBool(8)))
				{
					target.AddBuff(ModContent.BuffType<ElectrifiedV2>(), Main.rand.Next(60, 120));

					for (int k = 0; k < 40; k++)
						Dust.NewDust(target.position, target.width, target.height, DustID.Electric, 2.5f, -2.5f, 0, Color.White, Main.rand.NextFloat(0.2f, 0.9f));
				}
			}

			if (shotFromHolyBurst)
				target.AddBuff(ModContent.BuffType<AngelLight>(), 60);

			if (shotFromTrueHolyBurst)
				target.AddBuff(ModContent.BuffType<AngelWrath>(), 60);

			if (witherLeaf)
				target.AddBuff(ModContent.BuffType<WitheringLeaf>(), 180);

			if (modPlayer.HealCloak && projectile.minion && Main.rand.NextBool(25))
			{
				player.HealEffect(4);
				player.statLife += 4;
			}

			if (modPlayer.SpiritCloak && projectile.minion && Main.rand.NextBool(15))
			{
				player.HealEffect(9);
				player.statLife += 9;
			}

			if (modPlayer.VampireCloak && projectile.minion && Main.rand.Next(100) < 30)
			{
				player.HealEffect(3);
				player.statLife += 3;
			}

			if (shotFromBismiteBow && Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<FesteringWounds>(), 120, true);

			// Jellynaut Helmet
			if (modPlayer.jellynautHelm && modPlayer.jellynautStacks < 4 && projectile.IsMagic() && (target.life <= 0 || Main.rand.NextBool(8)) && !target.friendly && !target.SpawnedFromStatue)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 position = projectile.position + (Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 20);
					Projectile.NewProjectileDirect(projectile.GetSource_OnHit(target), position, new Vector2(1, -1), ModContent.ProjectileType<Magic.JellynautOrbiter>(), 0, 0, player.whoAmI).scale = Main.rand.NextFloat(.5f, 1f);

					modPlayer.jellynautStacks++;
				}
			}
		}

		public override void Kill(Projectile projectile, int timeLeft)
		{
			if (Main.netMode != NetmodeID.Server)
				Mechanics.Trails.TrailManager.TryTrailKill(projectile);
		}
	}
}