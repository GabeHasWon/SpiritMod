using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class BloodyCyst : ModProjectile
	{
		private const int timeLeftMax = 120;

        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private Vector2 relativeOffset;

        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Cyst");
			Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = timeLeftMax;
			Projectile.scale = 0;
		}

        public override void AI()
		{
			if (Main.npc[TargetWhoAmI] is NPC npc && npc.active)
			{
				if (Projectile.timeLeft == timeLeftMax)
				{
					relativeOffset = Projectile.position - Main.npc[TargetWhoAmI].position;
					Projectile.frame = Main.rand.Next(Main.projFrames[Type]);
				}

				int fadeoutTime = 12;
				if (Projectile.timeLeft < fadeoutTime)
					Projectile.scale -= 1f / fadeoutTime;
				else
					Projectile.scale = Math.Min(1, Projectile.scale + .1f);

				Projectile.rotation = Projectile.velocity.ToRotation();
				Projectile.position = npc.position + relativeOffset;

				if (Main.rand.NextBool(20))
					Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Main.rand.NextVector2Unit());
			}
			else Projectile.Kill();
		}

		public override bool? CanCutTiles() => false;

		public override bool? CanDamage() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle rect = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);
			Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

			Main.EntitySpriteDraw(texture, position, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}