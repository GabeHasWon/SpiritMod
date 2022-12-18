using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Bullet.Blaster;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class Exotic : ModItem
	{
		public byte style;
		private enum StyleType : byte
		{
			Festive = 0,
			Golden = 1,
			Bleak = 2,
			Count
		}

		protected override bool CloneNewInstances => true;
		public override ModItem Clone(Item itemClone)
		{
			var myClone = (Exotic)base.Clone(itemClone);
			myClone.style = style;
			ApplyStats();

			return myClone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Exotic Blaster");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			StateDefaults();
			Generate();
		}

		private void StateDefaults()
		{
			Item.DamageType = DamageClass.Ranged;
			Item.width = 46;
			Item.height = 30;
			Item.damage = 13;
			Item.useTime = 27;
			Item.useAnimation = 27;
			Item.reuseDelay = 0;
			Item.knockBack = 2.5f;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.rare = ItemRarityID.Green;
			Item.channel = false;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Bauble>();
			Item.shootSpeed = 9f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override void UseItemFrame(Player player)
		{
			int offset = (int)MathHelper.Clamp(player.itemAnimation - (Item.useAnimation - 8), 0, Item.useAnimation);
			player.itemLocation -= new Vector2(offset * player.direction, 0).RotatedBy(player.itemRotation);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 40f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;
			type = Item.shoot;
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			frame.Height = (texture.Height / (int)StyleType.Count) - 2;
			frame.Y = texture.Height / (int)StyleType.Count * style;

			scale *= (int)StyleType.Count;
			if (frame.Width > 32 || frame.Height > 32)
				scale = (frame.Width <= frame.Height) ? (32f / (float)frame.Height) : (32f / (float)frame.Width);
			scale *= Main.inventoryScale;

			position += new Vector2(-(frame.Width / 2), frame.Height) * scale / 2;

			//Draw the item normally
			spriteBatch.Draw(texture, position, frame, Item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
			//Draw a glowmask
			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, position, frame, Item.GetAlpha(Color.White), 0f, origin, scale, SpriteEffects.None, 0f);
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;
			Rectangle frame = new Rectangle(0, texture.Height / (int)StyleType.Count * style, texture.Width, (texture.Height / (int)StyleType.Count) - 2);

			Vector2 position = new Vector2(Item.position.X - Main.screenPosition.X + Item.width / 2, Item.position.Y - Main.screenPosition.Y + Item.height - (frame.Height / 2));

			//Manage fancy drop visuals
			#region extraVFX
			float quoteant = 1f - (float)MathHelper.Clamp(Item.timeSinceItemSpawned / 240f, 0f, 1f);

			if (quoteant > 0f)
			{
				float time = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3) * quoteant;

				Texture2D outline = ModContent.Request<Texture2D>(Texture + "_Outline").Value;
				for (int i = 0; i < 2; i++)
				{
					Color color = (i < 1) ? Color.White * time * .75f : Main.DiscoColor * time * .25f;
					spriteBatch.Draw(outline, position - new Vector2(2), new Rectangle(0, outline.Height / (int)StyleType.Count * style, outline.Width, (outline.Height / (int)StyleType.Count) - 2),
						Item.GetAlpha(color), rotation, frame.Size() / 2, MathHelper.Lerp(0.5f, 1.0f, time / 2 + 0.5f) * scale, SpriteEffects.None, 0f);
				}

				if (Main.rand.NextBool(10))
				{
					Dust dust = Dust.NewDustDirect(Item.position, frame.Width, frame.Height, DustID.SilverCoin);
					dust.noGravity = true;
					dust.fadeIn = .5f;
					dust.velocity = Vector2.Zero;
				}
			}
			#endregion

			//Draw the item normally
			spriteBatch.Draw(texture, position, frame, Item.GetAlpha(lightColor), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
			//Draw a glowmask
			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, position, frame, Color.White * ((255f - Item.alpha) / 255f), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void SaveData(TagCompound tag) => tag[nameof(style)] = style;
		public override void LoadData(TagCompound tag) => style = tag.Get<byte>(nameof(style));

		public override void NetSend(BinaryWriter writer) => writer.Write(style);
		public override void NetReceive(BinaryReader reader)
		{
			style = reader.ReadByte();
			ApplyStats();
		}

		public void Generate()
		{
			style = (byte)Main.rand.Next((int)StyleType.Count);
			ApplyStats();
		}

		public void ApplyStats()
		{
			StateDefaults();
			string[] nameSelection = new string[] { "Bulb", "Golden", "Bleak" };

			Item.shoot = style switch
			{
				1 => ModContent.ProjectileType<GoldBullet>(),
				2 => ModContent.ProjectileType<EnergyBurst>(),
				_ => Item.shoot
			};

			Item.SetNameOverride(nameSelection[style] + " Blaster");
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
		}

		//public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => base.ModifyWeaponDamage(player, ref damage);
	}
}