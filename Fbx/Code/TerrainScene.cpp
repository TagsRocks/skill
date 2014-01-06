
#include "Stdafx.h"
#include "TerrainScene.h"
#include "MeshCreator.h"
#include "FbxHelper.h"
#include "Primitives.h"

#ifdef IOS_REF
#undef  IOS_REF
#define IOS_REF (*(pManager->GetIOSettings()))
#endif

#define UShortMax 65535

namespace Skill
{
	namespace Fbx
	{

#pragma region Common Methods

		void TerrainScene::Destroy()
		{			
			this->_Patches->Clear();

			if( this->_Indices) delete[] this->_Indices;
			if( this->_Vertices) delete[] this->_Vertices;
			if( this->_Normals ) delete[] this->_Normals;
			if( this->_LocalUVs) delete[] this->_LocalUVs;
			if( this->_GlobalUVs) delete[] this->_GlobalUVs;			

			this->_Indices = NULL;
			this->_Vertices = NULL;
			this->_Normals = NULL;
			this->_LocalUVs = NULL;
			this->_GlobalUVs = NULL;

			FbxHelper::DestroySdkObjects(_SdkManager, _Scene);			

			_SdkManager = NULL;
			_Scene = NULL;
		}		

		TerrainScene::TerrainScene(String^ name , array<double>^ heights , Dimension terrainSize, Dimension patchSize , int lod )
		{
			this->_Indices = NULL;
			this->_Vertices = NULL;
			this->_LocalUVs = NULL;
			this->_GlobalUVs = NULL;
			this->_Normals = NULL;

			this->_InverseY = false;
			this->_MinHeight = 0;
			this->_MaxHeight = 255;
			this->_Position.X = this->_Position.Y = 0;
			this->_Scale.X = this->_Scale.Y = this->_Scale.Z = 1;			

			this->_TerrainSize = terrainSize;
			this->_PatchSize = patchSize;
			this->_Heights = heights;
			this->_Name = name;		
			this->_Patches = gcnew List<CreatePatchParams>();
			// Initialize the FbxManager and the FbxScene
			if(InitializeSdkObjects(_Name) == false)
			{
				throw gcnew Exception(L"Can not create scene");
			}

			// patchsize is like 16, 32, 64, ...
			// terrainsize is like 17,33,65,...,257,...

			_LodFactor = Math::Pow(2,lod);

			_IndexCount = (this->_PatchSize.Width / _LodFactor) * (this->_PatchSize.Height / _LodFactor) * 6; 
			_VertexCount = this->_TerrainSize.Width * this->_TerrainSize.Height; 
			if(_VertexCount != this->_Heights->Length)
			{
				throw gcnew Exception(L"Inavlid parameters heights and terrainSize");
			}

			this->_PatchCountX = this->_TerrainSize.Width / this->_PatchSize.Width;
			this->_PatchCountX = this->_TerrainSize.Height / this->_PatchSize.Height;


			// create a single texture shared by all cubes
			CreateTexture();

			// create a material shared by all faces of all cubes
			CreateMaterial();
		}

		bool TerrainScene::InitializeSdkObjects(String^ sceneName)
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

			IntPtr ptrName = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(sceneName);
			const char* sceneNamechars = static_cast<const char*>(ptrName.ToPointer());

			//Create an FBX scene. This object holds most objects imported/exported from/to files.
			_Scene = FbxScene::Create(_SdkManager, sceneNamechars);
			if( !_Scene )
			{
				return false;
			}			

			return true;
		}

		bool TerrainScene::Save(String^ filename, SaveSceneOptions options)		
		{
			IntPtr ptrName = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename);
			const char* filenameChars = static_cast<const char*>(ptrName.ToPointer());
			return FbxHelper::SaveScene(_SdkManager, _Scene, filenameChars, options);
		}				

		// Create a global texture for cube.
		void TerrainScene::CreateTexture()
		{
			_Texture = FbxFileTexture::Create(_Scene,"Diffuse Texture");

			// Resource file must be in the application's directory.
			FbxString lTexPath = "";

			// Set texture properties.
			_Texture->SetFileName(lTexPath.Buffer()); 
			_Texture->SetTextureUse(FbxTexture::eStandard);
			_Texture->SetMappingType(FbxTexture::eUV);
			_Texture->SetMaterialUse(FbxFileTexture::eModelMaterial);
			_Texture->SetSwapUV(false);
			_Texture->SetTranslation(0.0, 0.0);
			_Texture->SetScale(1.0, 1.0);
			_Texture->SetRotation(0.0, 0.0);
		}


		// Create global material for cube.
		void TerrainScene::CreateMaterial()
		{
			FbxString lMaterialName = "material";
			FbxString lShadingName  = "Phong";
			FbxDouble3 lBlack(0.0, 0.0, 0.0);
			FbxDouble3 lGray(0.1, 0.1, 0.1);
			FbxDouble3 lDiffuseColor(0.75, 0.75, 0.0);
			_Material = FbxSurfacePhong::Create(_Scene, lMaterialName.Buffer());

			// Generate primary and secondary colors.
			_Material->Emissive.Set(lBlack);
			_Material->Ambient.Set(lGray);
			_Material->Diffuse.Set(lDiffuseColor);
			_Material->TransparencyFactor.Set(40.5);
			_Material->ShadingModel.Set(lShadingName);
			_Material->Shininess.Set(0.5);

			// the texture need to be connected to the material on the corresponding property
			if (_Texture)
				_Material->Diffuse.ConnectSrcObject(_Texture);
		}

		//void TerrainScene::AddMaterials(FbxMesh* pMesh)
		//{
		//	int polyCount = _PatchSize.Width * _PatchSize.Height * 2;

		//	// Set material mapping.
		//	FbxGeometryElementMaterial* lMaterialElement = pMesh->CreateElementMaterial();
		//	lMaterialElement->SetMappingMode(FbxGeometryElement::eByPolygon);
		//	lMaterialElement->SetReferenceMode(FbxGeometryElement::eIndexToDirect);

		//	//get the node of mesh, add material for it.
		//	FbxNode* lNode = pMesh->GetNode();
		//	if(lNode == NULL) 
		//		return;
		//	lNode->AddMaterial(_Material);
		//	
		//	lMaterialElement->GetIndexArray().SetCount(polyCount);

		//	// Set the Index 0 to polyCount to the material in position 0 of the direct array.
		//	for(int i=0; i<polyCount; ++i)
		//		lMaterialElement->GetIndexArray().SetAt(i,0);
		//}
#pragma endregion

		void TerrainScene::Build()
		{
			this->CalcIndices();
			this->CalcVertices();
			this->CalcUVs();
			this->CalcNormals();			

			_Progress = 0;
			_ProgressChange = 100.0 / (this->_Patches->Count * PatchSize.Width * PatchSize.Height);
			for(int i=0; i < this->_Patches->Count ; i++)
			{
				CreatePatch(this->_Patches[i]);
			}
		}

		void TerrainScene::AddPatch(CreatePatchParams patchParams)
		{
			this->_Patches->Add(patchParams);
		}

		void TerrainScene::CreatePatch(CreatePatchParams patchParams)
		{
			IntPtr ptrName = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(patchParams.PatchName);
			const char* patchNameChars = static_cast<const char*>(ptrName.ToPointer());

			_Status = String::Format(L"Create Patche {0} ..." , patchParams.PatchName );
			CreatePatch(patchNameChars, patchParams);				
		}

		void TerrainScene::CreatePatch(const char* patchName, CreatePatchParams patchParams)
		{

			FbxNode* patch = CreatePatchMesh(patchName, patchParams);

			// set the cube position
			patch->LclTranslation.Set(FbxVector4(0, 0, 0));			

			// if we asked to create the cube with a texture, we need 
			// a material present because the texture connects to the
			// material DiffuseColor property
			//AddMaterials(patch->GetMesh());
			patch->AddMaterial(_Material);

			_Scene->GetRootNode()->AddChild(patch);
		}

		FbxNode* TerrainScene::CreatePatchMesh(const char* patchName,  CreatePatchParams patchParams)
		{			
			MeshData data = MeshData();

			// Create control points.			
			int vertexCount = (_PatchSize.Width / _LodFactor + 1)  * (_PatchSize.Height/ _LodFactor + 1);
			data.Vertices.Reserve(vertexCount);
			data.HasUV2 = true;			

			int i,j;
			int startIndexI = patchParams.IndexI * _PatchSize.Height * _TerrainSize.Width;						
			int index = 0;
			for (i = 0; i <= PatchSize.Width / _LodFactor; i++)
			{
				int startIndexJ = patchParams.IndexJ * _PatchSize.Width;
				for (j = 0; j <= PatchSize.Height / _LodFactor; j++)
				{					
					int jj = startIndexI + startIndexJ + (j * _LodFactor);
					if(jj >= _VertexCount || startIndexJ + j >= _TerrainSize.Width)
					{
						index++;
						continue;
					}

					Vertex vertex = Vertex();
					vertex.Position = _Vertices[jj];					
					vertex.Normal = _Normals[jj];
					vertex.UV = _GlobalUVs[jj];	
					vertex.UV2 = _LocalUVs[index];

					double x = vertex.Position[0];
					double y = vertex.Position[1];
					double z = vertex.Position[2];


					data.Vertices.SetAt(index,vertex);

					index++;
					_Progress += _ProgressChange;
				}				
				startIndexI += _TerrainSize.Width * _LodFactor;
			}

			int faceCount = _IndexCount / 3;
			data.Faces.Reserve(faceCount);			
			index = 0;
			//Create polygons. Assign texture and texture UV indices.
			for (i = 0; i < _IndexCount; i += 3)
			{
				Face face = Face();

				face.A = _Indices[i];
				face.B = _Indices[i + 1];
				face.C = _Indices[i + 2];
				face.MaterialId = -1;
				data.Faces.SetAt(index, face);
				index++;
			}			


			MeshCreator creator = MeshCreator();

			FbxNode* mesh = creator.Create(_Scene,patchName,data);

			// return the FbxNode
			return mesh;
		}


		void TerrainScene::CalcIndices()
		{
			_Status = L"Calculate Indices ...";
			_Progress = 0;

			int i, j;
			int p = 0;
			int** seqIndex = new int*[PatchSize.Width / _LodFactor + 1];
			for(int i = 0; i < PatchSize.Width/ _LodFactor + 1; ++i)
				seqIndex[i] = new int[PatchSize.Height/ _LodFactor + 1];

			for (i = 0; i <= PatchSize.Width/ _LodFactor; i++)
			{
				for (j = 0; j <= PatchSize.Height/ _LodFactor; j++)
				{
					seqIndex[i][j] = p++;
				}
			}

			if(!_Indices)
				_Indices = new int[_IndexCount];

			_ProgressChange = 100.0 / _IndexCount;
			int index = 0;			
			for (i = 0; i < PatchSize.Width/ _LodFactor; i++)
			{
				for (j = 0; j < PatchSize.Height/ _LodFactor; j++)
				{

					_Indices[index++] = seqIndex[i][j];                    
					_Indices[index++] = seqIndex[i + 1][ j];
					_Indices[index++] = seqIndex[i][ j + 1];

					_Indices[index++] = seqIndex[i + 1][ j];                    
					_Indices[index++] = seqIndex[i + 1][ j + 1];
					_Indices[index++] = seqIndex[i][ j + 1];

					_Progress += _ProgressChange;
				}
			}

			for( i = 0; i < PatchSize.Width / _LodFactor + 1; ++i)
				delete[] seqIndex[i];
			delete[] seqIndex;
		}

		void TerrainScene::CalcVertices()
		{
			_Status = L"Calculate Vertices ...";
			_Progress = 0;

			if(MaxHeight < MinHeight)
				MaxHeight = MinHeight + 10;

			int i,j;
			double deltaH = MaxHeight - MinHeight;
			int index = 0;
			double startX = _Position.X;
			double startZ = _Position.Y;

			if(!_Vertices)
				_Vertices = new FbxVector4[_VertexCount];


			_ProgressChange = 100.0 / _VertexCount;

			for (i = 0; i < _TerrainSize.Width; i++)
			{
				for (j = 0; j < _TerrainSize.Height; j++)
				{					
					int ix = this->_InverseY? GetInverseYIndex(index) : index;					
					_Vertices[index] = FbxVector4(startX + (j * _Scale.X),
						_MinHeight + (_Heights[ix] * (deltaH / UShortMax) * _Scale.Y),
						startZ) ;					
					index++;
					_Progress += _ProgressChange;
				}
				startZ += _Scale.Z;
			}

		}

		int TerrainScene::GetInverseYIndex(int index)
		{
			int y = index / _TerrainSize.Width;
			int x = index % _TerrainSize.Width;

			return (_TerrainSize.Height - y - 1) * _TerrainSize.Width + x;
		}

		void TerrainScene::CalcUVs()
		{
			_Status = L"Calculate Global UVs ...";
			_Progress = 0;

			_GlobalUVCount = _TerrainSize.Width * _TerrainSize.Height;			
			if(!_GlobalUVs) _GlobalUVs = new FbxVector2[_GlobalUVCount];

			_ProgressChange = 100.0 / _GlobalUVCount;

			int i,j;
			// Global UV
			int index = 0;
			for (i = 0; i < _TerrainSize.Width; i++)
			{
				double u = 1.0 -  ((double)i) / (_TerrainSize.Width - 1);
				for (j = 0; j < _TerrainSize.Height; j++)
				{					
					double v = ((double)j) / (_TerrainSize.Height - 1);					
					_GlobalUVs[index] = FbxVector2(v,u);					
					index++;
					_Progress += _ProgressChange;
				}
			}


			_Status = L"Calculate Local UVs ...";
			_Progress = 0;
			_LocalUVCount = (_PatchSize.Width + 1) * (_PatchSize.Height + 1);
			if(!_LocalUVs) _LocalUVs = new FbxVector2[_LocalUVCount];

			_ProgressChange = 100.0 / _LocalUVCount;
			index = 0;
			for (i = 0; i <= _PatchSize.Width; i++)
			{
				double u = 1.0 -  ((double)i) / (_PatchSize.Width);
				for (j = 0; j <= _PatchSize.Height; j++)
				{					
					double v = ((double)j) / (_PatchSize.Height);					
					_LocalUVs[index] = FbxVector2(v,u);

					_Progress += _ProgressChange;
					index++;
				}
			}
		}

		void TerrainScene::CalcNormals()
		{
			_Status = L"Calculate Normals ...";
			_Progress = 0;

			int i, j;
			int p = 0;

			if(!_Normals)
				_Normals = new FbxVector4[_VertexCount];

			for (i = 0; i < _VertexCount; i++)
				_Normals[i]  = FbxVector4(0,0,0);

			int** seqIndex = new int*[_TerrainSize.Width];
			for(i = 0; i < _TerrainSize.Width; ++i)
				seqIndex[i] = new int[_TerrainSize.Height];

			for (i = 0; i < _TerrainSize.Width; i++)
			{
				for (j = 0; j < _TerrainSize.Height; j++)
				{
					seqIndex[i][j] = p++;
				}
			}						


			_ProgressChange = 100.0 / ((_TerrainSize.Width - 1) * (_TerrainSize.Height - 1));

			for (i = 0; i < _TerrainSize.Width - 1; i++)
			{
				for (j = 0; j < _TerrainSize.Height - 1; j++)
				{										
					FbxVector4 v1 = _Vertices[seqIndex[i][j]];
					FbxVector4 v2 = _Vertices[seqIndex[i + 1][ j]];
					FbxVector4 v3 = _Vertices[seqIndex[i][ j + 1]];

					FbxVector4 v21 = v2 - v1;
					FbxVector4 v32 = v3 - v2;
					FbxVector4 cross = v21.CrossProduct(v32);

					FbxVector4 normal = FbxHelper::SafeNormalize(cross);

					_Normals[seqIndex[i][j]] += normal;
					_Normals[seqIndex[i + 1][ j]] += normal;
					_Normals[seqIndex[i][ j + 1]] += normal;                


					v1 = _Vertices[seqIndex[i + 1][ j]];
					v2 = _Vertices[seqIndex[i + 1][ j + 1]];
					v3 = _Vertices[seqIndex[i][ j + 1]];

					v21 = v2 - v1;
					v32 = v3 - v2;
					cross = v21.CrossProduct(v32);

					normal = FbxHelper::SafeNormalize(cross);

					_Normals[seqIndex[i + 1][ j]] += normal;
					_Normals[seqIndex[i + 1][ j + 1]] += normal;
					_Normals[seqIndex[i][ j + 1]] += normal;					

					_Progress += _ProgressChange;
				}
			}			

			for (int i = 0; i < _VertexCount; i++)
			{
				_Normals[i] = FbxHelper::SafeNormalize(_Normals[i]);
			}
		}		



	}
}