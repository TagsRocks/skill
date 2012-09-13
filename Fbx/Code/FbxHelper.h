#pragma once

#include "Dimension.h"
#include "Vector2.h"
#include "Vector3.h"
#include "FbxVersion.h"
#include "FbxAxis.h"
#include "FbxSystemUnits.h"
#include "SaveSceneOptions.h"

using namespace System;
using namespace System::Collections::Generic;

namespace Skill
{
	namespace Fbx
	{
		class FbxHelper
		{	

		public:

			static void GetVersion(FbxVersion version , int* lMajor,int* lMinor,int* lRevision);

			static void ConvertScene(FbxScene* scene, FbxAxis axis, FbxSystemUnits unit);

			static FbxVector4 SafeNormalize(FbxVector4& value);

			static FbxString GetNodeNameAndAttributeTypeName(const FbxNode* pNode);		

			static String^ ConvertToString(FbxString& fbxStr);			

			static void DestroySdkObjects(FbxManager *pSdkManager, FbxScene  *pScene);

			static bool LoadScene( FbxManager *pSdkManager, FbxScene  *pScene,  const char *pFbxFilePath );

			static bool SaveScene(FbxManager* pManager, FbxScene* pScene, const char* pFilename, SaveSceneOptions options);		

			static void TriangulateRecursive(FbxNode* pNode);

			static FbxAMatrix GetGeometry(FbxNode* pNode);
			static FbxAMatrix GetGlobalPosition(FbxNode* pNode, const FbxTime& pTime, FbxPose* pPose = NULL, FbxAMatrix* pParentGlobalPosition = NULL);
			static FbxAMatrix GetPoseMatrix(FbxPose* pPose, int pNodeIndex);

			/// <summary>
			/// Is two given vertices equal?
			/// </summary>
			/// <param name="v1">Vertex 1</param>
			/// <param name="v2">Vertex 2</param>
			/// <param name="telorance">Telorance</param>
			/// <returns>True if equals, otherwise false</returns>
			static bool IsEqual(FbxVector4& v1, FbxVector4& v2, double telorance);
			static bool IsEqual(FbxVector2& v1, FbxVector2& v2, double telorance);

		};
	}
}