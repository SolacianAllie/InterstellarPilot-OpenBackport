using UnityEngine;

namespace OpenFrontier.IP.Engine.ActiveUnitFx
{
	public class GasCloudBilboardController : MonoBehaviour
	{
		public float Distance = 100f;

		public float MinDistanceFromCamera = 50f;

		// Open Frontier: set by GasCloudController - when the camera is
		// inside ANY gas cloud, every cloud billboard is hidden (distant
		// billboards read as crisp shapes floating inside the fog you are
		// flying through). Each controller honors the flag by toggling its
		// own billboard group, so dynamically spawned clouds follow too.
		public static bool BillboardsHidden;

		private void Awake()
		{
			EngineASX.Instance.CameraMoved += Instance_CameraMoved;
		}

		private void Instance_CameraMoved(EngineASX sender)
		{
			Reposition();
		}

		private void OnDestroy()
		{
			EngineASX.Instance.CameraMoved -= Instance_CameraMoved;
		}

		private void FixedUpdate()
		{
			Reposition();
		}

		private void Update()
		{
			if (gameObject.activeSelf == BillboardsHidden)
			{
				gameObject.SetActive(!BillboardsHidden);
			}
			if (GameController.Instance != null && GameController.Instance.MainCamera != null)
			{
				Vector3 forward = Vector3.Normalize(transform.parent.position - GameController.Instance.MainCamera.transform.position);
				transform.rotation = Quaternion.LookRotation(forward);
			}
		}

		private void Reposition()
		{
			if (transform.parent != null && GameController.Instance != null && GameController.Instance.MainCamera != null)
			{
				Vector3 vector = GameController.Instance.MainCamera.transform.position - transform.parent.position;
				if (vector != Vector3.zero)
				{
					float num = Mathf.Min(Distance, vector.magnitude - MinDistanceFromCamera);
					transform.localPosition = Vector3.Normalize(vector) * num;
				}
			}
		}
	}
}
