[System.Serializable]
public class GridTile
{
    public int x;
    public int y;
	public bool allowWalking = false;
	public bool containsStone = false;
    public int tileID;	//Welche ID sagt was? 
	//0- Land
	//3 - Wasser 
	//5 - Berg 
	//4-Steinuntergrund 
	//Was ist tileID 10?
	//5-Baum untergrund
    public GridDirection tileRotation;
    public int itemID; //-1 is no item 
    public GridDirection itemRotation;
}