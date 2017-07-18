// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Lerp", "Operators", "Linear interpolation of two scalars or vectors based on a weight", null, KeyCode.L )]
	public sealed class LerpOp : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_textLabelWidth = 55;
			m_previewShaderGUID = "34d9c4cdcf1fadb49af2de3f90bbc57d";
		}
		override protected void AddPorts()
		{
			base.AddPorts();
			AddInputPort( WirePortDataType.FLOAT, false, "Alpha" );
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			string interp = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, true );
			string result = "lerp( " + m_inputA + " , " + m_inputB + " , " + interp + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
