
#include "Stdafx.h"
#include "OptimizeScene.h"
#include "FbxHelper.h"
#include "SceneNode.h"
#include "Primitives.h"
#include "MeshLoader.h"
#include "MeshCreator.h"
#include "MeshProcessor.h"

#ifdef IOS_REF
#undef  IOS_REF
#define IOS_REF (*(pManager->GetIOSettings()))
#endif

namespace Skill
{
	namespace Fbx
	{
		
		void OptimizeScene::Destroy()
		{			
			DestroySdkObjects();			
		}		

		OptimizeScene::OptimizeScene(String^ filename)
		{	
			_SkeletonRoot = nullptr;
			_PositionsTelorance = 0.02;
			_UvTelorance = 0.01;

			_Filename = filename;


			// Initialize the FbxManager and the FbxScene
			if(InitializeSdkObjects() == false)
			{
				throw gcnew Exception(L"Can not create scene");
			}			

			IntPtr ptrName = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename);
			const char* filenameChars = static_cast<const char*>(ptrName.ToPointer());
			// Load the scene.
			if(!FbxHelper::LoadScene(_SdkManager, _Scene, filenameChars))
				throw gcnew Exception("Can not load file");

			// Convert Axis System to what is used in this example, if needed
            FbxAxisSystem SceneAxisSystem = _Scene->GetGlobalSettings().GetAxisSystem();
            FbxAxisSystem OurAxisSystem(FbxAxisSystem::eYAxis, FbxAxisSystem::eParityOdd, FbxAxisSystem::eRightHanded);
            if( SceneAxisSystem != OurAxisSystem )
            {
                OurAxisSystem.ConvertScene(_Scene);
            }

            // Convert Unit System to what is used in this example, if needed
            FbxSystemUnit SceneSystemUnit = _Scene->GetGlobalSettings().GetSystemUnit();
            if( SceneSystemUnit.GetScaleFactor() != 1.0 )
            {
                //The unit in this example is centimeter.
                FbxSystemUnit::cm.ConvertScene( _Scene);
            }
			
			FbxHelper::TriangulateRecursive(_Scene->GetRootNode());
			CreateHierarchy();
		}

		void OptimizeScene::CreateHierarchy()
		{
			FbxNode* rootNode = _Scene->GetRootNode();
			if(rootNode)			
				_Root = CreateHierarchy(rootNode);
		}

		SceneNode^ OptimizeScene::CreateHierarchy(FbxNode* node)
		{						
			SceneNode^ sceneNode = gcnew SceneNode(node);
			if(_SkeletonRoot == nullptr)
			{
				if(sceneNode->AttributeType == Skill::Fbx::FbxAttributeType::Skeleton )
					_SkeletonRoot = sceneNode;
			}

			int num = node->GetChildCount();
			for (int i = 0; i < num; i++)
			{
				FbxNode* child = node->GetChild(i);
				if(child)
				{				
					SceneNode^ childNode = CreateHierarchy(child);
					sceneNode->AddEmpty(childNode);
				}
			}
			return sceneNode;
		}


		bool OptimizeScene::Save(String^ filename, SaveSceneOptions options)		
		{
			IntPtr ptrName = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename);
			const char* filenameChars = static_cast<const char*>(ptrName.ToPointer());
			return FbxHelper::SaveScene(_SdkManager, _Scene, filenameChars, options);
		}

		bool OptimizeScene::InitializeSdkObjects()
		{
			//The first thing to do is to create the FBX Manager which is the object allocator for almost all the classes in the SDK
			_SdkManager = FbxManager::Create();
			if( !_SdkManager )
			{
				return false;
			}

			//Create an IOSettings object. This object holds all import/export settings.
			FbxIOSettings* ios = FbxIOSettings::Create(_SdkManager, IOSROOT);
			_SdkManager->SetIOSettings(ios);			

			//Create an FBX scene. This object holds most objects imported/exported from/to files.
			_Scene = FbxScene::Create(_SdkManager,"");
			if( !_Scene )
			{
				return false;
			}			

			return true;
		}

		void OptimizeScene::DestroySdkObjects()
		{
			//Delete the FBX Manager. All the objects that have been allocated using the FBX Manager and that haven't been explicitly destroyed are also automatically destroyed.
			if( _SdkManager ) _SdkManager->Destroy();			
			_SdkManager = NULL;
		}
			

		SceneNode^ OptimizeScene::Merge(array<SceneNode^>^ meshes, String^ newName)
		{	
			MeshProcessor processor;
			FbxArray<MeshData*> meshDataArray;

			for(int i=0; i < meshes->Length; i++)
			{
				FbxMesh* mesh = meshes[i]->GetFbxNode()->GetMesh();
				MeshLoader loader = MeshLoader(mesh);

				MeshData* data = new MeshData();
				loader.Fill(data);
								
				if(data->HasSkin)
				{					
					processor.TransformToBindPose(mesh,*data);										
				}
				else
				{
					processor.TransformToGlobal(meshes[i]->GetFbxNode(),*data);
				}
				meshDataArray.Add(data);
			}
					
			
			MeshData* mergedMesh = processor.Merge(meshDataArray);

			IntPtr ptrName = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(newName);
			const char* newNameChars = static_cast<const char*>(ptrName.ToPointer());

			MeshCreator creator;
			creator.SetCreateSkin(true);
			FbxNode* resultMesh = creator.Create(_Scene,newNameChars , *mergedMesh);			

			SceneNode^ result = gcnew SceneNode(resultMesh);
			delete mergedMesh;

			for(int i=0; i < meshDataArray.GetCount(); i++)
			{
				MeshData* data = meshDataArray.GetAt(i);
				delete data;
			}
			meshDataArray.Clear();

			return result;			
		}


		void OptimizeScene::Optimize(SceneNode^ mesh , bool generateNormals, bool generateTangents)
		{			
			MeshLoader loader = MeshLoader(mesh->GetFbxNode()->GetMesh());
			MeshData* meshData = new MeshData();
			loader.Fill(meshData);			
						
			MeshProcessor processor;

			processor.SetPositionsTelorance(_PositionsTelorance);
			processor.SetUvTelorance(_UvTelorance);

			if(generateNormals || (!meshData->HasNormal && generateTangents))
				processor.GenerateNormals(*meshData);
			if(generateTangents)
				processor.GenerateTangentAndBinormals(*meshData);

			processor.ReduceVertex(*meshData);

			MeshCreator creator;
			FbxNode* resultMesh = creator.Create(_Scene, mesh->GetFbxNode()->GetName(), *meshData);

			FbxNode* parent = mesh->GetFbxNode()->GetParent();
			if(parent)
			{
				parent->RemoveChild(mesh->GetFbxNode());
				parent->AddChild(resultMesh);
			}			
			mesh->SetFbxNode(resultMesh);
			delete meshData;
		}		
	}
}