using isTakipMVC3.Controllers;
using isTakipMVC3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace isTakipMVC3.Controllers
{
    public class IsDurum
    {
        public int? Id { get; set; }
        public string isBaslık { get; set; }
        public string isAcıklama { get; set; }
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
                //var birim =(from b in entity.Birimler where b.birimid == birimId select b );

                var birim = entity.Birimler.FirstOrDefault(b => b.birimid == birimId);
                ViewBag.birimAdı = birim?.biriminAdı;
                int personelid = Convert.ToInt32(Session["personelid"]);

                //var isler =(from i in entity.isler where i.isPersonelid == personelid && i.isOkunma==false orderby i.baslanan_ve_iletilentarih descending select i).ToList();
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
            //var tekIs = (from i in entity.isler where i.isId == isId select i).FirstOrDefault();
            var tekIs = entity.isler.FirstOrDefault(i => i.isId == isId);
            tekIs.isOkunma = true;
            tekIs.isDurumİd = 2;
            
            entity.SaveChanges();
            TempData["id"] = isId;
            return RedirectToAction("Yap");
        }
        public ActionResult Yap()
        {
            int isId = int.Parse(TempData["id"].ToString());
            int yetkiTurId = Convert.ToInt16(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)

            {
                int personelId = Convert.ToInt16(Session["PersonelBirimId"]);
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
        public ActionResult Yap(int isId, string isYorum)
        {
            var tekIs = (from i in entity.isler where i.isId == isId select i).FirstOrDefault();

            if (isYorum == "") isYorum = "Kullanıcı Yorum Yapmadı";

            tekIs.bitirilen_ve_yapılantarih = DateTime.Now;
            tekIs.isDurumİd = 3;
            tekIs.isYorum = isYorum;

            entity.SaveChanges();
            return RedirectToAction("Index", "Calisan");
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
                    IsDurum isDurum = new IsDurum();
                    isDurum.Id = i.isDurumİd;
                    isDurum.isBaslık = i.isBaslık;
                    isDurum.isAcıklama = i.isAcıklama;
                    isDurum.baslanan_ve_iletilentarih = i.baslanan_ve_iletilentarih;
                    isDurum.bitirilen_ve_yapılantarih = i.bitirilen_ve_yapılantarih;
                    isDurum.durumlarAd = i.durumlar_.durumlarAd;
                    isDurum.isYorum = i.isYorum;
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
