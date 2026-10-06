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
		

		[MenuItem("OpenFrontier/URP/1 - Create and Assign URP Pipeline")]
		public static void CreateAndAssign()
		{
			Directory.CreateDirectory(SettingsDir);

			var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
			if (pipeline == null)
			{
				// Create() builds the asset with a default Universal Renderer as a sub-asset
				pipeline = UniversalRenderPipelineAsset.Create();
				AssetDatabase.CreateAsset(pipeline, PipelinePath);
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
