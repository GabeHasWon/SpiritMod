using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Effects;
using SpiritMod.Particles;
using Terraria.DataStructures;
using static Terraria.ModLoader.PlayerDrawLayer;
using Terraria.ID;
using Terraria.Audio;
using System;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class StarplateHologram : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private float AnimCounter
		{
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private Vector2 direction;
		private bool Hologram => Projectile.frame > 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blaster");
			Main.projFrames[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 28;
			Projectile.height = 28;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 60;
			Projectile.alpha = 80;
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

			if (player.channel)
			{
				player.itemAnimation = 2;
				player.itemTime = 2;

				Projectile.timeLeft = 2;
			}

			Projectile.Center = player.Center + new Vector2(18f - (AnimCounter * 8f), 0).RotatedBy(direction.ToRotation());
			Projectile.rotation = direction.ToRotation() + ((direction.X < 0) ? MathHelper.Pi : 0) - (AnimCounter * player.direction);
			player.itemRotation = MathHelper.WrapAngle(direction.ToRotation() + ((player.direction < 0) ? MathHelper.Pi : 0));

			if (Counter == (Hologram ? (player.itemAnimationMax / 2) : 0))
			{
				SetDirection(player);
				AnimCounter = 1f;

				Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, direction * 10f, ModContent.ProjectileType<HoloShot>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Hologram ? 1f : 0f);

				if (!Main.dedServ)
				{
					Vector2 position = Projectile.Center + ((direction * 28) - (Vector2.UnitY * 4));

					if (Hologram)
					{
						ParticleHandler.SpawnParticle(new HoloFlash(position, 1, direction.ToRotation()));

						for (int i = 0; i < 3; i++)
							ParticleHandler.SpawnParticle(new FireParticle(position, (direction * Main.rand.NextFloat(0.5f, 1.2f)).RotatedByRandom(0.8f), Color.White, Color.Blue, Main.rand.NextFloat(0.2f, 0.5f), 12));
					}
					else
					{
						ParticleHandler.SpawnParticle(new BlasterFlash(position, 1, direction.ToRotation()));

						for (int i = 0; i < 3; i++)
							ParticleHandler.SpawnParticle(new FireParticle(position, (direction * Main.rand.NextFloat(0.5f, 1.2f)).RotatedByRandom(0.8f), Color.White, Color.Red, Main.rand.NextFloat(0.2f, 0.5f), 12));
					}
				}

				if (Main.netMode != NetmodeID.Server)
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1") with { PitchVariance = 0.5f, MaxInstances = 3 }, Projectile.Center);
			}

			if (Counter > 0)
				Counter--;
			else
				Counter = player.itemAnimationMax;

			if (AnimCounter > 0)
				AnimCounter -= 0.1f;
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
			Vector2 offset = new Vector2(0, Projectile.gfxOffY);

			Main.EntitySpriteDraw(texture, Projectile.Center + offset - Main.screenPosition, Projectile.DrawFrame(), Projectile.GetAlpha(Color.White), 
				Projectile.rotation, Projectile.DrawFrame().Size() / 2, Projectile.scale, effects, 0);
			return false;
		}

		public override bool? CanDamage() => false;
	}
}