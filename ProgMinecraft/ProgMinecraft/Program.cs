using System.ComponentModel;
using System.Net.Http.Headers;
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
        static int [] indiciRicerca= new int [64];
        
        static void Main(string[] args)
        {
            int scelta;
            Random rand = new Random();
            int slotGen = 0;
            while (slotGen < MAT_AVVIO) {
                int matGen = rand.Next(0, 6);
                int quantGen = rand.Next(1, 65);
                bool res = genSlotInventory(matGen, quantGen, slotGen);
                if (res) {
                    slotGen++;
                }
            }
            do
            {
                stampaMenu();
                Console.Write("inserisci la tua scelta: "); 
                if(!int.TryParse(Console.ReadLine(),out scelta))
                {
                    break;
                }
                switch (scelta)
                {
                    case 1:
                        sortQuickInventory();
                        sortMatrix();
                        Console.WriteLine("Inventario veloce \n"); 
                        stampaQuickInventory();
                        Console.WriteLine("\nInventario");
                        stampaInventory();
                        break;
                    case 2:
                        int quantItem;
                        Console.Write("Inserire il nome dell'item: ");
                        String nomeItem=Console.ReadLine();
                        Console.Write("Inserire la quantita dell'item: ");
                        while(!int.TryParse(Console.ReadLine(),out quantItem))
                        {
                            Console.WriteLine("Errore, inserire una quantita numerica");
                        }
                        resetVetInd();
                        bool res=addItem(nomeItem.ToUpper(),quantItem);
                        if (res)
                        {
                            Console.WriteLine("Inventario aggiornato");
                        }
                        else
                        {
                            Console.WriteLine("Errore nell'aggiunta");
                        } 
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }
            } while (scelta > 0 && scelta < 4);
        }
        static void stampaMenu()
        {
            Console.WriteLine("--- PROVA LA LOGICA DI MINECRAFT ---");
            Console.WriteLine("SELEZIONA L'OPERAZIONE CHE VUOI FARE");
            Console.WriteLine("1. Visualizza il tuo inventario");
            Console.WriteLine("2. Aggiungi un item al tuo inventario");
            Console.WriteLine("3. Cerca un item nel tuo inventario");
            Console.WriteLine("Qualsiasi tasto: Esci");
        }
        static bool genSlotInventory(int posMatGen, int quantMatGen, int posInventory)
        {
            if (!statusMatGen[posMatGen])
            {
                nomeMateriale[posInventory] = possibiliMaterialiAvvio[posMatGen];
                quantitaMateriale[posInventory] = quantMatGen;
                statusMatGen[posMatGen]=true;
                return true;
            }
            return false;
        }
        static void stampaQuickInventory()
        {
            for (int i = 0; i < nomeMateriale.Length; i++)
            {
                if (quantitaMateriale[i]!=0)
                {
                    Console.Write($"{nomeMateriale[i]} {quantitaMateriale[i]} \t");
                }
            }
        }
        static void stampaInventory()
        {
            for (int i = 0; i < RAW_INVENTARY; i++)
            {
                for (int j = 0; j < COLL_INVENTARY; j++)
                {
                    if (quantitaMatInv[i, j] != 0)
                    {
                        Console.Write($"{nomeMaterialeInventario[i, j]} {quantitaMatInv[i, j]} \t");
                    }
                }
                Console.WriteLine("\n");
            }
        }
        static void cercaItemInventory(String nomeItem, int inx1, int inx2 , int indVetInd)
        {
            bool sameIndexs=false;
            bool addedIndex=false;
            for (int i=0 ; i < RAW_INVENTARY && !addedIndex; i++)
            {
                for(int j=0 ; j < COLL_INVENTARY; j++)
                {
                    for(int k=0; k<indiciRicerca.Length; k+=2)
                    {
                        if (i == indiciRicerca[k] && j == indiciRicerca[k + 1])
                        {
                            sameIndexs=true;
                            break;
                        }
                    }
                    if (sameIndexs)
                    {
                        continue;
                    }
                    else if (nomeMaterialeInventario[i, j] == nomeItem)
                    {
                        indiciRicerca[indVetInd] = i;
                        indiciRicerca[indVetInd+1] = j;
                        indVetInd +=2;
                        addedIndex=true;
                        break;
                        
                    }
                }
            }
            if (addedIndex && !(inx2 == 8 && inx1==3))
            {
                if(inx2 == 8)
                {
                    cercaItemInventory(nomeItem, inx1, 0, indVetInd);
                }
                else
                {
                    inx1 --;
                    inx2++;
                    cercaItemInventory(nomeItem,inx1,inx2, indVetInd);
                }
            }  
        }
        static void resetVetInd()
        {
            for (int i = 0; i < indiciRicerca.Length; i++)
            {
                indiciRicerca[i] = -1;
            }
        }
        static bool addItem(String nomeItem, int quantItem)
        {
            bool added = false;
            cercaItemInventory(nomeItem, 0, 0, 0);
            if (indiciRicerca[0]==-1)
            {
                addItemFirstFreeSlot(nomeItem, quantItem);
                added = true;
            }
            else  
            {  
                int somma = 0;
                for (int i = 0; i < indiciRicerca.Length; i+=2)
                {
                    if (indiciRicerca[i] != -1) {
                        int ind1=indiciRicerca[i];
                        int ind2=indiciRicerca[i+1];
                        somma = somma+ quantitaMatInv[ind1, ind2]+quantItem;
                        quantitaMatInv[ind1, ind2] = 0;
                        nomeMaterialeInventario[ind1, ind2] = null;
                    }
                }
                if (somma!=0&&somma > MAX_MAT && somma % 64 == 0)
                {
                    int numEsecuzioni = somma / 64;
                    for (int i = 0; i < numEsecuzioni; i++)
                    {
                        addItemFirstFreeSlot(nomeItem, 64);
                    }
                    added = true;
                }
                else if (somma!=0)
                {
                    int numStack = somma / 64;
                    for (int i = 0; i < numStack; i++)
                    {
                        addItemFirstFreeSlot(nomeItem, 64);
                    }
                    int quantRimanente = somma - (numStack * 64);
                    addItemFirstFreeSlot(nomeItem, quantRimanente);
                    added = true;
                }
            }
            return added;
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
        static void sortMatrix()
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
                            case 8:
                                if (i == 2)
                                {
                                    break;
                                }
                                else if (quantitaMatInv[i + 1, 0] > quantitaMatInv[i, j])
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
        }
        static void sortQuickInventory()
        {
            for (int i = 0; i < nomeMateriale.Length - 1; i++) {
                for (int j = 0; j < nomeMateriale.Length-1-i; j++) { 
                    if (quantitaMateriale[j] < quantitaMateriale[j + 1])
                    {
                        int copia = quantitaMateriale[j];
                        quantitaMateriale[j]=quantitaMateriale[j + 1];
                        quantitaMateriale[j+1] = copia;
                        String copia1 = nomeMateriale[j];
                        nomeMateriale[j] = nomeMateriale[j + 1];
                        nomeMateriale[j + 1] = copia1;
                    }
                }
            }
        }
    } 
}

