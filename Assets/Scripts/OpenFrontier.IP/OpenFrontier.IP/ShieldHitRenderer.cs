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

		private static bool readbackLogged;

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
					pooledShieldHit.HitColor = EngineASX.Instance.GetUnitShieldColor(unit, unit.GetShieldIndex(damageSourceWorldPosition));
					// Cache the instanced material once per pooled object -
					// the property block tint never reached the shader on
					// this stack, so the color is set directly on it.
					if (pooledShieldHit.CachedMaterial == null)
					{
						pooledShieldHit.CachedMaterial = pooledShieldHit.Renderer.material;
					}
					Debug.Log(string.Format("[ShieldHit] mat={0} shader={1} snapshot=({2:0.00},{3:0.00},{4:0.00})", pooledShieldHit.CachedMaterial.name, pooledShieldHit.CachedMaterial.shader.name, pooledShieldHit.HitColor.r, pooledShieldHit.HitColor.g, pooledShieldHit.HitColor.b));
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
					float num = shieldHitInfo.StartExpiryTime + ShieldHitDuration;
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
			float a = 1f - num;
			hitInfo.transform.position = hitInfo.TargetUnit.transform.TransformPoint(hitInfo.LocalTranslation);
			hitInfo.transform.rotation = hitInfo.TargetUnit.transform.rotation * hitInfo.LocalRotation;
			float num2 = (1f - Mathf.Clamp01(num / ShieldScaleDuration)) * hitInfo.MaxScale;
			hitInfo.transform.localScale = new Vector3(num2, num2, num2);
			// Snapshot tint from impact time + the stock alpha fade, set
			// directly on the cached instanced material.
			if (hitInfo.CachedMaterial != null)
			{
				Color color = new Color(hitInfo.HitColor.r, hitInfo.HitColor.g, hitInfo.HitColor.b, a);
				hitInfo.CachedMaterial.SetColor("_BaseColor", color);
				if (!readbackLogged)
				{
					readbackLogged = true;
					Color color2 = hitInfo.CachedMaterial.GetColor("_BaseColor");
					Debug.Log(string.Format("[ShieldHit] readback _BaseColor=({0:0.00},{1:0.00},{2:0.00},{3:0.00})", color2.r, color2.g, color2.b, color2.a));
				}
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
