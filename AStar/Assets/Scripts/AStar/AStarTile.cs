using UnityEngine;
using System.Collections.Generic;
using Pathing;

public class AStarTile : IAStarNode
{
    public string myTexture;
    public int myID;
    public List<AStarTile> myNeighbours;


    public AStarTile(string texture, int id, List<AStarTile> _neighbours)
    {
        myTexture = texture;
        myID = id;
        myNeighbours = _neighbours;
    }
    public IEnumerable<IAStarNode> Neighbours
    {
        // Implement the Neighbours property to return a list of neighbouring IAStarNode objects
        get
        {
            return myNeighbours;
        }
    }

    public float CostTo(IAStarNode neighbour)
    {   
        AStarTile temp_neighbour = (AStarTile) neighbour;
        myTexture = temp_neighbour.myTexture;
        if(myTexture == "grass"){ return 1.0f;}
        else if (myTexture == "desert"){ return 5.0f;}
        else if (myTexture == "mountain"){return 10.0f;}
        else if (myTexture == "forest"){ return 3.0f;}
        else{
            // In case there is an unknown tile texture, I chose 100 arbitrarily 
            return 100.0f;
        }
    }

    public float EstimatedCostTo(IAStarNode goal)
    {
        // I didn't know how I could calculate this, so I assigned an arbitrary value according to a sort of average cost
        return 15.0f;
    }
}

