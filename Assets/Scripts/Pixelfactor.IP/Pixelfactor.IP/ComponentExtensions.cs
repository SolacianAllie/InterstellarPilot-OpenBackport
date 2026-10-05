using UnityEngine;

namespace Pixelfactor.IP
{
	public static class ComponentExtensions
	{
		public static T FindInParents<T>(this Component component) where T : Component
		{
			return UnityObjectHelper.FindInParentsOrSelf<T>(component.gameObject);
		}

		public static T FindInParents<T>(this GameObject gameObject) where T : Component
		{
			return UnityObjectHelper.FindInParentsOrSelf<T>(gameObject);
		}
	}
}
