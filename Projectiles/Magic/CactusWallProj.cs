using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic;

public class CactusWallProj : ModProjectile
{
	private const int MaxTimeLeft = 15 * 60;

	public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 3;

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 72;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.friendly = true;
		Projectile.timeLeft = MaxTimeLeft;
		Projectile.penetrate = -1;
		Projectile.frame = Main.rand.Next(3);
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;

		DrawOriginOffsetY = -30;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Tile tile = Main.tile[Projectile.Bottom.ToTileCoordinates()];

		if (tile.TopSlope)
			Projectile.position.Y += 16;
		else if (tile.IsHalfBlock)
			Projectile.position.Y += 8;

		for (int j = 0; j < 3; ++j) //This is for tripling per offset
			Dust.NewDust(Projectile.BottomLeft, 16, 1, DustID.t_Cactus);
	}

	public override void AI()
	{
		Tile tile = Framing.GetTileSafely(Projectile.Bottom.ToTileCoordinates());
		if (!WorldGen.SolidOrSlopedTile(tile) && !Main.tileSolidTop[tile.TileType])
			Projectile.Kill();
		//If no solid tiles exist below the projectile, kill it

		if (Projectile.timeLeft < 20)
			Projectile.Opacity = Projectile.timeLeft / 10f;

		Projectile.rotation = System.MathF.Sin((Projectile.timeLeft + Projectile.whoAmI) * 0.05f) * 0.05f;
	}

	public override void OnKill(int timeLeft)
	{
		if (timeLeft <= 0)
			return;
		for (int i = 0; i < 4; ++i) //This is for offsetting position
			for (int j = 0; j < 3; ++j) //This is for tripling per offset
				Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + (i * 16)), 16, 16, DustID.t_Cactus);

		if (Main.netMode != NetmodeID.Server)
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		=> modifiers.HitDirectionOverride = target.Center.X < Projectile.Center.X ? -1 : 1;

	public override bool PreDraw(ref Color lightColor)
	{
		var tex = TextureAssets.Projectile[Type].Value;
		int frameHeight = tex.Height / Main.projFrames[Type];
		var src = new Rectangle(0, frameHeight * Projectile.frame, Projectile.width, frameHeight);
		var pos = Projectile.position - Main.screenPosition + new Vector2(0, frameHeight - 4);

		const int SpawnAnimSpeed = 10;

		if (Projectile.timeLeft - MaxTimeLeft > -SpawnAnimSpeed)
		{
			src.Height = (int)(System.Math.Abs(Projectile.timeLeft - MaxTimeLeft) / (float)SpawnAnimSpeed * frameHeight);
			pos.Y += frameHeight - src.Height;
		}

		Main.EntitySpriteDraw(tex, pos, src, Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(0, frameHeight), 1f, SpriteEffects.None, 0);
		return false;
	}
}
