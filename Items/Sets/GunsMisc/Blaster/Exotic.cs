using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Particles;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;
using SpiritMod.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
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
			Swift = 2,
			Starplate = 3,
			Count
		}

		protected override bool CloneNewInstances => true;
		public override ModItem Clone(Item itemClone)
		{
			var myClone = (Exotic)base.Clone(itemClone);
			myClone.style = style;

			myClone.ApplyStats();

			return myClone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Exotic Blaster");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
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
			Item.shoot = ModContent.ProjectileType<EnergyBurst>();
			Item.shootSpeed = 9f;
			Item.useAmmo = AmmoID.Bullet;

			Generate();
		}

		public override void UseItemFrame(Player player)
		{
			//Shot feedback
			if (style == (int)StyleType.Starplate)
			{
				int offset = (int)MathHelper.Clamp(player.itemAnimation - (player.itemAnimationMax - 12), 0, player.itemAnimationMax);

				player.itemRotation = player.itemRotation - (offset * 0.001f * player.direction) + (12 * 0.001f * player.direction);
				player.itemLocation -= new Vector2(offset * player.direction, 0).RotatedBy(player.itemRotation);
			}
			else
			{
				int offset = (int)MathHelper.Clamp(player.itemAnimation - (player.itemAnimationMax - 8), 0, player.itemAnimationMax);
				player.itemLocation -= new Vector2(offset * player.direction, 0).RotatedBy(player.itemRotation);
			}
		}

		public override bool AltFunctionUse(Player player) => style == (int)StyleType.Golden;
		public override bool CanUseItem(Player player)
		{
			if (style != (int)StyleType.Golden)
				return base.CanUseItem(player);

			if (player.altFunctionUse == 2)
			{
				Projectile.NewProjectile(Entity.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<GoldBlasterProj>(), (int)player.GetDamage(DamageClass.Ranged).ApplyTo(Item.damage), Item.knockBack, player.whoAmI);
				Item.useStyle = ItemUseStyleID.Rapier;
				SoundEngine.PlaySound(SoundID.Item149, player.Center);
			}
			else
				Item.useStyle = ItemUseStyleID.Shoot;

			return player.altFunctionUse != 2;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 2)) * 40f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;
			type = Item.shoot;

			if (style == (int)StyleType.Swift)
			{
				if (!Main.dedServ)
				{
					ParticleHandler.SpawnParticle(new BlasterFlash(position + (muzzleOffset / 2.5f), 1, velocity.ToRotation()));
					for (int i = 0; i < 3; i++)
						ParticleHandler.SpawnParticle(new FireParticle(position, (velocity * Main.rand.NextFloat(0.1f, 0.8f)).RotatedByRandom(0.8f), Color.White, Color.Red, Main.rand.NextFloat(0.2f, 0.5f), 12));
				}
				if (Main.netMode != NetmodeID.Server)
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1") with { Volume = 0.5f, PitchVariance = 0.2f, MaxInstances = 3 }, position);
			}
			else
			{
				if (style == (int)StyleType.Festive)
				{
					for (int i = 0; i < 4; i++)
						Dust.NewDustPerfect(position, DustID.Confetti, (velocity * Main.rand.NextFloat(0.4f, 0.8f)).RotatedByRandom(1f), 100);
				}

				if (style == (int)StyleType.Starplate)
				{
					if (player.ownedProjectileCounts[ModContent.ProjectileType<StarplateHologram>()] < 1)
						Projectile.NewProjectile(Entity.GetSource_ItemUse(Item), position, Vector2.Zero, ModContent.ProjectileType<StarplateHologram>(), damage, knockback, player.whoAmI);
				}
				else if (Main.netMode != NetmodeID.Server)
				{
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1"), position);
				}
			}
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

			position += new Vector2(-(frame.Width / 2), frame.Height / 2) * scale / 2;

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
			float quoteant = 1f - (float)MathHelper.Clamp(Item.timeSinceItemSpawned / 300f, 0f, 1f);

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
		public override void LoadData(TagCompound tag)
		{
			style = tag.Get<byte>(nameof(style));
			ApplyStats();
		}

		public override void NetSend(BinaryWriter writer) => writer.Write(style);
		public override void NetReceive(BinaryReader reader)
		{
			style = reader.ReadByte();
			ApplyStats();
		}

		public void Generate()
		{
			List<byte> list = new List<byte> { (byte)StyleType.Golden, (byte)StyleType.Swift, (byte)StyleType.Starplate };
			if (Main.xMas || Main.snowMoon)
				list.Add((byte)StyleType.Festive); //This weapon is a seasonal exclusive

			style = Main.rand.NextFromCollection(list);

			ApplyStats();
		}

		public void ApplyStats()
		{
			string[] nameSelection = new string[] { "Bulb", "Golden", "Swift", "Starplate" };

			Item.shoot = style switch
			{
				0 => Item.shoot = ModContent.ProjectileType<Bauble>(),
				1 => Item.shoot = ModContent.ProjectileType<GoldBullet>(),
				2 => Item.shoot = ModContent.ProjectileType<EnergyBurst>(),
				_ => Item.shoot = ModContent.ProjectileType<StarshotOrange>()
			};
			Item.channel = style == (int)StyleType.Starplate;

			Item.SetNameOverride(nameSelection[style] + " Blaster");
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string text = style switch
			{
				1 => "Struck enemies are marked for damage\nRight click to detonate all marked enemies\nAll marks instantly expire when missing a shot",
				2 => "Fires a rapid volley of energy",
				3 => "Creates a hologram to supply additional fire",
				_ => "Launches bouncing glass baubles...ouch!"
			};
			if (text == string.Empty)
				return;

			tooltips.Insert(5, new TooltipLine(Mod, string.Empty, text));
		}

		public override float UseSpeedMultiplier(Player player)
		{
			if (style == (int)StyleType.Swift)
				return 2f;
			return base.UseSpeedMultiplier(player);
		}
	}
}