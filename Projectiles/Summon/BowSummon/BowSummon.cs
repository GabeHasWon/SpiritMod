using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Accessory.BowSummonItem;
using SpiritMod.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon.BowSummon
{
	public class BowSummon : ModProjectile
	{
		private Item ammoItem;

		public ref float Counter => ref Projectile.ai[0];

		public Player Owner => Main.player[Projectile.owner];

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.BowSummonItem.DisplayName");

		public override void SetStaticDefaults()
		{
			Main.projPet[Type] = true;
			Main.projFrames[Type] = 4;

			ProjectileID.Sets.MinionSacrificable[Type] = true;
			ProjectileID.Sets.CultistIsResistantTo[Type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 18000;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			if (Owner.HasAccessory<BowSummonItem>())
				Projectile.timeLeft = 2;

			NPC target = (Owner.MinionAttackTargetNPC > -1 && Main.npc[Owner.MinionAttackTargetNPC] is NPC npc) ? 
				npc : 
				Main.npc.Where(x => x.CanBeChasedBy(Projectile) && x.Distance(Projectile.Center) < 900 && Collision.CanHit(Projectile.Center, 0, 0, x.position, x.width, x.height)).OrderBy(x => x.Distance(Projectile.Center)).FirstOrDefault();

			if (target != default)
			{
				if (++Counter >= 70 && Projectile.Distance(target.Center) < (16 * 30))
				{
					if (Counter == 70)
						ammoItem = Owner.PickAmmo(ContentSamples.ItemsByType[ItemID.WoodenBow], out _, out _, out _, out _, out int type) ? ContentSamples.ItemsByType[type] : null;
					//Select an ammo item to use in PreDraw before firing

					if (++Projectile.frameCounter % 5 == 0)
						Projectile.frame++;
					if (Projectile.frame >= Main.projFrames[Type])
						Shoot(target);
				}
				Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.AngleTo(target.Center), .3f);
			}
			else
			{
				Projectile.rotation = Utils.AngleLerp(Projectile.rotation, (Owner.direction == -1) ? MathHelper.Pi : 0, .15f);
				Projectile.frameCounter = Projectile.frame = 0;
			}

			float offY = (float)Math.Sin(Main.timeForVisualEffects / 30f) * 2f;
			Projectile.Center = Owner.Center - new Vector2(0, 50 - Owner.gfxOffY + offY);
			Projectile.spriteDirection = (Projectile.rotation.ToRotationVector2().X < 0) ? -1 : 1;
			Projectile.velocity *= .95f;
		}

		private void Shoot(NPC target)
		{
			#region stats
			int type = ProjectileID.JestersArrow;
			float speed = 8f;
			int damage = Projectile.damage;
			float knockback = Projectile.knockBack;

			if (ammoItem != null)
			{
				type = ammoItem.shoot;
				speed = MathHelper.Min(speed + ammoItem.shootSpeed, 20);
				damage += ContentSamples.ItemsByType[ammoItem.type].damage;
				knockback += ContentSamples.ItemsByType[ammoItem.type].knockBack;
				//Avoid using Player.PickAmmo to assign stats because we don't want the projectile to benefit from ranged buffs
			}
			Vector2 velocity = Projectile.DirectionTo(target.Center + (target.velocity * (Projectile.Distance(target.Center) / speed))) * speed;
			#endregion

			if (Main.myPlayer == Projectile.owner)
			{
				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + velocity, velocity, type, damage, knockback, Owner.whoAmI);
				proj.DamageType = DamageClass.Summon;
				proj.friendly = true;
				proj.netUpdate = true;
			}
			Projectile.frame = Projectile.frameCounter = 0;
			Counter = 0;
			Projectile.velocity = Vector2.Normalize(velocity) * 4f;
		}

		public override bool? CanDamage() => false;

		public override bool? CanCutTiles() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.QuickDraw(spriteEffects: (Projectile.spriteDirection == -1) ? SpriteEffects.FlipVertically : SpriteEffects.None);
			if (Projectile.frame > 0)
			{
				int type = (ammoItem == null) ? ProjectileID.JestersArrow : ammoItem.shoot;
				Texture2D arrow = TextureAssets.Projectile[type].Value;

				float offX = (float)Projectile.frameCounter / (float)(Main.projFrames[Type] * 5f);

				Rectangle frame = ContentSamples.ProjectilesByType[type].DrawFrame();
				Color color = lightColor * MathHelper.Min(offX / .5f, 1);
				Vector2 pos = Projectile.Center + new Vector2(0, Projectile.gfxOffY) - new Vector2((offX * offX * 15) - 10, 0).RotatedBy(Projectile.rotation);
				float rotation = Projectile.rotation + MathHelper.PiOver2;

				Main.EntitySpriteDraw(arrow, pos - Main.screenPosition, frame, Projectile.GetAlpha(color), rotation, frame.Size() / 2, 1, SpriteEffects.None);
			}
			return false;
		}
	}
}