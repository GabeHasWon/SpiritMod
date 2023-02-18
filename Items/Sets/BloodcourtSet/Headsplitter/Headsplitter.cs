using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using SpiritMod.Mechanics.CooldownItem;
using SpiritMod.NPCs.Boss.Occultist.Particles;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BloodcourtSet.Headsplitter
{
	public class Headsplitter : ModItem, ICooldownItem
	{
		private bool reverseSwing = false;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Headsplitter");
			Tooltip.SetDefault("Right click to release an explosion of vengeance, effective on anguished foes\nStrikes inflict 'Surging Anguish'");
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.DamageType = DamageClass.Melee;
			Item.width = 34;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<HeadsplitterProj>();
			Item.shootSpeed = 6f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.autoReuse = true;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, .42f, .02f, .13f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			velocity = new Vector2(reverseSwing ? -1 : 1, 0);
			reverseSwing = !reverseSwing;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();
			if (player.altFunctionUse == 2)
			{
				if (modPlayer.cooldowns[Type] == 0)
				{
					if (!Main.dedServ)
					{
						//Create an Occultist-esque explosion effect
						ParticleHandler.SpawnParticle(new OccultistDeathBoom(player.Center, 0.8f));

						for (int i = 0; i < 25; i++)
							ParticleHandler.SpawnParticle(new GlowParticle(player.Center, Main.rand.NextVector2Circular(7, 7), new Color(99, 23, 25), Main.rand.NextFloat(0.04f, 0.08f), 35));

						for (int i = 0; i < 3; i++)
							ParticleHandler.SpawnParticle(new PulseCircle(player.Center, new Color(255, 33, 66) * 0.7f, 80 * i, 20) { RingColor = Color.Red });
					}
					if (Main.netMode != NetmodeID.Server)
						SoundEngine.PlaySound(SoundID.NPCDeath39 with { PitchVariance = 0.2f }, player.position);

					int maxDist = 100;
					foreach (NPC npc in Main.npc)
					{
						if (Collision.CanHitLine(player.position, player.width, player.height, npc.position, npc.width, npc.height) && player.Distance(npc.Center) < maxDist && npc.CanDamage() && npc.active)
						{
							int damage = (int)(Item.damage * (npc.HasBuff(ModContent.BuffType<SurgingAnguish>()) ? 3f : 0.75f));

							if ((int)(npc.life - npc.StrikeNPC(damage, 8, Math.Sign(npc.Center.X - player.Center.X))) <= 0 && !Main.dedServ)
								ParticleHandler.SpawnParticle(new OccultistDeathBoom(npc.Center, 0.4f));
						}
					}

					modPlayer.cooldowns[Type] = 180;
				}
				return false;
			}
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<DreamstrideEssence>(), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}