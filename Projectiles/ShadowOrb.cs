using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles;

public class ShadowOrb : ModProjectile
{
	private int timer = 16;

	public override void SetStaticDefaults() => Main.projFrames[Type] = 4;

	public override void SetDefaults()
	{
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.width = 50;
		Projectile.height = 50;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 18000;
	}

	public override bool PreAI()
	{
		var dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0f, 0f);
		dust.scale = 0.8f;
		dust.noGravity = true;

		return true;
	}

	public override void AI()
	{
		if (++Projectile.frameCounter >= 4)
		{
			Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
			Projectile.frameCounter = 0;
		}

		if (--timer == 0)
		{
			SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X + Main.rand.Next(-3, 5), Projectile.velocity.Y + Main.rand.Next(-3, 5), ModContent.ProjectileType<VoidStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
			timer = 21;
		}

		if (++Projectile.ai[1] >= 7200f)
		{
			Projectile.alpha += 5;
			if (Projectile.alpha > 255)
			{
				Projectile.alpha = 255;
				Projectile.Kill();
			}
		}

		if (++Projectile.localAI[0] >= 10f)
		{
			Projectile.localAI[0] = 0f;
			int num416 = 0;
			int num417 = 0;
			float num418 = 0f;
			int num419 = Projectile.type;

			for (int num420 = 0; num420 < 1000; num420++)
			{
				if (Main.projectile[num420].active && Main.projectile[num420].owner == Projectile.owner && Main.projectile[num420].type == num419 && Main.projectile[num420].ai[1] < 3600f)
				{
					num416++;
					if (Main.projectile[num420].ai[1] > num418)
					{
						num417 = num420;
						num418 = Main.projectile[num420].ai[1];
					}
				}
			}

			if (num416 > 1)
			{
				Main.projectile[num417].netUpdate = true;
				Main.projectile[num417].ai[1] = 36000f;
				return;
			}
		}
	}
}
