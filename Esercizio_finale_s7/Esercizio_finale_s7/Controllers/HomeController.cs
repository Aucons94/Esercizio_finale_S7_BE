using Esercizio_finale_s7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Esercizio_finale_s7.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        readonly ModelDbContext db = new ModelDbContext();

        [AllowAnonymous]
        public ActionResult Home()
        {
            return View(db.Prodotto.ToList());
        }

        public ActionResult Profilo()
        {
            return View();
        }

        public ActionResult OrdiniUtente()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId;
                if (int.TryParse(User.Identity.Name, out userId))
                {
                    var ordiniUtente = db.Ordine
                        .Where(o => o.IdUtente == userId)
                        .OrderByDescending(o => o.DataOrdine)
                        .ToList();

                    return View(ordiniUtente);
                }
            }

            return RedirectToAction("Login", "Home");
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View(new Autorizzazione());
        }

        [AllowAnonymous]
        public ActionResult DettagliProdotto(int? id)
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Utente utente)
        {
            if (ModelState.IsValid)
            {

                Utente utenteTrovato = db.Utente.FirstOrDefault(u => u.Email == utente.Email);

                if (utenteTrovato != null)
                {
                    if (utenteTrovato.Password == utente.Password)
                    {
                        Session["UserId"] = utenteTrovato.IdUtente;
                        FormsAuthentication.SetAuthCookie(utente.Email, true);
                        return RedirectToAction("Home", "Home");
                    }
                    else
                    {
                        ViewBag.AuthError = "Aoh scrivi mejo. Riprova.";
                    }
                }
                else
                {
                    ViewBag.AuthError = "Hai sbajato maile!!";
                }
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Registrazione()
        {
            return View(new Utente());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Registrazione([Bind(Exclude = "IdRuolo")] Utente utente)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicateEmail = db.Utente.Any(u => u.Email == utente.Email && !u.IsDeleted);

                if (isDuplicateEmail)
                {
                    ModelState.AddModelError("Email", "Questo indirizzo email è già utilizzato da un altro utente attivo.");
                    return View(utente);
                }

                utente.IdRuolo = 1;
                db.Utente.Add(utente);
                db.SaveChanges();

                Session["IdUtente"] = utente.IdUtente;
                FormsAuthentication.SetAuthCookie(utente.Email, true);

                return RedirectToAction("Home");
            }

            return View(utente);
        }

        public ActionResult VisualizzaCarrello()
        {
            if (Session["Carrello"] is List<CarrelloItem> carrelloItems && carrelloItems.Any())
            {
                return View(carrelloItems);
            }

            return RedirectToAction("Home");
        }


        [HttpPost]
        public ActionResult AggiungiAlCarrello(int id, string nome, decimal prezzo, int quantita)
        {
            if (!(Session["Carrello"] is List<CarrelloItem> carrello))
            {
                carrello = new List<CarrelloItem>();
            }

            var carrelloItem = carrello.FirstOrDefault(item => item.Prodotto.IdProdotto == id);

            if (carrelloItem != null)
            {
                carrelloItem.Quantita += quantita;
            }
            else
            {
                carrello.Add(new CarrelloItem
                {
                    Prodotto = new Prodotto { IdProdotto = id, Nome = nome, Prezzo = prezzo },
                    Quantita = quantita
                });
            }

            Session["Carrello"] = carrello;

            return RedirectToAction("Home");
        }
        [HttpPost]
        public ActionResult RimuoviDalCarrello(int productId)
        {
            var carrello = Session["Carrello"] as List<CarrelloItem>;
            if (carrello != null)
            {
                var carrelloItem = carrello.FirstOrDefault(item => item.Prodotto.IdProdotto == productId);
                if (carrelloItem != null)
                {
                    carrello.Remove(carrelloItem);
                    Session["Carrello"] = carrello;
                }
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public ActionResult ConcludiOrdine(string indirizzoConsegna, string noteSpeciali)
        {
            if (User.Identity.IsAuthenticated)
            {
                string userEmail = User.Identity.Name;
                var utente = db.Utente.FirstOrDefault(u => u.Email == userEmail);

                if (utente != null)
                {
                    if (Session["Carrello"] is List<CarrelloItem> carrelloItems && carrelloItems.Any())
                    {
                        if (string.IsNullOrWhiteSpace(indirizzoConsegna))
                        {
                            ModelState.AddModelError("IndirizzoConsegna", "Inserisci un indirizzo di consegna.");
                            return View("VisualizzaCarrello", carrelloItems);
                        }

                        if (!ModelState.IsValid)
                        {
                            return View("VisualizzaCarrello", carrelloItems);
                        }

                        var nuovoOrdine = new Ordine
                        {
                            IdUtente = utente.IdUtente,
                            DataOrdine = DateTime.Now,
                            Evaso = false,
                            Importo = carrelloItems.Sum(item => item.Prodotto.Prezzo * item.Quantita),
                            IndirizzoConsegna = indirizzoConsegna,
                            NoteSpeciali = noteSpeciali,
                            DettaglioOrdine = new List<DettaglioOrdine>()
                        };

                        foreach (var item in carrelloItems)
                        {
                            var dettaglioOrdine = new DettaglioOrdine
                            {
                                IdProdotto = item.Prodotto.IdProdotto,
                                Quantita = item.Quantita
                            };
                            nuovoOrdine.DettaglioOrdine.Add(dettaglioOrdine);
                        }
                        db.Ordine.Add(nuovoOrdine);
                        db.SaveChanges();
                        Session["Carrello"] = null;

                        return RedirectToAction("Home");
                    }
                }
            }

            return RedirectToAction("Login");
        }

    }
}