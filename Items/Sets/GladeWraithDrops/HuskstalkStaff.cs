using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Projectiles.BaseProj;
using Terraria.Localization;
using SpiritMod.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace SpiritMod.Items.Sets.GladeWraithDrops;

public class HuskstalkStaff : ModItem
{
	private int swingDir = 1;

	public override void SetStaticDefaults() => Item.staff[Item.type] = true;

	public override void SetDefaults()
	{
		Item.damage = 16;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 11;
		Item.width = 40;
		Item.height = 40;
		Item.useTime = 10;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.HiddenAnimation;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.knockBack = 6;
		Item.useTurn = false;
		Item.value = Item.sellPrice(0, 0, 15, 0);
		Item.rare = ItemRarityID.Blue;
		Item.UseSound = SoundID.Item20;
		Item.autoReuse = true;
		Item.shoot = ProjectileID.Leaf;
		Item.shootSpeed = 5.5f;
		Item.reuseDelay = 25;
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
	{
		Vector2 offset = Vector2.Normalize(velocity) * 25f;
		if (Collision.CanHit(position, 0, 0, position + offset, 0, 0))
			position += offset;

		int heldType = ModContent.ProjectileType<HuskstalkStaffHeld>();
		if (player.ownedProjectileCounts[heldType] <= 0)
			Projectile.NewProjectile(Item.GetSource_FromThis(), position, velocity, heldType, damage, knockback, player.whoAmI, ai1: swingDir);

		swingDir = -swingDir;
	}
}

public class HuskstalkStaffHeld : BaseHeldProj
{
	public ref float Counter => ref Projectile.ai[0];

	public int SwingDir { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

	private Vector2 initialVelocity;

	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.HuskstalkStaff.DisplayName");

	public override string Texture => "SpiritMod/Items/Sets/GladeWraithDrops/HuskstalkStaff";

	public override void SetDefaults()
	{
		Projectile.DamageType = DamageClass.Melee;
		Projectile.Size = new Vector2(20);
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
	}

	public override bool AutoAimCursor() => false;

	public override Vector2 HoldoutOffset() => Vector2.Normalize(Projectile.velocity) * (Projectile.Size.Length() / 2) * Projectile.scale;

	public override void AI()
	{
		bool firstTick = Projectile.timeLeft > 2;
		if (firstTick)
			initialVelocity = Projectile.velocity;

		float swingRange = .1f;
		float progress = ++Counter / Owner.itemTimeMax;
		progress = EaseFunction.EaseCircularInOut.Ease(progress);

		Projectile.velocity = initialVelocity.RotatedBy(MathHelper.Lerp(swingRange / 2 * SwingDir, -swingRange / 2 * SwingDir, progress));

		float rotation = (Owner.gravDir == -1) ? -(Projectile.velocity.ToRotation() + 1.57f) : (Projectile.velocity.ToRotation() - 1.57f);
		Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);

		Projectile.direction = Projectile.spriteDirection = Owner.direction;
		Projectile.rotation = Projectile.velocity.ToRotation();
		Owner.itemRotation = Projectile.rotation;

		Owner.heldProj = Projectile.whoAmI;
		Projectile.Center = Owner.MountedCenter + HoldoutOffset();

		Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f)) + (Vector2.Normalize(Projectile.velocity) * 20f),
			Main.rand.NextBool(4) ? DustID.OrangeStainedGlass : DustID.GrassBlades, Projectile.velocity * Main.rand.NextFloat(.5f), 150, default, Main.rand.NextFloat() + 1).noGravity = true;

		if (initialVelocity.X == Owner.direction)
			Projectile.direction = Projectile.spriteDirection *= -1;

		if (Counter < Owner.itemTimeMax)
			Projectile.timeLeft = 2;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
		Projectile.QuickDraw(null, rotation, SpriteEffects.None, Projectile.GetAlpha(lightColor));
		return false;
	}

	public override bool? CanDamage() => false;

	public override bool? CanCutTiles() => false;
}