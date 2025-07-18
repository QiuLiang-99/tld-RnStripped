// https://raw.githubusercontent.com/ShieldHeart1/RelentlessNight/master/Gameplay/ElectricTorchLighting.cs
// i do not know if this even works ;) 

using HarmonyLib;
using Il2CppTLD.Interactions;
using UnityEngine;

namespace ttrRnStripped
{

	internal class ElectricTorchLighting
	{

		// maybe move to globals ?
		private static string[] itemsCanLightTorch =
			{
			"socket",
			"outlet",
			"cableset",
			"electricdamage_temp",
			};

		private static GameObject? lookingAt = null;

		[HarmonyPatch(typeof(MissionServicesManager), nameof(MissionServicesManager.RegisterAnyMissionObjects))]
		internal class MissionServicesManager_RegisterAnyMissionObjects
		{
			private static void Postfix()
			{
				if (!Settings.options.electricTorchLightingEnabled) return;

				MakeTorchLightingItemsInteractible();
			}
		}

		[HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.InteractiveObjectsProcessInteraction))]
		internal static class PlayerManager_InteractiveObjectsProcessInteraction
		{
			private static void Postfix(PlayerManager __instance)
			{
				if (!Settings.options.electricTorchLightingEnabled) return;

				if (!GameManager.GetAuroraManager().AuroraIsActive() || !PlayerInteractingWithElectricLightSource(__instance) || !__instance.PlayerHoldingTorchThatCanBeLit()) return;

				if (InterfaceManager.GetPanel<Panel_TorchLight>() != null) InterfaceManager.GetPanel<Panel_TorchLight>().StartTorchIgnite(2f, string.Empty, true);
			}
		}

		// Removes burn damage from stepping on wires, ensures player wont get burned trying to light torch from cable
		[HarmonyPatch(typeof(DamageTrigger), nameof(DamageTrigger.ApplyOneTimeDamage), new Type[] { typeof(GameObject), typeof(float) })]
		internal class DamageTrigger_ApplyOneTimeDamage
		{
			private static bool Prefix(DamageTrigger __instance)
			{
				if (!Settings.options.electricTorchLightingEnabled) return true;

				if (__instance.m_DamageSource != DamageSource.Electrical) return true;
				return false;
			}
		}

		[HarmonyPatch(typeof(DamageTrigger), nameof(DamageTrigger.ApplyContinuousDamage), new Type[] { typeof(GameObject), typeof(float) })]
		internal class DamageTrigger_ApplyContinousDamage
		{
			private static bool Prefix(DamageTrigger __instance)
			{
				if (!Settings.options.electricTorchLightingEnabled || __instance.m_DamageSource != DamageSource.Electrical) return true;

				return false;
			}
		}

		// Prevents saving when moving over live wires, as burn damage is removed
		[HarmonyPatch(typeof(DamageTrigger), nameof(DamageTrigger.OnTriggerExit))]
		internal class DamageTrigger_OnTriggerExit
		{
			private static bool Prefix(DamageTrigger __instance)
			{
				if (!Settings.options.electricTorchLightingEnabled) return true;

				return false;
			}
		}

		[HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.GetInteractiveObjectUnderCrosshairs), new Type[] { typeof(float) })]
		internal class GetInteractiveObjectUnderCrosshairs
		{
			public static void Postfix(PlayerManager __instance, ref GameObject? __result)
			{
				if (__result == null || __result != lookingAt)
				{
					if (lookingAt != null)
					{
						SimpleInteraction si = lookingAt.GetComponent<SimpleInteraction>();
						if (si != null)
						{
							si.enabled = false;
							lookingAt = null;
						}
					}
				}

				if (
					__result != null 
					&& itemsCanLightTorch.Any(__result.name.ToLowerInvariant().Contains)
					&& GameManager.GetAuroraManager().AuroraIsActive()
					&& __instance.PlayerHoldingTorchThatCanBeLit()
					)
				{
					SimpleInteraction si = __result.GetComponent<SimpleInteraction>();
					if (si != null && __result != lookingAt)
					{
						si.enabled = true;
						lookingAt = __result;
					}
				}
			}
		}

		private static bool PlayerInteractingWithElectricLightSource(PlayerManager __instance)
		{
			float maxPickupRange = GameManager.GetGlobalParameters().m_MaxPickupRange;
			float maxRange = __instance.ComputeModifiedPickupRange(maxPickupRange);

			GameObject itemUnderCrosshair = __instance.GetInteractiveObjectUnderCrosshairs(maxRange);

			if (
				itemUnderCrosshair != null
				&& itemsCanLightTorch.Any(itemUnderCrosshair.name.ToLowerInvariant().Contains)
				)
			{
				return true;
			}

			return false;
		}

		internal static void MakeTorchLightingItemsInteractible()
		{
			List<GameObject> rObjs = Utilities.GetRootObjects();
			Dictionary<int, GameObject> found = new();

			foreach (GameObject rootObj in rObjs)
			{
				found.Clear();
				Utilities.GetChildrenWithNameArray(rootObj, itemsCanLightTorch, found);
				if (found.Count > 0)
				{
					foreach (KeyValuePair<int, GameObject> item in found)
					{
						item.Value.layer = vp_Layer.InteractivePropNoCollideGear;
						SimpleInteraction interaction = item.Value.gameObject.AddComponent<SimpleInteraction>();
						LocalizedString loStr = new();
						loStr.m_LocalizationID = "GAMEPLAY_Light";
						interaction.m_DefaultHoverText = loStr;
						interaction.enabled = false;
					}
				}
			}
		}
	}
}