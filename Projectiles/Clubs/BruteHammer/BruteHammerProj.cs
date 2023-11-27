using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using SpiritMod.Mechanics.Trails;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Localization;

namespace SpiritMod.Projectiles.Clubs.BruteHammer
{
	public class BruteHammerProj : ClubProj, IManualTrailProjectile
	{
		public BruteHammerProj() : base(new Vector2(50, 80)) { }

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.BruteHammer.DisplayName");

		public override void SafeSetStaticDefaults() => Main.projFrames[Projectile.type] = 2;

		public override void SafeSetDefaults() => animMax = 360;

		public void DoTrailCreation(TrailManager tM) => tM.CreateCustomTrail(new BruteHammerTrail(Projectile, new Color(123, 19, 19), 18, 10));

		private float spinTimer;
		private bool spawnedTrail = false;

		public override float TrueRotation => (float)radians + ((Effects == SpriteEffects.FlipHorizontally) ? MathHelper.PiOver2 + 3.6f : 4.2f);

		public override bool? CanDamage() => (_lingerTimer == 0) ? null : false;

		public override void Smash(Vector2 position)
		{
			for (int i = 0; i < 100; i++)
			{
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), ModContent.DustType<Dusts.EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);

				if (i < 50)
					Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustID.Blood, Main.player[Projectile.owner].DirectionTo(Projectile.Center).RotatedBy(Main.rand.NextFloat(-1.57f, 0f) * Main.player[Projectile.owner].direction) * Main.rand.NextFloat(1.0f, 7.0f), Scale: Main.rand.NextFloat(0.5f, 1.5f));
			}
		}

		public override void DrawTrail(Color lightColor)
		{
			if (_lingerTimer == 0)
			{
				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
					Color trailColor = lightColor * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * .5f;
					Main.EntitySpriteDraw(texture, drawPos, texture.Frame(1, Main.projFrames[Type]), trailColor, Projectile.oldRot[k], Origin, Projectile.scale, Effects, 0);
				}
			}
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (player.channel && !released)
			{
				float rate = 24f;
				spinTimer += Projectile.ai[0] / ChargeTime * rate * player.direction;

				radians = spinTimer * (Math.PI / 180);

				Projectile.position.X = player.Center.X - (int)(Math.Cos(radians * 0.96) * Size.X) - (Projectile.width / 2);
				Projectile.position.Y = player.Center.Y - (int)(Math.Sin(radians * 0.96) * Size.Y) - (Projectile.height / 2);

				float rotation = (float)radians + 1.7f;
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);
				player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);
			}
			else if (!spawnedTrail)
			{
				spawnedTrail = true;
				TrailManager.ManualTrailSpawn(Projectile);
			}

			Projectile.rotation = TrueRotation; //This is set for drawing afterimages
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (!released)
			{
				modifiers.HitDirectionOverride = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);
				modifiers.FinalDamage.Flat = (int)(MinDamage * .6f);
				modifiers.Knockback.Base = MinKnockback * .5f;

				Projectile.numHits--;
			}
		}
	}
}