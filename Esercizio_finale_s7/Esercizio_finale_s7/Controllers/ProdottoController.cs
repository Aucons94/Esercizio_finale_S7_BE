using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Esercizio_finale_s7.Models;

namespace Esercizio_finale_s7.Controllers
{
    public class ProdottoController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Prodottoe
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var prodotti = db.Prodotto.Where(p => !p.IsDeleted).ToList();
            return View(prodotti);
        }

        // GET: Prodottoe/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prodotto prodotto = db.Prodotto.Find(id);
            if (prodotto == null)
            {
                return HttpNotFound();
            }
            return View(prodotto);
        }

        // GET: Prodottoe/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new Prodotto());
        }

        // POST: Prodottoe/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "IdProdotto,Nome,Foto,Prezzo,TempoConsegna,Ingredienti")] Prodotto prodotto, HttpPostedFileBase Foto)
        {
            if (Foto != null && Foto.ContentLength > 0)
            {
                string nomeFile = Path.GetFileName(Foto.FileName);
                string pathToSave = Path.Combine(Server.MapPath("~/Content/Images/"), nomeFile);
                Foto.SaveAs(pathToSave);
                prodotto.Foto = nomeFile;
            }
            if (ModelState.IsValid)
            {
                db.Prodotto.Add(prodotto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(prodotto);
        }

        // GET: Prodottoe/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prodotto prodotto = db.Prodotto.Find(id);
            if (prodotto == null)
            {
                return HttpNotFound();
            }
            return View(prodotto);
        }

        // POST: Prodottoe/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "IdProdotto,Nome,Foto,Prezzo,TempoConsegna,Ingredienti")] Prodotto prodotto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prodotto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(prodotto);
        }

        // GET: Prodottoe/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prodotto prodotto = db.Prodotto.Find(id);
            if (prodotto == null)
            {
                return HttpNotFound();
            }
            return View(prodotto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Prodotto prodotto = db.Prodotto.Find(id);
            if (prodotto == null)
            {
                return HttpNotFound();
            }
            prodotto.IsDeleted = true;
            db.Entry(prodotto).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
