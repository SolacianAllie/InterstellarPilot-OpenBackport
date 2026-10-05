using System.IO;
using Pixelfactor.IP.SavedGames.V2.Model;

namespace Pixelfactor.IP.SavedGames.V2.BinarySerialization.Readers.Helpers
{
	public static class ComponentUnitCargoDataItemReader
	{
		public static ModelComponentUnitCargoDataItem Read(BinaryReader reader)
		{
			return new ModelComponentUnitCargoDataItem
			{
				CargoClass = (ModelCargoClass)reader.ReadInt32(),
				Quantity = reader.ReadInt32()
			};
		}
	}
}
