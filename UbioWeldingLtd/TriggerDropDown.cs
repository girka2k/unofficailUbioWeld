using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

namespace UbioWeldingLtd
{
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class TriggerDropDown : MonoBehaviour
	{



		private AdvancedDropDownManager ddlManager = new AdvancedDropDownManager();
		private Rect _window;
		private AdvancedDropDown ddlChecksPerSec;




		public void Awake()
		{
			InitDropDowns();
			RenderingManager.AddToPostDrawQueue(1, DrawGUI);
		}




		public void InitDropDowns()
		{
			String[] strChecksPerSecChoices = { "10", "20", "50", "100", "Custom" };

			ddlChecksPerSec = new AdvancedDropDown(strChecksPerSecChoices, _window);
			ddlChecksPerSec.OnSelectionChanged += ddlChecksPerSec_OnSelectionChanged;

			ddlManager.AddDropDown(ddlChecksPerSec);
		}

		private void DestroyDropDowns()
		{
			ddlChecksPerSec.OnSelectionChanged -= ddlChecksPerSec_OnSelectionChanged;
		}

		private void SetDropDownWindowPositions()
		{
			ddlChecksPerSec.windowRect = UbioZurWeldingLtd.instance.editorInfoWindow;
		}

		#region DDLEvents code

		void ddlChecksPerSec_OnSelectionChanged(AdvancedDropDown sender, int OldIndex, int NewIndex)
		{
			switch (NewIndex)
			{
				case 0:
					Debug.Log(string.Format("{0}- TriggerDropDown - {1}", Constants.logPrefix, 10));
					break;
				case 1:
					Debug.Log(string.Format("{0}- TriggerDropDown - {1}", Constants.logPrefix, 20));
					break;
				case 2:
					Debug.Log(string.Format("{0}- TriggerDropDown - {1}", Constants.logPrefix, 50));
					break;
				case 3:
					Debug.Log(string.Format("{0}- TriggerDropDown - {1}", Constants.logPrefix, 100));
					break;
				default:
					Debug.Log(string.Format("{0}- TriggerDropDown - {1}", Constants.logPrefix, 40));
					break;
			}
		}

		#endregion

		private void DrawWindowsPre()
		{
			ddlManager.DrawBlockingSelectors();
		}

		private void DrawWindowsPost()
		{
			ddlManager.DrawDropDownLists();
		}

		private void DrawWindows()
		{
			_window = GUILayout.Window(0, new Rect(Screen.width / 6, Screen.height / 6, Screen.width / 6, Screen.height / 6), FillWindow, "Test window", UbioZurWeldingLtd.instance.guistyle);
		}

		private void FillWindow(int WindowID)
		{
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			ddlChecksPerSec.DrawButton();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		public void DrawGUI()
		{
			DrawWindowsPre();
			DrawWindows();
			DrawWindowsPost();
		}

	}

}
