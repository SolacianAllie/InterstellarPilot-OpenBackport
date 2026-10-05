using UnityEngine;

namespace Pixelfactor.IP.Engine.ActiveUnitFx
{
	public class GasCloudBilboardController : MonoBehaviour
	{
		public float Distance = 100f;

		public float MinDistanceFromCamera = 50f;

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
