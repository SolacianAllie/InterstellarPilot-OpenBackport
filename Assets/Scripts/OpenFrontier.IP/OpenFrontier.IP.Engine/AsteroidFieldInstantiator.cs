using System;
using System.Collections.Generic;
using OpenFrontier.Unity.Utils;
using UnityEngine;
using Random = System.Random;
using Object = UnityEngine.Object;

namespace OpenFrontier.IP.Engine
{
	public static class AsteroidFieldInstantiator
	{
		public static void Instantiate(AsteroidCluster asteroidCluster, Vector3 position, Transform parent, AsteroidFieldPrefabSettings prefabSettings, AsteroidFieldPlacementSettings placementSettings, System.Random random)
		{
			List<GameObject> list = new List<GameObject>(Mathf.CeilToInt(asteroidCluster.Unit.Radius / placementSettings.CountReferenceRadius * (float)placementSettings.Count));
			for (int i = 0; i < placementSettings.Count; i++)
			{
				GetSafeSpawnPosition(asteroidCluster, position, placementSettings, list, random, out var randomPosition, out var scale);
				GameObject gameObject = InstantiateAsteroid(prefabSettings, randomPosition, random.RandomQuaternion(), parent, random);
				gameObject.transform.localScale = scale;
				gameObject.isStatic = true;
				list.Add(gameObject);
			}
			if (GameController.Instance.GameSettings.VideoSettings.StaticBatchAsteroids)
			{
				StaticBatchingUtility.Combine(parent.gameObject);
			}
		}

		public static GameObject InstantiateAsteroid(AsteroidFieldPrefabSettings prefabSettings, Vector3 position, Quaternion rotation, Transform parent, System.Random random)
		{
			return UnityEngine.Object.Instantiate(prefabSettings.Prefabs.GetRandom(random), position, rotation, parent);
		}

		public static Vector3 GetRandomScale(AsteroidFieldPlacementSettings settings, float dist, float asteroidFieldRadius, System.Random random)
		{
			float b = asteroidFieldRadius / settings.MaxSizeReferenceAsteroidFieldRadius * settings.MaxSize;
			float num = Mathf.Lerp(settings.MinSize, b, Mathf.Pow(random.NextFloat(), settings.SizePower));
			return new Vector3(num, num, num);
		}

		public static void GetSafeSpawnPosition(AsteroidCluster asteroidCluster, Vector3 basePosition, AsteroidFieldPlacementSettings settings, List<GameObject> instances, System.Random random, out Vector3 randomPosition, out Vector3 scale)
		{
			float num = asteroidCluster.Unit.Radius * settings.MaxPlacementRadiusMultiplier;
			randomPosition = Vector3.zero;
			scale = Vector3.zero;
			for (int i = 0; i < 20; i++)
			{
				scale = GetRandomScale(settings, Mathf.Abs(randomPosition.y), asteroidCluster.Unit.Radius, random);
				Vector3 vector = random.RandomQuaternion() * (Vector3.forward * (random.NextFloat() * (num - scale.x / 2f)));
				randomPosition = basePosition + vector;
				if (Mathf.Abs(randomPosition.y) - scale.y < settings.MinYPosition)
				{
					randomPosition.y = Mathf.Sign(randomPosition.y) * (settings.MinYPosition + Mathf.Abs(scale.y));
				}
				bool flag = false;
				int num2 = 0;
				while (!flag && num2 < instances.Count)
				{
					if (Vector3.Distance(randomPosition, instances[num2].transform.position) < scale.x / 2f + settings.MinSpacing + instances[num2].transform.localScale.x / 2f)
					{
						flag = true;
					}
					num2++;
				}
				if (!flag)
				{
					break;
				}
			}
		}
	}
}
