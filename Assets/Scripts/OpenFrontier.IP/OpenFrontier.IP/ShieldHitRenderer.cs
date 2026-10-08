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
					pooledShieldHit.Duration = (pooledShieldHit.DepletedFlash ? 0.4f : ShieldHitDuration);
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
			// Bright at the moment of impact, then bleed out - no
			// fade-in (a fade-in mutes the impact).
			float num2 = 1f - num;
			Color color;
			if (hitInfo.DepletedFlash)
			{
				// Section emptied by the hit: very bright RED pop (HDR -
				// feeds bloom), gone in 0.4s.
				color = new Color(2f, 0.05f, 0.05f, num2);
			}
			else
			{
				// LIVE health color, exactly like the UI widgets read it
				// (no snapshot: damage lands before the effect request,
				// so a snapshot is always one hit stale). An HDR pulse
				// (1.8x -> 1x over the life) makes the impact flash.
				Color unitShieldColor = EngineASX.Instance.GetUnitShieldColor(hitInfo.TargetUnit, hitInfo.ShieldIndex);
				float num3 = 1f + 0.8f * (1f - num);
				color = new Color(unitShieldColor.r * num3, unitShieldColor.g * num3, unitShieldColor.b * num3, num2);
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
