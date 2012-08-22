#include "Stdafx.h"
#include "MeshCreator.h"
#include "Primitives.h"
#include "FbxHelper.h"

namespace Skill
{
	namespace Fbx
	{
		FbxNode* MeshCreator::Create(FbxScene* scene , const char* meshName, MeshData& meshData)
		{				
			FbxMesh* lMesh = FbxMesh::Create(scene,meshName);

			int vertexCount = meshData.Vertices.GetCount();

			// Create control points.			
			lMesh->InitControlPoints(vertexCount);
			FbxVector4* lControlPoints = lMesh->GetControlPoints();

			
			// We want to have one normal for each vertex (or control point),
			// so we set the mapping mode to eByControlPoint.
			FbxGeometryElementNormal* lGeometryElementNormal =  lMesh->CreateElementNormal();
			lGeometryElementNormal->SetMappingMode(FbxGeometryElement::eByControlPoint);
			//// Set the normal values for every control point.
			lGeometryElementNormal->SetReferenceMode(FbxGeometryElement::eDirect);
			lGeometryElementNormal->GetDirectArray().SetCount(vertexCount);
			


			// Create UV for channel 0.
			FbxGeometryElementUV* lUVFirstElement = lMesh->CreateElementUV("UV");
			FBX_ASSERT( lUVFirstElement != NULL);
			lUVFirstElement->SetMappingMode(FbxGeometryElement::eByControlPoint);
			lUVFirstElement->SetReferenceMode(FbxGeometryElement::eDirect);			
			lUVFirstElement->GetDirectArray().SetCount(vertexCount);

			FbxGeometryElementUV* lUVSecondElement = NULL;
			if(meshData.HasUV2)
			{
			// Create UV for Local channel.
				lUVSecondElement = lMesh->CreateElementUV("UV2");
				FBX_ASSERT( lUVSecondElement != NULL);
				lUVSecondElement->SetMappingMode(FbxGeometryElement::eByControlPoint);
				lUVSecondElement->SetReferenceMode(FbxGeometryElement::eDirect);			
				lUVSecondElement->GetDirectArray().SetCount(vertexCount);	
			}

			FbxGeometryElementVertexColor* lColorElement;
			if(meshData.HasColor)
			{
				lColorElement = lMesh->CreateElementVertexColor();
				FBX_ASSERT( lColorElement != NULL);
				lColorElement->SetMappingMode(FbxGeometryElement::eByControlPoint);
				lColorElement->SetReferenceMode(FbxGeometryElement::eDirect);			
				lColorElement->GetDirectArray().SetCount(vertexCount);	
			}

			FbxGeometryElementTangent* lTangentElement;
			FbxGeometryElementBinormal* lBinormalElement;
			if(meshData.HasTangent)
			{
				lTangentElement = lMesh->CreateElementTangent();				
				FBX_ASSERT( lTangentElement != NULL);
				lTangentElement->SetMappingMode(FbxGeometryElement::eByControlPoint);
				lTangentElement->SetReferenceMode(FbxGeometryElement::eDirect);			
				lTangentElement->GetDirectArray().SetCount(vertexCount);	

				lBinormalElement = lMesh->CreateElementBinormal();
				FBX_ASSERT( lBinormalElement != NULL);
				lBinormalElement->SetMappingMode(FbxGeometryElement::eByControlPoint);
				lBinormalElement->SetReferenceMode(FbxGeometryElement::eDirect);			
				lBinormalElement->GetDirectArray().SetCount(vertexCount);	
			}


			int i,j;			
			for (i = 0; i < vertexCount; i++)
			{
				Vertex vertex = meshData.Vertices.GetAt(i);

				lControlPoints[i] = vertex.Position;					
				lGeometryElementNormal->GetDirectArray().SetAt(i , vertex.Normal);
				lUVFirstElement->GetDirectArray().SetAt(i , vertex.UV);	
				if(meshData.HasUV2)				
					lUVSecondElement->GetDirectArray().SetAt(i , vertex.UV2);
				if(meshData.HasColor)
					lColorElement->GetDirectArray().SetAt(i , vertex.Color);
				if(meshData.HasTangent)
				{
					lTangentElement->GetDirectArray().SetAt(i , vertex.Tangent);
					lBinormalElement->GetDirectArray().SetAt(i , vertex.Binormal);
				}
				
			}																												

			int faceCount = meshData.Faces.GetCount();

			//Create polygons. Assign texture and texture UV indices.
			for (i = 0; i < faceCount; i++)
			{
				Face face = meshData.Faces[i];

				// all faces of the cube have the same texture
				lMesh->BeginPolygon(face.MaterialId, -1, -1, false);

				// Control point index
				lMesh->AddPolygon(face.A);
				lMesh->AddPolygon(face.B);
				lMesh->AddPolygon(face.C);

				lMesh->EndPolygon ();
			}			

			// create a FbxNode
			FbxNode* lNode = FbxNode::Create(scene,meshName);

			// set the node attribute
			lNode->SetNodeAttribute(lMesh);

			// set the cube position
			lNode->LclTranslation.Set(FbxDouble3(0, 0, 0));			
			lNode->LclScaling.Set(FbxDouble3(1, 1, 1));
			lNode->LclRotation.Set(FbxDouble3(0, 0, 0));

			// set the shading mode to view texture
			lNode->SetShadingMode(FbxNode::eTextureShading);

			CreateMaterialMapping(lMesh , meshData);
			if(_CreateSkin)
			{
				LinkMeshToSkeleton(scene , lNode , meshData );
				StoreBindPose(scene, lNode, meshData);
			}
			// return the FbxNode
			return lNode;
		}

		void MeshCreator::CreateMaterialMapping(FbxMesh* pMesh , MeshData& meshData)
		{			
			// Set material mapping.
			FbxGeometryElementMaterial* lMaterialElement = pMesh->CreateElementMaterial();
			lMaterialElement->SetMappingMode(FbxGeometryElement::eByPolygon);
			lMaterialElement->SetReferenceMode(FbxGeometryElement::eIndexToDirect);						
			
			lMaterialElement->GetIndexArray().SetCount(meshData.Faces.GetCount());

			int faceCount = meshData.Faces.GetCount();
			// Set the Index 0 to polyCount to the material in position 0 of the direct array.
			for(int i=0; i< faceCount; ++i)
				lMaterialElement->GetIndexArray().SetAt(i,meshData.Faces[i].MaterialId);
		}

		void MeshCreator::LinkMeshToSkeleton(FbxScene* scene , FbxNode* lNode , MeshData& meshData )
		{
			
			if(meshData.HasSkin)
			{
				int i,j;
				// these two lists are correspond eachother
				FbxArray<FbxNode*> bonesArray; // list to hold reference of all bones
				FbxArray<FbxCluster*> clustersArray; // list to hold reference of all clusters
				FbxCluster* cluster = NULL;
				FbxNode* bone = NULL;

				int vertexCount = meshData.Vertices.GetCount();

				for (int vIndex = 0; vIndex < vertexCount; vIndex++) // travers through all vertex in mesh
				{
					Vertex vertex = meshData.Vertices.GetAt(vIndex);
					int boneCount = vertex.Bones.GetCount();
					if(boneCount > 0)
					{
						for (int bIndex = 0; bIndex < boneCount ; bIndex++) // for each bones that this vertex is influenced
						{
							cluster = NULL;
							bone = vertex.Bones.GetAt(bIndex);
							int findIndex = bonesArray.Find(bone); // if we added this node to list before - - whether one of previous nodes influenced by this node
							if(findIndex < 0) // we see this node first time
							{
								cluster = FbxCluster::Create(scene,""); // create cluster and link to bone
								bonesArray.Add(bone);
								clustersArray.Add(cluster);

								const char* name = bone->GetName();

								cluster->SetLink(bone);
								cluster->SetLinkMode(FbxCluster::eTotalOne);
							}
							else // we saw it before and is already in list
							{
								cluster = clustersArray.GetAt(findIndex); // get correspondant cluster
							}
							if(cluster)
								cluster->AddControlPointIndex(vIndex,vertex.Weights.GetAt(bIndex)); // add vertex weight to cluster
						}
					}
				}

				// Now we have the Patch and the skeleton correctly positioned,
				// set the Transform and TransformLink matrix accordingly.

				FbxAMatrix lXMatrix;
				lXMatrix = lNode->EvaluateGlobalTransform();

				int clusterCount = clustersArray.GetCount();

				for (i = 0; i < clusterCount; i++)
				{					
					cluster = clustersArray.GetAt(i);
					cluster ->SetTransformMatrix(lXMatrix);					
				}
				
				for (i = 0; i < clusterCount; i++)
				{
					bone = bonesArray.GetAt(i);
					cluster = clustersArray.GetAt(i);
					lXMatrix = bone->EvaluateGlobalTransform();
					cluster->SetTransformLinkMatrix(lXMatrix);
				}				


				// Add the clusters to the mesh by creating a skin and adding those clusters to that skin.
				// After add that skin.

				FbxGeometry* lMeshhAttribute = (FbxGeometry*) lNode->GetNodeAttribute();
				FbxSkin* lSkin = FbxSkin::Create(scene, "");

				for (i = 0; i < clusterCount; i++)
				{					
					cluster = clustersArray.GetAt(i);
					lSkin->AddCluster(cluster);				
				}				

				lMeshhAttribute->AddDeformer(lSkin);
			}
		}

		void MeshCreator::StoreBindPose(FbxScene* scene, FbxNode* node, MeshData& meshData )
		{
			// In the bind pose, we must store all the link's global matrix at the time of the bind.
			// Plus, we must store all the parent(s) global matrix of a link, even if they are not
			// themselves deforming any model.

			// In this example, since there is only one model deformed, we don't need walk through 
			// the scene
			//

			// Now list the all the link involve in the patch deformation
			FbxArray<FbxNode*> lClusteredFbxNodes;
			int i, j;


			int lSkinCount=0;
			int lClusterCount=0;


			lSkinCount = ((FbxGeometry*)node->GetNodeAttribute())->GetDeformerCount(FbxDeformer::eSkin);
			//Go through all the skins and count them
			//then go through each skin and get their cluster count
			for(i=0; i<lSkinCount; ++i)
			{
				FbxSkin *lSkin=(FbxSkin*)((FbxGeometry*)node->GetNodeAttribute())->GetDeformer(i, FbxDeformer::eSkin);
				lClusterCount+=lSkin->GetClusterCount();
			}			

			//if we found some clusters we must add the node
			if (lClusterCount)
			{
				//Again, go through all the skins get each cluster link and add them
				for (i=0; i<lSkinCount; ++i)
				{
					FbxSkin *lSkin=(FbxSkin*)((FbxGeometry*)node->GetNodeAttribute())->GetDeformer(i, FbxDeformer::eSkin);
					lClusterCount=lSkin->GetClusterCount();
					for (j=0; j<lClusterCount; ++j)
					{
						FbxNode* lClusterNode = lSkin->GetCluster(j)->GetLink();
						AddNodeRecursively(lClusteredFbxNodes, lClusterNode);
					}
				}

				// Add the patch to the pose
				lClusteredFbxNodes.Add(node);
			}


			// Now create a bind pose with the link list
			if (lClusteredFbxNodes.GetCount())
			{
				// A pose must be named. Arbitrarily use the name of the patch node.
				FbxPose* lPose = FbxPose::Create(scene,node->GetName());

				// default pose type is rest pose, so we need to set the type as bind pose
				lPose->SetIsBindPose(true);

				for (i=0; i<lClusteredFbxNodes.GetCount(); i++)
				{
					FbxNode*  lKFbxNode   = lClusteredFbxNodes.GetAt(i);
					FbxMatrix lBindMatrix = lKFbxNode->EvaluateGlobalTransform();

					lPose->Add(lKFbxNode, lBindMatrix);
				}

				// Add the pose to the scene
				scene->AddPose(lPose);
			}
		}		

		// Store a Rest Pose
		void MeshCreator::StoreRestPose(FbxScene* pScene, FbxNode* pSkeletonRoot)
		{
			// This example show an arbitrary rest pose assignment.
			// This rest pose will set the bone rotation to the same value 
			// as time 1 second in the first stack of animation, but the 
			// position of the bone will be set elsewhere in the scene.
			FbxString     lNodeName;
			FbxNode*   lKFbxNode;
			FbxMatrix  lTransformMatrix;
			FbxVector4 lT,lR,lS(1.0, 1.0, 1.0);

			// Create the rest pose
			FbxPose* lPose = FbxPose::Create(pScene,"A Bind Pose");

			// Set the skeleton root node to the global position (0, 0, 0)
			// and global rotation of along the Z axis.
			lT.Set(0.0, 0.0, 0.0);
			lR.Set( 0.0,  0.0, 0.0);

			lTransformMatrix.SetTRS(lT, lR, lS);

			// Add the skeleton root node to the pose
			lKFbxNode = pSkeletonRoot;
			lPose->Add(lKFbxNode, lTransformMatrix, false /*it's a global matrix*/);

			// Set the lLimbNode1 node to the local position of (0, 40, 0)
			// and local rotation of -90deg along the Z axis. This show that
			// you can mix local and global coordinates in a rest pose.
			lT.Set(0.0, 40.0,   0.0);
			lR.Set(0.0,  0.0, -90.0);

			lTransformMatrix.SetTRS(lT, lR, lS);

			// Add the skeleton second node to the pose
			lKFbxNode = lKFbxNode->GetChild(0);
			lPose->Add(lKFbxNode, lTransformMatrix, true /*it's a local matrix*/);

			// Set the lLimbNode2 node to the local position of (0, 40, 0)
			// and local rotation of 45deg along the Z axis.
			lT.Set(0.0, 40.0, 0.0);
			lR.Set(0.0,  0.0, 45.0);

			lTransformMatrix.SetTRS(lT, lR, lS);

			// Add the skeleton second node to the pose
			lKFbxNode = lKFbxNode->GetChild(0);
			lNodeName = lKFbxNode->GetName();
			lPose->Add(lKFbxNode, lTransformMatrix, true /*it's a local matrix*/);

			// Now add the pose to the scene
			pScene->AddPose(lPose);
		}

		// Add the specified node to the node array. Also, add recursively
		// all the parent node of the specified node to the array.
		void MeshCreator::AddNodeRecursively(FbxArray<FbxNode*>& pNodeArray, FbxNode* pNode)
		{
			if (pNode)
			{
				AddNodeRecursively(pNodeArray, pNode->GetParent());

				if (pNodeArray.Find(pNode) == -1)
				{
					// Node not in the list, add it
					pNodeArray.Add(pNode);
				}
			}
		}
	}
}