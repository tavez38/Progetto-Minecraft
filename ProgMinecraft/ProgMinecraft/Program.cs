using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace ProgMinecraft
{
    internal class Program
    {
        const int RAW_INVENTARY = 3;
        const int COLL_INVENTARY = 9;
        const int MAT_AVVIO = 4;
        const int MAX_MAT = 64;
        static String[] nomeMateriale = new String[COLL_INVENTARY];
        static int[] quantitaMateriale = new int[COLL_INVENTARY];
        static String[,] nomeMaterialeInventario = new String[RAW_INVENTARY, COLL_INVENTARY];
        static int[,] quantitaMatInv = new int[RAW_INVENTARY, COLL_INVENTARY];
        static String[] possibiliMaterialiAvvio = { "TRONCO_LEGNO", "COBBLESTONE", "CARBONE", "CARNE_CRUDA", "PATATE", "CAROTE" };
        static bool[] statusMatGen = { false, false, false, false, false, false };
        static int indiceRawRicerca;
        static int indiceCollRicerca;
        static void Main(string[] args)
        {
            int scelta;
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
            do
            {
                stampaMenu();
                Console.Write("inserisci la tua scelta: ");
            }while (int.TryParse(Console.ReadLine(),out scelta)&& scelta>0 && scelta<3 );
        }
        static void stampaMenu()
        {
            Console.WriteLine("--- PROVA LA LOGICA DI MINECRAFT ---");
            Console.WriteLine("SELEZIONA L'OPERAZIONE CHE VUOI FARE");
            Console.WriteLine("1. Visualizza il tuo inventario");
            Console.WriteLine("2. Aggiungi un item al tuo inventario");
            Console.WriteLine("Qualsiasi tasto: Esci");
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
            for (int i = 0; i < nomeMateriale.Length; i++)
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
        static bool cercaItem(String nomeItem)
        {
            for (int i = 0; i < RAW_INVENTARY; i++)
            {
                for(int j = 0;j < COLL_INVENTARY; j++)
                {
                    if (nomeMaterialeInventario[i, j] == nomeItem)
                    {
                        indiceRawRicerca = i;
                        indiceCollRicerca = j;
                        return true;
                    }
                }
            }
            return false;
        }
        static void addItem(String nomeItem, int quantItem)
        {
            if (!cercaItem(nomeItem))
            {
                addItemFirstFreeSlot(nomeItem, quantItem);
            }
            else if (quantitaMatInv[indiceRawRicerca, indiceCollRicerca]==MAX_MAT) 
            {
                addItemFirstFreeSlot(nomeItem, quantItem);
            }
            else
            {
                int somma = quantItem + quantitaMatInv[indiceRawRicerca, indiceCollRicerca];
                if(somma > MAX_MAT && somma%64==0)
                {
                    int numEsecuzioni = somma/64;
                    quantitaMatInv[indiceRawRicerca, indiceCollRicerca] = 0;
                    nomeMaterialeInventario[indiceRawRicerca, indiceCollRicerca] = null;
                    for (int i = 0;i < numEsecuzioni; i++)
                    {
                        addItemFirstFreeSlot(nomeItem, 64);
                    }
                }
                //todo
            }
        }
        static bool addItemFirstFreeSlot(String nomeItem, int quantItem)
        {
            bool addedItem = false;
            for (int i = 0; i < RAW_INVENTARY && !addedItem; i++)
            {
                for (int j = 0; j < COLL_INVENTARY; j++)
                {
                    if (nomeMaterialeInventario[i, j] == null)
                    {
                        nomeMaterialeInventario[i, j] = nomeItem;
                        quantitaMatInv[i, j] = quantItem;
                        addedItem = true;
                        break;
                    }
                }
            }
            return addedItem;
        }
        /*static void sortMatrix()
        {
            int scambi;
            do
            {
                scambi = 0;
                for (int i = 0; i < RAW_INVENTARY; i++)
                {
                    for (int j = 0; j < COLL_INVENTARY; j++)
                    {
                        switch (j)
                        {
                            case 9:
                                if (quantitaMatInv[i + 1, 0] > quantitaMatInv[i, j])
                                {
                                    int copia = quantitaMatInv[i, j];
                                    quantitaMatInv[i, j] = quantitaMatInv[i + 1, 0];
                                    quantitaMatInv[i + 1, 0] = copia;
                                    String copiaName = nomeMaterialeInventario[i, j];
                                    nomeMaterialeInventario[i, j] = nomeMaterialeInventario[i + 1, 0];
                                    nomeMaterialeInventario[i + 1, 0] = copiaName;
                                    scambi++;
                                }
                                break;
                            default:
                                if (quantitaMatInv[i, j + 1] > quantitaMatInv[i, j])
                                {
                                    int copia = quantitaMatInv[i, j];
                                    quantitaMatInv[i, j] = quantitaMatInv[i, j + 1];
                                    quantitaMatInv[i, j + 1] = copia;
                                    String copiaName = nomeMaterialeInventario[i, j];
                                    nomeMaterialeInventario[i, j] = nomeMaterialeInventario[i, j + 1];
                                    nomeMaterialeInventario[i, j + 1] = copiaName;
                                    scambi++;
                                }
                                break;
                        }
                    }
                }
            }while (scambi!=0);
        }*/
    } 
}

