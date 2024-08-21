using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium;

namespace ExamPrep1
{
    public class ExamPrep1Tests
    {
        private readonly static string BaseUrl = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:83";
        private WebDriver driver;
        private Actions actions;
        private string? lastCreatedIdeaTitle;
        private string? lastCreatedIdeaDescription;

        [OneTimeSetUp]
        public void Setup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("profile.password_manager_enable", false);
            chromeOptions.AddArgument("--disable-search-engine-choice-screen");

            driver = new ChromeDriver(chromeOptions);
            actions = new Actions(driver);
            driver.Navigate().GoToUrl(BaseUrl);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl($"{BaseUrl}/Users/Login");

            driver.FindElement(By.XPath("//input[@name='Email']")).SendKeys("petqtest@test.bg");
            driver.FindElement(By.XPath("//input[@name='Password']")).SendKeys("123456");

            driver.FindElement(By.XPath("//button[text()='Sign in']")).Click();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Test, Order(1)]
        public void CreateIdeaWithInvalidDataTest()
        {

            driver.Navigate().GoToUrl($"{BaseUrl}/Ideas/Create");

            var titleInput = driver.FindElement(By.XPath("//input[@name='Title']"));
            titleInput.Clear();
            titleInput.SendKeys("");

            var descriptionInput = driver.FindElement(By.XPath("//textarea[@name='Description']"));
            descriptionInput.Clear();
            descriptionInput.SendKeys("");

            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}/Ideas/Create"), "User is redirect");

            var errorMessage = driver.FindElement(By.XPath("//div[@class='text-danger validation-summary-errors']//li"));
            Assert.That(errorMessage.Text, Is.EqualTo("Unable to create new Idea!"), "The error message for invalid data when creating Idea is not there");
        }

        [Test, Order(2)]
        public void CreateRandomIdeaTest()
        {

            lastCreatedIdeaTitle = GenerateRandomTitle();
            lastCreatedIdeaDescription = GenerateRandomDescription();
            driver.Navigate().GoToUrl($"{BaseUrl}/Ideas/Create");

            var titleInput = driver.FindElement(By.XPath("//input[@name='Title']"));
            titleInput.Clear();
            titleInput.SendKeys(lastCreatedIdeaTitle);

            var descriptionInput = driver.FindElement(By.XPath("//textarea[@name='Description']"));
            descriptionInput.Clear();
            descriptionInput.SendKeys(lastCreatedIdeaDescription);

            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}/Ideas/MyIdeas"), "User is not redirect");

            var ideas = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastRevueDescription = ideas.Last().FindElement(By.XPath(".//p")).Text;
            Assert.That(lastRevueDescription, Is.EqualTo(lastCreatedIdeaDescription), "The last Idea is not present on the screen.");
        }

        [Test, Order(3)]
        public void ViewLastCreatedIdeaTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Ideas/MyIdeas");

            var formIdeas = driver.FindElement(By.XPath("//div[@class='album py-5 bg-light']"));
            actions.ScrollToElement(formIdeas).Perform();

            var ideas = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastIdeaViewButton = ideas.Last().FindElement(By.XPath(".//a[text()='View']"));
            lastIdeaViewButton.Click();

            var lastIdeaTitle = driver.FindElement(By.XPath("//h1")).Text;
            Assert.That(lastIdeaTitle, Is.EqualTo(lastCreatedIdeaTitle), "The title is not as expected.");
        }

        [Test, Order(4)]
        public void EditLastCreatedIdeaTitleTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Ideas/MyIdeas");

            var formIdeas = driver.FindElement(By.XPath("//div[@class='album py-5 bg-light']"));
            actions.ScrollToElement(formIdeas).Perform();

            var ideas = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastIdeaEditButton = ideas.Last().FindElement(By.XPath(".//a[text()='Edit']"));
            lastIdeaEditButton.Click();

            lastCreatedIdeaTitle = "Edited" + lastCreatedIdeaTitle;

            var titleInput = driver.FindElement(By.XPath("//input[@name='Title']"));
            titleInput.Clear();
            titleInput.SendKeys(lastCreatedIdeaTitle);

            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            formIdeas = driver.FindElement(By.XPath("//div[@class='album py-5 bg-light']"));
            actions.ScrollToElement(formIdeas).Perform();

            ideas = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastIdeaViewButton = ideas.Last().FindElement(By.XPath(".//a[text()='View']"));
            lastIdeaViewButton.Click();
            var lastIdeaTitle = driver.FindElement(By.XPath("//h1")).Text;
            Assert.That(lastIdeaTitle, Is.EqualTo(lastCreatedIdeaTitle), "The title is not edited.");
        }

        [Test, Order(5)]
        public void EditLastCreatedIdeaDescriptionTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Ideas/MyIdeas");

            var formIdeas = driver.FindElement(By.XPath("//div[@class='album py-5 bg-light']"));
            actions.ScrollToElement(formIdeas).Perform();

            var ideas = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastIdeaEditButton = ideas.Last().FindElement(By.XPath(".//a[text()='Edit']"));
            lastIdeaEditButton.Click();

            lastCreatedIdeaDescription = "Edited" + lastCreatedIdeaDescription;

            var descriptionInput = driver.FindElement(By.XPath("//textarea[@name='Description']"));
            descriptionInput.Clear();
            descriptionInput.SendKeys(lastCreatedIdeaDescription);

            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            formIdeas = driver.FindElement(By.XPath("//div[@class='album py-5 bg-light']"));
            actions.ScrollToElement(formIdeas).Perform();

            ideas = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastRevueDescription = ideas.Last().FindElement(By.XPath(".//p")).Text;
            Assert.That(lastRevueDescription, Is.EqualTo(lastCreatedIdeaDescription), "The last description is not edited.");
        }

        [Test, Order(6)]
        public void DeleteLastCreatedIdeaTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Ideas/MyIdeas");

            var formIdeas = driver.FindElement(By.XPath("//div[@class='album py-5 bg-light']"));
            actions.ScrollToElement(formIdeas).Perform();

            var ideas = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastIdeaDeleteButton = ideas.Last().FindElement(By.XPath(".//a[text()='Delete']"));
            lastIdeaDeleteButton.Click();

            formIdeas = driver.FindElement(By.XPath("//div[@class='album py-5 bg-light']"));
            actions.ScrollToElement(formIdeas).Perform();

            ideas = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastRevueDescription = ideas.Last().FindElement(By.XPath(".//p")).Text;
            Assert.That(lastRevueDescription, Is.Not.EqualTo(lastCreatedIdeaDescription), "The last description is not edited.");
        }

        public string GenerateRandomTitle()
        {
            var random = new Random();
            return "TITLE: " + random.Next(1000, 10000);
        }

        public string GenerateRandomDescription()
        {
            var random = new Random();
            return "DESCRIPTION: " + random.Next(1000, 10000);
        }
    }
}