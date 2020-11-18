using OpenCvSharp;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StreamVideo : MonoBehaviour
{
    public RawImage ProcessedImage;
    public Point coordinate;
    public Boolean isStreaming = false;
    public int th, br;
   
    public Texture process;


    private SceneController pippo;
    private int centrox, centroy;
    // Use this for initialization
    void Start()
    {
        //prendo lo script scene controller
        pippo = GetComponent<SceneController>();
    }
    //Funzione per trovare l'area più grande da una lista di array
    int MaxA(Point[][] cont)
    {
        double Prec = 0;
        int index = 0;
        for (int i = 0; i < cont.Length; i++)
        {
            double A = Cv2.ContourArea(cont[i]);
            if (A >= Prec)
                Prec = A;
            index = i;
        }
        return index;
    }

    void Update()
    {

        //metto un if nel caso in cui non detecta la faccia. In quel caso mi ritrovo con una texture null
        if (pippo.ToBeProcessed != null)
        {

            //prendo l'immagine
            Texture image = pippo.ToBeProcessed;
            
           //converto la texture in texture 2D
          
            RenderTexture videoframe = new RenderTexture(image.width, image.height, 0);
            RenderTexture.active = videoframe;
            
            Graphics.Blit(image, videoframe);
            Texture2D texture = new Texture2D(videoframe.width, videoframe.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new UnityEngine.Rect(0, 0, videoframe.width, videoframe.height), 0, 0, false);

            RenderTexture.active = null;
            //me la porto in Mat
            Mat img = OpenCvSharp.Unity.TextureToMat(texture);

            Destroy(texture);

            Mat imgGRAY = new Mat();
            //conversione in scala di grigi
            Cv2.CvtColor(img, imgGRAY, ColorConversionCodes.RGB2GRAY);

            //blurring gaussiano, smussa un po' l'immagine
            Cv2.GaussianBlur(imgGRAY, imgGRAY, new Size(7, 7), 0);
            Mat threshold = new Mat();
            //segmentazione con una threshold
            Cv2.Threshold(imgGRAY, threshold, th, 255, OpenCvSharp.ThresholdTypes.BinaryInv);
            // dimensioni del kernel per fare una dilatazione
            Size ksize = new OpenCvSharp.Size(7, 7);
            Cv2.Dilate(threshold, threshold, Cv2.GetStructuringElement(MorphShapes.Ellipse, ksize));

            Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;
            //trovo i contorni (lista di array)
            Cv2.FindContours(threshold, out contours, out hierarchyIndexes, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            if (contours.Length != 0)
            {

                //chiamo la funzione  e mi ricavo l'indice
                int index = MaxA(contours);


                //faccio il rettangolo e le linee   
                OpenCvSharp.Rect boundrect = Cv2.BoundingRect(contours[index]);
                Cv2.Rectangle(img, boundrect.TopLeft, boundrect.BottomRight, OpenCvSharp.Scalar.Magenta, 1);

                Cv2.Line(img, new Point(boundrect.Center.X, 0), new Point(boundrect.Center.X, image.height), OpenCvSharp.Scalar.Green, 1);
                Cv2.Line(img, new Point(0, boundrect.Center.Y), new Point(image.width, boundrect.Center.Y), OpenCvSharp.Scalar.Green, 1);
                //aspetto che stream calcoli le coordinate in modo tale che coordinate parta dopo l'elaborazione dei punti
                isStreaming = true;

                //necessito di convertire le coordinate nel rettangolo in coordinate "crop", cioè nelle coordinate dell'immagine croppata
                centrox = ((boundrect.Right - boundrect.Left) / 2) + boundrect.Left;
                centroy = ((boundrect.Bottom - boundrect.Top) / 2) + boundrect.Top;
                print("centrox " + centrox + " centroy " + centroy + " ex centro x " + boundrect.Center.X + "ex y " + boundrect.Center.Y);
                coordinate = new Point(centrox, centroy);

            }

            //stesso discorso di SceneController: metto il try perché questa riga mi serve solo
            try
            {
                ProcessedImage.texture = OpenCvSharp.Unity.MatToTexture(img);
            }

            catch
            {

            }


        }
    }

}