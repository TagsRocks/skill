#include "StdAfx.h"
#include "MeshLoader.h"
#include "Primitives.h"



namespace Skill
{
	namespace Fbx
	{

		MeshLoader::MeshLoader(FbxMesh* mesh)		 
		{
			_ControlsPointsCount = 0;			
			_NormalCount = 0;
			_PolyCount = 0;
			_UvsCountA = 0;			
			_UvsCountB = 0;
			_ColorCount = 0;
			_BoneSkinCount = 0;
			_MaterialCount = 0;	
			_VertexWeightCount = 0;

			_WeightPerVertex = NULL;
			_PolygonIndices = NULL;
			_ControlsPoints = NULL;
			_Normals = NULL;
			_UvsA = NULL;
			_UvsB = NULL;
			_Colors = NULL;
			_BoneSkins = NULL;			
			_FaceMaterial = NULL;	
			_Materials = NULL;

			this->_Mesh = mesh;
			Destroy();

			LoadControlsPoints();
			LoadPolygonIndices();
			LoadNormals();
			LoadUvsA();
			LoadUvsB();
			LoadColors();
			LoadSkins();
			LoadFaceMaterials();
			LoadMaterials();

			bool hasSkin = _BoneSkinCount > 0;			
			if (hasSkin)
				GenerateBlendWeightIndices();
		}

		void MeshLoader::Destroy()
		{								
			if(_ControlsPoints) delete[] _ControlsPoints;
			if(_WeightPerVertex) delete[] _WeightPerVertex;
			if(_BoneSkins) delete[] _BoneSkins;
			if(_Normals) delete[] _Normals;
			if(_PolygonIndices) delete[] _PolygonIndices;						
			if(_UvsA) delete[] _UvsA;
			if(_UvsB) delete[] _UvsB;			
			if(_Colors) delete[] _Colors;			
			if(_FaceMaterial) delete[] _FaceMaterial;			
			if(_Materials) delete[] _Materials;						


			_VertexWeightCount = 0;
			_ControlsPointsCount = 0;			
			_NormalCount = 0;
			_PolyCount = 0;
			_UvsCountA = 0;			
			_UvsCountB = 0;
			_ColorCount = 0;
			_BoneSkinCount = 0;
			_MaterialCount = 0;

			_WeightPerVertex = NULL;
			_PolygonIndices = NULL;
			_ControlsPoints = NULL;
			_Normals = NULL;
			_UvsA = NULL;
			_UvsB = NULL;
			_Colors = NULL;
			_BoneSkins = NULL;			
			_FaceMaterial = NULL;	
			_Materials = NULL;
		}

		MeshLoader::~MeshLoader(void)
		{
			_Mesh = NULL;
			Destroy();

		}


		void MeshLoader::LoadControlsPoints()
		{
			_ControlsPointsCount = _Mesh->GetControlPointsCount();

			_ControlsPoints = new FbxVector4[_ControlsPointsCount];

			memcpy(_ControlsPoints,_Mesh->GetControlPoints(), sizeof(FbxVector4) * _ControlsPointsCount );			

		}

		void MeshLoader::LoadNormals()
		{
			if(_Mesh->GetElementNormalCount() > 0)
			{
				FbxGeometryElementNormal* leNormals = _Mesh->GetElementNormal(0);
				if (leNormals->GetMappingMode() == FbxGeometryElement::eByControlPoint)
				{	
					if (leNormals->GetReferenceMode() == FbxGeometryElement::eDirect)
					{
						_NormalCount = _ControlsPointsCount;
						_Normals = new FbxVector4[_NormalCount];

						for(int i=0; i < _NormalCount; i++)
						{
							_Normals[i]= leNormals->GetDirectArray().GetAt(i);
						}
					}
				}
				else if (leNormals->GetMappingMode() == FbxGeometryElement::eByPolygonVertex)
				{
					_NormalCount = _PolyCount * 3;
					_Normals = new FbxVector4[_NormalCount];
					FbxVector4 lCurrentNormal;
					int lVertexCount = 0;
					for (int lPolygonIndex = 0; lPolygonIndex < _PolyCount; ++lPolygonIndex)
					{
						for (int lVerticeIndex = 0; lVerticeIndex < 3; ++lVerticeIndex)
						{
							_Mesh->GetPolygonVertexNormal(lPolygonIndex, lVerticeIndex, lCurrentNormal);
							_Normals[lVertexCount] = lCurrentNormal;	
							lVertexCount++;
						}
					}					
				}
			}

		}

		void MeshLoader::LoadPolygonIndices()
		{
			_PolyCount = _Mesh->GetPolygonCount();

			int indexCount = _PolyCount * 3;// the mesh must be triangulate

			_PolygonIndices = new int[indexCount];                
			int indexID = 0;
			for (int polygonIndex = 0; polygonIndex < _PolyCount; polygonIndex++)
			{
				int polygonSize = _Mesh->GetPolygonSize(polygonIndex);
				for (int i = 0; i < polygonSize; i++)
				{
					_PolygonIndices[indexID++] = _Mesh->GetPolygonVertex(polygonIndex, i);
				}               
			}
		}

		void MeshLoader::LoadUvsA()
		{
			int uvCount = _Mesh->GetElementUVCount();
			if(uvCount < 1) return;

			FbxGeometryElementUV* leUV =_Mesh->GetElementUV(0);
			_UvsA = LoadUvs(leUV, _UvsCountA);
		}

		void MeshLoader::LoadUvsB()
		{
			int uvCount = _Mesh->GetElementUVCount();
			if(uvCount < 2) return;

			FbxGeometryElementUV* leUV =_Mesh->GetElementUV(1);
			_UvsB = LoadUvs(leUV, _UvsCountB);
		}

		FbxVector2* MeshLoader::LoadUvs(FbxGeometryElementUV* leUV, int& uvsCount)
		{	
			FbxVector2* result = NULL;

			for (int polygonIndex = 0; polygonIndex < _PolyCount; polygonIndex++)
			{
				int polygonSize = _Mesh->GetPolygonSize(polygonIndex);
				for (int i = 0; i < polygonSize; i++)
				{
					int controlPointIndex = _Mesh->GetPolygonVertex(polygonIndex, i); 
					if (leUV)
					{
						switch (leUV->GetMappingMode())
						{
						case FbxGeometryElement::eByControlPoint:
							{
								if(result == NULL)
								{
									uvsCount = _ControlsPointsCount;
									result = new FbxVector2[uvsCount];
								}
								switch (leUV->GetReferenceMode())
								{
								case FbxGeometryElement::eDirect:
									result[controlPointIndex] = leUV->GetDirectArray().GetAt(controlPointIndex);
									break;
								case FbxGeometryElement::eIndexToDirect:
									{
										int id = leUV->GetIndexArray().GetAt(controlPointIndex);
										result[controlPointIndex] = leUV->GetDirectArray().GetAt(id);
									}
									break;
								}
							}
							break;

						case FbxLayerElement::eByPolygonVertex:
							{
								if(result == NULL)
								{
									uvsCount = _PolyCount * 3;
									result = new FbxVector2[uvsCount];
								}
								int TextureUVIndex = _Mesh->GetTextureUVIndex(polygonIndex, i);

								switch (leUV->GetReferenceMode())
								{
								case FbxLayerElement::eDirect:
								case FbxLayerElement::eIndexToDirect:
									{
										result[polygonIndex * 3 + i] = leUV->GetDirectArray().GetAt(TextureUVIndex);
									}
									break;
								}
								break;
							}
						}
					}

				}               
			}

			return result;
		}

		void MeshLoader::LoadColors()
		{	
			if( _Mesh->GetElementVertexColorCount() < 1) return;

			FbxLayerElementVertexColor* leVtxc =_Mesh->GetElementVertexColor(0);

			for (int polygonIndex = 0; polygonIndex < _PolyCount; polygonIndex++)
			{
				int polygonSize = _Mesh->GetPolygonSize(polygonIndex);
				for (int i = 0; i < polygonSize; i++)
				{
					int controlPointIndex = _Mesh->GetPolygonVertex(polygonIndex, i);
					if (leVtxc)
					{
						switch (leVtxc->GetMappingMode())
						{
						case FbxGeometryElement::eByControlPoint:
							{
								if(_ColorCount == 0)
								{
									_ColorCount = _ControlsPointsCount;
									_Colors = new FbxColor[_ColorCount];
								}
								switch (leVtxc->GetReferenceMode())
								{
								case FbxGeometryElement::eDirect:
									_Colors[controlPointIndex] = leVtxc->GetDirectArray().GetAt(controlPointIndex);
									break;
								case FbxGeometryElement::eIndexToDirect:
									{
										int id = leVtxc->GetIndexArray().GetAt(controlPointIndex);
										_Colors[controlPointIndex] = leVtxc->GetDirectArray().GetAt(id);
									}
									break;
								}
							}
							break;

						case FbxLayerElement::eByPolygonVertex:
							{
								if(_ColorCount == 0)
								{
									_ColorCount = _PolyCount * 3;
									_Colors = new FbxColor[_ColorCount];
								}
								int TextureUVIndex = _Mesh->GetTextureUVIndex(polygonIndex, i);

								switch (leVtxc->GetReferenceMode())
								{
								case FbxLayerElement::eDirect:
								case FbxLayerElement::eIndexToDirect:
									{
										_Colors[polygonIndex * 3 + i] = leVtxc->GetDirectArray().GetAt(TextureUVIndex);
									}
									break;
								}
								break;
							}
						}
					}
				}               
			}
		}

		void MeshLoader::LoadSkins()
		{
			FbxCluster* cluster;
			
			int skinCount = _Mesh->GetDeformerCount(FbxDeformer::eSkin);
			if (skinCount > 0)
			{

				int clusterCount = ((FbxSkin*)_Mesh->GetDeformer(0, FbxDeformer::eSkin))->GetClusterCount();
				_BoneSkins = new BoneSkin[clusterCount];
				_BoneSkinCount = clusterCount;
				for (int j = 0; j < clusterCount; ++j)
				{
					cluster=((FbxSkin *) _Mesh->GetDeformer(0, FbxDeformer::eSkin))->GetCluster(j);
					if(cluster)
					{

						_BoneSkins[j].WeightCount = cluster->GetControlPointIndicesCount();
						_BoneSkins[j].Indices = new int[ _BoneSkins[j].WeightCount ];
						_BoneSkins[j].Weights = new double[_BoneSkins[j].WeightCount];
						_BoneSkins[j].Bone = cluster->GetLink();

						memcpy( _BoneSkins[j].Indices , cluster->GetControlPointIndices() , sizeof(int) * _BoneSkins[j].WeightCount);
						memcpy( _BoneSkins[j].Weights , cluster->GetControlPointWeights() , sizeof(double) * _BoneSkins[j].WeightCount);
						
					}
				}
			}
		}

		void MeshLoader::LoadFaceMaterials()
		{
			if(_Mesh->GetElementMaterialCount() < 1) return;			
			FbxGeometryElementMaterial* leMat = _Mesh->GetElementMaterial(0);
			if (leMat )
			{
				_FaceMaterial = new int[_PolyCount];
				switch (leMat->GetReferenceMode())
				{
				case FbxLayerElement::eIndex:
				case FbxLayerElement::eIndexToDirect:

					switch (leMat->GetMappingMode())
					{
					case FbxLayerElement::eAllSame:
						for (int i = 0; i < _PolyCount; i++)
						{
							_FaceMaterial[i] = leMat->GetIndexArray().GetAt(0);
						}
						break;
					case FbxLayerElement::eByPolygon:
						for (int i = 0; i < _PolyCount; i++)
						{
							_FaceMaterial[i] = leMat->GetIndexArray().GetAt(i);
						}
						break;
					case FbxLayerElement::eByControlPoint:
					case FbxLayerElement::eByEdge:
					case FbxLayerElement::eByPolygonVertex:
					case FbxLayerElement::eNone:
						break;
					default:
						break;
					}
					break;
				case FbxLayerElement::eDirect:
					throw gcnew Exception("material mapping Reference_Mode is Direct mapping");
					break;

				}
			}

		}

		void MeshLoader::LoadMaterials()
		{
			FbxNode*  lNode = _Mesh->GetNode();
			if(lNode) _MaterialCount = lNode->GetMaterialCount();  

			_Materials = new Material[_MaterialCount];
			
			for(int i=0;i<_MaterialCount;i++)
			{
				FbxSurfaceMaterial *lMaterial = lNode->GetMaterial(i);
				_Materials[i].Name = lMaterial->GetName();
				_Materials[i].Index = i;

				if (lMaterial->GetClassId().Is(FbxSurfacePhong::ClassId))
				{
					// We found a Phong material.
					_Materials[i].IsPhong = true;
					
					_Materials[i].Ambient = ((FbxSurfacePhong*) lMaterial)->Ambient.Get();
					_Materials[i].Diffuse = ((FbxSurfacePhong*) lMaterial)->Diffuse.Get();
					_Materials[i].Specular = ((FbxSurfacePhong*) lMaterial)->Specular.Get();					
					_Materials[i].Emissive = ((FbxSurfacePhong*) lMaterial)->Emissive.Get();

					_Materials[i].TransparencyFactor = ((FbxSurfacePhong *)lMaterial)->TransparencyFactor.Get();
					_Materials[i].Shininess = ((FbxSurfacePhong*) lMaterial)->Shininess.Get();
					_Materials[i].ReflectionFactor = ((FbxSurfacePhong*) lMaterial)->ReflectionFactor.Get();
					
					_Materials[i].AmbientTexture = GetFileTexture(((FbxSurfacePhong*) lMaterial)->Ambient);
					_Materials[i].DiffuseTexture = GetFileTexture(((FbxSurfacePhong*) lMaterial)->Diffuse);
					_Materials[i].SpecularTexture = GetFileTexture(((FbxSurfacePhong*) lMaterial)->Specular);
					_Materials[i].EmissiveTexture = GetFileTexture(((FbxSurfacePhong*) lMaterial)->Emissive);

				}
				else if(lMaterial->GetClassId().Is(FbxSurfaceLambert::ClassId) )
				{
					// We found a Lambert material.
					_Materials[i].IsPhong = false;

					_Materials[i].Ambient = ((FbxSurfaceLambert*) lMaterial)->Ambient.Get();
					_Materials[i].Diffuse = ((FbxSurfaceLambert*) lMaterial)->Diffuse.Get();
					_Materials[i].Emissive = ((FbxSurfaceLambert*) lMaterial)->Emissive.Get();

					_Materials[i].TransparencyFactor = ((FbxSurfaceLambert*)lMaterial)->TransparencyFactor.Get();

					_Materials[i].AmbientTexture = GetFileTexture(((FbxSurfaceLambert*) lMaterial)->Ambient);
					_Materials[i].DiffuseTexture = GetFileTexture(((FbxSurfaceLambert*) lMaterial)->Diffuse);					 
					_Materials[i].EmissiveTexture = GetFileTexture(((FbxSurfaceLambert*) lMaterial)->Emissive);
				}
				else // unknown material set default value
				{
					_Materials[i].IsPhong = false;

					_Materials[i].Ambient = FbxDouble3(0,0,0);
					_Materials[i].Diffuse = FbxDouble3(0,0,0);
					_Materials[i].Specular = FbxDouble3(0,0,0);
					_Materials[i].Emissive = FbxDouble3(0,0,0);
					_Materials[i].TransparencyFactor = 1.0f;
					_Materials[i].Shininess = 0;
					_Materials[i].ReflectionFactor = 0;
				}
			}
		}

		TextureInfo* MeshLoader::GetFileTexture(FbxPropertyT<FbxDouble3>& lProperty)
		{			
			if (lProperty.IsValid())
			{
				const int lTextureCount = lProperty.GetSrcObjectCount<FbxFileTexture>();
				if (lTextureCount)
				{
					const FbxFileTexture* lTexture = lProperty.GetSrcObject<FbxFileTexture>();
					if (lTexture )
					{	
						TextureInfo* info = new TextureInfo();	
						info->Name = lTexture->GetName();
						info->FileName = lTexture->GetFileName();
						info->TextureUse = lTexture->GetTextureUse();
						info->MaterialUse = lTexture->GetMaterialUse();
						info->MappingType = lTexture->GetMappingType();
						info->SwapUV = lTexture->GetSwapUV();
						info->Translation =  FbxDouble2( lTexture->GetTranslationU(),lTexture->GetTranslationV());
						info->Scale = FbxDouble2( lTexture->GetScaleU(),lTexture->GetScaleV());
						info->Rotation = FbxDouble2( lTexture->GetRotationU(),lTexture->GetRotationV());
						info->DefaultAlpha = lTexture->GetDefaultAlpha();
						info->BlendMode = lTexture->GetBlendMode();
						return info;
					}
				}
			}
			return NULL; 
		}

		void MeshLoader::GenerateBlendWeightIndices()
		{
			_WeightPerVertex = new VertexWeightArray[_ControlsPointsCount];
			for (int i = 0; i < _ControlsPointsCount; i++) _WeightPerVertex[i] =  VertexWeightArray();

			//for each bone in skin data we try to find bone in skeleton and add blend data to appropriate vertex
			for(int i=0; i<_BoneSkinCount; i++)            
			{				
				for (int j = 0; j < _BoneSkins[i].WeightCount; j++)
					_WeightPerVertex[_BoneSkins[i].Indices[j]].Array.Add(VertexWeight(_BoneSkins[i].Bone,_BoneSkins[i].Weights[j]));
			}

		}		

		void MeshLoader::Fill(MeshData* meshdata)
		{
			

			int index = 0;// helper index
			bool hasColor = _ColorCount > 0;
			bool hasSkin = _BoneSkinCount > 0;
			bool hasUv1 = _UvsCountA > 0;
			bool hasUv2 = _UvsCountB > 0;
			bool hasNormal =_NormalCount > 0;			

#pragma region Create faces

			Face* faces = new Face[_PolyCount];
			Face face;
			int polyindex = 0;
			for (int i = 0; i <_PolyCount; i++)
			{
				face.A = polyindex++; 
				face.B = polyindex++;
				face.C = polyindex++;
				face.MaterialId = _FaceMaterial[i];
				faces[i] = face;
			}
#pragma endregion

#pragma region Prepare for Optimization

			int vertexCount = _UvsCountA;
			if(_UvsCountB > _UvsCountA)
				vertexCount = _UvsCountB;

			Vertex* vertices = new Vertex[vertexCount];

			if (_ControlsPointsCount == vertexCount)//we have one uv for each vertex
			{
				for (int i = 0; i < vertexCount; i++)
				{
					vertices[i].Position = _ControlsPoints[i];
					if (hasSkin)
					{
						for(int j=0; j < _WeightPerVertex[i].Array.GetCount(); j++)
						{
							vertices[i].Bones.Add(_WeightPerVertex[i].Array.GetAt(j).Bone);
							vertices[i].Weights.Add(_WeightPerVertex[i].Array.GetAt(j).Weight);
						}
					}
					if( hasUv1)
						vertices[i].UV = _UvsA[i];

					if( hasUv2)
						vertices[i].UV2 = _UvsB[i];


				}
			}
			else // number of uvs is more than count of vertex, so
			{				
				index = 0;
				for (int i = 0; i < _PolyCount; i++)
				{															
					for (int k = 0; k < 3; k++)
					{
						face = faces[i];						
						//duplicate first vertex in polygon and correct polygon index A
						int controlPointindex =_PolygonIndices[index];
						vertices[index].Position = _ControlsPoints[controlPointindex];

						if (hasSkin)
						{
							for(int j=0; j <_WeightPerVertex[controlPointindex].Array.GetCount(); j++)
							{
								vertices[index].Bones.Add(_WeightPerVertex[controlPointindex].Array.GetAt(j).Bone);
								vertices[index].Weights.Add(_WeightPerVertex[controlPointindex].Array.GetAt(j).Weight);
							}
						}

						if( hasUv1)
							vertices[index].UV =_UvsA[index];
						if(hasUv2)
							vertices[index].UV2 =_UvsB[index];
						face[k] = index;						

						index++;
					}
					faces[i] = face;
				}
			}
			if (hasColor)
			{
				if ( _ColorCount ==_UvsCountA && _ColorCount ==_UvsCountB)
					for (int i = 0; i < vertexCount; i++)
						vertices[i].Color = _Colors[i];
				else
				{
					index = 0;
					for (int i = 0; i < _PolyCount; i++)
					{
						vertices[index].Color = _Colors[index++];
						vertices[index].Color = _Colors[index++];
						vertices[index].Color = _Colors[index++];
					}
				}
			}

			if (hasNormal)
			{
				if (_NormalCount ==_UvsCountA && ( _UvsCountB == 0 || _NormalCount == _UvsCountB ))
					for (int i = 0; i < vertexCount; i++)
						vertices[i].Normal = _Normals[i];
				else
				{
					index = 0;
					for (int i = 0; i < _PolyCount; i++)
					{
						vertices[index].Normal = _Normals[index++];
						vertices[index].Normal= _Normals[index++];
						vertices[index].Normal= _Normals[index++];
					}
				}
			}
#pragma endregion

			int i = 0;

			meshdata->Vertices.Reserve(vertexCount);
			for( i = 0 ; i < vertexCount; i++)
			{
				Vertex v = vertices[i];
				meshdata->Vertices.SetAt(i,v);				
			}
			for( i = 0; i <_PolyCount; i++)
			{
				meshdata->Faces.Add(faces[i]);
			}
			
			for( i=0; i < _MaterialCount; i++)
			{				
				meshdata->Materials.Add(_Materials[i]);
			}


			meshdata->HasUV2 = hasUv2;
			meshdata->HasColor = hasColor;
			meshdata->HasSkin = hasSkin;
			meshdata->HasTangent = false;
			meshdata->HasNormal = hasNormal;
			delete[] faces;
			delete[] vertices;
		}


		
	}
}