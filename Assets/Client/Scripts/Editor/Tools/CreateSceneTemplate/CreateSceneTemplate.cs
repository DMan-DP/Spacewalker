using UnityEditor;
using UnityEngine;

namespace Client.Tools
{
	public class CreateSceneTemplate : MonoBehaviour
	{
		private static readonly string[] TemplateEmptyObjectName =
		{
			"------ GENERAL ------",
			// Игровые менеджеры, Логика, Event system
			"------       UI        ------",
			// Объекты Canvas и все UI менеджеры
			"------   PLAYER  ------",
			// Объекты, связанные с персонажем
			"------    LIGHT    ------",
			// Все источники света
			"------    FX    ------",
			// Все спецэффекты
			"------    LEVEL    ------"
			// Все объекты уровня 
		};

		[MenuItem("Client/Tools/Scene Preparing/Create template empty objects")]
		public static void CreateSceneTemplateEmptyObjects()
		{
			var sceneObjects = new GameObject[TemplateEmptyObjectName.Length];
			var _allObjetsFind = true;

			for (var i = 0; i < TemplateEmptyObjectName.Length; i++)
			{
				sceneObjects[i] = GameObject.Find(TemplateEmptyObjectName[i]);
				if (sceneObjects[i] == null) _allObjetsFind = false;
			}

			if (_allObjetsFind)
			{
				Debug.Log("All objects have already been added to the scene");
				return;
			}

			for (var i = 0; i < TemplateEmptyObjectName.Length; i++)
				if (sceneObjects[i] == null)
				{
					var newObject = new GameObject(TemplateEmptyObjectName[i]);
				}

			Debug.Log("All objects will be created");
		}
	}
}