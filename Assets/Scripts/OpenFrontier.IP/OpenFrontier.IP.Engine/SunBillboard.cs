using UnityEngine;
using UnityEngine.Rendering;

namespace OpenFrontier.IP.Engine
{
	/// <summary>
	/// Open Frontier: the sector's directional light has no visible source.
	/// This builds a simple two-layer sun (bright core quad + soft glow quad,
	/// additive) placed along the light's direction at a constant angular
	/// size, following the camera every frame so it reads as infinitely
	/// distant. Added at runtime by ActiveSectorData onto the sector's
	/// directional light.
	///
	/// The materials (Assets/Material/SunCore.mat, SunGlow.mat) are the
	/// source of truth for textures and base brightness; StarTint is
	/// multiplied in per frame, so material edits apply live.
	/// </summary>
	public class SunBillboard : MonoBehaviour
	{
		public Material CoreMaterial;

		public Material GlowMaterial;

		// Per-sector star color (set by ActiveSectorData via
		// StarColorGenerator.ResolveCurrent); defaults to white.
		public Color StarTint = Color.white;

		// Placement: camera-origin, every frame - the sun sits on the
		// light's axis at a fixed distance from the camera. Zero parallax,
		// so it NEVER drifts or snaps no matter how far the camera flies.
		// (World-pinning fails: flying 9000 units in hand-crafted sectors
		// drags the disk ~65 degrees off the light axis. Child-of-camera
		// fails: rotation glues it to the screen. Per-frame camera-origin
		// is the only scheme that is stable under both translation and
		// rotation - it IS the sky.)
		private const float CoreAngularDiameter = 6.6f;

		private const float GlowAngularDiameter = 60f;

		// Shared unit quad (XY plane, -Z front face); also used by the
		// StarColorMarker editor preview.
		public static Mesh QuadMesh => SharedMeshes.Quad;

		// The sun quads live on the BackgroundObjects layer (16), which the
		// moving GameCamera renders. NOT DeepSpace (30): that layer is
		// rendered only by the pinned SpaceScene camera, which made the sun
		// draw in the backdrop's frame instead of the world's - fine in
		// scene view, offset in game view.
		// NOTE: looked up lazily - NameToLayer (a native call) is forbidden
		// from a MonoBehaviour's type initializer; a static readonly there
		// throws TypeInitializationException and poisons the whole type.
		private static int sunLayer = -2;

		private static int SunLayer
		{
			get
			{
				if (sunLayer == -2)
				{
					sunLayer = LayerMask.NameToLayer("BackgroundObjects");
				}
				return sunLayer;
			}
		}

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
		}

		private void OnDestroy()
		{
			if (coreTransform != null)
			{
				Object.Destroy(coreTransform.gameObject);
			}
			if (glowTransform != null)
			{
				Object.Destroy(glowTransform.gameObject);
			}
		}

		// Materials are assigned AFTER AddComponent runs Awake, so the quads
		// are built lazily on the first LateUpdate that has them.
		private void Build()
		{
			coreTransform = CreateQuad("SunCore", CoreMaterial, out coreRenderer);
			glowTransform = CreateQuad("SunGlow", GlowMaterial, out glowRenderer);
		}

		private Transform CreateQuad(string objectName, Material material, out MeshRenderer meshRenderer)
		{
			GameObject gameObject = new GameObject(objectName);
			gameObject.transform.SetParent(transform, worldPositionStays: false);
			if (SunLayer >= 0)
			{
				gameObject.layer = SunLayer;
			}
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = SharedMeshes.Quad;
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = material;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			return gameObject.transform;
		}

		private void LateUpdate()
		{
			if (coreTransform == null)
			{
				if (CoreMaterial == null || GlowMaterial == null)
				{
					return;
				}
				Build();
			}
			Camera main = WorldCamera.Resolve();
			if (main == null || cachedLight == null)
			{
				return;
			}
			Vector3 vector = -cachedLight.transform.forward;
			// Publish the sun direction for the planet atmosphere shader
			// (day/night scattering follows the same light the sun shows).
			Shader.SetGlobalVector("_SunDirectionWorld", new Vector4(vector.x, vector.y, vector.z, 0f));
			float num = Mathf.Max(main.farClipPlane * 0.5f, 100f);
			Vector3 position = main.transform.position + vector * num;
			// True billboard: face the camera dead-on (forward along the view
			// ray, up aligned to the camera's up) so the sun reads correctly
			// from any angle, including near the zenith/nadir.
			Vector3 vector3 = position - main.transform.position;
			Quaternion rotation = ((vector3.sqrMagnitude > 0.0001f) ? Quaternion.LookRotation(vector3, main.transform.up) : Quaternion.identity);
			float num2 = Vector3.Distance(main.transform.position, position);
			float num3 = 2f * num2 * Mathf.Tan(CoreAngularDiameter * 0.5f * (Mathf.PI / 180f));
			coreTransform.SetPositionAndRotation(position, rotation);
			coreTransform.localScale = new Vector3(num3, num3, 1f);
			glowTransform.SetPositionAndRotation(position + vector, rotation);
			float num4 = num3 * (GlowAngularDiameter / CoreAngularDiameter);
			glowTransform.localScale = new Vector3(num4, num4, 1f);
			Color starTint = StarTint;
			// Material base color x star tint; the core is additionally
			// pulled towards white so it reads as the blinding disk.
			Color value = new Color((starTint.r + 1f) * 0.5f, (starTint.g + 1f) * 0.5f, (starTint.b + 1f) * 0.5f, 1f);
			propertyBlock.SetColor("_Color", CoreMaterial.GetColor("_Color") * value);
			coreRenderer.SetPropertyBlock(propertyBlock);
			propertyBlock.SetColor("_Color", GlowMaterial.GetColor("_Color") * starTint);
			glowRenderer.SetPropertyBlock(propertyBlock);
		}
	}
}
