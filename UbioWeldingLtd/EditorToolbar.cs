using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UbioWeldingLtd
{

	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class EditorToolbar : MonoBehaviour
	{
		public static EditorToolbar instance { get; private set; }

		private ApplicationLauncher _toolbar = ApplicationLauncher.Instance;
		private ApplicationLauncherButton _toolbarButton;
		private bool _isEnabled = false;

		public void initToolbar()
		{
			Debug.Log(string.Format("{0}- {1} => initToolbar", Constants.logPrefix, instance.GetType()));
			if (!this._isEnabled)
			{
				GameEvents.onGUIApplicationLauncherReady.Add(this.OnGuiAppLauncherReady);
				this._isEnabled = true;
			}
		}


		/// <summary>
		/// the public access for the toolbar
		/// </summary>
		private void Awake()
		{
			instance = this;
			Debug.Log(string.Format("{0}- {1} => Awake", Constants.logPrefix, instance.GetType()));
			if (UbioZurWeldingLtd.instance != null)
			{
				if (UbioZurWeldingLtd.instance.config.useStockToolbar)
				{
					initToolbar();
				}
			}
			//this._iconTexture = GameDatabase.Instance.GetTexture(Constants.settingIconGetPath, false);
		}


		/// <summary>
		/// initiallizes the toolbar
		/// </summary>
		private void OnGuiAppLauncherReady()
		{
			try
			{
				//if (this._iconTexture != null)
				//{
					this._toolbarButton = this._toolbar.AddModApplication(
						useToolbarButton,
						null,
						null,
						null,
						null,
						null,
						ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
						GameDatabase.Instance.GetTexture(Constants.settingIconGetPath, false)
						);
					this._isEnabled = true;
					Debug.Log(string.Format("{0}-> OnGuiAppLauncherReady done",Constants.logPrefix));
				//}
				//else
				//{
				//	Debug.Log(string.Format("{0}-> OnGuiAppLauncherReady texture missing", Constants.logPrefix));
				//}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, this);
			}
		}


		/// <summary>
		/// used when the toolbar button is clicked
		/// starts the welding process and resets the buttonstate
		/// </summary>
		private void useToolbarButton()
		{
			UbioZurWeldingLtd.instance.stockToolbarButtonUsed();
			this._toolbarButton.SetFalse();
		}


		/// <summary>
		/// removes the buttom from the toolbar
		/// </summary>
		private void OnDestroy()
		{
			GameEvents.onGUIApplicationLauncherReady.Remove(this.OnGuiAppLauncherReady);
			if (this._toolbarButton != null)
			{
				ApplicationLauncher.Instance.RemoveModApplication(this._toolbarButton);
				this._isEnabled = false;
			}
			Debug.Log("BuildToolbar->OnDestroy");
		}


		/// <summary>
		/// regular update of the toolbarbutton
		/// </summary>
		private void Update()
		{
			try
			{
				if (this._toolbarButton == null)
				{
					return;
				}

				if (EditorLogic.fetch != null && EditorLogic.fetch.ship.parts.Count > 0)
				{
					this._toolbarButton.Enable();
				}
				else
				{
					this._toolbarButton.Disable();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, this);
			}
		}


	}
}
