#include "Stdafx.h"
#include "FbxHelper.h"

#ifdef IOS_REF
#undef  IOS_REF
#define IOS_REF (*(pSdkManager->GetIOSettings()))
#endif

namespace Skill
{
	namespace Fbx
	{
		void FbxHelper::GetVersion(FbxVersion version , int* lMajor, int* lMinor, int* lRevision)
		{
			if(version == FbxVersion::V2013)		{ *lMajor = 2013; *lMinor = 0; *lRevision = 0; }
			else if(version == FbxVersion::V2012)   { *lMajor = 2012; *lMinor = 0; *lRevision = 0; }			
			else if(version == FbxVersion::V2011)   { *lMajor = 2011; *lMinor = 0; *lRevision = 0; }			
			else if(version == FbxVersion::V2010)   { *lMajor = 2010; *lMinor = 0; *lRevision = 0; }			
			else if(version == FbxVersion::V2009)   { *lMajor = 2009; *lMinor = 0; *lRevision = 0; }
			else if(version == FbxVersion::V2006)   { *lMajor = 2006; *lMinor = 0; *lRevision = 0; }
			else									{ *lMajor = 2013; *lMinor = 2; *lRevision = 0; }

		}

		void FbxHelper::ConvertScene(FbxScene* scene,FbxAxis axis, FbxSystemUnits unit)
		{
			FbxSystemUnit::ConversionOptions lConversionOptions;
			lConversionOptions.mConvertRrsNodes = false; /* mConvertRrsNodes */
			lConversionOptions.mConvertLimits = true; /* mConvertAllLimits */
			lConversionOptions.mConvertClusters  = true; /* mConvertClusters */
			lConversionOptions.mConvertLightIntensity = true; /* mConvertLightIntensity */
			lConversionOptions.mConvertPhotometricLProperties = true; /* mConvertPhotometricLProperties */
			lConversionOptions.mConvertCameraClipPlanes = true; //true  /* mConvertCameraClipPlanes */  };

			switch(unit)
			{
			case FbxSystemUnits::Inches :
				FbxSystemUnit::Inch.ConvertScene(scene,lConversionOptions);
				break;					
			case FbxSystemUnits::Feet :
				FbxSystemUnit::Foot.ConvertScene(scene,lConversionOptions);
				break;					
			case FbxSystemUnits::Yards :
				FbxSystemUnit::Yard.ConvertScene(scene,lConversionOptions);
				break;
			case FbxSystemUnits::Miles :
				FbxSystemUnit::Mile.ConvertScene(scene,lConversionOptions);
				break;
			case FbxSystemUnits::Millimeters :
				FbxSystemUnit::mm.ConvertScene(scene,lConversionOptions);
				break;
			case FbxSystemUnits::Centimeters :
				FbxSystemUnit::cm.ConvertScene(scene,lConversionOptions);
				break;
			case FbxSystemUnits::Meters :
				FbxSystemUnit::m.ConvertScene(scene,lConversionOptions);
				break;					
			case FbxSystemUnits::Kilometers :
				FbxSystemUnit::km.ConvertScene(scene,lConversionOptions);
				break;
			}

			switch(axis)
			{
			case FbxAxis::YUp :
				FbxAxisSystem::MayaYUp.ConvertScene(scene);
				break;					
			case FbxAxis::ZUp :
				FbxAxisSystem::MayaZUp.ConvertScene(scene);
				break;					
			}
		}

		FbxVector4 FbxHelper::SafeNormalize(FbxVector4& value)
		{
			double num = value.Length();
			if (num == 0)
			{
				return FbxVector4(0,0,0);
			}
			return value / num;
		}

		FbxString FbxHelper::GetNodeNameAndAttributeTypeName(const FbxNode* pNode)
		{
			FbxString s = pNode->GetName();

			FbxNodeAttribute::EType lAttributeType;

			if(pNode->GetNodeAttribute() != NULL)
			{
				lAttributeType = (pNode->GetNodeAttribute()->GetAttributeType());

				switch (lAttributeType)
				{
				case FbxNodeAttribute::eMarker:					s += " (Marker)";				break;
				case FbxNodeAttribute::eSkeleton:				s += " (Skeleton)";				break;
				case FbxNodeAttribute::eMesh:					s += " (Mesh)";                 break;
				case FbxNodeAttribute::eCamera:					s += " (Camera)";               break;
				case FbxNodeAttribute::eLight:					s += " (Light)";                break;
				case FbxNodeAttribute::eBoundary:				s += " (Boundary)";             break;
				case FbxNodeAttribute::eOpticalMarker:			s += " (Optical marker)";       break;
				case FbxNodeAttribute::eOpticalReference:		s += " (Optical reference)";    break;
				case FbxNodeAttribute::eCameraSwitcher:			s += " (Camera switcher)";      break;
				case FbxNodeAttribute::eNull:					s += " (Null)";                 break;
				case FbxNodeAttribute::ePatch:					s += " (Patch)";                break;
				case FbxNodeAttribute::eNurbs:					s += " (NURB)";                 break;
				case FbxNodeAttribute::eNurbsSurface:			s += " (Nurbs surface)";        break;
				case FbxNodeAttribute::eNurbsCurve:				s += " (NURBS curve)";          break;
				case FbxNodeAttribute::eTrimNurbsSurface:		s += " (Trim nurbs surface)";   break;
				case FbxNodeAttribute::eUnknown:				s += " (Unidentified)";         break;
				}   
			}

			return s;
		}

		String^ FbxHelper::ConvertToString(FbxString& fbxStr)
		{				
			return gcnew String(fbxStr.Buffer());
		}		

		void FbxHelper::DestroySdkObjects(FbxManager *pSdkManager, FbxScene  *pScene)
		{
			//Delete the FBX Manager. All the objects that have been allocated using the FBX Manager and that haven't been explicitly destroyed are also automatically destroyed.
			if( pSdkManager) pSdkManager->Destroy();
		}

		// to read a file using an FBXSDK reader
		bool FbxHelper::LoadScene( FbxManager *pSdkManager, FbxScene  *pScene,  const char *pFbxFilePath )
		{
			bool lStatus;

			// Create an importer.
			FbxImporter* lImporter = FbxImporter::Create(pSdkManager,"");

			// Initialize the importer by providing a filename.
			bool lImportStatus = lImporter->Initialize(pFbxFilePath, -1, pSdkManager->GetIOSettings() );

			if( !lImportStatus )
			{
				// Destroy the importer
				lImporter->Destroy();
				return false;
			}

			if (lImporter->IsFBX())
			{
				// Set the import states. By default, the import states are always set to 
				// true. The code below shows how to change these states.
				IOS_REF.SetBoolProp(IMP_FBX_MATERIAL,        true);
				IOS_REF.SetBoolProp(IMP_FBX_TEXTURE,         true);
				IOS_REF.SetBoolProp(IMP_FBX_LINK,            true);
				IOS_REF.SetBoolProp(IMP_FBX_SHAPE,           true);
				IOS_REF.SetBoolProp(IMP_FBX_GOBO,            true);
				IOS_REF.SetBoolProp(IMP_FBX_ANIMATION,       true);
				IOS_REF.SetBoolProp(IMP_FBX_GLOBAL_SETTINGS, true);
			}

			// Import the scene
			lStatus = lImporter->Import(pScene);

			// Destroy the importer
			lImporter->Destroy();

			return lStatus;
		}

		bool FbxHelper::SaveScene(FbxManager* pSdkManager, FbxScene* pScene, const char* pFilename, SaveSceneOptions options)
		{									
			bool lStatus = true;
			int pFileFormat = -1;

			//ConvertScene(pScene, options.Axis,options.Units);

			// Create an exporter.
			FbxExporter* lExporter = FbxExporter::Create(pSdkManager, "");

			if(options.FileFormat == FbxFileFormat::Ascii)
			{
				// Write in fall back format in less no ASCII format found
				pFileFormat = pSdkManager->GetIOPluginRegistry()->GetNativeWriterFormat();

				//Try to export in ASCII if possible
				int lFormatIndex, lFormatCount = pSdkManager->GetIOPluginRegistry()->GetWriterFormatCount();

				for (lFormatIndex=0; lFormatIndex<lFormatCount; lFormatIndex++)
				{
					if (pSdkManager->GetIOPluginRegistry()->WriterIsFBX(lFormatIndex))
					{
						FbxString lDesc =pSdkManager->GetIOPluginRegistry()->GetWriterFormatDescription(lFormatIndex);
						char *lASCII = "ascii";
						if (lDesc.Find(lASCII)>=0)
						{
							pFileFormat = lFormatIndex;
							break;
						}
					}
				} 
			}

			// Set the export states. By default, the export states are always set to 
			// true except for the option eEXPORT_TEXTURE_AS_EMBEDDED. The code below 
			// shows how to change these states.
			IOS_REF.SetBoolProp(EXP_FBX_MATERIAL,        true);
			IOS_REF.SetBoolProp(EXP_FBX_TEXTURE,         true);
			IOS_REF.SetBoolProp(EXP_FBX_EMBEDDED,        false);
			IOS_REF.SetBoolProp(EXP_FBX_SHAPE,           true);
			IOS_REF.SetBoolProp(EXP_FBX_GOBO,            true);
			IOS_REF.SetBoolProp(EXP_FBX_ANIMATION,       true);
			IOS_REF.SetBoolProp(EXP_FBX_GLOBAL_SETTINGS, true);
			
			IOS_REF.SetBoolProp(EXP_SMOOTHING_GROUPS, options.SmoothingGroups);			
			IOS_REF.SetBoolProp(EXP_TANGENTSPACE, options.TangentsAndBinormals);			
			IOS_REF.SetBoolProp(EXP_SMOOTHMESH, options.SmoothMesh);			



			//if(options.Axis == FbxAxis::ZUp)
				//IOS_REF.SetEnumProp(EXP_AXISCONVERSIONMETHOD, options.Units);			

			// Initialize the exporter by providing a filename.
			if(lExporter->Initialize(pFilename, pFileFormat, pSdkManager->GetIOSettings()) == false)
			{				
				System::Diagnostics::Debugger::Log(0,"Warning","Call to FbxExporter::Initialize() failed.");
				FBXSDK_printf("Error returned: %s\n\n", lExporter->GetLastErrorString());
				return false;
			}

			int lMajor, lMinor, lRevision;
			FbxHelper::GetVersion(options.Version , &lMajor, &lMinor, &lRevision);

			FbxManager::GetFileFormatVersion(lMajor, lMinor, lRevision);			
			FBXSDK_printf("FBX file format version %d.%d.%d\n\n", lMajor, lMinor, lRevision);

			// Export the scene.
			lStatus = lExporter->Export(pScene); 

			// Destroy the exporter.
			lExporter->Destroy();
			return lStatus;
		}

		bool FbxHelper::IsEqual(FbxVector4& v1, FbxVector4& v2, double telorance)
		{
			if (abs(v1[0] - v2[0]) < telorance &&
				abs(v1[1] - v2[1]) < telorance &&
				abs(v1[2] - v2[2]) < telorance)
				return true;
			return false;
		}

		bool FbxHelper::IsEqual(FbxVector2& v1, FbxVector2& v2, double telorance)
		{
			if (abs(v1[0] - v2[0]) < telorance &&
				abs(v1[1] - v2[2]) < telorance )
				return true;
			return false;
		}

		void FbxHelper::TriangulateRecursive(FbxNode* pNode)
		{
			FbxNodeAttribute* lNodeAttribute = pNode->GetNodeAttribute();

			if (lNodeAttribute)
			{
				if (lNodeAttribute->GetAttributeType() == FbxNodeAttribute::eMesh ||
					lNodeAttribute->GetAttributeType() == FbxNodeAttribute::eNurbs ||
					lNodeAttribute->GetAttributeType() == FbxNodeAttribute::eNurbsSurface ||
					lNodeAttribute->GetAttributeType() == FbxNodeAttribute::ePatch)
				{
					FbxGeometryConverter lConverter(pNode->GetFbxManager());
					lConverter.TriangulateInPlace(pNode);
				}
			}

			const int lChildCount = pNode->GetChildCount();
			for (int lChildIndex = 0; lChildIndex < lChildCount; ++lChildIndex)
			{
				TriangulateRecursive(pNode->GetChild(lChildIndex));
			}
		}

		// Get the geometry offset to a node. It is never inherited by the children.
		FbxAMatrix FbxHelper::GetGeometry(FbxNode* pNode)
		{
			const FbxVector4 lT = pNode->GetGeometricTranslation(FbxNode::eSourcePivot);
			const FbxVector4 lR = pNode->GetGeometricRotation(FbxNode::eSourcePivot);
			const FbxVector4 lS = pNode->GetGeometricScaling(FbxNode::eSourcePivot);

			return FbxAMatrix(lT, lR, lS);
		}

		// Get the global position of the node for the current pose.
		// If the specified node is not part of the pose or no pose is specified, get its
		// global position at the current time.
		FbxAMatrix FbxHelper::GetGlobalPosition(FbxNode* pNode, const FbxTime& pTime, FbxPose* pPose, FbxAMatrix* pParentGlobalPosition)
		{
			FbxAMatrix lGlobalPosition;
			bool        lPositionFound = false;

			if (pPose)
			{
				int lNodeIndex = pPose->Find(pNode);

				if (lNodeIndex > -1)
				{
					// The bind pose is always a global matrix.
					// If we have a rest pose, we need to check if it is
					// stored in global or local space.
					if (pPose->IsBindPose() || !pPose->IsLocalMatrix(lNodeIndex))
					{
						lGlobalPosition = GetPoseMatrix(pPose, lNodeIndex);
					}
					else
					{
						// We have a local matrix, we need to convert it to
						// a global space matrix.
						FbxAMatrix lParentGlobalPosition;

						if (pParentGlobalPosition)
						{
							lParentGlobalPosition = *pParentGlobalPosition;
						}
						else
						{
							if (pNode->GetParent())
							{
								lParentGlobalPosition = GetGlobalPosition(pNode->GetParent(), pTime, pPose);
							}
						}

						FbxAMatrix lLocalPosition = GetPoseMatrix(pPose, lNodeIndex);
						lGlobalPosition = lParentGlobalPosition * lLocalPosition;
					}

					lPositionFound = true;
				}
			}

			if (!lPositionFound)
			{
				// There is no pose entry for that node, get the current global position instead.

				// Ideally this would use parent global position and local position to compute the global position.
				// Unfortunately the equation 
				//    lGlobalPosition = pParentGlobalPosition * lLocalPosition
				// does not hold when inheritance type is other than "Parent" (RSrs).
				// To compute the parent rotation and scaling is tricky in the RrSs and Rrs cases.
				lGlobalPosition = pNode->EvaluateGlobalTransform(pTime);
			}

			return lGlobalPosition;
		}

		// Get the matrix of the given pose
		FbxAMatrix FbxHelper::GetPoseMatrix(FbxPose* pPose, int pNodeIndex)
		{
			FbxAMatrix lPoseMatrix;
			FbxMatrix lMatrix = pPose->GetMatrix(pNodeIndex);

			memcpy((double*)lPoseMatrix, (double*)lMatrix, sizeof(lMatrix.mData));

			return lPoseMatrix;
		}


	// Get specific property value and connected texture if any.
    // Value = Property value * Factor property value (if no factor property, multiply by 1).
		FbxDouble3 FbxHelper::GetMaterialProperty(const FbxSurfaceMaterial * pMaterial, const char * pPropertyName, const char * pFactorPropertyName, unsigned int& pTextureName)
		{
			FbxDouble3 lResult(0, 0, 0);
			const FbxProperty lProperty = pMaterial->FindProperty(pPropertyName);
			const FbxProperty lFactorProperty = pMaterial->FindProperty(pFactorPropertyName);
			if (lProperty.IsValid() && lFactorProperty.IsValid())
			{
				lResult = lProperty.Get<FbxDouble3>();
				double lFactor = lFactorProperty.Get<FbxDouble>();
				if (lFactor != 1)
				{
					lResult[0] *= lFactor;
					lResult[1] *= lFactor;
					lResult[2] *= lFactor;
				}
			}

			if (lProperty.IsValid())
			{
				const int lTextureCount = lProperty.GetSrcObjectCount<FbxFileTexture>();
				if (lTextureCount)
				{
					const FbxFileTexture* lTexture = lProperty.GetSrcObject<FbxFileTexture>();
					if (lTexture && lTexture->GetUserDataPtr())
					{
						pTextureName = *(static_cast<unsigned int*>(lTexture->GetUserDataPtr()));
					}
				}
			}

			return lResult;
		}
	}
}