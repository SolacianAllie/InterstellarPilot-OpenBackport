using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace OpenFrontier.EditorTools
{
	/// <summary>
	/// Converts the 10 legacy "Particles/VertexLit Blended" materials to
	/// URP Particles/Unlit using the Material API (YAML hand-edits of these
	/// files get stripped by URP's material postprocessor, so we create the
	/// materials natively instead).
	/// </summary>
	public static class LegacyParticleConverter
	{
		static readonly string[] Paths =
		{
			"Assets/Material/MaterialAsteroidClusterTypeHDistant.mat",
			"Assets/Material/MaterialAsteroidClusterTypeADistant.mat",
			"Assets/Material/MaterialWormholeBilboard.mat",
			"Assets/Material/MaterialGasCloudExtremeDarkBilboard.mat",
			"Assets/Material/ShieldHit32MaterialNew.mat",
			"Assets/Material/MaterialWormholeParticles.mat",
			"Assets/Material/MaterialWormholeUnstableBilboard.mat",
			"Assets/Material/MaterialGasCloudBilboard.mat",
			"Assets/Material/MaterialStarParticle.mat",
			"Assets/Material/wormhole_capMaterial.mat",
		};

		[MenuItem("OpenFrontier/URP/2 - Convert Remaining Legacy Particle Materials")]
		public static void Convert()
		{
			var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
			if (shader == null)
			{
				Debug.LogError("URP Particles/Unlit shader not found - is URP active?");
				return;
			}
			var sb = new StringBuilder();
			foreach (var path in Paths)
			{
				var legacy = AssetDatabase.LoadAssetAtPath<Material>(path);
				if (legacy == null) { sb.AppendLine($"MISSING: {path}"); continue; }

				var tex = legacy.HasProperty("_MainTex") ? legacy.GetTexture("_MainTex") : null;
				var scale = legacy.HasProperty("_MainTex") ? legacy.GetTextureScale("_MainTex") : Vector2.one;
				var offset = legacy.HasProperty("_MainTex") ? legacy.GetTextureOffset("_MainTex") : Vector2.zero;
				var tint = legacy.HasProperty("_TintColor") ? legacy.GetColor("_TintColor") : Color.white;
				var invFade = legacy.HasProperty("_InvFade") ? legacy.GetFloat("_InvFade") : 1f;

				var mat = new Material(shader)
				{
					name = Path.GetFileNameWithoutExtension(path)
				};
				// transparent, alpha blend, no cull (faithful to legacy particle shaders)
				mat.SetFloat("_Surface", 1f);
				mat.SetFloat("_Blend", 0f);
				mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
				mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				mat.SetFloat("_SrcBlendAlpha", 1f);
				mat.SetFloat("_DstBlendAlpha", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				mat.SetFloat("_BlendOp", 0f);
				mat.SetFloat("_ZWrite", 0f);
				mat.SetFloat("_Cull", 0f);
				mat.SetColor("_BaseColor", tint);
				if (tex != null)
				{
					mat.SetTexture("_BaseMap", tex);
					mat.SetTextureScale("_BaseMap", scale);
					mat.SetTextureOffset("_BaseMap", offset);
				}
				if (invFade > 0f)
				{
					mat.SetFloat("_SoftParticlesEnabled", 1f);
					mat.SetFloat("_SoftParticlesNearFadeDistance", 0f);
					mat.SetFloat("_SoftParticlesFarFadeDistance", 1f / Mathf.Max(invFade, 0.0001f));
				}
				mat.SetOverrideTag("RenderType", "Transparent");
				mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

				AssetDatabase.DeleteAsset(path);
				AssetDatabase.CreateAsset(mat, path);
				sb.AppendLine($"converted: {path} (tex={(tex ? tex.name : "none")}, tint={tint})");
			}
			AssetDatabase.SaveAssets();
			Debug.Log(sb.ToString());
		}
	}
}
