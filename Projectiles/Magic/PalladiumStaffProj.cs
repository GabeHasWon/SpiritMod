using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Prim;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	public class PalladiumStaffProj : ModProjectile
	{
		public ref float Counter => ref Projectile.ai[0];

		public override string Texture => "Terraria/Images/Extra_60";

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 40;
			Projectile.height = 60;
			Projectile.penetrate = -1;
			Projectile.hide = true;
			Projectile.timeLeft = 18000;
			Projectile.tileCollide = false;
		}

		public override bool? CanHitNPC(NPC target) => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			float lerp = (float)Math.Sin(Main.timeForVisualEffects / 30f);
			Color color = Projectile.GetAlpha(Color.Lerp(Color.Orange, Color.Goldenrod, lerp) with { A = 0 });
			Vector2 pos = Projectile.Bottom;

			Main.EntitySpriteDraw(texture, pos - Main.screenPosition, null, color, 0f, (texture.Size() / 2) + new Vector2(0, 10), 1f, SpriteEffects.None, 0f);

			Effect blurEffect = ModContent.Request<Effect>("SpiritMod/Effects/BlurLine").Value;
			SquarePrimitive blurLine = new SquarePrimitive()
			{
				Position = pos - Main.screenPosition,
				Height = 100,
				Length = 18,
				Rotation = MathHelper.PiOver2,
				Color = Color.Yellow
			};
			PrimitiveRenderer.DrawPrimitiveShape(blurLine, blurEffect);
			return false;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);

		public override void AI()
		{
			Lighting.AddLight(Projectile.position, 0.5f, .5f, .4f);

			Counter = (Counter + 1) % 22;
			if (Projectile.owner == Main.myPlayer && Counter == 21)
			{
				Vector2 pos = Projectile.BottomLeft + new Vector2(Main.rand.NextFloat(Projectile.width), -2);

				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, (Vector2.UnitY * -Main.rand.NextFloat(.5f, 2.5f)), 
					ModContent.ProjectileType<PalladiumRuneEffect>(), Projectile.damage, 0, Projectile.owner);
				
				proj.scale = Main.rand.NextFloat(.4f, .8f);
				proj.frame = Main.rand.Next(0, 8);
				proj.netUpdate = true;
			}

			Player player = Main.LocalPlayer;
			if ((int)Vector2.Distance(Projectile.Center, player.Center) < 53 && player.statLife <= player.statLifeMax / 3)
				player.AddBuff(BuffID.RapidHealing, 300);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, 0f, -2f, 0, Color.White, 2f);
				Main.dust[num].noLight = true;
				Main.dust[num].noGravity = true;
				Dust dust = Main.dust[num];
				dust.position.X += ((Main.rand.Next(-40, 41) / 20) - 1.5f);
				dust.position.Y += ((Main.rand.Next(-40, 41) / 20) - 1.5f);

				if (Main.dust[num].position != Projectile.Center)
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
			}
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}
	}
}