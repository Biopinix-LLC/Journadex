using Journadex.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Journadex.Tests
{
    [TestClass]
    public class JournalTreeTests
    {
        [TestMethod]
        public void RootRangeLengthShouldMatchJournalLength()
        {
            const string JournalText = "test";
            JournalTree journalTree = new JournalTree(JournalText);
            Assert.AreEqual(journalTree.Root.Range.Length, JournalText.Length);
        }

        [TestMethod]
        public void RootShouldContainOneChildEntryWithDateOfFeb4_2001()
        {
            const string JournalText = @"February 4th 2001
This is a test sentance.

This is a second test sentance.
 
";
            JournalTree journalTree = new JournalTree(JournalText);
            Assert.IsTrue(journalTree.Root.Children.Count== 1);
            Assert.AreEqual(journalTree.Root.Children[0].DateTime, new DateTime(2001, 2, 4));
        }

        [TestMethod]
        public void RootShouldContainThreeEntries()
        {
            const string JournalText = @"February 4th 2001
This is a test sentance.

February 5th 2001 - Page 6
 
This is another test sentance.
 
Page 8 
February 6th 2001 
 
This is a third test sentance.
 ";
            JournalTree journalTree = new JournalTree(JournalText);
            Assert.IsTrue(journalTree.Root.Children.Count == 3);
            Assert.AreEqual(journalTree.Root.Children[1].DateTime, new DateTime(2001, 2, 5));
            Assert.AreEqual(journalTree.Root.Children[2].DateTime, new DateTime(2001, 2, 6));

        }


        [TestMethod]
        public void RootShouldContainThreeEntriesMatchingText()
        {
            const string Entry1 = "February 4th 2001\r\nThis is a test sentance.\r\n\r\n";
            const string Entry2 = "February 5th 2001 - Page 6\r\n\r\nThis is another test sentance.\r\n\r\nPage 8\r\n";
            const string Entry3 = "February 6th 2001\r\n\r\nThis is a third test sentance.\r\n";
            const string JournalText = Entry1+ Entry2 + Entry3;
            JournalTree journalTree = new JournalTree(JournalText);
            Assert.IsTrue(journalTree.Root.Children.Count == 3);
            Assert.AreEqual(Entry1, journalTree.Root.Children[0].Range.Substring(JournalText));
            Assert.AreEqual(Entry2, journalTree.Root.Children[1].Range.Substring(JournalText));
            Assert.AreEqual(Entry3, journalTree.Root.Children[2].Range.Substring(JournalText));

        }

    }
}
