using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

namespace UbioWeldingLtd
{
	public class TriggerDropDown
	{

		internal AdvancedDropDownManager ddlManager = new AdvancedDropDownManager();

		private AdvancedDropDown ddlChecksPerSec;
		private AdvancedDropDown ddlSettingsSkin;
		private AdvancedDropDown ddlSettingsButtonStyle;
		private AdvancedDropDown ddlSettingsAlarmSpecs;
		private AdvancedDropDown ddlSettingsContractAutoOffered;
		private AdvancedDropDown ddlSettingsContractAutoActive;

		internal void InitDropDowns()
		{
			String[] strChecksPerSecChoices = { "10", "20", "50", "100", "Custom" };

			ddlChecksPerSec = new AdvancedDropDown(strChecksPerSecChoices, UbioZurWeldingLtd.instance.editorInfoWindow);
			ddlChecksPerSec.OnSelectionChanged += ddlChecksPerSec_OnSelectionChanged;

			ddlManager.AddDropDown(ddlChecksPerSec);
		}

		internal void DestroyDropDowns()
		{
			ddlChecksPerSec.OnSelectionChanged -= ddlChecksPerSec_OnSelectionChanged;
		}

		internal void SetDropDownWindowPositions()
		{
			ddlChecksPerSec.windowRect = UbioZurWeldingLtd.instance.editorInfoWindow;
		}

		#region DDLEvents code

		void ddlChecksPerSec_OnSelectionChanged(AdvancedDropDown sender, int OldIndex, int NewIndex)
		{
			switch (NewIndex)
			{
				case 0:
					settings.BehaviourChecksPerSec = 10;
					break;
				case 1:
					settings.BehaviourChecksPerSec = 20;
					break;
				case 2:
					settings.BehaviourChecksPerSec = 50;
					break;
				case 3:
					settings.BehaviourChecksPerSec = 100;
					break;
				default:
					settings.BehaviourChecksPerSec = settings.BehaviourChecksPerSec_Custom;
					break;
			}
		}

		#endregion

		internal void DrawWindowsPre()
		{
			ddlManager.DrawBlockingSelectors();
		}

		internal void DrawWindowsPost()
		{
			ddlManager.DrawDropDownLists();
		}

	}

}
