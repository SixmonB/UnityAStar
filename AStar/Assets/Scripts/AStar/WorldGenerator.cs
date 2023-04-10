using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pathing;

public class WorldGenerator : MonoBehaviour, UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerDownHandler
{
    int MapWidth = 8,  MapHeight = 8;
    public float TileXOffset =1.1f;
    public float TileZOffset = 0.85f;
    public GameObject Hexagon_Model;
    public Texture2D[] TextureArray;
    public List<GameObject> myTiles;
    public List<AStarTile> nodes;
    public List<AStarTile> neighbours;
    public Color highlightColor = Color.red;
    private Color baseColor = Color.black;
    public Color startColor = Color.yellow;
    public Color endColor = Color.green;
    private Renderer renderer;
    private Color color;
    private int highlightIndex;
    bool start = false;
    int start_index = 0;
    bool end = false;
    int end_index = 0;
    public AStarTile tile;
    // Start is called before the first frame update
    void Start()
    {
        myTiles = new List<GameObject>();
        nodes = new List<AStarTile>();
        TextureArray = Resources.LoadAll<Texture2D>("Textures");
        GenerateWorld();
    }

    void GenerateWorld()
    {
        int index = 0;
        for(int x = 0; x < MapWidth; x++)
        {
            for(int z = 0; z < MapHeight; z++)
            {
                /*
                myTiles list has instances of the Hexagon_Model gameObject, with an asociated index.
                We assign the texture randomly according to the TextureArray loaded at the beginning of runtime.
                nodes is a list of AStarTile instances, who inherit the interface IAStarNode provided.
                We then initialise the neighbours list as empty to fill it later.
                This implementation is not the most efficient as it's necessary to use two nested for loops later
                to get each node's neighbours, because we have to create every node before creating links between them.
                However, as I worked at the beginning only with the tile grid and not the IA nodes, I already had a specific
                code structure and I didn't have enough time to optimise it. Ideally, the node list would have the IA node 
                information as well as the gameObject tile so as to avoid parallel lists.
                The only link between them is the implementation of index as a sort of ID to keep track of the node/tile.

                Simulation considerations:
                The start tile will be highlighted in yellow when clicked, and the end tile will be green. The shortest path will be blue.
                Water tiles are not recognised as neighbours so if we choose a water tile as a goal node, there will be no available path and tiles won't turn blue.
                I didn't add any game interface to control the flow of the simulation (Play button, Instruction messages, Restart button, Error messages, etc) because of a time
                restriction, so to retest the code it's necessary to stop and relaunch the game.


                ----------------IMPORTANT----------------

                I didn't find a solution but there is a particularity about the first time launching the simulation:
                It's necessary to expand the Material category inside the Inspector tab of the Hexagon_Model prefab to load the material
                information, otherwise the code won't recognise the mouse hovering over the tiles. I looked online and I read that it's necessary for Unity to load the materials the
                first time it runs and to preload this inside the code can have certain complications.
                */
                myTiles.Add(Instantiate(Hexagon_Model, transform));
                Texture2D texture = TextureArray[Random.Range(0, TextureArray.Length)];
                myTiles[index].GetComponent<Renderer>().material.mainTexture = texture;
                neighbours = new List<AStarTile>();
                nodes.Add(new AStarTile(texture.name, index, neighbours));
                Collider collider = myTiles[index].GetComponent<Collider>();
                MeshCollider meshcollider = myTiles[index].GetComponent<MeshCollider>();
                meshcollider.convex = true;
                collider.isTrigger = true;

                if(z%2 == 0)
                {
                    myTiles[index].transform.position = new Vector3(x*TileXOffset, 0, z*TileZOffset);
                }
                else
                {
                    myTiles[index].transform.position = new Vector3(x*TileXOffset + TileXOffset/2, 0, z*TileZOffset);
                }
                index++;
            }
        }

        index = 0;
        /* NEIGHBOURS ASSIGNMENT
        Taking into consideration the tile map generated, we have different amount of neighbours depending on the position in the grid and the z component in certain cases.
        Vertical neighbours are +-1 the current index
        Horizontal neighbours are +-8 the current index
        Diagonal neighbours are +-7/+-9 the current index, depending on the row indicated by z.
        */
        for(int x = 0; x < MapWidth; x++)
        {
            for(int z = 0; z < MapHeight; z++)
            {
                if(x == 0)
                {
                    if(z == 0)
                    {
                        nodes[index].myNeighbours.Add(nodes[index+1]);
                        nodes[index].myNeighbours.Add(nodes[index+8]);
                    }
                    else if(z > 0 && z < MapHeight-1)
                    {
                        if(z%2 == 1)
                        {
                            nodes[index].myNeighbours.Add(nodes[index+1]);
                            nodes[index].myNeighbours.Add(nodes[index+8]);
                            nodes[index].myNeighbours.Add(nodes[index-1]);
                            nodes[index].myNeighbours.Add(nodes[index+9]);
                            nodes[index].myNeighbours.Add(nodes[index+7]);
                        }
                        else
                        {
                            nodes[index].myNeighbours.Add(nodes[index+1]);
                            nodes[index].myNeighbours.Add(nodes[index-1]);
                            nodes[index].myNeighbours.Add(nodes[index+8]);
                        }
                    }
                    else if(z == MapHeight - 1)
                    {
                        nodes[index].myNeighbours.Add(nodes[index-1]);
                        nodes[index].myNeighbours.Add(nodes[index+8]);
                        nodes[index].myNeighbours.Add(nodes[index+7]);
                    }
                }
                else if(x > 0 && x < MapWidth - 1)
                {
                    if(z == 0)
                    {
                        nodes[index].myNeighbours.Add(nodes[index+1]);
                        nodes[index].myNeighbours.Add(nodes[index-8]);
                        nodes[index].myNeighbours.Add(nodes[index+8]);
                        nodes[index].myNeighbours.Add(nodes[index-7]);
                    }
                    else if(z > 0 && z < MapHeight-1)
                    {
                        if(z%2 == 1)
                        {
                            nodes[index].myNeighbours.Add(nodes[index+1]);
                            nodes[index].myNeighbours.Add(nodes[index-1]);
                            nodes[index].myNeighbours.Add(nodes[index+8]);
                            nodes[index].myNeighbours.Add(nodes[index-8]);
                            nodes[index].myNeighbours.Add(nodes[index+7]);
                            nodes[index].myNeighbours.Add(nodes[index+9]);
                        }
                        else
                        {
                            nodes[index].myNeighbours.Add(nodes[index+1]);
                            nodes[index].myNeighbours.Add(nodes[index-1]);
                            nodes[index].myNeighbours.Add(nodes[index+8]);
                            nodes[index].myNeighbours.Add(nodes[index-8]);
                            nodes[index].myNeighbours.Add(nodes[index-7]);
                            nodes[index].myNeighbours.Add(nodes[index-9]);
                        }
                    }
                    else if(z == MapHeight - 1)
                    {
                        nodes[index].myNeighbours.Add(nodes[index+8]);
                        nodes[index].myNeighbours.Add(nodes[index-1]);
                        nodes[index].myNeighbours.Add(nodes[index-8]);
                        nodes[index].myNeighbours.Add(nodes[index+7]);
                    }
                }
                else if(x == MapWidth - 1)
                {
                    if(z == 0)
                    {
                        nodes[index].myNeighbours.Add(nodes[index+1]);
                        nodes[index].myNeighbours.Add(nodes[index-8]);
                        nodes[index].myNeighbours.Add(nodes[index-7]);
                    }
                    else if(z > 0 && z < MapHeight-1)
                    {
                        if(z%2 == 1)
                        {
                            nodes[index].myNeighbours.Add(nodes[index-1]);
                            nodes[index].myNeighbours.Add(nodes[index+1]);
                            nodes[index].myNeighbours.Add(nodes[index-8]);
                        }
                        else
                        {
                            nodes[index].myNeighbours.Add(nodes[index-1]);
                            nodes[index].myNeighbours.Add(nodes[index+1]);
                            nodes[index].myNeighbours.Add(nodes[index-8]);
                            nodes[index].myNeighbours.Add(nodes[index-7]);
                            nodes[index].myNeighbours.Add(nodes[index-9]);
                        }
                    }
                    else if(z == MapHeight - 1)
                    {
                        nodes[index].myNeighbours.Add(nodes[index-1]);
                        nodes[index].myNeighbours.Add(nodes[index-8]);
                    }
                }
                int size = nodes[index].myNeighbours.Count;
                for(int i=0; i< size; i ++)
                {   
                    if(nodes[index].myNeighbours[i].myTexture == "water")
                    {
                        nodes[index].myNeighbours.Remove(nodes[index].myNeighbours[i]);
                        i--;
                        size--;
                        // as we remove the current element, we need to check the new element in that position
                        // and reduce the list size by one to avoid falling out of bounds
                    }
                }
                index++;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Get the index of the tile that was hovered over
        int index = myTiles.IndexOf(eventData.pointerEnter);
        /* Unhighlight the currently highlighted tile
        This proved to be more efficient than implementing a OnPointerExit event as each time I entered a tile, there was a chance the event didn't get recognised
        and therefore having more than one recognised as actual tile highlighted in red. With this method, I unhighlight the previous tile as we find a new
        hovered over tile. Then, we only have one red tile at the time.

        The if condition before assigning the emission color is intended to avoid changing the chosen tiles as start/end/path
        */
        if (highlightIndex < myTiles.Count)
        {
            GameObject tileToUnhighlight = myTiles[highlightIndex];
            renderer = tileToUnhighlight.GetComponent<Renderer>();
            color = renderer.material.GetColor("_EmissionColor");
            if (renderer != null && color == highlightColor)
            {
                renderer.material.SetColor("_EmissionColor", Color.black);
                renderer.material.SetFloat("_EmissionScaleUI", 0.0f);
            }
        }

        // Highlight the new tile
        GameObject tileToHighlight = myTiles[index];
        renderer = tileToHighlight.GetComponent<Renderer>();
        color = renderer.material.GetColor("_EmissionColor");
        if (renderer != null && color == baseColor)
        {
            renderer.material.SetColor("_EmissionColor", highlightColor);
            renderer.material.SetFloat("_EmissionScaleUI", 1.0f);
        }

        // Update the highlight index
        highlightIndex = index;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        int clickedIndex = myTiles.IndexOf(eventData.pointerEnter);
        if(clickedIndex >= 0 && clickedIndex < myTiles.Count)
        {
            GameObject tileClicked = myTiles[clickedIndex];
            renderer = tileClicked.GetComponent<Renderer>();
            if(!start && !end)
            {

                if (renderer != null)
                {
                    renderer.material.SetColor("_EmissionColor", Color.yellow);
                    renderer.material.SetFloat("_EmissionScaleUI", 1f);
                    start_index = clickedIndex;
                    start = true;
                }
            }

            else if (start && !end)
            {
                if (renderer != null)
                {
                    renderer.material.SetColor("_EmissionColor", Color.green);
                    renderer.material.SetFloat("_EmissionScaleUI", 1f);
                    end_index = clickedIndex;
                    IList<IAStarNode> path = AStar.GetPath(nodes[start_index], nodes[end_index]);
                    for(int i = 0; i<path.Count; i++)
                    {
                        tile = (AStarTile)path[i];
                        renderer = myTiles[tile.myID].GetComponent<Renderer>();
                        renderer.material.SetColor("_EmissionColor", Color.blue);
                        renderer.material.SetFloat("_EmissionScaleUI", 1.0f);
                    }
                    
                    end = true;
                }                
            }
                
        }
    }
}
