using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UbioWeldingLtd
{
	public class AdvancedDropDownManager : List<AdvancedDropDown>
	{


		public AdvancedDropDownManager()
		{
		}


		public void AddDropDown(AdvancedDropDown NewDropDown)
		{
			this.Add(NewDropDown);
			NewDropDown.OnListVisibleChanged += NewDDL_OnListVisibleChanged;
		}


		public void NewDDL_OnListVisibleChanged(AdvancedDropDown sender, bool VisibleState)
		{
			if (VisibleState)
			{
				foreach (AdvancedDropDown ddlTemp in this)
				{
					if (sender != ddlTemp)
						ddlTemp.ListVisible = false;
				}
			}
		}


		public void DrawBlockingSelectors()
		{
			foreach (AdvancedDropDown tempDropDown in this)
			{
				tempDropDown.DrawBlockingSelector();
			}
		}


		public void DrawDropDownLists()
		{
			foreach (AdvancedDropDown tempDropDown in this)
			{
				tempDropDown.DrawDropDown();
			}
		}


		public void CloseOnOutsideClicks()
		{
			foreach (AdvancedDropDown tempDropDown in this)
			{
				tempDropDown.CloseOnOutsideClick();
			}
		}


		public void SetListBoxOffset(Vector2 WrapperOffset)
		{
			foreach (AdvancedDropDown tempDropDown in this)
			{
				tempDropDown.SetListBoxOffset(WrapperOffset);
			}
		}


		public StyledGUIContent DropDownGlyphs
		{
			set
			{
				foreach (AdvancedDropDown tempDropDown in this)
				{
					tempDropDown.dropDownGlyph = value;
				}
			}
		}


		public StyledGUIContent DropDownSeparators
		{
			set
			{
				foreach (AdvancedDropDown tempDropDown in this)
				{
					tempDropDown.dropDownSeparator = value;
				}
			}
		}

	}
}
