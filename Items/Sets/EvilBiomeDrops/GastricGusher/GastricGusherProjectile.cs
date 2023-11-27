using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.EvilBiomeDrops.GastricGusher
{
	public class GastricGusherProjectile : ModProjectile
	{
		private int _charge = 0;
		private int _endCharge = -1;

		const int MinimumCharge = 0; //How long it takes for a minimum charge - 1/2 second by default

		private float Scaling => ((_charge - MinimumCharge) * 0.05f) + 1f; //Scale factor for projectile damage, spread and speed
		private float ScalingCapped => Scaling >= 4f ? 4f : Scaling; //Cap for scaling so there's not super OP charging lol

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.GastricGusher.DisplayName");

		public override void SetStaticDefaults() => Main.projFrames[Type] = 6;

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(24);
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = -1;
		}

		public override bool? CanDamage() => false;

		public override void AI()
		{
			const int HoldOutLength = 42;

			Player p = Main.player[Projectile.owner];
			p.heldProj = Projectile.whoAmI;

			Projectile.timeLeft++; //dont die
			_charge++; //Increase charge timer...

			if (_endCharge == -1)
			{
				if (p.whoAmI == Main.myPlayer)
				{
					Projectile.velocity = p.DirectionTo(Main.MouseWorld);
					Projectile.netUpdate = true;
				}

				Projectile.rotation = Projectile.velocity.ToRotation();
				Projectile.spriteDirection = Projectile.direction;

				if (p.channel) //Use turn functionality
					p.ChangeDir(Projectile.direction);

				p.itemTime = p.HeldItem.useTime;
				p.itemAnimation = p.HeldItem.useAnimation;
			}

			float compRotation = Projectile.rotation - (1.57f - (.5f * Projectile.direction));
			p.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, compRotation);
			p.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, compRotation);

			Projectile.Center = p.Center + new Vector2(HoldOutLength, 0).RotatedBy(Projectile.velocity.ToRotation());

			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;

				int maxFrame = (_endCharge != -1) ? 5 : ((ScalingCapped >= 4f) ? 2 : 0);
				Projectile.frame = Math.Min(Projectile.frame + 1, maxFrame);
			}

			if (_charge > _endCharge && _endCharge != -1 && p.ItemAnimationEndingOrEnded) //Kill projectile when done shooting - does nothing special but allowed for a cooldown timer before polish
			{
				Projectile.active = false;
				Projectile.netUpdate = true;
			}

			if (!p.channel && _endCharge == -1) //Fire (if possible)
			{
				_endCharge = _charge;
				Projectile.frame = 2;

				if (p.whoAmI == Main.myPlayer && _endCharge >= MinimumCharge)
					Fire(p);
			}
		}

		private void Fire(Player p)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);

			Vector2 vel = Vector2.Normalize(Projectile.velocity) * 10f * (ScalingCapped * 0.8f);
			int inc = 3 + (int)ScalingCapped;

			p.PickAmmo(p.HeldItem, out int _, out float _, out int damage, out float kb, out int ammo, false);

			for (int i = 0; i < inc; i++) //Projectiles
			{
				Vector2 velocity = vel.RotatedBy((i - (inc / 2f)) * 0.16f) * Main.rand.NextFloat(0.7f, 1.2f);
				Projectile.NewProjectile(p.GetSource_ItemUse_WithPotentialAmmo(p.HeldItem, ammo), Projectile.Center, velocity, ModContent.ProjectileType<GastricAcid>(), (int)(damage * ScalingCapped), 1f, Projectile.owner);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D t = TextureAssets.Projectile[Projectile.type].Value;
			SpriteEffects e = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Rectangle f = t.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);

			float realRot = Projectile.rotation - ((e == SpriteEffects.FlipHorizontally) ? MathHelper.Pi : 0); //Rotate towards mouse

			Vector2 origin = (e == SpriteEffects.FlipHorizontally) ? new Vector2(0, f.Height / 2) : new Vector2(f.Width, f.Height / 2);
			Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY); //Draw position + charge shaking
			if (_charge > MinimumCharge && _endCharge == -1)
				drawPos += new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * (ScalingCapped * 0.75f);

			Main.EntitySpriteDraw(t, drawPos, f, lightColor, realRot, origin, 1f, e, 1f);
			return false;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(_endCharge);
			writer.Write(_charge);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			_endCharge = reader.ReadInt32();
			_charge = reader.ReadInt32();
		}
	}
}
