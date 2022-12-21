using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class GoldBlasterProj : ModProjectile
	{
		Vector2 Direction => Main.player[Projectile.owner].DirectionTo(Main.MouseWorld);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blaster");
			Main.projFrames[Projectile.type] = 9;
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			Projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			if (Projectile.velocity != Vector2.Zero)
				Projectile.velocity = Vector2.Zero;

			Player player = Main.player[Projectile.owner];
			player.ChangeDir(Main.MouseWorld.X > player.Center.X ? 1 : -1);

			player.heldProj = Projectile.whoAmI;

			player.itemTime = 2;
			player.itemAnimation = 2;
			Projectile.position = player.Center + new Vector2(22f, -6 * player.direction).RotatedBy(Direction.ToRotation()) - (Projectile.Size / 2);

			Projectile.rotation = Direction.ToRotation() + ((Direction.X < 0) ? MathHelper.Pi : 0);
			player.itemRotation = MathHelper.WrapAngle(Direction.ToRotation() + ((player.direction < 0) ? MathHelper.Pi : 0));

			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Type])
				{
					foreach (NPC npc in Main.npc)
					{
						if (npc.active && npc.TryGetGlobalNPC(out GoldBlasterGNPC GNPC))
						{
							GNPC.TryDetonate(npc, Projectile.damage, out bool detonated, player);

							if (!Main.dedServ && detonated)
								ParticleHandler.SpawnParticle(new BlasterFlash(Projectile.Center + new Vector2(28, 0).RotatedBy(Direction.ToRotation()), 1, Direction.ToRotation()));
						}
					}

					Projectile.active = false;
				}
			}
			return true;
		}

		public override bool? CanDamage() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);

			SpriteEffects effects = (Direction.X < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);
			return false;
		}
	}
}