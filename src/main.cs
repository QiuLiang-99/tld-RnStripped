global using Il2Cpp;

using UnityEngine.SceneManagement;
using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace ttrRnStripped
{
	public static class BuildInfo
	{
		public const string Name = "RnStripped"; // Name of the Mod.  (MUST BE SET)
		public const string Description = "Some functionality ported out of Relentless Nighs."; // Description for the Mod.  (Set as null if none)
		public const string Author = "ttr"; // Author of the Mod.  (MUST BE SET)
		public const string Company = null; // Company that made the Mod.  (Set as null if none)
		public const string Version = "0.3.0"; // Version of the Mod.  (MUST BE SET)
		public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
	}
	internal class ttrRnStripped : MelonMod
	{
		public override void OnInitializeMelon()
		{
			Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
			Settings.OnLoad();
		}
	}
    // https://github.com/ShieldHeart1/RelentlessNight/blob/master/Utility/Utilities.cs
    internal class Utilities
	{
        internal static void PlayGameErrorAudio()
        {
            GameAudioManager.PlaySound(GameAudioManager.Instance.m_ErrorAudio, GameAudioManager.Instance.gameObject);
        }
        internal static void DisallowActionWithModMessage(string message)
        {
            PlayGameErrorAudio();
            HUDMessage.AddMessage(message, false);
        }
        internal static void ModLog(string message)
        {
            MelonLoader.MelonLogger.Msg(ConsoleColor.Cyan, "RnStrip > " + message);
        }
        internal static List<GameObject> GetRootObjects()
        {
            List<GameObject> rootObj = new List<GameObject>();
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            GameObject[] sceneObj = scene.GetRootGameObjects();
            foreach (GameObject obj in sceneObj)
            {
                rootObj.Add(obj);
            }
            return rootObj;
        }

        internal static void GetChildrenWithNameArray(GameObject obj, string[] lookup, Dictionary<int, GameObject> found)
        {
            if (obj.transform.childCount > 0)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    GameObject child = obj.transform.GetChild(i).gameObject;
                    if (lookup.Any(child.name.ToLower().Contains) && !found.ContainsKey(child.GetInstanceID()))
                    {
                        found.Add(child.GetInstanceID(), child);
                    }
                    GetChildrenWithNameArray(child, lookup, found);
                }
            }
        }
    }
}