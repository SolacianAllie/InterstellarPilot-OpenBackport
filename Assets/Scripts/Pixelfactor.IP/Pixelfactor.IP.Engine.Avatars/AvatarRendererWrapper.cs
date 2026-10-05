using Pixelfactor.IP.Avatars;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Avatars
{
	public class AvatarRendererWrapper : MonoBehaviour
	{
		private Person person;

		public AvatarRenderer AvatarRenderer;

		public void SetPerson(Person person)
		{
			if (person != this.person)
			{
				this.person = person;
				GameController.Instance.AvatarController.UpdateAvatarRenderer(AvatarRenderer, this.person);
			}
		}
	}
}
