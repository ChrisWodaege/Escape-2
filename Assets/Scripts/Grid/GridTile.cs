[System.Serializable]
public class GridTile
{
    public int x;
    public int y;
	public bool containsStone = false;
    public int tileID;	//Welche ID sagt was? 3-Wasser 0- Land 5 ist ein Berg 4-Steinuntergrund
	//5-Baum untergrund
    public GridDirection tileRotation;
    public int itemID; //-1 is no item 
    public GridDirection itemRotation;
}