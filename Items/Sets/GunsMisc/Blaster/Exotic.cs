using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Effects;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;
using SpiritMod.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.PlayerDrawLayer;

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
			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(2, (int)StyleType.Count) { NotActuallyAnimating = true });
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

		public override Vector2? HoldoutOffset() => Vector2.UnitX * -2f;

		public override void UseItemFrame(Player player)
		{
			if (style == (int)StyleType.Golden)
			{
				float amount = .5f;

				if (player.ItemAnimationJustStarted)
					player.itemRotation += (float)player.itemAnimation / player.itemAnimationMax * -player.direction * amount;

				player.itemRotation -= amount / (float)player.itemAnimationMax * -player.direction;
			}

			//Shot feedback
			int offset = (int)MathHelper.Clamp(player.itemAnimation - (player.itemAnimationMax - 8), 0, player.itemAnimationMax);
			player.itemLocation -= new Vector2(offset * player.direction, 0).RotatedBy(player.itemRotation);
		}

		public override bool AltFunctionUse(Player player) => style == (int)StyleType.Golden;

		public override bool? UseItem(Player player)
		{
			if (Main.netMode == NetmodeID.Server)
				return base.UseItem(player);

			if (style == (int)StyleType.Festive)
			{
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1") with { PitchVariance = 0.3f }, player.Center);
			}
			if (style == (int)StyleType.Golden)
			{
				if (player.altFunctionUse == 2)
				{
					Projectile.NewProjectile(Entity.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<GoldBlasterProj>(), (int)player.GetDamage(DamageClass.Ranged).ApplyTo(Item.damage), Item.knockBack, player.whoAmI);
					SoundEngine.PlaySound(SoundID.Item149, player.Center);

					player.GetModPlayer<BlasterPlayer>().hide = true;
					return false;
				}
				else
				{
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1") with { PitchVariance = 0.3f }, player.Center);
				}
			}
			if (style == (int)StyleType.Starplate)
			{
				player.GetModPlayer<BlasterPlayer>().hide = true;
			}
			if (style == (int)StyleType.Swift)
			{
				SoundEngine.PlaySound(SoundID.Item41 with { Volume = 0.7f, PitchVariance = 0.5f, MaxInstances = 3 }, player.Center);
				SoundEngine.PlaySound(SoundID.Item45 with { Volume = 0.7f, PitchVariance = 0.5f, MaxInstances = 3 }, player.Center);
			}

			return base.UseItem(player);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			type = Item.shoot;

			bool muzzleFlare = style == (int)StyleType.Swift || (style == (int)StyleType.Golden && player.altFunctionUse != 2);

			if (style == (int)StyleType.Festive)
			{
				for (int i = 0; i < 4; i++)
					Dust.NewDustPerfect(position, DustID.Confetti, (velocity * Main.rand.NextFloat(0.4f, 0.8f)).RotatedByRandom(1f), 100);
			}
			if (style == (int)StyleType.Starplate)
			{
				if (player.ownedProjectileCounts[ModContent.ProjectileType<StarplateHologram>()] < 2)
					for (int i = 0; i < 2; i++)
					{
						Projectile proj = Projectile.NewProjectileDirect(Entity.GetSource_ItemUse(Item), position, Vector2.Zero, ModContent.ProjectileType<StarplateHologram>(), damage, knockback, player.whoAmI);
						proj.frame = i;
					}
			}
			if (style == (int)StyleType.Swift)
			{
				velocity = velocity.RotatedByRandom(0.25f);
			}

			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 2)) * 40f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			if (muzzleFlare && !Main.dedServ)
			{
				ParticleHandler.SpawnParticle(new BlasterFlash(position + (muzzleOffset / 3.5f), 1, velocity.ToRotation()));
				for (int i = 0; i < 3; i++)
					ParticleHandler.SpawnParticle(new FireParticle(position, (velocity * Main.rand.NextFloat(0.1f, 0.8f)).RotatedByRandom(0.8f), Color.White, Color.Red, Main.rand.NextFloat(0.1f, 0.3f), 12));
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => style != (int)StyleType.Starplate && !(style == (int)StyleType.Golden && player.altFunctionUse == 2);

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			frame.Y = texture.Height / (int)StyleType.Count * style;
			frame.Height -= 2;

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
				2 => Item.shoot = ModContent.ProjectileType<SwiftShot>(),
				_ => Item.shoot = ModContent.ProjectileType<HoloShot>()
			};
			Item.channel = style == (int)StyleType.Starplate;

			Item.SetNameOverride(nameSelection[style] + " Blaster");
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string text = style switch
			{
				1 => "Struck enemies are marked for damage\nRight click to detonate all marked enemies\nAll marks instantly expire after missing a shot",
				2 => "Fires a rapid volley of energy",
				3 => "Creates a hologram to supply additional fire",
				_ => "Launches bouncing glass baubles...ouch!"
			};
			if (text == string.Empty)
				return;

			tooltips.Add(new TooltipLine(Mod, string.Empty, text));
		}

		public override float UseSpeedMultiplier(Player player)
		{
			if (style == (int)StyleType.Swift)
				return 3f;
			return base.UseSpeedMultiplier(player);
		}
	}
}