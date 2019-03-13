using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEditor;
using UnitySerializing;
using UnityEngine;
using UnityEngine.UI;
using Formatting = Newtonsoft.Json.Formatting;

public class UiCodeGenerator : MonoBehaviour
{
	public GameObject coreLayoutContainer;
	public GameObject toCopyTest;
	
	// Use this for initialization
	void Start ()
	{
		Generate();
	}

	private void Generate()
	{
		if (coreLayoutContainer == null)
		{
			Debug.LogError("coreLayoutContainer == null");
			return;
		}

		var converters = new List<JsonConverter>();
//		GetComponent<Image>().type
		converters.Add(new EnumSerializer());
		converters.Add(new ColorSerializer());
		converters.Add(new RectSerializer());
		
		converters.Add(new RectTransformSerializer());
		converters.Add(new VectorSerializer());
		converters.Add(new ComponentSerializer<Image>(new[] {"raycastTarget", "sprite", "preserveAspect", "type", "fillCenter"}, 
			new Dictionary<string, Func<Image, object>>
			{
				["sprite"] = image =>
				{
					var assetPath = AssetDatabase.GetAssetPath(image.sprite);
					if (assetPath == "Resources/unity_builtin_extra")
					{
						assetPath += "|" + image.sprite.name;
					}
					return assetPath;
				}
			}, 
			new Dictionary<string, Action<Image, object>>
			{
				["sprite"] = (image, o) =>
				{
					var assetPath = (string) o;
					if (assetPath.Contains("Resources/unity_builtin_extra"))
					{
						var pathParts = assetPath.Split('|');
						if (pathParts.Length != 2)
						{
							Debug.LogError($"bad path: {assetPath}");
							return;
						}
						
						var builtinresname = pathParts[1];
						image.sprite = AssetDatabase.GetBuiltinExtraResource(typeof(Sprite), $"UI/Skin/{builtinresname}.psd") as Sprite;
					}
					else
					{
						image.sprite = Resources.Load<Sprite>(assetPath.Replace(".png", "").Replace("Assets/Resources/", ""));						
					}
					 
				}
			})
		);
		
		converters.Add(new ComponentSerializer<Text>(new[] {"raycastTarget", "text", "fontSize", "lineSpacing", "supportRichText", "alignment", 
				"alignByGeometry", "horizontalOverflow", "verticalOverflow", "resizeTextForBestFit", "font", "color"}, 
			new Dictionary<string, Func<Text, object>>
			{
				["font"] = text => { return text.font.name; }
			}, 
			new Dictionary<string, Action<Text, object>>
			{
				["font"] = (text, o) =>
				{
					var fontstr = (string) o;
					Font font = null;
					if (fontstr == "Arial")
						font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
					
					text.font = font;
				}
			})
		);

		converters.Add(new ComponentSerializer<Button>(new [] {"interactable", "transition"},
			new Dictionary<string, Func<Button, object>>
			{
//				["targetGraphic"] = button => "inner"
			}, 
			new Dictionary<string, Action<Button, object>>
			{
//				["targetGraphic"] = (button, o) =>
//				{
//					if ((string) o == "inner")
//						button.targetGraphic = button.gameObject.GetComponent<Image>();
//				}
			}));
		
//		converters.Add(new ComponentSerializer<Button>(new [] {""},
//			new Dictionary<string, Func<Button, object>>
//			{
//				
//			}, 
//			new Dictionary<string, Action<Button, object>>
//			{
//				
//			}));
		
		var settings = new JsonSerializerSettings();
		//settings.TraceWriter = new MyTraceViewer();
		settings.Converters = converters;
		
		string json = "zhopa";
		using (StreamWriter file = File.CreateText(@"c:\Test\test.txt"))
		{
			try
			{
				var toTest = coreLayoutContainer.GetComponent<RectTransform>();
				json = JsonConvert.SerializeObject(toTest, Formatting.Indented, settings);
			}
			catch (Exception e)
			{
				Debug.LogError(string.Format("zhopaInserialize: {0}", e));
			}
			
			file.Write(json);
		}
		
		coreLayoutContainer.SetActive(false);
		if (json != "zhopa")
		{
			var zhopaObject = JsonConvert.DeserializeObject<RectTransform>(json, settings);
			if (zhopaObject != null)
				zhopaObject.transform.SetParent(toCopyTest.transform);
		}
	}
	
}