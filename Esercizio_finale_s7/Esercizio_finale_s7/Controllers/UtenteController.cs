using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Esercizio_finale_s7.Models;

namespace Esercizio_finale_s7.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UtenteController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Utente
        public ActionResult Index()
        {
            var utentiAttivi = db.Utente.Where(u => !u.IsDeleted).Include(u => u.Ruolo);
            return View(utentiAttivi.ToList());
        }

        // GET: Utente/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utente utente = db.Utente.Find(id);
            if (utente == null)
            {
                return HttpNotFound();
            }
            return View(utente);
        }

        // GET: Utente/Create
        public ActionResult Create()
        {
            ViewBag.IdRuolo = new SelectList(db.Ruolo, "IdRuolo", "Role");
            return View(new Utente());
        }

        // POST: Utente/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUtente,Nome,Cognome,Provincia,Citta,Indirizzo,Email,Password,IdRuolo")] Utente utente)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicateEmail = db.Utente.Any(u => u.Email == utente.Email && !u.IsDeleted);

                if (isDuplicateEmail)
                {
                    ModelState.AddModelError("Email", "Questo indirizzo email è già utilizzato da un altro utente attivo.");
                    ViewBag.IdRuolo = new SelectList(db.Ruolo, "IdRuolo", "Role", utente.IdRuolo);
                    return View(utente);
                }

                db.Utente.Add(utente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdRuolo = new SelectList(db.Ruolo, "IdRuolo", "Role", utente.IdRuolo);
            return View(utente);
        }

        // GET: Utente/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utente utente = db.Utente.Find(id);
            if (utente == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdRuolo = new SelectList(db.Ruolo, "IdRuolo", "Role", utente.IdRuolo);
            return View(utente);
        }

        // POST: Utente/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUtente,Nome,Cognome,Provincia,Citta,Indirizzo,Email,Password,IdRuolo")] Utente utente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(utente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdRuolo = new SelectList(db.Ruolo, "IdRuolo", "Role", utente.IdRuolo);
            return View(utente);
        }

        // GET: Utente/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utente utente = db.Utente.Find(id);
            if (utente == null)
            {
                return HttpNotFound();
            }
            return View(utente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Utente utente = db.Utente.Find(id);
            if (utente == null)
            {
                return HttpNotFound();
            }

            try
            {
                utente.IsDeleted = true;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Si è verificato un errore durante la soft delete dell'utente.";
                return View("Error");
            }
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
