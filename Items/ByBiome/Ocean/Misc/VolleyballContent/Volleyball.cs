using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.SportSystem;
using SpiritMod.Mechanics.SportSystem.Volleyball;
using System;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.ByBiome.Ocean.Misc.VolleyballContent;

[Sacrifice(1)]
internal class Volleyball : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	public override void SetDefaults()
	{
		Item.width = Item.height = 26;
		Item.rare = ItemRarityID.White;
		Item.value = Item.buyPrice(copper: 20);
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 20;
		Item.noMelee = true;
		Item.autoReuse = true;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<VolleyballProjectile>();
		Item.shootSpeed = 8;
		Item.UseSound = SoundID.Item1;
		Item.consumable = true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		var court = SportCourts.CourtAt<VolleyballGameTracker>(player.Center.ToTileCoordinates());
		if (court is not null)
		{
			VolleyballGameTracker tracker = court.tracker as VolleyballGameTracker;

			if (!court.Active)
				tracker.Start();

			int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

			if (tracker.volleyballWhoAmI == -1)
			{
				(Main.projectile[proj].ModProjectile as VolleyballProjectile).courtLocation = court.center;
				tracker.volleyballWhoAmI = (short)proj;
			}
			return false;
		}
		return true;
	}
}

internal class VolleyballProjectile : ModProjectile
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	public override string Texture => base.Texture.Replace("Projectile", "");

	private ref float HitTimer => ref Projectile.ai[0];
	private ref float TimesHit => ref Projectile.ai[1];

	internal Point? courtLocation = null;

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.BeachBall);
		Projectile.width = Projectile.height = 26;
		Projectile.hostile = false;
		Projectile.friendly = false;
	}

	public override void AI() //Adapted mostly from vanilla code
	{ 
		Projectile.timeLeft = 10;
		HitTimer++;

		if (HitTimer >= 20f)
		{
			HitTimer = 18f;
			Rectangle hitbox = new((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);

			for (int i = 0; i < 255; i++)
			{
				Entity plr = Main.player[i];
				if (plr.active && hitbox.Intersects(plr.Hitbox))
					HitSingleEntity(plr);
			}

			for (int i = 0; i < 1000; i++)
			{
				if (i == Projectile.whoAmI)
					continue;

				Entity entity = Main.projectile[i];

				if (entity.active && hitbox.Intersects(entity.Hitbox))
					HitSingleEntity(entity);
			}
		}

		if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
			Projectile.Kill();

		Projectile.rotation += 0.02f * Projectile.velocity.X;

		if (Projectile.velocity.Y == 0f)
			Projectile.velocity.X *= 0.98f;
		else if (Projectile.wet)
			Projectile.velocity.X *= 0.99f;
		else
			Projectile.velocity.X *= 0.995f;

		if (Projectile.velocity.X > -0.03 && Projectile.velocity.X < 0.03)
			Projectile.velocity.X = 0f;

		if (Projectile.wet)
		{
			TimesHit = 0f;

			if (Projectile.velocity.Y > 0f)
				Projectile.velocity.Y *= 0.95f;

			Projectile.velocity.Y -= 0.1f;

			if (Projectile.velocity.Y < -4f)
				Projectile.velocity.Y = -4f;

			if (Projectile.velocity.X == 0f)
				Projectile.Kill();
		}
		else
			Projectile.velocity.Y += 0.1f;

		if (Projectile.velocity.Y > 10f)
			Projectile.velocity.Y = 10f;

		return;
	}

	private void HitSingleEntity(Entity entity)
	{
		HitTimer = 0f;
		TimesHit++;
		Projectile.velocity.Y = entity.velocity.Y;

		if (Projectile.velocity.X > 2f)
			Projectile.velocity.X = 2f;

		if (Projectile.velocity.X < -2f)
			Projectile.velocity.X = -2f;

		Projectile.velocity.X = (Projectile.velocity.X + entity.direction * 1.75f) / 2f;
		Projectile.velocity.X += entity.velocity.X * 4f;
		Projectile.velocity.Y += entity.velocity.Y;

		if (Projectile.velocity.LengthSquared() > 16f * 16f)
			Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 16f;

		Projectile.netUpdate = true;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
			Projectile.velocity.X = -oldVelocity.X;

		if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
		{
			Projectile.velocity.Y = -oldVelocity.Y * 0.5f;

			if (oldVelocity.Y > 0)
			{
				if (courtLocation is not null)
				{
					var court = SportCourts.CourtAt<VolleyballGameTracker>(courtLocation.Value);

					if (court is not null && court.Active && Projectile.Center.Y / 16 > court.bounds.Bottom - 2)
					{
						VolleyballGameTracker tracker = court.tracker as VolleyballGameTracker;
						tracker.OnScorePoint(Projectile.Center.X > court.center.X * 16);

						Projectile.Kill();
					}
				}
				else if (TimesHit > 10)
				{
					string text = $"The volley lasted {TimesHit} hits!";
					if (Main.netMode == NetmodeID.SinglePlayer)
						Main.NewText(text);
					else if (Main.netMode == NetmodeID.Server)
						ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.White);
				}
			}

			TimesHit = 0;
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		int item = Item.NewItem(Projectile.GetSource_Death(), Projectile.Center - new Vector2(0, 2), ModContent.ItemType<Volleyball>());
		Main.item[item].velocity = Vector2.Zero;

		if (courtLocation is null)
			return;

		var court = SportCourts.CourtAt<VolleyballGameTracker>(courtLocation.Value);

		if (court is not null && court.Active)
		{
			VolleyballGameTracker tracker = court.tracker as VolleyballGameTracker;
			tracker.volleyballWhoAmI = -1;
		}
	}
}