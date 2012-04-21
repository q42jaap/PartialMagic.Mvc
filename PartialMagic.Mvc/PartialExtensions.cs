using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using System.IO;
using System.Globalization;

namespace PartialMagic.Mvc
{
  public static class PartialExtensions
  {

    // copied from HtmlHelper.FindPartialView because it's original is internal
    internal static IView FindPartialView(ViewContext viewContext, string partialViewName, ViewEngineCollection viewEngineCollection)
    {
      ViewEngineResult result = viewEngineCollection.FindPartialView(viewContext, partialViewName);
      if (result.View != null)
      {
        return result.View;
      }
      StringBuilder builder = new StringBuilder();
      foreach (string str in result.SearchedLocations)
      {
        builder.AppendLine();
        builder.Append(str);
      }
      throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, 
        "The partial view '{0}' was not found or no view engine supports the searched locations. The following locations were searched:{1}",
        new object[] { partialViewName, builder }));
    }

    // copied from HtmlHelper.RenderPartialInternal (thanks Red Gate!)
    // modified the logics to OrNull functionality
    internal static void RenderPartialInternalOrNull(HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData, object model, TextWriter writer, ViewEngineCollection viewEngineCollection)
    {
      if (string.IsNullOrEmpty(partialViewName))
      {
        throw new ArgumentNullException("partialViewName");
      }
      ViewDataDictionary dictionary = new ViewDataDictionary(viewData ?? htmlHelper.ViewData);

      // we explicitly set the model here so we don't get the current Model as fallback.
      dictionary.Model = model;

      ViewContext viewContext = new ViewContext(htmlHelper.ViewContext, htmlHelper.ViewContext.View, dictionary, htmlHelper.ViewContext.TempData, writer);
      FindPartialView(viewContext, partialViewName, viewEngineCollection).Render(viewContext, writer);
    }

    /// <summary>
    /// Renders the specified partial view as an HTML-encoded string (even if the model is null).
    /// This is a safer method than the normal HtmlHelper.Partial() because that one will use the current Model as a fallback when the given model is null.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="partialViewName">The name of the partial view to render.</param>
    /// <param name="model">The model for the partial view, may be null</param>
    /// <param name="viewData">A new dictionary or null (in which case the current view data is used as a fallback)</param>
    /// <returns></returns>
    public static MvcHtmlString PartialOrNull(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData = null)
    {
      using (StringWriter writer = new StringWriter(CultureInfo.CurrentCulture))
      {
        RenderPartialInternalOrNull(htmlHelper, partialViewName, viewData, model, writer, ViewEngines.Engines);
        return MvcHtmlString.Create(writer.ToString());
      }
    }

    /// <summary>
    /// Renders the specified partial view as an HTML-encoded string unless the model is null.
    /// Note that the partial is not exectued when the model is null.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="partialViewName">The name of the partial view to render.</param>
    /// <param name="model">The model for the partial view, may be null</param>
    /// <param name="viewData">A new dictionary or null (in which case the current view data is used as a fallback)</param>
    /// <returns>The partial view that is rendered as an HTML-encoded string or null if the model is null.</returns>
    public static MvcHtmlString PartialOrDiscard(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData = null)
    {
      if (model == null)
        return null;
      return htmlHelper.Partial(partialViewName, model, viewData);
    }

    /// <summary>
    /// Renders the specified partial view as an HTML-encoded string unless the model is null.
    /// The given wrapper is used to wrap around the output of the partial result, it uses the Razor @item to place the partial output within the wrapper's template.
    /// Note the partial and the wrapper are not exectued when the model is null.
    /// </summary>
    /// <remarks>
    /// Note that the partial is rendered before wrapper is executed (should there be side-effects in either of them).
    /// </remarks>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="partialViewName">The name of the partial view to render.</param>
    /// <param name="model">The model for the partial view, may be null</param>
    /// <param name="wrapper">This wrapper is excuted when the model is not null, use @item to render the output of the partial</param>
    /// <param name="viewData">A new dictionary or null (in which case the current view data is used as a fallback)</param>
    /// <returns>The partial view that is rendered as an HTML-encoded string or null if the model is null.</returns>
    public static HelperResult PartialOrDiscard(this HtmlHelper htmlHelper, string partialViewName, object model, Func<MvcHtmlString, HelperResult> wrapper, ViewDataDictionary viewData = null)
    {
      if (model == null)
        return null;
      return new HelperResult(writer =>
      {
        var partialResult = htmlHelper.Partial(partialViewName, model, viewData);
        wrapper(partialResult).WriteTo(writer);
      });
    }

    /// <summary>
    /// Renders the specified partial view as an HTML-encoded string unless the model is null or empty.
    /// Note that the partial is not exectued when the model is null or empty.
    /// </summary>
    /// <remarks>
    /// The enumerable passed into the model is checked with Any() to see whether it is empty.
    /// In case of a LINQ query this may cause the enumerable to be called twice (once for the Any() check and once propably inside the partial).
    /// </remarks>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="partialViewName">The name of the partial view to render.</param>
    /// <param name="model">The model for the partial view, may be null or an empty enumerable</param>
    /// <param name="viewData">A new dictionary or null (in which case the current view data is used as a fallback)</param>
    /// <returns>The partial view that is rendered as an HTML-encoded string or null if the model is null or empty.</returns>
    public static MvcHtmlString PartialOrDiscardIfEmpty(this HtmlHelper htmlHelper, string partialViewName, IEnumerable<object> model, ViewDataDictionary viewData = null)
    {
      if (model == null || !model.Any())
        return null;
      return htmlHelper.Partial(partialViewName, model, viewData);
    }

    /// <summary>
    /// Renders the specified partial view as an HTML-encoded string unless the model is null or empty.<br/>
    /// The given wrapper is used to wrap around the output of the partial result, it uses the Razor @item to place the partial output within the wrapper's template.
    /// Note the partial and the wrapper are not exectued when the model is null or empty.
    /// </summary>
    /// <remarks>
    /// The enumerable passed into the model is checked with Any() to see whether it is empty.
    /// In case of a LINQ query this may cause the enumerable to be called twice (once for the Any() check and once propably inside the partial).
    /// Note that the partial is rendered before wrapper is executed (should there be side-effects in either of them).
    /// </remarks>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="partialViewName">The name of the partial view to render.</param>
    /// <param name="model">The model for the partial view, may be null or an empty enumerable</param>
    /// <param name="wrapper">This wrapper is excuted when the model is not null or empty, use @item to render the output of the partial</param>
    /// <param name="viewData">A new dictionary or null (in which case the current view data is used as a fallback)</param>
    /// <returns>The partial view that is rendered as an HTML-encoded string or null if the model is null or empty.</returns>
    public static HelperResult PartialOrDiscardIfEmpty(this HtmlHelper htmlHelper, string partialViewName, IEnumerable<object> model, Func<MvcHtmlString, HelperResult> wrapper, ViewDataDictionary viewData = null)
    {
      if (model == null || !model.Any())
        return null;
      return new HelperResult(writer =>
      {
        var partialResult = htmlHelper.Partial(partialViewName, model, viewData);
        wrapper(partialResult).WriteTo(writer);
      });
    }

  }
}
