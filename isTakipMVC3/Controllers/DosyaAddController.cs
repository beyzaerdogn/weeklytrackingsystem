using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace isTakipMVC3.Controllers
{
    public class DosyaAddController : Controller
    {
        // GET: DosyaAdd
        public ActionResult AddFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddFile(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    // Set the file size limit to 200MB
                    const int maxFileSize = 200 * 1024 * 1024; // 200MB in bytes

                    if (file.ContentLength > maxFileSize)
                    {
                        ViewBag.Message = "Dosya boyutu 200 MB'ı geçmemelidir.";
                        return View();
                    }

                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/web/uploads"), _FileName);
                    file.SaveAs(_path);

                    ViewBag.Message = "Dosya başarıyla yüklendi!!";
                }
                else
                {
                    ViewBag.Message = "Lütfen yüklenecek dosyayı seçin.";
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Dosya yükleme başarısız oldu! Hata:" + ex.Message;
                return View();
            }
        }
    }
}
