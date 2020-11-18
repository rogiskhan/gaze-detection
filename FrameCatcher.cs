//Script generico da attaccare ad una RawImage affinche venga 'colorata' dal frame catturato dal device video

//Versione minima perchè si veda qualcosa

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCatcher : MyWebCamera
{
    protected override void Awake()
    {
        base.Awake();
        base.forceFrontalCamera = true; // we work with frontal cams here
    }
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        OpenCvSharp.Mat img = OpenCvSharp.Unity.TextureToMat(input, TextureParameters);

        output = OpenCvSharp.Unity.MatToTexture(img, output);

        return true;
    }
}
