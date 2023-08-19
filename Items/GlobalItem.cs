using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Halloween.DevMasks;
using SpiritMod.NPCs.Critters;
using SpiritMod.NPCs.Reach;
using SpiritMod.NPCs.BlueMoon.LunarSlime;
using SpiritMod.NPCs.OceanSlime;
using SpiritMod.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace SpiritMod.Items
{
	public class GItem : GlobalItem
	{
		public override bool InstancePerEntity => true;
		protected override bool CloneNewInstances => true;

		public override void SetDefaults(Item item)
		{
			if (item.type == ItemID.ArmoredCavefish)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<Cavefish>();
			}
			if (item.type == ItemID.Damselfish)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<Damselfish>();
			}
			if (item.type == ItemID.CrimsonTigerfish)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<CrismonTigerfish>();
			}
			if (item.type == ItemID.GoldenCarp)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<GoldenCarp>();
			}
			if (item.type == ItemID.SpecularFish)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<SpecularFish>();
			}
			if (item.type == ItemID.Prismite)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<Prismite>();
			}
			if (item.type == ItemID.Ebonkoi)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<Ebonkoi>();
			}
			if (item.type == ItemID.NeonTetra)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<NeonTetra>();
			}
			if (item.type == ItemID.AtlanticCod)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<AtlanticCod>();
			}
			if (item.type == ItemID.FrostMinnow)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<FrostMinnow>();
			}
			if (item.type == ItemID.RedSnapper)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<RedSnapper>();
			}
			if (item.type == ItemID.VariegatedLardfish)
			{
				item.useStyle = ItemUseStyleID.Swing;
				item.useTime = item.useAnimation = 20;
				item.noMelee = true;
				item.consumable = true;
				item.autoReuse = true;
				item.makeNPC = ModContent.NPCType<Lardfish>();
			}
		}

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			if (item.type == ItemID.RoyalGel)
			{
				player.npcTypeNoAggro[ModContent.NPCType<LunarSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<ReachSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<NPCs.GraniteSlime.GraniteSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<NPCs.DiseasedSlime.DiseasedSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<OceanSlime>()] = true;
			}
		}

		public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
		{
			if (item.type != ItemID.GoodieBag)
				return;

			itemLoot.Add(ItemDropRule.OneFromOptions(1, ItemUtils.DropCandyTable()));

			if (Main.rand.NextBool(3))
			{
				int[] lootTable = {
						ModContent.ItemType<MaskSchmo>(),
						ModContent.ItemType<MaskGraydee>(),
						ModContent.ItemType<MaskLordCake>(),
						ModContent.ItemType<MaskVladimier>(),
						ModContent.ItemType<MaskKachow>(),
						ModContent.ItemType<MaskBlaze>(),
						ModContent.ItemType<MaskSvante>(),
						ModContent.ItemType<MaskIggy>(),
						ModContent.ItemType<MaskYuyutsu>(),
						ModContent.ItemType<MaskLeemyy>()
					};
				itemLoot.Add(ItemDropRule.OneFromOptions(3, lootTable));
			}
		}

		public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();
			if (modPlayer.talonSet && (item.IsRanged() || item.IsMagic()) && Main.rand.NextBool(10))
			{
				Projectile proj = Projectile.NewProjectileDirect(source, position, new Vector2(velocity.X, velocity.Y + 2), ProjectileID.HarpyFeather, 10, 2f, player.whoAmI);
				proj.hostile = false;
				proj.friendly = true;
				proj.netUpdate = true;
			}
			if (modPlayer.cultistScarf && item.IsMagic() && Main.rand.NextBool(8))
			{
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<WildMagic>(), 66, 2f, player.whoAmI);
			}
			if (modPlayer.timScroll && item.IsMagic() && Main.rand.NextBool(12))
			{
				int[] projTypes = new int[] { ProjectileID.Starfury, ProjectileID.PurificationPowder, ProjectileID.FallingStar, ProjectileID.Bullet, ProjectileID.BallofFire, ProjectileID.MagicMissile, ProjectileID.Flamarang, ProjectileID.GreenLaser, ProjectileID.WaterStream, ProjectileID.WaterBolt, ProjectileID.ThornChakram, ProjectileID.Flamelash, ProjectileID.DemonScythe, ProjectileID.Stinger, ProjectileID.EnchantedBeam };

				Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projTypes[Main.rand.Next(projTypes.Length)], damage, knockback, player.whoAmI);
				proj.friendly = true;
				proj.hostile = false;
				proj.netUpdate = true;
			}
			if (modPlayer.runeWizardScroll && item.IsMagic() && Main.rand.NextBool(12))
			{
				int[] projTypes = new int[] { ProjectileID.SwordBeam, ProjectileID.TopazBolt, ProjectileID.EmeraldBolt, ProjectileID.TerraBeam, ProjectileID.NightBeam, ProjectileID.ChlorophyteOrb, ProjectileID.EyeFire, ProjectileID.HeatRay, ProjectileID.IchorBullet };

				Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projTypes[Main.rand.Next(projTypes.Length)], damage, knockback, player.whoAmI);
				proj.friendly = true;
				proj.hostile = false;
				proj.netUpdate = true;
			}
			if (modPlayer.fireMaw && item.IsMagic() && Main.rand.NextBool(4))
			{
				Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<FireMaw>(), 30, 2f, player.whoAmI);
				proj.hostile = false;
				proj.friendly = true;
				proj.netUpdate = true;
			}
			if (modPlayer.manaWings && item.IsMagic() && Main.rand.NextBool(7))
			{
				float d1 = 20 + ((player.statManaMax2 - player.statMana) / 3);
				Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<ManaSpark>(), (int)d1, 2f, player.whoAmI);
				proj.hostile = false;
				proj.friendly = true;
				proj.netUpdate = true;
			}
			return true;
		}

		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			int[] metalItems = new int[] { ItemID.IronOre, ItemID.CopperOre, ItemID.SilverOre, ItemID.GoldOre, ItemID.TinOre, ItemID.LeadOre, ItemID.TungstenOre, ItemID.PlatinumOre, ItemID.Meteorite, ItemID.LunarOre, ItemID.ChlorophyteOre,
				ItemID.DemoniteOre, ItemID.CrimtaneOre, ItemID.Obsidian, ItemID.Hellstone, ItemID.CobaltOre, ItemID.PalladiumOre, ItemID.MythrilOre, ItemID.OrichalcumOre, ItemID.AdamantiteOre, ItemID.TitaniumOre };

			if (player.GetModPlayer<MyPlayer>().metalBand)
				if (metalItems.Contains(item.type))
					grabRange *= 10;

			if (player.GetModPlayer<MyPlayer>().teslaCoil)
				if (metalItems.Contains(item.type))
					grabRange *= 15;
		}

		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Color glowColor = new Color(250, 250, 250, item.alpha);

			if (item.ModItem is IGlowing glow)
			{
				Texture2D texture = glow.Glowmask(out float bias);
				Color alpha = Color.Lerp(alphaColor, glowColor, bias);
				Vector2 origin = new Vector2(texture.Width >> 1, texture.Height >> 1);
				Vector2 position = item.position - Main.screenPosition;
				position.X += item.width >> 1;
				position.Y += item.height - (texture.Height >> 1);
				spriteBatch.Draw(texture, position, null, alpha, rotation, origin, scale, SpriteEffects.None, 0f);
			}
		}

		/// <summary>Directly uses ammo given an index.</summary>
		/// <param name="p">Player to use.</param>
		/// <param name="index">Index of the item in player.inventory.</param>
		public static void UseAmmoDirect(Player p, int index)
		{
			if (p.inventory[index].consumable && VanillaAmmoConsumption(p, p.inventory[index].ammo)) //Do not consume ammo if possible
			{
				p.inventory[index].stack--;
				if (p.inventory[index].stack <= 0)
					p.inventory[index].TurnToAir();
			}
		}

		public static bool VanillaAmmoConsumption(Player p, int ammo)
		{
			float chance = 0;

			static float CombineChances(float p1, float p2) => p1 + p2 - (p1 * p2);

			if (p.ammoBox) //1/5 chance to reduce
				chance = 0.2f;

			if (p.ammoCost75) //1/4 chance to reduce
				chance = CombineChances(chance, 0.25f);

			if (p.ammoCost80) //1/5 chance to reduce
				chance = CombineChances(chance, 0.2f);

			if (p.ammoPotion) //1/5 chance to reduce
				chance = CombineChances(chance, 0.2f);

			if (ammo == AmmoID.Arrow && p.magicQuiver) //1/5 chance to reduce for arrows only
				chance = CombineChances(chance, 0.2f);

			if (p.armor[0].type == ItemID.ChlorophyteHelmet) //1/5 chance to reduce -- seems unique??
				chance = CombineChances(chance, 0.2f);

			return Main.rand.NextFloat(1f) > chance;
		}

		/*/// <summary>Aims the player's arms to the given radians. If no player is passed, assumes <see cref="Main.LocalPlayer"/>.</summary>
		/// <param name="radians"></param>
		/// <param name="p"></param>
		public static void ArmsTowardsMouse(Player p = null, Vector2? targetLoc = null)
		{
			p ??= Main.LocalPlayer;

			if (targetLoc == null)
				targetLoc = Main.MouseWorld;

			float radians = p.AngleTo(targetLoc.Value) + MathHelper.PiOver2;
			radians = Math.Abs(radians);

			int FrameSize = 56;
			bool WithinAngle(float target) => Math.Abs(target - radians) < MathHelper.PiOver4;

			if (WithinAngle(MathHelper.PiOver4) || radians < MathHelper.PiOver4)
				p.bodyFrame.Y = FrameSize * 2;
			else if (WithinAngle(MathHelper.PiOver2))
				p.bodyFrame.Y = FrameSize * 3;
			else if (WithinAngle(MathHelper.PiOver4 * 3))
				p.bodyFrame.Y = FrameSize * 4;
			else if (WithinAngle(MathHelper.PiOver2 * 3))
				p.bodyFrame.Y = FrameSize * 3;
		}*/
	}
}