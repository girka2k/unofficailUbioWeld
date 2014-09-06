using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using KSP.IO;

namespace UbioWeldingLtd
{
	//UbioZurWeldingLtd Class
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class UbioZurWeldingLtd : MonoBehaviour
	{
		enum DisplayState
		{
			none,
			weldError,
			weldWarning,
			infoWindow,
			savedWindow,
			overwriteDial
		}

		private Rect _editorButton;
		private Rect _editorErrorDial;
		private Rect _editorWarningDial;
		private Rect _editorInfoWindow;
		private Rect _editorOverwriteDial;
		private Rect _editorSavedDial;
		private Welder _welder;
		private DisplayState _state;
		private List<GUIContent> _catNames = new List<GUIContent>();
        private GUIDropdown _catDropdown;
		private GUIStyle _catListStyle = new GUIStyle();
		private Vector2 _scrollRes = Vector2.zero;
		private Vector2 _scrollMod = Vector2.zero;

		private PluginConfiguration _config;
		private Texture2D _iconTexture;
		private ApplicationLauncherButton _stockToolbarButton;
		private bool _guiVisible = false;


		/*
		 * Called when plug in loaded
		 */
		public void Awake()
		{
			initConfig();

			_state = DisplayState.none;

			RenderingManager.AddToPostDrawQueue(0, OnDraw);

			if (_config.useStockToolbar)
			{
				if (_iconTexture == null)
				{
					_iconTexture = new Texture2D(38, 38, TextureFormat.RGBA32, false);
					_iconTexture.LoadImage(System.IO.File.ReadAllBytes(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Constants.settingIconFileName)));
				}
			}
			else
			{
				_editorButton = new Rect(Screen.width - _config.editorButtonX, _config.editorButtonY, Constants.guiWeldButWidth, Constants.guiWeldButHeight);
			}
			_editorErrorDial = new Rect(Screen.width / 2 - Constants.guiDialogX, Screen.height / 2 - Constants.guiDialogY, Constants.guiDialogW, Constants.guiDialogH);
			_editorWarningDial = new Rect(Screen.width / 2 - Constants.guiDialogX, Screen.height / 2 - Constants.guiDialogY, Constants.guiDialogW, Constants.guiDialogH);
			_editorInfoWindow = new Rect(Screen.width / 2 - Constants.guiInfoWindowX, Screen.height / 2 - Constants.guiInfoWindowY, Constants.guiInfoWindowW, Constants.guiInfoWindowH);
			_editorOverwriteDial = new Rect(Screen.width / 2 - Constants.guiDialogX, Screen.height / 2 - Constants.guiDialogY, Constants.guiDialogW, Constants.guiDialogH);
			_editorSavedDial = new Rect(Screen.width / 2 - Constants.guiDialogX, Screen.height / 2 - Constants.guiDialogY, Constants.guiDialogW, Constants.guiDialogH);
		}


        /// <summary>
        /// Welds the whole active craft in the scene in case the stocktoolbar is used
        /// </summary>
		private void stockToolbarButtonUsed()
		{
			if (!EditorLogic.softLock)
			{
                if (EditorLogic.startPod != null)
                {
                    weldPart(EditorLogic.startPod);
                }
			}
			_stockToolbarButton.SetFalse();
		}


        /// <summary>
        /// Loads the config for the Welding or prepares default values and generates a new config
        /// </summary>
		private void initConfig()
		{
            if (!System.IO.File.Exists(string.Concat(Constants.settingRuntimeDirectory, Constants.settingCompleteConfigFilePath)))
			{
				FileManager.saveConfig(new PluginConfiguration());
				_config = new PluginConfiguration();
			}
			else
			{
				_config = FileManager.loadConfig(new PluginConfiguration());
			}
			Welder.includeAllNodes = _config.includeAllNodes;
			Welder.dontProcessMasslessParts = _config.dontProcessMasslessParts;
			Welder.runInTestMode = _config.runInTestMode;
		}


        /// <summary>
        /// Prepares the Stocktoolbar with the Icon or enables the old style button
        /// </summary>
		private void initGUI()
		{
			if (null == EditorLogic.fetch || (!_config.allowCareerMode && HighLogic.fetch.currentGame.Mode == Game.Modes.CAREER))
			{
				_guiVisible = false;
			}
			else
			{
				if (_config.useStockToolbar)
				{
					_stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(stockToolbarButtonUsed, null, null, null, null, null, ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, _iconTexture);
				}
				_guiVisible = true;
			}
		}

		/*
		 * Called once everything in scene is loaded
		 */
		public void Start()
		{
            _catNames = WeldingHelpers.initPartCategories(_catNames);
            _catListStyle = WeldingHelpers.initGuiStyle(_catListStyle);
            _catDropdown = WeldingHelpers.initDropDown(_catNames, _catListStyle, _catDropdown);
            initGUI();
		}


        /// <summary>
        /// called once the item gets destroyed
        /// makes clear that the button is removed from the toolbar
        /// </summary>
        public void OnDestroy()
        {
            if (_stockToolbarButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(_stockToolbarButton);
                _stockToolbarButton = null;
            }
        }

		/*
		 * To draw the UI
		 */
		private void OnDraw()
		{
			if (_guiVisible)
			{
				//Set the GUI Skin
				GUI.skin = HighLogic.Skin;

				switch (_state)
				{
					case DisplayState.none:
						if (!_config.useStockToolbar)
						{
							OnWeldButton();
						}
						break;
					case DisplayState.weldError:
						_editorErrorDial = GUILayout.Window(1, _editorErrorDial, OnErrorDisplay, Constants.weldManufacturer);
						break;
					case DisplayState.weldWarning:
						_editorWarningDial = GUILayout.Window(2, _editorWarningDial, OnWarningDisplay, Constants.weldManufacturer);
						break;
					case DisplayState.infoWindow:
						_editorInfoWindow = GUI.Window(3, _editorInfoWindow, OnInfoWindow, Constants.weldManufacturer);
						break;
					case DisplayState.savedWindow:
						_editorSavedDial = GUILayout.Window(4, _editorSavedDial, OnSavedDisplay, Constants.weldManufacturer);
						break;
					case DisplayState.overwriteDial:
						_editorOverwriteDial = GUILayout.Window(5, _editorOverwriteDial, OnOverwriteDisplay, Constants.weldManufacturer);
						break;
				}
			}
		} //private void OnDraw()


		private void weldPart(Part partToWeld )
		{
			//Lock editor
			EditorLogic.fetch.Lock(true, true, true, "UBILOCK9213");

			//process the welding
#if (DEBUG)
			Debug.ClearDeveloperConsole();
#endif
			Debug.Log(string.Format("{0}{1}", Constants.logPrefix, Constants.logVersion));
			Debug.Log(string.Format("{0}{1}", Constants.logPrefix, Constants.logStartWeld));

			bool warning = false;
			_welder = new Welder();

            WeldingReturn ret = _welder.weldThisPart(partToWeld);
			if (0 > ret)
			{
				Debug.Log(string.Format("{0}{1}", Constants.logPrefix, Constants.logEndWeld));
				_state = DisplayState.weldError;
				return;
			}
			else
			{
				warning = warning || (0 < ret);
			}

            Part[] children = partToWeld.FindChildParts<Part>(true);
			if (null != children)
			{
				foreach (Part child in children)
				{
					ret = _welder.weldThisPart(child);
					if (0 > ret)
					{
						Debug.Log(string.Format("{0}{1}", Constants.logPrefix, Constants.logEndWeld));
						_state = DisplayState.weldError;
						return;
					}
					else
					{
						warning = warning || (0 < ret);
					}
				}
			}
			_welder.processNewCoM();
			_scrollMod = Vector2.zero;
			_scrollRes = Vector2.zero;
            Debug.Log(string.Format("{0} {1} | {2} Parts welded", Constants.logPrefix, Constants.logEndWeld, _welder.NbParts));
			if (warning)
			{
				_state = DisplayState.weldWarning;
			}
			else
			{
				_catDropdown.SelectedItemIndex = (int)_welder.Category;
				_state = DisplayState.infoWindow;
			}
		}

		/*
		 * Editor Button Draw
		 */
		private void OnWeldButton()
		{
			//Making sure the editor is not on softlock or no parts are selected
			if (EditorLogic.softLock || null == EditorLogic.SelectedPart)
			{
				GUI.Box(_editorButton, Constants.guiWeldLabel);
			}
			else
			{
				if (GUI.Button(_editorButton, Constants.guiWeldLabel))
				{
					weldPart(EditorLogic.fetch.PartSelected);
				} // if (GUI.Button(_editorButton, Constants.guiWeldLabel))
			} // elsef if (EditorLogic.softLock || null == EditorLogic.SelectedPart)
		} //private void OnWeldButton()

		/*
		 * Error Message
		 */
		private void OnErrorDisplay(int windowID)
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label(Constants.guiDialFail);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(Constants.guiOK))
			{
				EditorLogic.fetch.Unlock("UBILOCK9213");
				_state = DisplayState.none;
			}
			GUILayout.EndVertical();

			GUI.DragWindow();
		} //private void OnErrorDisplay()

		/*
		 * Warning Message
		 */
		private void OnWarningDisplay(int windowID)
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label(Constants.guiDialWarn);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(Constants.guiOK))
			{
				_state = DisplayState.infoWindow;
			}
			GUILayout.EndVertical();

			GUI.DragWindow();
		} //private void OnErrorDisplay()

		/*
		 * Overwrite Message
		 */
		private void OnOverwriteDisplay(int windowID)
		{
			string filepath = string.Format("{0}{1}/{2}{3}", Constants.weldPartPath, _welder.Category.ToString(), _welder.Name, Constants.weldPartFile);
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label(Constants.guiDialOverwrite);
			GUILayout.Label(filepath);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(Constants.guiOK))
			{
				WriteCfg(filepath);
				_state = DisplayState.savedWindow;
			}
			if (GUILayout.Button(Constants.guiCancel))
			{
				_state = DisplayState.infoWindow;
			}
			GUILayout.EndVertical();

			GUI.DragWindow();
		} //private void OnErrorDisplay()

		/*
		 * Saved Message
		 */
		private void OnSavedDisplay(int windowID)
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label(Constants.guiDialSaved);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(Constants.guiOK))
			{
				ClearEditor();
				_state = DisplayState.none;
			}
			GUILayout.EndVertical();

			GUI.DragWindow();
		} //private void OnErrorDisplay()

		/*
		 * Display the info window
		 */
		private void OnInfoWindow(int windowID)
		{
			float margin = 2.5f;
			float quarterpos = Constants.guiInfoWindowW * 0.25f;
			float quarterwidth = quarterpos - margin - margin;
			float height = 20.0f;
			float inith = height;
			float posh = inith;

			//First Colomn
			GUI.Label(new Rect(margin, posh, quarterwidth, height), "Name");
			posh += height;
			_welder.Name = GUI.TextField(new Rect(margin, posh, quarterwidth, height), _welder.Name, 100);
			posh += height;
			GUI.Label(new Rect(margin, posh, quarterwidth, height), "Title");
			posh += height;
			_welder.Title = GUI.TextField(new Rect(margin, posh, quarterwidth, height), _welder.Title, 100);
			posh += height;
			GUI.Label(new Rect(margin, posh, quarterwidth, height), "Description");
			posh += height;
			_welder.Description = GUI.TextArea(new Rect(margin, posh, quarterwidth, 6 * height), _welder.Description, 200);
			posh += 6 * height;

			//Second
			posh = inith;
			float posw = quarterpos + margin;
			GUI.Label(new Rect(posw, posh, quarterwidth, height), "Category");
			posh += height;
			_catDropdown.SelectedItemIndex = (int)_welder.Category;
			int SelectedCat = _catDropdown.Show(new Rect(posw, posh, quarterwidth, height));
			_welder.Category = (PartCategories)SelectedCat;
			if (!_catDropdown.IsOpen)
			{
				posh += height;
				GUI.Label(new Rect(posw, posh, quarterwidth, height), string.Format("Nb Parts: {0}", _welder.NbParts));
				posh += height;
				GUI.Label(new Rect(posw, posh, quarterwidth, height), string.Format("Cost: {0}", _welder.Cost));
				posh += height;
				GUI.Label(new Rect(posw, posh, quarterwidth, height), string.Format("Mass: {0} / {1}", _welder.Mass, _welder.WetMass));
				posh += height;
				GUI.Label(new Rect(posw, posh, quarterwidth, height), string.Format("T: {0}", _welder.MaxTemp));
				posh += height;
				GUI.Label(new Rect(posw, posh, quarterwidth, height), string.Format("F: {0}", _welder.BreakingForce));
				posh += height;
				GUI.Label(new Rect(posw, posh, quarterwidth, height), string.Format("T: {0}", _welder.BreakingTorque));
				posh += height;
				GUI.Label(new Rect(posw, posh, quarterwidth, height), string.Format("Drag: {0} / {1}", _welder.MinDrag, _welder.MaxDrag));
			}

			//Module
			posh = inith;
			posw = (quarterpos * 2.0f) + margin;
			GUI.Label(new Rect(posw, posh, quarterwidth, height), "Modules");
			posh += height;
			float scrollwidth = quarterwidth - 20.0f;
			_scrollMod = GUI.BeginScrollView(new Rect(posw, posh, quarterwidth, 10 * height), _scrollMod, new Rect(0, 0, scrollwidth, 200.0f), false, true);
			posh = 0.0f;
			string[] modulenames = _welder.Modules;
			GUIStyle style = new GUIStyle();
			style.wordWrap = false;
			style.normal.textColor = Color.white;
			foreach (string modulename in modulenames)
			{
				GUI.Label(new Rect(0, posh, scrollwidth, height), modulename, style);
				posh += height;
			}
			GUI.EndScrollView();

			//Res
			posh = inith;
			posw = (quarterpos * 3.0f) + margin;
			GUI.Label(new Rect(posw, posh, quarterwidth, height), "Resources");
			posh += height;
			_scrollRes = GUI.BeginScrollView(new Rect(posw, posh, quarterwidth, 10 * height), _scrollRes, new Rect(0, 0, scrollwidth, 200.0f), false, true);
			posh = 0.0f;
			string[] resourcesdata = _welder.Resources;
			foreach (string resdata in resourcesdata)
			{
				GUI.Label(new Rect(0, posh, scrollwidth, height), resdata, style);
				posh += height;
			}
			GUI.EndScrollView();

			bool nameOk = WelderNameNotUsed();
			if (!nameOk)
			{
				style.normal.textColor = Color.red;
				GUI.Label(new Rect(margin, 13 * height, 2 * quarterpos, height), Constants.guiNameUsed, style);
			}
			else if (!string.IsNullOrEmpty(_welder.Name))
			{
				if (GUI.Button(new Rect(2 * quarterpos, 13 * height, quarterwidth * 0.5f, height), Constants.guiSave))
				{
					//check if the file exist
					string dirpath = string.Format("{0}{1}/{2}", Constants.weldPartPath, _welder.Category.ToString(), _welder.Name);
					string filepath = dirpath + Constants.weldPartFile;
					if (!System.IO.File.Exists(filepath))
					{
						if (!Directory.Exists(dirpath))
						{
							Directory.CreateDirectory(dirpath);
						}
						//create the file
						StreamWriter partfile = System.IO.File.CreateText(filepath);
						partfile.Close();
						WriteCfg(filepath);
						_state = DisplayState.savedWindow;
					}
					else
					{
						_state = DisplayState.overwriteDial;
					}
				}
			}
			if (GUI.Button(new Rect(3 * quarterpos, 13 * height, quarterwidth * 0.5f, height), Constants.guiCancel))
			{
				ClearEditor();
				_state = DisplayState.none;
			}

			GUI.DragWindow();
		} //private void OnWarningDisplay(int windowID)

		/*
		 * Test if the name is not already in use
		 */
		private bool WelderNameNotUsed()
		{
			foreach (UrlDir.UrlConfig part in GameDatabase.Instance.GetConfigs(Constants.weldPartNode))
			{
				if (string.Equals(part.name, _welder.Name))
				{
					return false;
				}
			}
			return true;
		}

		/*
		 * Writing the cfg File
		 */
		private void WriteCfg(string filepath)
		{
			Debug.Log(string.Format("{0}{1}{2}", Constants.logPrefix, Constants.logWritingFile, filepath));

			_welder.CreateFullConfigNode();

			_welder.FullConfigNode.Save(filepath);

			if (_config.dataBaseAutoReload)
			{
				ReloadDatabase();
			}
		}

		/*
		 * Reload the Database
		 */
		private void ReloadDatabase()
		{
			//reload database Big thanks to AncientGammoner (KSP Forum)
			GameDatabase.Instance.Recompile = true;
			GameDatabase.Instance.StartLoad();
			PartLoader.Instance.Recompile = true;
			PartLoader.Instance.StartLoad();
		}

		/*
		 * Free and clear the editor
		 */
		private void ClearEditor()
		{
            if (_config.useStockToolbar || EditorLogic.SelectedPart == null)
            {
                EditorLogic.fetch.PartSelected = EditorLogic.startPod;
            }
            EditorLogic.fetch.DestroySelectedPart();
			EditorLogic.fetch.Unlock("UBILOCK9213");
			EditorPartList.Instance.Refresh();
		}
	} //public class UbioZurWeldingLtd : MonoBehaviour
} //namespace UbioWeldingLtd
