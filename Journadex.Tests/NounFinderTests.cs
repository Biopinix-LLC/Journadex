using System;
using Journadex.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Journadex.Tests
{
    [TestClass]
    public class NounFinderTests
    {
        //    [TestMethod]
        //[DeploymentItem("stanford-corenlp-4.5.1-models.jar")]
        //    public void ShouldIdentifyDogInText_usingCoreNLP()
        //    {
        //        var nouns = new NounFinder(new StanfordCoreNLPStrategy()).FindNouns("I am not a dog that likes to bark.");
        //        Assert.IsTrue(nouns.TryGetValue("dog", out int index) && index == 10);
        //    }

        //[TestMethod]
        //public void ShouldIdentifyDogInText_UsingRegex()
        //{
        //    const string Input = "I am not a dog that likes to bark.";
        //    var nouns = new NounFinder(new RegexStrategy()).FindNouns(Input);
        //    Assert.IsTrue(nouns.TryGetValue("dog", out int index) && index == Input.IndexOf("dog"));
        //}

        //[TestMethod]
        //public void ShouldIdentifyBillWilliamsInText_UsingRegex()
        //{
        //    const string Input = "Bill Williams looks to find what he lost.";
        //    var nouns = new NounFinder(new RegexStrategy()).FindNouns(Input);
        //    Assert.IsTrue(nouns.TryGetValue("Bill", out int index) && index == Input.IndexOf("Bill"));
        //    Assert.IsTrue(nouns.TryGetValue("Williams", out int index2) && index2 == Input.IndexOf("Williams"));
        //}

        [TestMethod]
        public void ShouldIdentifyBillWilliamsInText_UsingSplit()
        {
            const string Input = "Bill Williams looks to find what he lost.";
            var nouns = new NounFinder(new SplitProperCaseStrategy()).FindNouns(Input);
            Assert.IsTrue(nouns.TryGetValue("Bill", out int index) && index == Input.IndexOf("Bill"));
            Assert.IsTrue(nouns.TryGetValue("Williams", out int index2) && index2 == Input.IndexOf("Williams"));
        }
    }
}
