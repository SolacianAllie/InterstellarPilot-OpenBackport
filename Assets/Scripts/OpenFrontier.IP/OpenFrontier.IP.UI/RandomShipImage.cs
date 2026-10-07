using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenFrontier.IP.UI
{
	/// <summary>
	/// Open Frontier: picks a random ship thumbnail for the loading screen
	/// each time the object is enabled, so every load shows a different ship.
	/// Sprites are loaded once from Resources/textures/ui/unitthumbnails and
	/// filtered to ship names (the folder also holds stations, asteroids,
	/// icons, etc). Never picks the same ship twice in a row.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class RandomShipImage : MonoBehaviour
	{
		private const string ThumbnailsPath = "textures/ui/unitthumbnails/";

		private static readonly string[] ShipThumbnailNames =
		{
			"achilles", "ares", "creon", "drake", "flyer", "hauler", "hornet",
			"interceptor", "lancer", "magnus", "orion", "overlord", "pioneer",
			"ranger", "raptor", "shuttle", "thunder", "venture"
		};

		private static Sprite[] shipSprites;

		private static int lastSpriteIndex = -1;

		private Image image;

		private void Awake()
		{
			image = GetComponent<Image>();
		}

		private void OnEnable()
		{
			EnsureSpritesLoaded();
			if (shipSprites.Length == 0)
			{
				return;
			}
			int num = PickRandomIndex();
			image.sprite = shipSprites[num];
		}

		private static int PickRandomIndex()
		{
			if (shipSprites.Length == 1)
			{
				return 0;
			}
			int num;
			do
			{
				num = UnityEngine.Random.Range(0, shipSprites.Length);
			}
			while (num == lastSpriteIndex);
			lastSpriteIndex = num;
			return num;
		}

		private static void EnsureSpritesLoaded()
		{
			if (shipSprites != null)
			{
				return;
			}
			List<Sprite> list = new List<Sprite>();
			foreach (string text in ShipThumbnailNames)
			{
				Sprite sprite = Resources.Load<Sprite>(ThumbnailsPath + text);
				if (sprite != null)
				{
					list.Add(sprite);
				}
				else
				{
					Debug.LogWarning("[RandomShipImage] Missing ship thumbnail: " + text);
				}
			}
			shipSprites = list.ToArray();
		}
	}
}
