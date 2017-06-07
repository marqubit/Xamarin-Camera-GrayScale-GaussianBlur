using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
//using Android.Support.V7.App;
using Android.Graphics;
using Android.Renderscripts;

namespace App_Camara
{
    [Activity(Label = "App_Camara", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //declaramos variables de las cositas
        ImageView imageView;
        TextView tvValor;
        Button btnCamarita;
        Button btnGrises;
        Button btnGaus;
        Bitmap bitmap;
        EditText etNumber;
        Bitmap bitmapGris;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.Main);
            btnCamarita = FindViewById<Button>(Resource.Id.btnShoot);
            btnGrises = FindViewById<Button>(Resource.Id.btnGrises);
            btnGaus = FindViewById<Button>(Resource.Id.btnGaus);
            tvValor = FindViewById<TextView>(Resource.Id.tvValor);
            imageView = FindViewById<ImageView>(Resource.Id.ivPicture);
            etNumber = FindViewById<EditText>(Resource.Id.etNumber);
            //para cuando le demos click :P 7
            bitmap = BitmapFactory.DecodeResource(imageView.Resources, Resource.Drawable.mimami);
            btnCamarita.Click += BtnCamarita_Click;
            btnGrises.Click += BtnGrises_Click;
            btnGaus.Click += BtnGaus_Click;

        }

        private void BtnGrises_Click(object sender, EventArgs e)
        {
            btnGrises.Clickable = false;
            bitmapGris = GrayScaleImage(bitmap);
            imageView.SetImageBitmap(bitmapGris);
            btnGaus.Alpha = 100;
            btnGaus.Clickable = true;
            tvValor.Alpha = 100;
            etNumber.Alpha = 100;
        }

        private void BtnGaus_Click(object sender, EventArgs e)
        {
            int value = int.Parse(etNumber.Text);
            if(value >24)
                value = 24;
            if(value <1)
                value = 1;
            imageView.SetImageBitmap(CreateBlurredImage(value, bitmapGris));
        }

        private void BtnCamarita_Click(object sender, EventArgs e)
        {
            //A menudo se utiliza para iniciar aplicaciones externas con la intención de hacer algo,
            //en este caso usaremos el intent para poder abrir la camarita :D 
            //creando una nueva actividad de android!
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }
        //Este metodo  Se le llama cuando sale una actividad que ha iniciado,
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            //cuando se llame este metodo 
            //se crea un bitmap el cual el imageVIew contendra
            base.OnActivityResult(requestCode, resultCode, data);
            bitmap = (Bitmap)data.Extras.Get("data");
            imageView.SetImageBitmap(bitmap);
            imageView.Alpha = 100;
            btnGrises.Alpha = 100;
            btnGrises.Clickable = true;
            btnCamarita.Clickable = false;

        }

        private Bitmap CreateBlurredImage(int radius, Bitmap originalBitmap)
        {
            // Load a clean bitmap and work from that.

            //** Asi estaba antes Bitmap originalBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.dog_and_monkeys);

            // Create another bitmap that will hold the results of the filter.
            Bitmap blurredBitmap;
            blurredBitmap = Bitmap.CreateBitmap(originalBitmap);

            // Create the Renderscript instance that will do the work.
            RenderScript rs = RenderScript.Create(this);

            // Allocate memory for Renderscript to work with
            Allocation input = Allocation.CreateFromBitmap(rs, originalBitmap, Allocation.MipmapControl.MipmapFull, AllocationUsage.Script);
            Allocation output = Allocation.CreateTyped(rs, input.Type);

            // Load up an instance of the specific script that we want to use.
            ScriptIntrinsicBlur script = ScriptIntrinsicBlur.Create(rs, Element.U8_4(rs));
            script.SetInput(input);

            // Set the blur radius
            script.SetRadius(radius);

            // Start the ScriptIntrinisicBlur
            script.ForEach(output);

            // Copy the output to the blurred bitmap
            output.CopyTo(blurredBitmap);

            return blurredBitmap;
        }

        private Bitmap GrayScaleImage(Bitmap src)
        {
            //Method which receives a bitmap and transforms it into a GrayScale bitmap
            double GS_RED = 0.299;
            double GS_GREEN = 0.587;
            double GS_BLUE = 0.114;

            Bitmap bmOut = Bitmap.CreateBitmap(src.Width, src.Height, src.GetConfig());
            //Pixel info
            int A, R, G, B;
            int pixel;

            // get image size
            int width = src.Width;
            int height = src.Height;

            // scan through every single pixel
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    // get one pixel color
                    pixel = src.GetPixel(x, y);
                    // retrieve color of all channels
                    A = Color.GetAlphaComponent(pixel);

                    R = Color.GetRedComponent(pixel);
                    G = Color.GetGreenComponent(pixel);
                    B = Color.GetBlueComponent(pixel);
                    // take conversion up to one single value
                    R = G = B = (int)(GS_RED * R + GS_GREEN * G + GS_BLUE * B);
                    // set new pixel color to output bitmap
                    bmOut.SetPixel(x, y, Color.Argb(A, R, G, B));
                }
            }

            return bmOut;
        }
    }
}

