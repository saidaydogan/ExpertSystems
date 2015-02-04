using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using numl.Model;
using numl.Supervised.DecisionTree;
using numl;
using System.Threading;

// S. Said Aydoğan

namespace TemizAraba
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private String OzellikGetir(String key)
        {
            return TemelBilgiler.Where(w => w.Key.Equals(key)).FirstOrDefault().Value.ToString();

        }

        private String NumerikVeri(String data)
        {
            return Regex.Replace(data, "[^0-9]", string.Empty).Replace(".", string.Empty);

        }

        private void SerializeEt()
        {
            System.IO.FileStream stream = new System.IO.FileStream(txtSerializeName.Text, System.IO.FileMode.OpenOrCreate);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(stream, Otomobiller);
            stream.Close();
        }

        private void DeSerializeEt()
        {
            System.IO.FileStream stream = new System.IO.FileStream(txtSerializeName.Text, System.IO.FileMode.OpenOrCreate);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Otomobiller = (List<Otomobil>)formatter.Deserialize(stream);
            stream.Close();
        }
     
        Dictionary<String, Object> TemelBilgiler = new Dictionary<String, Object>();
        List<Otomobil> Otomobiller = new List<Otomobil>();
        LearningModel Model;
        public void OtomobilEkle(string ilanAdresi)
        {
            if (KontrolleriYap(ilanAdresi))
            {

                TemelBilgiler.Clear();

                HtmlWeb hw = new HtmlWeb();
                hw.UseCookies = true;
                hw.CachePath = "cache.db";
                hw.UsingCache = true;
                HtmlAgilityPack.HtmlDocument doc = hw.Load(ilanAdresi);


                HtmlNode nodef = doc.DocumentNode.SelectSingleNode("//div[@class='classifiedInfo']//h3");
                if (nodef == null)
                    return;

                Otomobil oto = new Otomobil();
                string para = nodef.InnerHtml.Trim();
                double kur = 0;
                if (para.Contains("TL"))
                    kur = 1;
                else if (para.Contains("€"))
                    kur = 2.8;
                else if (para.Contains("$"))
                    kur = 2.1;
                else if (para.Contains("£"))
                    kur = 3.5;


                para = NumerikVeri(para);
                double fiyat = 0;
                if (para == null || !Double.TryParse(para, out fiyat))
                    return;

                TemelBilgiler.Add("Fiyat", fiyat * kur);


                HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//ul[@class='classifiedInfoList']//li");
                foreach (var node in nodeCollection)
                {
                    string key = HtmlEntity.DeEntitize(node.Element("strong").InnerText).Trim();
                    string value = HtmlEntity.DeEntitize(node.Element("span").InnerText).Trim();

                    TemelBilgiler.Add(key, (value));

                }

                oto.Fiyat = int.Parse(OzellikGetir("Fiyat"));
                oto.Yil = int.Parse(OzellikGetir("Yıl"));
                oto.Kilometre = int.Parse(NumerikVeri(OzellikGetir("Km")));
                oto.Vites = OzellikGetir("Vites").Equals("Manuel") ? Vites.Manuel : Vites.Otomatik;
                oto.Yakit = OzellikGetir("Yakıt").Equals("Dizel") ? Yakit.Dizel : Yakit.Benzin;
                oto.MotorHacmi = int.Parse(NumerikVeri(OzellikGetir("Motor Hacmi")));
                oto.MotorGücü = int.Parse(NumerikVeri(OzellikGetir("Motor Gücü")));
                oto.Garantili = OzellikGetir("Garanti").Equals("Evet") ? true : false;
                oto.TakasEdilir = OzellikGetir("Takas").Equals("Evet") ? true : false;
                oto.Durum = OzellikGetir("Durumu").Equals("Sıfır") ? Durum.Sıfır : Durum.IkinciEl;


                // Güvenlik özellikleri ve  İç donanım özellikleri ilk olarak YOK atanıyor.
                oto.GuvenlikOzellikleri = GuvenlikOzellikleri.YOK;
                oto.IcDonanim = IcDonanim.YOK;

                // Seçili özellikleri varsa okunarak oluşturulur.
                nodeCollection = doc.DocumentNode.SelectNodes("//div[@id='classifiedProperties']//ul//li[@class='selected']");
                if (nodeCollection != null && nodeCollection.Count > 0)
                {
                    HtmlNodeCollection col = nodeCollection[0].SelectNodes("//li[@class='selected']");
                    foreach (var ncol in nodeCollection)
                    {
                        if (ncol.InnerText.Trim().Equals("ABS"))
                            oto.GuvenlikOzellikleri = oto.GuvenlikOzellikleri | GuvenlikOzellikleri.Seviye1;
                        else if (ncol.InnerText.Trim().Equals("ASR"))
                            oto.GuvenlikOzellikleri = oto.GuvenlikOzellikleri | GuvenlikOzellikleri.Seviye2;
                        else if (ncol.InnerText.Trim().Equals("ESP"))
                            oto.GuvenlikOzellikleri = oto.GuvenlikOzellikleri | GuvenlikOzellikleri.Seviye3;
                        else if (ncol.InnerText.Trim().Equals("Kumaş Koltuk"))
                            oto.IcDonanim = oto.IcDonanim | IcDonanim.Seviye1;
                        else if (ncol.InnerText.Trim().Equals("Klima (Dijital)"))
                            oto.IcDonanim = oto.IcDonanim | IcDonanim.Seviye2;
                        else if (ncol.InnerText.Trim().Equals("Deri Koltuk"))
                            oto.IcDonanim = oto.IcDonanim | IcDonanim.Seviye3;
                        else if (ncol.InnerText.Trim().Equals("Yol Bilgisayarı"))
                            oto.IcDonanim = oto.IcDonanim | IcDonanim.Seviye4;
                    }


                    // Boyalı parça sayısı
                    nodeCollection = doc.DocumentNode.SelectNodes("//div[@id='classifiedProperties']//ul");
                    HtmlNodeCollection collect = nodeCollection[4].SelectNodes("li[@class='selected']");
                    int boyaliSayisi = collect != null ? collect.Count : 0;
                    oto.BoyaliParcaSayisi = boyaliSayisi;

                    // Degisen parça sayısı      
                    collect = nodeCollection[5].SelectNodes("li[@class='selected']");
                    int degisenSayisi = collect != null ? collect.Count : 0;
                    oto.DegisenParcaSayisi = degisenSayisi;
                }

                // Örnek Uzman Verisine Göre Fiyatı 10.000 - 30.000 TL arası olan absli her araba
                // 30.000 - 40.000 arasında klimalı ve deri koltuklu her araba
                // Abs, Asr , Esp ye sahip , garantili her araba alınabilir şeklinde kural belirlenmiştir
                bool alinirBu = false;
                if (fiyat >= 10000 && fiyat <= 30000)
                    alinirBu = true;
                if (fiyat >= 30000 && fiyat <= 40000 && oto.IcDonanim.HasFlag(IcDonanim.Seviye3) && oto.IcDonanim.HasFlag(IcDonanim.Seviye2))
                    alinirBu = true;
                if (oto.GuvenlikOzellikleri.HasFlag(GuvenlikOzellikleri.Seviye1) && oto.GuvenlikOzellikleri.HasFlag(GuvenlikOzellikleri.Seviye2) && oto.GuvenlikOzellikleri.HasFlag(GuvenlikOzellikleri.Seviye3))
                    alinirBu = true;

                oto.Alinir = alinirBu;


                Otomobiller.Add(oto);


            }

        }


        private void Basla(string AnaIlanSayfasi, int okunacakIlanSayisi, int offset)
        {        
            try
            {
                int sayfaSayisi = okunacakIlanSayisi / 50;
                bool bit = false;
                int i = 0;
                int eskiOffset;
                while (!bit && i < sayfaSayisi)
                {
                    eskiOffset = offset;
                    List<string> linkler = IlanlariAl(AnaIlanSayfasi.Replace("pagingOffset=" + eskiOffset, "pagingOffset=" + offset).ToString());
                    offset += 50; ;
                    for (int j = 0; j < linkler.Count; j++)
                    {
                        lblSite.Text = linkler[j];
                        Application.DoEvents();
                        OtomobilEkle(linkler[j]);
                        Thread.Sleep(500);
                        if (Otomobiller.Count >= okunacakIlanSayisi)
                            bit = true;
                    }
                    i++;
                }


                if (cbSerialize.Checked && Otomobiller.Count > 1)
                {
                    SerializeEt();
                    lblSite.Text = "Serialize edildi";
                }

                var d = Descriptor.Create<Otomobil>();
                var g = new DecisionTreeGenerator(d);
                g.SetHint(true);
                var model = Learner.Learn(Otomobiller, 0.80, 1000, g);

                lblSite.ForeColor = Color.Green;
                lblSite.Text = Otomobiller.Count + " adet otomobil için Model çıkarıldı.";

            }
            catch (Exception e)
            {
                lblSite.ForeColor = Color.Red;
                lblSite.Text = "Veriler alınamadı!";

            }

        }

        private List<string> IlanlariAl(string AnaIlanSayfasi)
        {
            List<string> linkler = new List<string>();

            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = hw.Load(AnaIlanSayfasi);
            HtmlNodeCollection coll = doc.DocumentNode.SelectNodes("//a[@class='classifiedTitle']");
            foreach (HtmlNode node in coll)
            {
                linkler.Add(@"http://www.sahibinden.com" + node.Attributes["href"].Value.Trim());
            }

            return linkler;

        }

        bool UrlMi(string Url)
        {
            string pattern = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(Url);
        }

        bool KontrolleriYap(string Link)
        {
            if (UrlMi(Link) && Link.Contains("www.sahibinden.com"))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Geçerli bir web adresi giriniz. Adresin 'sahibinden.com' olmasına dikkat ediniz.");

                return false;
            }
        }

        void BilgiYerlestir()
        {

            Otomobil oto = new Otomobil();

            foreach (Control l in this.Controls)
            {
                if (l is Label)
                {
                    if (l.TabIndex < 30)
                    {
                        ((Label)l).Text = TemelBilgiler.Where(w => l.Text.Contains(w.Key)).FirstOrDefault().Value.ToString();
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (cbDesialize.Checked)
                    DeSerializeEt();
            }
            catch (Exception e2)
            {
                MessageBox.Show("Deserialize edilemedi");
            }



        }

        private void btnOgren_Click(object sender, EventArgs e)
        {
            try
            {
                GuvenlikOzellikleri go = GuvenlikOzellikleri.YOK;
                IcDonanim id = IcDonanim.YOK;

                if (cbAbs.Checked)
                    go = go | GuvenlikOzellikleri.Seviye1;
                else if (cbAsr.Checked)
                    go = go | GuvenlikOzellikleri.Seviye2;
                else if (cbEsp.Checked)
                    go = go | GuvenlikOzellikleri.Seviye3;
                else if (cbKumasKoltuk.Checked)
                    id = id | IcDonanim.Seviye1;
                else if (cbKlima.Checked)
                    id = id | IcDonanim.Seviye2;
                else if (cbDeriKoltuk.Checked)
                    id = id | IcDonanim.Seviye3;
                else if (cbYolBilgsayarı.Checked)
                    id = id | IcDonanim.Seviye4;

                Otomobiller.Add(new Otomobil(int.Parse(txtFiyat.Text), int.Parse(txtYıl.Text), int.Parse(txtKm.Text), cbOtomatik.Checked ? Vites.Otomatik : Vites.Manuel, cbDizel.Checked ? Yakit.Dizel : Yakit.Benzin, int.Parse(txtMotorHacmi.Text), int.Parse(txtMotorGucu.Text), cbGaranti.Checked, cbTakas.Checked, cbSıfır.Checked ? Durum.Sıfır : Durum.IkinciEl, go, id, int.Parse(txtBoyalıSayısı.Text), int.Parse(txtDegisenSayısı.Text), cbAlınır.Checked));

                if (Otomobiller.Count < 2)
                {
                    MessageBox.Show("Öğrenme Yapılabilmesi için en az 2 kayıt olması gerek");
                    lblDurum.ForeColor = Color.Green;
                    lblDurum.Text = "Kayıt eklendi.";
                    return;
                }
                var d = Descriptor.Create<Otomobil>();
                var g = new DecisionTreeGenerator(d);
                g.SetHint(true);
                Model = Learner.Learn(Otomobiller, 0.80, 1000, g);

                lblDurum.ForeColor = Color.Green;
                lblDurum.Text = "Öğrenme Yapıldı : Doğruluk:" + Model.Accuracy;
                btnOgren.Enabled = false;
                linkLabel1.Visible = true;
            }
            catch (Exception)
            {
                lblDurum.ForeColor = Color.DarkRed;
                lblDurum.Text = "Otomobil eklenemedi.";
                linkLabel1.Visible = false;

            }

        }

        private void btnSina_Click(object sender, EventArgs e)
        {
            if (Model == null)
            {
                lblDurumSina.Text = "Model Yok";
                return;
            }
            try
            {
                GuvenlikOzellikleri go = GuvenlikOzellikleri.YOK;
                IcDonanim id = IcDonanim.YOK;

                if (cbAsr1.Checked)
                    go = go | GuvenlikOzellikleri.Seviye1;
                else if (cbAsr1.Checked)
                    go = go | GuvenlikOzellikleri.Seviye2;
                else if (cbEsp1.Checked)
                    go = go | GuvenlikOzellikleri.Seviye3;
                else if (cbDeriKoltuk1.Checked)
                    id = id | IcDonanim.Seviye1;
                else if (cbKlima1.Checked)
                    id = id | IcDonanim.Seviye2;
                else if (cbDeriKoltuk1.Checked)
                    id = id | IcDonanim.Seviye3;
                else if (cbYolBilgsayarı1.Checked)
                    id = id | IcDonanim.Seviye4;

                Otomobil oto = new Otomobil(int.Parse(txtFiyat1.Text), int.Parse(txtYıl1.Text), int.Parse(txtKm1.Text), cbOtomatik1.Checked ? Vites.Otomatik : Vites.Manuel, cbDizel1.Checked ? Yakit.Dizel : Yakit.Benzin, int.Parse(txtMotorHacmi1.Text), int.Parse(txtMotorGucu1.Text), cbGaranti1.Checked, cbTakas1.Checked, cbSıfır1.Checked ? Durum.Sıfır : Durum.IkinciEl, go, id, int.Parse(txtBoyalıSayısı1.Text), int.Parse(txtDegisenSayısı1.Text), null);

                lblDurumSina.ForeColor = Color.Green;
                lblDurumSina.Text = "Sınama Yapıldı :  Bu araba " + (Model.Model.Predict(oto).Alinir ? "alınır" : "alınmaz");
            }
            catch (Exception)
            {
                lblDurumSina.ForeColor = Color.DarkRed;
                lblDurumSina.Text = "Otomobil test edilemedi.";

            }




        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Otomobiller;
            dataGridView1.Refresh();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int index =txtLink.Text.IndexOf("&");
                // &pagingOffet= 13 karakter
                string off = txtLink.Text.Substring(index + 14, txtLink.Text.Length - index - 14);
                int offset = int.Parse(off);
                Basla("http://www.sahibinden.com/otomobil?pagingSize=50&pagingOffset=0", int.Parse(txtIlanSayisi.Text), offset);

            }
            catch (Exception ex)
            {
                lblDurumSina.ForeColor = Color.Red;
                lblDurumSina.Text = "Sahibinden.com'a bağlanılamadı.";

            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(Model.ToString());
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            btnOgren.Enabled = true;
        }

    }
}
