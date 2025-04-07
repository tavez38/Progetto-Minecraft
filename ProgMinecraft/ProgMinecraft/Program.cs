namespace ProgMinecraft
{
    internal class Program
    {
        const int RAW_INVENTARY = 3;
        const int COLL_INVENTARY = 10;
        static String[] nomeMateriale = new String[COLL_INVENTARY];
        static int[] quantitaMateriale = new int[COLL_INVENTARY];
        static String[,] nomeMaterialeInventario = new String[RAW_INVENTARY, COLL_INVENTARY];
        static String[] possibiliMaterialiAvvio = { "TRONCO_LEGNO", "COBBLESTONE", "CARBONE", "CARNE_CRUDA", "PATATE", "CAROTE" };
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
