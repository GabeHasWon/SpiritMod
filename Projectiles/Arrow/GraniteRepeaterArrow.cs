using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Arrow
{
	public class GraniteRepeaterArrow : ModProjectile
	{
		public int TargetIndex { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

		public bool Stuck => Projectile.penetrate <= 1;

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(12);
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 2;
		}

		public override void AI()
		{
			if (!Stuck)
			{
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

				for (int i = 0; i < 3; i++)
				{
					Vector2 pos = Projectile.Center - Projectile.velocity / 5f * i;
					Dust dust = Dust.NewDustPerfect(pos, DustID.Electric, Vector2.Zero, 0, default, .5f);
					dust.velocity *= 0f;
					dust.noGravity = true;
				}
			}
			else if (Main.npc[TargetIndex] is NPC target && (target.CanBeChasedBy(Projectile) || target.type == NPCID.TargetDummy))
			{
				Projectile.Center = target.Center - Projectile.velocity;
				Projectile.gfxOffY = target.gfxOffY;
				Projectile.hide = true;

				if (++Projectile.localAI[0] % 60 == 0)
					target.HitEffect(0, 1.0);
			}
			else Projectile.Kill();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			TargetIndex = target.whoAmI;
			Projectile.velocity = target.Center - Projectile.Center;
			Projectile.netUpdate = true;

			HandleStuck(target);

			if (target.life <= 0)
			{
				ProjectileExtras.Explode(Projectile.whoAmI, 30, 30, delegate { });
				SoundEngine.PlaySound(SoundID.Item109);
				{
					for (int i = 0; i < 20; i++)
					{
						Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, -2f, 0, default, .5f);
						dust.noGravity = true;
						dust.position += Main.rand.NextVector2Unit() * 2.5f - new Vector2(1.5f);

						if (dust.position != Projectile.Center)
							dust.velocity = Projectile.DirectionTo(dust.position) * 6f;
					}
				}

				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<GraniteSpike1>(), Projectile.damage / 3 * 2, Projectile.knockBack, Projectile.owner);
				proj.timeLeft = 2;
				proj.netUpdate = true;
			}
		}

		private void HandleStuck(NPC target)
		{
			Point[] array = new Point[1];
			int counter = 0;

			foreach (Projectile proj in Main.projectile)
			{
				if (proj.whoAmI != Projectile.whoAmI && proj.active && proj.owner == Projectile.owner && proj.type == Projectile.type && (proj.ModProjectile is GraniteRepeaterArrow rArrow) && rArrow.Stuck && rArrow.TargetIndex == target.whoAmI)
				{
					array[counter++] = new Point(proj.whoAmI, proj.timeLeft);
					if (counter >= array.Length)
						break;
				}
			}
			if (counter >= array.Length)
			{
				int num33 = 0;
				for (int num34 = 1; num34 < array.Length; num34++)
				{
					if (array[num34].Y < array[num33].Y)
						num33 = num34;
				}
				Main.projectile[array[num33].X].Kill();
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int k = 0; k < 6; k++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 2.5f, -2.5f, 0, default, .27f);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 2.5f, -2.5f, 0, default, .37f);
			}
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(lightColor), 
				Projectile.rotation, new Vector2(texture.Width / 2, Projectile.height / 2), Projectile.scale, SpriteEffects.None);

			return false;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
			=> behindNPCs.Add(index);

		public override bool? CanDamage() => Stuck ? false : null;

		public override bool? CanCutTiles() => Stuck ? false : null;

		public override bool ShouldUpdatePosition() => !Stuck;
	}
}
