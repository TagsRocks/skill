#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace Skill
{
	namespace Fbx
	{				
		class MeshData;		

		class MeshCreator
		{

		private:

			bool _CreateSkin;


			void LinkMeshToSkeleton(FbxScene* scene , FbxNode* lNode , MeshData& meshData);
			void StoreBindPose(FbxScene* scene, FbxNode* node, MeshData& meshData );
			void StoreRestPose(FbxScene* pScene, FbxNode* pSkeletonRoot);
			void AddNodeRecursively(FbxArray<FbxNode*>& pNodeArray, FbxNode* pNode);
			
		public:			
			
			MeshCreator() { _CreateSkin = true; }

			FbxNode* Create(FbxScene* scene , const char* meshName, MeshData& meshData );
			void CreateMaterialMapping(FbxMesh* pMesh , MeshData& meshData);					

			bool GetCreateSkin() {return _CreateSkin;}
			void SetCreateSkin(bool value) { _CreateSkin = value;}
		};
	}
}