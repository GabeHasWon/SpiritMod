using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Particles;
using System;
using System.Linq;
using SpiritMod.Mechanics.Trails;
using Terraria.DataStructures;

namespace SpiritMod.Items.Sets.OlympiumSet.ArtemisHunt
{
	public class ArtemisHunt : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iokheira");
			Tooltip.SetDefault("Hit an enemy to mark them\nRight click to fire a volley of arrows at marked foes");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 43;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 20;
			Item.height = 38;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.Shuriken;
			Item.useAmmo = AmmoID.Arrow;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 2);
			Item.autoReuse = true;
			Item.shootSpeed = 12f;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (type == ProjectileID.WoodenArrowFriendly)
			{
				SoundEngine.PlaySound(SoundID.Item5 with { PitchVariance = 0.2f }, player.Center);
				SoundEngine.PlaySound(SoundID.Item20 with { PitchVariance = 0.2f, Volume = 0.5f }, player.Center);
				type = ModContent.ProjectileType<ArtemisHuntArrow>();
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			if (player.altFunctionUse == 2)
			{
				Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<ArtemisHuntProj>(), damage, knockback, player.whoAmI);
				return false;
			}
			return true;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-4, 0);

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
				Item.noUseGraphic = true;
			else
				Item.noUseGraphic = false;
			return base.CanUseItem(player);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, rotation, scale);
	}
	public class ArtemisHuntArrow : ModProjectile
	{
		float noiseRotation;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Celestial Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.extraUpdates = 1;
			Projectile.timeLeft = 600;
		}

		public override void AI()
		{
			if (noiseRotation < 0.02f)
				noiseRotation = Main.rand.NextFloat(6.28f);
			noiseRotation += 0.02f;

			if (Main.rand.NextBool(5))
			{
				StarParticle particle = new StarParticle(
				Projectile.Center + Main.rand.NextVector2Circular(5, 5),
				Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 0.1f,
				new Color(48, 255, 176),
				Main.rand.NextFloat(0.08f, 0.23f),
				Main.rand.Next(20, 40));

				ParticleHandler.SpawnParticle(particle);
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			Lighting.AddLight(new Vector2(Projectile.Center.X, Projectile.Center.Y), 0.075f * .75f, 0.255f * .75f, 0.193f * .75f);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Color color = new Color(48, 255, 176) { A = 0 };

			Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(7, 0), 1, SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			SpiritMod.ConicalNoise.Parameters["vnoise"].SetValue(ModContent.Request<Texture2D>("SpiritMod/Textures/voronoiLooping", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
			SpiritMod.ConicalNoise.Parameters["rotation"].SetValue(noiseRotation);
			SpiritMod.ConicalNoise.Parameters["transparency"].SetValue(0.8f);
			SpiritMod.ConicalNoise.Parameters["color"].SetValue(color.ToVector4());
			SpiritMod.ConicalNoise.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(TextureAssets.Extra[49].Value, Projectile.Center + (Projectile.rotation.ToRotationVector2() * 17)- Main.screenPosition, null, color, Projectile.rotation - 1.57f, new Vector2(100, 83), new Vector2(0.6f, 0.4f), SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 vel = Vector2.Zero - Projectile.velocity;
				vel.Normalize();
				vel = vel.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
				vel *= Main.rand.NextFloat(2, 5);
				ImpactLine line = new ImpactLine(target.Center - (vel * 10), vel, new Color(48, 255, 176), new Vector2(0.25f, Main.rand.NextFloat(0.75f, 1.75f)), 70)
				{
					TimeActive = 30
				};
				ParticleHandler.SpawnParticle(line);
			}

			for (int j = 0; j < 7; j++)
			{
				Vector2 vel = Vector2.Zero - Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 0.25f;
				int timeLeft = Main.rand.Next(40, 100);

				StarParticle particle = new StarParticle(
				Projectile.Center + Main.rand.NextVector2Circular(10, 10) - (vel * 5),
				vel + Main.rand.NextVector2Circular(3, 3),
				new Color(48, 255, 176),
				Main.rand.NextFloat(0.1f, 0.2f),
				timeLeft)
				{
					TimeActive = (uint)(timeLeft / 2)
				};
				ParticleHandler.SpawnParticle(particle);
			}

			ArtemisPlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<ArtemisPlayer>();

			if (modPlayer.targetWhoAmI != target.whoAmI)
				Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<ArtemisCrescent>(), 0, 0, Projectile.owner, target.whoAmI);
			modPlayer.targetWhoAmI = target.whoAmI;
			modPlayer.targetTime = 360;
		}
	}
	public class ArtemisHuntProj : ModProjectile
	{
		const int FRAMETIME = 5;
		const float SPREAD = 0.1f;
		const int NUMBEROFSHOTS = 7;
		const int NUMBEROFFRAMES = 7;
		const int SHOOTTIME = 2;

		bool Firing => frame > 4;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Artemis Hunt");
			Main.projFrames[Projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 999999;
			Projectile.ignoreWater = true;
		}

		int frame = 0;
		int frameCounter = 0;

		float offsetAngle = -0.45f;

		int shootCounter = 0;

		int shots = NUMBEROFSHOTS;

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			player.itemTime = player.itemAnimation = 5;
			Projectile.Center = player.Center;
			player.heldProj = Projectile.whoAmI;

			if (player.whoAmI == Main.myPlayer)
			{
				Projectile.velocity = player.DirectionTo(Main.MouseWorld);
				Projectile.netUpdate = true;
			}
			player.ChangeDir((Projectile.velocity.X > 0) ? 1 : -1);

			if (frame < NUMBEROFFRAMES)
			{
				frameCounter++;
				if (frameCounter >= FRAMETIME)
				{
					frameCounter = 0;
					frame++;
					Projectile.frame++;
				}
			}

			if (Firing)
			{
				shootCounter++;
				if (shootCounter % SHOOTTIME == 0 && Projectile.timeLeft > 30)
				{
					SoundEngine.PlaySound(SoundID.Item20 with { PitchVariance = 0.2f }, Projectile.Center);

					offsetAngle += SPREAD;
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(offsetAngle) * 20, ModContent.ProjectileType<ArtemisHuntVolley>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

					ImpactLine line = new ImpactLine(Projectile.Center - (Projectile.velocity.RotatedBy(offsetAngle) * 50), Projectile.velocity.RotatedBy(offsetAngle) * 4, new Color(125, 255, 253), new Vector2(0.25f, 2f), 70)
					{
						TimeActive = 30
					};
					ParticleHandler.SpawnParticle(line);

					shots--;
					if (shots <= 0)
						Projectile.timeLeft = 30;
				}
				Projectile.velocity = Projectile.velocity.RotatedBy(offsetAngle);
			}

			player.itemRotation = Projectile.velocity.ToRotation();
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation - 1.57f);
			player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, player.itemRotation - 1.57f);

			if (player.direction != 1)
				player.itemRotation -= 3.14f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Texture2D glow = ModContent.Request<Texture2D>("SpiritMod/Items/Sets/OlympiumSet/ArtemisHunt/ArtemisHuntProj_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			Vector2 position = player.Center + (Projectile.velocity * -6) - Main.screenPosition;

			SpriteEffects effects = (player.direction == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			float rotation = Projectile.velocity.ToRotation() - ((effects == SpriteEffects.FlipHorizontally) ? 3.14f : 0);
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);

			Main.EntitySpriteDraw(texture, position, frame, lightColor, rotation, frame.Size() / 2, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(glow, position, frame, Color.White, rotation, frame.Size() / 2, Projectile.scale, effects, 0);

			return false;
		}

		public override bool ShouldUpdatePosition() => false;
	}
	public class ArtemisHuntVolley : ModProjectile, ITrailProjectile
	{
		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new GradientTrail(new Color(48, 255, 176), new Color(125, 255, 253)), new RoundCap(), new DefaultTrailPosition(), 8f, 400f, new ImageShader(ModContent.Request<Texture2D>("SpiritMod/Textures/Trails/Trail_2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
			tManager.CreateTrail(Projectile, new GradientTrail(new Color(48, 255, 176) * .5f, new Color(255, 255, 255) * 0.3f), new RoundCap(), new DefaultTrailPosition(), 26f, 100f, new DefaultShader());
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Celestial Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 14;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.extraUpdates = 1;
			Projectile.timeLeft = 600;
			AIType = ProjectileID.WoodenArrowFriendly;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

		public override bool PreAI()
		{
			if (Main.rand.NextBool(3))
			{
				StarParticle particle = new StarParticle(
				Projectile.Center + Main.rand.NextVector2Circular(5, 5),
				Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f,0.3f)) * 0.1f,
				Main.rand.NextBool() ? new Color(125, 255, 253) : new Color(48, 255, 176),
				Main.rand.NextFloat(0.05f, 0.15f),
				Main.rand.Next(20, 40));

				ParticleHandler.SpawnParticle(particle);
			}

			var target = Main.npc.Where(n => n.active && Vector2.Distance(n.Center, Projectile.Center) < 200 && Main.player[Projectile.owner].GetModPlayer<ArtemisPlayer>().targetWhoAmI == n.whoAmI).FirstOrDefault();
			if (target != default)
			{
				Vector2 direction = Projectile.DirectionTo(target.Center) * 10f;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction, 0.05f);
			}
			Lighting.AddLight(Projectile.Center, new Color(48, 255, 176).ToVector3() * 0.3f);

			return true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			for (int i = 0; i < 6; i++)
			{
				Vector2 vel = Vector2.Normalize(Vector2.Zero - Projectile.velocity);
				vel = vel.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
				vel *= Main.rand.NextFloat(2, 5);
				ImpactLine line = new ImpactLine(target.Center - (vel * 10), vel, Main.rand.NextBool() ? new Color(125, 255, 253) : new Color(48, 255, 176), new Vector2(0.25f, Main.rand.NextFloat(0.5f, 1.5f)), 70)
				{
					TimeActive = 30
				};
				ParticleHandler.SpawnParticle(line);
			}

			for (int j = 0; j < 10; j++)
			{
				Vector2 vel = Vector2.Zero - Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 0.25f;
				int timeLeft = Main.rand.Next(40, 100);

				StarParticle particle = new(
				Projectile.Center + Main.rand.NextVector2Circular(10, 10) - (vel * 5),
				vel + Main.rand.NextVector2Circular(3, 3),
				new Color(48, 255, 176),
				Main.rand.NextFloat(0.1f, 0.2f),
				timeLeft)
				{
					TimeActive = (uint)(timeLeft / 2)
				};
				ParticleHandler.SpawnParticle(particle);
			}
			StarParticle particle2 = new StarParticle(
			Projectile.Center,
			Vector2.Zero,
			Color.White,
			0.6f,
			20);
			ParticleHandler.SpawnParticle(particle2);
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (Main.player[Projectile.owner].GetModPlayer<ArtemisPlayer>().targetWhoAmI == target.whoAmI)
				damage = (int)(damage * 1.5f);
		}
	}

	public class ArtemisCrescent : ModProjectile
	{
		private NPC Target => Main.npc[(int)Projectile.ai[0]];

		private float counter;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Artemis Crescent");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}
		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 12;
			Projectile.ignoreWater = true;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;
		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, new Color(48, 255, 176).ToVector3() * 0.3f);
			counter += 0.025f;
			if (Target.active)
			{
				if (Main.player[Projectile.owner].GetModPlayer<ArtemisPlayer>().targetWhoAmI == Target.whoAmI)
				{
					Projectile.scale = MathHelper.Clamp(counter * 3, 0, 1);
					Projectile.timeLeft = 12;
				}
				else
					Projectile.scale -= 0.083f;
				Projectile.Center = Target.Center;
			}
			else
				Projectile.active = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			float progress = counter % 1;
			float transparency = (float)Math.Pow(1 - progress, 2);
			float scale = 1 + progress;

			Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * transparency, Projectile.rotation, tex.Size() / 2, scale * Projectile.scale, SpriteEffects.None, 0);
			return true;
		}
	}
	public class ArtemisPlayer : ModPlayer
	{
		public int targetTime;
		public int targetWhoAmI;

		public override void PostUpdateEquips()
		{
			if ((targetTime = Math.Max(targetTime - 1, 0)) == 0)
				targetWhoAmI = -1;
		}
	}
}