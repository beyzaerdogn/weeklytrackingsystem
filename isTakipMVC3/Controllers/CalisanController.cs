using isTakipMVC3.Controllers;
using isTakipMVC3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace isTakipMVC3.Controllers
{
    public class IsDurum
    {

        public int? Id { get; set; }
        public string isBaslık { get; set; }
        public string isAciklama { get; set; }

        public DateTime? baslanan_ve_iletilentarih { get; set; }
        public DateTime? bitirilen_ve_yapılantarih { get; set; }
        public string durumlarAd { get; set; }
        public string isYorum { get; set; }
    }

    public class CalisanController : Controller
    {
        İsTakipDBEntities1 entity = new İsTakipDBEntities1();

        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 2)
            {
                int birimId = Convert.ToInt16(Session["PersonelBirimId"]);
                var birim = entity.Birimler.FirstOrDefault(b => b.birimid == birimId);
                ViewBag.birimAdı = birim?.biriminAdı;
                int personelid = Convert.ToInt32(Session["personelid"]);
                var isler = entity.isler.Where(i => i.isPersonelid == personelid && i.isDurumİd != 3).OrderByDescending(i => i.baslanan_ve_iletilentarih).ToList();
                ViewBag.isler = isler;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Index(int isId)
        {
            var tekIs = entity.isler.FirstOrDefault(i => i.isId == isId);
            tekIs.isOkunma = true;
            tekIs.isDurumİd = 2;
            entity.SaveChanges();
            TempData["id"] = isId;
            return RedirectToAction("Yap");
        }



        public ActionResult Yap()
        {
            int isId;
            if (TempData["id"] == null || !int.TryParse(TempData["id"].ToString(), out isId))
            {
                // Hata mesajı veya yönlendirme
                return RedirectToAction("Index", "Home");
            }

            int yetkiTurId;
            if (Session["PersonelYetkiTurId"] == null || !int.TryParse(Session["PersonelYetkiTurId"].ToString(), out yetkiTurId))
            {
                // Hata mesajı veya yönlendirme
                return RedirectToAction("Index", "Login");
            }

            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt16(Session["PersonelID"]);
                var isler = entity.isler.FirstOrDefault(i => i.isId == isId);
                ViewBag.isler = isler;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        [HttpPost]
        public ActionResult Yap(int isId, string isYorum, HttpPostedFileBase file, string button)
        {
            var tekIs = entity.isler.FirstOrDefault(i => i.isId == isId);
            tekIs.isYorum = isYorum;

            if (string.IsNullOrWhiteSpace(isYorum))
            {
                ViewBag.Message = "Lütfen bir yorum giriniz.";
                ViewBag.isler = tekIs; // Tekrar sayfayı yüklerken veri kaybını önlemek için
                return View();
            }

            try
            {
                if (button == "Guncelle")
                {
                    tekIs.isDurumİd = 2;
                    if (file != null && file.ContentLength > 0)
                    {
                        // Set the file size limit to 200MB
                        const int maxFileSize = 200 * 1024 * 1024; // 200MB in bytes

                        if (file.ContentLength > maxFileSize)
                        {
                            ViewBag.Message = "Dosya boyutu 200 MB'ı geçmemelidir.";
                            ViewBag.isler = tekIs; // Tekrar sayfayı yüklerken veri kaybını önlemek için
                            return View();
                        }

                        string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/web/uploads"), _FileName);

                        file.SaveAs(_path);
                        tekIs.fileName = _FileName;
                    }

                    entity.SaveChanges();
                     return RedirectToAction("Index");
                }
                else if (button == "Tamamla")
                {
                    if (file == null || file.ContentLength <= 0)
                    {
                        ViewBag.Message = "Lütfen yüklenecek dosyayı seçin.";
                        ViewBag.isler = tekIs; // Tekrar sayfayı yüklerken veri kaybını önlemek için
                        return View();
                    }

                    // Set the file size limit to 200MB
                    const int maxFileSize = 200 * 1024 * 1024; // 200MB in bytes

                    if (file.ContentLength > maxFileSize)
                    {
                        ViewBag.Message = "Dosya boyutu 200 MB'ı geçmemelidir.";
                        ViewBag.isler = tekIs; // Tekrar sayfayı yüklerken veri kaybını önlemek için
                        return View();
                    }

                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/web/uploads"), _FileName);

                    file.SaveAs(_path);
                    tekIs.fileName = _FileName;

                    tekIs.isDurumİd = 3;
                    tekIs.bitirilen_ve_yapılantarih = DateTime.Now;
                    entity.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Dosya yükleme başarısız oldu! Hata: " + ex.Message;
                ViewBag.isler = tekIs; // Tekrar sayfayı yüklerken veri kaybını önlemek için
                return View();
            }
        }
        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler = (from i in entity.isler
                             join d in entity.durumlar_ on i.isDurumİd equals d.durumlarid
                             where i.isPersonelid == personelId
                             select i).ToList().OrderByDescending(i => i.baslanan_ve_iletilentarih);
                IsDurumModel model = new IsDurumModel();
                List<IsDurum> list = new List<IsDurum>();
                foreach (var i in isler)
                {
                    IsDurum isDurum = new IsDurum
                    {
                        Id = i.isDurumİd,
                        isBaslık = i.isBaslık,
                        isAciklama = i.isAciklama,
                        baslanan_ve_iletilentarih = i.baslanan_ve_iletilentarih,
                        bitirilen_ve_yapılantarih = i.bitirilen_ve_yapılantarih,
                        durumlarAd = i.durumlar_.durumlarAd,
                        isYorum = i.isYorum
                    };
                    list.Add(isDurum);
                }
                model.isDurumlar = list;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }





    }
}





//using isTakipMVC3.Controllers;
//using isTakipMVC3.Models;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace isTakipMVC3.Controllers
//{
//    public class IsDurum
//    {

//        public int? Id { get; set; }
//        public string isBaslık { get; set; }
//        public string isAciklama { get; set; }

//        public DateTime? baslanan_ve_iletilentarih { get; set; }
//        public DateTime? bitirilen_ve_yapılantarih { get; set; }
//        public string durumlarAd { get; set; }
//        public string isYorum { get; set; }
//    }

//    public class CalisanController : Controller
//    {
//        İsTakipDBEntities1 entity = new İsTakipDBEntities1();

//        public ActionResult Index()
//        {
//            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);
//            if (yetkiTurId == 2)
//            {
//                int birimId = Convert.ToInt16(Session["PersonelBirimId"]);
//                var birim = entity.Birimler.FirstOrDefault(b => b.birimid == birimId);
//                ViewBag.birimAdı = birim?.biriminAdı;
//                int personelid = Convert.ToInt32(Session["personelid"]);
//                var isler = entity.isler.Where(i => i.isPersonelid == personelid && i.isDurumİd != 3).OrderByDescending(i => i.baslanan_ve_iletilentarih).ToList();
//                ViewBag.isler = isler;
//                return View();
//            }
//            else
//            {
//                return RedirectToAction("Index", "Login");
//            }
//        }

//        [HttpPost]
//        public ActionResult Index(int isId)
//        {
//            var tekIs = entity.isler.FirstOrDefault(i => i.isId == isId);
//            tekIs.isOkunma = true;
//            tekIs.isDurumİd = 2;
//            entity.SaveChanges();
//            TempData["id"] = isId;
//            return RedirectToAction("Yap");
//        }



//        public ActionResult Yap()
//        {
//            int isId;
//            if (TempData["id"] == null || !int.TryParse(TempData["id"].ToString(), out isId))
//            {
//                // Hata mesajı veya yönlendirme
//                return RedirectToAction("Index", "Home");
//            }

//            int yetkiTurId;
//            if (Session["PersonelYetkiTurId"] == null || !int.TryParse(Session["PersonelYetkiTurId"].ToString(), out yetkiTurId))
//            {
//                // Hata mesajı veya yönlendirme
//                return RedirectToAction("Index", "Login");
//            }

//            if (yetkiTurId == 2)
//            {
//                int personelId = Convert.ToInt16(Session["PersonelID"]);
//                var isler = entity.isler.FirstOrDefault(i => i.isId == isId);
//                ViewBag.isler = isler;
//                return View();
//            }
//            else
//            {
//                return RedirectToAction("Index", "Login");
//            }
//        }


//        [HttpPost]
//        public ActionResult Yap(int isId, string isYorum, HttpPostedFileBase file, string button)
//        {
//            var tekIs = entity.isler.FirstOrDefault(i => i.isId == isId);
//            tekIs.isYorum = isYorum;

//            if (string.IsNullOrWhiteSpace(isYorum))
//            {
//                ViewBag.Message = "Lütfen bir yorum giriniz.";
//                ViewBag.isler = tekIs; // Tekrar sayfayı yüklerken veri kaybını önlemek için
//                return View();
//            }

//            try
//            {
//                if (button == "Guncelle")
//                {
//                    tekIs.isDurumİd = 2;
//                    entity.SaveChanges();
//                    return RedirectToAction("Index");
//                }
//                else if (button == "Tamamla")
//                {
//                    if (file == null || file.ContentLength <= 0)
//                    {
//                        ViewBag.Message = "Lütfen yüklenecek dosyayı seçin.";
//                        ViewBag.isler = tekIs; // Tekrar sayfayı yüklerken veri kaybını önlemek için
//                        return View();
//                    }

//                    // Set the file size limit to 200MB
//                    const int maxFileSize = 200 * 1024 * 1024; // 200MB in bytes

//                    if (file.ContentLength > maxFileSize)
//                    {
//                        ViewBag.Message = "Dosya boyutu 200 MB'ı geçmemelidir.";
//                        ViewBag.isler = tekIs; // Tekrar sayfayı yüklerken veri kaybını önlemek için
//                        return View();
//                    }

//                    string _FileName = Path.GetFileName(file.FileName);
//                    string _path = Path.Combine(Server.MapPath("~/web/uploads"), _FileName);

//                    file.SaveAs(_path);
//                    tekIs.fileName = _FileName;

//                    tekIs.isDurumİd = 3;
//                    tekIs.bitirilen_ve_yapılantarih = DateTime.Now;
//                    entity.SaveChanges();

//                    return RedirectToAction("Index");
//                }

//                return View();
//            }
//            catch (Exception ex)
//            {
//                ViewBag.Message = "Dosya yükleme başarısız oldu! Hata: " + ex.Message;
//                ViewBag.isler = tekIs; // Tekrar sayfayı yüklerken veri kaybını önlemek için
//                return View();
//            }
//        }
//        public ActionResult Takip()
//        {
//            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);
//            if (yetkiTurId == 2)
//            {
//                int personelId = Convert.ToInt32(Session["PersonelId"]);
//                var isler = (from i in entity.isler
//                             join d in entity.durumlar_ on i.isDurumİd equals d.durumlarid
//                             where i.isPersonelid == personelId
//                             select i).ToList().OrderByDescending(i => i.baslanan_ve_iletilentarih);
//                IsDurumModel model = new IsDurumModel();
//                List<IsDurum> list = new List<IsDurum>();
//                foreach (var i in isler)
//                {
//                    IsDurum isDurum = new IsDurum
//                    {
//                        Id = i.isDurumİd,
//                        isBaslık = i.isBaslık,
//                        isAciklama = i.isAciklama,

//                        baslanan_ve_iletilentarih = i.baslanan_ve_iletilentarih,
//                        bitirilen_ve_yapılantarih = i.bitirilen_ve_yapılantarih,
//                        durumlarAd = i.durumlar_.durumlarAd,
//                        isYorum = i.isYorum
//                    };
//                    list.Add(isDurum);
//                }
//                model.isDurumlar = list;
//                return View(model);
//            }
//            else
//            {
//                return RedirectToAction("Index", "Login");
//            }
//        }





//    }
//}
