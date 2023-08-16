using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Utilities;
using SpiritMod.Particles;
using Terraria.DataStructures;

namespace SpiritMod.Items.Sets.DashSwordSubclass.BladeOfTheDragon
{
    public class BladeOfTheDragon : DashSwordItem
    {
		public int combo;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of the Dragon");
            Tooltip.SetDefault("Hold and release to slice through nearby enemies\nBuild up a combo by repeatedly hitting enemies");
            Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
        {
            Item.channel = true;
            Item.damage = 150;
			Item.knockBack = 1;
			Item.crit = 4;
			Item.width = Item.height = 60;
			Item.useTime = Item.useAnimation = 90;
			Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = false;
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<BladeOfTheDragonProj>();
            Item.shootSpeed = 6f;
            Item.noUseGraphic = true;
        }

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override void DrawHeld(PlayerDrawSet info)
		{
			Item item = info.drawPlayer.HeldItem;
			Texture2D texture = ModContent.Request<Texture2D>(item.ModItem.Texture + "_Held").Value;

			Rectangle frame = texture.Frame(1, 2, 0, info.drawPlayer.channel ? 1 : 0);
			Vector2 origin = new Vector2(texture.Width * (.5f + (.25f * info.drawPlayer.direction)), frame.Height * .5f);

			Vector2 offset = new Vector2(14, frame.Height / 2 * info.drawPlayer.gravDir);
			ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);
			offset = new Vector2(14 * info.drawPlayer.direction, offset.Y);

			Color lightColor = Lighting.GetColor((int)info.drawPlayer.Center.X / 16, (int)info.drawPlayer.Center.Y / 16);
			Texture2D sparkle = TextureAssets.Projectile[79].Value;

			Vector2 position = new Vector2((int)(info.drawPlayer.Center.X - Main.screenPosition.X + offset.X), (int)(info.drawPlayer.Center.Y - Main.screenPosition.Y + offset.Y) + info.drawPlayer.gfxOffY);

			info.DrawDataCache.Add(new DrawData(texture, position, frame, lightColor, 0, origin, item.scale, info.playerEffect, 0));
			if (info.drawPlayer.channel && !info.drawPlayer.GetModPlayer<DashSwordPlayer>().dashing)
				info.DrawDataCache.Add(new DrawData(sparkle, position + new Vector2(-6 * info.drawPlayer.direction, -12 * info.drawPlayer.gravDir),
					null, Color.White, Main.GlobalTimeWrappedHourly * 0.5f, sparkle.Size() / 2, item.scale * .5f, SpriteEffects.None, 0));
		}
	}

    public class BladeOfTheDragonProj : DashSwordProjectile
    {
		private DragonPrimTrail trail;
		private Vector2 lastPos;

		public override int ChargeupTime => 20;
		public override int DashDuration => 9;
		public override int StrikeDelay => 2;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Blade of the Dragon");

		public override void SetDefaults()
		{
            Projectile.width = Projectile.height = 40;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = 12;
			Projectile.tileCollide = false;
		}

        public override void AbstractAI()
        {
			if (Counter == ChargeupTime && Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/slashdash") with { PitchVariance = 0.4f, Volume = 0.4f }, Projectile.Center);
				SpiritMod.primitives.CreateTrail(trail = new DragonPrimTrail(Projectile));
			} //The dash has just started
			if (Counter > ChargeupTime && Counter < (ChargeupTime + DashDuration))
			{
				if (Owner.velocity.Length() > 2f)
				{
					for (int i = 0; i < (int)(Owner.velocity.Length() / 7f); i++)
					{
						ParticleHandler.SpawnParticle(new ImpactLine(Vector2.Lerp(lastPos, Projectile.Center, Main.rand.NextFloat()) + Main.rand.NextVector2Circular(35, 35), Vector2.Normalize(Owner.velocity) * .5f, Color.Lerp(Color.Green, Color.LightGreen, Main.rand.NextFloat()), new Vector2(0.25f, Main.rand.NextFloat(0.5f, 1.5f)) * 3, 60)
						{ TimeActive = 30 });
					}
				}
				lastPos = Owner.Center;
			}
			if (Counter == (ChargeupTime + DashDuration + 1) && Main.netMode != NetmodeID.Server)
			{
				if (trail != null)
					trail.OnDestroy();
			}//The dash has just ended a tick before
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
			=> Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<DragonSlash>(), 0, 0, Main.player[Projectile.owner].whoAmI, target.whoAmI);
	}

	internal class DragonSlash : ModProjectile, IDrawAdditive
	{
		int frameX = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dragon Slash");
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = false;
			Projectile.Size = new Vector2(88, 123);
			Projectile.penetrate = -1;
			Projectile.hide = true;
		}

		public override void AI()
		{
			if (Projectile.frameCounter == 0)
			{
				frameX = Main.rand.Next(2);
				Projectile.rotation = Main.rand.NextFloat(6.28f);
			}

			Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;
			Projectile.velocity = Vector2.Zero;
			Projectile.frameCounter++;

			if (Projectile.frameCounter % 5 == 0)
				Projectile.frame++;
			if (Projectile.frame >= Main.projFrames[Projectile.type])
				Projectile.active = false;
		}

		public void AdditiveCall(SpriteBatch sB, Vector2 screenPos)
		{
			//Adjust framing due to secondary column
			Rectangle frame = Projectile.DrawFrame();
			frame.Width /= 2;
			frame.X = frame.Width * frameX;

			void Draw(Vector2 offset, float opacity)
			{
				sB.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + offset - Main.screenPosition, frame, 
					Color.White * opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}

			PulseDraw.DrawPulseEffect((float)Math.Asin(-0.6), 8, 12, delegate (Vector2 posOffset, float opacityMod)
			{
				Draw(posOffset, opacityMod * 0.33f);
			});
			Draw(Vector2.Zero, 1);
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;
	}
}