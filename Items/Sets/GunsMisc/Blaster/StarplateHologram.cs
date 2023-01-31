using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;
using Terraria.ID;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Particles;
using SpiritMod.Particles;
using Terraria.DataStructures;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class StarplateHologram : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private Vector2 direction;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Blaster");

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

			if (Counter > 0)
				Counter--;
			if ((player.itemAnimation == (player.itemAnimationMax / 2)) || (player.itemAnimation >= player.itemAnimationMax))
			{
				if (player.itemAnimation == (player.itemAnimationMax / 2)) //Fire a shot
				{
					SetDirection(player);

					Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, direction * 11f, ModContent.ProjectileType<StarshotBlue>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
					Counter = player.itemAnimationMax;
				}
				if (Main.netMode != NetmodeID.Server) //Play a firing sound, also accounting for the normal weapon due to instance issues
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1") with { MaxInstances = 3, PitchVariance = 0.3f }, Projectile.Center);

				if (!Main.dedServ)
				{
					Vector2 position = Projectile.Center + (direction * 28);
					ParticleHandler.SpawnParticle(new BlasterFlash(position, 1, direction.ToRotation()));
					for (int i = 0; i < 3; i++)
						ParticleHandler.SpawnParticle(new FireParticle(position, (direction * Main.rand.NextFloat(0.5f, 1.2f)).RotatedByRandom(0.8f), Color.White, Color.Red, Main.rand.NextFloat(0.2f, 0.5f), 12));
				}
			}

			int offset = (int)MathHelper.Clamp(Counter - (player.itemAnimationMax - 12), 0, player.itemAnimationMax);

			Projectile.Center = player.Center + new Vector2(18f - offset, 0).RotatedBy(direction.ToRotation());
			Projectile.rotation = direction.ToRotation() + ((direction.X < 0) ? MathHelper.Pi : 0) - (offset * 0.03f * player.direction);

			if (Projectile.alpha < 255)
				Projectile.alpha += 255 / 30;
		}

		private void SetDirection(Player player)
		{
			if (player.channel && player == Main.LocalPlayer) //Readjust the gun to face the owner's cursor
			{
				direction = player.DirectionTo(Main.MouseWorld);
				Projectile.timeLeft = player.itemAnimationMax + 1;
				Projectile.alpha = 0;

				Projectile.netUpdate = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			SpriteEffects effects = (direction.X < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 offset = new Vector2(0, Projectile.gfxOffY);

			Main.EntitySpriteDraw(texture, Projectile.Center + offset - Main.screenPosition, null, Projectile.GetAlpha(Color.White), 
				Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);
			return false;
		}

		public override bool? CanDamage() => false;
	}
}