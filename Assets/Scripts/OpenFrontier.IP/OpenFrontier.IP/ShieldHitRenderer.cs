using System.Collections.Generic;
using OpenFrontier.IP.Engine;
using UnityEngine;

namespace OpenFrontier.IP
{
	public class ShieldHitRenderer : MonoBehaviour
	{
		private List<ShieldHitInfo> pooledObjects = new List<ShieldHitInfo>();

		public ShieldHitInfo ShieldHitPrefab;

		public float ShieldHitDuration = 2f;

		public float ShieldScaleDuration = 0.5f;

		public float MaxShieldHitScale = 5f;

		public float MinShieldHitScale = 1f;

		public float DamageScaleMultiplier = 5f;

		public int InitialPoolSize = 25;

		// Per (unit, section) contact latch for the one-flash-per-contact
		// rule (pure C# dictionary - no native calls, safe as a static).
		private static readonly Dictionary<(Unit, int), float> LastFlashRequestTimes = new Dictionary<(Unit, int), float>();

		private void Awake()
		{
			for (int i = 0; i < InitialPoolSize; i++)
			{
				ShieldHitInfo item = UnityObjectHelper.InstantiateAndGetComponent(ShieldHitPrefab);
				pooledObjects.Add(item);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < pooledObjects.Count; i++)
			{
				pooledObjects[i].gameObject.SetActive(value: false);
			}
		}

		public ShieldHitInfo RequestShieldHit(Unit unit, Vector3 damageSourceWorldPosition, float shieldDamage)
		{
			if (Vector3.Distance(unit.transform.position, GameController.Instance.MainCamera.transform.position) < EngineASX.Instance.PerformanceSettings.MaxShieldHitDist)
			{
				int shieldIndex = unit.GetShieldIndex(damageSourceWorldPosition);
				// One flash per CONTACT: a sustained beam lands a damage
				// tick every frame, and without a latch each tick after
				// the flash would re-trigger it - strobing for as long
				// as the beam holds. Allow a new flash only when this
				// section has gone 0.6s without a hit; the latch updates
				// on every request, so a continuous beam flashes once.
				(Unit, int) key = (unit, shieldIndex);
				float num;
				bool flag = LastFlashRequestTimes.TryGetValue(key, out num) && Time.time - num < 1f;
				LastFlashRequestTimes[key] = Time.time;
				if (LastFlashRequestTimes.Count > 1024)
				{
					LastFlashRequestTimes.Clear();
				}
				if (flag)
				{
					return null;
				}
				ShieldHitInfo pooledShieldHit = GetPooledShieldHit();
				if (pooledShieldHit != null)
				{
					// Open Frontier: constant size - the stock damage-scaled
					// sizing made depleted-shield hits (which absorb almost
					// nothing) spawn tiny and vanish fast. Always max.
					pooledShieldHit.MaxScale = MaxShieldHitScale;
					pooledShieldHit.gameObject.SetActive(value: true);
					pooledShieldHit.TargetUnit = unit;
					pooledShieldHit.ShieldIndex = shieldIndex;
					// A section this hit just emptied pops: bright red
					// flash, short life.
					pooledShieldHit.DepletedFlash = unit.Components != null && unit.Components.ShieldComponent != null && unit.Components.ShieldComponent.GetShieldPointNormalized(pooledShieldHit.ShieldIndex) <= 0f;
					// 1.15s envelope (1s hold + 0.15s fade); the
					// depletion pop is a quick 0.25s.
					pooledShieldHit.Duration = (pooledShieldHit.DepletedFlash ? 0.25f : 1.15f);
					// Cache the instanced material once per pooled object -
					// the property block tint never reached the shader on
					// this stack, so the color is set directly on it.
					if (pooledShieldHit.CachedMaterial == null)
					{
						pooledShieldHit.CachedMaterial = pooledShieldHit.Renderer.material;
					}
					pooledShieldHit.StartExpiryTime = Time.time;
					pooledShieldHit.StartTime = Time.time;
					pooledShieldHit.SetShieldHitOrientation(damageSourceWorldPosition);
					// Apply the first frame NOW: a pooled object would
					// otherwise activate for one frame wearing its
					// PREVIOUS effect's transform and tint - a stale
					// ghost that reads as a flicker before the flash.
					pooledShieldHit.transform.position = unit.transform.TransformPoint(pooledShieldHit.LocalTranslation);
					pooledShieldHit.transform.rotation = unit.transform.rotation * pooledShieldHit.LocalRotation;
					pooledShieldHit.transform.localScale = new Vector3(pooledShieldHit.MaxScale, pooledShieldHit.MaxScale, pooledShieldHit.MaxScale);
					if (pooledShieldHit.CachedMaterial != null)
					{
						pooledShieldHit.CachedMaterial.SetColor("_TintColor", new Color(1.4f, 1.4f, 1.4f, 0f));
					}
					return pooledShieldHit;
				}
			}
			return null;
		}

		private void Update()
		{
			for (int i = 0; i < pooledObjects.Count; i++)
			{
				ShieldHitInfo shieldHitInfo = pooledObjects[i];
				if (shieldHitInfo.gameObject.activeSelf)
				{
					float num = shieldHitInfo.StartExpiryTime + shieldHitInfo.Duration;
					if (Time.time > num || shieldHitInfo.TargetUnit == null)
					{
						shieldHitInfo.gameObject.SetActive(value: false);
					}
					else
					{
						UpdateShieldHitMaterial(shieldHitInfo, num);
					}
				}
			}
		}

		private void UpdateShieldHitMaterial(ShieldHitInfo hitInfo, float expiryTime)
		{
			float num = Mathf.Clamp01((Time.time - hitInfo.StartExpiryTime) / (expiryTime - hitInfo.StartExpiryTime));
			hitInfo.transform.position = hitInfo.TargetUnit.transform.TransformPoint(hitInfo.LocalTranslation);
			hitInfo.transform.rotation = hitInfo.TargetUnit.transform.rotation * hitInfo.LocalRotation;
			// Constant size for the whole life - the fade is carried by
			// alpha now, not by shrinking away.
			hitInfo.transform.localScale = new Vector3(hitInfo.MaxScale, hitInfo.MaxScale, hitInfo.MaxScale);
			float num2 = Time.time - hitInfo.StartExpiryTime;
			Color color;
			if (hitInfo.DepletedFlash)
			{
				// Death: heavier overdrive bright red, dead in 0.25s.
				float num3 = Mathf.Clamp01(num2 / 0.25f);
				float num4 = Mathf.Clamp01(num2 / 0.06f);
				color = new Color(4f * num4, 0.1f * num4, 0.1f * num4, 1f - num3);
			}
			else
			{
				Color unitShieldColor = EngineASX.Instance.GetUnitShieldColor(hitInfo.TargetUnit, hitInfo.ShieldIndex);
				if (num2 < 0.1f)
				{
					// 0.0-0.1s: fade in toward white.
					float num5 = num2 / 0.1f;
					color = new Color(1.4f, 1.4f, 1.4f, num5);
				}
				else if (num2 < 0.2f)
				{
					// 0.1-0.2s: overdrive peak (HDR white).
					color = new Color(2f, 2f, 2f, 1f);
				}
				else if (num2 < 0.45f)
				{
					// 0.2-0.45s: settle back to the health color.
					Color color2 = Color.Lerp(new Color(2f, 2f, 2f), unitShieldColor, (num2 - 0.2f) / 0.25f);
					color = new Color(color2.r, color2.g, color2.b, 1f);
				}
				else if (num2 < 1f)
				{
					// 0.45-1.0s: hold steady at the health color.
					color = new Color(unitShieldColor.r, unitShieldColor.g, unitShieldColor.b, 1f);
				}
				else
				{
					// 1.0-1.15s: fade out.
					color = new Color(unitShieldColor.r, unitShieldColor.g, unitShieldColor.b, 1f - (num2 - 1f) / 0.15f);
				}
			}
			// The runtime shader is Legacy Particles/Additive: it tints
			// via _TintColor (NOT _Color or _BaseColor).
			if (hitInfo.CachedMaterial != null)
			{
				hitInfo.CachedMaterial.SetColor("_TintColor", color);
			}
		}

		private ShieldHitInfo GetPooledShieldHit()
		{
			for (int i = 0; i < pooledObjects.Count; i++)
			{
				if (!pooledObjects[i].gameObject.activeSelf)
				{
					return pooledObjects[i];
				}
			}
			return null;
		}
	}
}
