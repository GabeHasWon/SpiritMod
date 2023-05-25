using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.EvilBiomeDrops.GastricGusher
{
	public class GastricGusherProjectile : ModProjectile
	{
		private int _charge = 0;
		private int _endCharge = -1;

		const int MinimumCharge = 0; //How long it takes for a minimum charge - 1/2 second by default

		private float Scaling => ((_charge - MinimumCharge) * 0.03f) + 1f; //Scale factor for projectile damage, spread and speed
		private float ScalingCapped => Scaling >= 4f ? 4f : Scaling; //Cap for scaling so there's not super OP charging lol

		public override void SetStaticDefaults() => DisplayName.SetDefault("Gastric Gusher");

		public override void SetDefaults()
		{
			Projectile.width = 42;
			Projectile.height = 24;
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
			const int HoldOutLength = 22;

			Player p = Main.player[Projectile.owner];
			p.heldProj = Projectile.whoAmI;

			Projectile.timeLeft++; //dont die
			_charge++; //Increase charge timer...

			if (_endCharge == -1)
			{
				if (p.whoAmI == Main.myPlayer)
				{
					Projectile.velocity = new Vector2(HoldOutLength, 0).RotatedBy(p.MountedCenter.AngleTo(Main.MouseWorld));

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
			p.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, compRotation);
			p.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, compRotation);

			//GItem.ArmsTowardsMouse(p);
			Projectile.Center = p.Center + new Vector2(21, 12 + p.gfxOffY);

			if (_charge > _endCharge && _endCharge != -1) //Kill projectile when done shooting - does nothing special but allowed for a cooldown timer before polish
			{
				Projectile.active = false;
				Projectile.netUpdate = true;
			}

			if (p.whoAmI == Main.myPlayer)
			{
				if (!p.channel && _endCharge == -1) //Fire (if possible)
				{
					_endCharge = _charge;

					if (_endCharge >= MinimumCharge)
						Fire(p);
				}
			}
		}

		private void Fire(Player p)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);

			Vector2 vel = Vector2.Normalize(Main.MouseWorld - p.Center) * 10f * (ScalingCapped * 0.8f);
			int inc = 3 + (int)ScalingCapped;

			p.PickAmmo(p.HeldItem, out int _, out float _, out int damage, out float kb, out int ammo, false);

			for (int i = 0; i < inc; i++) //Projectiles
			{
				Vector2 velocity = vel.RotatedBy((i - (inc / 2f)) * 0.16f) * Main.rand.NextFloat(0.85f, 1.15f);
				Projectile.NewProjectile(p.GetSource_ItemUse_WithPotentialAmmo(p.HeldItem, ammo), p.Center, velocity, ModContent.ProjectileType<GastricAcid>(), (int)(damage * ScalingCapped), 1f, Projectile.owner);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D t = TextureAssets.Projectile[Projectile.type].Value;
			SpriteEffects e = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			float realRot = Projectile.rotation - ((e == SpriteEffects.FlipHorizontally) ? MathHelper.Pi : 0); //Rotate towards mouse

			Vector2 drawPos = Projectile.position - Main.screenPosition; //Draw position + charge shaking
			if (_charge > MinimumCharge && _endCharge == -1)
				drawPos += new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * (ScalingCapped * 0.75f);

			Main.spriteBatch.Draw(t, drawPos, new Rectangle(0, 0, 42, 24), lightColor, realRot, new Vector2(21, 12), 1f, e, 1f);
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
