using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Thrown.Charge
{
	public abstract class JavelinProj : ModProjectile
	{
		protected float Counter { get; private set; }
		protected bool Released { get; private set; }
		protected int? StruckNPCIndex { get; private set; }

		internal abstract int ChargeTime { get; }

		protected bool Embeded => StruckNPCIndex is not null;
		private float ChargeRate => ChargeTime / 10000f * (float)Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee);

		public const float maxDamageMult = 3f;
		private readonly int holdoutLength = 18;
		private readonly int lingerTime = 500;

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 12;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.penetrate = 2;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 600;
			Projectile.alpha = 255;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public sealed override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];

			int fadeTime = 14;
			Projectile.alpha = Math.Max(0, Projectile.alpha - 255 / fadeTime);

			if (!Released)
			{
				float quoteant = (float)Counter / 1f;

				if (player.channel)
				{
					Projectile.timeLeft++;
					Counter = Math.Min(1f, Counter + ChargeRate);

					if (player == Main.LocalPlayer)
					{
						Projectile.velocity = player.DirectionTo(Main.MouseWorld);
						Projectile.netUpdate = true;
					}

					Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
					player.ChangeDir(Projectile.direction);

					player.heldProj = Projectile.whoAmI;
					player.itemTime = ++player.itemAnimation;

					float rotationOffset = MathHelper.ToRadians(quoteant * -30) * Projectile.direction * player.gravDir;
					Projectile.rotation = rotationOffset;
					Projectile.Center = player.MountedCenter - new Vector2(-(TextureAssets.Projectile[Type].Width() / 2) * Projectile.direction, holdoutLength * player.gravDir).RotatedBy(rotationOffset);

					player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, MathHelper.Pi + rotationOffset * player.gravDir);
					player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (Projectile.velocity.ToRotation() * player.gravDir) - 1.57f);
				}
				else
				{
					Released = true;
					Counter = 30 - (quoteant * 30f);

					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);

					float magnitude = Math.Max(3f, quoteant * 10f);
					Projectile.velocity *= magnitude;

					Projectile.extraUpdates = 1;
					Projectile.damage = (int)MathHelper.Lerp(Projectile.damage, Projectile.damage * maxDamageMult, quoteant);
					Projectile.tileCollide = true;

					Projectile.netUpdate = true;
				}

				if (!player.active || player.dead || player.frozen)
					Projectile.active = false;
			}
			else
			{
				if (Embeded)
				{
					NPC npc = Main.npc[StruckNPCIndex ?? 0];

					if (!npc.active)
						Projectile.active = false;
					else
						Projectile.position = Projectile.position + npc.velocity - Projectile.velocity;

					if (Projectile.timeLeft % 30 == 0)
						npc.HitEffect(Projectile.direction, 1.0);

					int fadeoutTime = 20;
					if (Projectile.timeLeft <= fadeoutTime)
						Projectile.alpha = 255 / fadeoutTime * (fadeoutTime - Projectile.timeLeft);
				}
				else
				{
					if (++Counter > 30)
						Projectile.velocity.Y += 0.07f;

					Projectile.rotation = Projectile.velocity.ToRotation() + ((Projectile.direction == -1) ? MathHelper.Pi : 0);
				}
			}
			return true;
		}

		public virtual void DrawGlowmask(ref Color lightColor) { }

		public sealed override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			SpriteEffects effects = (Projectile.direction < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 origin = (effects == SpriteEffects.None) ? new Vector2(texture.Width - (Projectile.width / 2), texture.Height / 2) : new Vector2(Projectile.width / 2, texture.Height / 2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);

			if (Released)
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * .5f;
					Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, origin, Projectile.scale, effects, 0);
				}
			}

			DrawGlowmask(ref lightColor);

			if (!Released && Counter >= 1f)
			{
				Projectile.localAI[0]++;
				Vector2 scale = new Vector2(.5f) * Math.Max(0, 1f - (Projectile.localAI[0] / 12));
				Vector2 position = Projectile.Center + (Vector2.UnitX * Projectile.localAI[0] * -Projectile.direction).RotatedBy(Projectile.rotation);

				for (int i = 0; i < 2; i++)
					Main.EntitySpriteDraw(TextureAssets.Extra[89].Value, position - Main.screenPosition, null, Color.White with { A = 0 } * .7f, 1.57f * i, TextureAssets.Extra[89].Size() / 2, scale, SpriteEffects.None, 0);
			}
			return false;
		}

		public virtual void HitNPC(NPC target, int damage, float knockback, bool crit) { }

		public sealed override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Projectile.numHits >= Projectile.penetrate)
			{
				Projectile.timeLeft = lingerTime;
				Projectile.penetrate++;
				Projectile.position += Projectile.velocity;

				StruckNPCIndex = target.whoAmI;
			}

			if (crit)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Stab") with { Volume = .7f }, Projectile.Center);

			HitNPC(target, damage, knockback, crit);
		}

		public override bool? CanDamage() => Released && !Embeded;

		public override bool? CanCutTiles() => Released && !Embeded;

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(Counter);
			writer.Write(Released);
			writer.Write(StruckNPCIndex ?? -1);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Counter = reader.Read();
			Released = reader.ReadBoolean();
			int index = reader.Read();
			StruckNPCIndex = index == -1 ? null : index;
		}
	}
}
