using System;
using System.Collections.Generic;
using UnityEngine;

namespace UbioWeldingLtd
{
	public static class WeldingHelpers
	{

        /// <summary>
        /// prepares the Categories for the saveing window
        /// </summary>
        /// <param name="inputList"></param>
        /// <param name="inputDropDown"></param>
        /// <param name="inputGUIStyle"></param>
        public static List<GUIContent> initPartCategories(List<GUIContent> inputList)
		{
			//inputList = new List<GUIContent>();
			List<string> catlist = new List<string>(System.Enum.GetNames(typeof(PartCategories)));
			catlist.Remove(PartCategories.none.ToString());
			foreach (string cat in catlist)
			{
				inputList.Add(new GUIContent(cat));
			}
            return inputList;
        }

        public static GUIStyle initGuiStyle(GUIStyle inputGUIStyle)
        {
			//inputGUIStyle = new GUIStyle();
			inputGUIStyle.normal.textColor = Color.white;
			inputGUIStyle.onHover.background =
			inputGUIStyle.hover.background = new Texture2D(2, 2);
			inputGUIStyle.padding.left =
			inputGUIStyle.padding.right =
			inputGUIStyle.padding.top =
			inputGUIStyle.padding.bottom = 1;
            return inputGUIStyle;
        }

        public static GUIDropdown initDropDown(List<GUIContent> categoryList, GUIStyle guiStyle,GUIDropdown dropDown)
        {
            dropDown = new GUIDropdown(categoryList[0], categoryList.ToArray(), "button", "box", guiStyle);
            return dropDown;
		}
	}
}
