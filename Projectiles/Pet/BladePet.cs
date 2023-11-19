using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Players;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Pet
{
	public class BladePet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 5;
			ProjectileID.Sets.TrailingMode[Type] = 2;
			Main.projPet[Type] = true;

			ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, 0)
				.WithCode(DelegateMethods.CharacterPreview.Float);
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 46;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.hostile = false;
		}

		private int TargetIndex
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			player.GetModPlayer<PetPlayer>().PetFlag(Projectile);

			float speedMult = MathHelper.Clamp(Projectile.Distance(player.Center) / 50, 4, 15);

			Vector2 restingPos = player.Center - new Vector2(50 * player.direction, 40 * player.gravDir);
			if (Projectile.Distance(restingPos) > 60)
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(restingPos) * speedMult, .1f);
			else
				Projectile.velocity *= .95f;

			Projectile.position.Y += Main.GlobalTimeWrappedHourly.ToRotationVector2().Y / 4f; //Cause the projectile to bob up and down

			int maxRange = 1200;
			int lastRarity = 0;

			TargetIndex = -1;
			foreach (NPC npc in Main.npc)
			{
				if (npc.active && npc.rarity > lastRarity && Projectile.Distance(npc.Center) <= maxRange) //Find the rarest NPC in range
				{
					lastRarity = npc.rarity;
					TargetIndex = npc.whoAmI;
				}
			}

			if (TargetIndex >= 0 && Main.npc[TargetIndex] != null)
			{
				NPC target = Main.npc[TargetIndex];

				Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.AngleTo(target.Center), 0.1f);

				int loops = Math.Max(2, (target.width + target.height) / 50);
				for (int i = 0; i < loops; i++)
				{
					Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, Main.rand.NextBool(2) ? DustID.Shadowflame : DustID.PurpleCrystalShard);
					dust.velocity = new Vector2(0, -2);
					dust.scale = target.scale * Main.rand.NextFloat(0.8f, 1.3f);
					dust.noGravity = true;
				}
			}
			else
			{
				float directionTo = player.Center.X - Projectile.Center.X;
				Projectile.rotation = Utils.AngleLerp(Projectile.rotation, (Projectile.velocity.X / 8) + ((directionTo < 0) ? MathHelper.Pi : 0), 0.1f);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.LightPink) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

				Main.EntitySpriteDraw(texture, drawPos, null, (color * .5f) with { A = 0 }, Projectile.oldRot[k], texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}
	}
}