using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Buffs.DoT;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpiritMod.Items.Sets.CryoliteSet.CryoSword
{
	public class IceWall : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int counterMax = 40;

		private int InitialDir
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void SetStaticDefaults() => DisplayName.SetDefault("Cryo Pillar");

		public override void SetDefaults()
		{
			Projectile.width = 78;
			Projectile.height = 90;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.aiStyle = -1;
			DrawOriginOffsetY = 2;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			if (InitialDir == 0)
				InitialDir = Math.Sign(Projectile.velocity.X);

			Projectile.rotation = 0f;

			Projectile.velocity *= 0.95f;

			if (++Counter > counterMax)
				Projectile.Kill();
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<CryoCrush>(), 300);

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 50; i++)
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceGolem, Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f), 80, default, Main.rand.NextFloat(1.0f, 2.0f));
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 scale = new Vector2(1, Math.Min(Counter / 8f, 1)) * Projectile.scale;
			Vector2 pos = Projectile.Center + (Main.rand.NextVector2Unit() * (float)((float)Counter / counterMax)) - (Vector2.UnitY * Projectile.gfxOffY);

			Main.EntitySpriteDraw(texture, pos - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, scale, (InitialDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
			return false;
		}
	}
}