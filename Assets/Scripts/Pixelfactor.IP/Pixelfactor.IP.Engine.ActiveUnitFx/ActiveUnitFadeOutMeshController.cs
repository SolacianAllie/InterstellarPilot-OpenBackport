using System.Collections.Generic;
using UnityEngine;

namespace Pixelfactor.IP.Engine.ActiveUnitFx
{
	public class ActiveUnitFadeOutMeshController : MonoBehaviour
	{
		private struct FadeOutMesh
		{
			public MaterialPropertyBlock MaterialPropertyBlock;

			public MeshRenderer MeshRenderer;

			public Color OriginalColor;

			public FadeOutMesh(MeshRenderer meshRenderer, MaterialPropertyBlock materialPropertyBlock, Color originalColor)
			{
				this = default;
				MeshRenderer = meshRenderer;
				MaterialPropertyBlock = materialPropertyBlock;
				OriginalColor = originalColor;
			}
		}

		private ActiveUnit activeUnit;

		public float UpperDistance = 300f;

		public float LowerDistance = 280f;

		public float FadeOutRate = 1f;

		public float FadeInRate = 1f;

		public bool Inverse;

		public bool RecalculateUnitDistanceInUpdate = true;

		public Color OriginalColor = Color.white;

		public bool UseRealTime = true;

		private List<FadeOutMesh> fadeOutMeshes = new List<FadeOutMesh>();

		public float GetDeltaTime()
		{
			if (UseRealTime)
			{
				return RealTime.deltaTime;
			}
			return Time.deltaTime;
		}

		private void Awake()
		{
			activeUnit = GetComponentInParent<ActiveUnit>();
			if (activeUnit == null)
			{
				Debug.LogError("Require active unit", this);
			}
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				// URP port: URP Lit exposes _BaseColor (not _Color). Seed
				// the block with the material's own color so the fade only
				// ever animates alpha and never wipes out the albedo.
				Color originalColor = OriginalColor;
				if (meshRenderer.sharedMaterial != null)
				{
					originalColor = meshRenderer.sharedMaterial.GetColor("_BaseColor");
				}
				fadeOutMeshes.Add(new FadeOutMesh(meshRenderer, materialPropertyBlock, originalColor));
				SetColour(materialPropertyBlock, meshRenderer, originalColor);
			}
			activeUnit.DrawDistanceChangedImmediate += ActiveUnit_DrawDistanceChanged;
		}

		private void OnDestroy()
		{
			if (activeUnit != null)
			{
				activeUnit.DrawDistanceChangedImmediate -= ActiveUnit_DrawDistanceChanged;
			}
		}

		private void ActiveUnit_DrawDistanceChanged(ActiveUnit sender)
		{
			Apply(immediate: true);
		}

		private void Update()
		{
			if (RecalculateUnitDistanceInUpdate && activeUnit != null)
			{
				activeUnit.UpdateCameraDistance();
			}
			Apply(immediate: false);
		}

		private void Apply(bool immediate)
		{
			if (!(activeUnit != null) || !(activeUnit.LastDistanceFromCamera < float.MaxValue))
			{
				return;
			}
			foreach (FadeOutMesh fadeOutMesh in fadeOutMeshes)
			{
				MeshRenderer meshRenderer = fadeOutMesh.MeshRenderer;
				float originalAlpha = fadeOutMesh.OriginalColor.a;
				// Read the animated color back from the block (written
				// as _BaseColor by SetColour), keeping the original albedo.
				Color color = fadeOutMesh.OriginalColor;
				color.a = fadeOutMesh.MaterialPropertyBlock.GetColor("_BaseColor").a;
				if (!(meshRenderer != null))
				{
					continue;
				}
				if (!Inverse)
				{
					if (color.a <= originalAlpha && activeUnit.LastDistanceFromCamera < LowerDistance)
					{
						color.a = (immediate ? originalAlpha : Mathf.Clamp(color.a + FadeInRate * GetDeltaTime(), 0f, originalAlpha));
						SetColour(fadeOutMesh.MaterialPropertyBlock, fadeOutMesh.MeshRenderer, color);
					}
					else if (color.a >= 0f && activeUnit.LastDistanceFromCamera > UpperDistance)
					{
						color.a = (immediate ? 0f : Mathf.Clamp(color.a - FadeOutRate * GetDeltaTime(), 0f, originalAlpha));
						SetColour(fadeOutMesh.MaterialPropertyBlock, fadeOutMesh.MeshRenderer, color);
					}
				}
				else if (color.a >= 0f && activeUnit.LastDistanceFromCamera < LowerDistance)
				{
					color.a = (immediate ? 0f : Mathf.Clamp(color.a - FadeOutRate * GetDeltaTime(), 0f, originalAlpha));
					SetColour(fadeOutMesh.MaterialPropertyBlock, fadeOutMesh.MeshRenderer, color);
				}
				else if (color.a < originalAlpha && activeUnit.LastDistanceFromCamera > UpperDistance)
				{
					color.a = (immediate ? originalAlpha : Mathf.Clamp(color.a + FadeInRate * GetDeltaTime(), 0f, originalAlpha));
					SetColour(fadeOutMesh.MaterialPropertyBlock, fadeOutMesh.MeshRenderer, color);
				}
			}
		}

		private static void SetColour(MaterialPropertyBlock materialPropertyBlock, MeshRenderer meshRenderer, Color color)
		{
			materialPropertyBlock.SetColor("_BaseColor", color); // URP port: _Color -> _BaseColor
			meshRenderer.SetPropertyBlock(materialPropertyBlock);
		}
	}
}
