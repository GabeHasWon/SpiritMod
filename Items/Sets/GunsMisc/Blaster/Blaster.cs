using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using SpiritMod.Projectiles.Bullet.Blaster;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class Blaster : ModItem
	{
		public byte element;
		private enum ElementType : byte
		{
			Fire = 0,
			Poison = 1,
			Frost = 2,
			Plasma = 3,
			Count
		}

		public byte build;
		private enum BuildType : byte
		{
			None = 0,
			Heavy = 1,
			Laser = 2,
			Count
		}

		private byte auxillary;
		private enum AuxillaryType : byte
		{
			None = 0,
			Charge = 1,
			Boomerang = 2,
			Count
		}

		public bool usingAltTexture;

		protected override bool CloneNewInstances => true;
		public override ModItem Clone(Item itemClone)
		{
			var myClone = (Blaster)base.Clone(itemClone);
			myClone.element = element;
			myClone.build = build;
			myClone.auxillary = auxillary;
			ApplyStats();

			return myClone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Blaster");
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
			Item.shoot = ModContent.ProjectileType<EnergyBurst>();
			Item.shootSpeed = 9f;
			Item.useAmmo = AmmoID.Bullet;

			usingAltTexture = false;
		}

		public override void UseItemFrame(Player player)
		{
			if (auxillary == (int)AuxillaryType.Charge || (auxillary == (int)AuxillaryType.Boomerang && player.altFunctionUse == 2))
				return;
			int offset = (int)MathHelper.Clamp(player.itemAnimation - (Item.useAnimation - 8), 0, Item.useAnimation);
			player.itemLocation -= new Vector2(offset * player.direction, 0).RotatedBy(player.itemRotation);
		}

		public override bool AltFunctionUse(Player player) => auxillary == (int)AuxillaryType.Boomerang;
		public override bool CanUseItem(Player player)
		{
			if (auxillary == (int)AuxillaryType.Boomerang)
				Item.useAnimation = Item.useTime = (player.altFunctionUse == 2) ? 14 : 24;
			return base.CanUseItem(player);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 40f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;
			type = Item.shoot;

			if (auxillary == (int)AuxillaryType.Boomerang)
			{
				if (player.altFunctionUse == 2)
				{
					velocity = velocity.RotatedBy(-0.3f * player.direction);
					type = ModContent.ProjectileType<BoomerangBlaster>();
					Item.useStyle = ItemUseStyleID.Rapier;
				}
				else Item.useStyle = ItemUseStyleID.Shoot;
			}
			if (build == (int)BuildType.Heavy && player.altFunctionUse != 2)
				velocity = (velocity * Main.rand.NextFloat(0.75f, 1.0f)).RotatedByRandom(0.2f);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (!(auxillary == (int)AuxillaryType.Boomerang && player.altFunctionUse == 2) && auxillary != (int)AuxillaryType.Charge)
			{
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1"), position);
				FireVisuals(position, velocity, element);
			}

			Projectile proj;

			if (auxillary == (int)AuxillaryType.Charge)
			{
				proj = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<BlasterProj>(), damage, knockback, player.whoAmI);

				proj.ai[0] = type;
				proj.ai[1] = build switch
				{
					1 => ModContent.ProjectileType<LongRocket>(),
					2 => ModContent.ProjectileType<BigBeam>(),
					_ => ModContent.ProjectileType<PhaseBlast>()
				};
			}
			else if (auxillary == (int)AuxillaryType.Boomerang && player.altFunctionUse == 2)
			{
				proj = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<BoomerangBlaster>(), damage, knockback, player.whoAmI);

				proj.ai[1] = element + (usingAltTexture ? 4 : 0);
			}
			else proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);

			if (proj.ModProjectile is SubtypeProj)
				(proj.ModProjectile as SubtypeProj).Subtype = element;
			return false;
		}

		public static void FireVisuals(Vector2 position, Vector2 velocity, int visualElement)
		{
			if (Main.dedServ)
				return;
			switch (visualElement)
			{
				case (int)ElementType.Fire:
					for (int i = 0; i < 10; i++)
					{
						if (i < 3)
							ParticleHandler.SpawnParticle(new SmokeParticle(position, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(Color.DarkGray, Color.Orange, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.25f, 0.5f), 12));
						Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.Torch : DustID.Flare, null);
						dust.velocity = (velocity * Main.rand.NextFloat(0.15f, 0.3f)).RotatedByRandom(1f);
						if (dust.type == DustID.Torch)
							dust.fadeIn = 1.1f;
						dust.noGravity = true;
					}
					break;
				case (int)ElementType.Poison:
					for (int i = 0; i < 8; i++)
					{
						if (i < 3)
							ParticleHandler.SpawnParticle(new SmokeParticle(position, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(Color.Green, Color.LimeGreen, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.7f, 1.1f), 22));
						Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.FartInAJar : DustID.GreenTorch, null);
						dust.velocity = new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * .5f, Main.rand.NextFloat(-1.0f, 1.0f) * .5f);
						dust.fadeIn = 1.1f;
						dust.noGravity = true;
					}
					break;
				case (int)ElementType.Frost:
					for (int i = 0; i < 8; i++)
					{
						if (i < 3)
							ParticleHandler.SpawnParticle(new SmokeParticle(position, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(new Color(25, 236, 255), Color.White, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.5f, 1.0f), 14));
						Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.FrostHydra : DustID.GemSapphire, null);
						dust.velocity = new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * .5f, Main.rand.NextFloat(-1.0f, 1.0f) * .5f);
						dust.fadeIn = 1.1f;
						dust.noGravity = true;
					}
					break;
				case (int)ElementType.Plasma:
					for (int i = 0; i < 8; i++)
					{
						if (i == 0)
							ParticleHandler.SpawnParticle(new PulseCircle(position, Color.Lerp(Color.Magenta, Color.White, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(20f, 40f), 14)
							{
								Angle = velocity.ToRotation(),
								Velocity = velocity * Main.rand.NextFloat(0.04f, 0.08f),
								ZRotation = 0.6f
							});
						Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.Pixie : DustID.PinkTorch, null);
						dust.velocity = (velocity * Main.rand.NextFloat(0.2f, 0.8f)).RotatedByRandom(1.2f);
						dust.fadeIn = 1.1f;
						dust.noGravity = true;
					}
					break;
			}
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			frame.Width = (texture.Width / (int)ElementType.Count) - 2;
			frame.X = texture.Width / (int)ElementType.Count * element;

			int yFrame = usingAltTexture ? 1 : 0;
			frame.Height = (texture.Height / 2) - 2;
			frame.Y = texture.Height / 2 * yFrame;

			scale *= (int)BuildType.Count;
			if (frame.Width > 32 || frame.Height > 32)
				scale = (frame.Width <= frame.Height) ? (32f / (float)frame.Height) : (32f / (float)frame.Width);
			scale *= Main.inventoryScale;

			//Draw the item normally
			spriteBatch.Draw(texture, position, frame, Item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
			//Draw a glowmask
			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, position, frame, Item.GetAlpha(Color.White), 0f, origin, scale, SpriteEffects.None, 0f);
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;
			int yFrame = usingAltTexture ? 1 : 0;
			Rectangle frame = new Rectangle(TextureAssets.Item[Type].Value.Width / (int)ElementType.Count * element, TextureAssets.Item[Type].Value.Height / 2 * yFrame, 
				(TextureAssets.Item[Type].Value.Width / (int)ElementType.Count) - 2, (TextureAssets.Item[Type].Value.Height / 2) - 2);

			Vector2 position = new Vector2(Item.position.X - Main.screenPosition.X + Item.width / 2, Item.position.Y - Main.screenPosition.Y + Item.height - (frame.Height / 2));
			
			//Draw the item normally
			spriteBatch.Draw(texture, position, frame, Item.GetAlpha(lightColor), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
			//Draw a glowmask
			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, position, frame, Color.White * ((255f - Item.alpha) / 255f), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void SaveData(TagCompound tag)
		{
			tag[nameof(element)] = element;
			tag[nameof(build)] = build;
			tag[nameof(auxillary)] = auxillary;
		}

		public override void LoadData(TagCompound tag)
		{
			element = tag.Get<byte>(nameof(element));
			build = tag.Get<byte>(nameof(build));
			auxillary = tag.Get<byte>(nameof(auxillary));
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(element);
			writer.Write(build);
			writer.Write(auxillary);
		}

		public override void NetReceive(BinaryReader reader)
		{
			element = reader.ReadByte();
			build = reader.ReadByte();
			auxillary = reader.ReadByte();
			ApplyStats();
		}

		public void Generate()
		{
			element = (byte)Main.rand.Next((int)ElementType.Count);
			build = (byte)Main.rand.Next((int)BuildType.Count);
			auxillary = (byte)Main.rand.Next((int)AuxillaryType.Count);
			ApplyStats();
		}

		public void ApplyStats()
		{
			StateDefaults();
			string[] nameSelection = new string[] { "Luminous", "Ecliptic", "Aphelaic", "Cosmic", "Perihelaic", "Ionized", "Axial" };

			switch (build)
			{
				case (int)BuildType.Heavy:
					usingAltTexture = true;
					Item.shoot = ModContent.ProjectileType<MiniRocket>();
					break;
				case (int)BuildType.Laser:
					Item.shoot = ModContent.ProjectileType<Beam>();
					break;
			}

			if (auxillary == (int)AuxillaryType.Charge)
				Item.channel = true;

			Item.SetNameOverride(nameSelection[element + build] + " Blaster");
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string text = auxillary switch
			{
				1 => "Attacks can be charged for greater effectiveness",
				2 => "Right click to throw the gun out like a boomerang",
				_ => string.Empty
			};
			if (text == string.Empty)
				return;
			tooltips.Add(new TooltipLine(Mod, "Tooltip1", text));
			//ToDo: fix reforge stats flickering in the tooltip
		}

		//public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => base.ModifyWeaponDamage(player, ref damage);
	}
}