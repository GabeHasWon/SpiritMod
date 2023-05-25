using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using SpiritMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Players;

namespace SpiritMod.Projectiles.DonatorItems
{
	public class FaeriePet : ModProjectile
	{
		private float frameCounter;

		private const float FOV = (float)Math.PI / 2;
		private const float Max_Range = 16 * 50;
		private const int Damage = 15;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moonlit Faerie");
			Main.projFrames[Projectile.type] = 6;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.aiStyle = 26;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft *= 5;
			AIType = ProjectileID.BabyHornet;
		}

		private float Timer
		{
			get => Projectile.localAI[1];
			set => Projectile.localAI[1] = value;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			owner.GetModPlayer<PetPlayer>().PetFlag(Projectile);
			owner.hornet = false;

			Projectile.frame = (int)(frameCounter += .2f) % Main.projFrames[Type];

			if (Projectile.owner != Main.myPlayer)
				return;

			if (Timer > 0)
			{
				Timer--;
				return;
			}

			float direction;
			if (Projectile.direction < 0)
				direction = FOVHelper.POS_X_DIR + Projectile.rotation;
			else
				direction = FOVHelper.NEG_X_DIR - Projectile.rotation;

			var origin = Projectile.Center;
			var fov = new FOVHelper();
			fov.AdjustCone(origin, FOV, direction);
			float maxDistSquared = Max_Range * Max_Range;

			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				NPC npc = Main.npc[i];
				Vector2 npcPos = npc.Center;
				if (npc.CanBeChasedBy() &&
					fov.IsInCone(npcPos) &&
					Vector2.DistanceSquared(origin, npcPos) < maxDistSquared &&
					Collision.CanHitLine(origin, 0, 0, npc.position, npc.width, npc.height))
				{
					if (Main.rand.NextBool(10))
						ShootFeathersAt(npcPos);

					Timer = 140;
					break;
				}
			}
		}

		private void ShootFeathersAt(Vector2 target)
		{
			var origin = Projectile.Center;
			var direction = Projectile.DirectionTo(target) * 18;
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), origin, direction, ModContent.ProjectileType<HarpyBolt>(), Damage * 2, 0, Projectile.owner);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);
			return false;
		}
	}

	public class HarpyBolt : ModProjectile, IDrawAdditive
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Faerie Bolt");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 16;
			Projectile.hostile = false;
			Projectile.scale = .75f;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 70;
			Projectile.tileCollide = false;
			Projectile.aiStyle = -1;
			AIType = ProjectileID.Bullet;
		}

		public override bool PreAI()
		{
			Projectile.velocity *= 1.01f;
			float num = 1f - (float)Projectile.alpha / 255f;
			num *= Projectile.scale;
			Lighting.AddLight(Projectile.Center, 0.255f * num, 0.184f * num, 0.229f * num);
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;

			if (Projectile.timeLeft < 55)
				Projectile.tileCollide = true;

			return true;
		}
		public override void Kill(int timeLeft)
		{
			Vector2 vector9 = Projectile.position;
			Vector2 value19 = (Projectile.rotation - 1.57079637f).ToRotationVector2();
			vector9 += value19 * 16f;
			for (int num257 = 0; num257 < 24; num257++)
			{
				int newDust = Dust.NewDust(vector9, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, 0f, 0, default, 1.2f);
				Main.dust[newDust].position = (Main.dust[newDust].position + Projectile.Center) / 2f;
				Main.dust[newDust].velocity += value19 * 2f;
				Main.dust[newDust].velocity *= 0.5f;
				Main.dust[newDust].noGravity = true;
				vector9 -= value19 * 8f;
			}
			if (Projectile.minion)
			{
				ProjectileExtras.Explode(Projectile.whoAmI, 60, 60, delegate
				{
					SoundEngine.PlaySound(SoundID.NPCHit3);
					for (int i = 0; i < 10; i++)
					{
						int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, -2f, 0, default, 2f);
						Main.dust[num].noGravity = true;
						Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
						Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
						Main.dust[num].scale *= .25f;
						if (Main.dust[num].position != Projectile.Center)
							Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
					}
				}, true);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return false;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Color color = new Color(74, 255, 186) * 0.95f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Color color1 = new Color(255, 209, 244) * 0.95f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

				float scale = Projectile.scale;
				Texture2D tex = ModContent.Request<Texture2D>("SpiritMod/Projectiles/DonatorItems/HarpyBolt_Glow").Value;

				spriteBatch.Draw(tex, Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition, null, color, Projectile.rotation, tex.Size() / 2, scale, default, default);
				spriteBatch.Draw(tex, Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition, null, color1 * .5f, Projectile.rotation, tex.Size() / 2, scale * 2.3f, default, default);
			}
		}
	}
}