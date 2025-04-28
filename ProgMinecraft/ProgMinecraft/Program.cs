using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ProgMinecraft
{
    internal class Program
    {
        const int RAW_INVENTARY = 3;
        const int COLL_INVENTARY = 10;
        const int MAT_AVVIO = 4;
        static String[] nomeMateriale = new String[COLL_INVENTARY];
        static int[] quantitaMateriale = new int[COLL_INVENTARY];
        static String[,] nomeMaterialeInventario = new String[RAW_INVENTARY, COLL_INVENTARY];
        static int[,] quantitaMatInv = new int[RAW_INVENTARY, COLL_INVENTARY];
        static String[] possibiliMaterialiAvvio = { "TRONCO_LEGNO", "COBBLESTONE", "CARBONE", "CARNE_CRUDA", "PATATE", "CAROTE" };
        static bool[] statusMatGen = { false, false, false, false, false, false };
        static void Main(string[] args)
        {
            Random rand = new Random();
            int matGen = rand.Next(0, 6);
            int quantGen = rand.Next(1, 65);
            int slotGen = 0;
            while (slotGen < MAT_AVVIO) {
                bool res = genSlotInventory(matGen, quantGen, slotGen);
                if (res) {
                    slotGen++;
                }
            }
        }
        static bool genSlotInventory(int posMatGen, int quantMatGen, int posInventory)
        {
            if (!statusMatGen[posMatGen]) 
            { 
                nomeMateriale[posInventory] = possibiliMaterialiAvvio[posMatGen];
                quantitaMateriale[posInventory] = quantMatGen;
                return true;
            }
            return false;
        }
        static void stampaQuickInventory()
        {
            for(int i = 0; i < nomeMateriale.Length; i++)
            {
                Console.WriteLine($"{nomeMateriale[i]} {quantitaMateriale[i]}");
            }
        }
        static void stampaInventory()
        {
            for (int i = 0; i < RAW_INVENTARY; i++)
            {
                for (int j = 0; j < COLL_INVENTARY; j++)
                {
                    Console.WriteLine($"{nomeMaterialeInventario[i, j]} {quantitaMatInv[i, j]}");
                }
            }
        }
    }
}
