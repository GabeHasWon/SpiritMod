using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Effects;
using SpiritMod.Particles;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;
using System;
using System.IO;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class StarplateHologram : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private ref float AnimCounter => ref Projectile.ai[1];

		private bool released;
		private Vector2 direction;
		private bool Hologram => Projectile.frame == 0;

		public override void SetStaticDefaults() => Main.projFrames[Type] = 2;

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = Projectile.height = 28;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 60;
		}

		public override void OnSpawn(IEntitySource source)
		{
			Projectile.velocity = Vector2.Zero;
			SetDirection(Main.player[Projectile.owner]);
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			player.heldProj = Projectile.whoAmI;

			if (player.channel && !released)
			{
				player.itemAnimation = player.itemTime = 30;
				Projectile.timeLeft = 30;
			}
			else released = true;

			Projectile.Center = player.Center + new Vector2(22f * (float)(1f - (AnimCounter * 2f)), -(player.direction * 10f) * AnimCounter).RotatedBy(direction.ToRotation());
			Projectile.rotation = direction.ToRotation() + ((direction.X < 0) ? MathHelper.Pi : 0) - (AnimCounter * player.direction);

			player.itemRotation = MathHelper.WrapAngle(direction.ToRotation() + ((player.direction < 0) ? MathHelper.Pi : 0));

			if (!released && Counter == (Hologram ? (player.itemAnimationMax / 2) : 0))
			{
				SetDirection(player);
				AnimCounter = .5f;

				Vector2 position = Projectile.Center + (direction * 26) - (Vector2.UnitY * 2);
				Projectile.NewProjectile(Entity.GetSource_FromAI(), position, direction * 10f, ModContent.ProjectileType<HoloShot>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Hologram ? 1f : 0f);

				if (!Main.dedServ)
				{
					if (Hologram)
					{
						ParticleHandler.SpawnParticle(new HoloFlash(position, 1, direction.ToRotation()));
						for (int i = 0; i < 3; i++)
							ParticleHandler.SpawnParticle(new FireParticle(position, (direction * Main.rand.NextFloat(.5f, 1.2f)).RotatedByRandom(0.8f), Color.White, Color.Blue, Main.rand.NextFloat(.2f, .5f), 12));
					}
					else
					{
						ParticleHandler.SpawnParticle(new BlasterFlash(position, 1, direction.ToRotation()));
						for (int i = 0; i < 3; i++)
							ParticleHandler.SpawnParticle(new FireParticle(position, (direction * Main.rand.NextFloat(.5f, 1.2f)).RotatedByRandom(0.8f), Color.White, Color.Red, Main.rand.NextFloat(.2f, .5f), 12));
					}
				}

				if (Main.netMode != NetmodeID.Server)
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1") with { PitchVariance = .5f, MaxInstances = 3 }, Projectile.Center);
			}

			if (Counter > 0)
				Counter--;
			else if (!released)
				Counter = player.itemAnimationMax;

			if (AnimCounter > 0)
				AnimCounter -= .05f;
		}

		private void SetDirection(Player player)
		{
			if (player.channel && player == Main.LocalPlayer) //Readjust the gun to face the owner's cursor
			{
				direction = player.DirectionTo(Main.MouseWorld);
				player.ChangeDir(Math.Sign(player.DirectionTo(Main.MouseWorld).X));

				Projectile.netUpdate = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			SpriteEffects effects = (direction.X < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Color color = Hologram ? (Color.White with { A = 0 }) * .5f : lightColor;

			Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, Projectile.DrawFrame(), color, 
				Projectile.rotation, Projectile.DrawFrame().Size() / 2, Projectile.scale, effects, 0);
			return false;
		}

		public override bool? CanDamage() => false;

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(released);

		public override void ReceiveExtraAI(BinaryReader reader) => released = reader.ReadBoolean();
	}
}