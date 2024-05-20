using isTakipMVC3.Models;
using System;
using System.Linq;
using System.Web.Mvc;

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

            int yetkiTurId = Convert.ToInt16(Session["personelyetkiturid"]);

            if (yetkiTurId == 1)

            {
                int birimId = Convert.ToInt16(Session["personelBirimid"]);
                //var calisanlar = entity.Personeller.FirstOrDefault(p => p.personelBirimid == birimId && p.personelyetkiturid == 1);
                var calisanlar = entity.Personeller.Where(p => p.personelBirimid == birimId && p.personelyetkiturid == 2).ToList();

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
        public ActionResult Ata(isler gelenIs)
        {
            gelenIs.baslanan_ve_iletilentarih = DateTime.Now;
            gelenIs.isDurumİd = 1;
            gelenIs.isOkunma = false;

            entity.isler.Add(gelenIs);
            entity.SaveChanges(); //VERİ TABANINI ETKİLEMEK

            return RedirectToAction("Takip", "Yonetici");


        }
        //verileri form ile tutarız
        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt16(Session["personelyetkiturid"]);

            if (yetkiTurId == 1)

            {
                int birimId = Convert.ToInt16(Session["personelBirimid"]);
                //var calisanlar = entity.Personeller.FirstOrDefault(p => p.personelBirimid == birimId && p.personelyetkiturid == 1);
                var calisanlar = entity.Personeller.Where(p => p.personelBirimid == birimId && p.personelyetkiturid == 2).ToList();

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
            int yetkiTurId = Convert.ToInt16(Session["personelyetkiturid"]);

            if (yetkiTurId == 1)
            {
                Personeller secilenPersonel = (Personeller)TempData["secilen"];

                try
                {
                    //var isler = (from i in entity.isler where i.isPersonelid == secilenPersonel.personelid select i).ToList().OrderByDescending(i => i.baslanan_ve_iletilentarih);
                    var isler = entity.isler.Where(i => i.isPersonelid == secilenPersonel.personelid).ToList().OrderByDescending(i => i.baslanan_ve_iletilentarih);
                    ViewBag.isler = isler;
                    ViewBag.personeller = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();
                    return View();
                }
                catch (Exception)
                {
                    // Hata durumunda uygun bir hata sayfasına yönlendirme yapabilirsiniz.
                    return RedirectToAction("Takip", "Yonetici");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult CalisanEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CalisanEkle(Personeller yeniCalisan)
        {
            // Yeni çalışanın bilgilerini veritabanına ekleyin.
            entity.Personeller.Add(yeniCalisan);
            entity.SaveChanges();

            // Yöneticiyi çalışan takip sayfasına yönlendirin.
            return RedirectToAction("Takip", "Yonetici");
        }


    }

}





//önce view sonra controller controller içinden rekrar index view önceki oluşturduğumuz view yolunu kullanarak 
//sayfaya giriş yapmadan girilmemeli yetki türü 1 ise diSe koyarız