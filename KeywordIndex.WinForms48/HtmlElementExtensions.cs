
using System;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public static class HtmlElementExtensions
    {

        public static string GetIdFromEditAnchor(this HtmlElement element, string idPrefix = "about:blank#edit_")
        {
            if (element.TagName == "LI")
            {
                var anchorElements = element.GetElementsByTagName("A");
                foreach (HtmlElement anchorElement in anchorElements)
                {
                    if (anchorElement.GetAttribute("href").StartsWith(idPrefix))
                    {
                        return anchorElement.GetAttribute("href").Substring(idPrefix.Length);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the previous HtmlElement with the specified tag name.
        /// ChatGPT: I need it to traverse backward up the tree until it finds a previous element with the tagName. Right now it just looks at the current parent but I need it to be recursive.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static HtmlElement GetPreviousElementByTagName(this HtmlElement element, string tagName)
        {
            int index = element.GetElementIndex();
            if (index > -1)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    if (element.Parent.Children[i].TagName == tagName)
                    {
                        return element.Parent.Children[i];
                    }
                }
                return GetPreviousElementByTagName(element.Parent, tagName);
            }
            return null;
        }

        /// <summary>
        /// ChatGPT: Write a C# extension method to a WinForms HtmlElement that will compare to another HtmlElement in the document compare if the current is before or after the other.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static int CompareTo(this HtmlElement current, HtmlElement other)
        {
            int currentIndex = current.GetElementIndex();
            int otherIndex = other.GetElementIndex();
            return currentIndex.CompareTo(otherIndex);
        }

        private static int GetElementIndex(this HtmlElement element)
        {
            if (element == null) return -1;
            HtmlElement parent = element.Parent;
            if (parent == null) return 0;
            int index = 0;
            foreach (HtmlElement child in parent.Children)
            {
                if (child.Equals(element))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
    }

}
