using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System.Collections.Generic;

public class Chat : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		Connect();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/// 
	/// The message which store current input text.
	/// 
	string user_name = "";
	string message = "";
	/// 
	/// The list of chat message.
	/// 
	List<string> messages = new List<string>();
	
	/// 
	/// Raises the GU event.
	///
	/// 
	void OnGUI(){
		
		// Input text
		user_name = GUI.TextArea(new Rect(0,10,Screen.width * 0.2f,Screen.height / 10),user_name);
		// Input text
		message = GUI.TextArea(new Rect(Screen.width * 0.2f,10,Screen.width * 0.55f,Screen.height / 10),message);
		
		// Send message button
		if(GUI.Button(new Rect(Screen.width * 0.8f,10,Screen.width * 0.2f,Screen.height / 10),"Send")){
			SendChatMessage();
		}
		
		// Show chat messages
		var l = string.Join("\n",messages.ToArray());
		var height = Screen.height * 0.1f * messages.Count;
		GUI.Label(
			new Rect(0,Screen.height * 0.1f + 10,Screen.width,height),
			l);
		
	}
	
	WebSocket ws;
	
	void Connect(){
		int id = (int)((1.0+Random.value)*0x10000);
		ws =  new WebSocket("ws://localhost:3000/websocket");
		
		// called when websocket messages come.
		ws.OnMessage += (sender, e) =>
		{
			//JSONObjectで解析
			JSONObject json = new JSONObject(e.Data);
			switch(json[0][0].str){
			case "new_message":
				messages.Add(string.Format("> {0}:{1}",json[0][1]["data"]["name"].str,
				                           json[0][1]["data"]["body"].str));
				if(messages.Count > 10){
					messages.RemoveAt(0);
				}
				break;
			case "websocket_rails.ping":
				Debug.Log(string.Format("Send: [\"websocket_rails.pong\",{{\"id\":{0},\"data\":{{}}}}]", id));
				ws.Send(string.Format("[\"websocket_rails.pong\",{{\"id\":{0},\"data\":{{}}}}]", id));
				this.message = "";
				break;
			}
			Debug.Log("Receive: " + e.Data);
		};
		
		ws.Connect();
		Debug.Log("Connect to: " + ws.Url);
	}
	
	void SendChatMessage(){
		int id = (int)((1.0+Random.value)*0x10000);
		Debug.Log(string.Format("Send: [\"new_message\",{{\"id\":{0},\"data\":{{\"name\":\"{1}\",\"body\":\"{2}\"}}}}]", id, user_name, message));
		ws.Send(string.Format("[\"new_message\",{{\"id\":{0},\"data\":{{\"name\":\"{1}\",\"body\":\"{2}\"}}}}]", id, user_name, message));
		this.message = "";
	}
}