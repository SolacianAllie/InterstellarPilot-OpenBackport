using System.Text;
using Pixelfactor.IP.Engine;
using UnityEditor;
using UnityEngine;

namespace OpenFrontier.EditorTools
{
	public static class MigrationDiagnostics
	{
		[MenuItem("OpenFrontier/Dump Formation Chain State")]
		public static void Dump()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Application.isPlaying: {Application.isPlaying}");
			var gc = GameController.Instance;
			sb.AppendLine($"GameController.Instance: {Describe(gc)}");
			if (gc != null)
			{
				var gs = gc.GameSettings;
				sb.AppendLine($"GameSettings: {Describe(gs)}");
				if (gs != null)
				{
					var fs = gs.FormationSettings;
					sb.AppendLine($"FormationSettings: {Describe(fs)}");
					if (fs != null)
					{
						sb.AppendLine($"FormationSettings.GetInstanceID-safe name: {fs.name}, gameObject: {Describe(fs.gameObject)}");
						var list = fs.FormationStyles;
						sb.AppendLine($"FormationStyles list: {(list == null ? "LIST IS NULL" : $"count={list.Count}")}");
						if (list != null)
						{
							for (int i = 0; i < list.Count; i++)
							{
								var e = list[i];
								sb.AppendLine($"  [{i}] fakeNull={(e == null)} refNull={object.ReferenceEquals(e, null)} {Describe(e)}");
								if (e != null)
								{
									sb.AppendLine($"       name={e.name} uniqueId={e.UniqueId} gameObject={Describe(e.gameObject)} childCount={(e.transform == null ? "TRANSFORM FAKENULL" : e.transform.childCount.ToString())}");
								}
							}
							sb.AppendLine($"DefaultFormationStyle: {Describe(fs.DefaultFormationStyle)}");
						}
					}
				}
			}
			Debug.Log(sb.ToString());
		}

		private static string Describe(Object o)
		{
			if (o == null)
				return object.ReferenceEquals(o, null) ? "REAL-NULL" : "FAKE-NULL(destroyed/missing)";
			return $"OK[{o.GetType().Name} '{o.name}' persistent={EditorUtility.IsPersistent(o)}]";
		}
	}
}
