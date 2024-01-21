using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mounts.RlyehianMount
{
	public class RlyehianMount_Proj : ModProjectile
	{
		private readonly int timeLeftMax = 42;
		private int Length
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 8;

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = timeLeftMax;
			Projectile.extraUpdates = 1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1; // 1 hit per npc max
		}

		public override void AI()
		{
			Projectile.Center = Main.player[Projectile.owner].Center;
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
			}
		}

		public override bool ShouldUpdatePosition() => false;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 offset = new Vector2(Length, 0).RotatedBy(Projectile.rotation);
			projHitbox.X += (int)offset.X;
			projHitbox.Y += (int)offset.Y;
			return projHitbox.Intersects(targetHitbox);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			for (int i = 0; i < 6; i++)
				Dust.NewDustPerfect(target.Center, DustID.Blood, (Projectile.DirectionFrom(target.Center) * Main.rand.NextFloat(2.0f, 4.0f)).RotatedByRandom(1f), 0, default, Main.rand.NextFloat(0.8f, 1.3f));
			modifiers.DisableCrit();
		}

		public override bool PreDraw(ref Color lightColor)
		{
			int hFrameCount = 2;
			Rectangle frame = new Rectangle(0, Projectile.frame * (TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]),
				TextureAssets.Projectile[Projectile.type].Value.Width / hFrameCount - 2, TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type] - 2);

			float maxValue = timeLeftMax / 2;
			float scalar = (float)(Projectile.timeLeft / maxValue);
			if (scalar > 1)
				scalar -= (scalar - 1) * 2;

			Vector2 scale = new Vector2(1f - scalar, 1f + (scalar / 2)) * Projectile.scale;
			int tentacleLength = (Length / frame.Width) + 1;
			for (int i = 0; i < tentacleLength; i++)
			{
				Vector2 position = Projectile.Center + new Vector2(frame.Width * i * scale.X, 0).RotatedBy(Projectile.rotation);
				if (i == (tentacleLength - 1))
					frame.X = frame.Width + 2;
				Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position - Main.screenPosition, frame, Projectile.GetAlpha(lightColor),
					Projectile.rotation, frame.Size() * 0.5f, scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}