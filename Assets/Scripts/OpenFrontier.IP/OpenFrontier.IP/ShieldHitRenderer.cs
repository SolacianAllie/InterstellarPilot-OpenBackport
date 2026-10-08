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
					pooledShieldHit.MaxScale = Mathf.Clamp(shieldDamage * DamageScaleMultiplier, MinShieldHitScale, MaxShieldHitScale);
					pooledShieldHit.gameObject.SetActive(value: true);
					pooledShieldHit.TargetUnit = unit;
					pooledShieldHit.ShieldIndex = unit.GetShieldIndex(damageSourceWorldPosition);
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
			// Open Frontier: tint with the hit shield section's LIVE
			// health color (the same GetUnitShieldColor the UI widgets
			// use) - blue while the section holds, shifting as it
			// depletes, so shield state reads in the world, not just on
			// the HUD. Alpha keeps the stock fade.
			Color unitShieldColor = EngineASX.Instance.GetUnitShieldColor(hitInfo.TargetUnit, hitInfo.ShieldIndex);
			Color color = new Color(unitShieldColor.r, unitShieldColor.g, unitShieldColor.b, a);
			hitInfo.transform.position = hitInfo.TargetUnit.transform.TransformPoint(hitInfo.LocalTranslation);
			hitInfo.transform.rotation = hitInfo.TargetUnit.transform.rotation * hitInfo.LocalRotation;
			float num2 = (1f - Mathf.Clamp01(num / ShieldScaleDuration)) * hitInfo.MaxScale;
			hitInfo.transform.localScale = new Vector3(num2, num2, num2);
			hitInfo.Renderer.material.color = color;
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
