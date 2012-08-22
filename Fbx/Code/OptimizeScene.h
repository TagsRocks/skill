#pragma once
#include "Dimension.h"
#include "Vector2.h"
#include "Vector3.h"
#include "SaveSceneOptions.h"

using namespace System;
using namespace System::Collections::Generic;

namespace Skill
{
	namespace Fbx
	{
		ref class SceneNode;

		public ref class OptimizeScene
		{	
		private:

			double _PositionsTelorance;
			double _UvTelorance;
						
			String^ _Status;
			double _Progress;
			double _ProgressChange;						

			String^ _Filename;						

			FbxManager*   _SdkManager;
			FbxScene*        _Scene;
			SceneNode^ _Root;
			SceneNode^ _SkeletonRoot;

		public:			

			property double PositionsTelorance { double get(){ return _PositionsTelorance; } void set(double value) { _PositionsTelorance = value;} }
			property double UvTelorance { double get(){ return _UvTelorance; } void set(double value) { _UvTelorance = value;} }
			property String^ Status { String^ get(){ return _Status; } }
			property double Progress { double  get(){ return _Progress; } }
			property String^ Filename { String^ get(){ return _Filename; } }
			property SceneNode^ Root { SceneNode^ get(){ return _Root; } }

			OptimizeScene(String^ filename);
			void Destroy();					
			bool Save(String^ filename, SaveSceneOptions options);
			SceneNode^ Merge(array<SceneNode^>^ meshes,String^ newName);			
			void Optimize(SceneNode^ mesh , bool generateNormals, bool generateTangents);

		private:
			
			bool InitializeSdkObjects();
			void DestroySdkObjects();				


			void CreateHierarchy();
			SceneNode^ CreateHierarchy(FbxNode* node);
		};
	}
}