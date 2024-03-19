using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V120.FedCm;

namespace WebContrall_Selenium
{
    public partial class Form1 : Form
    {
        //chrome 브라우저와 소동 하게 해주는 ChromeDriver
        private IWebDriver driver;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void LoginState()
        {
            driver = new ChromeDriver();
            //자바스크립트 요소를 통해서 요소가 생성되기 전에도 클릭가능
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

            driver.Navigate().GoToUrl("https://m.elysian.co.kr/member/login.asp");

            // 아이디 입력 필드 찾기 ,아이디 입력
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_01').value = arguments[0];", "11037301");

            // 비밀번호 입력 필드 찾기 , 비밀번호 입력
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_02').value = arguments[0];", "9999");


            jsExecutor.ExecuteScript("fnLogin(1);");

            //// 아이디 저장 체크박스 찾기
            //IWebElement SaveIdCheckbox = driver.FindElement(By.Id("id_save"));
            //SaveIdCheckbox.Click();
            //
            //// 자동로그인 체크박스 찾기
            //IWebElement LoggedInAutoCheckbox = driver.FindElement(By.Id("login_auto"));
            //LoggedInAutoCheckbox.Click();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginState();
            driver.Navigate().GoToUrl("https://m.elysian.co.kr/reservation/golf_choice.asp");
        }
    }
}
