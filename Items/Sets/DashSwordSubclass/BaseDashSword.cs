using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.BaseProj;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.DashSwordSubclass
{
	public abstract class DashSwordItem : ModItem
	{
		public override bool CanUseItem(Player player)
		{
			if (player.GetModPlayer<DashSwordPlayer>().hasDashCharge)
			{
				player.GetModPlayer<DashSwordPlayer>().hasDashCharge = false;
				return true;
			}
			return false;
		}

		public override void HoldItem(Player player)
		{
			player.GetModPlayer<DashSwordPlayer>().holdingSword = true;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -1.1f * player.direction);
		}

		/// <summary>
		/// Controls the drawn visual when this item is held by the player. Make sure that DashSwordPlayer.holdingSword returns true for this to be called.
		/// </summary>
		/// <param name="info"></param>
		public virtual void DrawHeld(PlayerDrawSet info)
		{
			Item item = info.drawPlayer.HeldItem;
			Texture2D texture = ModContent.Request<Texture2D>(item.ModItem.Texture + "_Held").Value;

			Rectangle frame = texture.Frame(1, 2, 0, info.drawPlayer.channel ? 1 : 0);
			Vector2 origin = new Vector2(texture.Width * (.5f + (.25f * info.drawPlayer.direction)), frame.Height * .5f);

			Vector2 offset = new Vector2(6, frame.Height / 2 * info.drawPlayer.gravDir);
			ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);
			offset = new Vector2(6 * info.drawPlayer.direction, offset.Y);

			Color lightColor = Lighting.GetColor((int)info.drawPlayer.Center.X / 16, (int)info.drawPlayer.Center.Y / 16);
			Texture2D sparkle = TextureAssets.Projectile[79].Value;

			Vector2 position = new Vector2((int)(info.drawPlayer.Center.X - Main.screenPosition.X + offset.X), (int)(info.drawPlayer.Center.Y - Main.screenPosition.Y + offset.Y) + info.drawPlayer.gfxOffY);

			info.DrawDataCache.Add(new DrawData(texture, position, frame, lightColor, 0, origin, item.scale, info.playerEffect, 0));
			if (info.drawPlayer.channel && !info.drawPlayer.GetModPlayer<DashSwordPlayer>().dashing)
				info.DrawDataCache.Add(new DrawData(sparkle, position + new Vector2(-6 * info.drawPlayer.direction, -2 * info.drawPlayer.gravDir),
					null, Color.White, Main.GlobalTimeWrappedHourly * 0.5f, sparkle.Size() / 2, item.scale * .5f, SpriteEffects.None, 0));
		}
	}

	public abstract class DashSwordProjectile : BaseHeldProj
	{
		public ref float Counter => ref Projectile.ai[0];

		public virtual int ChargeupTime => 0;
		public virtual int DashDuration => 3;
		public virtual int StrikeDelay => 20;
		public virtual int WaitTime => 0;

		public Vector2 startPos = Vector2.Zero;
		public Vector2 endPos = Vector2.Zero;

		public List<int> targets = new();

		private bool shouldDamage = true;

		public override void AI()
		{
			base.AI();

			if (Counter < ChargeupTime)
			{
				if (!Owner.channel)
					Projectile.Kill();
			}
			else if (Counter < (ChargeupTime + DashDuration))
			{
				if (Counter == ChargeupTime)
					startPos = Owner.Center;
				if (Owner.HeldItem.channel && !Owner.channel) //Cancel the dash early
					Counter = ChargeupTime + DashDuration - 1;

				Owner.GetModPlayer<DashSwordPlayer>().dashing = true;
				Owner.velocity = Vector2.Normalize(Projectile.velocity) * 80f; //* 16 * 8;

				float collisionPoint = 0;
				var crossed = Main.npc.Where(x => (x.CanBeChasedBy(Projectile) || x.type == NPCID.TargetDummy) && x.active && Collision.CheckAABBvLineCollision(x.Hitbox.TopLeft(), x.Hitbox.Size(), startPos, Owner.Center, 50, ref collisionPoint)).OrderBy(x => x.Distance(startPos));
				foreach (NPC npc in crossed)
				{
					if (!targets.Contains(npc.whoAmI) && (Projectile.maxPenetrate > targets.Count || Projectile.maxPenetrate == -1))
						targets.Add(npc.whoAmI);
				} //Find targets
			}
			else
			{
				Owner.channel = false;

				if (Counter == (ChargeupTime + DashDuration))
				{
					Owner.velocity *= .001f;
					endPos = Owner.Center;
				}
				if (Counter >= (ChargeupTime + DashDuration + StrikeDelay + WaitTime))
					Projectile.Kill();
			}

			Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
			Counter++;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (CanDamage() == false)
				return false;

			bool Matching(Rectangle hitbox)
			{
				foreach (int whoAmI in targets)
				{
					if (Main.npc[whoAmI].Hitbox == hitbox)
						return true;
				}
				return false;
			}

			if (Counter == (ChargeupTime + DashDuration + StrikeDelay) && Matching(targetHitbox))
			{
				if (Projectile.penetrate == 1)
				{
					Projectile.penetrate++;
					shouldDamage = false;
				}

				return true;
			}
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => Owner.GetModPlayer<DashSwordPlayer>().hasDashCharge = true;

		public override bool? CanDamage() => shouldDamage ? null : false;

		public override bool ShouldUpdatePosition() => false;

		public override bool AutoAimCursor() => Counter < ChargeupTime;
	}
}