using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.ByBiome.Ocean.Misc;

[Sacrifice(1)]
internal class Volleyball : ModItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 26;
		Item.rare = ItemRarityID.White;
		Item.value = Item.sellPrice(copper: 20);
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
}

internal class VolleyballProjectile : ModProjectile
{
	public override string Texture => base.Texture.Replace("Projectile", "");

	private int _waitTime = 0; //It didn't expire like Beach Balls do so this is a workaround ig

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.BeachBall);
		Projectile.width = Projectile.height = 26;
		Projectile.hostile = false;
		Projectile.friendly = false;
	}

	public override void AI()
	{
		_waitTime++;

		if (Projectile.velocity.LengthSquared() > 0.09f)
			_waitTime = 0;

		if (_waitTime > 2 * 60)
			Projectile.Kill();
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
			Projectile.velocity.X = -oldVelocity.X;

		if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
		{
			Projectile.velocity.Y = -oldVelocity.Y * 0.5f;

			if (Projectile.ai[1] > 10)
			{
				string text = $"The volley lasted {Projectile.ai[1]} hits!";
				if (Main.netMode == NetmodeID.SinglePlayer)
					Main.NewText(text);
				else if (Main.netMode == NetmodeID.Server)
					ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.White);
			}	

			Projectile.ai[1] = 0;
		}
		return false;
	}

	public override void Kill(int timeLeft)
	{
		int item = Item.NewItem(Projectile.GetSource_Death(), Projectile.Center - new Vector2(0, 2), ModContent.ItemType<Volleyball>());
		Main.item[item].velocity = Vector2.Zero;
	}
}