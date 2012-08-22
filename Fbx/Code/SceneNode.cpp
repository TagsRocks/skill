#include "Stdafx.h"
#include "SceneNode.h"
#include "FbxHelper.h"


namespace Skill
{
	namespace Fbx
	{
		SceneNode::SceneNode(FbxNode* node)
		{
			if(node)
			{				
				 _Node = node;
				 FbxString  fbxName = node->GetName();
				 FbxString   fbxNameAndAttributeTypeName = FbxHelper::GetNodeNameAndAttributeTypeName(node);

				_Name = FbxHelper::ConvertToString(fbxName);
				_NameAndAttributeTypeName = FbxHelper::ConvertToString(fbxNameAndAttributeTypeName);
				
				if(node->GetNodeAttribute() == NULL)
				{
					_AttributeType = FbxAttributeType::Unknown;
				}
				else
				{
					_AttributeType = (FbxAttributeType)(node->GetNodeAttribute()->GetAttributeType());					
				}
			}
			_Children = gcnew List<SceneNode^>();
		}

		void SceneNode::AddEmpty(SceneNode^ node)
		{
			if(!_Children->Contains(node))
				_Children->Add(node);
		}

		void SceneNode::Add(SceneNode^ node)
		{
			if(!_Children->Contains(node))
			{
				this->_Node->AddChild(node->GetFbxNode());
				_Children->Add(node);
			}
		}
		bool SceneNode::Remove(SceneNode^ node)
		{
			this->_Node->RemoveChild(node->GetFbxNode());
			return _Children->Remove(node);			
		}
		void SceneNode::Clear()
		{
			_Children->Clear();
		}

		bool SceneNode::IsTriangleMesh::get() 
		{
			if(_Node->GetNodeAttribute() != NULL)
			{
				FbxNodeAttribute::EType lAttributeType = (_Node->GetNodeAttribute()->GetAttributeType());

				if(lAttributeType == FbxNodeAttribute::eMesh )
				{
					FbxMesh* mesh = _Node->GetMesh();
					if(mesh )
						return mesh->IsTriangleMesh();
				}				
			}
			return false;
		}
	}
}