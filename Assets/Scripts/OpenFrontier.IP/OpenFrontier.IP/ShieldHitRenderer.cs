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
				ShieldHitInfo pooledShieldHit = GetPooledShieldHit();
				if (pooledShieldHit != null)
				{
					// Open Frontier: constant size - the stock damage-scaled
					// sizing made depleted-shield hits (which absorb almost
					// nothing) spawn tiny and vanish fast. Always max.
					pooledShieldHit.MaxScale = MaxShieldHitScale;
					pooledShieldHit.gameObject.SetActive(value: true);
					pooledShieldHit.TargetUnit = unit;
					pooledShieldHit.ShieldIndex = unit.GetShieldIndex(damageSourceWorldPosition);
					// A section this hit just emptied pops: bright red
					// flash, short life.
					pooledShieldHit.DepletedFlash = unit.Components != null && unit.Components.ShieldComponent != null && unit.Components.ShieldComponent.GetShieldPointNormalized(pooledShieldHit.ShieldIndex) <= 0f;
					// ~Laser fade-out time (0.25-0.4s on the classes):
					// the shield flash dies with the beam that made it.
					pooledShieldHit.Duration = (pooledShieldHit.DepletedFlash ? 0.4f : 0.5f);
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
			float num2 = num * (expiryTime - hitInfo.StartExpiryTime);
			Color color;
			if (hitInfo.DepletedFlash)
			{
				// Section emptied by the hit: very bright RED pop (HDR -
				// feeds bloom), gone in 0.4s.
				color = new Color(2f, 0.05f, 0.05f, 1f - num);
			}
			else
			{
				Color unitShieldColor = EngineASX.Instance.GetUnitShieldColor(hitInfo.TargetUnit, hitInfo.ShieldIndex);
				if (num2 < 0.1f)
				{
					// Flash phase (0.1s): fade in, overdrive a little to
					// white, settle back to the health color.
					float num3 = num2 / 0.1f;
					Color color2 = Color.Lerp(new Color(1.4f, 1.4f, 1.4f), unitShieldColor, num3);
					color = new Color(color2.r, color2.g, color2.b, Mathf.Clamp01(num3 * 2f));
				}
				else
				{
					// Sustain phase: live UI health color, alpha bleeding
					// out so the flash dies with the laser's fade.
					color = new Color(unitShieldColor.r, unitShieldColor.g, unitShieldColor.b, 1f - (num2 - 0.1f) / (expiryTime - hitInfo.StartExpiryTime - 0.1f));
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
