using UnityEngine;
using UnityEngine.Rendering;

namespace OpenFrontier
{
	/// <summary>
	/// Open Frontier: the sector's directional light has no visible source.
	/// This builds a simple two-layer sun (bright core quad + soft glow quad,
	/// additive) placed along the light's direction at a constant angular
	/// size, following the camera every frame so it reads as infinitely
	/// distant. Added at runtime by ActiveSectorData onto the sector's
	/// directional light; tint follows the light's color.
	/// </summary>
	public class SunBillboard : MonoBehaviour
	{
		public Texture2D CoreTexture;

		public Texture2D GlowTexture;

		private const float CoreAngularDiameter = 2.2f;

		private const float GlowAngularDiameter = 10f;

		private const float CoreIntensity = 2.2f;

		private const float GlowIntensity = 0.85f;

		private static Mesh quadMesh;

		private static Material sunMaterial;

		private Light cachedLight;

		private Transform coreTransform;

		private Transform glowTransform;

		private MeshRenderer coreRenderer;

		private MeshRenderer glowRenderer;

		private MaterialPropertyBlock propertyBlock;

		private void Awake()
		{
			cachedLight = GetComponent<Light>();
			propertyBlock = new MaterialPropertyBlock();
			if (quadMesh == null)
			{
				quadMesh = Resources.GetBuiltinResource<Mesh>("Quad.mesh");
			}
			if (sunMaterial == null)
			{
				Shader shader = Resources.Load<Shader>("Shaders/OpenFrontier_SunGlow");
				if (shader == null)
				{
					Debug.LogWarning("[SunBillboard] Shaders/OpenFrontier_SunGlow not found in Resources", this);
					enabled = false;
					return;
				}
				sunMaterial = new Material(shader);
			}
			coreTransform = CreateQuad("SunCore", out coreRenderer);
			glowTransform = CreateQuad("SunGlow", out glowRenderer);
		}

		private Transform CreateQuad(string objectName, out MeshRenderer meshRenderer)
		{
			GameObject gameObject = new GameObject(objectName);
			gameObject.transform.SetParent(transform, worldPositionStays: false);
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = quadMesh;
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = sunMaterial;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			return gameObject.transform;
		}

		private void LateUpdate()
		{
			Camera main = Camera.main;
			if (main == null || cachedLight == null || coreTransform == null)
			{
				return;
			}
			Vector3 vector = -cachedLight.transform.forward;
			float num = Mathf.Max(main.farClipPlane * 0.5f, 100f);
			Vector3 position = main.transform.position + vector * num;
			Quaternion rotation = Quaternion.LookRotation(vector);
			float num2 = 2f * num * Mathf.Tan(CoreAngularDiameter * 0.5f * (Mathf.PI / 180f));
			coreTransform.SetPositionAndRotation(position, rotation);
			coreTransform.localScale = new Vector3(num2, num2, 1f);
			glowTransform.SetPositionAndRotation(position + vector, rotation);
			float num3 = num2 * (GlowAngularDiameter / CoreAngularDiameter);
			glowTransform.localScale = new Vector3(num3, num3, 1f);
			Color color = cachedLight.color;
			propertyBlock.SetTexture("_BaseMap", (CoreTexture != null) ? CoreTexture : Texture2D.whiteTexture);
			propertyBlock.SetColor("_Color", new Color((color.r + 1f) * 0.5f * CoreIntensity, (color.g + 1f) * 0.5f * CoreIntensity, (color.b + 1f) * 0.5f * CoreIntensity, 1f));
			coreRenderer.SetPropertyBlock(propertyBlock);
			propertyBlock.SetTexture("_BaseMap", (GlowTexture != null) ? GlowTexture : Texture2D.whiteTexture);
			propertyBlock.SetColor("_Color", new Color(color.r * GlowIntensity, color.g * GlowIntensity, color.b * GlowIntensity, 1f));
			glowRenderer.SetPropertyBlock(propertyBlock);
		}
	}
}
