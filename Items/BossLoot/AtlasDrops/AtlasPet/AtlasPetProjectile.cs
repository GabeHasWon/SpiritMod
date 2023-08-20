using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AtlasDrops.AtlasPet
{
	public class AtlasPetProjectile : ModProjectile
	{
		private const int HandOldPosSlot = 5;

		private Player Owner => Main.player[Projectile.owner];
		private NPC Target => Main.npc[(int)TargetNPC];
		private Vector2 HandPosition(Vector2 off) => Vector2.Lerp(Projectile.oldPos[HandOldPosSlot], Target.position + off, _handFactor);

		public ref float State => ref Projectile.ai[0];
		public ref float TargetNPC => ref Projectile.ai[1];

		private readonly List<AtlasPetPart> _parts = new();
		private float _handFactor = 0;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Atlas Jr.");
			Main.projFrames[Projectile.type] = 2;
			Main.projPet[Projectile.type] = true;

			ProjectileID.Sets.TrailCacheLength[Type] = 8;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Truffle);
			Projectile.aiStyle = 0;
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.light = 0;
			Projectile.tileCollide = false;

			AIType = 0;
		}

		public override void AI()
		{
			Owner.GetModPlayer<GlobalClasses.Players.PetPlayer>().PetFlag(Projectile);

			if (State == 0)
				Initialize();
			else
			{
				Behaviour();
				UpdateParts();
			}

			if (Main.rand.NextBool(3))
			{
				Vector2 position = Projectile.oldPos[1] + (Projectile.Size / 2) + new Vector2(2, 8);

				Vector2 factor = Main.rand.NextVector2Unit() * 10f;
				factor.Y *= .5f;
				float scale = Main.rand.NextFloat(0.2f, 1.0f);

				Vector2 velocity = Vector2.Normalize(factor).RotatedBy(1.57f) * scale;
				velocity.Y *= .35f;

				Dust dust = Dust.NewDustPerfect(position + factor, DustID.PinkTorch, velocity, 80, default, scale);
				dust.noGravity = true;
				dust.noLightEmittence = true;
			}
		}

		private void Behaviour()
		{
			bool dist = TargetNPC == -1 ? Projectile.DistanceSQ(Owner.Center) > 100 * 100 : Projectile.DistanceSQ(Target.Center) > 60 * 60;
			if (!Main.mouseLeft && dist)
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(TargetNPC != -1 ? Target.Center : Owner.Center) * 16, 0.015f);
			else
				Projectile.velocity *= TargetNPC == -1 ? 0.96f : 0.88f;

			if (TargetNPC == -1)
				FindTargetIfAny();
			else
				GrabTarget();
		}

		private void GrabTarget()
		{
			if (!Target.active || Target.life <= 0 || Target.DistanceSQ(Projectile.Center) > 700 * 700)
			{
				for (int i = 0; i < 10; ++i)
					Dust.NewDust(HandPosition(Vector2.Zero), 10, 10, Main.rand.NextBool() ? DustID.Shadowflame : DustID.Ebonwood);

				TargetNPC = -1;
				_handFactor = 0;
				return;
			}

			//if (TargetNPC != Owner.lastCreatureHit && Owner.lastCreatureHit >= 0 && Main.npc[Owner.lastCreatureHit].CanBeChasedBy())
			//	TargetNPC = Owner.lastCreatureHit;

			_handFactor += 0.01f;

			if (_handFactor > 1f)
				_handFactor = 1f;
		}

		private void FindTargetIfAny()
		{
			//if (Owner.lastCreatureHit >= 0 && Main.npc[Owner.last].CanBeChasedBy())
			//{
			//	TargetNPC = Owner.lastCreatureHit;
			//	return;
			//}

			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				NPC npc = Main.npc[i];

				if (npc.CanBeChasedBy() && npc.DistanceSQ(Projectile.Center) < 600 * 600 && (TargetNPC == -1 || Target.DistanceSQ(Projectile.Center) > npc.DistanceSQ(Projectile.Center)))
					TargetNPC = npc.whoAmI;
			}
		}

		private void UpdateParts()
		{
			foreach (var item in _parts)
			{
				item.Update();

				int slot = new int[4] { 2, HandOldPosSlot, 0, HandOldPosSlot }[item.column];
				item.position = Projectile.oldPos[slot];

				float adj = Projectile.oldPos[slot].X - Projectile.oldPos[slot + 1].X;
				item.effects = Math.Sign(float.IsNaN(adj) ? 0 : adj) == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

				if ((item.column == 1 || item.column == 3) && TargetNPC != -1)
				{
					item.position = HandPosition(item.column == 0 ? new Vector2(10, 0) : new Vector2(-10, 0));
					item.effects = Target.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
				}
			}
		}

		private void Initialize()
		{
			for (int i = 0; i < 4; ++i)
			{
				int col = i;
				AtlasPetPart part = new AtlasPetPart()
				{ 
					column = col,
					position = Projectile.Center
				};

				_parts.Add(part);
			}

			TargetNPC = -1;
			State = 1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D ray = Mod.Assets.Request<Texture2D>("Textures/Ray_2").Value;
			Vector2 offset = new Vector2((Projectile.direction == -1) ? 4 : 0, -4);
			Color color = (Color.Purple * 1.8f * (1f - (float)(Projectile.velocity.Length() * 0.2f))) with { A = 0 };

			Main.spriteBatch.Draw(ray, Projectile.oldPos[1] + (Projectile.Size / 2) + offset - Main.screenPosition, null, color, 
				Projectile.rotation, new Vector2(ray.Width / 2, 0), new Vector2(1.2f, 0.3f) * Projectile.scale, SpriteEffects.None, 0);

			foreach (var item in _parts)
				item.Draw();
			return false;
		}
	}
}
