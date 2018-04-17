using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData {
	[SerializeField]
	public static Vector3 playerPos = new Vector3(10, 0, 0);
	//TODO add more Data
}

[System.Serializable]
public class SaveLoad {
	public static void Save() {
		GameData.playerPos = new Vector3(12, 0, 0);
		BinaryFormatter bin = new BinaryFormatter();
		using(FileStream fs = new FileStream("./Assets/SaveGames/saveGame.bin", FileMode.Create, FileAccess.Write)) {
			bin.Serialize(fs, GameData.playerPos);
		}
	}
	
	public static void Load() {
		if(!File.Exists("saveGame.bin")) {
			return;
		}
		
		BinaryFormatter bin = new BinaryFormatter();
		using(FileStream fs = new FileStream("./Assets/SaveGames/saveGame.bin", FileMode.Open, FileAccess.Read)) {
			GameData.playerPos = (Vector3) bin.Deserialize(fs);
		}
	}
}