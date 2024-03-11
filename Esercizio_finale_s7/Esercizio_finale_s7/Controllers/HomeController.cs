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

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
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
        public ActionResult Login(Utente utente)
        {
            Utente utenteTrovato = db.Utente.FirstOrDefault(u => u.Email == utente.Email && u.Password == utente.Password);

            if (utenteTrovato != null)
            {
                Session["UserId"] = utenteTrovato.IdUtente;

                FormsAuthentication.SetAuthCookie(utente.Email, true);

                return RedirectToAction("Home", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Credenziali non valide. Riprova.");
                return View();
            }
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
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Registrazione([Bind(Exclude = "IdRuolo")] Utente utente)
        {
            if (ModelState.IsValid)
            {
                utente.IdRuolo = 1;
                db.Utente.Add(utente);
                db.SaveChanges();

                Session["IdUtente"] = utente.IdUtente;
                FormsAuthentication.SetAuthCookie(utente.Email, true);

                return RedirectToAction("Home");
            }

            return View();
        }

        public ActionResult VisualizzaCarrello()
        {
            if (Session["Carrello"] is List<CarrelloItem> carrelloItems && carrelloItems.Any())
            {
                return View(carrelloItems);
            }
            else
            {
                return View("Home");
            }
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
        public ActionResult ConcludiOrdine(string indirizzoConsegna, string noteSpeciali)
        {

            int userId = (int)Session["UserId"];

            if (userId == 0)
            {
                FormsAuthentication.SignOut();
            }

            if (Session["UserId"] != null)
            {
                if (Session["Carrello"] is List<CarrelloItem> carrelloItems && carrelloItems.Any())
                {
                    decimal totale = carrelloItems.Sum(item => item.Prodotto.Prezzo * item.Quantita);

                    var nuovoOrdine = new Ordine
                    {
                        IdUtente = (int)Session["UserId"],
                        DataOrdine = DateTime.Now,
                        Evaso = false,
                        Importo = totale,
                        IndirizzoConsegna = indirizzoConsegna,
                        NoteSpeciali = noteSpeciali,
                        DettaglioOrdine = carrelloItems.Select(item => new DettaglioOrdine
                        {
                            IdProdotto = item.Prodotto.IdProdotto,
                            Quantita = item.Quantita
                        }).ToList()
                    };

                    db.Ordine.Add(nuovoOrdine);
                    db.SaveChanges();

                    Session["Carrello"] = null;

                    return RedirectToAction("Home");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            return RedirectToAction("Home");
        }

    }
}