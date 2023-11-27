using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Prim;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Clubs
{
	class EnergizedAxeProj : ClubProj
	{
		private bool spawnedTrail = false;

		public EnergizedAxeProj() : base(new Vector2(96)) { }

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.RageBlazeDecapitator.DisplayName");

		public override void SafeSetStaticDefaults()
		{
			// DisplayName.SetDefault("Unstable Adze");
			Main.projFrames[Projectile.type] = 3;
		}

		public override void Smash(Vector2 position)
		{
			for (int k = 0; k <= 100; k++)
			{
				Vector2 newPosition = Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2);

				Dust.NewDustPerfect(newPosition, ModContent.DustType<Dusts.BoneDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 9f);
				if (k < 30)
					Dust.NewDustPerfect(newPosition, DustID.Electric, new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 9f);
			}

			if (Main.player[Projectile.owner].whoAmI == Main.myPlayer)
			{
				Vector2 spawnPos = Projectile.Center;
				for (int i = 0; i < 10; i++)
				{
					Tile tile = Framing.GetTileSafely(spawnPos / 16);
					Tile aboveTile = Framing.GetTileSafely((spawnPos / 16) - Vector2.UnitY);

					if (WorldGen.SolidTile(tile) && !WorldGen.SolidTile(aboveTile))
						break;
					else
						spawnPos.Y -= 16;
				}

				Vector2 velocity = Vector2.UnitX * 12 * Main.player[Projectile.owner].direction;
				int id = Projectile.NewProjectile(Projectile.GetSource_FromAI("ClubSmash"), spawnPos, velocity, ModContent.ProjectileType<EnergizedShockwave>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
				Main.projectile[id].position.Y += Projectile.height + 32;

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncProjectile, number: id);
			}

			if (Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Item/GranitechLaserBlast"), position);
		}

		public override void SafeAI()
		{
			if (!spawnedTrail && Projectile.ai[0] >= ChargeTime && Main.netMode != NetmodeID.Server)
			{
				SpiritMod.primitives.CreateTrail(new SkullPrimTrail(Projectile, new Color(90, 150, 255), 30, 5));
				spawnedTrail = true;
			}
		}

		public override void SafeDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (Projectile.ai[0] >= ChargeTime)
			{
				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
				Rectangle drawFrame = texture.Frame(1, Main.projFrames[Type], 0, 2, 0, 0);

				spriteBatch.Draw(texture, Main.player[Projectile.owner].Center - Main.screenPosition, drawFrame, Color.White * 0.9f, TrueRotation, Origin, Projectile.scale, Effects, 0);
			}
		}
	}
}
