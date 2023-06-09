using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BloodcourtSet.Heartstrike
{
	public class HeartstrikeProj : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int counterMax = 10;

		private bool Secondary
		{
			get => (int)Projectile.ai[1] != 0;
			set => Projectile.ai[1] = value ? 1 : 0;
		}

		private int shotLength = 1200;
		private Vector2 origin;

		public override string Texture => "SpiritMod/Items/Sets/BloodcourtSet/Heartstrike/Heartstrike";

		public override void SetStaticDefaults() => DisplayName.SetDefault("Heartstrike");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.Center = player.Center;
			player.heldProj = Projectile.whoAmI;

			if (Secondary && Counter == 0)
			{
				origin = Projectile.Center;
				CheckCollision();
			}

			if (Counter < counterMax)
				Counter++;

			if (player.itemAnimation < 2)
				Projectile.Kill();
		}

		private void CheckCollision()
		{
			Vector2 dirUnit = Vector2.Normalize(Projectile.velocity);

			//Test tile collision
			float[] samples = new float[3];
			Collision.LaserScan(origin, dirUnit, 1, shotLength, samples);

			shotLength = 0;
			foreach (float sample in samples)
				shotLength += (int)(sample / samples.Length);

			NPC target = null;
			//Test NPC collision
			foreach (NPC npc in Main.npc)
			{
				float collisionPoint = shotLength;
				if (Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(), origin, origin + (dirUnit * shotLength), 1, ref collisionPoint) && npc.active && !npc.friendly && npc.CanDamage())
				{
					if (collisionPoint < shotLength)
					{
						shotLength = (int)collisionPoint;
						target = npc; //Get the first NPC to the player regardless of their position in the array
					}
				}
			}
			if (target != null)
			{
				target.StrikeNPC((int)(Projectile.damage * 1.5f), Projectile.knockBack * 1.25f, Math.Sign(Projectile.velocity.X));
				target.AddBuff(ModContent.BuffType<SurgingAnguish>(), 200);

				Projectile.NewProjectile(Entity.GetSource_FromAI(), origin + (dirUnit * shotLength), Projectile.velocity, ModContent.ProjectileType<FlayedArrow>(), 0, 0f, Projectile.owner, target.whoAmI);
			}

			for (int i = 0; i < 12; i++) //Do impact dusts
			{
				Dust dust = Dust.NewDustPerfect(origin + (dirUnit * shotLength), DustID.LavaMoss, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(1.0f, 1.5f));
				dust.velocity = -(Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f)).RotatedByRandom(0.8f);
				dust.noGravity = true;
				dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			Vector2 position = player.Center + Projectile.velocity - Main.screenPosition;
			SpriteEffects effects = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = Projectile.velocity.ToRotation() + ((effects == SpriteEffects.FlipHorizontally) ? MathHelper.Pi : 0);

			if (Secondary && Counter > 0)
				DrawSecondaryBeam();

			//Draw the projectile normally
			Main.EntitySpriteDraw(texture, position, null, lightColor, rotation, texture.Size() / 2, Projectile.scale, effects, 0);
			//Draw the projectile glowmask
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, position, null, Color.White, rotation, texture.Size() / 2, Projectile.scale, effects, 0);
			//Draw a projectile pulse effect
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Pulse").Value, position, null, Color.White * (float)(1f - ((float)Counter / counterMax)), rotation, texture.Size() / 2, Projectile.scale, effects, 0);

			return false;
		}

		private void DrawSecondaryBeam()
		{
			float quoteant = (float)Counter / counterMax;
			float initScaleY = 30;

			for (int i = 0; i < 3; i++)
			{
				Texture2D texture = (i > 1) ? Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_1").Value : Mod.Assets.Request<Texture2D>("Textures/GlowTrail").Value;
				Vector2 scale = new Vector2(shotLength, MathHelper.Lerp(initScaleY, 5, quoteant)) / texture.Size();

				Color color = i switch
				{
					0 => Color.Red * .5f,
					1 => Color.Magenta * .8f,
					_ => Color.Yellow * .6f
				};
				color = (color with { A = 0 }) * (float)(1f - quoteant);
				scale.Y -= 0.03f * i;

				Main.EntitySpriteDraw(texture, origin - Main.screenPosition, null, color, Projectile.velocity.ToRotation(), new Vector2(0, texture.Height / 2), scale, SpriteEffects.None, 0);
			}

			if (Counter == 1) //Do fancy dusts
			{
				for (int i = 0; i < (shotLength / 10); i++)
				{
					Vector2 dustPos = Projectile.Center + new Vector2(Main.rand.NextFloat(shotLength), Main.rand.NextFloat(-(initScaleY / 4), initScaleY / 4)).RotatedBy(Projectile.velocity.ToRotation());
					Dust dust = Dust.NewDustPerfect(dustPos, DustID.LavaMoss, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.2f, 0.5f));
					dust.velocity = Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f);
					dust.noGravity = true;
					dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
				}
			}
		}

		public override bool? CanDamage() => false;
	}
}