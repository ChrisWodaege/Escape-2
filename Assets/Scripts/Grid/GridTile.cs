[System.Serializable]
public class GridTile
{
    public int x;
    public int y;
    public int tileID;	//Welche ID sagt was?
    public GridDirection tileRotation;
    public int itemID; //-1 is no item
    public GridDirection itemRotation;
}