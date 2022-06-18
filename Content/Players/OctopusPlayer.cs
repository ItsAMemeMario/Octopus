using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Octopus.Content.Mounts;
using Octopus.Content.Projectiles;
using Octopus.Content.Systems;
using System;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;
using Octopus.Content.Items;

namespace Octopus.Content.Players
{
		public class OctopusPlayer : ModPlayer
		{
				public bool isInOctopusForm = false;

				public int headTurnTimer = 0;

				private const int DoubleJumpCooldown = 30;
				private int DoubleJumpDelay = 0;

				private const int DashCooldown = 50;
				//private const int DashDuration = 35;
				private const float DashSpeed = 10f;
				private int DashDelay = 0;
				//private int DashTimer = 0;
				private int DashDir = 0;

				public override void ResetEffects()
				{
						if (isInOctopusForm && DoubleJumpDelay == 0)
						{
								Player.hasJumpOption_Fart = true;
								Player.canJumpAgain_Fart = true;
						}

						isInOctopusForm = false;

						if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[2] < 15)
						{
								DashDir = 1;
						} else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[3] < 15)
						{
								DashDir = -1;
						} else
						{
								DashDir = 0;
						}
				}

				public override void FrameEffects()
				{
						// TODO: Need new hook, FrameEffects doesn't run while paused.
						if (isInOctopusForm)
						{
								Player.head = EquipLoader.GetEquipSlot(Mod, "InvisiblePlayer", EquipType.Head);
								Player.body = EquipLoader.GetEquipSlot(Mod, "InvisiblePlayer", EquipType.Body);
								Player.legs = EquipLoader.GetEquipSlot(Mod, "InvisiblePlayer", EquipType.Legs);

								ArmorIDs.Head.Sets.DrawHead[KetchupBall.equipSlotHead] = false;
								ArmorIDs.Body.Sets.HidesTopSkin[KetchupBall.equipSlotBody] = true;
								ArmorIDs.Body.Sets.HidesArms[KetchupBall.equipSlotBody] = true;
								ArmorIDs.Legs.Sets.HidesBottomSkin[KetchupBall.equipSlotLegs] = true;

						}
						if (headTurnTimer != 0)
						{
								Player.direction = headTurnTimer / Math.Abs(headTurnTimer);
								headTurnTimer -= headTurnTimer / Math.Abs(headTurnTimer);
						}
				}

				public override void PreUpdateMovement()
				{
						if (isInOctopusForm)
						{
								// Fart
								if (DoubleJumpDelay == 0 && Player.oldVelocity.Y != 0 && Player.velocity.Y != 0 && Player.controlJump)
								{
										DoubleJumpDelay = DoubleJumpCooldown;
								}
								if (DoubleJumpDelay > 0)
								{
										DoubleJumpDelay--;
								}

								// Dash
								if (DashDir != 0 && DashDelay == 0)
								{
										Vector2 newVelocity = Player.velocity;

										switch (DashDir)
										{
												// Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
												case -1 when Player.velocity.X > -DashSpeed:
												case 1 when Player.velocity.X < DashSpeed:
														{
																newVelocity.X = DashDir * DashSpeed;
																// start our dash
																DashDelay = DashCooldown;
																//DashTimer = DashDuration;
																Player.velocity = newVelocity;
																break;
														}
										}

										// Fart
										SoundEngine.PlaySound(SoundID.Item16, Player.Center);
										// Poop
										Vector2 poopPosition = new Vector2(Player.position.X - 10f, Player.Center.Y - DashDir * 10f);
										Projectile.NewProjectile(Player.GetSource_FromThis(), poopPosition, Player.velocity, ProjectileID.ToiletEffect, 0, 0f, Owner: Player.whoAmI);

								}
								else if (DashDelay > 0)
								{
										DashDelay--;
								}
						}
				}

				public override void PreUpdate()
				{
						if (isInOctopusForm)
						{
								// Squirt water when WaterSpitKeyBind is held
								if (KeybindSystem.WaterSpitKeybind.Current)
								{
										Vector2 shootPosition = new Vector2(Player.Center.X - Player.direction * 12f, Player.Center.Y + Player.velocity.Y * 2f);
										if ((int)Main.time % 6 == 0)
										{
												SoundEngine.PlaySound(SoundID.Item13);
												Projectile.NewProjectile(Player.GetSource_FromThis(), shootPosition.X, shootPosition.Y, Player.direction * 8f + Player.velocity.X * 0.3f, -1f, ProjectileID.WaterStream, 15, 5f, Player.whoAmI);
										}
								}
								else if (headTurnTimer == 0 && KeybindSystem.InkBombKeybind.JustPressed)
								{
										SoundEngine.PlaySound(SoundID.NPCHit25);
										int headTurn;
										if (Main.MouseWorld.X >= Player.Center.X)
										{
												headTurn = 1;
										}
										else
										{
												headTurn = -1;
										}
										Vector2 shootPosition = new Vector2(Player.position.X + headTurn * 10f + 10f + Player.velocity.X * 2f, Player.position.Y + 14f + Player.velocity.Y * 2f);
										Vector2 shootVelocity = Main.MouseWorld - shootPosition;
										shootVelocity = (shootVelocity / shootVelocity.Length()) * 16f;
										Projectile.NewProjectile(Player.GetSource_FromThis(), shootPosition, shootVelocity, ModContent.ProjectileType<InkBomb>(), 100, 5f, Player.whoAmI);
										headTurnTimer = headTurn * 15;
								}
						}
				}

				public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
				{
						if (isInOctopusForm)
						{
								SoundEngine.PlaySound(SoundID.NPCHit25, Player.position);
						}
				}
		}
}