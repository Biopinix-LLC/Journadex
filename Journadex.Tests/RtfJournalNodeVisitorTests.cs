using Journadex.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Journadex.Tests
{
    [TestClass]
    public class RtfJournalNodeVisitorTests
    {
        [TestMethod]
        public void MyTestMethod()
        {
            JournalTree tree = new JournalTree("test");
            // Populate the tree with JournalNodes

            RtfJournalNodeVisitor visitor = new RtfJournalNodeVisitor(tree, isIndex: true);
            tree.Accept(visitor);

        }
    }
}
