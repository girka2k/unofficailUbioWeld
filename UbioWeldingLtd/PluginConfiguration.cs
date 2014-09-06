using System;
using System.Collections.Generic;
using System.Linq;

namespace UbioWeldingLtd
{
	public class PluginConfiguration
	{
		private int _editorButtonXPosition = 190;
        private int _editorButtonYPosition = 100;
		private bool _dataBaseAutoReload = false;
		private bool _includeAllNodes = false;
		private bool _allowCareerMode = true;
		private bool _dontProcessMasslessParts = true;
		private bool _runInTestMode = true;
		private bool _useStockToolbar = true;

		public int editorButtonX
		{
            get { return _editorButtonXPosition; }
            set { _editorButtonXPosition = value; }
		}

		public int editorButtonY
		{
            get { return _editorButtonYPosition; }
            set { _editorButtonYPosition = value; }
		}

		public bool dataBaseAutoReload
		{
			get { return _dataBaseAutoReload; }
			set { _dataBaseAutoReload = value; }
		}

		public bool includeAllNodes
		{
			get { return _includeAllNodes; }
			set { _includeAllNodes = value; }
		}

		public bool allowCareerMode
		{
			get { return _allowCareerMode; }
			set { _allowCareerMode = value; }
		}

		public bool dontProcessMasslessParts
		{
			get { return _dontProcessMasslessParts; }
			set { _dontProcessMasslessParts = value; }
		}

		public bool runInTestMode
		{
			get { return _runInTestMode; }
			set { _runInTestMode = value; }
		}

		public bool useStockToolbar
		{
			get { return _useStockToolbar; }
			set { _useStockToolbar = value; }
		}
	}
}
