#pragma once
#include "Dimension.h"
#include "Vector2.h"
#include "Vector3.h"
#include "SaveSceneOptions.h"
#include "FbxHelper.h"

using namespace System;
using namespace System::Collections::Generic;

namespace Skill
{
	namespace Fbx
	{
		public value class CreatePatchParams
		{
		public:
			String^ PatchName;		
			int IndexI;
			int IndexJ;			
		};

		public ref class TerrainScene
		{	
		private:

			bool _InverseY;
			Dimension _PatchSize;
			Dimension _TerrainSize;
			int _LodFactor;
			int _PatchCountX;
			int _PatchCountZ;
			double _MinHeight;
			double _MaxHeight;
			Vector2 _Position;			
			Vector3 _Scale;

			String^ _Name;
			String^ _Status;
			double _Progress;
			double _ProgressChange;			


			int _VertexCount;
			FbxVector4* _Vertices;
			FbxVector4* _Normals;
			int _IndexCount;
			int* _Indices;

			int _LocalUVCount;			
			FbxVector2* _LocalUVs;			
			int _GlobalUVCount;
			FbxVector2* _GlobalUVs;
			
			FbxManager*   _SdkManager;
			FbxScene*        _Scene;	
			FbxFileTexture*  _Texture;
			FbxSurfacePhong* _Material;

			array<double>^ _Heights;
			List<CreatePatchParams>^ _Patches;
		public:

			property bool InverseY { bool get(){ return _InverseY; } void set(bool value) { _InverseY = value; } }
			property double MinHeight { double get(){ return _MinHeight; } void set(double value) { _MinHeight = value; } }
			property double MaxHeight { double get(){ return _MaxHeight; } void set(double value) { _MaxHeight = value; } }			
			property Vector2 Position { Vector2 get(){ return _Position; } void set(Vector2 value) { _Position = value; } }
			property Vector3 Scale { Vector3 get(){ return _Scale; } void set(Vector3 value) { _Scale = value; } }			

			property Dimension PatchSize { Dimension get(){ return _PatchSize; } }			
			property Dimension TerrainSize { Dimension get(){ return _TerrainSize; } }						
			property int PatchCountX { int get(){ return _PatchCountX; } }
			property int PatchCountZ { int get(){ return _PatchCountZ; } }

			property String^ Name { String^ get(){ return _Name; } }
			property String^ Status { String^ get(){ return _Status; } }
			property double Progress { double  get(){ return _Progress; } }

			TerrainScene(String^ name, array<double>^ heights, Dimension terrainSize,  Dimension patchSize , int lod);
			void Destroy();					
			bool Save(String^ filename, SaveSceneOptions options);			
			void Build();
			void AddPatch(CreatePatchParams patchParams);

		private:
						
			bool InitializeSdkObjects(String^ sceneName);			
			void CreateTexture();
			void CreateMaterial();
			//void AddMaterials(FbxMesh* pMesh);

			void CreatePatch(CreatePatchParams patchParams);
			void CreatePatch(const char* patchName, CreatePatchParams patchParams);
			FbxNode* CreatePatchMesh(const char* patchName, CreatePatchParams patchParams);			

			void CalcIndices();
			void CalcVertices();
			void CalcUVs();
			void CalcNormals();						
			int GetInverseYIndex(int index);
		};
	}
}