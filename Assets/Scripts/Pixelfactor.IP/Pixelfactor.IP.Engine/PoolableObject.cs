using System;
using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public class PoolableObject : MonoBehaviour
	{
		[NonSerialized]
		public int PrefabId = -1;
	}
}
