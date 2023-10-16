using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Held;

public class MarbleBidentProj : ModProjectile
{
	private const int ExtensionSize = 20;

	private float MeleeSpeed => (Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee) - 1) * 10 + 1;

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.Trident);
		AIType = ProjectileID.Trident;
	}

	public override void AI()
	{
		Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 2;

		Dust newDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 0, default, 1f)];
		newDust.position = position;
		newDust.velocity = Projectile.velocity.RotatedBy(Math.PI / 2, default) * 0.33F + Projectile.velocity / 4;
		newDust.position += Projectile.velocity.RotatedBy(Math.PI / 2, default);
		newDust.fadeIn = 0.5f;
		newDust.noGravity = true;
		newDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 0, default, 1)];
		newDust.position = position;
		newDust.velocity = Projectile.velocity.RotatedBy(-Math.PI / 2, default) * 0.33F + Projectile.velocity / 4;
		newDust.position += Projectile.velocity.RotatedBy(-Math.PI / 2, default);
		newDust.fadeIn = 0.5F;
		newDust.noGravity = true;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.rand.NextBool(4) && !target.SpawnedFromStatue)
			target.AddBuff(BuffID.Midas, 180);
		if (!target.SpawnedFromStatue && Main.rand.NextBool(4) && target.HasBuff(BuffID.Midas))
			Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center.X, target.Center.Y, 0, 0, ModContent.ProjectileType<GildedFountain>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteEffects effects = (Projectile.direction < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		float rotation = Projectile.rotation + (Projectile.direction >= 0 ? MathHelper.PiOver4 : -MathHelper.PiOver4);
		Texture2D wave = TextureAssets.Extra[98].Value;
		float scale = Projectile.Distance(Main.player[Projectile.owner].Center) / 120f;
		Vector2 origin = new Vector2(wave.Width / 2f, wave.Height * 0.75f);
		var wavePos = Projectile.Center - Main.screenPosition - Projectile.velocity * 4;
		scale *= (MeleeSpeed - 1) * 0.2f + 1; // Scale to melee speed

		for (int i = 0; i < 2; ++i)
		{
			float rot = rotation - (MathHelper.PiOver4 / 2f) + (MathHelper.PiOver4 * i);
			Main.EntitySpriteDraw(wave, wavePos, null, Color.Gold * 0.5f, rot, origin, Projectile.scale * scale, effects, 0);
			Main.EntitySpriteDraw(wave, wavePos, null, Color.Gold * 0.35f, rot, origin, new Vector2(Projectile.scale * 0.75f, Projectile.scale * 1.25f) * scale, effects, 0);
			Main.EntitySpriteDraw(wave, wavePos, null, Color.Gold * 0.2f, rot, origin, new Vector2(Projectile.scale * 0.5f, Projectile.scale * 1.5f) * scale, effects, 0);
		}
		return true;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		int halfSize = ExtensionSize / 2;
		Point hitbox = (Projectile.Center - new Vector2(halfSize) + (Projectile.velocity * 0.2f * MeleeSpeed)).ToPoint();

		if (targetHitbox.Intersects(new Rectangle(hitbox.X, hitbox.Y, ExtensionSize, ExtensionSize)))
			return true;

		return base.Colliding(projHitbox, targetHitbox);
	}
}