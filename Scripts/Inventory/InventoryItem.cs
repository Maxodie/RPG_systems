public class InventoryItem 
{
    public int useNumber;
    public ItemData itemData;
    public int stackNumber;

    public static void MakeEqualTo(InventoryItem a, InventoryItem b)
    {
        a.useNumber = b.useNumber;
        a.itemData = b.itemData;
        a.stackNumber = b.stackNumber;
    }
}
