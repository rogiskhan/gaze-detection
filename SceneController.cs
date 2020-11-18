using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using OpenCvSharp;
using System.Collections.Specialized;
using JetBrains.Annotations;
using System;
unsafe
public class SceneController : MonoBehaviour
   
{
    public List<DetectedFace> processedFaces;
  
    public RawImage camTexture;
    public int* pr;
    public int* pl;
    private Point eye_centre;
    public RawImage ModifiedImage;
    private Color[] pixels;
    public Mat croppedImage;
    private Texture2D EmptyImage;
    private int supy, infy;
    public int il, ir, sy, iy, sx, dx;
    public Stopwatch timer;
    public int occhio;
    public bool pushed,right;
    public Texture2D ToBeProcessed;
    public bool face;

    public bool rflag, lflag;
    // Start is called before the first frame update
    void Start()
    {
    
        processedFaces = new List<DetectedFace>();
        
        //occhio è la variabile che tiene l'indice dell'occhio selezionato (dx o sx)
        occhio = 0;
        //faccio partire il timer che triggera la selezione di uno dei due occhi
        timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        //trigger del pulsante
        //pushed serve per il pulsante right serve per decidere se destra o sinistra
        pushed = false;
        right = false;
        //variabile di check se ha trovato gli occhi 
        face = false;

    }

    
    // Update is called once per frame
    void Update()
    {

        //Controllo se sono passati 3,3 minuti
        if (timer.ElapsedMilliseconds>200000)
        {
            print("timer a 0");
            //resetto il timer
            timer.Restart();
         
        }
  
        //dimensione schermo
        int sh = Screen.height;
        int sw = Screen.width;
        // dimensione texture restituita da webcam
        int th = (int)camTexture.rectTransform.rect.height;
        int tw = (int)camTexture.rectTransform.rect.width;
        //puntatori che servono per tenere traccia dell'indice dell'occhio selezionato
        int ir = 0;
        int il = 0;
        pr = &ir;
        pl = &il;
        
        //Setto di default il centro a zero qualora non trovi una faccia
        eye_centre.X = 0;
        eye_centre.Y = 0;
        
        
        
        Texture2D rawwebcam = camTexture.texture as Texture2D;
        // dichiaro variabile di supporto per croppare webcam
        int ext_right = 0, ext_left = tw, ext_sup = 0, ext_inf = th;
        //booleani per avere trovato i due occhi
        bool rflag = false;
        bool lflag = false;

        foreach (DetectedFace f in processedFaces)
        unsafe
        {
            //variabili per scegliere l'occhio: le due aree in formato double, il flag che indica se è stato trovato l'occhio e i due interi che indicano l'indice
            double Aleft = 0;
            double Aright = 0;
            
           
            for (int i = 0; i < f.Elements.Length; i++)
            {

                if (f.Elements[i].Name.CompareTo("RightEye") == 0) // controllo se è l'occhio destro
                {
                    if (f.Elements[i].Marks != null)
                    {
                        Aright = Cv2.ContourArea(f.Elements[i].Marks);
                        ir = i;

                    }

                    rflag = true;
                }

                else if (f.Elements[i].Name.CompareTo("LeftEye") == 0) // o se è il sinistro
                {
                    if (f.Elements[i].Marks != null)
                    {
                        Aleft = Cv2.ContourArea(f.Elements[i].Marks);
                         il = i;

                    }

                    lflag = true;
                }

                if (lflag == true && rflag == true)
                {
                    //ho trovato entrambi gli occhi il resto non mi importa più
                    break;
                }
            }


            //controllo se è passato un secondo 
            //è stato scelto un secondo perché se si tiene anche un buon pc con il risparmio energetico potrebbe non fare in tempo ad arrivare fin qua
                if (timer.ElapsedMilliseconds < 1000)
                {
                  
             
                    if (Aright > Aleft)
                    {
                        //assegno il puntatore all'occhio così so sempre l'indice giusto
                        //senza puntatore una volta scelto l'occhio ho sempre quell'indice ma non si può essere sicuri che prenda sempre
                        //con lo stesso ordine gli elementi della faccia (abbiamo fatto qualche prova e qualche cosa l'abbiamo salvata così)
                        occhio = *pr;
                       
                    }
                    else 
                    {
                        occhio = *pl;
                    
                    }
                }
                //controllo se il pulsante è stato premuto.
                //Ho diviso perché se non facessi così non saprei distinguere il motivo per cui sono nell'if
                //non sapere il motivo mi obbliga a mettere la condizione di destra con un or ma siccome le aree si aggiornano sempre se premo il pulsante per andare a sinistra e Aright>Aleft me ne vado a dx comunque
                //l'alternativa sarebbe non calcolare l'area sempre così che posso modificare le aree quando premo il pulsante ma il calcolo dell'area è una riga di codice all'interno di una sezione che deve essere eseguita ad ogni frame per cui meglio mettere due if diversi anche se simili

                if (pushed == true)
                {
                    pushed = false;

                    if (right == true)
                    {
                        
                        occhio = *pr;
                        right = false;
                    }
                    else
                    {
                        occhio = *pl;

                    }
                }



                //vengono presi i marker estremi dell'occhio selezionato
                if ((f.Elements[occhio].Name.CompareTo("RightEye") == 0 && f.Elements[occhio].Marks != null) || (f.Elements[occhio].Name.CompareTo("LeftEye") == 0 && f.Elements[occhio].Marks != null))
            {
                
                for (int j = 0; j < f.Elements[occhio].Marks.Length; j++)
                {
                    if (f.Elements[occhio].Marks[j].X > ext_right)
                    {
                        ext_right = f.Elements[occhio].Marks[j].X;
                    }
                    if (f.Elements[occhio].Marks[j].X < ext_left)
                    {
                        ext_left = f.Elements[occhio].Marks[j].X;
                    }
                    if (f.Elements[occhio].Marks[j].Y > ext_sup)
                    {
                        ext_sup = f.Elements[occhio].Marks[j].Y;
                    }
                    if (f.Elements[occhio].Marks[j].Y < ext_inf)
                    {
                        ext_inf = f.Elements[occhio].Marks[j].Y;
                    }


                }
                   
                    int crop_x = ext_left - 10;
                    int crop_y = th - ext_sup -5;
                    //distanza marker e crop
                    sx = ext_left - crop_x;
                    dx = ext_right - crop_x;
                    //spiegone: quando fa il crop inspiegabilmente cambia sistema di riferimetnto e quindi pone lo zero delle y in fondo all'immagine anziché in cima
                    //non solo: per il crop vuole il valore più in basso proprio perché il suo sistema di riferimento è in basso
                    //quindi la formula per convertire i valori per fare il crop è th-y dove y è un valore qualsiasi delle y del valore precedente
                    // dopo aver fatto questa trasformazione sarà necessario convertire tutto nelle coordinate crop alla stessa maniera delle x. 
                    //Ho scelto di mantenere la continuità dei nomi quindi se parleremo di sup sarà il punto più in basso e di inf come del punto più in alto.
                    supy = (th - ext_sup) - crop_y;
                    infy = (th - ext_inf) - crop_y;
                    print("ext_inf" + ext_inf + "ext_sup" + ext_sup + "infy" + infy);
                    //prendo i pixel e li metto su  una immagine vuota
                    pixels = rawwebcam.GetPixels(crop_x, crop_y, ext_right + 20 - ext_left, ext_sup + 10 - ext_inf);
                   
                    EmptyImage = new Texture2D(ext_right + 20 - ext_left, ext_sup + 10 - ext_inf);
                    EmptyImage.SetPixels(pixels);
                    EmptyImage.Apply();
                    
                    if (lflag == true || rflag == true)
                    {
                        eye_centre.X = ((dx - sx) / 2) + sx;
                        eye_centre.Y = (((EmptyImage.height -supy)-(EmptyImage.height-infy)) / 2) +EmptyImage.height-infy;
                        face = true;
                        

                    }
                    //riporto in coordinate "normali" con origine in alto a sinistra
                    sy = EmptyImage.height - supy;
                    iy = EmptyImage.height - infy;
                    print("crop_x: " + crop_x + " crop_y: " + crop_y + " sx : " + sx + " dx : " + dx + " supy : " + sy + " infy: " + iy + " eyecentrex: " + eye_centre.X + " eyecentrey " + eye_centre.Y);

                }

        }//ci protegge su coordinate da visi non riconosciuti
        if (lflag == false & rflag == false)
        {
            face = false;
        }
         
        //assegno alla variabile da far passare
        ToBeProcessed = EmptyImage;

        //il try serve quando sono nella scena di configurazione per far vedere il rettangolo con l'occhio e il suo centro
        try
        {

            Mat img = OpenCvSharp.Unity.TextureToMat(EmptyImage);

     
            Point TopLeft = new Point(sx, infy);
            Point BottomRight = new Point(dx, supy);
          
            Cv2.Rectangle(img, new Point(sx, img.Height-infy), new Point(dx, img.Height-supy), OpenCvSharp.Scalar.Crimson, 1);
         
            Cv2.Line(img, new Point(eye_centre.X, 0), new Point(eye_centre.X, EmptyImage.height), OpenCvSharp.Scalar.Green, 1);
            Cv2.Line(img, new Point(0, eye_centre.Y), new Point(EmptyImage.width, eye_centre.Y), OpenCvSharp.Scalar.Green, 1);


            ModifiedImage.texture = OpenCvSharp.Unity.MatToTexture(img);
        }

        catch
        {
            
        }
    }
    
}
