using isTakipMVC3.Models;
using System;
using System.IO;
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

                bool yeniGorevVar = entity.isler.Any(i => i.isDurumİd == 1); // 1: Yeni görev durumu
                ViewBag.yeniGorevVar = yeniGorevVar;

                var yeniTamamlananGorevVar = entity.isler.Any(i => i.isDurumİd == 3 && i.bitirilen_ve_yapılantarih >= DateTime.Today);
                ViewBag.yeniTamamlananGorevVar = yeniTamamlananGorevVar;
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
            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)

            {
                int birimId = Convert.ToInt16(Session["PersonelBirimId"]);
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

            TempData["secilen"] = secilenPersonel; //viewbag tek aşamada bu da iki aşamada saklanmayı sağlar
            return RedirectToAction("Listele", "Yonetici");


        }

        [HttpPost]
        public ActionResult Sil(int id)
        {
            var isToDelete = entity.isler.FirstOrDefault(i => i.isId == id);
            if (isToDelete != null)
            {
                entity.isler.Remove(isToDelete);
                entity.SaveChanges();
            }

            return RedirectToAction("Listele", "Yonetici");
        }

        [HttpGet]
        public ActionResult Listele()
        {
            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                Personeller secilenPersonel = (Personeller)TempData["secilen"];

                if (secilenPersonel == null || secilenPersonel.personelid == null)
                {
                    // secilenPersonel veya personelid null ise hata sayfasına yönlendir
                    return RedirectToAction("Takip", "Yonetici");
                }

                try
                {
                    var isler = entity.isler?.Where(i => i.isPersonelid == secilenPersonel.personelid)
                                              .OrderByDescending(i => i.baslanan_ve_iletilentarih)
                                              .ToList();

                    if (isler == null)
                    {
                        // isler null ise hata sayfasına yönlendir
                        return RedirectToAction("Takip", "Yonetici");
                    }

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



        [HttpPost]
        public ActionResult CalisanEkle(Personeller yeniCalisan)
        {
            // Yeni çalışanın bilgilerini veritabanına ekleyin.
            entity.Personeller.Add(yeniCalisan);
            entity.SaveChanges();

            // Yöneticiyi çalışan takip sayfasına yönlendirin.
            return RedirectToAction("Takip", "Yonetici");
        }

        public ActionResult DosyaIndir(string dosyaAdi)
        {
            string filePath = Server.MapPath("~/web/uploads/" + dosyaAdi);

            if (!System.IO.File.Exists(filePath))
            {
                ViewBag.message = "Dosya Bulunamadı";
                return RedirectToAction("Listele");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, dosyaAdi);
        }
        public ActionResult GuncellenecekIsler()
        {
            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);
            int birimId = Convert.ToInt16(Session["PersonelBirimId"]);

            if (yetkiTurId == 1)
            {
                try
                {
                    // Yöneticinin birimindeki personellerin idDurum 2 olan işleri getir
                    var isler = entity.isler
                        .Where(i => i.isDurumİd == 2 && i.Personeller.personelBirimid == birimId)
                        .OrderByDescending(i => i.baslanan_ve_iletilentarih)
                        .ToList();

                    ViewBag.isler = isler;
                    ViewBag.isSayisi = isler.Count();
                    return View();
                }
                catch (Exception ex)
                {
                    // Hata durumunda uygun bir hata sayfasına yönlendirme yapabilirsiniz veya loglayabilirsiniz
                    ViewBag.ErrorMessage = "Bir hata oluştu: " + ex.Message;
                    return RedirectToAction("Index", "Yonetici");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Guncelleme(int id, string isAciklama)
        {
            if (string.IsNullOrEmpty(isAciklama))
            {
                ViewBag.ErrorMessage = "Açıklama boş olamaz.";
                return RedirectToAction("GuncellenecekIsler", "Yonetici");
            }

            try
            {
                var job = entity.isler.FirstOrDefault(i => i.isId == id);
                if (job != null)
                {
                    job.isAciklama = isAciklama; // İş açıklamasını güncelle
                    entity.SaveChanges(); // Veritabanına kaydet
                }
                else
                {
                    ViewBag.ErrorMessage = "Güncellenecek iş bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("GuncellenecekIsler", "Yonetici");
        }


    }


}
