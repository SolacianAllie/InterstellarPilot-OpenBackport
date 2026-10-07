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

		private const float CoreAngularDiameter = 6.6f;

		private const float GlowAngularDiameter = 60f;

		private static Mesh quadMesh;

		// Shared unit quad (XY plane, -Z front face); also used by the
		// StarColorMarker editor preview.
		public static Mesh QuadMesh
		{
			get
			{
				if (quadMesh == null)
				{
					quadMesh = BuildQuadMesh();
				}
				return quadMesh;
			}
		}

		// The sun quads live on the DeepSpace layer so the space reflection
		// probe (and only background-aware systems) can see them.
		// NOTE: looked up lazily - NameToLayer (a native call) is forbidden
		// from a MonoBehaviour's type initializer; a static readonly there
		// throws TypeInitializationException and poisons the whole type.
		private static int deepSpaceLayer = -2;

		private static int DeepSpaceLayer
		{
			get
			{
				if (deepSpaceLayer == -2)
				{
					deepSpaceLayer = LayerMask.NameToLayer("DeepSpace");
				}
				return deepSpaceLayer;
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
			Mesh sharedMesh = QuadMesh;
			coreTransform = CreateQuad("SunCore", CoreMaterial, out coreRenderer);
			glowTransform = CreateQuad("SunGlow", GlowMaterial, out glowRenderer);
		}

		// Unit quad in the XY plane, front face looking down -Z (same
		// orientation as Unity's builtin Quad, but with no dependency on
		// builtin resource names).
		private static Mesh BuildQuadMesh()
		{
			Mesh mesh = new Mesh();
			mesh.name = "SunQuad";
			mesh.vertices = new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, 0f),
				new Vector3(0.5f, -0.5f, 0f),
				new Vector3(0.5f, 0.5f, 0f),
				new Vector3(-0.5f, 0.5f, 0f)
			};
			mesh.normals = new Vector3[]
			{
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward
			};
			mesh.uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(1f, 1f),
				new Vector2(0f, 1f)
			};
			mesh.triangles = new int[] { 0, 3, 2, 0, 2, 1 };
			mesh.RecalculateBounds();
			return mesh;
		}

		private Transform CreateQuad(string objectName, Material material, out MeshRenderer meshRenderer)
		{
			GameObject gameObject = new GameObject(objectName);
			gameObject.transform.SetParent(transform, worldPositionStays: false);
			if (DeepSpaceLayer >= 0)
			{
				gameObject.layer = DeepSpaceLayer;
			}
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = quadMesh;
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
			// WorldCamera.Resolve, not Camera.main - two cameras share the
			// MainCamera tag (MenuCamera/GameCamera) and the menu one is
			// static, which made the sun parallax wrongly while flying.
			Camera main = WorldCamera.Resolve();
			if (main == null || cachedLight == null)
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
