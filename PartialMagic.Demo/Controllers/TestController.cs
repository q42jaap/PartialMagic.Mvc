using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PartialMagic.Demo.Models;

namespace PartialMagic.Demo.Controllers
{
  public class TestController : Controller
  {
    public ActionResult TheProblem()
    {
      return View();
    }

    public ActionResult TheProblemException()
    {
      var model = new PageModel { PageTitle = "This view will throw an exception" };
      return View(model);
    }

    public ActionResult PartialOrNull()
    {
      var model = new PageModel { PageTitle = "This view will render 'The fragment is null!'" };
      //model.MainFragment = new FragmentModel { Title = "Uncomment this line", Text = "If you want to see that it works when not null"};
      return View(model);
    }

    public ActionResult PartialOrDiscard()
    {
      var model = new PageModel { PageTitle = "This view will not render the partial at all" };
      //model.MainFragment = new FragmentModel { Title = "Uncomment this line", Text = "If you want to see that it works when not null"};
      return View(model);
    }

    public ActionResult PartialOrDiscardIfEmpty()
    {
      var model = new PageModel { PageTitle = "This view will not render the partial at all" };
      //model.OtherFragments = new[] { new FragmentModel { Title = "Uncomment this line", Text = "If you want to see that it works when not null" } };
      //model.OtherFragments = new[] { new FragmentModel { Title = "Uncomment this line", Text = "If you want to see the wrapper in action (there is no empty <li></li>)" }, null };
      return View(model);
    }

    public ActionResult PartialOrDiscardWithWrapper()
    {
      var model = new PageModel { PageTitle = "This view will not render the partial or the wrapper at all" };
      //model.MainFragment = new FragmentModel { Title = "Uncomment this line", Text = "If you want to see that it works when not null"};
      return View(model);
    }

    public ActionResult PartialOrDiscardIfEmptyWithWrapper()
    {
      var model = new PageModel { PageTitle = "This view will not render the partial at all" };
      //model.OtherFragments = new[] { new FragmentModel { Title = "Uncomment this line", Text = "If you want to see that it works when not null" } };
      return View(model);
    }

  }
}
