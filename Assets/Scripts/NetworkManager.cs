using UnityEngine;
using System.Collections;
using KyleDulce.SocketIo;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Pixel.Inc
{
	public class NetworkManager : MonoBehaviour
	{
		public TankNetworkPlayerObject player = new TankNetworkPlayerObject();
		public static NetworkManager instance;
		public List<TankNetworkPlayerObject> tankNetworkPlayerObjects = new List<TankNetworkPlayerObject>();

		public Dictionary<string, TankNetworkPlayerObject> tankNetworkPlayersDictionary = new Dictionary<string, TankNetworkPlayerObject>();
		public List<Transform> spawnPoints = new List<Transform>();
		public GameObject PlayerPrefab;
		public GameObject TankPrefabParent;
		public Text text;
		public string url;
		Socket io;


		private void Awake()
		{
			instance = this;
		}
		IEnumerator Start()
		{
			yield return null;
			io = SocketIo.establishSocketConnection(url).connect();
			//s.connect();
			Debug.Log(io);
			io.on("on", OnConnect);
			io.on("spawn", OnSpawn);
			io.on("transform", OnTransform);
			io.on("disconnected", OnDisconnect);
		}

		private void OnConnect(string s)
		{
			Debug.Log(s);

			player.id = io.id.ToString();
			player.name = "Default" + io.id.ToString();
			text.text ="Your ID:"+ player.id;
			int i = Random.Range(0, spawnPoints.Count);
			player.position = spawnPoints[i].position;
			player.rotation = spawnPoints[i].rotation.eulerAngles;

			string playerJson = JsonUtility.ToJson(player);
			io.emit("spawn", playerJson);
		}
		private void OnSpawn(string s)
		{
			TankNetworkPlayerObject playerObject = JsonUtility.FromJson<TankNetworkPlayerObject>(s);
			Debug.Log("Spawning " + playerObject.id);

			bool isSpawned = tankNetworkPlayerObjects.Find(x => x.id == playerObject.id) != null;
			if (isSpawned)
				return;
			Debug.Log("Spawning1 " + playerObject.id);
			GameObject go = Instantiate(PlayerPrefab);
			go.transform.SetParent(TankPrefabParent.transform);
			playerObject.playerOb = go;

			go.transform.position= playerObject.position;
			go.transform.eulerAngles = playerObject.rotation; 
			if (playerObject.id == player.id)
			{
				player.isPlayer = true;
				player.playerOb = go;
				playerObject.isPlayer = player.isPlayer;
			}
			go.GetComponent<TankNetworkPlayer>().player = playerObject;
			if (playerObject.id != player.id)
			{
				player.isPlayer= false;
				player.position= player.playerOb.transform.position;
				player.rotation= player.playerOb.transform.eulerAngles;
				string playerJson = JsonUtility.ToJson(player);
				io.emit("spawn", playerJson);
				player.isPlayer = true;
			}

			tankNetworkPlayerObjects.Add(playerObject);
			tankNetworkPlayersDictionary.Add(playerObject.id, playerObject);
		}

		private void OnTransform(string data)
		{
			TankNetworkPlayerObject playerObject = JsonUtility.FromJson<TankNetworkPlayerObject>(data);

			tankNetworkPlayersDictionary[playerObject.id].position = playerObject.position;
			tankNetworkPlayersDictionary[playerObject.id].rotation = playerObject.rotation;

			tankNetworkPlayersDictionary[playerObject.id].playerOb.transform.position = tankNetworkPlayersDictionary[playerObject.id].position;
			tankNetworkPlayersDictionary[playerObject.id].playerOb.transform.eulerAngles= tankNetworkPlayersDictionary[playerObject.id].rotation;

		}
		public void SendTankPos(string transform)
		{
			io.emit("transform", transform);


		}

		void OnDisconnect(string data)
		{
			TankNetworkPlayerObject playerObject = JsonUtility.FromJson<TankNetworkPlayerObject>(data);

			tankNetworkPlayerObjects.ForEach((player) => {
			
				if (player.id==playerObject.id)
				{
					Destroy(player.playerOb);
					tankNetworkPlayerObjects.Remove(playerObject);
					tankNetworkPlayersDictionary.Remove(playerObject.id);
				}
			
			});
			
			

		}
		private void OnApplicationQuit()
		{
			string playerJson = JsonUtility.ToJson(player);
			
			io.emit("disconnected", playerJson);

		}
	}

}
