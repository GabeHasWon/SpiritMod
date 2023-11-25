using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Held;

public class SpiritSpearProjectile : ModProjectile
{
	private const int ExtensionSize = 20;

	private float MeleeSpeed => (Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee) - 1) * 10 + 1;

	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.SpiritSpear.DisplayName");

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.Trident);
		AIType = ProjectileID.Trident;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<SoulBurn>(), 280);

	int timer = 16;

	public override void AI()
	{
		if (timer-- <= 0)
		{
			Vector2 vel = Projectile.velocity * 3;
			int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, vel.X, vel.Y, ModContent.ProjectileType<SoulSpirit>(), 
				Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
			Main.projectile[proj].timeLeft = 15;

			timer = 100;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteEffects effects = (Projectile.direction < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		float rotation = Projectile.rotation + (Projectile.direction >= 0 ? MathHelper.PiOver4 : -MathHelper.PiOver4);
		Texture2D wave = TextureAssets.Extra[98].Value;
		float scale = Projectile.Distance(Main.player[Projectile.owner].Center) / 120f;
		Vector2 origin = new Vector2(wave.Width / 2f, wave.Height * 0.75f);
		var wavePos = Projectile.Center - Main.screenPosition - Projectile.velocity * 4;
		Color baseColor = new(51, 255, 252);
		scale *= (MeleeSpeed - 1) * 0.4f + 1;

		Main.EntitySpriteDraw(wave, wavePos, null, baseColor * 0.45f, rotation, origin, Projectile.scale * scale, effects, 0);
		Main.EntitySpriteDraw(wave, wavePos, null, baseColor * 0.25f, rotation, origin, new Vector2(Projectile.scale * 0.75f, Projectile.scale * 1.25f) * scale, effects, 0);
		Main.EntitySpriteDraw(wave, wavePos, null, baseColor * 0.15f, rotation, origin, new Vector2(Projectile.scale * 0.5f, Projectile.scale * 1.5f) * scale, effects, 0);
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
