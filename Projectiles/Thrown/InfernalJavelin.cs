using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SpiritMod.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Thrown;

public class InfernalJavelin : ModProjectile
{
	private static Asset<Texture2D> glow;

	public override void Load() => glow = ModContent.Request<Texture2D>(Texture + "_Glow");

	public override void SetDefaults()
	{
		Projectile.Size = new Vector2(16);
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = 1;
		Projectile.alpha = 255;
	}

	public override bool PreAI()
	{
		if (Main.rand.NextBool(2))
		{
			var dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
			dust.scale *= 1.5f;
			dust.noGravity = true;
		}

		if (Projectile.alpha > 0)
			Projectile.alpha -= 25;

		if (Projectile.alpha < 0)
			Projectile.alpha = 0;

		if (++Projectile.ai[1] >= 45f)
		{
			Projectile.ai[1] = 45f;
			Projectile.velocity.X = Projectile.velocity.X * 0.98F;
			Projectile.velocity.Y = Projectile.velocity.Y + 0.35F;
		}

		Projectile.rotation = Projectile.velocity.ToRotation() + 2.355f;
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<StackingFireBuff>(), 300);

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
			targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);

		return projHitbox.Intersects(targetHitbox);
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		Vector2 vector9 = Projectile.position;
		Vector2 value19 = (Projectile.rotation - 1.57079637f).ToRotationVector2();
		vector9 += value19 * 16f;

		for (int i = 0; i < 20; i++)
		{
			var dust = Dust.NewDustDirect(vector9, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default, 1f);
			dust.position = (dust.position + Projectile.Center) / 2f;
			dust.velocity += value19 * 2f;
			dust.velocity *= 0.5f;
			dust.noGravity = true;

			vector9 -= value19 * 8f;
		}

		for (int i = 0; i < 2; ++i)
		{
			int randFire = Main.rand.Next(3);
			var vel = new Vector2(Main.rand.Next(-1000, 1000) / 100, Main.rand.Next(-8, 8));

			var proj = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, vel, ProjectileID.GreekFire1 + randFire, 20, 0, Projectile.owner);
			proj.hostile = false;
			proj.friendly = true;
			proj.netUpdate = true;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = TextureAssets.Projectile[Type].Value;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, Projectile.Size / 2, Projectile.scale, default);
		Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.Size / 2, Projectile.scale, default);

		return false;
	}
}