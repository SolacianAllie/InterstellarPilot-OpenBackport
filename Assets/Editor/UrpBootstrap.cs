using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OpenFrontier.EditorTools
{
	public static class UrpBootstrap
	{
		const string SettingsDir = "Assets/Settings";
		const string PipelinePath = SettingsDir + "/OpenFrontier-URP.asset";
		const string RendererPath = SettingsDir + "/OpenFrontier-URP-Renderer.asset";

		[MenuItem("OpenFrontier/URP/1 - Create and Assign URP Pipeline")]
		public static void CreateAndAssign()
		{
			Directory.CreateDirectory(SettingsDir);

			var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
			UniversalRendererData renderer = null;
			if (pipeline == null)
			{
				pipeline = UniversalRenderPipelineAsset.Create();
				AssetDatabase.CreateAsset(pipeline, PipelinePath);
				renderer = UniversalRenderPipelineAsset.CreateRendererAsset(RendererPath, RendererType.UniversalRenderer, false);
				// wire the renderer into the pipeline
				var so = new SerializedObject(pipeline);
				var prop = so.FindProperty("m_RendererDataList");
				prop.arraySize = 1;
				prop.GetArrayElementAtIndex(0).objectReferenceValue = renderer;
				prop = so.FindProperty("m_DefaultRendererIndex");
				if (prop != null) prop.intValue = 0;
				so.ApplyModifiedPropertiesWithoutUndo();
			}
			else
			{
				Debug.Log("Pipeline asset already exists, reassigning only.");
			}

			// Graphics settings
			GraphicsSettings.defaultRenderPipeline = pipeline;
			// All quality levels
			int levels = QualitySettings.names.Length;
			for (int i = 0; i < levels; i++)
			{
				QualitySettings.SetQualityLevel(i, false);
				QualitySettings.renderPipeline = pipeline;
			}
			QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel(), true);

			AssetDatabase.SaveAssets();
			Debug.Log($"URP pipeline created at {PipelinePath} and assigned to Graphics + all Quality levels. Now run: Edit > Rendering > Materials > Convert All Built-in Materials to URP (or our scripted converters).");
		}

		[MenuItem("OpenFrontier/URP/Report Current Pipeline")]
		public static void Report()
		{
			Debug.Log($"GraphicsSettings.defaultRenderPipeline: {(GraphicsSettings.defaultRenderPipeline == null ? "BUILT-IN" : GraphicsSettings.defaultRenderPipeline.name)}\n" +
				$"QualitySettings.renderPipeline: {(QualitySettings.renderPipeline == null ? "BUILT-IN" : QualitySettings.renderPipeline.name)}");
		}
	}
}
