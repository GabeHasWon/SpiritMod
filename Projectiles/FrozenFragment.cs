using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class FrozenFragment : ModProjectile
	{
		private const int timeLeftMax = 120;

        private int TargetWhoAmI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float MaxScale
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private Vector2 relativeOffset;

        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Ice");
			Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
		{
			Projectile.Size = new Vector2(10);
			Projectile.aiStyle = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = timeLeftMax;
			Projectile.scale = 0;
		}

		public override void OnSpawn(IEntitySource source)
		{
            
        }

        public override void AI()
		{
			if (Projectile.timeLeft == 120)
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

			NPC npc = Main.npc[TargetWhoAmI];
			Projectile.position = npc.position + relativeOffset;

			if (!npc.active)
				Projectile.Kill();
		}

		public override bool? CanCutTiles() => false;
		public override bool? CanDamage() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle rect = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);
			Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

			Main.EntitySpriteDraw(texture, position, rect, Projectile.GetAlpha(Color.White), Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(relativeOffset);

		public override void ReceiveExtraAI(BinaryReader reader) => relativeOffset = reader.ReadVector2();
	}
}