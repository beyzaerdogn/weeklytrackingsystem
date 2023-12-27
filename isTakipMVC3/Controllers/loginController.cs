using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using isTakipMVC3.Models;
namespace isTakipMVC3.Controllers
{
    public class loginController : Controller
    {
        İsTakipDBEntities1 entity = new İsTakipDBEntities1();
        // GET: login
        public ActionResult Index()
        {
            ViewBag.mesaj = null;
            return View();
        }
        [HttpPost]
        public ActionResult Index(string kullaniciAd, string parola)
        {
            var personel = entity.Personeller.FirstOrDefault(p => p.personelKullanıcıAd == kullaniciAd && p.personelParola == parola);
            //var personel = (from p in entity.Personeller where p.personelKullanıcıAd == kullaniciAd && p.personelParola == parola select p).FirstOrDefault();//bir veri dönmesi için 
            if (personel != null)
            {
                Session["PersonelAdSoyad"] = personel.personelAdSoyad;
                Session["PersonelId"]= personel.personelid;
                Session["PersonelBirimId"] = personel.personelBirimid;
                Session["PersonelYetkiTurId"] = personel.personelyetkiturid;
                switch (personel.personelyetkiturid)

                {  //yönetici ise 1 kullanılır
                    case 1: 
                        return RedirectToAction("Index","Yonetici"); //farklı sayfaya yönlenmek için method
                    case 2:
                        return RedirectToAction("Index", "Calisan");
                    default:
                        return View();
                }
    
            }
            else
            {
                ViewBag.mesaj = "Kullanıcı adı ya da parola yanlış";
                return View();
            }
        }
    }
}