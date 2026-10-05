using System.Linq;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.UnitComponents;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Screens
{
	public class CompatibleProjectilesScreen : ScreenBase
	{
		public CompatibleProjectileItemList AmmoTypesList;

		public Text TitleText;

		private ProjectileTurretClass turretClass;

		public ProjectileTurretClass TurretClass
		{
			get
			{
				return turretClass;
			}
			set
			{
				if (turretClass != value)
				{
					turretClass = value;
					Refresh();
				}
			}
		}

		public void PopulateCompatibleProjectiles()
		{
			AmmoTypesList.SetItems(turretClass.CompatibleProjectiles.OrderBy((ProjectileClass e) => e.AmmoClass.BasePrice));
		}

		protected override void refresh()
		{
			base.refresh();
			if (turretClass != null)
			{
				PopulateCompatibleProjectiles();
				TitleText.text = $"{turretClass.Name} - Ammo Types";
			}
		}
	}
}
