using System;
using UnityEngine;

namespace Pixelfactor.IP
{
	public class BitMaskAttribute : PropertyAttribute
	{
		public Type propType;

		public BitMaskAttribute(Type aType)
		{
			propType = aType;
		}
	}
}
