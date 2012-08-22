#pragma once
#include "FbxAttributeType.h"

using namespace System;
using namespace System::Collections::Generic;

namespace Skill
{
	namespace Fbx
	{
		public ref class SceneNode
		{	
		private:
			FbxAttributeType _AttributeType;			
			String^ _Name;
			String^ _NameAndAttributeTypeName;			
			FbxNode* _Node;
			
			List<SceneNode^>^ _Children;


		internal:
			SceneNode(FbxNode* node);
			void AddEmpty(SceneNode^ node);
			void Clear();
			FbxNode* GetFbxNode() {return _Node;}
			void SetFbxNode(FbxNode* node) {_Node = node;}
			
		public:
			
			void Add(SceneNode^ node);
			bool Remove(SceneNode^ node);

			property String^ Name { String^ get(){ return _Name; } }
			property String^ NameAndAttributeTypeName { String^ get(){ return _NameAndAttributeTypeName; } }
			property FbxAttributeType AttributeType { FbxAttributeType get(){ return _AttributeType; } }

			property int Count {  int get() { return _Children->Count; } }
			property SceneNode^ default[int] {  SceneNode^ get(int index) { return _Children[index]; } }

			property bool IsTriangleMesh {bool get();}
		};
	}
}