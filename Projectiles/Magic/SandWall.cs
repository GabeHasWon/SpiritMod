using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.Trails;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace SpiritMod.Projectiles.Magic
{
	public class SandWall : ModProjectile, ITrailProjectile
	{
		private Vector2 initialVel;

		float distance = 5f;
		readonly int rotationalSpeed = 2;
		float initialSpeedMult = 1;

		private float Counter
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Sand Wall");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.Size = new Vector2(15);
			Projectile.friendly = true;
			Projectile.penetrate = 5;
			Projectile.alpha = 255;
			Projectile.timeLeft = 450;
			Projectile.extraUpdates = 2;
			Projectile.tileCollide = false;
		}

		public void DoTrailCreation(TrailManager tM)
		{
			tM.CreateTrail(Projectile, new StandardColorTrail(new Color(255, 236, 115, 200)), new RoundCap(), new DefaultTrailPosition(), 100f, 130f, new ImageShader(Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
			tM.CreateTrail(Projectile, new StandardColorTrail(Color.White * 0.2f), new RoundCap(), new DefaultTrailPosition(), 12f, 80f, new DefaultShader());
			tM.CreateTrail(Projectile, new StandardColorTrail(Color.White * 0.2f), new RoundCap(), new DefaultTrailPosition(), 12f, 80f, new DefaultShader());
			tM.CreateTrail(Projectile, new StandardColorTrail(Color.Gold * 0.4f), new RoundCap(), new DefaultTrailPosition(), 20f, 250f, new DefaultShader());
		}

		public override void OnSpawn(IEntitySource source) => initialVel = Projectile.velocity;

		public override void AI()
		{
			if (Projectile.timeLeft < 360)
				Projectile.tileCollide = true;

			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin);
				dust.velocity = Vector2.Zero;
				dust.noGravity = true;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			distance += 0.025f;
			initialSpeedMult += 0.01f;
			Counter += rotationalSpeed;

			Vector2 offset = Vector2.Normalize((initialVel * initialSpeedMult).RotatedBy(Math.PI / 2));
			offset *= (float)(Math.Cos(Counter * (Math.PI / 180)) * (distance / 3));
			Projectile.velocity = (initialVel * initialSpeedMult) + offset;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (!target.boss && target.velocity != Vector2.Zero && target.knockBackResist != 0)
				target.velocity.Y = -4f;
		}
	}
}
