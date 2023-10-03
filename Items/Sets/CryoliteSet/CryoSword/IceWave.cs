using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Buffs.DoT;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using SpiritMod.Particles;

namespace SpiritMod.Items.Sets.CryoliteSet.CryoSword
{
	public class IceWave : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int counterMax = 40;

		private bool NoDamage
		{
			get => (int)Projectile.ai[1] != 0;
			set => Projectile.ai[1] = value ? 1 : 0;
		}

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cryo Pillar");
			Main.projFrames[Type] = 5;
		}

		public override void SetDefaults()
		{
			Projectile.width = 78;
			Projectile.height = 90;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.aiStyle = -1;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void OnSpawn(IEntitySource source) => Projectile.rotation = Projectile.velocity.ToRotation();

		public override void AI()
		{
			Projectile.velocity *= 0.98f;

			int fadeoutTime = 8;

			if (++Counter > counterMax)
				Projectile.Kill();
			else if (Counter > (counterMax - fadeoutTime))
				Projectile.scale -= 1f / fadeoutTime;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CryoCrush>(), 300);

		public override bool? CanDamage() => !NoDamage;

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			Vector2 newSize = new Vector2(34, 100);
			hitbox = new Rectangle((int)Projectile.position.X + (Projectile.width / 2) - (int)(newSize.X / 2), (int)Projectile.position.Y + (Projectile.height / 2) - (int)(newSize.Y / 2), (int)newSize.X, (int)newSize.Y);
		}

		public override void OnKill(int timeLeft)
		{
			Vector2 velocity = Vector2.UnitX.RotatedBy(Projectile.rotation);

			for (int i = 0; i < 4; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(2) ? DustID.GemDiamond : DustID.DungeonSpirit, velocity.X * 3, velocity.Y * 3, 80, default, Main.rand.NextFloat(1.0f, 2.0f));
				dust.noGravity = true;

				if (i < 2 && !Main.dedServ)
					ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(10.0f)), (velocity * Main.rand.NextFloat(1.0f, 1.8f)).RotatedByRandom(0.5f), Color.White, Color.Blue, Main.rand.NextFloat(0.15f, 0.45f), 30));
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			int numColumns = 2;
			Rectangle rect = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width / numColumns, (texture.Height / Main.projFrames[Type]) - 2);
			
			Vector2 scale = new Vector2(1, Math.Min(Counter / 8f, 1)) * Projectile.scale;
			Vector2 pos = Projectile.Center + (Main.rand.NextVector2Unit() * (float)((float)Counter / counterMax)) - (Vector2.UnitY * Projectile.gfxOffY);

			for (int i = 0; i < 2; i++)
			{
				Color color = (i == 0) ? lightColor : Color.White * (float)(1f - Projectile.scale);
				if (i > 0)
					rect.X += texture.Width / numColumns;

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, rect, Projectile.GetAlpha(color), Projectile.rotation, rect.Size() / 2, scale, SpriteEffects.None, 0);
			}
			
			return false;
		}
	}
}