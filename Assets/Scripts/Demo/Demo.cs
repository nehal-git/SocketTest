using UnityEngine;
using KyleDulce.SocketIo;
using UnityEngine.UI;
using System;
using TMPro;

public class Demo : MonoBehaviour
{
	Socket s;

	public InputField inputField;

	public TextMeshProUGUI textMeshProUGUIPrefab;

	public GameObject contentParent;

	public string ID;

	public Text playerID;

	// Start is called before the first frame update
	void Start()
	{
	///http://nodesocket-example.herokuapp.com/
		s = SocketIo.establishSocketConnection("wss://nodesocket-example.herokuapp.com").connect();
		//s.connect();
		Debug.Log(s);
		s.on("recieve", rec);
		ID = s.id.ToString();
		playerID.text = $" PID: {ID}";
	}
	
	void rec(string d)
	{
		Debug.Log("RECIEVED EVENT: " + d);
		InputClass inputClass = JsonUtility.FromJson<InputClass>(d);

		GameObject go = Instantiate(textMeshProUGUIPrefab.gameObject, contentParent.transform);
		go.GetComponent<TextMeshProUGUI>().text =$" PID: {inputClass.id}- {inputClass.value}";
	}

	public void OnSendMessage()
	{
		if (!string.IsNullOrEmpty(inputField.text))
		{


			InputClass inputClass = new InputClass();
			inputClass.id = ID.ToString();
			inputClass.value = inputField.text.ToString();
			inputField.text = "";
			string str = JsonUtility.ToJson(inputClass);
			s.emit("send", str);
		}
	}
}

[Serializable]
public class InputClass
{
	public string id;
	public string value;
	
}
