using numl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemizAraba
{
    public enum Vites
    {
        Manuel, Otomatik

    }
    public enum Yakit
    {
        Benzin, Dizel
    }

    public enum Durum
    {
        Sıfır, IkinciEl
    }
    [Flags]
    public enum GuvenlikOzellikleri
    {
        YOK, Seviye1, Seviye2, Seviye3
    }
    [Flags]
    public enum IcDonanim
    {
        YOK, Seviye1, Seviye2, Seviye3, Seviye4
    }
    [Serializable]
    public class Otomobil
    {
        [Feature]
        public int Fiyat { get; set; }

        [Feature]
        public int Yil { get; set; }

        [Feature]
        public int Kilometre { get; set; }

        [Feature]
        public Vites Vites { get; set; }

        [Feature]
        public Yakit Yakit { get; set; }

        [Feature]
        public int MotorHacmi { get; set; }

        [Feature]
        public int MotorGücü { get; set; }

        [Feature]
        public bool Garantili { get; set; }

        [Feature]
        public bool TakasEdilir { get; set; }

        [Feature]
        public Durum Durum { get; set; }

        [Feature]
        public GuvenlikOzellikleri GuvenlikOzellikleri { get; set; }

        [Feature]
        public IcDonanim IcDonanim { get; set; }
             
        [Feature]
        public int BoyaliParcaSayisi { get; set; }

        [Feature]
        public int DegisenParcaSayisi { get; set; }
        [Label]
        public bool Alinir { get; set; }

        public Otomobil()
        {

        }
        public Otomobil(int Fiyat,int Yil, int Kilometre, Vites Vites, Yakit Yakit, int MotorHacmi,
                        int MotorGücü, bool Garantili, bool TakasEdilir, Durum Durum,
                        GuvenlikOzellikleri GuvenlikOzellikleri, IcDonanim IcDonanim,
                        int BoyaliParcaSayisi, int DegisenParcaSayisi,bool? Alinir)
        {
            this.Fiyat = Fiyat;
            this.Yil = Yil;
            this.Kilometre = Kilometre;
            this.Vites = Vites;
            this.Yakit = Yakit;
            this.MotorHacmi = MotorHacmi;
            this.MotorGücü = MotorGücü;
            this.Garantili = Garantili;
            this.TakasEdilir = TakasEdilir;
            this.Durum = Durum;
            this.GuvenlikOzellikleri = GuvenlikOzellikleri;
            this.IcDonanim = IcDonanim;
            this.BoyaliParcaSayisi = BoyaliParcaSayisi;
            this.DegisenParcaSayisi = DegisenParcaSayisi;
            if (Alinir.HasValue)
                this.Alinir = Alinir.Value;
        }


    }
}

