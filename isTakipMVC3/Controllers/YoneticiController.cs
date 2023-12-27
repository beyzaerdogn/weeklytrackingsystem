using isTakipMVC3.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace isTakipMVC3.Controllers
{
    public class YoneticiController : Controller
    {
        İsTakipDBEntities1 entity = new İsTakipDBEntities1();
        // GET: Yonetici
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)

            {
                int birimId = Convert.ToInt16(Session["PersonelBirimId"]);
                //var birim =(from b in entity.Birimler where b.birimid == birimId select b );

                var birim = entity.Birimler.FirstOrDefault(b => b.birimid == birimId);
                ViewBag.birimAdı = birim?.biriminAdı;


                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }


        }
        public ActionResult Ata()
        {

            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)

            {
                int birimId = Convert.ToInt16(Session["PersonelBirimId"]);
                //var calisanlar = entity.Personeller.FirstOrDefault(p => p.personelBirimid == birimId && p.personelyetkiturid == 1);
                var calisanlar = entity.Personeller.Where(p => p.personelBirimid == birimId && p.personelyetkiturid == 1).ToList();

                ViewBag.personeller = calisanlar;
                var birim = entity.Birimler.FirstOrDefault(b => b.birimid == birimId);
                ViewBag.birimAdı = birim.biriminAdı;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
        [HttpPost]
        public ActionResult Ata(System.Web.Mvc.FormCollection formCollection)

        {
            String isBaslik = formCollection["isBaslik"];
            string isAciklama = formCollection["isAciklama"];
            int selectPersonelId = Convert.ToInt16(formCollection["SelectPer"]);

            isler yeniIs = new isler();
            yeniIs.isBaslık = isBaslik;
            yeniIs.isAcıklama = isAciklama;
            yeniIs.isPersonelid = selectPersonelId;
            yeniIs.baslanan_ve_iletilentarih = DateTime.Now;
            yeniIs.isDurumİd = 1;

            entity.isler.Add(yeniIs);
            entity.SaveChanges(); //VERİ TABANINI ETKİLEMEK

            return RedirectToAction("Takip", "Yonetici");


        }
        //verileri form ile tutarız
        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)

            {
                int birimId = Convert.ToInt16(Session["PersonelBirimId"]);
                //var calisanlar = entity.Personeller.FirstOrDefault(p => p.personelBirimid == birimId && p.personelyetkiturid == 1);
                var calisanlar = entity.Personeller.Where(p => p.personelBirimid == birimId && p.personelyetkiturid == 1).ToList();

                ViewBag.personeller = calisanlar;
                var birim = entity.Birimler.FirstOrDefault(b => b.birimid == birimId);
                ViewBag.birimAdı = birim.biriminAdı;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }


        }
       [HttpPost]
        public ActionResult Takip(int selectPer)
        {
            var secilenPersonel = entity.Personeller.FirstOrDefault(p => p.personelid == selectPer);
            //var secilenPersonel=(from p in entity.Personeller where p.personelid == selectPer select p).FirstOrDefault();
            
            TempData["secilen"]= secilenPersonel; //viewbag tek aşamada bu da iki aşamada saklanmayı sağlar
            return RedirectToAction("Listele", "Yonetici");


        }
        [HttpGet]
        public ActionResult Listele()
        {
            {
                int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);

                if (yetkiTurId == 1)

                {
                    Personeller secilenPersonel = (Personeller)TempData["secilen"];
                    var isler = (from i in entity.isler where i.isPersonelid == secilenPersonel.personelid select i).ToList();
                    ViewBag.isler = isler;
                    ViewBag.personeller = secilenPersonel;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }


            }
        }
    }
}












//önce view sonra controller controller içinden rekrar index view önceki oluşturduğumuz view yolunu kullanarak 
//sayfaya giriş yapmadan girilmemeli yetki türü 1 ise diye koyarız