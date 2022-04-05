//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Debug UI shown for the player
//
//=============================================================================

using Controller;
using UnityEngine;

namespace UI
{
	//-------------------------------------------------------------------------
	public class DebugUI : MonoBehaviour
	{
		private PlayerController player;

		//-------------------------------------------------
		static private DebugUI _instance;
		static public DebugUI instance
		{
			get
			{
				if ( _instance == null )
				{
					_instance = GameObject.FindObjectOfType<DebugUI>();
				}
				return _instance;
			}
		}


		//-------------------------------------------------
		void Start()
		{
			player = PlayerController.instance;
		}


#if !HIDE_DEBUG_UI
        //-------------------------------------------------
        private void OnGUI()
		{
            if (Debug.isDebugBuild)
            {
                player.Draw2DDebug();
            }
        }
#endif
    }
}
