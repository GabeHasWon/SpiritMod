using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Summon.LaserGate;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Summon
{
	public class GateStaff : ModItem
	{
		private static int Hopper => ModContent.ProjectileType<Hopper>();

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gate Staff");
			Tooltip.SetDefault("Summons an electric field\nRight click to remove summoned fields");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 14;
			Item.DamageType = DamageClass.Summon;
			Item.mana = 16;
			Item.width = 44;
			Item.height = 48;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 5;
			Item.value = 20000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<Hopper>();
			Item.shootSpeed = 0f;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				foreach (Projectile proj in Main.projectile.Where(x => x.active && x.owner == player.whoAmI && x.type == Hopper))
					proj.Kill();

				return false;
			}
			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			float[] scanarray = new float[3];
			float dist = player.Distance(Main.MouseWorld);
			Collision.LaserScan(player.Center, player.DirectionTo(Main.MouseWorld), 0, dist, scanarray);
			dist = 0;

			foreach (float array in scanarray)
				dist += array / scanarray.Length;

			Vector2 spawnpos = player.Center + player.DirectionTo(Main.MouseWorld) * dist;
			int projCounts = player.ownedProjectileCounts[Hopper];

			Projectile.NewProjectileDirect(source, spawnpos, Vector2.Zero, Hopper, damage, knockback, player.whoAmI, projCounts);

			int hoppersMax = 2;
			if (projCounts >= hoppersMax)
			{
				foreach (Projectile proj in Main.projectile.Where(x => x.active && x.owner == player.whoAmI && x.type == Hopper))
				{
					if (proj.ai[0] == 0)
						proj.Kill();
					else
						proj.ai[0]--; //Readjust all entries
				}
			}

			return false;
		}
	}
}