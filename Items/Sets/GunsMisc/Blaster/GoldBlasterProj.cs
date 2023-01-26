using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Particles;
using SpiritMod.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class GoldBlasterProj : ModProjectile
	{
		private Vector2 direction;

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

		public override void OnSpawn(IEntitySource source) => Projectile.velocity = Vector2.Zero;

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (player == Main.LocalPlayer)
			{
				direction = player.DirectionTo(Main.MouseWorld);
				Projectile.netUpdate = true;
			}
			player.ChangeDir(direction.X > player.Center.X ? 1 : -1);

			player.heldProj = Projectile.whoAmI;

			player.itemTime = 2;
			player.itemAnimation = 2;
			Projectile.position = player.Center + new Vector2(22f, -6 * player.direction).RotatedBy(direction.ToRotation()) - (Projectile.Size / 2);

			Projectile.rotation = direction.ToRotation() + ((direction.X < 0) ? MathHelper.Pi : 0);
			player.itemRotation = MathHelper.WrapAngle(direction.ToRotation() + ((player.direction < 0) ? MathHelper.Pi : 0));

			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				if (Projectile.frame == 0)
				{
					//Spawn a visual indicator
					foreach (NPC npc in Main.npc)
					{
						if (npc.active && npc.TryGetGlobalNPC(out GoldBlasterGNPC GNPC))
						{
							if (GNPC.numMarks > 0 && !Main.dedServ)
								ParticleHandler.SpawnParticle(new GoldMark(npc.Center, 0f, npc.whoAmI));
						}
					}
				}
				if (++Projectile.frame >= Main.projFrames[Type])
				{
					foreach (NPC npc in Main.npc)
					{
						if (npc.active && npc.TryGetGlobalNPC(out GoldBlasterGNPC GNPC))
						{
							GNPC.TryDetonate(npc, Projectile.damage, out bool detonated, player);

							if (!Main.dedServ && detonated)
								ParticleHandler.SpawnParticle(new BlasterFlash(Projectile.Center + new Vector2(28, 0).RotatedBy(direction.ToRotation()), 1, Direction.ToRotation()));
						}
					}

					Projectile.active = false;
				}
			}

		}

		public override bool? CanDamage() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);

			SpriteEffects effects = (direction.X < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);
			return false;
		}
	}
}