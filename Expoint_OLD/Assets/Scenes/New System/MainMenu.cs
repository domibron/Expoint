using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System;

namespace Expoint.NewSytem
{
	public class MainMenu : MonoBehaviour
	{
		// Network
		[SerializeField] NetworkRoomManager _manager;

		[SerializeField] TMP_InputField _inputField;

		string code;

		public void OnValueChanged()
		{
			code = _inputField.text;
		}

		public void HostGame()
		{
			_manager.networkAddress = GetGlobalIPAddress();

			_manager.StartHost();
		}

		public void ConnectToServer()
		{
			_manager.networkAddress = code;
			_manager.StartClient();
		}

		private string GetGlobalIPAddress()
		{
			var url = "https://api.ipify.org/";

			WebRequest request = WebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			Stream dataStream = response.GetResponseStream();

			using StreamReader reader = new StreamReader(dataStream);

			var ip = reader.ReadToEnd();
			reader.Close();

			return ip;
		}
	}
}
