using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.NPCs.Boss.MoonWizard.Projectiles;
using SpiritMod.Projectiles.BaseProj;
using System.Linq;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;

namespace SpiritMod.Projectiles
{
	public class CornucopionProj : BaseHeldProj
	{
		private ref float Counter => ref Projectile.ai[0];
		private bool StruckTarget { get => Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 1 : 0; }

		private int Charge => (int)((float)Counter / chargeRate);

		private readonly int chargeRate = 45;

		private bool released = false;

		public override void SetStaticDefaults() => Main.projFrames[Type] = 7;

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 2;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

        public override void AbstractAI()
		{
			void DoDusts()
			{
				float magnitude = Main.rand.NextFloat();
				Vector2 startPos = Projectile.Center + new Vector2(25 * Owner.direction, -4 * Owner.gravDir);
				Vector2 randomPos = startPos + (Main.rand.NextVector2Unit() * MathHelper.Lerp(20f, 30f, magnitude));
				
				Dust.NewDustPerfect(randomPos, DustID.Electric, randomPos.DirectionTo(startPos) * MathHelper.Lerp(2f, 3f, magnitude), 0, default, Main.rand.NextFloat(.3f, .7f)).noGravity = true;
			}

			if (!released)
			{
				Owner.velocity.X *= 0.97f;

				if (++Counter % chargeRate == 0)
				{
					SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
					for (int i = 0; i < 10; i++)
						DoDusts();
				}
				if (Counter > 240)
					Owner.AddBuff(BuffID.Electrified, (Charge - 5) * 45);

				if (Main.rand.NextBool(5))
					DoDusts();
			}
			else
			{
				if (!StruckTarget)
					Projectile.Kill();
				else if (++Projectile.frameCounter >= 3)
				{
					Projectile.frameCounter = 0;
					if (++Projectile.frame >= Main.projFrames[Type])
						Projectile.Kill();
				}
			}
			if (!Owner.channel)
				released = true;

			Projectile.rotation = Math.Min(Counter / chargeRate, 1) / 3 * -(Owner.direction * Owner.gravDir);
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -1.57f * Owner.direction);
		}

		public override bool? CanDamage() => (!Owner.channel && (Charge > 0)) ? null : false;

		public override bool? CanCutTiles() => false;
		
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (CanDamage() == false)
				return false;

			NPC pTarget = Main.npc.Where(x => x.Hitbox == targetHitbox).FirstOrDefault();
			if (pTarget is NPC target && (target.Distance(Owner.Center) < 800))
			{
				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<MoonThunder>(), 20, 8);
				proj.friendly = false;
				proj.hostile = false;
				proj.netUpdate = true;

				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Thunder2"), Projectile.Center);
				SpiritMod.tremorTime = 18;

				Counter = Math.Max(Counter - chargeRate, 0);
				StruckTarget = true;
				Projectile.netUpdate = true;
				return true;
			}
			return false;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage += 2 * Charge;

		public override Vector2 HoldoutOffset() => new(6 * Owner.direction, 6 * Owner.gravDir);

		public override bool PreDraw(ref Color lightColor)
		{
			Rectangle frame = Projectile.DrawFrame();
			float rotation = Projectile.rotation + ((Owner.gravDir == -1) ? MathHelper.Pi : 0);
			SpriteEffects effects = ((Projectile.spriteDirection * Owner.gravDir) == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Vector2 origin = (effects == SpriteEffects.None) ? new Vector2(0, frame.Height / 2) : new Vector2(frame.Width, frame.Height / 2);
			
			Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor),
				rotation, origin, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White),
				rotation, origin, Projectile.scale, effects, 0);
			return false;
		}
	}
}
