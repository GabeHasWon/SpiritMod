using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BloodcourtSet.Heartstrike
{
	public class FlayedArrow : ModProjectile
	{
		private int TargetIndex
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private int Counter
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}
		private readonly int counterMax = 10;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flayed Bolt");
			Main.projFrames[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.aiStyle = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 60;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			NPC npc = Main.npc[TargetIndex];

			Projectile.position = Projectile.position + npc.velocity - Projectile.velocity;

			if (Counter < counterMax)
				Counter++;

			int fadeTime = 10;
			if (Projectile.timeLeft < fadeTime)
				Projectile.alpha += 255 / fadeTime;

			if (!npc.active)
				Projectile.Kill();
		}

		public override bool? CanDamage() => false;

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
			=> behindNPCsAndTiles.Add(index);

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle rect = new Rectangle(0, 0, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);
			Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

			Main.EntitySpriteDraw(texture, position, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			
			if (Counter < counterMax)
			{
				rect.Y += texture.Height / Main.projFrames[Type];
				Color color = Color.White * (float)(1f - ((float)Counter / counterMax));

				Main.EntitySpriteDraw(texture, position, rect, Projectile.GetAlpha(color), Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}

			return false;
		}
	}
}