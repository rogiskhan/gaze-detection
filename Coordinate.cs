using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class Coordinate : MonoBehaviour
{
    
    public float x, y, prev_x = 0, prev_y = 0, x_0, y_0;
    public int control_x, control_y;

    private StreamVideo video;
    float dist_x, dist_y;
    Point centro, new_point;
    private SceneController pippo;

    void Start()
    {
        pippo = GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()

    {
        //controllo se ho trovato almeno un occhio
        if (pippo.face==true)
        {
            video = GetComponent<StreamVideo>();
            //controllo se stream video sta facendo il suo lavoro
            if (video.isStreaming == true)
            {
                //qua mi le coordinate vengono normalizzate in un intervallo tra -1 e 1. Prima si fa un min max scaling
                new_point = video.coordinate;
                float numx = new_point.X - pippo.sx;
                float denx = pippo.dx - pippo.sx;
                float numy = new_point.Y - pippo.iy;
                float deny = pippo.sy - pippo.iy;
                //poi si trasla di -0.5 e si moltiplica per 2 per mantenermi tra -1 e 1 (b-a)*... scaling 
                dist_x = ((numx / denx) - 0.5f) * 2;
                //Sulle y è necessario un piccolo fattore correttivo in quanto la rete riconosce come bordo inferiore dell'occhio un punto dove la pupilla non ci arriva
                //inoltre a causa della bassa risoluzione se si guarda molto giù si perde la segmentazione perché le ciglia coprono l'iride e nei casi peggiori pure il riconoscimento dell'occhio
                dist_y = (((numy / deny) - 0.5f) * 2) + 0.3f;
                print("new_point " + new_point.Y + " centro " + centro.Y + " supy " + pippo.sy + " infy " + pippo.iy);
                print("new_point " + new_point.Y + " centro " + centro.Y + "dist_x: " + dist_x + " dist_y: " + dist_y + " distx " + " sx : " + pippo.sx + " dx : " + pippo.dx + " supy : " + pippo.sy + " infy: " + pippo.iy);

                //la coordinata da dare in pasto al cursore viene ottenuta in una somma pesata con le coordinate del frame precedente
                //questo serve ad ottenere un effetto più stabilizzato. I pesi sono comunque messi a 0.2 per la coordinata vecchia e 0.8 per la nuova
                //La scelta è ricaduta su questa combinazione perché alzando di più il peso delle vecchie coordinate si avrebbe una eccessiva latenza
                x_0 = 0.2f * prev_x + 0.8f * dist_x;
                y_0 = 0.2f * prev_y + 0.8f * dist_y;
                //  viene fatto un controllo se si sono superati i limiti dell'occhio
                //serve per pulire i frame dagli errori di segmentazione
                if (y_0 > 1.3 || y_0 < -0.7 || x_0 > 1 || x_0 < -1)
                {
                    x = 0;
                    y = 0;
                    print("x :" + dist_x);
                    print("y :" + dist_y + " pre " + new_point.Y + " supy " + pippo.sy + " infy " + pippo.iy + " centro " + centro.Y);
                }
                else
                {
                    x = x_0;
                    y = y_0;
                }
                prev_x = x;
                prev_y = y;
                print("x: " + x);
                print("y: " + y);

            }
        }
        else
        {
            print("Sono nell'else");
            x = 0;
            y = 0;
            prev_x = 0;
            prev_y = 0;
        }
    }
}