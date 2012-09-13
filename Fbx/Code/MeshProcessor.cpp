#include "Stdafx.h"
#include "MeshProcessor.h"
#include "SceneNode.h"
#include "Primitives.h"
#include "FbxHelper.h"
#include "Matrix.h"

namespace Skill
{
	namespace Fbx
	{
		

		MeshProcessor::MeshProcessor()
		{
			_PositionsTelorance = 0.02f;
			_UvTelorance = 0.01f;
		}				
		

		void MeshProcessor::ReduceVertex(MeshData& mesh  )
		{	
			int vertexCount = mesh.Vertices.GetCount();
			int faceCount = mesh.Faces.GetCount();

			// create two dimension array as follow
            // dimension 0 specifies that each vertex is same as witch vertex
            // dimension 1 specifies that each vertex is at witch location index in optimized array
            int* references = new int[vertexCount];
            int* opIndex = new int[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                references[i] = opIndex[i] = -1;

            for (int i = 0; i < vertexCount; i++)
            {
                if (references[i] == -1)// if this vertex is not same as any of latter vertices
                {
                    references[i] = i;//set vertex as reference
                    //go through next vertices and check witch vertex is same as current vertex 
                    for (int j = i + 1; j < vertexCount; j++)
                    {
                        if (references[j] == -1)// if vertex is not same as any latter vertices
                        {
                            //check if two vertex are enough close
                            if (FbxHelper::IsEqual(mesh.Vertices[i].Position, mesh.Vertices[j].Position, _PositionsTelorance))
                                if (FbxHelper::IsEqual(mesh.Vertices[i].UV, mesh.Vertices[j].UV, _UvTelorance))
                                    references[j] = i;//set vertex is same as reference vertex
                        }
                    }
                }
            }

            int max = 0;// maximum usable vertex in optimized vertices
            for (int polIndex = 0; polIndex < faceCount; polIndex++)
            {
                Face face = mesh.Faces[polIndex];
                for (int i = 0; i < 3; i++)
                {
                    int refer = references[face[i]];//find reference vertex face[i]
                    if (opIndex[refer] == -1)// if this vertex did not place in optimized vertices latter
                    {
                        mesh.Vertices[max] = mesh.Vertices[face[i]];//place vertex in next free space in optimized vertices
                        face[i] = opIndex[refer] = max++;//correct index in opIndex and face
                    }
                    else face[i] = opIndex[refer];//refer to first optimized vertex
                }
                mesh.Faces[polIndex] = face;// save optimized face
            }

			delete[] references;
			delete[] opIndex;

			mesh.Vertices.SetCount(max);
		}

		MeshData* MeshProcessor::ReduceFace(MeshData& mesh)
		{
			return NULL;
		}
		
        void MeshProcessor::GenerateNormals(MeshData& meshData)
		{
			int i;
			int p = 0;			

			int vertexCount = meshData.Vertices.GetCount();
			int faceCount = meshData.Faces.GetCount();

			for (i = 0; i < vertexCount; i++)
				meshData.Vertices[i].Normal  = FbxVector4(0,0,0);											

			for (i = 0; i < faceCount; i++)
			{
				Face face = meshData.Faces[i];

				FbxVector4 v1 =  meshData.Vertices[face.A].Position;
				FbxVector4 v2 = meshData.Vertices[face.B].Position;
				FbxVector4 v3 = meshData.Vertices[face.C].Position;

				FbxVector4 v21 = v2 - v1;
				FbxVector4 v32 = v3 - v2;
				FbxVector4 cross = v21.CrossProduct(v32);

				FbxVector4 normal = FbxHelper::SafeNormalize(cross);


				meshData.Vertices[face.A].Normal += normal;
				meshData.Vertices[face.B].Normal += normal;
				meshData.Vertices[face.C].Normal += normal;				

			}				

			for (int i = 0; i < vertexCount; i++)
            {
				meshData.Vertices[i].Normal  = FbxHelper::SafeNormalize(meshData.Vertices[i].Normal);                
            }
		}

		void MeshProcessor::GenerateTangentAndBinormals(MeshData& meshData)
		{
			FbxVector4 tangents[3];

			int vertexCount = meshData.Vertices.GetCount();
			int faceCount = meshData.Faces.GetCount();

			for (int i = 0; i < vertexCount; i++)			
				meshData.Vertices[i].Tangent = FbxVector4();							

            for (int i = 0; i < faceCount; i++)
            {
                Face face = meshData.Faces[i];
                //create 3 tangent for each polygon
                CreateTangent(meshData.Vertices[face.A], meshData.Vertices[face.B] , meshData.Vertices[face.C], tangents);

                //save result
                meshData.Vertices[face.A].Tangent += tangents[0];
                meshData.Vertices[face.B].Tangent += tangents[1];
                meshData.Vertices[face.C].Tangent += tangents[2];
            }

            for (int i = 0; i < vertexCount - 1; i++)
            {
                for (int j = i + 1; j < vertexCount; j++)
                {
                    if (FbxHelper::IsEqual(meshData.Vertices[i].Position, meshData.Vertices[j].Position, _PositionsTelorance))
                    {
                        FbxVector4 tangent = meshData.Vertices[i].Tangent;
                        meshData.Vertices[i].Tangent += meshData.Vertices[j].Tangent;
                        meshData.Vertices[j].Tangent += tangent;
                    }
                }
            }

            for (int i = 0; i < vertexCount; i++)
            {
                meshData.Vertices[i].Tangent = FbxHelper::SafeNormalize(meshData.Vertices[i].Tangent);
				meshData.Vertices[i].Binormal = meshData.Vertices[i].Tangent.CrossProduct(meshData.Vertices[i].Normal);
            }
		}

		void MeshProcessor::CreateTangent(Vertex& vertex0, Vertex& vertex1, Vertex& vertex2, FbxVector4* tangentArray)
        {
            FbxVector4 edge1;
            FbxVector4 edge2;
            FbxVector4 crossP;

			FbxVector4 v0 = vertex0.Position;
			FbxVector4 v1 = vertex1.Position;
			FbxVector4 v2 = vertex2.Position;

			FbxVector4 n0 = vertex0.Normal;
			FbxVector4 n1 = vertex1.Normal;
			FbxVector4 n2 = vertex2.Normal;

			FbxVector2 t0 = vertex0.UV;
			FbxVector2 t1 = vertex1.UV;
			FbxVector2 t2 = vertex2.UV;

            //==============================================
            // x, s, t
            // S & T vectors get used several times in this vector,
            // but are only computed once.
            //==============================================
            edge1[0] = v1[0] - v0[0];
            edge1[1] = t1[0] - t0[0]; // s-vector - don't need to compute this multiple times
            edge1[2] = t1[1] - t0[1]; // t-vector

            edge2[0] = v2[0] - v0[0];
            edge2[1] = t2[0] - t0[0]; // another s-vector
            edge2[2] = t2[2] - t0[2]; // another t-vector

            crossP = edge1.CrossProduct(edge2);
            crossP.Normalize();

            bool degnerateUVTangentPlane = abs(crossP[0]) < 0.001;
            if (degnerateUVTangentPlane)
                crossP[0] = 1.0f;

            double tanX = -crossP[1] / crossP[0];

            tangentArray[0][0] = tanX;
            tangentArray[1][0] = tanX;
            tangentArray[2][0] = tanX;

            ////--------------------------------------------------------
            //// y, s, t
            ////--------------------------------------------------------
            edge1[0] = v1[1] - v0[1];

            edge2[0] = v2[1] - v0[1];
            edge2[1] = t2[0] - t0[0];
            edge2[2] = t2[1] - t0[1];

            crossP = edge1.CrossProduct(edge2);
            crossP.Normalize();

            degnerateUVTangentPlane = abs(crossP[0]) < 0.001;
            if (degnerateUVTangentPlane)
                crossP[0] = 1.0f;

            double tanY = -crossP[1] / crossP[0];

            tangentArray[0][1] = tanY;
            tangentArray[1][1] = tanY;
            tangentArray[2][1] = tanY;

            ////------------------------------------------------------
            //// z, s, t
            ////------------------------------------------------------
            edge1[0] = v1[2] - v0[2];

            edge2[0] = v2[2] - v0[2];
            edge2[1] = t2[0] - t0[0];
            edge2[2] = t2[1] - t0[1];

            crossP = edge1.CrossProduct(edge2);
            crossP.Normalize();

            degnerateUVTangentPlane = abs(crossP[0]) < 0.001;
            if (degnerateUVTangentPlane)
                crossP[0] = 1.0f;

            double tanZ = -crossP[1] / crossP[0];

            tangentArray[0][2] = tanZ;
            tangentArray[1][2] = tanZ;
            tangentArray[2][2] = tanZ;

            tangentArray[0] -= n0 * tangentArray[0].DotProduct(n0);
            tangentArray[1] -= n1 * tangentArray[1].DotProduct(n1);
            tangentArray[2] -= n2 * tangentArray[2].DotProduct(n2);

            //// Normalize tangents
            tangentArray[0].Normalize();
            tangentArray[1].Normalize();
            tangentArray[2].Normalize();

        }


		MeshData* MeshProcessor::Merge(FbxArray<MeshData*>& meshes)
		{
			int i,j,k;
			int totalVertex = 0;
			int totalFace = 0;
			
			for(i = 0; i < meshes.GetCount(); i++)
			{
				MeshData* mesh = meshes.GetAt(i);
				totalVertex += mesh->Vertices.GetCount(); // totalvertex = sum of all vertex in all meshes
				totalFace += mesh->Faces.GetCount();// totalface = sum of all face in all meshes
			}

			MeshData* result = new MeshData(); // create result MeshData

			result->Vertices.Reserve(totalVertex); // reserve space for all vertex
			result->Faces.Reserve(totalFace); // reserve space for all face

			// merge materials of all mesh to single list and remove duplicates
			for(i = 0; i < meshes.GetCount(); i++)
			{
				MeshData* mesh = meshes.GetAt(i);
				for(j = 0;j < mesh->Materials.GetCount(); j++) // go through materials of mesh
				{
					FbxString newMatName = mesh->Materials.GetAt(j).Name;
					bool exist = false;
					for(k = 0; k < result->Materials.GetCount(); k++) // go through new materials
					{
						FbxString matName = result->Materials.GetAt(k).Name;
						if(newMatName == matName)// check wether we add this material to list before ( two materials can not have same names in fbx file )
						{
							exist = true;
							break;
						}
					}
					if(!exist) // if we did not add this material to list
					{
						Material newMat = Material();
						newMat.Name = newMatName;
						result->Materials.Add(newMat); // add material to list of result materials
					}
				}
			}

			const int MaxMaterial = 50; // suppose that maximum number of materials for each mesh does not exited from 50
			int vIndex = 0; // index of current modifing vertex
			int fIndex = 0; // index of current modifing face
			int materialMapping[MaxMaterial]; // new material mapping - materialMapping[i] = index of new material in result mesh for materialId i

			int vertexFaceOffset = 0;

			for(i = 0; i < meshes.GetCount(); i++)
			{
				MeshData* mesh = meshes.GetAt(i);	
				result->HasUV2 |= mesh->HasUV2;
				result->HasTangent |= mesh->HasTangent;
				result->HasColor |= mesh->HasColor;		
				result->HasSkin |= mesh->HasSkin;

				for(j = 0;j < mesh->Vertices.GetCount(); j++) // copy all vertex at end of result vertex
				{
					Vertex v = mesh->Vertices.GetAt(j);
					result->Vertices.SetAt(vIndex,v);
					vIndex++;
				}

				for(j = 0; j < MaxMaterial; j++) materialMapping[j] = -1; // reset material mappings

				for(j = 0;j < mesh->Materials.GetCount(); j++) // for each material in mesh
				{
					FbxString meshMatName = mesh->Materials.GetAt(j).Name;					
					for(k = 0; k < result->Materials.GetCount(); k++) // for each material in result mesh
					{
						FbxString resultMatName = result->Materials.GetAt(k).Name;
						if(meshMatName == resultMatName) // find material in result material
						{
							materialMapping[j] = k; // specify that all face with MaterialId : j, will map to MaterialId : k
							break;
						}
					}					
				}

				for(j = 0;j < mesh->Faces.GetCount(); j++) // copy all face at end of result face
				{
					Face f = mesh->Faces.GetAt(j);
					f.A += vertexFaceOffset;
					f.B += vertexFaceOffset;					
					f.C += vertexFaceOffset;
					f.MaterialId = materialMapping[f.MaterialId]; // remap MaterialId to new MaterialId
					result->Faces.SetAt(fIndex,f);
					fIndex++;
				}

				vertexFaceOffset += mesh->Vertices.GetCount();
			}

			return result;
		}

		void MeshProcessor::TransformToGlobal(FbxNode* node, MeshData& meshData)
		{
			Matrix gTransform = Matrix::GlobalNode(node);

			int vertexCount = meshData.Vertices.GetCount();
			
			for( int i = 0; i < vertexCount; i++ )
			{
				Vertex v = meshData.Vertices.GetAt(i);

				v.Position = Matrix::Transform(v.Position,gTransform );
				if(meshData.HasNormal)
					v.Normal = Matrix::TransformNormal(v.Normal,gTransform );

				meshData.Vertices.SetAt(i,v);
			}
		}

		void MeshProcessor::TransformToLocal(FbxNode* node, MeshData& meshData)
		{
			Matrix gTransform = Matrix::LocalNode(node);

			int vertexCount = meshData.Vertices.GetCount();
			
			for( int i = 0; i < vertexCount; i++ )
			{
				Vertex v = meshData.Vertices.GetAt(i);

				v.Position = Matrix::Transform(v.Position,gTransform );
				if(meshData.HasNormal)
					v.Normal = Matrix::TransformNormal(v.Normal,gTransform );

				meshData.Vertices.SetAt(i,v);
			}
		}
		
		void MeshProcessor::TransformToBindPose(FbxMesh* mesh,MeshData& meshData)
		{

			FbxSkin* lSkin = reinterpret_cast<FbxSkin*>(mesh->GetDeformer(0, FbxDeformer::eSkin));
			if(!lSkin) return;
			
			FbxCluster* lCluster = lSkin->GetCluster(0);
			FbxCluster::ELinkMode lClusterMode = lCluster->GetLinkMode();

			FbxAMatrix lReferenceGlobalInitPosition;
			FbxAMatrix lReferenceGlobalCurrentPosition;
			FbxAMatrix lAssociateGlobalInitPosition;
			FbxAMatrix lAssociateGlobalCurrentPosition;
			FbxAMatrix lClusterGlobalInitPosition;
			FbxAMatrix lClusterGlobalCurrentPosition;
			FbxAMatrix  vertexTransformMatrix;

			FbxAMatrix lReferenceGeometry;
			FbxAMatrix lAssociateGeometry;
			FbxAMatrix lClusterGeometry;

			FbxAMatrix lClusterRelativeInitPosition;
			FbxAMatrix lClusterRelativeCurrentPositionInverse;
			

			lReferenceGlobalCurrentPosition.SetIdentity();

			if (lClusterMode == FbxCluster::eAdditive && lCluster->GetAssociateModel())
			{
				lCluster->GetTransformAssociateModelMatrix(lAssociateGlobalInitPosition);
				// Geometric transform of the model
				lAssociateGeometry = FbxHelper::GetGeometry(lCluster->GetAssociateModel());
				lAssociateGlobalInitPosition *= lAssociateGeometry;
				lAssociateGlobalCurrentPosition = FbxHelper::GetGlobalPosition(lCluster->GetAssociateModel(), FBXSDK_TIME_ZERO);

				lCluster->GetTransformMatrix(lReferenceGlobalInitPosition);
				// Multiply lReferenceGlobalInitPosition by Geometric Transformation
				lReferenceGeometry = FbxHelper::GetGeometry(mesh->GetNode());
				lReferenceGlobalInitPosition *= lReferenceGeometry;				

				// Get the link initial global position and the link current global position.
				lCluster->GetTransformLinkMatrix(lClusterGlobalInitPosition);
				// Multiply lClusterGlobalInitPosition by Geometric Transformation
				lClusterGeometry = FbxHelper::GetGeometry(lCluster->GetLink());
				lClusterGlobalInitPosition *= lClusterGeometry;
				lClusterGlobalCurrentPosition = FbxHelper::GetGlobalPosition(lCluster->GetLink(), FBXSDK_TIME_ZERO);

				// Compute the shift of the link relative to the reference.
				//ModelM-1 * AssoM * AssoGX-1 * LinkGX * LinkM-1*ModelM
				vertexTransformMatrix = lReferenceGlobalInitPosition.Inverse() * lAssociateGlobalInitPosition * lAssociateGlobalCurrentPosition.Inverse() *
					lClusterGlobalCurrentPosition * lClusterGlobalInitPosition.Inverse() * lReferenceGlobalInitPosition;
			}
			else
			{

				lCluster->GetTransformMatrix(lReferenceGlobalInitPosition);
								
				// Multiply lReferenceGlobalInitPosition by Geometric Transformation
				lReferenceGeometry = FbxHelper::GetGeometry(mesh->GetNode());
				lReferenceGlobalInitPosition *= lReferenceGeometry;

				// Get the link initial global position and the link current global position.
				lCluster->GetTransformLinkMatrix(lClusterGlobalInitPosition);
				lClusterGlobalCurrentPosition = FbxHelper::GetGlobalPosition(lCluster->GetLink(), FBXSDK_TIME_ZERO);

				// Compute the initial position of the link relative to the reference.
				lClusterRelativeInitPosition = lClusterGlobalInitPosition.Inverse() * lReferenceGlobalInitPosition;

				// Compute the current position of the link relative to the reference.
				lClusterRelativeCurrentPositionInverse = lReferenceGlobalCurrentPosition.Inverse() * lClusterGlobalCurrentPosition;

				// Compute the shift of the link relative to the reference.
				vertexTransformMatrix = lClusterRelativeCurrentPositionInverse * lClusterRelativeInitPosition;
			}
			
			int vertexCount = meshData.Vertices.GetCount();
			
			for( int i = 0; i < vertexCount; i++ )
			{

				Vertex v = meshData.Vertices.GetAt(i);						
				v.Position = vertexTransformMatrix.MultT(v.Position);												
				
				//v.Position = Matrix::Transform(v.Position,gTransform );
				//if(meshData.HasNormal)
					//v.Normal = Matrix::TransformNormal(v.Normal,gTransform );

				meshData.Vertices.SetAt(i,v);
			}
		}

		
	}
}
