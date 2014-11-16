using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using UnityEngine;
using KSP;

namespace UbioWeldingLtd
{
	public class AdvancedDropDown
	{

		private Boolean _listVisible;
		private Rect _rectButton;
		private Rect _rectListBox;
		private GUIStyle stylePager;
		private GUIStyle _styleButton = null;
		private GUIStyle _styleListItem;
		private GUIStyle _styleListBox;
		private GUIStyle styleListBlocker = new GUIStyle();
		private StyledGUIContent _dropDownSeparator = null;
		private StyledGUIContent _dropDownGlyph = null;
		private Int32 _listItemHeight = 20;
		private RectOffset _listBoxPadding = new RectOffset(1, 1, 1, 1);
		//internal MonoBehaviourWindow Window;
		private Rect _windowRect = new Rect();
		private Int32 _listPageLength = 0;
		private Int32 _listPageNum = 0;
		private Boolean _listPageOverflow = false;
		private Boolean _listPagingRequired = false;
		private GUIStyle _styleButtonToDraw = null;
		private GUIStyle _styleListBoxToDraw = null;
		private GUIStyle _styleListItemToDraw = null;
		// This is for DropDowns inside extra containers like scrollviews - where getlastrect does not cater to the scrollview position
		private Vector2 _vectListBoxOffset = new Vector2(0, 0);

		//event for changes
		public delegate void SelectionChangedEventHandler(AdvancedDropDown sender, Int32 OldIndex, Int32 NewIndex);
		public event SelectionChangedEventHandler OnSelectionChanged;
		public delegate void ListVisibleChangedHandler(AdvancedDropDown sender, Boolean VisibleState);
		public event ListVisibleChangedHandler OnListVisibleChanged;

		public AdvancedDropDown(IEnumerable<String> Items, Int32 Selected, Rect WindowRect)
		{
			selectedIndex = Selected;
			this.items = Items.ToList<String>();
			this._windowRect = WindowRect;

			Debug.Log(string.Format("{0}- {1} | {2}", Constants.logPrefix, items.Count, _windowRect.height));
		}

		public AdvancedDropDown(IEnumerable<String> Items, Rect WindowRect)
		{
			this._windowRect = WindowRect;
			this.items = Items.ToList<String>();

			Debug.Log(string.Format("{0}- {1} | {2}", Constants.logPrefix, items.Count, _windowRect.height));
		}

		public AdvancedDropDown(Rect WindowRect)
		{
			this._windowRect = WindowRect;
			_listVisible = false;
			selectedIndex = 0;

			Debug.Log(string.Format("{0}- {1} | {2}", Constants.logPrefix, items.Count, _windowRect.height));
		}


		//properties to use
		public List<String> items
		{
			get;
			set;
		}

		public Int32 selectedIndex
		{
			get;
			set;
		}

		public String SelectedValue
		{
			get { return items[selectedIndex]; }
		}

		public Rect windowRect
		{
			get { return _windowRect; }
			set { _windowRect = value; }
		}

		public Boolean listVisible
		{
			get { return _listVisible; }
			set
			{
				_listVisible = value;
				if (_listVisible)
				{
					CalcPagesAndSizes();
				}
				if (OnListVisibleChanged != null)
				{
					OnListVisibleChanged(this, _listVisible);
				}
			}
		}

		internal void SetListBoxOffset(Vector2 WrapperOffset)
		{
			_vectListBoxOffset = WrapperOffset;
		}

		internal GUIStyle styleButton
		{
			get { return _styleButton; }
			set { _styleButton = value; }
		}

		public GUIStyle styleListItem
		{
			get { return _styleListItem; }
			set { _styleListItem = value; }
		}

		public GUIStyle styleListBox
		{
			get { return _styleListBox; }
			set { _styleListBox = value; }
		}

		public StyledGUIContent dropDownSeparator
		{
			get { return _dropDownSeparator; }
			set { _dropDownSeparator = value; }
		}

		public StyledGUIContent dropDownGlyph
		{
			get { return _dropDownGlyph; }
			set { _dropDownGlyph = value; }
		}


		//Draw the button behind everything else to catch the first mouse click
		internal void DrawBlockingSelector()
		{
			//do we need to draw the blocker
			if (listVisible)
			{
				//This will collect the click event before any other controls under the listrect
				if (GUI.Button(_rectListBox, "", styleListBlocker))
				{
					Int32 oldIndex = selectedIndex;

					if (!_listPageOverflow)
					{
						selectedIndex = (Int32)Math.Floor((Event.current.mousePosition.y - _rectListBox.y) / (_rectListBox.height / items.Count));
					}
					else
					{
						//do some maths to work out the actual index
						Int32 SelectedRow = (Int32)Math.Floor((Event.current.mousePosition.y - _rectListBox.y) / (_rectListBox.height / _listPageLength));

						if (SelectedRow == 0)
						{
							//this is the paging row...
							if (Event.current.mousePosition.x > (_rectListBox.x + _rectListBox.width - 40 - _listBoxPadding.right))
							{
								_listPageNum++;
							}
							else if (Event.current.mousePosition.x > (_rectListBox.x + _rectListBox.width - 80 - _listBoxPadding.right))
							{
								_listPageNum--;
							}
							if (_listPageNum < 0)
							{
								_listPageNum = (Int32)Math.Floor((Single)items.Count / _listPageLength);
							}
							if (_listPageNum * _listPageLength > items.Count)
							{
								_listPageNum = 0;
							}
							return;
						}
						else
						{
							selectedIndex = (_listPageNum * _listPageLength) + (SelectedRow - 1);
						}
						if (selectedIndex >= items.Count)
						{
							selectedIndex = oldIndex;
							return;
						}
					}
					//Throw an event or some such from here
					if (OnSelectionChanged != null)
					{
						OnSelectionChanged(this, oldIndex, selectedIndex);
					}
					listVisible = false;
				}
			}
		}

		//Draw the actual button for the list
		internal Boolean DrawButton()
		{
			Boolean blnReturn = false;

			if (_styleButtonToDraw == null)
			{
				blnReturn = GUILayout.Button(SelectedValue);
			}
			else
			{
				blnReturn = GUILayout.Button(SelectedValue, _styleButtonToDraw);
			}

			if (blnReturn)
			{
				listVisible = !listVisible;
			}

			//get the drawn button rectangle
			if (Event.current.type == EventType.repaint)
			{
				_rectButton = GUILayoutUtility.GetLastRect();
			}

			//draw a dropdown symbol on the right edge
			if (_dropDownGlyph != null)
			{
				Rect rectDropIcon = new Rect(_rectButton) { x = (_rectButton.x + _rectButton.width - 20), width = 20 };

				if (_dropDownSeparator != null)
				{
					Rect rectDropSep = new Rect(rectDropIcon) { x = (rectDropIcon.x - _dropDownSeparator.calculateWidth), width = _dropDownSeparator.calculateWidth };
					if (_dropDownSeparator.style == null)
					{
						GUI.Box(rectDropSep, _dropDownSeparator.content);
					}
					else
					{
						GUI.Box(rectDropSep, _dropDownSeparator.content, _dropDownSeparator.style);
					}
				}

				if (_dropDownGlyph.style == null)
				{
					GUI.Box(rectDropIcon, _dropDownGlyph.content);
				}
				else
				{
					GUI.Box(rectDropIcon, _dropDownGlyph.content, _dropDownGlyph.style);
				}

			}
			return blnReturn;
		}

		private void CalcPagesAndSizes()
		{
			//raw box size
			_rectListBox = new Rect(_rectButton)
			{
				x = _rectButton.x + _windowRect.x + _vectListBoxOffset.x,
				y = _rectButton.y + _windowRect.y + _rectButton.height + _vectListBoxOffset.y,
				height = (items.Count * _listItemHeight) + (_listBoxPadding.top + _listBoxPadding.bottom)
			};

			Debug.Log(string.Format("{0}- {1} | {2}", Constants.logPrefix, (_rectListBox.y + _rectListBox.height), (_windowRect.y + _windowRect.height)));

			//if it doesnt fit below the list
			if ((_rectListBox.y + _rectListBox.height) > (_windowRect.y + _windowRect.height))
			{
				if (_rectListBox.height < _windowRect.height - 8)
				{
					//move the top up so that the full list is visible
					_listPageOverflow = false;
					_rectListBox.y = _windowRect.y + _windowRect.height - _rectListBox.height - 4;
				}
				else
				{
					//Need multipages for this list
					_listPageOverflow = true;
					_rectListBox.y = 4;
					_rectListBox.height = (Single)(_listItemHeight * Math.Floor((_windowRect.height - 8) / _listItemHeight));
					_listPageLength = (Int32)(Math.Floor((_windowRect.height - 8) / _listItemHeight) - 1);
					_listPageNum = (Int32)Math.Floor((Double)selectedIndex / _listPageLength);
				}
			}
			else
			{
				_listPageOverflow = false;
			}

			stylePager = new GUIStyle(UbioZurWeldingLtd.instance.guiskin.label) { fontStyle = FontStyle.Italic };
		}

		//Draw the hovering dropdown
		internal void DrawDropDown()
		{
			if (listVisible)
			{
				GUI.depth = 0;

				if (_styleListBoxToDraw == null)
				{
					_styleListBoxToDraw = GUI.skin.box;
				}

				if (_styleListItemToDraw == null)
				{
					_styleListItemToDraw = GUI.skin.label;
				}

				//and draw it
				GUI.Box(_rectListBox, "", _styleListBoxToDraw);

				Int32 iStart = 0, iEnd = items.Count, iPad = 0;
				if (_listPageOverflow)
				{
					//calc the size of each page
					iStart = _listPageLength * _listPageNum;

					if (_listPageLength * (_listPageNum + 1) < items.Count)
					{
						iEnd = _listPageLength * (_listPageNum + 1);
					}

					//this moves us down a row to fit the paging buttons in the main loop
					iPad = 1;

					//Draw paging buttons
					GUI.Label(new Rect(_rectListBox) { x = _rectListBox.x + _listBoxPadding.left, height = 20 }, String.Format("Page {0}/{1:0}", _listPageNum + 1, Math.Floor((Single)items.Count / _listPageLength) + 1), stylePager);
					GUI.Button(new Rect(_rectListBox) { x = _rectListBox.x + _rectListBox.width - 80 - _listBoxPadding.right, y = _rectListBox.y + 2, width = 40, height = 16 }, "Prev");
					GUI.Button(new Rect(_rectListBox) { x = _rectListBox.x + _rectListBox.width - 40 - _listBoxPadding.right, y = _rectListBox.y + 2, width = 40, height = 16 }, "Next");
				}

				//now draw each listitem
				for (int i = iStart; i < iEnd; i++)
				{
					Rect ListButtonRect = new Rect(_rectListBox)
					{
						x = _rectListBox.x + _listBoxPadding.left,
						width = _rectListBox.width - _listBoxPadding.left - _listBoxPadding.right,
						y = _rectListBox.y + ((i - iStart + iPad) * _listItemHeight) + _listBoxPadding.top,
						height = 20
					};

					if (GUI.Button(ListButtonRect, items[i], _styleListItemToDraw))
					{
						listVisible = false;
						selectedIndex = i;
					}
					if (i == selectedIndex)
					{
						GUI.Label(new Rect(ListButtonRect) { x = ListButtonRect.x + ListButtonRect.width - 20 }, "✔");
					}
				}

				CloseOnOutsideClick();
			}

		}

		internal Boolean CloseOnOutsideClick()
		{
			if (listVisible && Event.current.type == EventType.mouseDown && !_rectListBox.Contains(Event.current.mousePosition))
			{
				listVisible = false;
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
