using System.ComponentModel;
using System.Globalization;
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
        const int NEED_ASSI = 2;
        const int NEED_STICK = 1;
        const int NEED_CARBONE = 1;
        static String[] nomeMateriale = new String[COLL_INVENTARY];
        static int[] quantitaMateriale = new int[COLL_INVENTARY];
        static String[,] nomeMaterialeInventario = new String[RAW_INVENTARY, COLL_INVENTARY];
        static int[,] quantitaMatInv = new int[RAW_INVENTARY, COLL_INVENTARY];
        static String[] possibiliMaterialiAvvio = { "TRONCO_LEGNO", "COBBLESTONE", "CARBONE", "CARNE_CRUDA", "PATATE", "CAROTE" };
        static bool[] statusMatGen = { false, false, false, false, false, false };
        static int [] indiciRicerca= new int [54];

        
        static void Main(string[] args)
        {
            int scelta;
            String nomeItem;
            
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
            Console.WriteLine("--- PROVA LA LOGICA DI MINECRAFT ---");
            do
            {
                eliminaItem();
                stampaMenu();
                Console.Write("inserisci la tua scelta: "); 
                if(!int.TryParse(Console.ReadLine(),out scelta))
                {
                    break;
                }
                switch (scelta)
                {
                    case 1:
                        Console.Clear();
                        sortQuickInventory();
                        sortMatrix();
                        Console.WriteLine("Inventario veloce \n"); 
                        stampaQuickInventory();
                        Console.WriteLine("\nInventario");
                        stampaInventory();
                        break;
                    case 2:
                        Console.Clear();
                        if (isFull())
                        {
                            Console.WriteLine("impossibile aggiungere un item, inventario pieno; se hai spazio nel quick inventory sposta gli item li e poi aggiungi nuovamente");
                        }
                        else
                        {
                            int quantItem;
                            Console.Write("Inserire il nome dell'item (se esso è composto da piu parole separarle con _, non inserire articoli e/o preposizioni e inserire il nome al singolare: ex. asse e non assi): ");
                            nomeItem = Console.ReadLine();
                            Console.Write("Inserire la quantita dell'item: ");
                            while (!int.TryParse(Console.ReadLine(), out quantItem))
                            {
                                Console.WriteLine("Errore, inserire una quantita numerica");
                            }
                            resetVetInd();
                            bool res = addItem(nomeItem.ToUpper(), quantItem);
                            if (res)
                            {
                                Console.WriteLine("Inventario aggiornato");
                            }
                            else
                            {
                                Console.WriteLine("Errore nell'aggiunta");
                            }
                        }
                        break;
                    case 3:
                        int somma = 0;
                        Console.Clear();
                        Console.Write("Inserire il nome dell'item: ");
                        nomeItem = Console.ReadLine();
                        resetVetInd();
                        int indiceQuickInv=searchQuickInv(nomeItem.ToUpper());
                        cercaItemInventory(nomeItem.ToUpper(),0);
                        if (indiceQuickInv != -1)
                        {
                            somma += quantitaMateriale[indiceQuickInv];
                        }
                        if (indiciRicerca[0] != -1)
                        {
                            somma = calcSommaInv();
                        }
                        if (somma != 0)
                        {
                            Console.WriteLine($"Al momento hai {somma} unita di {nomeItem}");
                        }
                        else
                        {
                            Console.WriteLine($"Al momento non hai unita di {nomeItem}");
                        }
                        break;
                    case 4:
                        Console.Clear();
                        bool addInv=false;
                        int sceltaCraft;
                        String confCraft;
                        Console.WriteLine("Seleziona il craft che vuoi fare");
                        Console.WriteLine("1. Stick di legno");
                        Console.WriteLine("2. Torcia");
                        Console.WriteLine("Qualsiasi tasto per annullare");
                        Console.WriteLine("Scegli il craft");
                        if(!int.TryParse(Console.ReadLine(), out sceltaCraft) || sceltaCraft>=3 || sceltaCraft <= 0)
                        {
                            break;
                        }
                        switch (sceltaCraft)
                        {
                            
                            case 1:
                                Console.Clear();
                                resetVetInd();
                                Console.WriteLine("Materiale occorente: 2x ASSI_LEGNO");
                                cercaItemInventory("ASSE_LEGNO", 0);
                                int indexAssi=searchQuickInv("ASSE_LEGNO");
                                if (indiciRicerca[0] == -1 && indexAssi==-1)
                                {
                                    Console.WriteLine("Al momento non hai i materiali necessari");
                                }
                                else
                                {
                                    int sommaAssi = 0;
                                    if (indexAssi != -1)
                                    {
                                        sommaAssi += quantitaMateriale[indexAssi];
                                    }
                                     sommaAssi+=calcSommaInv();
                                    Console.WriteLine($"Al momento possiedi {sommaAssi} ASSE_LEGNO");
                                    if(sommaAssi>= NEED_ASSI)
                                    {
                                        Console.WriteLine("Quanti stick vuoi craftare?");
                                        if(!int.TryParse(Console.ReadLine(),out int quant))
                                        {
                                            break;
                                        }
                                        if (sommaAssi < (NEED_ASSI * quant))
                                        {
                                            Console.WriteLine($"impossibile craftare {quant} stick; materiali insufficienti");
                                            break;
                                        }
                                        Console.WriteLine("Vuoi procedere con il craft? (S/qualsiasi tasto per annullare)");
                                        confCraft=Console.ReadLine();
                                        if (confCraft.ToUpper() == "S")
                                        {
                                            if (indexAssi != -1)
                                            {
                                                svuotaQuickInv(indexAssi);
                                            }
                                            addInv =craftStick(sommaAssi, quant);
                                            if (addInv)
                                            {
                                                Console.WriteLine("craft eseguito; controlla l'inventario");
                                            }
                                            else
                                            {
                                                Console.WriteLine("craft eseguito; item non aggiunto all'inventario");
                                            }
                                        }
                                        else 
                                        {
                                            Console.WriteLine("Uscita in corso...");
                                        }
                                        
                                    }
                                    else
                                    {
                                        Console.WriteLine("Impossibile procedere con il craf, non possiedi abbastanza materiali");
                                    }
                                    
                                }
                                break;
                            case 2:
                                Console.Clear();
                                int sommaStick = 0;
                                int sommaCarbone = 0;
                                resetVetInd();
                                Console.WriteLine("Materiale occorrente: 1x CARBONE, 1x STICK_LEGNO");
                                cercaItemInventory("STICK_LEGNO", 0);
                                int indexStick = searchQuickInv("STICK_LEGNO");
                                if (indiciRicerca[0] == -1 && indexStick == -1)
                                {
                                    Console.WriteLine("Al momento non hai degli STICK_LEGNO");
                                    Console.WriteLine("Impossibile eseguire il craft");
                                }
                                else
                                {  
                                    if (indexStick != -1)
                                    {
                                        sommaStick += quantitaMateriale[indexStick];    
                                    }
                                    sommaStick += calcSommaInv();
                                    resetVetInd();
                                    cercaItemInventory("CARBONE", 0);
                                    int indexCarbone = searchQuickInv("CARBONE");
                                    if (indiciRicerca[0] == -1 && indexCarbone == -1)
                                    {
                                        Console.WriteLine("Al momento non hai il CARBONE");
                                        Console.WriteLine("Impossibile eseguire il craft");
                                        break;
                                    }
                                    else 
                                    {
                                        if (indexCarbone != -1)
                                        {
                                            sommaCarbone += quantitaMateriale[indexCarbone];
                                        }
                                        sommaCarbone += calcSommaInv();
                                    }
                                    Console.WriteLine($"Al momento possiedi: \n{sommaStick} unita di STICK_LEGNO\n{sommaCarbone} unita di CARBONE");
                                    if(sommaCarbone>NEED_CARBONE && sommaStick > NEED_STICK)
                                    {
                                        Console.WriteLine("Inserire la quantita di torce da craftare: ");
                                        if (!int.TryParse(Console.ReadLine(), out int quantTorce))
                                        {
                                            Console.WriteLine("Impossibile procedere, la quantita deve essere numerica");
                                            break;
                                        }
                                        if(sommaStick < (NEED_STICK*quantTorce) && sommaCarbone < (NEED_CARBONE * quantTorce))
                                        {
                                            Console.WriteLine($"impossibile craftare {quantTorce} torce; materiali insufficienti");
                                            break;
                                        }
                                        Console.WriteLine("Vuoi procedere con il craft? (S/qualsiasi tasto per annullare)");
                                        confCraft = Console.ReadLine();
                                        if (confCraft.ToUpper() == "S")
                                        {
                                            if (indexStick != -1)
                                            {
                                                svuotaQuickInv(indexStick);
                                            }
                                            if (indexCarbone != -1)
                                            {
                                                svuotaQuickInv(indexCarbone);
                                            }
                                            svuotaInvRicerca(); //svuotamento celle contenenti il carbone x redistribuzione
                                            resetVetInd();//pulizia vettore ricerca indici
                                            cercaItemInventory("STICK_LEGNO", 0);//caricamento vettore indici con indici degli stick
                                            svuotaInvRicerca(); //svuotamento celle contenenti stick x redistribuzione
                                            addInv = craftTorcia(sommaStick, sommaCarbone, quantTorce);
                                            if (addInv)
                                            {
                                                Console.WriteLine("craft eseguito; controlla l'inventario");
                                            }
                                            else
                                            {
                                                Console.WriteLine("craft eseguito; item non aggiunto all'inventario");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Uscita in corso...");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Impossibile eseguire il craft");
                                    }
                                    
                                }
                                break;
                            default: break;
                        }
                        break;
                    default:
                        break;
                }
            } while (scelta > 0 && scelta < 5);
        }
        static void svuotaQuickInv(int index)
        {
            quantitaMateriale[index] = 0;
            nomeMateriale[index] = null;
        }
        static void svuotaInvRicerca()//funzione eseguita SOLAMENTE quando e necessaria una redistribuzione dei materiali per cui bisogna svuotare le caselle contenenti esso dopo aver calcolato la somma di quanto se ne possiede
        {
            for (int i = 0; i < indiciRicerca.Length; i += 2)
            {
                if (indiciRicerca[i] != -1)
                {
                    int ind1 = indiciRicerca[i];
                    int ind2 = indiciRicerca[i + 1];
                    quantitaMatInv[ind1, ind2] = 0;
                    nomeMaterialeInventario[ind1, ind2] = null;
                }
            }
        }
        static bool craftTorcia(int sommaStick, int sommaCoal, int quant)
        {
            int stickRimanenti = sommaStick - (quant * NEED_STICK);
            int coalRimanente = sommaCoal - (quant * NEED_CARBONE);
            if (stickRimanenti > 0)
            {
                addItemNotStack(stickRimanenti, "STICK_LEGNO");
            }
            if (coalRimanente > 0)
            {
                addItemNotStack(coalRimanente, "CARBONE");
            }
            resetVetInd();
            return addItem("TORCIA", quant);

        }
        static bool craftStick(int sommaMax, int quant)
        {
            int maxRimanenti = sommaMax - (quant*NEED_ASSI);
            svuotaInvRicerca();
            if (maxRimanenti > 0) 
            {
                addItemNotStack(maxRimanenti, "ASSI_LEGNO"); 
            }
            resetVetInd();
            return addItem("STICK_LEGNO", quant);
             
        }
        static int calcSommaInv()
        {
            int somma = 0;
            for (int i = 0; i < indiciRicerca.Length; i += 2)
            {
                if (indiciRicerca[i] != -1)
                {
                    int ind1 = indiciRicerca[i];
                    int ind2 = indiciRicerca[i + 1];
                    somma += quantitaMatInv[ind1, ind2];
                }
            }
            return somma;
        }
        static void stampaMenu()
        {
            
            Console.WriteLine("SELEZIONA L'OPERAZIONE CHE VUOI FARE");
            Console.WriteLine("1. Visualizza il tuo inventario");
            Console.WriteLine("2. Aggiungi un item al tuo inventario");
            Console.WriteLine("3. Cerca un item nel tuo inventario");
            Console.WriteLine("4. Crafta un oggetto");
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
                else
                {
                    Console.Write("[] \t");
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
                    else {
                        Console.Write("[] \t");
                    }
                }
                Console.WriteLine("\n");
            }
        }
        static void cercaItemInventory(String nomeItem, int indVetInd)
        {
            bool sameIndexs;
            bool addedIndex=false;
            int i=0, j=0;
            for (; i < RAW_INVENTARY && !addedIndex; i++)
            {
                for(; j < COLL_INVENTARY; j++)
                {
                    sameIndexs = false;
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
            if (addedIndex && !(j == 8 && i==3))
            {
                cercaItemInventory(nomeItem, indVetInd);    
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
            cercaItemInventory(nomeItem, 0);
            if (indiciRicerca[0]==-1)
            {
               added = addItemNotStack(quantItem, nomeItem);
                
            }
            else  
            {  
                int somma = 0;
                somma += quantItem;
                for (int i = 0; i < indiciRicerca.Length; i+=2)
                {
                    if (indiciRicerca[i] != -1) {
                        int ind1=indiciRicerca[i];
                        int ind2=indiciRicerca[i+1];
                        somma = somma+ quantitaMatInv[ind1, ind2];
                        quantitaMatInv[ind1, ind2] = 0;
                        nomeMaterialeInventario[ind1, ind2] = null;
                    }
                }
                added=addItemNotStack(somma, nomeItem);
                                
            }
            return added;
        }
        static bool addItemNotStack(int somma, String nomeItem)
        {
            int numStack;
            bool res=false;
            numStack = (int)(somma / 64);
            for (int i = 0; i < numStack; i++)
            {
                res=addItemFirstFreeSlot(nomeItem, 64);
            }
            int quantRimanente = somma - (numStack * 64);
            if (quantRimanente > 0)
            {
                res=addItemFirstFreeSlot(nomeItem, quantRimanente);
            }
            return res;
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
        static int searchQuickInv(String nomeItem)
        {
            for (int i = 0; i < nomeMateriale.Length; i++)
            {
                if (nomeMateriale[i] == nomeItem)
                {
                    return i;
                }
            }
            return -1;
        }
        static void eliminaItem()
        {
            for (int i = 0; i < RAW_INVENTARY; i++)
            {
                for (int j = 0; j < COLL_INVENTARY; j++)
                {
                    if (quantitaMatInv[i, j] == 0)
                    {
                        nomeMaterialeInventario[i, j] = null;
                    }
                }
            }
        }
        static bool isFull()
        {
            for (int i = 0; i < RAW_INVENTARY; i++)
            {
                for (int j = 0; j < COLL_INVENTARY; j++)
                {
                    if (nomeMaterialeInventario[i, j] == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    } 
}

