using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.AstralAmalgam
{
	public class SpaceShield : ModNPC, IDrawAdditive
	{
		private float Counter
		{
			get => NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private readonly int counterMax = 90;
		private readonly int cooldownTime = 300; //5 seconds

		private readonly float rechargeRate = 0.1f;

		private bool Inactive => Counter < 0;

		private float Degrees
		{
			get => NPC.ai[1];
			set => NPC.ai[1] = value;
		}

		private int ParentIndex
		{
			get => (int)NPC.ai[3];
			set => NPC.ai[3] = value;
		}

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Buffer");
			NPCHelper.BuffImmune(Type, true);

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.noTileCollide = true;
			NPC.width = 32;
			NPC.height = 32;
			NPC.netAlways = true;
			NPC.damage = 15;
			NPC.defense = 9999;
			NPC.npcSlots = 0;
			NPC.dontTakeDamage = true;
			NPC.lifeMax = 1;
			NPC.friendly = false;
			NPC.chaseable = false;
			NPC.noGravity = true;
			NPC.knockBackResist = 0f;
			NPC.dontCountMe = true;
			NPC.dontTakeDamage = true;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override bool PreAI()
		{
			NPC.life = 100;

			if (Counter < 0)
				Counter++;
			else if (Counter > 0)
				Counter -= rechargeRate;

			NPC parent = (ParentIndex < (double)Main.npc.Length) ? Main.npc[ParentIndex] : null;
			float num8 = Main.LocalPlayer.miscCounter / 60f;

			if (!Inactive)
			{
				int chance = (int)Math.Abs(MathHelper.Clamp(Counter + 1, -counterMax, 1));
				if (Main.rand.NextBool(chance))
				{
					for (int i = 0; i < 6; i++)
					{
						Vector2 position = NPC.Center + (num8 * 6.28318548f + 1.05f * i).ToRotationVector2() * 12f;

						Dust dust = Dust.NewDustPerfect(position, DustID.DungeonSpirit, Vector2.Zero, 100, default, .8f);
						dust.noGravity = true;
						dust.velocity = (parent != null) ? parent.velocity : Vector2.Zero;
						dust.noLight = true;
					}
				}

				CheckCollision();
			}

			NPC.rotation += 0.04f;

			int fadeinTime = 10;
			if (Counter > -fadeinTime)
				NPC.scale = Inactive ? (float)(Counter + fadeinTime) / fadeinTime : 1f + (float)((float)Counter / counterMax * .5f);
			else
				NPC.scale = 0f;

			if (parent != null)
			{
				//Factors for calculations
				double deg = Degrees; //The degrees, you can multiply npc.ai[1] to make it orbit faster, may be choppy depending on the value
				double rad = deg * (Math.PI / 180); //Convert degrees to radians
				double dist = 60; //Distance away from the npc

				//Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
				Degrees += 1.2f;

				NPC.position.X = parent.Center.X - (int)(Math.Cos(rad) * dist) - NPC.width / 2;
				NPC.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * dist) - NPC.height / 2;

				NPC.life = (!parent.active || parent.type != ModContent.NPCType<AstralAmalgam>()) ? 0 : 1;

				if (NPC.life == 0)
				{
					NPC.active = false;
					OnKill();

					NPC.netUpdate = true;
				}
			}
			return false;
		}

		private void CheckCollision()
		{
			foreach(Projectile proj in Main.projectile)
			{
				if (proj.Hitbox.Intersects(NPC.Hitbox) && proj.localNPCImmunity[NPC.whoAmI] <= 0 && proj.friendly && !proj.minion && !proj.sentry && !Main.player[proj.owner].channel)
				{
					int damage = proj.damage;
					int thresholdCount = 3;

					if ((Counter += Math.Min(counterMax / thresholdCount, damage)) > counterMax) //The Shield is destroyed
					{
						SoundEngine.PlaySound(SoundID.Item91, NPC.Center);

						for (int i = 0; i < 3; i++)
						{
							Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f, 8.0f);

							Projectile.NewProjectile(NPC.GetSource_OnHurt(proj), NPC.Center, velocity, ModContent.ProjectileType<AstralCluster>(), 20, 3, Main.myPlayer);

							for (int o = 0; o < 3; o++)
							{
								float randomScale = Main.rand.NextFloat(0.2f, 1.0f);

								ParticleHandler.SpawnParticle(new GlowParticle(NPC.Center, velocity * .5f, Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat()), randomScale * 0.12f, (int)(randomScale * 20)));
							}
						}
						DustHelper.DrawStar(NPC.Center, 206, 4, 2, 1.5f, 1.3f);

						Counter = -cooldownTime;
					}

					proj.damage = (int)(damage * 0.8f);
					proj.localNPCImmunity[NPC.whoAmI] = 15;

					//SoundEngine.PlaySound(SoundID.Item14, NPC.position);

					for (int k = 0; k < 15; k++)
						Dust.NewDustPerfect(NPC.getRect().ClosestPointInRect(proj.Center), DustID.DungeonSpirit, (Vector2.Normalize(-proj.velocity) * Main.rand.NextFloat(3.0f, 5.0f)).RotatedByRandom(.8f), 0, default, 1.1f).noGravity = true;

					break;
				}
			}
		}

		public override void OnKill()
		{
			SoundEngine.PlaySound(SoundID.Item14, NPC.position);

			for (int i = 0; i < 20; i++)
			{
				int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple, 0f, -2f, 0, default, 1.2f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[num].position != NPC.Center)
					Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);
			Texture2D bloom = TextureAssets.Extra[49].Value;
			SpiritMod.SunOrbShader.Parameters["colorMod"].SetValue(new Color(120, 190, 255).ToVector4());
			SpiritMod.SunOrbShader.Parameters["colorMod2"].SetValue(Color.LightBlue.ToVector4());
			SpiritMod.SunOrbShader.Parameters["timer"].SetValue(Main.GlobalTimeWrappedHourly / 3 % 1);
			SpiritMod.SunOrbShader.CurrentTechnique.Passes[0].Apply();

			float scale = MathHelper.Lerp(0.4f, 0.6f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) / 2 + 0.5f);
			Color drawcolor = NPC.GetAlpha(Color.Blue);
			Vector2 drawcenter = NPC.Center - Main.screenPosition;

			Main.spriteBatch.Draw(bloom, drawcenter, null, drawcolor, NPC.rotation, bloom.Size() / 2, NPC.scale * 0.66f * MathHelper.Lerp(scale, 1, 0.25f), SpriteEffects.None, 0);

			Main.spriteBatch.Draw(bloom, drawcenter, null, drawcolor * 0.2f, NPC.rotation, bloom.Size() / 2, NPC.scale * scale, SpriteEffects.None, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

			return false;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			Vector2 primScale = new Vector2(30);

			for (int i = 0; i < 2; i++)
			{
				float zRot = Math.Abs(((Degrees / 30) + (MathHelper.PiOver2 * i)).ToRotationVector2().Y);
				primScale += Vector2.One * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * (3 + i)) - (i * 10));

				Effect effect = SpiritMod.ShaderDict["PulseCircle"];
				Color rColor = NPC.GetAlpha(new Color(100, 180, 255));

				effect.Parameters["BaseColor"].SetValue(rColor.ToVector4());
				effect.Parameters["RingColor"].SetValue(rColor.ToVector4());
				var square = new Prim.SquarePrimitive
				{
					Color = rColor * .5f,
					Length = primScale.X * zRot * NPC.scale,
					Height = primScale.Y * NPC.scale,
					Position = NPC.Center - Main.screenPosition,
					Rotation = NPC.rotation + (MathHelper.PiOver2 * i),
					ColorXCoordMod = 1 - zRot
				};
				Prim.PrimitiveRenderer.DrawPrimitiveShape(square, effect);
			}
		}
	}
}
