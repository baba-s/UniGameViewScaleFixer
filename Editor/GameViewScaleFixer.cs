using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace KoganeUnityLib
{
	[InitializeOnLoad]
	internal static class GameViewScaleFixer
	{
		private static readonly Assembly     GAME_VIEW_ASSEMBLY = typeof( EditorWindow ).Assembly;
		private static readonly Type         GAME_VIEW_TYPE     = GAME_VIEW_ASSEMBLY.GetType( "UnityEditor.GameView" );
		private static readonly BindingFlags BINDING_ATTRS      = BindingFlags.Instance | BindingFlags.NonPublic;
		private static readonly Vector2      DEFAULT_SCALE      = new Vector2( 0.1f, 0.1f );

		private static bool m_isUpdate;

		static GameViewScaleFixer()
		{
			EditorApplication.update    += OnUpdate;
			EditorApplication.delayCall += OnDelayCall;

			UpdateGameViewScale();
		}

		private static void OnDelayCall()
		{
			m_isUpdate = true;
		}

		private static void OnUpdate()
		{
			if ( !m_isUpdate ) return;

			UpdateGameViewScale();
			m_isUpdate = false;
		}

		private static void UpdateGameViewScale()
		{
			var objects        = Resources.FindObjectsOfTypeAll( GAME_VIEW_TYPE );
			var gameViewWindow = objects.FirstOrDefault() as EditorWindow;

			if ( gameViewWindow == null ) return;

			var zoomAreaField = GAME_VIEW_TYPE.GetField( "m_ZoomArea", BINDING_ATTRS );

			if ( zoomAreaField == null ) return;

			var zoomArea   = zoomAreaField.GetValue( gameViewWindow );
			var scaleField = zoomArea.GetType().GetField( "m_Scale", BINDING_ATTRS );

			if ( scaleField == null ) return;

			scaleField.SetValue( zoomArea, DEFAULT_SCALE );
		}
	}
}