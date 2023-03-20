using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;
using SpiritMod.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using static SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles.SubtypeProj.Subtypes;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class Blaster : ModItem
	{
		public byte element;
		//This enum is stored in SubtypeProj for wider use

		public byte build;
		private enum BuildType : byte
		{
			None = 0,
			Laser = 1,
			Heavy = 2,
			Wave = 3,
			Count
		}

		private byte auxillary;
		private enum AuxillaryType : byte
		{
			None = 0,
			Charge = 1,
			Boomerang = 2,
			Burst = 3,
			Count
		}

		protected override bool CloneNewInstances => true;
		public override ModItem Clone(Item itemClone)
		{
			var myClone = (Blaster)base.Clone(itemClone);
			myClone.element = element;
			myClone.build = build;
			myClone.auxillary = auxillary;

			myClone.ApplyStats();

			return myClone;
		}

		public override void SetStaticDefaults() => DisplayName.SetDefault("Cosmic Blaster");

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
			int offset = (int)MathHelper.Clamp(player.itemTime - (float)(player.itemTimeMax / 3f), 0, 10);
			player.itemLocation -= new Vector2(offset * player.direction, 0).RotatedBy(player.itemRotation);
		}

		public override bool AltFunctionUse(Player player) => auxillary == (int)AuxillaryType.Boomerang || auxillary == (int)AuxillaryType.Charge;
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
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (!((auxillary == (int)AuxillaryType.Boomerang || auxillary == (int)AuxillaryType.Charge) && player.altFunctionUse == 2))
			{
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MaliwanShot1") with { MaxInstances = 3 }, position);
				FireVisuals(position, velocity, element);
			}

			Projectile proj;
			int ai1 = 0;

			if (player.altFunctionUse == 2)
			{
				if (auxillary == (int)AuxillaryType.Charge)
				{
					//Reduce the damage of the laser due to its piercing and multi-hit abilities
					damage = (int)(damage * ((build == (int)BuildType.Laser) ? 0.8f : 2.5f));
					velocity *= 1.5f;
					type = ModContent.ProjectileType<BlasterProj>();

					ai1 = build switch
					{
						1 => ModContent.ProjectileType<PiercingBeam>(),
						2 => ModContent.ProjectileType<LongRocket>(),
						_ => ModContent.ProjectileType<PhaseBlast>()
					};
				}
				if (auxillary == (int)AuxillaryType.Boomerang)
				{
					velocity = velocity.RotatedBy(-0.3f * player.direction);
					type = ModContent.ProjectileType<BoomerangBlaster>();
					Item.useStyle = ItemUseStyleID.Rapier;

					ai1 = build;
				}
			}
			else
			{
				if (build == (int)BuildType.Wave)
				{
					if (auxillary == (int)AuxillaryType.Burst)
						velocity = (velocity * Main.rand.NextFloat(0.7f, 1.0f)).RotatedByRandom(0.15f);
					else
					{
						for (int i = 0; i < 3; i++)
						{
							Vector2 newVel = (velocity * Main.rand.NextFloat(0.5f, 0.8f)).RotatedByRandom(1f);
							Projectile starshot = Projectile.NewProjectileDirect(source, position, newVel, type, damage, knockback, player.whoAmI);
							(starshot.ModProjectile as SubtypeProj).Subtype = element;
							starshot.frame = 1;
						}
					}
				}
				else if (build == (int)BuildType.Heavy)
					velocity = (velocity * Main.rand.NextFloat(0.75f, 1.0f)).RotatedByRandom(0.2f);

				if (auxillary == (int)AuxillaryType.Boomerang)
					Item.useStyle = ItemUseStyleID.Shoot;
			}

			proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);

			if (proj.ModProjectile is SubtypeProj)
				(proj.ModProjectile as SubtypeProj).Subtype = element;
			proj.ai[1] = ai1;
			proj.netUpdate = true;

			return false;
		}

		public static void FireVisuals(Vector2 position, Vector2 velocity, int visualElement)
		{
			if (Main.dedServ)
				return;
			switch (visualElement)
			{
				case (int)Fire:
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
				case (int)Poison:
					for (int i = 0; i < 8; i++)
					{
						if (i < 3)
							ParticleHandler.SpawnParticle(new SmokeParticle(position, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(Color.White, Color.LimeGreen, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.25f, 0.5f), 12));
						Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.FartInAJar : DustID.GreenTorch, null);
						dust.velocity = (velocity * Main.rand.NextFloat(0.15f, 0.3f)).RotatedByRandom(1f);
						if (dust.type == DustID.GreenTorch)
							dust.fadeIn = 1.1f;
						dust.noGravity = true;
					}
					break;
				case (int)Frost:
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
				case (int)Plasma:
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

			frame.Height = (texture.Height / (int)BuildType.Count) - 2;
			frame.Y = texture.Height / (int)BuildType.Count * build;

			scale *= (int)BuildType.Count;
			if (frame.Width > 32 || frame.Height > 32)
				scale = (frame.Width <= frame.Height) ? (32f / (float)frame.Height) : (32f / (float)frame.Width);
			scale *= Main.inventoryScale;

			position += new Vector2(-(frame.Width / 2) - 2, (frame.Height / 2) - 2) * scale / 2;

			//Draw the item normally
			spriteBatch.Draw(texture, position, frame, Item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
			//Draw a glowmask
			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, position, frame, Item.GetAlpha(SubtypeProj.GetColor(element)), 0f, origin, scale, SpriteEffects.None, 0f);
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;
			Rectangle frame = new Rectangle(0, texture.Height / (int)BuildType.Count * build, texture.Width, (texture.Height / (int)BuildType.Count) - 2);

			Vector2 position = new Vector2(Item.position.X - Main.screenPosition.X + Item.width / 2, Item.position.Y - Main.screenPosition.Y + Item.height / 2);

			//Draw the item normally
			spriteBatch.Draw(texture, position, frame, Item.GetAlpha(lightColor), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
			//Draw a glowmask
			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, position, frame, SubtypeProj.GetColor(element).MultiplyRGBA(Color.White) * ((255f - Item.alpha) / 255f), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
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

			ApplyStats();
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
			element = (byte)Main.rand.Next((int)Count);
			build = (byte)Main.rand.Next((int)BuildType.Count);
			auxillary = (byte)Main.rand.Next((int)AuxillaryType.Count);

			ApplyStats();
		}

		public void ApplyStats()
		{
			string[] nameSelection = new string[] { "Luminous", "Ecliptic", "Aphelaic", "Cosmic", "Perihelaic", "Ionized", "Axial" };

			Item.shoot = build switch
			{
				1 => Item.shoot = ModContent.ProjectileType<EnergyBeam>(),
				2 => Item.shoot = ModContent.ProjectileType<MiniRocket>(),
				3 => Item.shoot = ModContent.ProjectileType<Starshot>(),
				_ => Item.shoot = ModContent.ProjectileType<EnergyBurst>()
			};

			Item.channel = auxillary == (int)AuxillaryType.Charge;
			Item.reuseDelay = (auxillary == (int)AuxillaryType.Burst) ? 60 : 0;

			Item.SetNameOverride(nameSelection[element + build] + " Blaster");
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string[] text = new string[]
			{
				auxillary switch
				{
					1 => "Right click to fire an empowered shot",
					2 => "Right click to throw the gun out like a boomerang",
					3 => "Fires in bursts",
					_ => string.Empty
				},
				element switch
				{
					0 => "Attacks ignite enemies",
					1 => "Attacks poison enemies",
					2 => "Attacks inflict Frostburn",
					_ => string.Empty,
				}
			};

			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != string.Empty)
					tooltips.Add(new TooltipLine(Mod, string.Empty, text[i]));
			}
		}

		public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
		{
			if (element == (int)Plasma)
				damage.ApplyTo(7);
		}

		public override float UseSpeedMultiplier(Player player)
		{
			if (auxillary == (int)AuxillaryType.Burst)
				return 1.5f;
			else if (build == (int)BuildType.Wave)
				return 1.8f;
			return base.UseSpeedMultiplier(player);
		}
	}
}