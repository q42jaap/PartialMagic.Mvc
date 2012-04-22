This small project contains a set of ASP.NET MVC3 HtmlHelper extension methods targeted at the Razor viewengine, helping with calling partial views in a safe and predictable way.

Suppose you're using partial views to share code between views or seperate pieces of html to create clean and readable cshtml files, like this:

    # Index.cshtml
    @model PageViewModel
    @Html.Partial("SomePartial", Model.SomeProperty)

    # SomePartial.cshtml
    @model PartialViewModel
    ....

You'll find yourselve getting the following exception when you (accidentally) pass null into the Partial method's model parameter (e.g. Model.SomeProperty is null):

    The model item passed into the dictionary is of type 'PageViewModel', but this dictionary requires a model item of type 'PartialViewModel'.

The problem is that this overload of Partial uses a fallback in case the model parameter is null. It falls back to the current model wich is of type PageViewModel. The exception says this in a cryptic way.

The reason this is the way it is (being very counter intuitive), is that when you work with dynamic views (i.e. you don't specify @model or specify @model dynamic in your partial view) you still get a model in the partial. The MVC's team assumption that this is what developers want, is unfortunately wrong:
http://stackoverflow.com/questions/650393/asp-net-mvc-renderpartial-with-null-model-gets-passed-the-wrong-type

This repository contains a set of 5 methods that can be used to work with partials in views that are written in the Razor syntax (cshtml). PartialOrNull is the main reason why you're reading this now. It is what the Partial method should have been in the first place.

PartialOrDiscard is what came up when I first put PartialOrNull in a file. It is a logic extension and is actualy very basic, it just ignores the whole partial if the model is null. It is shorter and clearly shows by it's name that you've considered the possibility of the parameter model being null.

After that things got a bit out of hand and I added an overload to PartialOrDiscard which allows you to specify a template (I like to call them Razor-lambdas) which can wrap the output of the partial with some html. I also added a variant that supports IEnumerables, both with and without a wrapper. The PartialOrDiscardIfEmpty methods check, in adition to a null check, whether the Enumerable.Any() method returns true. If not the whole partial doesn't get excuted.

NOTE:
When you use PartialOrDiscardIfEmpty, your enumerable gets activated at least once by the check Enumerable.Any(). If this is a Linq object that does a database query, you might end up with two queries because your partial that you're trying to call will probably foreach over the enumerable.
However, it is a good practice to use the .ToList() method on your linq query in your controllers. Fetching data is the responsibility of the controller not the view.
Don't say I didn't warn you!

By the way, this shows a flaw in the .NET core framework, there should have been an interface between IEnumerable and ICollection, so that you can have a lazy list but cheaply check (or check in a controlled manner) to see if it's empty or what is the count of a set. ICollection defines methods like Add and Remove, which throw exceptions in some situations. This makes that people generally prefer IEnumerable over ICollection. But I should probably take this discussion somewhere else!