using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartialMagic.Demo.Models
{
  public class PageModel
  {
    public string PageTitle { get; set; }
    public FragmentModel MainFragment { get; set; }
    public IEnumerable<FragmentModel> OtherFragments { get; set; }
  }
}