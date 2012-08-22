#pragma once
#include "FbxAxis.h"
#include "FbxVersion.h"
#include "FbxSystemUnits.h"
#include "FbxFileFormat.h"

namespace Skill
{
	namespace Fbx
	{
		public value class SaveSceneOptions
		{
		public:

			FbxVersion Version;			
			FbxFileFormat FileFormat;
			FbxSystemUnits Units;
			FbxAxis Axis;
			bool SmoothingGroups;        
			bool TangentsAndBinormals;
			bool SmoothMesh;   
		};
	}
}