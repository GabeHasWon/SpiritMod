using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Particles;
using System;
using Terraria.DataStructures;

namespace SpiritMod.Items.Weapon.Summon.StardustBomb
{
	public class StardustBomb : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.QueenSpiderStaff);
			Item.damage = 0;
			Item.mana = 12;
			Item.width = 40;
			Item.height = 40;
			Item.value = Item.sellPrice(0, 8, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.knockBack = 2.5f;
			Item.UseSound = SoundID.Item20;
			Item.DamageType = DamageClass.Summon;
			Item.shootSpeed = 10f;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.noUseGraphic = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc.ai[0] == player.whoAmI && npc.type == ModContent.NPCType<StardustBombNPC>())
				{
					npc.active = false;
					npc.netUpdate = true;
				}
			}

			if (player == Main.LocalPlayer)
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.SpawnStardustBomb, 2);
					packet.Write((byte)player.whoAmI);
					packet.WriteVector2(velocity);
					packet.Send();
				}
				else
				{
					int npcindex = NPC.NewNPC(player.GetSource_ItemUse(Item), (int)position.X, (int)position.Y + 100, ModContent.NPCType<StardustBombNPC>(), 0, player.whoAmI);
					Main.npc[npcindex].velocity = velocity;
					Main.npc[npcindex].netUpdate = true;
				}
			}
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.FragmentStardust, 18);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}
	}

	internal class StardustBombNPC : ModNPC
    {
        public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 7;

		public int OwnerWhoAmI { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }

		public ref float TimeAlive => ref NPC.ai[1];
		private readonly int timeAliveMax = 200;

		private readonly int highDamage = 5000; //General damage threshold for visual effects
		private int boomDamage;
        
		public override void SetDefaults()
        {
            NPC.width = 158;
            NPC.height = 197;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.lifeMax = 30000;
            NPC.damage = 0;
            NPC.defense = 0;
			NPC.HitSound = SoundID.NPCHit3;
            NPC.noTileCollide = true;
			NPC.noGravity = true;
            NPC.dontCountMe = true;
        }

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += .25f;
			NPC.frameCounter %= Main.npcFrameCount[Type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{
			if (++TimeAlive >= timeAliveMax)
			{
				if (boomDamage > 0) //Explode instantly
				{
					Explode();
				}
				else //Shrink over shrinkTime before dying
				{
					const int shrinkTime = 20;
					NPC.scale = 1f + (float)Math.Sin((TimeAlive - timeAliveMax) * .2f) * .75f;

					if (TimeAlive > (timeAliveMax + shrinkTime))
					{
						NPC.active = false;

						for (int j = 0; j < 14; j++)
						{
							int timeLeft = Main.rand.Next(20, 40);
							ParticleHandler.SpawnParticle(new StarParticle(NPC.Center, Main.rand.NextVector2Circular(10, 7), Color.Cyan, Main.rand.NextFloat(0.15f, 0.3f), timeLeft));
						}
					}
				}
			}
			else
			{
				if ((int)TimeAlive % 40 == 1)
				{
					float scale = MathHelper.Clamp((float)boomDamage / highDamage, 0, 1);
					DustHelper.DrawStar(NPC.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(100.0f)), 206, 5, scale, scale * 0.8f, scale * 1.5f);
				}
				NPC.scale = MathHelper.Min(TimeAlive / 15f, 1);
			}

			NPC.velocity *= .97f;
			NPC.rotation += .03f;
			Lighting.AddLight(NPC.Center, Color.White.ToVector3() * .005f);

			if ((int)TimeAlive == 20 && Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop, NPC.Center);
		}

		private void Explode()
		{
			Player owner = Main.player[OwnerWhoAmI];

			if (owner.whoAmI == Main.myPlayer)
				Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<StarShockwave>(), (int)owner.GetDamage(DamageClass.Summon).ApplyTo(boomDamage * .5f), 0, owner.whoAmI);
			
			if (Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Thunder"), NPC.Center);
			}
			for (int i = 0; i < 8; i++)
			{
				if (Main.netMode != NetmodeID.Server)
				{
					float randFloat = Main.rand.NextFloat(6.28f);
					int goreType = Mod.Find<ModGore>("StarbombGore" + ((i % 4) + 1)).Type;
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center + (randFloat.ToRotationVector2() * 60), Main.rand.NextFloat(6.28f).ToRotationVector2() * 16, goreType, 1f);
				}
				if (i < 4)
				{
					float scale = Main.rand.NextFloat(1.0f, 2.5f);
					DustHelper.DrawStar(NPC.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(120.0f)), 206, 5, scale, scale * 0.8f, Math.Min(scale, 1.8f));
				}
			}

			SpiritMod.tremorTime = 15;
			NPC.active = false;
		}

		public override bool? CanBeHitByItem(Player player, Item item) => (item.DamageType == DamageClass.Summon || item.DamageType == DamageClass.SummonMeleeSpeed) ? null : false;
		
		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			//Most minion projectiles don't use a summon damage class
			if (projectile.DamageType == DamageClass.Summon || projectile.DamageType == DamageClass.SummonMeleeSpeed || ProjectileID.Sets.MinionShot[projectile.type] || projectile.minion)
				return null;

			return false;
		}

		public override void HitEffect(NPC.HitInfo hit) => boomDamage += hit.Damage;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			float breakCounter = (float)MathHelper.Clamp((float)boomDamage / (highDamage * 2f), 0, .5f);

			SpiritMod.CircleNoise.Parameters["breakCounter"].SetValue(breakCounter * 5f);
			SpiritMod.CircleNoise.Parameters["rotation"].SetValue(0 - (NPC.rotation / 1.25f) + (breakCounter * 3.5f));
			SpiritMod.CircleNoise.Parameters["colorMod"].SetValue(Color.Silver.ToVector4());
			SpiritMod.CircleNoise.Parameters["noise"].SetValue(Mod.Assets.Request<Texture2D>("Textures/noise").Value);
			SpiritMod.CircleNoise.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49").Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), null, Color.White, 0f, new Vector2(50, 50), 0.64f + (breakCounter * 2f), SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49").Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), null, Color.Cyan with { A = 0 }, 0 - (NPC.rotation / 2), new Vector2(50, 50), 0.45f * NPC.scale + (breakCounter * 0.75f), SpriteEffects.None, 0f);

			for (int i = 0; i < 2; i++)
			{
				Rectangle frame = new Rectangle(0, 0, 48, 52);
				Color drawCol = Color.White;

				if (i > 0)
				{
					frame.Y = 52;
					drawCol *= breakCounter * 12f;
				}

				Main.spriteBatch.Draw(
					Mod.Assets.Request<Texture2D>("Items/Weapon/Summon/StardustBomb/StardustBombNPC_Star").Value,
					NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY),
					frame,
					drawCol,
					0 - (NPC.rotation / 2) + ((float)boomDamage / highDamage * 30f),
					frame.Size() / 2,
					NPC.scale,
					SpriteEffects.None, 0
				);
			}

            Main.spriteBatch.Draw(
                Mod.Assets.Request<Texture2D>("Items/Weapon/Summon/StardustBomb/StardustBombNPC").Value,
				NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY),
				NPC.frame,
				drawColor,
				NPC.rotation,
				NPC.frame.Size() / 2,
				NPC.scale,
				SpriteEffects.None, 0
			);
			return false;
        }

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			float num107 = 0f;

			SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.White);
			Color color28 = color29;
            color28 = NPC.GetAlpha(color28);
            color28 *= 0.5f;

            for (int num103 = 0; num103 < 6; num103++)
            {
                Vector2 vector29 = NPC.Center + ((float)num103 / 4f * 6.28318548f + NPC.rotation).ToRotationVector2() * (2f * num107 + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
                Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Weapon/Summon/StardustBomb/StardustBombNPC_Glow").Value, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }

            num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;

            spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.White);
            color28 = color29;
            color28 = NPC.GetAlpha(color28);
            color28 *= 1f - num107;

            for (int num103 = 0; num103 < 6; num103++)
            {
                Vector2 vector29 = NPC.Center + ((float)num103 / 4f * 6.28318548f + NPC.rotation).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
                Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Weapon/Summon/StardustBomb/StardustBombNPC_Glow").Value, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) => boundingBox = Rectangle.Empty;
	}
}
