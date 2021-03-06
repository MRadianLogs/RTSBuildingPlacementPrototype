﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildingToolPlacingBehavior : MonoBehaviour
{
    //The key used to place buildings. Probably left mouse button.
    [SerializeField] private KeyCode placingKey;

    [SerializeField] private GameObject worldUp;
    [SerializeField] private PlayerBuildingToolSelectionBehavior buildingToolSelector = null;
    [SerializeField] private PlayerResourceManager resourceManager = null;

    private GameObject ghostObject;
    private bool ghostObjectAlreadyCreated;
    private bool placingEnabled;    

    private void Update()
    {
        if(buildingToolSelector.IsBuildingEnabled())
        {
            CreateUpdateGhost();
            
            CheckPlaceButtonPressed();
        }
        else
        {
            CallGhostBuster();
        }
    }

    private void CheckPlaceButtonPressed()
    {
        if (Input.GetKeyDown(placingKey))
        {
            //Check if building is in mouse positions spot/area.
            if (placingEnabled == true)
            {
                if (resourceManager.GetNumResources() >= buildingToolSelector.GetCurrentSelectedBuilding().GetComponentInChildren<BuildingBehavior>().GetBuildingCreationCost())
                {
                    //Spawn building.
                    resourceManager.BuildBuilding(buildingToolSelector.GetCurrentSelectedBuilding().GetComponentInChildren<BuildingBehavior>().GetBuildingCreationCost());
                    Instantiate(buildingToolSelector.GetCurrentSelectedBuilding(), ghostObject.transform.position, ghostObject.transform.rotation);
                    placingEnabled = false;
                    CallGhostBuster();
                    buildingToolSelector.PlaceBuilding();
                }
            }
        }
    }

    private void CreateUpdateGhost()
    {
        Ray rayPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(rayPoint, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            //Create ghost version of building to place to use to check if theres another building in placing location.
            if (ghostObjectAlreadyCreated == false)
            {
                ghostObject = Instantiate(buildingToolSelector.GetCurrentSelectedGhostBuilding(), hit.point, worldUp.transform.rotation);
                ghostObjectAlreadyCreated = true;
            }

            ghostObject.transform.position = hit.point;

            //TODO This isnt working. Can still place within other buildings.
            if (ghostObject.GetComponentInChildren<BuildingGhostBehavior>().IsCollidingWBuilding() == true)
            {
                if (placingEnabled == true)
                {
                    Debug.Log("Cannot place here!");
                    placingEnabled = false;
                }
            }
            else
            {
                if (placingEnabled == false)
                {
                    Debug.Log("Can place here!");
                    placingEnabled = true;
                }
            }
        }
    }

    //Cleans up unwanted ghost object.
    private void CallGhostBuster()
    {
        if (ghostObject != null)
        {
            Destroy(ghostObject);
            ghostObject = null;
            ghostObjectAlreadyCreated = false;
        }
    }
}
