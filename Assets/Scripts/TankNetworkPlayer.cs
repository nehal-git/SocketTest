using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Pixel.Inc
{
	public class TankNetworkPlayer : MonoBehaviour
	{
		public TankNetworkPlayerObject player = new TankNetworkPlayerObject();
		public TankController controller;
		public Text nameText;
		IEnumerator Start()
		{
			yield return null;

			nameText.text = player.id;
		}

		private void FixedUpdate()
		{
			if (controller.isMoving && player.isPlayer)
			{
				player.position = transform.position;
				player.rotation= transform.rotation.eulerAngles;
				string playerJson = JsonUtility.ToJson(player);
				NetworkManager.instance.SendTankPos(playerJson);
			}
			
		}

	}
	[System.Serializable]
	public class TankNetworkPlayerObject
	{
		public string id;
		public string name;
		public bool isPlayer;
		public Vector3 position;
		public Vector3 rotation;
		public GameObject playerOb;
	}
}
