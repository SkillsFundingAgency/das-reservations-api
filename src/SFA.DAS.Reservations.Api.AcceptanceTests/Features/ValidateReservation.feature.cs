// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.0.0.0
//      SpecFlow Generator Version:3.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SFA.DAS.Reservations.Api.AcceptanceTests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("ValidateReservation")]
    public partial class ValidateReservationFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "ValidateReservation.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "ValidateReservation", "\tIn order to determine if a reservation can be used in a commitment\r\n\tAs a employ" +
                    "er or provider\r\n\tI would like to validate date and course against an existing re" +
                    "servation", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Validate existing reservation inside reservation window")]
        public virtual void ValidateExistingReservationInsideReservationWindow()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Validate existing reservation inside reservation window", null, ((string[])(null)));
#line 6
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 7
testRunner.Given("I have a non levy account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "StartDate",
                        "CourseId"});
            table1.AddRow(new string[] {
                        "2019-07-01",
                        "1"});
#line 8
testRunner.And("I have the following existing reservation:", ((string)(null)), table1, "And ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "StartDate",
                        "CourseId"});
            table2.AddRow(new string[] {
                        "2019-07-01",
                        "1"});
#line 11
testRunner.When("I validate the reservation against the following commitment data:", ((string)(null)), table2, "When ");
#line 14
testRunner.Then("no validation errors are returned", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Validate existing reservation before the reservation window")]
        public virtual void ValidateExistingReservationBeforeTheReservationWindow()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Validate existing reservation before the reservation window", null, ((string[])(null)));
#line 16
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 17
testRunner.Given("I have a non levy account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "StartDate",
                        "ExpiryDate",
                        "CourseId"});
            table3.AddRow(new string[] {
                        "2019-07-01",
                        "2019-09-30",
                        "1"});
#line 18
testRunner.And("I have the following existing reservation:", ((string)(null)), table3, "And ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "StartDate",
                        "CourseId"});
            table4.AddRow(new string[] {
                        "2019-06-01",
                        "1"});
#line 21
testRunner.When("I validate the reservation against the following commitment data:", ((string)(null)), table4, "When ");
#line 24
testRunner.Then("validation errors are returned", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Validate existing reservation after the reservation window")]
        public virtual void ValidateExistingReservationAfterTheReservationWindow()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Validate existing reservation after the reservation window", null, ((string[])(null)));
#line 26
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 27
testRunner.Given("I have a non levy account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "StartDate",
                        "ExpiryDate",
                        "CourseId"});
            table5.AddRow(new string[] {
                        "2019-07-01",
                        "2019-09-30",
                        "1"});
#line 28
testRunner.And("I have the following existing reservation:", ((string)(null)), table5, "And ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "StartDate",
                        "CourseId"});
            table6.AddRow(new string[] {
                        "2019-10-01",
                        "1"});
#line 31
testRunner.When("I validate the reservation against the following commitment data:", ((string)(null)), table6, "When ");
#line 34
testRunner.Then("validation errors are returned", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Validate existing reservation inside of reservation window with invalid course")]
        public virtual void ValidateExistingReservationInsideOfReservationWindowWithInvalidCourse()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Validate existing reservation inside of reservation window with invalid course", null, ((string[])(null)));
#line 36
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 37
testRunner.Given("I have a non levy account", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "CreatedDate",
                        "ActiveFrom",
                        "ActiveTo",
                        "CourseId"});
            table7.AddRow(new string[] {
                        "2019-07-01",
                        "2019-07-01",
                        "2019-09-30",
                        "1"});
#line 38
testRunner.And("the following rule exists:", ((string)(null)), table7, "And ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "StartDate",
                        "ExpiryDate",
                        "CourseId"});
            table8.AddRow(new string[] {
                        "2019-07-01",
                        "2019-09-30",
                        "1"});
#line 41
testRunner.And("I have the following existing reservation:", ((string)(null)), table8, "And ");
#line hidden
            TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                        "StartDate",
                        "CourseId"});
            table9.AddRow(new string[] {
                        "2019-07-01",
                        "1"});
#line 44
testRunner.When("I validate the reservation against the following commitment data:", ((string)(null)), table9, "When ");
#line 47
testRunner.Then("validation errors are returned", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion